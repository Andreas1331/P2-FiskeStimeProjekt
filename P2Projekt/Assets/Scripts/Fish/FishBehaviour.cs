﻿using Mathtools;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class FishBehaviour : MonoBehaviour
{
    private Fish _fish;
    public Fish Fish { get {return _fish; } set { if (value != null) _fish = value; } }
    private DataManager _dataManager;
    public DataManager DataManager { set { if (value != null) _dataManager = value; } }
    private GameObject _net;
    public GameObject Net { set { if (value != null) _net = value; } }
    private MathTools _mathTools = new MathTools();
    private Material _mat;

    private const float _stressMultiplier = 0.5f;

    public List<Vector3> lastKnownFoodSpots = new List<Vector3>() { new Vector3(50,20,10), new Vector3(10,10,10) };
    public Dictionary<int, Vector3> knownFoodSpots = new Dictionary<int, Vector3>();
    public Dictionary<int, Vector3> inInnerCollider = new Dictionary<int, Vector3>();
    public Dictionary<int, FishBehaviour> nearbyFish = new Dictionary<int, FishBehaviour>();

    private float[] lambdaArrayAlone = new float[5] { 0.1f, 1, 1, 0.21f, 1 };
    private float[] lambdaArrayStime = new float[6] { 0.1f, 1, 1, 0.21f, 0.21f, 0.21f };
    private Vector3[] D_tVectors = new Vector3[6];

    //Stress variables
    private float timerToDie = 0;
    private float timerToResetTimer = 0;
     
    private Color _defaultColor = new Color(191/255f, 249/255f, 249/255f, 255/255f);

    private void Awake()
    {
        DataManager = FindObjectOfType<DataManager>();

        Net = GameObject.FindGameObjectWithTag("Net");

        GetComponent<SphereCollider>().radius = 5f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_fish.CurrentDirection != null)
        {
            Vector3 newdir = Vector3.RotateTowards(transform.forward, _fish.CurrentDirection, Time.deltaTime * 5, 2.5f);
            transform.rotation = Quaternion.LookRotation(newdir);
        }
        ////AnimateDeath();
        _fish.MoveTowards(GetNewDirection());

        UpdateStress();
        UpdateHunger();
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleSpottedObject(other);
    }

    private void OnTriggerStay(Collider other)
    {
        //Dette kan genimplementeres hvis at maden skal bevæge sig
        //if (other.tag.Equals("Food"))
        //{
        //    if (knownFoodSpots.ContainsKey(other.GetComponent<FoodBehavior>().Food.Id)) {
        //        knownFoodSpots[other.GetComponent<FoodBehavior>().Food.Id] = other.transform.position;
        //        Debug.Log(knownFoodSpots[other.GetComponent<FoodBehavior>().Food.Id]);
        //    }
        //}
        //Debug.Log("Object is nearby .. ");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Food"))
        {
            int othersId = other.GetComponent<FoodBehavior>().Food.Id;
            if (knownFoodSpots.ContainsKey(othersId))
                knownFoodSpots.Remove(othersId);
            if (inInnerCollider.ContainsKey(othersId))
            {
                inInnerCollider.Remove(othersId);
                knownFoodSpots.Add(othersId, other.transform.position);
            }
        }
        else if (other.tag.Equals("Fish"))
        {
            int othersId = other.GetComponent<FishBehaviour>().Fish.Id;
            if (nearbyFish.ContainsKey(othersId))
            {
                nearbyFish.Remove(othersId);
            }
        }
    }

    private void HandleSpottedObject(Collider other)
    {
        // Check if the object detected is another fish, or an obstacle.
        if (other.tag.Equals("Fish"))
        {
            FishBehaviour fishBehav = other.GetComponent<FishBehaviour>();
            if (fishBehav == null)
                return;

            if (!nearbyFish.ContainsKey(fishBehav.Fish.Id))
            {
                nearbyFish.Add(fishBehav.Fish.Id, fishBehav);
            }
        }
        else if (other.tag.Equals("Obstacle"))
        {
            Vector3 closestPos = other.ClosestPoint(transform.position);
            Debug.DrawRay(transform.position, closestPos - transform.position, Color.blue, 15);

            float angle = _mathTools.GetAngleBetweenVectors(_fish.CurrentDirection, closestPos);
            float dist = _mathTools.GetDistanceBetweenVectors(_fish.CurrentDirection, closestPos);
            float catheter = _mathTools.GetOpposingCatheter(angle, dist);

            if (catheter <= _fish.Width / 2)
            {
                int offset = 1;
                Vector3 newDir = FindFreeDir(closestPos, ref offset);
                D_tVectors[3] = newDir;
                // Use the new direction.. (D4t)
            }
        }
        else if (other.tag.Equals("Food"))
        {
            FoodBehavior othersFoodBehavior = other.GetComponent<FoodBehavior>();
            if (!knownFoodSpots.ContainsKey(othersFoodBehavior.Food.Id))
            {
                knownFoodSpots.Add(othersFoodBehavior.Food.Id, other.transform.position);
                lastKnownFoodSpots.Add(other.transform.position);
            }
            else
            {
                _fish.Hunger = 1000;
                othersFoodBehavior.BeingEaten();
                Debug.Log("Fisken spiste");
                //grimt workaround
                knownFoodSpots.Remove(othersFoodBehavior.Food.Id);
            }
        }
    }

    private Vector3 FindFreeDir(Vector3 pos, ref int offset)
    {
        Vector3 posOne = new Vector3(pos.x + offset, pos.y, pos.z) - transform.position;
        Vector3 posTwo = new Vector3(pos.x - offset, pos.y, pos.z) - transform.position;

        Debug.DrawRay(transform.position, posOne, Color.yellow, 10);
        Debug.DrawRay(transform.position, posTwo, Color.red, 10);

        RaycastHit hit;
        if (!Physics.Raycast(transform.position, posOne, out hit, 10, LayerMask.GetMask("Obstacle")))
            return posOne;
        else if (!Physics.Raycast(transform.position, posTwo, out hit, 10, LayerMask.GetMask("Obstacle")))
            return posTwo;

        offset++;
        return FindFreeDir(pos, ref offset);
    }

    #region Lambda
    private void UpdateHunger()
    {
        _fish.Hunger -= 1 * Time.deltaTime;
        if(_fish.Hunger <= 0)
        {
            KillFish();
        }
    }

    private void UpdateStress()
    {
        // Increase or lower the stress based on the fish hunger.
        if (_fish.Hunger <= 0.5 * Fish.maxHunger && _fish.Hunger > 0.3 * Fish.maxHunger)
            _fish.Stress += 1 * _stressMultiplier * Time.deltaTime;
        else if (_fish.Hunger <= 0.3 * Fish.maxHunger)
            _fish.Stress += 1 * (_stressMultiplier * 2) * Time.deltaTime;
        else if (_fish.Stress > 0)
            _fish.Stress -= 1 * Time.deltaTime;

        // Start the timer if the fish is stressed.
        if (_fish.Stress >= 0.9 * Fish.maxStress)
        {
                SetColor(Color.red);

                timerToDie += Time.deltaTime;
            timerToResetTimer = 0;
            if (timerToDie > 30)
                KillFish();
        }
        else if (_fish.Stress < 0.9 * Fish.maxStress && timerToDie != 0) {
                    SetColor(_defaultColor);


                    timerToResetTimer += Time.deltaTime;
            if (timerToResetTimer > 30)
                timerToDie = 0;
        }
    }

    private void SetColor(Color col)
    {
        if (_mat == null && _fish.FishObject != null)
            _mat = _fish.FishObject.GetComponent<Renderer>().material;

        if (_mat.color != col)
            _mat.color = col;
    }
    #endregion

    #region Die Methods
    private void KillFish()
    {
        _fish.IsDead = true;
        _dataManager.RemoveFish(_fish);
    }

    public void AnimateDeath()
    {
        if (!_fish.IsDead)
            return;
        //Rotation of fish around the z-axis
        if (transform.rotation.x > -0.7f)
        {
            Vector3 newdir = Vector3.RotateTowards(transform.forward, new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime, 2.5f);
            transform.rotation = Quaternion.LookRotation(newdir);
            //Debug.Log(transform.rotation.x);
            //transform.RotateAround(transform.position, Vector3.forward, 10 * Time.deltaTime);
        }
        else if (transform.rotation.x <= -0.7f && transform.position.y < 10)
        {
            //transform.RotateAround(transform.position, Vector3.forward, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y + 5 * Time.deltaTime, transform.position.z);
            //Debug.Log("1");

            //MakeOpague;
        }
        else {
            transform.position = new Vector3(-5000.0f,-5000.0f, -5000.0f);
            this.transform.gameObject.SetActive(false);
        }
    }

    void MakeOpague()
    {
    }

    #endregion

    #region Food Methods
    public Vector3 canSeeFood()
    {
        Vector3 closestFood = new Vector3(100,100,100);
        //Iterate through list of food nearby, and choose the closest one. 
        foreach (KeyValuePair<int, Vector3> item in knownFoodSpots) {
            if (Mathf.Sqrt(Mathf.Pow(item.Value.x - this.transform.position.x, 2) + Mathf.Pow(item.Value.y - this.transform.position.y, 2) + Mathf.Pow(item.Value.z - this.transform.position.z, 2))
                <Mathf.Sqrt(Mathf.Pow(closestFood.x, 2) + Mathf.Pow(closestFood.y, 2) + Mathf.Pow(closestFood.z, 2))) {
                closestFood = item.Value;
            }
        }
        return closestFood;
    }

    public Vector3 cantSeeFood()
    {
        Vector3 sumVecD3 = new Vector3();
        float factor;
        if(lastKnownFoodSpots.Count == 0)
        {
            return new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0,10));
        }

        foreach (Vector3 vec in lastKnownFoodSpots)
        {
            factor = (1 / (_mathTools.GetDistanceBetweenVectors(vec, this.transform.position)));
                
            sumVecD3 += factor * (vec - this.transform.position);
        }
        return sumVecD3;
    }
    #endregion

    #region Swim towards other fish
    private Vector3 SwimTowardsOtherFish() {
        Vector3 D_3 = new Vector3(0,0,0);
        float distanceBetweenFish;
        //Debug.Log(nearbyFish.Count);
        foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish) {
            distanceBetweenFish = _mathTools.GetDistanceBetweenVectors(transform.position,item.Value.transform.position)/nearbyFish.Count;
            D_3.x += distanceBetweenFish *(item.Value.transform.position.x-transform.position.x);
            D_3.y += distanceBetweenFish *(item.Value.transform.position.y-transform.position.y);
            D_3.z += distanceBetweenFish *(item.Value.transform.position.z-transform.position.z);
        }
        return D_3;
    }
    #endregion

    #region Swim with fish
    private Vector3 SwimWithFriends() {
        Vector3 D_3 = new Vector3(0,0,0);
        foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish) {
            D_3.x += item.Value.Fish.CurrentDirection.x / nearbyFish.Count;
            D_3.y += item.Value.Fish.CurrentDirection.y / nearbyFish.Count;
            D_3.z += item.Value.Fish.CurrentDirection.z / nearbyFish.Count;
            //Debug.Log("D_3 er følgende " + D_3);
        }
        return D_3;
    }
    #endregion

    #region Hold distance to fish
    private Vector3 HoldDistanceToFish()
    {
        Vector3 GV = new Vector3(0, 0, 0);
        Vector3 GN = new Vector3(0, 0, 0);
        foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish)
        {
            if (_mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position) < 0.2f)
            {
                GV.x += (-1) * ((1 / Mathf.Sin(8 * _mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position))) - 1) *
                    (item.Value.transform.position.x - transform.position.x) / nearbyFish.Count;
                GV.y += (-1) * ((1 / Mathf.Sin(8 * _mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position))) - 1) *
                    (item.Value.transform.position.y - transform.position.y) / nearbyFish.Count;
                GV.z += (-1) * ((1 / Mathf.Sin(8 * _mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position))) - 1) *
                    (item.Value.transform.position.z - transform.position.z) / nearbyFish.Count;
            }
            else if (_mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position) < 0.35f) {
                GN.x += ((1 / Mathf.Sin(8 * _mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position))) - 1) *
                    (item.Value.transform.position.x - transform.position.x) / nearbyFish.Count;

                GN.y += ((1 / Mathf.Sin(8 * _mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position))) - 1) *
                                    (item.Value.transform.position.y - transform.position.y) / nearbyFish.Count;
                GN.z += ((1 / Mathf.Sin(8 * _mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position))) - 1) *
                                    (item.Value.transform.position.z - transform.position.z) / nearbyFish.Count;
            }
        }
        return GN+GV;
    }
    #endregion

    #region Search for optimal depth
    private Vector3 SearchForOptimalDepth() {
        var vec = new Vector3(transform.position.x, -_net.transform.lossyScale.y/2- transform.position.y, transform.position.z);
        
        return vec;
    }
    #endregion

    #region Get new direction
    private Vector3 GetNewDirection()
    {
        if (_fish.IsDead)
            return new Vector3(0, 0, 0);
        bool schooling = false;
        bool isThereNearbyFood = false;
        D_tVectors[0] = _fish.CurrentDirection;
        D_tVectors[4] = SearchForOptimalDepth();
        foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish) {
            if (_mathTools.GetDistanceBetweenVectors(transform.position, item.Value.transform.position) < 0.35f)
            {
                schooling = true;
                break;
            }
        }
        if (knownFoodSpots.Count < 0) {
            isThereNearbyFood = true;
        }
        if (schooling)
        {
            //Debug.Log("schooling sker");
            D_tVectors[2] = SwimWithFriends();
            D_tVectors[5] = HoldDistanceToFish();
            if (isThereNearbyFood) {
                //Debug.Log("Der er mad");
                D_tVectors[1] = canSeeFood();
                _fish.CurrentDirection = D_tVectors[0] * lambdaArrayStime[0] + D_tVectors[1] * lambdaArrayStime[1]+D_tVectors[2]*lambdaArrayStime[2]
                    + D_tVectors[3] * lambdaArrayStime[3] + D_tVectors[4] * lambdaArrayStime[4] + D_tVectors[5] * lambdaArrayStime[5];
            }
            else {
                D_tVectors[1] = cantSeeFood();
                _fish.CurrentDirection = D_tVectors[0] * lambdaArrayStime[0] + D_tVectors[1] * lambdaArrayStime[1] + D_tVectors[2] * lambdaArrayStime[2]
                    + D_tVectors[3] * lambdaArrayStime[3] + D_tVectors[4] * lambdaArrayStime[4] + D_tVectors[5] * lambdaArrayStime[5];
            }
        }
        else {
            D_tVectors[2] = SwimTowardsOtherFish();
            //Debug.Log("D_tVectors[2] = "+D_tVectors[2]);
            if (isThereNearbyFood)
            {
                D_tVectors[1] = canSeeFood();
                _fish.CurrentDirection = D_tVectors[0] * lambdaArrayAlone[0] + D_tVectors[1] * lambdaArrayAlone[1]
                    + D_tVectors[2] * lambdaArrayAlone[2] + D_tVectors[3] * lambdaArrayAlone[3] + D_tVectors[4] * lambdaArrayAlone[4];
            }
            else 
            {
                D_tVectors[1] = cantSeeFood();
                _fish.CurrentDirection = D_tVectors[0] * lambdaArrayAlone[0] + D_tVectors[1] * lambdaArrayAlone[1] 
                    + D_tVectors[2] * lambdaArrayAlone[2] + D_tVectors[3] * lambdaArrayAlone[3] + D_tVectors[4] * lambdaArrayAlone[4];
            }
        }
        D_tVectors[3] = new Vector3(0,0,0);
        schooling = false;
        //Debug.Log("Currect direction = "+ Vector3.Normalize(_fish.CurrentDirection));
        _fish.CurrentDirection = Vector3.Normalize(_fish.CurrentDirection);
        return _fish.CurrentDirection;
        //return _fish.CurrentDirection;
    }
    #endregion
}