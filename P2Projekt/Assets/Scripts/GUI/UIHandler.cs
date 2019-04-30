using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Slider DepthOfCageSlider;
    private float DefaultHunger = 999;
    private float DefaultStress = 999;

    private float _radiusOfCage;
    private float _depthOfCage;

    //Start menu settings
    private float _defaultFishAmount = 12;
    private float _defaultHungerLimit = 999;
    private float _defaultStressLimit = 999;
    private float _defaultRadiusOfCage = 10;
    private float _defaultDepthOfCage = 8;
    private float _defaultSimSpeed = 1;

    private void Awake()
    {
        DM = FindObjectOfType<DataManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        //DM.ChangeHungerLimit(DM.DefaultHungerLimit);
        //DM.ChangeStressLimit(DM.DefaultStressLimit);
        //AmountOfFishSlider.value = DefaultFishAmount;
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

    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        AmountOfFishSlider.value = _defaultFishAmount;
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

    [SerializeField] private GameObject PauseMenuUI;
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
            //DM.ChangeHungerLimit(float.Parse(FishHealthtxt.text));
        }

        if (FishStresstxt.text != "")
        {
            _defaultStressLimit = float.Parse(FishStresstxt.text);
            //DM.ChangeStressLimit(float.Parse(FishStresstxt.text));
        }

        if (FishDepthtxt.text != "")
        {
            _defaultDepthOfCage = float.Parse(FishDepthtxt.text);
        }

        if(SimSpeedtxt.text != "")
        {
            _defaultSimSpeed = float.Parse(SimSpeedtxt.text);
            //SetSimSpeed(float.Parse(SimSpeedtxt.text));
        } else
        {
            SetSimSpeed(_defaultSimSpeed);
        }
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
        Cage.transform.localScale = new Vector3(_radiusOfCage, 2 * _depthOfCage, _radiusOfCage);
    }
}
