using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private GameObject _net;
    public GameObject Net { set { if (value != null) _net = value; } }
    float scale = 0.2f;
    
    // Start is called before the first frame update
    void Start()
    {
        Net = GameObject.FindGameObjectWithTag("Net");
    }

    // Update is called once per frame
    void Update()
    {
        var mousewheel = Input.GetAxis("Mouse ScrollWheel") ;
        if (mousewheel > 0f) {
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z + mousewheel);
        }
        else {
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z + mousewheel);
        }
        if (Input.GetKeyDown("r"))
        {
            transform.localPosition = new Vector3(0,0, -_net.transform.lossyScale.z/2) ;
        }
        if (Input.GetKey("e"))
        {
            transform.localPosition = new Vector3(0,0,transform.localPosition.z + 0.3f);
        }
        if (Input.GetKey("q"))
        {
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z - 0.3f);
        }

        
    }
    void mouseScrolls() {
        
    }
}
