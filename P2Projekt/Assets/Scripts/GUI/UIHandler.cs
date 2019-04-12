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
        SceneManager.LoadScene("BeebsFishSCENEV1", LoadSceneMode.Single);
    }
}
