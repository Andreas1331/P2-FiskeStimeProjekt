using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoatScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadBeebScene()
    {
        SceneManager.LoadScene("BeebsFishSCENEV1", LoadSceneMode.Single);
    }
}
