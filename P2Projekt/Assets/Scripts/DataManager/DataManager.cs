using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    public UIHandler UI;
    private CultureInfo culture = CultureInfo.CreateSpecificCulture("da-DK");

    private float HungerSum = 0;
    private float StressSum = 0;
    
    // Start is called before the first frame update
    public void Start()
    {
        UI = FindObjectOfType<UIHandler>();
        UI.Cage = Instantiate(UI.CagePrefab, new Vector3(), Quaternion.identity);
    }

    public bool SaveStatistics(Statistic stats)
    {
        if (stats == null)
            return false;

        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (System.IO.Directory.Exists(path))
        {
            string data = JsonUtility.ToJson(stats);

            // Generate the file name based on the current date and amount of files
            string date =  DateTime.Now.ToString("d", culture);
            int amount = System.IO.Directory.GetFiles(path).Length;
            string fileName = String.Format(@"\FishStatistics_{0}_{1}.json", date, amount);

            System.IO.File.WriteAllText(path + fileName, data);
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
        int fishInFishPool = fishPool.Count;
        int fishToGenerate = howManyToAdd - fishInFishPool;

        if (fishInFishPool == 0)
        {
            SpawnNewFish(howManyToAdd);
        } else if (fishInFishPool - howManyToAdd >= 0)
        {
            ActivateFishFromPool(howManyToAdd);
        } else
        {
            ActivateFishFromPool(fishInFishPool);
            SpawnNewFish(fishToGenerate);
        }
    }

    public void SpawnNewFish(int howManyToAdd)
    {
        for (int i = 0; i < howManyToAdd; i++)
        {
            fishCounter++;
            fishList.Add(new Rainbowtrout(fishCounter, 0.1f, RainbowPreFab));
        }
    }

    public void AddFoodToNet(int howManyToAdd, int amountOfFood)
    {
        for (int i = 0; i < howManyToAdd; i++)
        {
            foodCounter++;
            foodList.Add(new Food(foodCounter, FoodPreFab, amountOfFood));
        }
    }
    public void KillFish(int amountToKill)
    {
        for (int i = 0; i < amountToKill; i++)
        {
            fishList[i].IsDead = true;
        }
    }
    
    public void RemoveFishFromNet(int amountToRemove)
    {
        for (int i = 0; i < amountToRemove; i++)
        {
            fishList[i].FishObject.transform.position = new Vector3(0, 10000, 0);
            fishList[i].FishObject.SetActive(false);

            fishPool.Add(fishList[i]);
            fishList.Remove(fishList[i]);
        }
    }

    public void ActivateFishFromPool(int amountToActivate)
    {
        for (int i = 0; i < amountToActivate; i++)
        {
            fishPool[i].FishObject.SetActive(true);
            fishPool[i].FishObject.transform.position = new Vector3(0, 0, 0); //Default spawn position
            fishList.Add(fishPool[i]);
            fishPool.Remove(fishPool[i]);
        }
    }

    public void GetAmountOfFishToAddOrRemove(int totalAmountOfFish)
    {
        int currentAmountOfFish = fishList.Count;
        int newAmountOfFish = totalAmountOfFish - currentAmountOfFish;

        if(newAmountOfFish > 0)
        {
            AddFishToNet(newAmountOfFish);
        } else
        {
            RemoveFishFromNet(Math.Abs(newAmountOfFish));
        }   
    }

    public void ChangeHungerLimit(float newMaxHunger)
    {
        Fish.maxHunger = newMaxHunger;
    }
    public void ChangeStressLimit(float newMaxStress)
    {
        Fish.maxStress = newMaxStress;
    }

    public void SaveHungerAndStress(Statistic stats)
    {
        float Timer = 0;
        float TimerThreshold = 5;

        Timer += Time.deltaTime;

        if(Timer >= TimerThreshold)
        {
            foreach (Fish item in fishList)
            {
                HungerSum += item.Hunger;
                StressSum += item.Stress;
            }
            
            HungerSum /= fishList.Count;
            StressSum /= fishList.Count;
            
            Timer = 0;
        }
    }
}
