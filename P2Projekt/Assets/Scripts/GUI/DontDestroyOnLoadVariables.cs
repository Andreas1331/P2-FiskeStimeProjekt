using UnityEngine;

public class DontDestroyOnLoadVariables : MonoBehaviour
{
    //Start menu settings
    public float defaultAmountOfFish = 1;
    public float defaultHungerLimit = 999;
    public float defaultStressLimit = 999;
    public float defaultDepthOfCage = 4;
    public float defaultRadiusOfCage = 3;
    public float defaultSimSpeed = 1;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
