using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    private Fish _fish;
    public Fish Fish { set { if (value != null) _fish = value; } }
    private DataManager _dataManager;
    public DataManager DataManager { set { if (value != null) _dataManager = value; } }
    private MathTools _mathTools;
    private bool isFishDeadAlready = false;
    public Vector3 sumVector;
    Vector3 newdir;
    private void Start()
    {
        Debug.Log("Fish spawned");
        _mathTools = this.GetComponent<MathTools>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(sumVector != null)
        //{
        //    Vector3 newdir = Vector3.RotateTowards(transform.forward, sumVector, Time.deltaTime, 2.5f);
        //    transform.rotation = Quaternion.LookRotation(newdir);
        //}
        AnimateDeath();

    }
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
