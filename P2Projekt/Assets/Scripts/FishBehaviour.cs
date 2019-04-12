using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

[RequireComponent(typeof(MathTools))]
[RequireComponent(typeof(Rigidbody))]
public class FishBehaviour : MonoBehaviour
{
    private Fish _fish;
    public Fish Fish { set { if (value != null) _fish = value; } }
    private DataManager _dataManager;
    public DataManager DataManager { set { if (value != null) _dataManager = value; } }
    private MathTools _mathTools;
    public Vector3 sumVector;
    Vector3 newdir;
    public float gotDistance = 0;
    // Stress variables
    private Timer _stressTimer;
    private const float _stressMultiplier = 0.5f;
    private const float _stressDuration = 30f; // In seconds
    private int innerColliderFoodCheck = 0;
    public List<Vector3> lastKnownFoodSpots = new List<Vector3>();
    public Dictionary<int, Vector3> knownFoodSpots = new Dictionary<int, Vector3>();
    //grimt workaround dictionary
    public Dictionary<int, Vector3> inInnerCollider = new Dictionary<int, Vector3>();
    private void Awake()
    {
        //_fish.IsDead = false;
        _mathTools = this.GetComponent<MathTools>();
        DataManager = FindObjectOfType<DataManager>();
        _dataManager.fishList.Add(_fish);
        transform.position = new Vector3(0, Random.value *(-20f), 0);
    }

    private void Start()
    {
        //Debug.Log("Fish spawned");
    }

