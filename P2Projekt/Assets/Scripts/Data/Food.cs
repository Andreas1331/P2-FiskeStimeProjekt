﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food
{
    public int Health { get; set; }
    public int Id { get; set; }
    public GameObject FoodObject { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Food(int id, GameObject preFab)
    {
        Id = id;
        //FoodObject = GameObject.Instantiate(preFab, new Vector3(), Quaternion.identity);
        //FoodObject.GetComponent<FoodBehavior>().Food = this;
    }
    
}
