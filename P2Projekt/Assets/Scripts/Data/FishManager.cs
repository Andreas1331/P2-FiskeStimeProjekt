using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public GameObject RainbowPreFab;
    // Start is called before the first frame update
    void Start()
    {
        Rainbowtrout rt = new Rainbowtrout(1, RainbowPreFab);
        rt.MoveTowards(new Vector3(0.5f, 0.2f, 0.4f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
