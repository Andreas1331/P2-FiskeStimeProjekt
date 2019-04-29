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
    public Slider DepthOfCageSlider;
    private float DefaultHunger = 999;
    private float DefaultStress = 999;

    private float RadiusOfCage = 10;
    private float DepthOfCage = 8;

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
        AmountOfFishSlider.value = DM.DefaultFishAmount;
        Debug.Log("Fish amount is: " + DM.DefaultFishAmount);
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
        DM.HideGui(overlayPanel);
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
        RadiusOfCage = SizeOfCageSlider.value;
        GameObject.Find("SizeOfCageText").GetComponent<Text>().text = "CAGE RADIUS: " + SizeOfCageSlider.value;
        ApplySizeOfCage();
    }
    public void ChangeCageSizeFromInput(Text inputText)
    {
        SizeOfCageSlider.value = float.Parse(inputText.text);
    }
    public void ChangeCageDepth()
    {
        DepthOfCage = DepthOfCageSlider.value;
        GameObject.Find("DepthOfCageText").GetComponent<Text>().text = "CAGE DEPTH: " + DepthOfCageSlider.value;
        ApplySizeOfCage();
    }
    public void ChangeCageDepthFromInput(Text inputText)
    {
        DepthOfCageSlider.value = float.Parse(inputText.text);
    }
    public void ApplySizeOfCage()
    {
        Cage.transform.localScale = new Vector3(RadiusOfCage, 2 * DepthOfCage, RadiusOfCage);
    }
}
