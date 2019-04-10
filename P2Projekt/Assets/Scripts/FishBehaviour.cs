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

    // Stress variables
    private Timer _stressTimer;
    private const float _stressMultiplier = 0.5f;
    private const float _stressDuration = 30f; // In seconds

    private void Awake()
    {
        _mathTools = this.GetComponent<MathTools>();
        DataManager = FindObjectOfType<DataManager>();
    }

    private void Start()
    {
        Debug.Log("Fish spawned");
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
        Debug.Log("Collided with other object: " + other.name);
        HandleSpottedObject(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Object is nearby .. ");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Object left the vicinity.. ");
    }

    private void HandleSpottedObject(Collider other)
    {
        // Check if the object detected is another fish, or an obstacle.
        if (other.tag.Equals("Fish"))
        {

        } else if (other.tag.Equals("Obstacle"))
        {

        }
    }

    private void UpdateHunger()
    {
        _fish.Hunger -= 1 * Time.deltaTime;
        if(_fish.Hunger <= 0)
        {
            // Kill fish.        
            Debug.Log("Fish has died due to hunger..");
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

    //DIE method ------------------------------------------------------------------START
    private void KillFish()
    {
        _dataManager.RemoveFish(_fish);
    }

    public void AnimateDeath()
    {
        if (!_fish.IsDead)
            return;

        //Rotation of fish around the z-axis

        if (transform.rotation.x > -0.7f)
        {
            newdir = Vector3.RotateTowards(transform.forward, new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime, 2.5f);
            transform.rotation = Quaternion.LookRotation(newdir);
            Debug.Log(transform.rotation.x);
            //transform.RotateAround(transform.position, Vector3.forward, 10 * Time.deltaTime);
        }
        else if (transform.rotation.x <= -0.7f && transform.position.y < 0)
        {
            //transform.RotateAround(transform.position, Vector3.forward, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y + 5 * Time.deltaTime, transform.position.z);
            Debug.Log("1");
        }
        else {
            Debug.Log("Er dissabled nu");
            transform.position = new Vector3(-5000.0f,-5000.0f, -5000.0f);
            this.transform.gameObject.SetActive(false);
        }
        
    }
    //DIE method ------------------------------------------------------------------END

    //D_3,t (FOOD) methods --------------------------------------------------------START
    public Vector3 canSeeFood(Vector3 knownFoodPosition)
    {
        float x = knownFoodPosition.x - this.transform.position.x;
        float y = knownFoodPosition.y - this.transform.position.y;
        float z = knownFoodPosition.z - this.transform.position.z;

        return new Vector3(x, y, z);
    }

    public Vector3 cantSeeFood(List<Vector3> listOfLastKnownEatingSpots)
    {
        Vector3 sumVecD3 = new Vector3();
        float factor;

        if(listOfLastKnownEatingSpots.Count == 0)
        {
            return new Vector3(0,0,0);
        }

        foreach (Vector3 vec in listOfLastKnownEatingSpots)
        {
            factor = (1 / (_mathTools.GetDistanceBetweenVectors(vec, this.transform.position)));
                
            sumVecD3 += factor * (vec - this.transform.position);
        }
        return sumVecD3;
    }    
    //D_3,t (FOOD) methods --------------------------------------------------------END
}
