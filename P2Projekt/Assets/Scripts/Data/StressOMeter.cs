using UnityEngine;

public class StressOMeter : MonoBehaviour
{
    private DataManager _dataManager;
    private DataManager DataManager { set { if (value != null) _dataManager = value; } }
    private float updatetimer;

    private void Start()
    {
        DataManager = FindObjectOfType<DataManager>();
    }

    private void Update()
    {
        updatetimer += Time.deltaTime;

        if (updatetimer > 0.5f)
        {
            updatetimer = 0;
            UpdateStressOMeter();
        }
    }

    private void UpdateStressOMeter() {
        if (_dataManager.StressSum != 0)
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(30,(_dataManager.StressSum/Fish.maxStress*100f)- 100.0f);
    }
}
