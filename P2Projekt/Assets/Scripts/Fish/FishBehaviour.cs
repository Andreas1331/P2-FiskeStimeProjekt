using Mathtools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class FishBehaviour : MonoBehaviour
{
    private Fish _fish;
    public Fish Fish { get { return _fish; } set { if (value != null) _fish = value; } }
    private DataManager _dataManager;
    public DataManager DataManager { set { if (value != null) _dataManager = value; } }
    private GameObject _net;
    public GameObject Net { set { if (value != null) _net = value; } }
    public MathTools MathTools { set { if (value != null) _mathTools = value; } }
    private bool _isObstacleDetected = false;
    public float gotDistance = 0;
    private const float _stressMultiplier = 4.5f; //tidligere 1,5
    private List<Vector3> lastKnownFoodSpots = new List<Vector3>() { new Vector3(5, 5, 5) }/*{ new Vector3(3, 3, 3), new Vector3(5, 10, 3), new Vector3(-10, -2, 4) }*/;
    private Dictionary<int, Vector3> knownFoodSpots = new Dictionary<int, Vector3>();
    public Dictionary<int, Vector3> inInnerCollider = new Dictionary<int, Vector3>();
    public Dictionary<int, FishBehaviour> nearbyFish = new Dictionary<int, FishBehaviour>();
    public Dictionary<int, FishBehaviour> innerColliderFish = new Dictionary<int, FishBehaviour>();

    private List<FishBehaviour> _nearbyFish = new List<FishBehaviour>();
    private float _schoolDst = 0.25f;

    private SphereCollider _outerCollider;
    private SphereCollider _innerCollider;

    //Stress timer
    private float timerToDie = 0;
    private float timerToResetTimer = 0;

    #region Lambda structs
    private lambdaStructAlone lambdaAlone = new lambdaStructAlone();
    private lambdaStructSchool lambdaSchool = new lambdaStructSchool();
    private stressFactorLambdaAlone stressFactorsAlone = new stressFactorLambdaAlone();
    private stressFactorLambdaSchool stressFactorsSchool = new stressFactorLambdaSchool();
    private hungerFactorLambdaAlone hungerFactorsAlone = new hungerFactorLambdaAlone();
    private hungerFactorLambdaSchool hungerFactorsSchool = new hungerFactorLambdaSchool();
    private depthFactorLambdaAlone depthFactorsAlone = new depthFactorLambdaAlone();
    private depthFactorLambdaSchool depthFactorsSchool = new depthFactorLambdaSchool();
    private directionVectors directions = new directionVectors();
    #endregion

    private MathTools _mathTools = new MathTools();

    private Material _mat;
    private Color _defaultColor = new Color(191 / 255f, 249 / 255f, 249 / 255f, 255 / 255f);

    Text txt2;
    Text txt;

    private void Start()
    {
        DataManager = FindObjectOfType<DataManager>();

        Net = GameObject.FindGameObjectWithTag("Net");

        GetComponent<SphereCollider>().radius = 5f;

        txt = GameObject.Find("FishDirectionTxt").GetComponent<Text>();
        txt2 = GameObject.Find("FishNormalizedTxt").GetComponent<Text>();

        _outerCollider = GetComponents<SphereCollider>()[0];
        _innerCollider = GetComponents<SphereCollider>()[1];
    }

    // Update is called once per frame
    private void Update()
    {
        _fish.MoveTowards(GetNewDirection());
        UpdateStress();
        UpdateHunger();
        AnimateDeath();

        //Debug.DrawRay(transform.position, ((_fish.CurrentDirection - transform.position).normalized / 2), Color.green);
        //txt.text = "Direction: " + (_fish.CurrentDirection - transform.position);
        //foreach (KeyValuePair<int, Vector3> item in knownFoodSpots) {
        //    Debug.Log("Madpunkt" + knownFoodSpots[item.Key]);
        //}
        //txt2.text = "Madcount" + knownFoodSpots.Count + " | position af maden: " + knownFoodSpots[0];
        //randomPoint = _dataManager.randomPoint;
        //_fish.MoveTowards(_fish.CurrentDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleSpottedObject(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Food"))
        {
            int othersId = other.GetComponent<FoodBehavior>().Food.Id;
            if (knownFoodSpots.ContainsKey(othersId) && _mathTools.GetDistanceBetweenVectors(other.transform.position, transform.position) < 4.5f)
            {
                knownFoodSpots.Remove(othersId);
            }
            if (inInnerCollider.ContainsKey(othersId))
            {
                inInnerCollider.Remove(othersId);
                knownFoodSpots.Add(othersId, other.transform.position);
            }
        }
        else if (other.tag.Equals("Fish"))
        {
            if (_mathTools.GetDistanceBetweenVectors(transform.position, other.gameObject.transform.position) >= _innerCollider.radius)
                return;

            FishBehaviour fishBehav = other.GetComponent<FishBehaviour>();
            if (fishBehav == null)
                return;

            if (_nearbyFish.Contains(fishBehav))
                _nearbyFish.Remove(fishBehav);

            int othersId = other.GetComponent<FishBehaviour>().Fish.Id;
            if (nearbyFish.ContainsKey(othersId))
            {
                if (_mathTools.GetDistanceBetweenVectors(other.gameObject.transform.position, transform.position) > 9)
                    nearbyFish.Remove(othersId);
            }
            //if (innerColliderFish.ContainsKey(othersId)) {
            //    innerColliderFish.Remove(othersId);
            //    nearbyFish.Add(othersId, other.GetComponent<FishBehaviour>());
            //}
        }
    }

    private void HandleSpottedObject(Collider other)
    {
        // Check if the object detected is another fish, or an obstacle.
        if (other.tag.Equals("Fish"))
        {
            if (_mathTools.GetDistanceBetweenVectors(transform.position, other.gameObject.transform.position) <= _innerCollider.radius)
                return;

            FishBehaviour fishBehav = other.GetComponent<FishBehaviour>();
            if (fishBehav == null)
                return;

            if (!_nearbyFish.Contains(fishBehav))
                _nearbyFish.Add(fishBehav);

            if (!nearbyFish.ContainsKey(fishBehav.Fish.Id))
            {
                nearbyFish.Add(fishBehav.Fish.Id, fishBehav);
            }

            //else {
            //    nearbyFish.Remove(fishBehav.Fish.Id);
            //    innerColliderFish.Add(fishBehav.Fish.Id, fishBehav);
            //}
        }
        else if (other.tag.Equals("Obstacle") || (other.tag.Equals("Net")))
        {
            Vector3 closestPos = other.ClosestPoint(transform.position);
            if (_mathTools.GetDistanceBetweenVectors(transform.position, closestPos) < 1)
            {
                if (_isObstacleDetected = IsHeadingTowardsPoint(closestPos))
                {
                    int offset = 1;
                    directions.dodgeCollisionDirection = FindFreeDir(closestPos, ref offset);
                }
            }
        }
        else if (other.tag.Equals("Food"))
        {
            FoodBehavior othersFoodBehavior = other.GetComponent<FoodBehavior>();
            if (!knownFoodSpots.ContainsKey(othersFoodBehavior.Food.Id))
            {
                knownFoodSpots.Add(othersFoodBehavior.Food.Id, other.transform.position);
                lastKnownFoodSpots.Add(other.transform.position);
                Debug.Log("Jeg har tilføjet maden til dictionary og listen");
            }
            else if (_mathTools.GetDistanceBetweenVectors(other.transform.position, transform.position) < 1.5f)
            {
                _fish.Hunger = Fish.maxHunger;
                Debug.Log("Jeg spiste maden");
                othersFoodBehavior.BeingEaten();
                //grimt workaround
                knownFoodSpots.Remove(othersFoodBehavior.Food.Id);
                Debug.Log("Jeg har fjernet maden");
                //inInnerCollider.Add(othersFoodBehavior.Food.Id, other.transform.position);
            }
        }
    }

    private bool IsHeadingTowardsPoint(Vector3 pos)
    {
        float angle = _mathTools.GetAngleBetweenVectors((_fish.CurrentDirection - _fish.FishObject.transform.position), (pos - transform.position));

        Vector3 posOne = _fish.CurrentDirection - transform.position;
        Vector3 posTwo = pos - transform.position;
        Debug.DrawRay(transform.position, posOne.normalized, Color.red, 30);

        float dist = _mathTools.GetDistanceBetweenVectors(transform.position, pos);
        float catheter = _mathTools.GetOpposingCatheter(angle, dist);

        return catheter <= _fish.Width / 2f;
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


    private void UpdateHunger()
    {
        // _fish.Hunger -= 3 * Time.deltaTime;
        if (_fish.Hunger <= 0)
        {
            KillFish();
        }
        else if (_fish.Hunger < 0.2f * Fish.maxHunger)
            _fish.MovementSpeed = _fish.Hunger / 200f;
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

    #region Lambda
    private void calculateLambdaAlone() {
        //CS = constant value
        float CS = 1.0f / 5.0f;
        lambdaAlone.prevDirectionLambda = CS * (stressFactorsAlone.prevDirectionStress + hungerFactorsAlone.prevDirectionHunger + depthFactorsAlone.prevDirectionDepth);
        lambdaAlone.findFoodLambda = CS * (stressFactorsAlone.findFoodStress + hungerFactorsAlone.findFoodHunger + depthFactorsAlone.findFoodDepth);
        lambdaAlone.findOtherFishLambda = CS * (stressFactorsAlone.findFishStress + hungerFactorsAlone.findFishHunger + depthFactorsAlone.findFishDepth);
        lambdaAlone.collisionDodgeLambda = CS * (stressFactorsAlone.collisionDodgeStress + hungerFactorsAlone.collisionDodgeHunger + depthFactorsAlone.collisionDodgeDepth);
        lambdaAlone.optimalDepthLambda = CS * (stressFactorsAlone.optimalDepthStress + hungerFactorsAlone.optimalDepthHunger + depthFactorsAlone.optimalDepthDepth);
    }
    private void calculateLambdaSchool() {
        //CS = constant value
        float CS = 1.0f / 6.0f;
        lambdaSchool.prevDirectionLambda = CS * (stressFactorsSchool.prevDirectionStress + hungerFactorsSchool.prevDirectionHunger + depthFactorsSchool.prevDirectionDepth);
        lambdaSchool.findFoodLambda = CS * (stressFactorsSchool.findFoodStress + hungerFactorsSchool.findFoodHunger + depthFactorsSchool.findFoodDepth);
        lambdaSchool.swimWithOtherFishLambda = CS * (stressFactorsSchool.swimWithOtherFishStress + hungerFactorsSchool.swimWithOtherFishHunger + depthFactorsSchool.swimWithOtherFishDepth);
        lambdaSchool.collisionDodgeLambda = CS * (stressFactorsSchool.collisionDodgeStress + hungerFactorsSchool.collisionDodgeHunger + depthFactorsSchool.collisionDodgeDepth);
        lambdaSchool.optimalDepthLambda = CS * (stressFactorsSchool.optimalDepthStress + hungerFactorsSchool.optimalDepthHunger + depthFactorsSchool.optimalDepthDepth);
        lambdaSchool.holdDistanceToFishLambda = CS * (stressFactorsSchool.holdDistanceToFishStress + hungerFactorsSchool.holdDistanceToFishHunger + depthFactorsSchool.holdDistanceToFishDepth);
    }

    #region Hunger factors
    private void calculateHungerFactorsAlone() {
        hungerFactorsAlone.findFoodHunger = 20 / (_fish.Hunger / Fish.maxHunger * 100);

        if (hungerFactorsAlone.findFoodHunger > 2)
            hungerFactorsAlone.findFoodHunger = 2;
        float leftOfHungerFactor = 2 - hungerFactorsAlone.findFoodHunger;
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            hungerFactorsAlone.prevDirectionHunger = leftOfHungerFactor * 0.5f;
            hungerFactorsAlone.findFishHunger = leftOfHungerFactor * 0.4f;
            hungerFactorsAlone.collisionDodgeHunger = 0;
            hungerFactorsAlone.optimalDepthHunger = leftOfHungerFactor * 0.1f;
        }
        else
        {
            hungerFactorsAlone.findFoodHunger = 0;
            hungerFactorsAlone.prevDirectionHunger = 0;
            hungerFactorsAlone.findFishHunger = 0;
            hungerFactorsAlone.collisionDodgeHunger = 2;
            hungerFactorsAlone.optimalDepthHunger = 0;
        }
    }
    private void calculateHungerFactorsSchool()
    {
        hungerFactorsSchool.findFoodHunger = 25 / (_fish.Hunger / Fish.maxHunger * 100);

        if (hungerFactorsSchool.findFoodHunger > 2.5f)
            hungerFactorsSchool.findFoodHunger = 2.5f;
        //Debug.Log(" FindfoodHunger "+hungerFactorsSchool.findFoodHunger);

        float leftOfHungerFactor = 2.5f - hungerFactorsSchool.findFoodHunger;
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            hungerFactorsSchool.prevDirectionHunger = leftOfHungerFactor * 0.2f;
            hungerFactorsSchool.swimWithOtherFishHunger = leftOfHungerFactor * 0.3f;
            hungerFactorsSchool.collisionDodgeHunger = 0;
            hungerFactorsSchool.optimalDepthHunger = leftOfHungerFactor * 0.1f;
            hungerFactorsSchool.holdDistanceToFishHunger = leftOfHungerFactor * 0.4f;

        }
        else
        {
            hungerFactorsSchool.findFoodHunger = 0;
            hungerFactorsSchool.prevDirectionHunger = 0;
            hungerFactorsSchool.swimWithOtherFishHunger = 0;
            hungerFactorsSchool.collisionDodgeHunger = 2.5f;
            hungerFactorsSchool.optimalDepthHunger = 0;
            hungerFactorsSchool.holdDistanceToFishHunger = 0;
        }
    }
    #endregion

    #region stress Factors
    public void calculateStressFactorsAlone() {
        stressFactorsAlone.findFoodStress = 20 / (_fish.Hunger / Fish.maxHunger * 100);

        if (stressFactorsAlone.findFoodStress > 2)
            stressFactorsAlone.findFoodStress = 2;

        float leftOfStressFactor = 2 - stressFactorsAlone.findFoodStress;
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            stressFactorsAlone.prevDirectionStress = leftOfStressFactor * 1f/*0.6f*/;
            stressFactorsAlone.findFishStress = leftOfStressFactor * 0/*.3f*/;
            stressFactorsAlone.collisionDodgeStress = 0;
            stressFactorsAlone.optimalDepthStress = leftOfStressFactor * 0/*.1*/;
        }
        else
        {
            stressFactorsAlone.prevDirectionStress = 0;
            stressFactorsAlone.findFoodStress = 0;
            stressFactorsAlone.findFishStress = 0;
            stressFactorsAlone.collisionDodgeStress = 2;
            stressFactorsAlone.optimalDepthStress = 0;
        }       
    }

    private void calculateStressFactorsSchool()
    {
        stressFactorsSchool.findFoodStress = 25 / (_fish.Hunger / Fish.maxHunger * 100);

        if (stressFactorsSchool.findFoodStress > 2.5f)
            stressFactorsSchool.findFoodStress = 2.5f;

        float leftOfStressFactor = 2.5f - stressFactorsSchool.findFoodStress;
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            stressFactorsSchool.prevDirectionStress = leftOfStressFactor * 0.2f;
            stressFactorsSchool.swimWithOtherFishStress = leftOfStressFactor * 0.4f;
            stressFactorsSchool.collisionDodgeStress = 0;
            stressFactorsSchool.optimalDepthStress = leftOfStressFactor * 0.1f;
            stressFactorsSchool.holdDistanceToFishStress = leftOfStressFactor * 0.3f;
        }
        else
        {
            stressFactorsSchool.prevDirectionStress = 0;
            stressFactorsSchool.findFoodStress = 0;
            stressFactorsSchool.swimWithOtherFishStress = 0;
            stressFactorsSchool.collisionDodgeStress = 2.5f;
            stressFactorsSchool.optimalDepthStress = 0;
            stressFactorsSchool.holdDistanceToFishStress = 0;
        }        
    }
    #endregion

    #region Depth Factors

    private void setDepthFactorsAlone()
    {
        depthFactorsAlone.optimalDepthDepth = 0 /*1 * (Mathf.Sqrt(Mathf.Pow((_net.transform.lossyScale.y / 2 - transform.position.y), 2))) / _net.transform.lossyScale.y / 2 */;
        //find bedre navn gidder ikke lige nu
        float theRest = 1 - depthFactorsAlone.optimalDepthDepth / 4;
        depthFactorsAlone.prevDirectionDepth = theRest;
        depthFactorsAlone.findFoodDepth = theRest;
        depthFactorsAlone.findFishDepth = theRest;
        depthFactorsAlone.collisionDodgeDepth = theRest;
    }

    private void setDepthFactorsSchool()
    {
        depthFactorsSchool.optimalDepthDepth = 1 * (Mathf.Sqrt(Mathf.Pow((_net.transform.lossyScale.y / 2 - transform.position.y), 2))) / _net.transform.lossyScale.y / 2;
        //find bedre navn gidder ikke lige nu
        float theRest = 1 - depthFactorsSchool.optimalDepthDepth / 5;
        depthFactorsSchool.prevDirectionDepth = theRest;
        depthFactorsSchool.findFoodDepth = theRest;
        depthFactorsSchool.swimWithOtherFishDepth = theRest;
        depthFactorsSchool.collisionDodgeDepth = theRest;
        depthFactorsSchool.holdDistanceToFishDepth = theRest;
    }
    #endregion
    #endregion
    private void SetColor(Color col)
    {
        if (_mat == null && _fish.FishObject != null)
            _mat = _fish.FishObject.GetComponent<Renderer>().material;

        if (_mat.color != col)
            _mat.color = col;
    }

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
        }
        else if (transform.rotation.x <= -0.7f && transform.position.y < 10)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 5 * Time.deltaTime, transform.position.z);
        }
        else {
            transform.position = new Vector3(-5000.0f, -5000.0f, -5000.0f);
            this.transform.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Food Methods
    public Vector3 canSeeFood()
    {
        Vector3 closestFood = new Vector3(100, 100, 100);
        //Iterate through list of food nearby, and choose the closest one. 
        foreach (KeyValuePair<int, Vector3> item in knownFoodSpots) {
            if (_mathTools.GetDistanceBetweenVectors(item.Value, transform.position)
                < _mathTools.GetDistanceBetweenVectors(closestFood, new Vector3())) {
                closestFood = item.Value;
            }
        }
        //Mathf.Sqrt(Mathf.Pow(item.Value.x - this.transform.position.x, 2) + Mathf.Pow(item.Value.y - this.transform.position.y, 2) + Mathf.Pow(item.Value.z - this.transform.position.z, 2))
        //Mathf.Sqrt(Mathf.Pow(closestFood.x, 2) + Mathf.Pow(closestFood.y, 2) + Mathf.Pow(closestFood.z, 2))
        return closestFood;
    }

    public Vector3 cantSeeFood()
    {
        Vector3 sumVecD3 = new Vector3();
        float factor;
        if (lastKnownFoodSpots.Count == 0)
        {
            return new Vector3(0, 0, 0);
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
    public Vector3 SwimTowardsOtherFish() {
        Vector3 D_3 = new Vector3(0, 0, 0);
        float distanceBetweenFish;
        foreach (FishBehaviour fish in _nearbyFish)
        {
            distanceBetweenFish = _mathTools.GetDistanceBetweenVectors(transform.position, fish.transform.position) / _nearbyFish.Count;
            D_3.x += distanceBetweenFish * (fish.transform.position.x - transform.position.x);
            D_3.y += distanceBetweenFish * (fish.transform.position.y - transform.position.y);
            D_3.z += distanceBetweenFish * (fish.transform.position.z - transform.position.z);
        }
        //foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish) {
        //    distanceBetweenFish = _mathTools.GetDistanceBetweenVectors(transform.position, item.Value.transform.position) / nearbyFish.Count;
        //    D_3.x += distanceBetweenFish * (item.Value.transform.position.x - transform.position.x);
        //    D_3.y += distanceBetweenFish * (item.Value.transform.position.y - transform.position.y);
        //    D_3.z += distanceBetweenFish * (item.Value.transform.position.z - transform.position.z);
        //}
        return D_3;
    }
    #endregion

    #region Swim with fish
    private Vector3 SwimWithFriends() {
        Vector3 D_3 = new Vector3(0, 0, 0);
        foreach (FishBehaviour fish in _nearbyFish)
        {
            D_3.x += Fish.CurrentDirection.x / nearbyFish.Count;
            D_3.y += Fish.CurrentDirection.y / nearbyFish.Count;
            D_3.z += Fish.CurrentDirection.z / nearbyFish.Count;
        }

        //foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish) {
        //    D_3 = item.Value.Fish.CurrentDirection;
        //    /*D_3.x += item.Value.Fish.CurrentDirection.x / nearbyFish.Count;
        //    D_3.y += item.Value.Fish.CurrentDirection.y / nearbyFish.Count;
        //    D_3.z += item.Value.Fish.CurrentDirection.z / nearbyFish.Count;*/
        //}
        return D_3;
    }
    #endregion

    #region Hold distance to fish
    private Vector3 HoldDistanceToFish()
    {
        Vector3 GoAway = new Vector3(0, 0, 0);
        Vector3 GoCloser = new Vector3(0, 0, 0);
        foreach (FishBehaviour item in _nearbyFish)
        {
            float distanceBetweenFish = _mathTools.GetDistanceBetweenVectors(item.transform.position, transform.position);
            float distanceFactor = (1 / Mathf.Sin(3 * distanceBetweenFish)) - 1;
            if (distanceBetweenFish < 0.52f)
            {
                float negativeDistanceFactor = (-1) * (distanceFactor);
                GoAway.x += negativeDistanceFactor * (item.transform.position.x - transform.position.x) / _nearbyFish.Count;
                GoAway.y += negativeDistanceFactor * (item.transform.position.y - transform.position.y) / _nearbyFish.Count;
                GoAway.z += negativeDistanceFactor * (item.transform.position.z - transform.position.z) / _nearbyFish.Count;
            }
            else if (distanceBetweenFish < 0.86f)
            {
                GoCloser.x += (distanceFactor) * (item.transform.position.x - transform.position.x) / _nearbyFish.Count;
                GoCloser.y += (distanceFactor) * (item.transform.position.y - transform.position.y) / _nearbyFish.Count;
                GoCloser.z += (distanceFactor) * (item.transform.position.z - transform.position.z) / _nearbyFish.Count;
            }
        }
        //foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish)
        //{
        //    float distanceBetweenFish = _mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position);
        //    float distanceFactor = (1 / Mathf.Sin(3 * distanceBetweenFish)) - 1;
        //    if (distanceBetweenFish < 0.52f)
        //    {
        //        float negativeDistanceFactor = (-1) * (distanceFactor);
        //        GoAway.x += negativeDistanceFactor * (item.Value.transform.position.x - transform.position.x) / nearbyFish.Count;
        //        GoAway.y += negativeDistanceFactor * (item.Value.transform.position.y - transform.position.y) / nearbyFish.Count;
        //        GoAway.z += negativeDistanceFactor * (item.Value.transform.position.z - transform.position.z) / nearbyFish.Count;
        //    }
        //    else if (distanceBetweenFish < 0.86f)
        //    {
        //        GoCloser.x += (distanceFactor) * (item.Value.transform.position.x - transform.position.x) / nearbyFish.Count;
        //        GoCloser.y += (distanceFactor) * (item.Value.transform.position.y - transform.position.y) / nearbyFish.Count;
        //        GoCloser.z += (distanceFactor) * (item.Value.transform.position.z - transform.position.z) / nearbyFish.Count;
        //    }
        //}
        //foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish)
        //{
        //    float distanceBetweenFish = _mathTools.GetDistanceBetweenVectors(item.Value.transform.position, transform.position);
        //    float distanceFactor = (1 / Mathf.Sin(3 * distanceBetweenFish)) - 1;
        //    if (distanceBetweenFish < 0.52f)
        //    {
        //        float negativeDistanceFactor = (-1) * (distanceFactor);
        //        GoAway.x += negativeDistanceFactor * (item.Value.transform.position.x - transform.position.x) / nearbyFish.Count;
        //        GoAway.y += negativeDistanceFactor * (item.Value.transform.position.y - transform.position.y) / nearbyFish.Count;
        //        GoAway.z += negativeDistanceFactor * (item.Value.transform.position.z - transform.position.z) / nearbyFish.Count;
        //    }
        //    else if (distanceBetweenFish < 0.86f) {
        //        GoCloser.x += (distanceFactor) * (item.Value.transform.position.x - transform.position.x) / nearbyFish.Count;
        //        GoCloser.y += (distanceFactor) * (item.Value.transform.position.y - transform.position.y) / nearbyFish.Count;
        //        GoCloser.z += (distanceFactor) * (item.Value.transform.position.z - transform.position.z) / nearbyFish.Count;
        //    }
        //}
        return GoAway + GoCloser;
    }
    #endregion

    #region Search for optimal depth
    public Vector3 SearchForOptimalDepth() {
        return new Vector3();
        var vec = new Vector3(transform.position.x, 0 - transform.position.y, transform.position.z);
        return vec;
    }
    #endregion

    #region Get new direction
    private Vector3 GetNewDirection()
    {
        if (_fish.IsDead)
        {
            return new Vector3(0, 0, 0);
        }
        bool schooling = IsSchooling();
        bool isThereNearbyFood = false;
        directions.previousDirection = _fish.CurrentDirection;
        directions.optimalDepthDirection = SearchForOptimalDepth();
        //foreach (KeyValuePair<int, FishBehaviour> item in nearbyFish) {
        //    if (_mathTools.GetDistanceBetweenVectors(transform.position, item.Value.transform.position) < 1f)
        //    {
        //        schooling = true;

        //        break;
        //    }
        //}
        if (knownFoodSpots.Count < 0) {
            isThereNearbyFood = true;
        }
        
        if (schooling)
        {
            setDepthFactorsSchool();
            calculateHungerFactorsSchool();
            calculateStressFactorsSchool();
            calculateLambdaSchool();
            directions.swimWithOrToFish = SwimWithFriends();
            directions.holdDistanceToFishDirection = HoldDistanceToFish();
            if (isThereNearbyFood) {
                directions.findFoodDirection = canSeeFood();
                _fish.CurrentDirection = directions.previousDirection * lambdaSchool.prevDirectionLambda + directions.findFoodDirection * lambdaSchool.findFoodLambda + directions.swimWithOrToFish * lambdaSchool.swimWithOtherFishLambda
                    + directions.dodgeCollisionDirection * lambdaSchool.collisionDodgeLambda + directions.optimalDepthDirection * lambdaSchool.optimalDepthLambda + directions.holdDistanceToFishDirection * lambdaSchool.holdDistanceToFishLambda;
            }
            else {
                directions.findFoodDirection = cantSeeFood();

                _fish.CurrentDirection = directions.previousDirection * lambdaSchool.prevDirectionLambda + directions.findFoodDirection * lambdaSchool.findFoodLambda + directions.swimWithOrToFish * lambdaSchool.swimWithOtherFishLambda
                    + directions.dodgeCollisionDirection * lambdaSchool.collisionDodgeLambda + directions.optimalDepthDirection * lambdaSchool.optimalDepthLambda + directions.holdDistanceToFishDirection * lambdaSchool.holdDistanceToFishLambda;                    
            }
        }
        else {
            setDepthFactorsAlone();
            calculateHungerFactorsAlone();
            calculateStressFactorsAlone();
            calculateLambdaAlone();
            directions.swimWithOrToFish = SwimTowardsOtherFish();
            if (isThereNearbyFood)
            {
                directions.findFoodDirection = canSeeFood();
                _fish.CurrentDirection = directions.previousDirection * lambdaAlone.prevDirectionLambda + directions.findFoodDirection * lambdaAlone.findFoodLambda
                    + directions.swimWithOrToFish * lambdaAlone.findOtherFishLambda + directions.dodgeCollisionDirection * lambdaAlone.collisionDodgeLambda + directions.optimalDepthDirection * lambdaAlone.optimalDepthLambda;
            }
            else 
            {
                directions.findFoodDirection = cantSeeFood();
                _fish.CurrentDirection = directions.previousDirection * lambdaAlone.prevDirectionLambda + directions.findFoodDirection * lambdaAlone.findFoodLambda
                    + directions.swimWithOrToFish * lambdaAlone.findOtherFishLambda + directions.dodgeCollisionDirection * lambdaAlone.collisionDodgeLambda + directions.optimalDepthDirection * lambdaAlone.optimalDepthLambda;
            }
        }
        if (!_isObstacleDetected)
        {
            directions.dodgeCollisionDirection = new Vector3(0, 0, 0);
        }

        schooling = false;
        return _fish.CurrentDirection;
    }

    private bool IsSchooling()
    {
        foreach (FishBehaviour fish in _nearbyFish)
        {
            if (_mathTools.GetDistanceBetweenVectors(transform.position, fish.transform.position) < 1f)
                return true;
        }
        return false;
    }
    #endregion

    private Vector3 collidingwithcagemovementtest(){
        if (_isObstacleDetected) {
            _fish.CurrentDirection = directions.dodgeCollisionDirection;
        }

        return _fish.CurrentDirection;
    }


}
