using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

[RequireComponent(typeof(MathTools))]
public class FishBehaviour : MonoBehaviour
{
    private Fish _fish;
    public Fish Fish { set { if (value != null) _fish = value; } }
    private MathTools _mathTools;

    // Stress variables
    private Timer _stressTimer;
    private const float _stressMultiplier = 0.5f;
    private const float _stressDuration = 30f; // In seconds

    public Vector3 sumVector;
    private void Start()
    {
        Debug.Log("Fish spawned");
        _mathTools = this.GetComponent<MathTools>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_fish.IsDead)
            return;

        //if(sumVector != null)
        //{
        //    Vector3 newdir = Vector3.RotateTowards(transform.forward, sumVector, Time.deltaTime, 2.5f);
        //    transform.rotation = Quaternion.LookRotation(newdir);
        //}

        UpdateStress();
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
    public void Die()
    {

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
