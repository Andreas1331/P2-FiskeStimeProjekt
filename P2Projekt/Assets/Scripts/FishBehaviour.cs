using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    private Fish _fish;
    public Fish Fish { set { if (value != null) _fish = value; } }

    // Stress variables
    private Timer _stressTimer;
    private const float _stressMultiplier = 0.5f;

    public Vector3 sumVector;
    private void Start()
    {
        Debug.Log("Fish spawned");
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
        Debug.LogWarning("Stress: " + Mathf.Round(_fish.Stress));
        if (Input.GetKeyDown(KeyCode.W))
            _fish.Hunger -= 100;
        else if (Input.GetKeyDown(KeyCode.S))
            Debug.Log("Hunger: " + _fish.Hunger);
    }

    private void UpdateStress()
    {
        // If the stress level is higher than 0, deduct 1 every second.
        if(_fish.Stress < 1000)
        {
            _fish.Stress += 50 * Time.deltaTime;
            return;

            // Increase or lower the stress based on the fish hunger.
            if (_fish.Hunger <= 500 && _fish.Hunger > 300)
                _fish.Stress += 1 * _stressMultiplier * Time.deltaTime;
            else if (_fish.Hunger <= 300)
                _fish.Stress += 1 * (_stressMultiplier * 2) * Time.deltaTime;
            else if(_fish.Stress > 0)
                _fish.Stress -= 1 * Time.deltaTime;

            // Start the timer if the fish is stressed.
            if(_fish.Stress >= 900)
            {
                if(!IsStressTimerRunning())
                    StartStressTimer();
            }
            // Stress is less than 900. Check if the timer is running.
            else if (IsStressTimerRunning())
            {
                ResetStressTimer();
            }
        }
    }

    #region Stress handler
    private void StartStressTimer()
    {
        _stressTimer = new Timer();
        _stressTimer.Interval = 30 * 1000;
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
            _fish.IsDead = true;
        }
    }
    #endregion
}
