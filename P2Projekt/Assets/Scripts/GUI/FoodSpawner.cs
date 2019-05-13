using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FoodSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject foodPrefab;
    private DataManager _DM;
    void Start()
    {
        _DM = FindObjectOfType<DataManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void GetPointClicked() {
        Vector2 cursorPosition = Input.mousePosition;
        cursorPosition.y -= 1030;
        cursorPosition.x -= 50;
        SpawnFoodAtPoint(cursorPosition);

    }

    public void SpawnFoodAtPoint(Vector2 point) {
        _DM.AddFoodToCage(1,20, point);
    }

}
