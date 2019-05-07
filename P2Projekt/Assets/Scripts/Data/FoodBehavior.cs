using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private Food _food;
    public Food Food { get {  return _food; } set { if (value != null) _food = value; } }
    private DataManager _dataManager;
    private DataManager DataManager { set { if (value != null) _dataManager = value; } }
    private void Start()
    {
        bool xPos = (Random.value < 0.5f);
        bool yPos = (Random.value < 0.5f);
        bool zPos = (Random.value < 0.5f);
       
        DataManager = FindObjectOfType<DataManager>();
        _dataManager.foodList.Add(_food);

        transform.position = new Vector3(Mathf.Cos(MathTools.RadianToDegree(Random.value)) * _dataManager.UI.Cage.gameObject.transform.lossyScale.x,
                                         Random.value * _dataManager.UI.Cage.gameObject.transform.lossyScale.y / 4,
                                         Mathf.Sin(MathTools.RadianToDegree(Random.value)) * _dataManager.UI.Cage.gameObject.transform.lossyScale.z);
        if (xPos)
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        if (yPos)
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
        if (zPos)
            transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z);
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
