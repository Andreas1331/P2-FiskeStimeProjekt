using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update

    public void Start()
    {
        //StartCoroutine(Test());
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

    private void Update()
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
