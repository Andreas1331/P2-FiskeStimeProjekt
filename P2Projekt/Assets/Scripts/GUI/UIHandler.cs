using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public DataManager DM;
    // Start is called before the first frame update
    public Text FishHealth;
    public Text FishStress;
    public Text FishDepth;
    public Text SimSpeed;
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
            DM.DeactivatePauseMenu(PauseMenuUI);
        } else if (PauseMenuUI.activeSelf == false)
        {
            DM.ActivatePauseMenu(PauseMenuUI);
        }
    }
    
    public void ApplyButtonValues()
    {
        int.Parse(FishHealth.text);
        int.Parse(FishStress.text);
        int.Parse(FishDepth.text);

        DM.SetSimSpeed(float.Parse(SimSpeed.text));
    }
}
