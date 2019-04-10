﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private List<Fish> fishList = new List<Fish>();
    private List<Fish> fishPool = new List<Fish>();

    public void Start()
    {

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
        fishPool.Add(fishToRemove);
        fishList.Remove(fishToRemove);
        
    }
}
