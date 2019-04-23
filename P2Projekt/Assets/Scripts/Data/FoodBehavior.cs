using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private Food _food;
    public Food Food { get {  return _food; } set { if (value != null) _food = value; } }
    private DataManager _dataManager;
    public DataManager DataManager { set { if (value != null) _dataManager = value; } }
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = new Vector30(Random.value*(-20), Random.value*(-20), Random.value*(-20));
        transform.position = new Vector3(0, -2, 0);
        DataManager = FindObjectOfType<DataManager>();
        _dataManager.foodList.Add(_food);
    }

    public void BeingEaten()
    {
        _food.Health--;
        _dataManager.RemoveFood(_food);
        transform.gameObject.SetActive(false);
    }
}
