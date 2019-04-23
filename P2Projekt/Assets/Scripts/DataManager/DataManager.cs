using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private UIHandler UI;
    private CultureInfo culture = CultureInfo.CreateSpecificCulture("da-DK");
    // Start is called before the first frame update

    public void Start()
    {
        //Rainbowtrout rt = new Rainbowtrout(1, 0.1f, RainbowPreFab);
        //Rainbowtrout rtt = new Rainbowtrout(1, RainbowPreFab);
        //rt.MoveTowards(new Vector3(0.5f, 0.2f, 0.4f));

        //Food firstFoodDrop = new Food(1,FoodPreFab);

        //AddFishToNet(5);
        AddFoodToNet(1, 3);
    }

    int id = 1;
    int amount = 0;
    private IEnumerator Test()
    {
        while (amount < 5)
        {
            yield return new WaitForSeconds(1.5f);

            new Rainbowtrout(id++, 0.1f, RainbowPreFab);
            amount++;
        }
    }

    bool started = false;
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && !started)
        {
            StartCoroutine(Test());
            started = true;
        }
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
        //If fish in fishpool, spawn those

        for (int i = 0; i < howManyToAdd; i++) {
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

    public void GetAmountOfFishToAddOrRemove(int totalAmountOfFish)
    {
        int currentAmountOfFish = fishList.Count;
        int newAmountOfFish =  totalAmountOfFish - currentAmountOfFish;
        Debug.Log(currentAmountOfFish);

        if(newAmountOfFish > 0)
        {
            AddFishToNet(newAmountOfFish);
            Debug.Log("Add fish: " + newAmountOfFish);
        } else if (newAmountOfFish < 0)
        {
            RemoveFishFromNet(-newAmountOfFish);
            Debug.Log("Remove fish: " + newAmountOfFish);
        } else
        {
            //Do nothing
            Debug.Log("Else: (Do nothing)" + newAmountOfFish);
        }

        Debug.Log("Amount of fish is: " + totalAmountOfFish);
    }

    //GUI TOOLS _______________________________________________________________START
    #region GUI TOOLS
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    public void ApplicationQuit()
    {
#if UNITY_EDITOR
        //Sættes sådan at vi kan teste i Unity editoren.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //Method to set simulation time
    public void SetSimSpeed(float timeFactor)
    {
        Time.timeScale = timeFactor;
    }

    public void ActivatePauseMenu(GameObject PauseMenuUI)
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
    }
    public void DeactivatePauseMenu(GameObject PauseMenuUI)
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
    }
    #endregion
}
