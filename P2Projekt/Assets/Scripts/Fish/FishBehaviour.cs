using Mathtools;
using System.Collections.Generic;
using UnityEngine;

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
    Vector3 newdir;
    public float gotDistance = 0;
    private const float _stressMultiplier = 0.5f;
    public List<Vector3> lastKnownFoodSpots = new List<Vector3>();
    public Dictionary<int, Vector3> knownFoodSpots = new Dictionary<int, Vector3>();
    public Dictionary<int, Vector3> inInnerCollider = new Dictionary<int, Vector3>();
    public Dictionary<int, FishBehaviour> nearbyFish = new Dictionary<int, FishBehaviour>();
    //Stress timer
    private float timerToDie = 0;
    private float timerToResetTimer = 0;

    #region Lambda structs
    lambdaStructAlone lambdaAlone = new lambdaStructAlone();
    lambdaStructSchool lambdaSchool = new lambdaStructSchool();
    stressFactorLambdaAlone stressFactorsAlone = new stressFactorLambdaAlone();
    stressFactorLambdaSchool stressFactorsSchool = new stressFactorLambdaSchool();
    hungerFactorLambdaAlone hungerFactorsAlone = new hungerFactorLambdaAlone();
    hungerFactorLambdaSchool hungerFactorsSchool = new hungerFactorLambdaSchool();
    depthFactorLambdaAlone depthFactorsAlone = new depthFactorLambdaAlone();
    depthFactorLambdaSchool depthFactorsSchool = new depthFactorLambdaSchool();
    directionVectors directions = new directionVectors();
    #endregion

    private MathTools _mathTools = new MathTools();
    private Material _mat;



    private Color _defaultColor = new Color(191 / 255f, 249 / 255f, 249 / 255f, 255 / 255f);

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
                directions.dodgeCollisionDirection = newDir;
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


    private void UpdateHunger()
    {
        _fish.Hunger -= 1 * Time.deltaTime;
        if (_fish.Hunger <= 0)
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

    #region Lambda
    private void calculateLambdaAlone() {
        //CS = constant value
        float CS = 1 / 5;
        lambdaAlone.prevDirectionLambda = CS * (stressFactorsAlone.prevDirectionStress + hungerFactorsAlone.prevDirectionHunger + depthFactorsAlone.prevDirectionDepth);
        lambdaAlone.findFoodLambda = CS * (stressFactorsAlone.findFoodStress + hungerFactorsAlone.findFoodHunger + depthFactorsAlone.findFoodDepth);
        lambdaAlone.findOtherFishLambda = CS * (stressFactorsAlone.findFishStress + hungerFactorsAlone.findFishHunger + depthFactorsAlone.findFishDepth);
        lambdaAlone.collisionDodgeLambda = CS * (stressFactorsAlone.collisionDodgeStress + hungerFactorsAlone.collisionDodgeHunger + depthFactorsAlone.collisionDodgeDepth);
        lambdaAlone.optimalDepthLambda = CS * (stressFactorsAlone.optimalDepthStress + hungerFactorsAlone.optimalDepthHunger + depthFactorsAlone.optimalDepthDepth);
    }
    private void calculateLambdaSchool() {
        //CS = constant value
        float CS = 1 / 6;
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
        float leftOfHungerFactor = 2 - hungerFactorsAlone.findFoodHunger;
        //if there is no object in the way
        if (true)
        {
            hungerFactorsAlone.prevDirectionHunger = leftOfHungerFactor * 0.3f;
            hungerFactorsAlone.findFishHunger = leftOfHungerFactor * 0.6f;
            hungerFactorsAlone.collisionDodgeHunger = 0;
            hungerFactorsAlone.optimalDepthHunger = leftOfHungerFactor * 0.1f;
        }
        //if there is an object in the way further ahead
        else if (true)
        {
            hungerFactorsAlone.prevDirectionHunger = leftOfHungerFactor * 0.15f;
            hungerFactorsAlone.findFishHunger = leftOfHungerFactor * 0.25f;
            hungerFactorsAlone.collisionDodgeHunger = leftOfHungerFactor * 0.5f;
            hungerFactorsAlone.optimalDepthHunger = leftOfHungerFactor * 0.1f;
        }
        //if there is an object in the way further ahead within 1 meter
        else if (true)
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
        hungerFactorsSchool.findFoodHunger = 25/(_fish.Hunger/Fish.maxHunger*100);
        float leftOfHungerFactor = 2 - hungerFactorsSchool.findFoodHunger;
        //if there is no object in the way
        if (true)
        {
            hungerFactorsSchool.prevDirectionHunger = leftOfHungerFactor * 0.2f;
            hungerFactorsSchool.swimWithOtherFishHunger = leftOfHungerFactor * 0.5f;
            hungerFactorsSchool.collisionDodgeHunger = 0;
            hungerFactorsSchool.optimalDepthHunger = leftOfHungerFactor * 0.1f;
            hungerFactorsSchool.holdDistanceToFishHunger = leftOfHungerFactor * 0.2f;

        }
        //if there is an object in the way further ahead
        else if (true)
        {
            hungerFactorsSchool.prevDirectionHunger = leftOfHungerFactor * 0.05f;
            hungerFactorsSchool.swimWithOtherFishHunger = leftOfHungerFactor * 0.20f;
            hungerFactorsSchool.collisionDodgeHunger = leftOfHungerFactor * 0.5f;
            hungerFactorsSchool.optimalDepthHunger = leftOfHungerFactor * 0.05f;
            hungerFactorsSchool.holdDistanceToFishHunger = leftOfHungerFactor *0.2f;
        }
        //if there is an object in the way further ahead within 1 meter
        else if (true)
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
    private void calculateStressFactorsAlone() {
        stressFactorsAlone.findFoodStress = 20 / (_fish.Hunger / Fish.maxHunger * 100);
        float leftOfStressFactor = 2 - stressFactorsAlone.findFoodStress;
        //if there is no object in the way
        if (true)
        {
            stressFactorsAlone.prevDirectionStress = leftOfStressFactor * 0.3f;
            stressFactorsAlone.findFishStress = leftOfStressFactor * 0.6f;
            stressFactorsAlone.collisionDodgeStress = 0;
            stressFactorsAlone.optimalDepthStress = leftOfStressFactor * 0.1f;
        }
        //if there is an object in the way further ahead
        else if (true)
        {
            stressFactorsAlone.prevDirectionStress = leftOfStressFactor * 0.15f;
            stressFactorsAlone.findFishStress = leftOfStressFactor * 0.25f;
            stressFactorsAlone.collisionDodgeStress = leftOfStressFactor * 0.5f;
            stressFactorsAlone.optimalDepthStress = leftOfStressFactor * 0.1f;
        }
        //if there is an object in the way further ahead within 1 meter
        else if (true)
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
    float leftOfStressFactor = 2 - stressFactorsSchool.findFoodStress;
        //if there is no object in the way
        if (true)
    {
        stressFactorsSchool.prevDirectionStress = leftOfStressFactor * 0.2f;
        stressFactorsSchool.swimWithOtherFishStress = leftOfStressFactor * 0.5f;
        stressFactorsSchool.collisionDodgeStress = 0;
            stressFactorsSchool.optimalDepthStress = leftOfStressFactor * 0.1f;
            stressFactorsSchool.holdDistanceToFishStress = leftOfStressFactor * 0.2f;

    }
    //if there is an object in the way further ahead
    else if (true)
    {
            stressFactorsSchool.prevDirectionStress = leftOfStressFactor * 0.05f;
            stressFactorsSchool.swimWithOtherFishStress = leftOfStressFactor * 0.20f;
            stressFactorsSchool.collisionDodgeStress = leftOfStressFactor * 0.5f;
            stressFactorsSchool.optimalDepthStress = leftOfStressFactor * 0.05f;
            stressFactorsSchool.holdDistanceToFishStress = leftOfStressFactor * 0.2f;
    }
    //if there is an object in the way further ahead within 1 meter
    else if (true)
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
        depthFactorsAlone.optimalDepthDepth = 1 * (Mathf.Sqrt(Mathf.Pow((_net.transform.lossyScale.y / 2 - transform.position.y), 2))) / _net.transform.lossyScale.y / 2;
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
            return new Vector3(0,0,0);
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
        Vector3 D_3 = new Vector3(0,0,0);
        float distanceBetweenFish;
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
        directions.previousDirection = _fish.CurrentDirection;
        directions.optimalDepthDirection = SearchForOptimalDepth();
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
        directions.dodgeCollisionDirection = new Vector3(0,0,0);
        schooling = false;
        //Debug.Log("Currect direction = "+ Vector3.Normalize(_fish.CurrentDirection));
        _fish.CurrentDirection = Vector3.Normalize(_fish.CurrentDirection);
        return _fish.CurrentDirection;
        //return _fish.CurrentDirection;
    }
    #endregion
}
