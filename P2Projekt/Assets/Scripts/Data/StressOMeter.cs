using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressOMeter : MonoBehaviour
{
    private DataManager _dataManager;
    private DataManager DataManager { set { if (value != null) _dataManager = value; } }
    // Start is called before the first frame update
    private float updatetimer;
    void Start()
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
            Debug.Log("maxstress = " +Fish.maxStress);
            Debug.Log(_dataManager.StressSum);
        }
    }
    // Update is called once per frame
    private void UpdateStressOMeter() {
        if (_dataManager.StressSum != 0)
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(30,(_dataManager.StressSum/Fish.maxStress*100f)- 100.0f);
        Debug.Log(100.0f - (_dataManager.StressSum / Fish.maxStress * 100f));
    }
}
