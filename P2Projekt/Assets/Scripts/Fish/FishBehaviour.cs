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

    private bool _isObstacleDetected = false;
    private List<Vector3> lastKnownFoodSpots = new List<Vector3>();
    private Dictionary<int, Vector3> inInnerCollider = new Dictionary<int, Vector3>();

    public List<FishBehaviour> _nearbyFish = new List<FishBehaviour>();
    public List<FoodBehavior> _nearbyFood = new List<FoodBehavior>();

    private SphereCollider _outerCollider;
    private SphereCollider _innerCollider;

    //Stress
    private float timerToDie = 0;
    private float timerToResetTimer = 0;
    private const float _stressMultiplier = 1.5f; //tidligere 1,5

    private float removePointTimer;
    public List<Vector3> savedKnownFoodSpots = new List<Vector3>();
    #region Lambda structs
    private LambdaAlone lambdaAlone = new LambdaAlone();
    private LambdaSchool lambdaSchool = new LambdaSchool();
    private StressLambdaAlone stressFactorsAlone = new StressLambdaAlone();
    private StressLambdaSchool stressFactorsSchool = new StressLambdaSchool();
    private HungerLambdaAlone hungerFactorsAlone = new HungerLambdaAlone();
    private HungerLambdaSchool hungerFactorsSchool = new HungerLambdaSchool();
    private DepthLambdaAlone depthFactorsAlone = new DepthLambdaAlone();
    private DepthLambdaSchool depthFactorsSchool = new DepthLambdaSchool();
    private DirectionVectors directions = new DirectionVectors();
    #endregion

    private Material _mat;
    private Color _defaultColor = new Color(191 / 255f, 249 / 255f, 249 / 255f, 255 / 255f);

    private Vector3 uniqueOffset;

    private void Start()
    {
        DataManager = FindObjectOfType<DataManager>();

        Net = GameObject.FindGameObjectWithTag("Net");

        GetComponent<SphereCollider>().radius = 5f;

        _outerCollider = GetComponents<SphereCollider>()[0];
        _innerCollider = GetComponents<SphereCollider>()[1];

        // Generate randomly last known food positions
        for (int i = 0; i < 5; i++)
           lastKnownFoodSpots.Add(new Vector3(Random.value * (_net.gameObject.transform.lossyScale.x / 3.75f), Random.value * (_net.gameObject.transform.lossyScale.y / 3.75f), Random.value * (_net.gameObject.transform.lossyScale.z / 3.75f)));

        GenerateRandomOffset();
    }

    private void GenerateRandomOffset()
    {
        uniqueOffset = new Vector3(GetRandomFloat(), GetRandomFloat(), GetRandomFloat());
        uniqueOffset *= 1.5f;
    }

    private float GetRandomFloat()
    {
        return Random.value > 0.5f ? Random.value : -Random.value;
    }

    // Update is called once per frame
    private void Update()
    {
        _fish.MoveTowards(GetNewDirection());
        UpdateStress();
        UpdateHunger();
        AnimateDeath();
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleSpottedObject(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Food"))
        {
            if(MathTools.GetDistanceBetweenVectors(transform.position, other.ClosestPoint(transform.position)) > _innerCollider.radius)
            {
                FoodBehavior foodBehav = other.GetComponent<FoodBehavior>();
                if (foodBehav == null)
                    return;

                if (_nearbyFood.Contains(foodBehav))
                    _nearbyFood.Remove(foodBehav);
            }
        }
        else if (other.tag.Equals("Fish"))
        {

            if (MathTools.GetDistanceBetweenVectors(transform.position, other.gameObject.transform.position) >= _innerCollider.radius)
                return;

            FishBehaviour fishBehav = other.GetComponent<FishBehaviour>();
            if (fishBehav == null)
                return;

            if (_nearbyFish.Contains(fishBehav))
                _nearbyFish.Remove(fishBehav);
        }
    }

    private void HandleSpottedObject(Collider other)
    {
        // Check if the object detected is another fish, or an obstacle.
        if (other.tag.Equals("Fish"))
        {
            if (MathTools.GetDistanceBetweenVectors(transform.position, other.gameObject.transform.position) <= _innerCollider.radius)
                return;

            FishBehaviour fishBehav = other.GetComponent<FishBehaviour>();
            if (fishBehav == null)
                return;


            if (!_nearbyFish.Contains(fishBehav))
                _nearbyFish.Add(fishBehav);
        }
        else if (other.tag.Equals("Obstacle") || (other.tag.Equals("Net")))
        {
            Vector3 closestPos = other.ClosestPoint(transform.position);

            if (MathTools.GetDistanceBetweenVectors(transform.position, closestPos) <= _innerCollider.radius)
            {
                if (_isObstacleDetected = IsHeadingTowardsPoint(closestPos))
                {
                    int offset = 1;
                    directions.DodgeCollisionDirection = FindFreeDir(closestPos, ref offset);
                }
            }
        }
        else if (other.tag.Equals("Food"))
        {
            FoodBehavior othersFoodBehavior = other.GetComponent<FoodBehavior>();
            if (othersFoodBehavior == null)
                return;

            if (MathTools.GetDistanceBetweenVectors(transform.position, other.ClosestPoint(transform.position)) <= _innerCollider.radius)
            {
                _fish.Hunger = Fish.maxHunger;
                othersFoodBehavior.BeingEaten();
            }
            else
            {
                if(!_nearbyFood.Contains(othersFoodBehavior))
                    _nearbyFood.Add(othersFoodBehavior);
                if(!lastKnownFoodSpots.Contains(other.transform.position))
                    lastKnownFoodSpots.Add(other.ClosestPoint(transform.position));
            }
        }
    }

    public void EatFood(FoodBehavior food)
    {
        if(_nearbyFood.Contains(food))
            _nearbyFood.Remove(food);

        foreach (Vector3 point in savedKnownFoodSpots)
        {
            lastKnownFoodSpots.Add(point);
        }
        savedKnownFoodSpots.Clear();
    }

    private bool IsHeadingTowardsPoint(Vector3 pos)
    {
        float angle = MathTools.GetAngleBetweenVectors((_fish.DesiredPoint - _fish.FishObject.transform.position), (pos - transform.position));

        Vector3 posOne = _fish.DesiredPoint - transform.position;
        Vector3 posTwo = pos - transform.position;
        Debug.DrawRay(transform.position, posOne.normalized, Color.red, 30);

        float dist = MathTools.GetDistanceBetweenVectors(transform.position, pos);
        float catheter = MathTools.GetOpposingCatheter(angle, dist);

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
        _fish.Hunger -= 1 * Time.deltaTime;
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
        lambdaAlone.PrevDirectionLambda = CS * (stressFactorsAlone.PrevDirectionStress + hungerFactorsAlone.PrevDirectionHunger + depthFactorsAlone.PrevDirectionDepth);
        lambdaAlone.FindFoodLambda = CS * (stressFactorsAlone.FindFoodStress + hungerFactorsAlone.FindFoodHunger + depthFactorsAlone.FindFoodDepth);
        lambdaAlone.FindOtherFishLambda = CS * (stressFactorsAlone.FindFishStress + hungerFactorsAlone.FindFishHunger + depthFactorsAlone.FindFishDepth);
        lambdaAlone.CollisionDodgeLambda = CS * (stressFactorsAlone.CollisionDodgeStress + hungerFactorsAlone.CollisionDodgeHunger + depthFactorsAlone.CollisionDodgeDepth);
        lambdaAlone.OptimalDepthLambda = CS * (stressFactorsAlone.OptimalDepthStress + hungerFactorsAlone.OptimalDepthHunger + depthFactorsAlone.OptimalDepthDepth);

    }
    private void calculateLambdaSchool() {
        //CS = constant value
        float CS = 1.0f / 6.0f;
        lambdaSchool.PrevDirectionLambda = CS * (stressFactorsSchool.PrevDirectionStress + hungerFactorsSchool.PrevDirectionHunger + depthFactorsSchool.PrevDirectionDepth);
        lambdaSchool.FindFoodLambda = CS * (stressFactorsSchool.FindFoodStress + hungerFactorsSchool.FindFoodHunger + depthFactorsSchool.FindFoodDepth);
        lambdaSchool.SwimWithOtherFishLambda = CS * (stressFactorsSchool.SwimWithOtherFishStress + hungerFactorsSchool.SwimWithOtherFishHunger + depthFactorsSchool.SwimWithOtherFishDepth);
        lambdaSchool.CollisionDodgeLambda = CS * (stressFactorsSchool.CollisionDodgeStress + hungerFactorsSchool.CollisionDodgeHunger + depthFactorsSchool.CollisionDodgeDepth);
        lambdaSchool.OptimalDepthLambda = CS * (stressFactorsSchool.OptimalDepthStress + hungerFactorsSchool.OptimalDepthHunger + depthFactorsSchool.OptimalDepthDepth);
        lambdaSchool.HoldDistanceToFishLambda = CS * (stressFactorsSchool.HoldDistanceToFishStress + hungerFactorsSchool.HoldDistanceToFishHunger + depthFactorsSchool.HoldDistanceToFishDepth);
    }

    #region Hunger factors
    private void calculateHungerFactorsAlone() {
        hungerFactorsAlone.FindFoodHunger = 40f / (_fish.Hunger / Fish.maxHunger * 100f);

        if (hungerFactorsAlone.FindFoodHunger > 2f)
            hungerFactorsAlone.FindFoodHunger = 2f;
        float leftOfHungerFactor = 2f - hungerFactorsAlone.FindFoodHunger;

        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            hungerFactorsAlone.PrevDirectionHunger = leftOfHungerFactor * 0.5f;
            hungerFactorsAlone.FindFishHunger = leftOfHungerFactor * 0.4f;
            hungerFactorsAlone.CollisionDodgeHunger = 0;
            hungerFactorsAlone.OptimalDepthHunger = leftOfHungerFactor * 0.1f;
        }
        else
        {
            hungerFactorsAlone.FindFoodHunger = 0;
            hungerFactorsAlone.PrevDirectionHunger = 0;
            hungerFactorsAlone.FindFishHunger = 0;
            hungerFactorsAlone.CollisionDodgeHunger = 2;
            hungerFactorsAlone.OptimalDepthHunger = 0;
        }
    }
    private void calculateHungerFactorsSchool()
    {
        hungerFactorsSchool.FindFoodHunger = 25 / (_fish.Hunger / Fish.maxHunger * 100);

        if (hungerFactorsSchool.FindFoodHunger > 2.5f)
            hungerFactorsSchool.FindFoodHunger = 2.5f;

        float leftOfHungerFactor = 2.5f - hungerFactorsSchool.FindFoodHunger;
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            hungerFactorsSchool.PrevDirectionHunger = leftOfHungerFactor * 0.2f;
            hungerFactorsSchool.SwimWithOtherFishHunger = leftOfHungerFactor * 0.3f;
            hungerFactorsSchool.CollisionDodgeHunger = 0;
            hungerFactorsSchool.OptimalDepthHunger = leftOfHungerFactor * 0.1f;
            hungerFactorsSchool.HoldDistanceToFishHunger = leftOfHungerFactor * 0.4f;
        }
        else
        {
            hungerFactorsSchool.FindFoodHunger = 0;
            hungerFactorsSchool.PrevDirectionHunger = 0;
            hungerFactorsSchool.SwimWithOtherFishHunger = 0;
            hungerFactorsSchool.CollisionDodgeHunger = 2.5f;
            hungerFactorsSchool.OptimalDepthHunger = 0;
            hungerFactorsSchool.HoldDistanceToFishHunger = 0;
        }
    }
    #endregion

    #region stress Factors
    public void calculateStressFactorsAlone() {
        stressFactorsAlone.FindFoodStress = 40f / (_fish.Hunger / Fish.maxHunger * 100f);


        if (stressFactorsAlone.FindFoodStress > 2f)
            stressFactorsAlone.FindFoodStress = 2f;
        float leftOfStressFactor = 2f - stressFactorsAlone.FindFoodStress;
        //txt2.text = "stress : " + stressFactorsAlone.findFoodStress;
       
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            stressFactorsAlone.PrevDirectionStress = leftOfStressFactor * 0.6f;
            stressFactorsAlone.FindFishStress = leftOfStressFactor * 0.3f;
            stressFactorsAlone.CollisionDodgeStress = 0;
            stressFactorsAlone.OptimalDepthStress = leftOfStressFactor * 0.1f;
        }
        else
        {
            stressFactorsAlone.PrevDirectionStress = 0;
            stressFactorsAlone.FindFoodStress = 0;
            stressFactorsAlone.FindFishStress = 0;
            stressFactorsAlone.CollisionDodgeStress = 2;
            stressFactorsAlone.OptimalDepthStress = 0;
        }       
    }

    private void calculateStressFactorsSchool()
    {
        stressFactorsSchool.FindFoodStress = 25 / (_fish.Hunger / Fish.maxHunger * 100);

        if (stressFactorsSchool.FindFoodStress > 2.5f)
            stressFactorsSchool.FindFoodStress = 2.5f;

        float leftOfStressFactor = 2.5f - stressFactorsSchool.FindFoodStress;
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            stressFactorsSchool.PrevDirectionStress = leftOfStressFactor * 0.2f;
            stressFactorsSchool.SwimWithOtherFishStress = leftOfStressFactor * 0.4f;
            stressFactorsSchool.CollisionDodgeStress = 0;
            stressFactorsSchool.OptimalDepthStress = leftOfStressFactor * 0.1f;
            stressFactorsSchool.HoldDistanceToFishStress = leftOfStressFactor * 0.3f;
        }
        else
        {
            stressFactorsSchool.PrevDirectionStress = 0;
            stressFactorsSchool.FindFoodStress = 0;
            stressFactorsSchool.SwimWithOtherFishStress = 0;
            stressFactorsSchool.CollisionDodgeStress = 2.5f;
            stressFactorsSchool.OptimalDepthStress = 0;
            stressFactorsSchool.HoldDistanceToFishStress = 0;
        }        
    }
    #endregion

    #region Depth Factors

    private void setDepthFactorsAlone()
    {
        depthFactorsAlone.OptimalDepthDepth = 1 * (Mathf.Sqrt(Mathf.Pow((_net.transform.lossyScale.y / 2 - transform.position.y), 2))) / _net.transform.lossyScale.y / 2;
        float theRest = (1 - depthFactorsAlone.OptimalDepthDepth) / 4;
        depthFactorsAlone.PrevDirectionDepth = theRest;
        depthFactorsAlone.FindFoodDepth = theRest;
        depthFactorsAlone.FindFishDepth = theRest;
        depthFactorsAlone.CollisionDodgeDepth = theRest;
    }

    private void setDepthFactorsSchool()
    {
        depthFactorsSchool.OptimalDepthDepth = 1 * (Mathf.Sqrt(Mathf.Pow((_net.transform.lossyScale.y / 2 - transform.position.y), 2))) / _net.transform.lossyScale.y / 2;
        //find bedre navn gidder ikke lige nu
        float theRest = (1 - depthFactorsSchool.OptimalDepthDepth) / 5;
        depthFactorsSchool.PrevDirectionDepth = theRest;
        depthFactorsSchool.FindFoodDepth = theRest;
        depthFactorsSchool.SwimWithOtherFishDepth = theRest;
        depthFactorsSchool.CollisionDodgeDepth = theRest;
        depthFactorsSchool.HoldDistanceToFishDepth = theRest;
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
    public Vector3 GetClosestFoodPoint()
    {
        Vector3 closestFood = _nearbyFood[0].transform.position;

        //Iterate through list of food nearby, and choose the closest one. 
        foreach (FoodBehavior food in _nearbyFood)
        {
            if (MathTools.GetDistanceBetweenVectors(food.transform.position, transform.position)
                < MathTools.GetDistanceBetweenVectors(transform.position, closestFood))
            {
                closestFood = food.transform.position;
            }
        }
        return closestFood;
    }

    public Vector3 cantSeeFood()
    {
        Vector3 sumVecD3 = new Vector3();

        if (lastKnownFoodSpots.Count == 0)
        {
            return new Vector3(0, 0, 0);
        }
        else 
            sumVecD3 = GetclosestPoint(lastKnownFoodSpots);

        if (MathTools.GetDistanceBetweenVectors(sumVecD3, transform.position) < 1f) {
            removePointTimer += Time.deltaTime;
        }
        if (removePointTimer > 4) {
            removePointTimer = 0;
            lastKnownFoodSpots.Remove(sumVecD3);
            savedKnownFoodSpots.Add(sumVecD3);
        }

        if(lastKnownFoodSpots.Count <= 0)
        {
            foreach (Vector3 point in savedKnownFoodSpots)
            {
                lastKnownFoodSpots.Add(point);
            }
            savedKnownFoodSpots.Clear();
        }

        return sumVecD3;
    }

    private Vector3 GetclosestPoint(List<Vector3> arrayOfPoints) {
        Vector3 closestPoint = arrayOfPoints[0];
        foreach (Vector3 point in arrayOfPoints)
        {
            if (MathTools.GetDistanceBetweenVectors(point, transform.position)
                < MathTools.GetDistanceBetweenVectors(transform.position, closestPoint))
            {
                closestPoint = point;
            }
        }
        return closestPoint;
    }

    #endregion

    #region Swim towards other fish
    public Vector3 SwimTowardsOtherFish() {
        Vector3 D_3 = new Vector3(0, 0, 0);
        float distanceBetweenFish;
        foreach (FishBehaviour fish in _nearbyFish)
        {
            distanceBetweenFish = MathTools.GetDistanceBetweenVectors(transform.position, fish.transform.position) / _nearbyFish.Count;
            D_3.x += distanceBetweenFish * (fish.transform.position.x - transform.position.x);
            D_3.y += distanceBetweenFish * (fish.transform.position.y - transform.position.y);
            D_3.z += distanceBetweenFish * (fish.transform.position.z - transform.position.z);
        }
        return D_3;
    }
    #endregion

    #region Swim with fish
    private Vector3 SwimWithFriends() {
        Vector3 D_3 = new Vector3(0, 0, 0);
        foreach (FishBehaviour fish in _nearbyFish)
        {
            D_3.x += Fish.DesiredPoint.x / _nearbyFish.Count;
            D_3.y += Fish.DesiredPoint.y / _nearbyFish.Count;
            D_3.z += Fish.DesiredPoint.z / _nearbyFish.Count;
        }
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
            float distanceBetweenFish = MathTools.GetDistanceBetweenVectors(item.transform.position, transform.position);
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
        return GoAway + GoCloser;
    }
    #endregion

    #region Search for optimal depth
    public Vector3 SearchForOptimalDepth() {
        return new Vector3(transform.position.x, 0 - transform.position.y, transform.position.z);
    }
    #endregion

    #region Get new direction
    private Vector3 GetNewDirection()
    {
        if (_fish.IsDead)
        {
            return new Vector3(0, 0, 0);
        }
        bool isSchooling = IsSchooling();
        bool isThereNearbyFood = false;

        directions.PreviousPoint = _fish.DesiredPoint + uniqueOffset;

        directions.OptimalDepthDirection = SearchForOptimalDepth();

        if (_nearbyFood.Count > 0) {
            isThereNearbyFood = true;
        }
        
        if (isSchooling)
        {
            setDepthFactorsSchool();
            calculateHungerFactorsSchool();
            calculateStressFactorsSchool();
            calculateLambdaSchool();
            directions.SwimWithOrToFish = SwimWithFriends();
            directions.HoldDistanceToFishDirection = HoldDistanceToFish();
            if (isThereNearbyFood) {
                directions.FindFoodDirection = GetClosestFoodPoint();
                _fish.DesiredPoint = directions.PreviousPoint * lambdaSchool.PrevDirectionLambda + directions.FindFoodDirection * lambdaSchool.FindFoodLambda + directions.SwimWithOrToFish * lambdaSchool.SwimWithOtherFishLambda
                    + directions.DodgeCollisionDirection * lambdaSchool.CollisionDodgeLambda + directions.OptimalDepthDirection * lambdaSchool.OptimalDepthLambda + directions.HoldDistanceToFishDirection * lambdaSchool.HoldDistanceToFishLambda;
            }
            else {
                directions.FindFoodDirection = cantSeeFood();

                _fish.DesiredPoint = directions.PreviousPoint * lambdaSchool.PrevDirectionLambda + directions.FindFoodDirection * lambdaSchool.FindFoodLambda + directions.SwimWithOrToFish * lambdaSchool.SwimWithOtherFishLambda
                    + directions.DodgeCollisionDirection * lambdaSchool.CollisionDodgeLambda + directions.OptimalDepthDirection * lambdaSchool.OptimalDepthLambda + directions.HoldDistanceToFishDirection * lambdaSchool.HoldDistanceToFishLambda;
            }
        }
        else {
            setDepthFactorsAlone();
            calculateHungerFactorsAlone();
            calculateStressFactorsAlone();
            calculateLambdaAlone();
            directions.SwimWithOrToFish = SwimTowardsOtherFish();
            if (isThereNearbyFood)
            {
                directions.FindFoodDirection = GetClosestFoodPoint();
                _fish.DesiredPoint = directions.PreviousPoint * lambdaAlone.PrevDirectionLambda + directions.FindFoodDirection * lambdaAlone.FindFoodLambda
                    + directions.SwimWithOrToFish * lambdaAlone.FindOtherFishLambda + directions.DodgeCollisionDirection * lambdaAlone.CollisionDodgeLambda + directions.OptimalDepthDirection * lambdaAlone.OptimalDepthLambda;
            }
            else 
            {
                directions.FindFoodDirection = cantSeeFood();
                _fish.DesiredPoint = directions.PreviousPoint * lambdaAlone.PrevDirectionLambda + directions.FindFoodDirection * lambdaAlone.FindFoodLambda
                    + directions.SwimWithOrToFish * lambdaAlone.FindOtherFishLambda + directions.DodgeCollisionDirection * lambdaAlone.CollisionDodgeLambda + directions.OptimalDepthDirection * lambdaAlone.OptimalDepthLambda;
            }
        }
        if (!_isObstacleDetected)
        {
            directions.DodgeCollisionDirection = new Vector3(0, 0, 0);
        }

        schooling = false;
        return _fish.DesiredPoint;
    }

    private bool IsSchooling()
    {
        foreach (FishBehaviour fish in _nearbyFish)
        {
            if (MathTools.GetDistanceBetweenVectors(transform.position, fish.transform.position) < 1f)
                return true;
        }
        return false;
    }
    #endregion



}
