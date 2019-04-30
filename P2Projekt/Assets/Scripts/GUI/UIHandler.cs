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

    private float _radiusOfCage;
    private float _depthOfCage;

    private void Awake()
    {
        DM = FindObjectOfType<DataManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        DM.ChangeHungerLimit(DM.DefaultHungerLimit);
        DM.ChangeStressLimit(DM.DefaultStressLimit);
        AmountOfFishSlider.value = DM.DefaultFishAmount;
        _radiusOfCage = DM.DefaultRadiusOfCage;
        _depthOfCage = DM.DefaultDepthOfCage;
        SetSimSpeed(DM.DefaultSimSpeed);
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
    public void SetSimSpeed(float timeFactor)
    {
        Time.timeScale = timeFactor;
    }
    public void ActivatePauseMenu(GameObject PauseMenuUI)
    {
        PauseMenuUI.SetActive(true);
        SetSimSpeed(0.0f);
    }
    public void DeactivatePauseMenu(GameObject PauseMenuUI)
    {
        PauseMenuUI.SetActive(false);
        SetSimSpeed(1.0f);
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
            DM.ChangeHungerLimit(float.Parse(FishHealthtxt.text));
        }

        if (FishStresstxt.text != "")
        {
            DM.ChangeStressLimit(float.Parse(FishStresstxt.text));
        }

        if (FishDepthtxt.text != "")
        {
            DM.DefaultDepthOfCage = float.Parse(FishDepthtxt.text);
        }

        if(SimSpeedtxt.text != "")
        {
            SetSimSpeed(float.Parse(SimSpeedtxt.text));
        } else
        {
            SetSimSpeed(1);
        }
    }
    public void SetSpeedButtens(float timefactor)
    {
        SetSimSpeed(timefactor);
    }

    public void ToggleGuiVisibility(GameObject overlayPanel)
    {
        HideGui(overlayPanel);
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
        DepthOfCageSlider.value = float.Parse(inputText.text);
    }
    public void ApplySizeOfCage()
    {
        Cage.transform.localScale = new Vector3(_radiusOfCage, 2 * _depthOfCage, _radiusOfCage);
    }
}
