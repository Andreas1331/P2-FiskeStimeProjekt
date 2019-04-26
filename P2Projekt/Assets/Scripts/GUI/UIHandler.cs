using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private DataManager DM;
    public GameObject GuiPanel;
    public GameObject Cage;
    public Text FishHealthtxt;
    public Text FishStresstxt;
    public Text FishDepthtxt;
    public Text SimSpeedtxt;
    public Text AmountOfFishtxt;
    public Text AmountOfFishFromInputtxt;
    public Slider AmountOfFishSlider;
    public Slider SizeOfCageSlider;
    private float DefaultHunger = 999;
    private float DefaultStress = 999;

    private void Awake()
    {
        DM = FindObjectOfType<DataManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        //GameObject.Find("OverlayMenu").SetActive(true);
        //GameObject.Find("PauseSettingPanel").SetActive(false);
        //GameObject.Find("PauseMenuItems").SetActive(true);
        //GameObject.Find("AdvancedSettings").SetActive(false);
        DM.ChangeMaxStress(DefaultStress);
        DM.ChangeMaxHunger(DefaultHunger);
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
        //Time.timeScale = 1.0f;
    }

    public void ApplyButtonValues()
    {
        if (FishHealthtxt.text != "")
        {
            DM.ChangeMaxHunger(float.Parse(FishHealthtxt.text));
        }

        if (FishStresstxt.text != "")
        {
            DM.ChangeMaxStress(float.Parse(FishStresstxt.text));
        }

        if (FishDepthtxt.text != "")
        {
            int.Parse(FishDepthtxt.text);
        }

        if(SimSpeedtxt.text != "")
        {
            DM.SetSimSpeed(float.Parse(SimSpeedtxt.text));
        } else
        {
            DM.SetSimSpeed(1);
        }
    }
    public void SetSpeedButtens(float timefactor)
    {
        DM.SetSimSpeed(timefactor);
    }

    public void ToggleGuiVisibility(GameObject overlayPanel)
    {
        DM.HideGui(overlayPanel); //Toggles the overlay GUI 
    }

    public void SetAmountOfFishInSimulationFromSlider()
    {
        //Calls method in DM to update the amount of fish
        DM.GetAmountOfFishToAddOrRemove((int)AmountOfFishSlider.value);

        //Change the text to display the current amount of fish
        AmountOfFishtxt.text = "Amount of fish: " + AmountOfFishSlider.value.ToString("0");
    }

    public void SetAmountOfFishInSimulationFromInput()
    {
        //Calls method in DM to update the amount of fish
        DM.GetAmountOfFishToAddOrRemove(int.Parse(AmountOfFishFromInputtxt.text));
        //AmountOfFishSlider.value = int.Parse(AmountOfFishFromInputtxt.text);

        //Change the text to display the current amount of fish
        AmountOfFishtxt.text = "Amount of fish: " + AmountOfFishFromInputtxt.text;
    }
    
    public void ChangeCageSize()
    {
        float sliderValue = SizeOfCageSlider.value;
        Cage.transform.localScale = new Vector3(sliderValue, 1, sliderValue);
    }
    public void ChangeCageDepth()
    {

    }
}
