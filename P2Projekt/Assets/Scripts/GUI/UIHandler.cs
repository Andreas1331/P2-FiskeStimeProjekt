using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Scans for escape key stroke
        TogglePauseMenuInGame();
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
    public void LoadScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    [SerializeField] private GameObject PauseMenuUI;
    [SerializeField] private bool isPaused;
    public void TogglePauseMenuInGame()
    {
        //Toggles pause menu on esc key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            ActivatePauseMenu();
        }
        else
        {
            DeactivatePauseMenu();
        }
    }

    void ActivatePauseMenu()
    {
        PauseMenuUI.SetActive(true);
    }
    void DeactivatePauseMenu()
    {
        PauseMenuUI.SetActive(false);
    }


}