    // Update is called once per frame
    private void Update()
    {
        //if(sumVector != null)
        //{
        //    Vector3 newdir = Vector3.RotateTowards(transform.forward, sumVector, Time.deltaTime, 2.5f);
        //    transform.rotation = Quaternion.LookRotation(newdir);
        //}
        AnimateDeath();
        

        UpdateStress();
        UpdateHunger();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with other object: " + other.name);
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
            if (knownFoodSpots.ContainsKey(other.GetComponent<FoodBehavior>().Food.Id))
                knownFoodSpots.Remove(other.GetComponent<FoodBehavior>().Food.Id);
            //grimt workaround
            if (inInnerCollider.ContainsKey(other.GetComponent<FoodBehavior>().Food.Id))
            {
                inInnerCollider.Remove(other.GetComponent<FoodBehavior>().Food.Id);
                knownFoodSpots.Add(other.GetComponent<FoodBehavior>().Food.Id, other.transform.position);
            }
        }
            //Debug.Log("Object left the vicinity.. ");
    }

    private void HandleSpottedObject(Collider other)
    {
        // Check if the object detected is another fish, or an obstacle.
        if (other.tag.Equals("Fish"))
        {

        }
        else if (other.tag.Equals("Obstacle"))
        {
            float angle = _mathTools.GetAngleBetweenVectors(_fish.CurrentDirection, other.transform.position);
            float dist = _mathTools.GetDistanceBetweenVectors(_fish.CurrentDirection, other.transform.position);
            float catheter = _mathTools.GetOpposingCatheter(angle, dist);

            Debug.Log("Angle: " + angle + " | Distance: " + dist + " | Catheter: " + catheter);
        }
        else if (other.tag.Equals("Food"))
        {
            if (!knownFoodSpots.ContainsKey(other.GetComponent<FoodBehavior>().Food.Id)) { 
                knownFoodSpots.Add(other.GetComponent<FoodBehavior>().Food.Id, other.transform.position);
                lastKnownFoodSpots.Add(other.transform.position);
            }
            else
            {
                _fish.Hunger = 1000;
                other.GetComponent<FoodBehavior>().BeingEaten();
                Debug.Log("Fisken spiste");
                //grimt workaround
                knownFoodSpots.Remove(other.GetComponent<FoodBehavior>().Food.Id);
            }
        }
    }

    private void UpdateHunger()
    {
        _fish.Hunger -= 1 * Time.deltaTime;
        if(_fish.Hunger <= 0)
        {
            // Kill fish.        
            //Debug.Log("Fish has died due to hunger..");
        }
    }

    private void UpdateStress()
    {
        // Increase or lower the stress based on the fish hunger.
        if (_fish.Hunger <= 500 && _fish.Hunger > 300)
            _fish.Stress += 1 * _stressMultiplier * Time.deltaTime;
        else if (_fish.Hunger <= 300)
            _fish.Stress += 1 * (_stressMultiplier * 2) * Time.deltaTime;
        else if (_fish.Stress > 0)
            _fish.Stress -= 1 * Time.deltaTime;

        // Start the timer if the fish is stressed.
        if (_fish.Stress >= 900)
        {
            if (!IsStressTimerRunning())
                StartStressTimer();
        }
        // Stress is less than 900. Check if the timer is running.
        else if (IsStressTimerRunning())
        {
            ResetStressTimer();
        }
    }

    #region Stress handler
    private void StartStressTimer()
    {
        _stressTimer = new Timer();
        _stressTimer.Interval = _stressDuration * 1000;
        _stressTimer.Elapsed += StressTimerElapsed;
        _stressTimer.AutoReset = false;
        _stressTimer.Enabled = true;
    }

    private void ResetStressTimer()
    {
        if(_stressTimer != null)
        {
            _stressTimer.Enabled = false;
        }
    }

    private bool IsStressTimerRunning()
    {
        return _stressTimer?.Enabled ?? false;
    } 

    private void StressTimerElapsed(object source, ElapsedEventArgs e)
    {
        // Check if stress is more than 900.
        if(_fish.Stress >= 900)
        {
            // Should call proper Kill() method instead that handles this.
            _fish.IsDead = true;
        }
    }
    #endregion

    #region Die Methods
    //DIE method ------------------------------------------------------------------START
    private void KillFish()
    {
        _dataManager.RemoveFish(_fish);
    }

    public void AnimateDeath()
    {
        //if (!_fish.IsDead)
        //    return;
        //Rotation of fish around the z-axis
        if (transform.rotation.x > -0.7f)
        {
            newdir = Vector3.RotateTowards(transform.forward, new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime, 2.5f);
            transform.rotation = Quaternion.LookRotation(newdir);
            //Debug.Log(transform.rotation.x);
            //transform.RotateAround(transform.position, Vector3.forward, 10 * Time.deltaTime);
        }
        else if (transform.rotation.x <= -0.7f && transform.position.y < 10)
        {
            //transform.RotateAround(transform.position, Vector3.forward, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y + 5 * Time.deltaTime, transform.position.z);
            //Debug.Log("1");

            if (gotDistance == 0)
            {
                gotDistance = -transform.position.y;
            }

            //MakeOpague;
        }
        else {
            //Debug.Log("Er dissabled nu");
            transform.position = new Vector3(-5000.0f,-5000.0f, -5000.0f);
            this.transform.gameObject.SetActive(false);
        }
    }
    //DIE method ------------------------------------------------------------------END

    void MakeOpague()
    {
    }





    #endregion

    #region Food Methods
    //D_2,t (FOOD) methods --------------------------------------------------------START
    public Vector3 canSeeFood(Dictionary<int, Vector3> knownFoodPositions)
    {
        Vector3 closestFood = new Vector3(100,100,100);
        //Iterate through list of food nearby, and choose the closest one. 
        foreach (KeyValuePair<int, Vector3> item in knownFoodPositions) {
            if (Mathf.Sqrt(Mathf.Pow(item.Value.x - this.transform.position.x, 2) + Mathf.Pow(item.Value.y - this.transform.position.y, 2) + Mathf.Pow(item.Value.z - this.transform.position.z, 2))
                <Mathf.Sqrt(Mathf.Pow(closestFood.x, 2) + Mathf.Pow(closestFood.y, 2) + Mathf.Pow(closestFood.z, 2))) {
                closestFood = item.Value;
            }
        }
        return closestFood;
    }

    public Vector3 cantSeeFood(List<Vector3> listOfLastKnownEatingSpots)
    {
        Vector3 sumVecD3 = new Vector3();
        float factor;

        if(listOfLastKnownEatingSpots.Count == 0)
        {
            return transform.position;
        }

        foreach (Vector3 vec in listOfLastKnownEatingSpots)
        {
            factor = (1 / (_mathTools.GetDistanceBetweenVectors(vec, this.transform.position)));
                
            sumVecD3 += factor * (vec - this.transform.position);
        }
        return sumVecD3;
    }
    //D_2,t (FOOD) methods --------------------------------------------------------END
    #endregion

    #region Get new direction
    private Vector3 GetNewDirection()
    {
        Vector3 foodVector =  new Vector3(0, 5000, 0); 


        return new Vector3(0,0,0);
    }
    #endregion
}
