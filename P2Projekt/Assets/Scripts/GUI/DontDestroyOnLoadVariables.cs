using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadVariables : MonoBehaviour
{
    //Start menu settings
    public float defaultAmountOfFish = 1;
    public float defaultHungerLimit = 999;
    public float defaultStressLimit = 999;
    public float defaultDepthOfCage = 15;
    public float defaultSimSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
