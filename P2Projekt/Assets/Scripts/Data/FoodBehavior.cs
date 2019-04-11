using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private Food _food;
    public Food Food { set { if (value != null) _food = value; } }
    private DataManager _dataManager;
    public DataManager DataManager { set { if (value != null) _dataManager = value; } }
    // Start is called before the first frame update
    void Start()
    {
        _dataManager.foodList.Add(_food);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BeingEaten()
    {
        _food.Health--;
        
        if (_food.Health == 0)
        {
            _dataManager.RemoveFood(_food);
        }
    }
}
