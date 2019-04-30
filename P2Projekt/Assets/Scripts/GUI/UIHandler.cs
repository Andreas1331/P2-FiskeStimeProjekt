﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    //The total PauseMenu Panel in Unity
    [SerializeField] private GameObject PauseMenuUI;
 
    //General objects
    private DataManager DM;
    private GameObject GuiPanel;
    public GameObject Cage;
    public GameObject CagePrefab;

    //Start-menu objects
    private Text SimSpeedtxt;

    //Overlay-menu objects
    private Text FishHealthtxt;
    private Text FishStresstxt;
    private Text FishDepthtxt;
    private Text AmountOfFishtxt;
    private Text AmountOfFishFromInputtxt;
    private Slider AmountOfFishSlider;
    private Slider SizeOfCageSlider;
    private Slider DepthOfCageSlider;

    private float _radiusOfCage;
    private float _depthOfCage;

    //Start menu settings
    private float _defaultFishAmount = 1;
    private float _defaultHungerLimit = 999;
    private float _defaultStressLimit = 999;
    public float defaultRadiusOfCage = 12;
    public float defaultDepthOfCage = 15;
    private float _defaultSimSpeed = 1;

    private void Awake()
    {
        //Menu objects
        SimSpeedtxt     = GameObject.Find("SimSpeedTxt").GetComponent<Text>();
        FishHealthtxt   = GameObject.Find("FishHungerTxt").GetComponent<Text>();
        FishStresstxt   = GameObject.Find("FishStresstxt").GetComponent<Text>();
        FishDepthtxt    = GameObject.Find("FishDepthtxt").GetComponent<Text>();

        //General objects
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            DM = FindObjectOfType<DataManager>();
            GuiPanel = GameObject.Find("OverlayMenu");
            //Find sliders and textfields when Main-scene is loaded
            AmountOfFishSlider       = GameObject.Find("AmountOfFishSlider").GetComponent<Slider>();
            SizeOfCageSlider         = GameObject.Find("SizeOfCageSlider").GetComponent<Slider>();
            DepthOfCageSlider        = GameObject.Find("DepthOfCageSlider").GetComponent<Slider>();
            AmountOfFishFromInputtxt = GameObject.Find("AmountOfFishFromInputtxt").GetComponent<Text>();
            AmountOfFishtxt          = GameObject.Find("AmountOfFishText").GetComponent<Text>();
            //Panels has to be active in Unity, then disabled on game-start
            GameObject.Find("AdvancedSettings").SetActive(false);
            GameObject.Find("PauseSettingPanel").SetActive(false);
        }
    }

    private void Start()
    {
        //_radiusOfCage = DM.DefaultRadiusOfCage;
        //_depthOfCage = DM.DefaultDepthOfCage;
        //SetSimSpeed(DM.DefaultSimSpeed);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenuInGame();
        }
        if (Input.GetKeyUp(KeyCode.K)) {
            DM.AddFoodToNet(1,5);
        }
    }

    public void SetCageSizeAfterCageLoad()
    {
        SizeOfCageSlider.value = defaultRadiusOfCage;
        DepthOfCageSlider.value = defaultDepthOfCage;
        AmountOfFishSlider.value = _defaultFishAmount;
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        //AmountOfFishSlider.value = _defaultFishAmount;
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

    public void TogglePauseMenuInGame()
    {   
        if(PauseMenuUI.activeSelf == true)
        {
            DeactivatePauseMenu();
        } else if (PauseMenuUI.activeSelf == false)
        {
            ActivatePauseMenu();
        }
    }
    
    public void ActivatePauseMenu()
    {
        PauseMenuUI.SetActive(true);
        SetSimSpeed(0.0f);
    }

    public void DeactivatePauseMenu()
    {
        PauseMenuUI.SetActive(false);
        SetSimSpeed(_defaultSimSpeed);
    }
    public void SetSimSpeed(float timeFactor)
    {
        Time.timeScale = timeFactor;
    }
    public void SetSpeedButtens(float timefactor)
    {
        SetSimSpeed(timefactor);
    }

    public void ToggleGuiVisibility(GameObject overlayPanel)
    {
        HideGui(overlayPanel);
    }
    public void HideGui(GameObject GuiPanel)
    {
        if (GuiPanel.activeSelf == true)
        {
            GuiPanel.SetActive(false);
        }
        else
        {
            GuiPanel.SetActive(true);
        }
    }

    public void ApplyButtonValues()
    {
        if (FishHealthtxt.text != "")
        {
            _defaultHungerLimit = float.Parse(FishHealthtxt.text);
        }

        if (FishStresstxt.text != "")
        {
            _defaultStressLimit = float.Parse(FishStresstxt.text);
        }

        if (FishDepthtxt.text != "")
        {
            defaultDepthOfCage = float.Parse(FishDepthtxt.text);
        }

        if(SimSpeedtxt.text != "")
        {
            _defaultSimSpeed = float.Parse(SimSpeedtxt.text);
            //SetSimSpeed(float.Parse(SimSpeedtxt.text));
        } else
        {
            SetSimSpeed(_defaultSimSpeed);
        }

        CallMethodsWithButtonValues();
    }

    public void CallMethodsWithButtonValues()
    {
        DM.ChangeHungerLimit(_defaultHungerLimit);
        DM.ChangeStressLimit(_defaultStressLimit);
        SizeOfCageSlider.value = defaultRadiusOfCage;
        DepthOfCageSlider.value = defaultDepthOfCage;

    }
    public void SetAmountOfFishInSimulationFromSlider()
    {
        DM.GetAmountOfFishToAddOrRemove((int)AmountOfFishSlider.value);
        AmountOfFishtxt.text = "Amount of fish: " + AmountOfFishSlider.value;
    }
    public void SetAmountOfFishInSimulationFromInput(Text inputText)
    {
        AmountOfFishSlider.value = float.Parse(inputText.text);
    }
    
    public void ChangeCageSize()
    {
        _radiusOfCage = SizeOfCageSlider.value;
        GameObject.Find("SizeOfCageText").GetComponent<Text>().text = "CAGE RADIUS: " + SizeOfCageSlider.value;
        ApplySizeOfCage();
    }
    public void ChangeCageSizeFromInput(Text inputText)
    {
        SizeOfCageSlider.value = float.Parse(inputText.text);
    }
    public void ChangeCageDepth()
    {
        _depthOfCage = DepthOfCageSlider.value;
        GameObject.Find("DepthOfCageText").GetComponent<Text>().text = "CAGE DEPTH: " + DepthOfCageSlider.value;
        ApplySizeOfCage();
    }
    public void ChangeCageDepthFromInput(Text inputText)
    {
        if (inputText.text != null && inputText.text.Length > 0)
            DepthOfCageSlider.value = float.Parse(inputText.text);
    }
    public void ApplySizeOfCage()
    {
        Cage.transform.localScale = new Vector3(_radiusOfCage, 2.5f * _depthOfCage, _radiusOfCage);
    }
}
