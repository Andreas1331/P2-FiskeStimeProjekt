using UnityEngine;

public class Food
{
    public int Health { get; set; }
    public int Id { get; set; }
    public GameObject FoodObject { get; set; }
    public Vector2 position;

    public Food(int id, GameObject preFab, int amountOfFood, Vector2 position)
    {
        Id = id;
        Health = amountOfFood;
        this.position = position;
        FoodObject = GameObject.Instantiate(preFab, new Vector3(), Quaternion.identity, GameObject.FindGameObjectWithTag("FoodContainer").transform);
        FoodObject.GetComponent<FoodBehavior>().Food = this;
    }
}
