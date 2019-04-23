using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTurningControlls : MonoBehaviour
{

    Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 0.0f);
    float maxCameraSpeed = 0.6f;
    float cameraSpeed = 0.6f;
    float normalCameraSpeed = 0.3f;
    // Start is called before the first frame update
    private GameObject _net;
    public GameObject Net { set { if (value != null) _net = value; } }
    private Vector3 lookAtCenter = new Vector3(0,0,0);
    void Start()
    {
        Net = GameObject.FindGameObjectWithTag("Net");
        //transform.position = new Vector3(-_net.transform.position.x / 2, -_net.transform.position.y / 2, -_net.transform.position.z / 2);
        // Michael er flot
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("a")) {
            transform.Rotate(0, cameraSpeed, 0);
        }
        if (Input.GetKey("d")) {
            transform.Rotate(0, -cameraSpeed, 0);
        }
        if (Input.GetKey("w")) {
            transform.Rotate(cameraSpeed, 0,0);
        }
        if (Input.GetKey("s")) {
            transform.Rotate(-cameraSpeed, 0,0);
        }
        if (Input.GetKeyDown("r")) {
            transform.rotation = Quaternion.Euler(new Vector3(-_net.transform.lossyScale.x / 2, -_net.transform.lossyScale.y / 2, -_net.transform.lossyScale.z / 2));
        }

        if (Input.GetKey("left shift"))
        {
            cameraSpeed = maxCameraSpeed;
        }
        else {
            cameraSpeed = normalCameraSpeed;
        }

    }
}
