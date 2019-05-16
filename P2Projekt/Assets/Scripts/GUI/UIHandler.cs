using System.Collections;
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
    private DontDestroyOnLoadVariables DDOLV;
    private GameObject GuiPanel;
    public GameObject Cage;
    public GameObject CagePrefab;

    //Start-menu objects
    private Text SimSpeedtxt;

    //Overlay-menu objects
    private Text FishAmounttxt;
    private Text FishHealthtxt;
    private Text FishStresstxt;
    private Text FishDepthtxt;
    private Text CageRadiustxt;
    private Text CageDepthtxt;
    private Text AmountOfFishtxt;
    private Text AmountOfFishFromInputtxt;
    private Slider AmountOfFishSlider;
    private Slider SizeOfCageSlider;
    private Slider DepthOfCageSlider;

    //Cage sizes
    private float _radiusOfCage;
    private float _depthOfCage;

    private void Awake()
    {
        //Class to store variables to transfor to Main-scene
        DDOLV = FindObjectOfType<DontDestroyOnLoadVariables>();

        //Start-menu objects
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            FishAmounttxt = GameObject.Find("FishAmounttxt").GetComponent<Text>();
            FishDepthtxt = GameObject.Find("FishDepthtxt").GetComponent<Text>();
            SimSpeedtxt = GameObject.Find("SimSpeedTxt").GetComponent<Text>();
        }

        //Start-menu and Main Scene objects
        FishHealthtxt   = GameObject.Find("FishHungerTxt").GetComponent<Text>();
        FishStresstxt   = GameObject.Find("FishStresstxt").GetComponent<Text>();

        //General objects
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            DM = FindObjectOfType<DataManager>();
            GuiPanel = GameObject.Find("OverlayMenu");
            //Find sliders and textfields when Main-scene is loaded
            AmountOfFishSlider       = GameObject.Find("AmountOfFishSlider").GetComponent<Slider>();
            SizeOfCageSlider         = GameObject.Find("SizeOfCageSlider").GetComponent<Slider>();
            CageRadiustxt            = GameObject.Find("SizeOfCageText").GetComponent<Text>();
            CageDepthtxt             = GameObject.Find("DepthOfCageText").GetComponent<Text>();
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
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            AmountOfFishSlider.value = DDOLV.defaultAmountOfFish;
        }
        SetSimSpeed(DDOLV.defaultSimSpeed);

        Vector3 D1 = new Vector3(7, 3, 6);
        Vector3 D2 = new Vector3(-6, -5, 3);
        Vector3 D3 = new Vector3(3, 3, -6);
        Vector3 D4 = new Vector3(-2, 3, 6);
        Vector3 D5 = new Vector3(1, -8, 0);
        Vector3 D6 = new Vector3(4, 2, -6);

        float lambda = 1 / 6;

        Vector3 D = (D1 * lambda) + (D2 * lambda) + (D3 * lambda) + (D4 * lambda) + (D5 * lambda) + (D6 * lambda);
        Debug.Log(D);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenuInGame();
        }
    }

    public void SetCageSizeAfterCageLoad() //Gets called after CageManager
    {
        SizeOfCageSlider.value = DDOLV.defaultRadiusOfCage;
        DepthOfCageSlider.value = DDOLV.defaultDepthOfCage;

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            InitializeButtonValues();
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
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
        SetSimSpeed(DDOLV.defaultSimSpeed);
    }
    public void SetSimSpeed(float timeFactor)
    {
        Time.timeScale = timeFactor;
    }

    public void ToggleGuiVisibility(GameObject overlayPanel)
    {
        HideGui(overlayPanel);
    }
    public void HideGui(GameObject guiPanel)
    {
        if (guiPanel.activeSelf == true)
        {
            guiPanel.SetActive(false);
        }
        else
        {
            guiPanel.SetActive(true);
        }
    }

    public void ApplyButtonValues()
    {
        if (FishHealthtxt.text != "")
        {
            DDOLV.defaultHungerLimit = float.Parse(FishHealthtxt.text);
        }

        if (FishStresstxt.text != "")
        {
            DDOLV.defaultStressLimit = float.Parse(FishStresstxt.text);
        }
        
        //Da disse værdier kun kan ændres i hovedmenuen, tjekkes der om vi er der. 
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (FishAmounttxt.text != "")
            {
                DDOLV.defaultAmountOfFish = float.Parse(FishAmounttxt.text);
            }
            if (FishDepthtxt.text != "")
            {
                DDOLV.defaultDepthOfCage = float.Parse(FishDepthtxt.text);
            } 
            if (SimSpeedtxt.text != "")
            {
                DDOLV.defaultSimSpeed = float.Parse(SimSpeedtxt.text);
                SetSimSpeed(DDOLV.defaultSimSpeed);
            }
        } else
        {
            InitializeButtonValues();
        }
    }
    
    public void InitializeButtonValues() //Bliver kun kaldt hvis main scene er aktiv. 
    {
        DM.ChangeHungerLimit(DDOLV.defaultHungerLimit);
        DM.ChangeStressLimit(DDOLV.defaultStressLimit);
    }

    public void SetAmountOfFishInSimulationFromSlider()
    {
        float amountFromSlider = AmountOfFishSlider.value;
        DM.GetAmountOfFishToAddOrRemove(amountFromSlider);
        AmountOfFishtxt.text = "Amount of fish: " + AmountOfFishSlider.value;
    }
    public void SetAmountOfFishInSimulationFromInput(Text inputText)
    {
        float amountFromInput = float.Parse(inputText.text);
        
        if (amountFromInput >= 0)
        {
            if (AmountOfFishSlider.minValue <= amountFromInput && amountFromInput <= AmountOfFishSlider.maxValue)
            {
                //AmountOfFishSlider.enabled = true;
                AmountOfFishSlider.value = amountFromInput;
            }
            else
            {
                AmountOfFishtxt.text = "Amount of fish: " + amountFromInput;
                DM.GetAmountOfFishToAddOrRemove(amountFromInput);
                //AmountOfFishSlider.enabled = false;
            }
        } else
        {
            AmountOfFishtxt.text = "Incorrect amount";
        }
    }
    
    public void ChangeCageSize()
    {
        _radiusOfCage = SizeOfCageSlider.value;
        CageRadiustxt.text = "CAGE RADIUS: " + SizeOfCageSlider.value;
        ApplySizeOfCage();
    }

    public void ChangeCageSizeFromInput(Text inputText)
    {
        float amountFromInput = float.Parse(inputText.text);

        if (amountFromInput >= 0)
        {
            if (SizeOfCageSlider.minValue <= amountFromInput && amountFromInput <= SizeOfCageSlider.maxValue)
            {
                //SizeOfCageSlider.enabled = true;
                SizeOfCageSlider.value = amountFromInput;
            }
            else
            {
                CageRadiustxt.text = "CAGE RADIUS: " + amountFromInput;
                _radiusOfCage = amountFromInput;
                //SizeOfCageSlider.enabled = false;
            }
            ApplySizeOfCage();
        }
        else
        {
            CageRadiustxt.text = "Incorrect amount";
        }
    }

    public void ChangeCageDepth()
    {
        _depthOfCage = DepthOfCageSlider.value;
        CageDepthtxt.text = "CAGE DEPTH: " + DepthOfCageSlider.value;
        ApplySizeOfCage();
    }
    public void ChangeCageDepthFromInput(Text inputText)
    {
        float amountFromInput = float.Parse(inputText.text);

        if (amountFromInput >= 0)
        {
            
            if (DepthOfCageSlider.minValue <= amountFromInput && amountFromInput <= DepthOfCageSlider.maxValue)
            {
                //DepthOfCageSlider.enabled = true;
                DepthOfCageSlider.value = amountFromInput;
            }
            else
            {
                CageDepthtxt.text = "CAGE DEPTH: " + amountFromInput;
                _depthOfCage = amountFromInput;
                //DepthOfCageSlider.enabled = false;
            }
            ApplySizeOfCage();
        }
        else
        {
            CageDepthtxt.text = "Incorrect amount";
        }
    }

    public void ApplySizeOfCage()
    {
        Cage.transform.localScale = new Vector3(_radiusOfCage, 2.5f * _depthOfCage, _radiusOfCage);
    }
}
