using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private DataManager DM;
    public Text FishHealth;
    public Text FishStress;
    public Text FishDepth;
    public Text SimSpeed;
    private void Awake()
    //Når gameobjectet bliver aktiveret
    {
        DM = FindObjectOfType<DataManager>();
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenuInGame();
        }
    }
    public void LoadMainMenu()
    {
        DM.LoadMainMenu();
    }
    public void LoadScene()
    {
        DM.LoadScene();
    }
    public void ApplicationQuit()
    {
        DM.ApplicationQuit();
    }

    [SerializeField] private GameObject PauseMenuUI;
    public void TogglePauseMenuInGame()
    {   
        if(PauseMenuUI.activeSelf == true)
        {
            //DM.DeactivatePauseMenu(PauseMenuUI);
            DeactivatePauseMenu();
        } else if (PauseMenuUI.activeSelf == false)
        {
            //DM.ActivatePauseMenu(PauseMenuUI);
            ActivatePauseMenu();
        }
    }
    public void ActivatePauseMenu()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
    }
    public void DeactivatePauseMenu()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ApplyButtonValues()
    {
        int.Parse(FishHealth.text);
        int.Parse(FishStress.text);
        int.Parse(FishDepth.text);

        DM.SetSimSpeed(float.Parse(SimSpeed.text));
    }
    public void SetButtenSpeed(float timefactor)
    {
        DM.SetSimSpeed(timefactor);
    }
}
