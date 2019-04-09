using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    private Fish _fish;
    public Fish Fish { set { if (value != null) _fish = value; } }

    public Vector3 sumVector;
    private void Start()
    {
        Debug.Log("Fish spawned");
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
}
