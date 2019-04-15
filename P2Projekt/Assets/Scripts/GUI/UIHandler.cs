using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Text FishHealth;
    public Text FishStress;
    public Text FishDepth;
    public Text SimSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Scans for escape key stroke
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenuInGame();
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("BeebMenu", LoadSceneMode.Single);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
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

    void ActivatePauseMenu()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
    }
    void DeactivatePauseMenu()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
    }
    
    public void ApplyButtonValues()
    {
        int.Parse(FishHealth.text);
        int.Parse(FishStress.text);
        int.Parse(FishDepth.text);

        SetSimSpeed(float.Parse(SimSpeed.text));
}

    public void SetSimSpeed(float timeFactor)
    {
        Time.timeScale = timeFactor;
    }

}
