﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private Food _food;
    public Food Food { get {  return _food; } set { if (value != null) _food = value; } }
    private DataManager _dataManager;
    private DataManager DataManager { set { if (value != null) _dataManager = value; } }
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = new Vector30(Random.value*(-20), Random.value*(-20), Random.value*(-20));
        bool xPos = (Random.value < 0.5f);
        bool yPos = (Random.value < 0.5f);
        bool zPos = (Random.value < 0.5f);
        
        transform.position = new Vector3(Random.value* _dataManager.UI.Cage.gameObject.transform.lossyScale.x / 3.75f, Random.value * _dataManager.UI.Cage.gameObject.transform.lossyScale.y / 3.75f, Random.value * _dataManager.UI.Cage.gameObject.transform.lossyScale.z / 3.75f);
        if (xPos)
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        if (yPos)
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
        if (zPos)
            transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z);


        DataManager = FindObjectOfType<DataManager>();
        _dataManager.foodList.Add(_food);
    }

    public void BeingEaten()
    {
        _food.Health--;

        if(_food.Health <= 0)
        {
            _dataManager.RemoveFood(_food);
            transform.gameObject.SetActive(false);

            foreach(Fish fish in _dataManager.fishList)
            {
                FishBehaviour fishBehav = fish.FishObject.GetComponent<FishBehaviour>();
                if(fishBehav != null)
                {
                    fishBehav.EatFood(this);
                }
            }
        }
    }
}
