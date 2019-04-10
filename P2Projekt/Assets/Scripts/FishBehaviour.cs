﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    private Fish _fish;
    public Fish Fish { set { if (value != null) _fish = value; } }
    private MathTools _mathTools;

    public Vector3 sumVector;
    private void Start()
    {
        Debug.Log("Fish spawned");
        _mathTools = this.GetComponent<MathTools>();
    }

    // Update is called once per frame
    void Update()
    {
        if(sumVector != null)
        {
            Vector3 newdir = Vector3.RotateTowards(transform.forward, sumVector, Time.deltaTime, 2.5f);
            transform.rotation = Quaternion.LookRotation(newdir);
        }
    }
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
