using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    private DataManager _DM;

    private void Start()
    {
        _DM = FindObjectOfType<DataManager>();
    }

    public void GetPointClicked() {
        Vector2 cursorPosition = Input.mousePosition;
        cursorPosition.y -= 1030;
        cursorPosition.x -= 50;
        SpawnFoodAtPoint(cursorPosition);
    }

    public void SpawnFoodAtPoint(Vector2 point) {
        _DM.AddFoodToCage(1,5, point);
    }
}
