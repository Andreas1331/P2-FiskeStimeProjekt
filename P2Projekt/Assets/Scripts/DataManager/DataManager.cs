using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public List<Fish> fishList = new List<Fish>();
    public List<Fish> fishPool = new List<Fish>();
    public List<Food> foodList = new List<Food>();
    public List<Food> foodPool = new List<Food>();
    public GameObject RainbowPreFab;
    public GameObject FoodPreFab;
    public int deathCounter = 0;
    public int timesAddedFood = 0;
    private int fishCounter=0;
    private int foodCounter=0;
    // Start is called before the first frame update

    public void Start()
    {

        //AddFishToNet(10);

        //Rainbowtrout rt = new Rainbowtrout(1, RainbowPreFab);
        //Rainbowtrout rtt = new Rainbowtrout(1, RainbowPreFab);
        //rt.MoveTowards(new Vector3(0.5f, 0.2f, 0.4f));

        //AddFishToNet(200);

        //AddFoodToNet(1);
        //Food firstFoodDrop = new Food(1,FoodPreFab);

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
    public void RemoveFood(Food foodToRemove)
    {
        if (foodList.Contains(foodToRemove))
        {
            foodPool.Add(foodToRemove);
            foodList.Remove(foodToRemove);
            
        }
    }

    public void AddFishToNet(int howManyToAdd)
    {
        for (int i = 0; i < howManyToAdd; i++) {
            fishCounter++;
            fishList.Add(new Rainbowtrout(fishCounter, RainbowPreFab));
        }
    }
    public void AddFoodToNet(int howManyToAdd)
    {
        for (int i = 0; i < howManyToAdd; i++)
        {
            foodCounter++;
            foodList.Add(new Food(foodCounter, FoodPreFab));
        }
    }
}
