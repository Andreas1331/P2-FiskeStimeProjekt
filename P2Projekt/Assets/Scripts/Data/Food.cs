using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food
{
    public int Health { get; set; }
    public int Id { get; set; }
    public GameObject FoodObject { get; set; }


    public Food(int id, GameObject preFab, int amountOfFood)
    {
        Id = id;
        Health = amountOfFood;
        FoodObject = GameObject.Instantiate(preFab, new Vector3(), Quaternion.identity);
        FoodObject.GetComponent<FoodBehavior>().Food = this;
    }
}
