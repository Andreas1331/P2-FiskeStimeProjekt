using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 v = new Vector3();
    float maxCameraSpeed = 1.2f;
    float cameraSpeed = 0.6f;
    float normalCameraSpeed = 0.8f;
    // Start is called before the first frame update
    private GameObject _cage;
    public GameObject Cage { set { if (value != null) _cage = value; } }
    public bool inMenu = false;
    private GameObject _seaBottom;
    void Start()
    {
        Cage = GameObject.FindGameObjectWithTag("Cage");
        //transform.position = new Vector3(_net.transform.position.x, _net.transform.position.y, _net.transform.position.z);
        _seaBottom = GameObject.FindGameObjectWithTag("Terrain");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            InMenuChange();
        }
        if (!inMenu) {
            if (Input.GetKey("a"))
            {
                transform.Rotate(0, cameraSpeed, 0);
            }
            if (Input.GetKey("d"))
            {
                transform.Rotate(0, -cameraSpeed, 0);
            }
            if (Input.GetKey("w"))
            {
                transform.Rotate(cameraSpeed, 0, 0);
            }
            if (Input.GetKey("s"))
            {
                transform.Rotate(-cameraSpeed, 0, 0);
            }
            if (Input.GetKeyDown("r"))
            {

                transform.rotation = Quaternion.Euler(new Vector3(-_cage.transform.lossyScale.x, _cage.transform.lossyScale.y, -_cage.transform.lossyScale.z));
            }
            if (Input.GetKey("left shift"))
            {
                cameraSpeed = maxCameraSpeed;
            }
            else
            {
                cameraSpeed = normalCameraSpeed;
            }
            if (transform.rotation.z != 0)
            {
                v = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(v.x, v.y, 0);
            }
            //_seaBottom.transform.position = new Vector3(0, -_net.transform.lossyScale.y - 5, 0);
        }
    }
    public void InMenuChange()
    {
        if (inMenu)
        {
            inMenu = false;
            GetComponentInChildren<CameraZoom>()._inMenu = false;
        }
        else
        {
            inMenu = true;
            GetComponentInChildren<CameraZoom>()._inMenu = true;
        }
        

    }
}
