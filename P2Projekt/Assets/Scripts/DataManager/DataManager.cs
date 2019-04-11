using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private List<Fish> fishList = new List<Fish>();
    private List<Fish> fishPool = new List<Fish>();
    public GameObject RainbowPreFab;
    public int deathCounter = 0;
    public int timesAddedFood = 0;
    // Start is called before the first frame update

    public void Start()
    {
        Rainbowtrout rt = new Rainbowtrout(1, RainbowPreFab);
        Rainbowtrout rtt = new Rainbowtrout(1, RainbowPreFab);
        //rt.MoveTowards(new Vector3(0.5f, 0.2f, 0.4f));
    }
    public bool SaveStatistics(Statistic stats)
    {
        if (stats == null)
            return false;

        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        
        if (System.IO.Directory.Exists(path))
        {
            string data = JsonUtility.ToJson(stats);
            System.IO.File.WriteAllText(path + @"\Noget_midlertidligt.json", data);
            return true;
        }

        return false;
    }

    public void RemoveFish(Fish fishToRemove)
    {

        if (fishList.Contains(fishToRemove)) {
            fishPool.Add(fishToRemove);
            fishList.Remove(fishToRemove);
            deathCounter++;
        }
    }
}
