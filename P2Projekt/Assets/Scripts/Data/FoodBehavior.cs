using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private Food _food;
    public Food Food { get {  return _food; } set { if (value != null) _food = value; } }
    private DataManager _dataManager;
    private DataManager DataManager { set { if (value != null) _dataManager = value; } }
    private float _scalingFactor;
    private void Start()
    {
        _scalingFactor = transform.lossyScale.x;
        transform.localScale = new Vector3(_scalingFactor *_food.Health, _scalingFactor *_food.Health, _scalingFactor *_food.Health);
        DataManager = FindObjectOfType<DataManager>();
        transform.position = new Vector3((_food.position.x*2)/100* _dataManager.UI.Cage.gameObject.transform.lossyScale.x, _dataManager.UI.Cage.gameObject.transform.lossyScale.y / 4, (_food.position.y*2)/100* _dataManager.UI.Cage.gameObject.transform.lossyScale.z);
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y-0.2f*Time.deltaTime, transform.position.z);
        if(transform.position.y < -20)
        {
            _dataManager.RemoveFood(_food);
            _dataManager.wastedFoodCounter += _food.Health;
            transform.gameObject.SetActive(false);
        }
    }
    public void BeingEaten()
    {
        _food.Health--;
        transform.localScale = new Vector3(transform.localScale.x-_scalingFactor, transform.localScale.y - _scalingFactor, transform.localScale.z- _scalingFactor);
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
