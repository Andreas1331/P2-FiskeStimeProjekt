using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickQuit : MonoBehaviour
{
    public void ApplicationQuit() {

        //Sættes sådan at vi kan teste i Unity editoren.
        UnityEditor.EditorApplication.isPlaying = false;

        Application.Quit();
    }
}
