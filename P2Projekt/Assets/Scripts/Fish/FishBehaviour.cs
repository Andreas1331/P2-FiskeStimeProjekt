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

    private GameObject _cage;
    public GameObject Cage { set { if (value != null) _cage = value; } }

    private bool _isObstacleDetected = false;

    private List<Vector2> lastKnownFoodSpotsVec2 = new List<Vector2>();

    private Dictionary<int, Vector3> inInnerCollider = new Dictionary<int, Vector3>();

    private List<FishBehaviour> _nearbyFish = new List<FishBehaviour>();
    private List<FoodBehavior> _nearbyFood = new List<FoodBehavior>();

    private SphereCollider _outerCollider;
    private SphereCollider _innerCollider;

    // Stress variables
    private float timerToDie = 0;
    private float timerToResetTimer = 0;
    private const float _stressMultiplier = 1.5f; 

    private float _removePointTimer;
    private List<Vector3> _savedKnownFoodSpots = new List<Vector3>();
    #region Lambda structs
    private Factors lambdaAlone = new Factors();
    private Factors stressFactorsAlone = new Factors();
    private Factors hungerFactorsAlone = new Factors();
    private Factors depthFactorsAlone = new Factors();

    private FactorsSchool lambdaSchool = new FactorsSchool();
    private FactorsSchool stressFactorsSchool = new FactorsSchool();
    private FactorsSchool hungerFactorsSchool = new FactorsSchool();
    private FactorsSchool depthFactorsSchool = new FactorsSchool();

    private DirectionVectors directions = new DirectionVectors();
    #endregion

    private Material _mat;
    private Color _defaultColor = new Color(191 / 255f, 249 / 255f, 249 / 255f, 255 / 255f);
    private const float _holdDistanceScale = 3;
    private const float _aloneScale = 40;
    private const float _schoolScale = 50;
    private const float _pointInterval = 4; // In seconds
    private const float _lambdaAloneScale = 5;
    private const float _lambdaSchoolScale = 6;
    private const float _depthScale = 4;
    private const float _depthDividerAlone = 4;
    private const float _depthDividerSchool = 5;

    private Vector3 _uniqueOffset;

    private void Start()
    {
        // Set the references to the DataManager and cage found in the scene
        DataManager = FindObjectOfType<DataManager>();
        Cage = GameObject.FindGameObjectWithTag("Cage");
        SetPositionOnSpawn();
        // Find both the colliders attached to the GameObject
        _outerCollider = GetComponents<SphereCollider>()[0];
        _outerCollider.radius = 5f;
        _innerCollider = GetComponents<SphereCollider>()[1];

        // Generate randomly last known food positions
        for (int i = 0; i < 5; i++)
        {
            lastKnownFoodSpotsVec2.Add(new Vector2(Random.value * (_cage.gameObject.transform.lossyScale.x / 3.75f), Random.value * (_cage.gameObject.transform.lossyScale.z / 3.75f)));
        }

        GenerateRandomOffset();
    }

    private void GenerateRandomOffset()
    {
        _uniqueOffset = new Vector3(GetRandomFloat(), GetRandomFloat(), GetRandomFloat());
        _uniqueOffset *= 1.5f;
    }

    private float GetRandomFloat()
    {
        return Random.value > 0.5f ? Random.value : -Random.value;
    }

    private float _interval = 0.05f;
    private float _counter = 0;
    // Update is called once per frame
    private void Update()
    {
        // Only calculate a new position every 0.05 second
        _counter += Time.deltaTime;
        if (_counter > _interval)
        {
            _fish.DesiredPoint = GetNewPosition();
            _counter = 0;
        }
        _fish.Swim();

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
            if (_innerCollider == null)
                return;

            if (MathTools.GetDistanceBetweenVectors(transform.position, other.gameObject.transform.position) <= _innerCollider.radius)
                return;

            FishBehaviour fishBehav = other.GetComponent<FishBehaviour>();
            if (fishBehav == null)
                return;


            if (!_nearbyFish.Contains(fishBehav))
                _nearbyFish.Add(fishBehav);
        }
        else if (other.tag.Equals("Obstacle") || (other.tag.Equals("Cage")))
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
                if (!lastKnownFoodSpotsVec2.Contains(new Vector2(other.transform.position.x, other.transform.position.z)))
                    lastKnownFoodSpotsVec2.Add(new Vector2 (other.transform.position.x, other.transform.position.z));
            }
        }
    }

    public void EatFood(FoodBehavior food)
    {
        if(_nearbyFood.Contains(food))
            _nearbyFood.Remove(food);
        foreach (Vector3 point in _savedKnownFoodSpots)
        {
            lastKnownFoodSpotsVec2.Add(new Vector2(point.x, point.z));
        }
        _savedKnownFoodSpots.Clear();
    }

    private bool IsHeadingTowardsPoint(Vector3 pos)
    {
        float angle = MathTools.GetAngleBetweenVectors((_fish.DesiredPoint - _fish.FishObject.transform.position), (pos - transform.position));

        float dist = MathTools.GetDistanceBetweenVectors(transform.position, pos);
        float catheter = MathTools.GetOpposingCatheter(angle, dist);

        return catheter <= _fish.Width / 2f;
    }

    private Vector3 FindFreeDir(Vector3 pos, ref int offset)
    {
        Vector3 posOne = new Vector3(pos.x + offset, pos.y, pos.z) - transform.position;
        Vector3 posTwo = new Vector3(pos.x - offset, pos.y, pos.z) - transform.position;

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
        else if (_fish.Stress < 0.9 * Fish.maxStress && timerToDie != 0)
        {
            SetColor(_defaultColor);


            timerToResetTimer += Time.deltaTime;
            if (timerToResetTimer > 30)
                timerToDie = 0;
        }
    }

    #region Lambda
    private void CalculateLambdaAlone() {
        SetDepthFactorsAlone();
        CalculateHungerFactorsAlone();
        CalculateStressFactorsAlone();
        
        float CS = 1.0f / _lambdaAloneScale; //Constant value

        lambdaAlone.PrevDirection = 
            CS * (stressFactorsAlone.PrevDirection + 
                  hungerFactorsAlone.PrevDirection + 
                  depthFactorsAlone.PrevDirection);

        lambdaAlone.FindFood = 
            CS * (stressFactorsAlone.FindFood + 
                  hungerFactorsAlone.FindFood + 
                  depthFactorsAlone.FindFood);

        lambdaAlone.SwimWithOrToFish = 
            CS * (stressFactorsAlone.SwimWithOrToFish + 
                  hungerFactorsAlone.SwimWithOrToFish + 
                  depthFactorsAlone.SwimWithOrToFish);

        lambdaAlone.CollisionDodge = 
            CS * (stressFactorsAlone.CollisionDodge + 
                  hungerFactorsAlone.CollisionDodge + 
                  depthFactorsAlone.CollisionDodge);

        lambdaAlone.OptimalDepth = 
            CS * (stressFactorsAlone.OptimalDepth + 
                  hungerFactorsAlone.OptimalDepth + 
                  depthFactorsAlone.OptimalDepth);
    }

    private void CalculateLambdaSchool() {
        SetDepthFactorsSchool();
        CalculateHungerFactorsSchool();
        CalculateStressFactorsSchool();
        //CS = constant value
        float CS = 1.0f / _lambdaSchoolScale;
        lambdaSchool.Factors.PrevDirection = CS * (stressFactorsSchool.Factors.PrevDirection + hungerFactorsSchool.Factors.PrevDirection + depthFactorsSchool.Factors.PrevDirection);
        lambdaSchool.Factors.FindFood = CS * (stressFactorsSchool.Factors.FindFood + hungerFactorsSchool.Factors.FindFood + depthFactorsSchool.Factors.FindFood);
        lambdaSchool.Factors.SwimWithOrToFish = CS * (stressFactorsSchool.Factors.SwimWithOrToFish + hungerFactorsSchool.Factors.SwimWithOrToFish + depthFactorsSchool.Factors.SwimWithOrToFish);
        lambdaSchool.Factors.CollisionDodge = CS * (stressFactorsSchool.Factors.CollisionDodge + hungerFactorsSchool.Factors.CollisionDodge + depthFactorsSchool.Factors.CollisionDodge);
        lambdaSchool.Factors.OptimalDepth = CS * (stressFactorsSchool.Factors.OptimalDepth + hungerFactorsSchool.Factors.OptimalDepth + depthFactorsSchool.Factors.OptimalDepth);
        lambdaSchool.HoldDistanceToFish = CS * (stressFactorsSchool.HoldDistanceToFish + hungerFactorsSchool.HoldDistanceToFish + depthFactorsSchool.HoldDistanceToFish);
    }

    #region Hunger factors
    private void CalculateHungerFactorsAlone() {
        if (_fish.Hunger <= 0)
        { 
            hungerFactorsAlone.FindFood = 2;
        }
        else
        { 
            hungerFactorsAlone.FindFood = _aloneScale / (_fish.Hunger / Fish.maxHunger * 100f);
        }
        if (hungerFactorsAlone.FindFood > 2f)
            hungerFactorsAlone.FindFood = 2f;
        float leftOfHungerFactor = 2f - hungerFactorsAlone.FindFood;

        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            hungerFactorsAlone.PrevDirection = leftOfHungerFactor * 0.5f;
            hungerFactorsAlone.SwimWithOrToFish = leftOfHungerFactor * 0.4f;
            hungerFactorsAlone.CollisionDodge = 0;
            hungerFactorsAlone.OptimalDepth = leftOfHungerFactor * 0.1f;
        }
        else
        {
            hungerFactorsAlone.FindFood = 0;
            hungerFactorsAlone.PrevDirection = 0;
            hungerFactorsAlone.SwimWithOrToFish = 0;
            hungerFactorsAlone.CollisionDodge = 2;
            hungerFactorsAlone.OptimalDepth = 0;
        }
    }
    private void CalculateHungerFactorsSchool()
    {
        if (_fish.Hunger <= 0)
        {
            hungerFactorsSchool.Factors.FindFood = 2.5f;
        } else
        {
            hungerFactorsSchool.Factors.FindFood = _schoolScale / (_fish.Hunger / Fish.maxHunger * 100);
        }

        if (hungerFactorsSchool.Factors.FindFood > 2.5f)
            hungerFactorsSchool.Factors.FindFood = 2.5f;

        float leftOfHungerFactor = 2.5f - hungerFactorsSchool.Factors.FindFood;
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            hungerFactorsSchool.Factors.PrevDirection = leftOfHungerFactor * 0.2f;
            hungerFactorsSchool.Factors.SwimWithOrToFish = leftOfHungerFactor * 0.3f;
            hungerFactorsSchool.Factors.CollisionDodge = 0;
            hungerFactorsSchool.Factors.OptimalDepth = leftOfHungerFactor * 0.1f;
            hungerFactorsSchool.HoldDistanceToFish = leftOfHungerFactor * 0.4f;
        }
        else
        {
            hungerFactorsSchool.Factors.FindFood = 0;
            hungerFactorsSchool.Factors.PrevDirection = 0;
            hungerFactorsSchool.Factors.SwimWithOrToFish = 0;
            hungerFactorsSchool.Factors.CollisionDodge = 2.5f;
            hungerFactorsSchool.Factors.OptimalDepth = 0;
            hungerFactorsSchool.HoldDistanceToFish = 0;
        }
    }
    #endregion

    #region stress Factors
    private void CalculateStressFactorsAlone() {
        if(_fish.Hunger <= 0)
        {
            stressFactorsAlone.FindFood = 2;
        } else
        {
            stressFactorsAlone.FindFood = _aloneScale / (_fish.Hunger / Fish.maxHunger * 100f);
        }


        if (stressFactorsAlone.FindFood > 2f)
            stressFactorsAlone.FindFood = 2f;
        float leftOfStressFactor = 2f - stressFactorsAlone.FindFood;
       
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            stressFactorsAlone.PrevDirection = leftOfStressFactor * 0.6f;
            stressFactorsAlone.SwimWithOrToFish = leftOfStressFactor * 0.3f;
            stressFactorsAlone.CollisionDodge = 0;
            stressFactorsAlone.OptimalDepth = leftOfStressFactor * 0.1f;
        }
        else
        {
            stressFactorsAlone.PrevDirection = 0;
            stressFactorsAlone.FindFood = 0;
            stressFactorsAlone.SwimWithOrToFish = 0;
            stressFactorsAlone.CollisionDodge = 2;
            stressFactorsAlone.OptimalDepth = 0;
        }       
    }

    private void CalculateStressFactorsSchool()
    {
        if(_fish.Hunger <= 0)
        {
            stressFactorsSchool.Factors.FindFood = 2.5f;
        } else
        {
            stressFactorsSchool.Factors.FindFood = _schoolScale / (_fish.Hunger / Fish.maxHunger * 100);
        }

        if (stressFactorsSchool.Factors.FindFood > 2.5f)
            stressFactorsSchool.Factors.FindFood = 2.5f;

        float leftOfStressFactor = 2.5f - stressFactorsSchool.Factors.FindFood;
        //if there is no object in the way
        if (!_isObstacleDetected)
        {
            stressFactorsSchool.Factors.PrevDirection = leftOfStressFactor * 0.2f;
            stressFactorsSchool.Factors.SwimWithOrToFish = leftOfStressFactor * 0.4f;
            stressFactorsSchool.Factors.CollisionDodge = 0;
            stressFactorsSchool.Factors.OptimalDepth = leftOfStressFactor * 0.1f;
            stressFactorsSchool.HoldDistanceToFish = leftOfStressFactor * 0.3f;
        }
        else
        {
            stressFactorsSchool.Factors.PrevDirection = 0;
            stressFactorsSchool.Factors.FindFood = 0;
            stressFactorsSchool.Factors.SwimWithOrToFish = 0;
            stressFactorsSchool.Factors.CollisionDodge = 2.5f;
            stressFactorsSchool.Factors.OptimalDepth = 0;
            stressFactorsSchool.HoldDistanceToFish = 0;
        }        
    }
    #endregion

    #region Depth Factors

    private void SetDepthFactorsAlone()
    {
        depthFactorsAlone.OptimalDepth = Mathf.Abs(transform.position.y) / _cage.transform.lossyScale.y / _depthScale;
        float theRest = (1 - depthFactorsAlone.OptimalDepth) / _depthDividerAlone;
        depthFactorsAlone.PrevDirection = theRest;
        depthFactorsAlone.FindFood = theRest;
        depthFactorsAlone.SwimWithOrToFish = theRest;
        depthFactorsAlone.CollisionDodge = theRest;
    }

    private void SetDepthFactorsSchool()
    {
        depthFactorsSchool.Factors.OptimalDepth = Mathf.Abs(transform.position.y) / _cage.transform.lossyScale.y / _depthScale;
        float theRest = (1 - depthFactorsSchool.Factors.OptimalDepth) / _depthDividerSchool;
        depthFactorsSchool.Factors.PrevDirection = theRest;
        depthFactorsSchool.Factors.FindFood = theRest;
        depthFactorsSchool.Factors.SwimWithOrToFish = theRest;
        depthFactorsSchool.Factors.CollisionDodge = theRest;
        depthFactorsSchool.HoldDistanceToFish = theRest;
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

    private void AnimateDeath()
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
    private Vector3 GetClosestFoodPoint()
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

    private Vector3 cantSeeFood()
    {
        Vector3 D2 = transform.position;
        if (lastKnownFoodSpotsVec2.Count <= 0)
            return new Vector3();
        else
            D2 = GetClosestPointVec2(lastKnownFoodSpotsVec2);

        if (MathTools.GetDistanceBetweenVectors(D2, transform.position) < 1f)
        {
            _removePointTimer += Time.deltaTime;
        }

        if (_removePointTimer >= _pointInterval)
        {
            _removePointTimer = 0;
            _savedKnownFoodSpots.Add(D2);
            lastKnownFoodSpotsVec2.Remove(new Vector2(D2.x, D2.z));
        }

        if (lastKnownFoodSpotsVec2.Count <= 0) {
            foreach (Vector3 point in _savedKnownFoodSpots)
            {
                lastKnownFoodSpotsVec2.Add(new Vector2 (point.x, point.z));
            }
            _savedKnownFoodSpots.Clear();
        }

        return (D2 - transform.position);
    }

    private Vector3 GetClosestPointVec2(List<Vector2> arrayOfPoints)
    {
        Vector3 closestPoint = arrayOfPoints[0];
        Vector3 placeholderpoint = new Vector3();
        foreach (Vector2 point in arrayOfPoints)
        {
            placeholderpoint = new Vector3(point.x, transform.position.y, point.y);
            if (MathTools.GetDistanceBetweenVectors(placeholderpoint, transform.position)
                < MathTools.GetDistanceBetweenVectors(transform.position, closestPoint))
            {
                closestPoint = placeholderpoint;
            }
        }
        return closestPoint;
    }

    #endregion

    #region Swim towards other fish
    private Vector3 SwimTowardsOtherFish() {
        Vector3 D_3 = new Vector3(0, 0, 0);
        float distanceBetweenFish;
        foreach (FishBehaviour fish in _nearbyFish)
        {
            distanceBetweenFish = MathTools.GetDistanceBetweenVectors(transform.position, fish.transform.position) / _nearbyFish.Count;
            D_3 += distanceBetweenFish * (fish.transform.position - transform.position);
        }
        return D_3;
    }
    #endregion

    #region Swim with fish
    private Vector3 SwimWithFriends() {
        Vector3 followClosest = new Vector3();
        if (_nearbyFish.Count > 0)
        {
            FishBehaviour closestFish = _nearbyFish[0];
            foreach (FishBehaviour fish in _nearbyFish) {
                if (MathTools.GetDistanceBetweenVectors(transform.position, fish.transform.position) <
                    MathTools.GetDistanceBetweenVectors(transform.position, closestFish.transform.position))
                { 
                    closestFish = fish;
                    followClosest = fish._fish.DesiredPoint;
                }
            }
        }
        return followClosest;
    }
    #endregion

    #region Hold distance to fish
    private Vector3 HoldDistanceToFish()
    {
        Vector3 HoldDistanceVector = new Vector3();
        Vector3 closestFish = new Vector3();
        if (_nearbyFish.Count > 0)
            closestFish = _nearbyFish[0].transform.position;
        foreach (FishBehaviour item in _nearbyFish)
        {
            if (MathTools.GetDistanceBetweenVectors(transform.position, item.transform.position) <
                         MathTools.GetDistanceBetweenVectors(transform.position, closestFish))
            {
                closestFish = item.transform.position;
            }
        }

        float distanceBetweenFish = MathTools.GetDistanceBetweenVectors(closestFish, transform.position);
        if (distanceBetweenFish < 0.52f) // too close so it makes it go away.
        {
            float distanceFactor = (1 / Mathf.Sin(_holdDistanceScale * distanceBetweenFish)) - 1;
            float negativeDistanceFactor = (-1) * (distanceFactor);
            HoldDistanceVector.x = negativeDistanceFactor * (closestFish.x - transform.position.x);
            HoldDistanceVector.y = negativeDistanceFactor * (closestFish.y - transform.position.y);
            HoldDistanceVector.z = negativeDistanceFactor * (closestFish.z - transform.position.z);
        }
        else if (distanceBetweenFish < 0.86f) //too far away so it makes it go closer.
        {
            float distanceFactor = (1 / Mathf.Sin(_holdDistanceScale * distanceBetweenFish)) - 1;
            HoldDistanceVector.x = (distanceFactor) * (closestFish.x - transform.position.x) ;
            HoldDistanceVector.y = (distanceFactor) * (closestFish.y - transform.position.y) ;
            HoldDistanceVector.z = (distanceFactor) * (closestFish.z - transform.position.z) ;
        }

        return HoldDistanceVector;
    }
    #endregion

    #region Search for optimal depth
    private Vector3 SearchForOptimalDepth() {
        return new Vector3(transform.position.x, 0 - transform.position.y, transform.position.z);
    }
    #endregion

    #region Get new direction
    private Vector3 GetNewPosition()
    {
        if (_fish.IsDead)
            return new Vector3(0,0,0);

        bool isSchooling = IsSchooling();
        bool isThereNearbyFood = _nearbyFood.Count > 0;
        Vector3 returnVector = new Vector3();

        directions.PreviousPoint = (_fish.DesiredPoint + _uniqueOffset);
       
        directions.OptimalDepthDirection = SearchForOptimalDepth();
        
        if (isSchooling)
        {
            CalculateLambdaSchool();
            directions.SwimWithOrToFish = SwimWithFriends();
            directions.HoldDistanceToFishDirection = HoldDistanceToFish();
            if (isThereNearbyFood) {
                directions.FindFoodDirection = GetClosestFoodPoint();
                returnVector = directions.PreviousPoint * lambdaSchool.Factors.PrevDirection + directions.FindFoodDirection * lambdaSchool.Factors.FindFood + directions.SwimWithOrToFish * lambdaSchool.Factors.SwimWithOrToFish
                    + directions.DodgeCollisionDirection * lambdaSchool.Factors.CollisionDodge + directions.OptimalDepthDirection * lambdaSchool.Factors.OptimalDepth + directions.HoldDistanceToFishDirection * lambdaSchool.HoldDistanceToFish;
            }
            else {
                directions.FindFoodDirection = cantSeeFood();

                returnVector = directions.PreviousPoint * lambdaSchool.Factors.PrevDirection + directions.FindFoodDirection * lambdaSchool.Factors.FindFood + directions.SwimWithOrToFish * lambdaSchool.Factors.SwimWithOrToFish
                    + directions.DodgeCollisionDirection * lambdaSchool.Factors.CollisionDodge + directions.OptimalDepthDirection * lambdaSchool.Factors.OptimalDepth + directions.HoldDistanceToFishDirection * lambdaSchool.HoldDistanceToFish;
            }
        }
        else {
            CalculateLambdaAlone();
            directions.SwimWithOrToFish = SwimTowardsOtherFish();
            if (isThereNearbyFood)
            {
                directions.FindFoodDirection = GetClosestFoodPoint();
                returnVector = directions.PreviousPoint * lambdaAlone.PrevDirection + directions.FindFoodDirection * lambdaAlone.FindFood
                    + directions.SwimWithOrToFish * lambdaAlone.SwimWithOrToFish + directions.DodgeCollisionDirection * lambdaAlone.CollisionDodge + directions.OptimalDepthDirection * lambdaAlone.OptimalDepth;
            }
            else 
            {
                directions.FindFoodDirection = cantSeeFood();
                returnVector = directions.PreviousPoint * lambdaAlone.PrevDirection + directions.FindFoodDirection * lambdaAlone.FindFood
                    + directions.SwimWithOrToFish * lambdaAlone.SwimWithOrToFish + directions.DodgeCollisionDirection * lambdaAlone.CollisionDodge + directions.OptimalDepthDirection * lambdaAlone.OptimalDepth;
            }
        }
        if (!_isObstacleDetected)
        {
            directions.DodgeCollisionDirection = new Vector3();
        }

        return returnVector;
    }

    private bool IsSchooling()
    {
        foreach (FishBehaviour fish in _nearbyFish)
        {
            if (MathTools.GetDistanceBetweenVectors(transform.position, fish.transform.position) < 0.86f)
                return true;
        }
        return false;
    }
    #endregion


    public void SetPositionOnSpawn() {
        transform.position = new Vector3(Random.value * Mathf.Sin(MathTools.DegreeToRadian(45))*_cage.transform.lossyScale.x , Random.value * _cage.transform.lossyScale.y/4f, Random.value * Mathf.Cos(MathTools.DegreeToRadian(45)) * _cage.transform.lossyScale.z);
        if (Random.value < 0.5f)
            transform.position = new Vector3(transform.position.x * (-1), transform.position.y, transform.position.z);
        if (Random.value < 0.5f)
            transform.position = new Vector3(transform.position.x, transform.position.y * (-1), transform.position.z);
        if (Random.value < 0.5f)
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z * (-1));
    }
}
