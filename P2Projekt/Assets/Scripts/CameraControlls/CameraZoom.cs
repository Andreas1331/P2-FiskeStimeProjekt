using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private GameObject _cage;
    public GameObject Cage { set { if (value != null) _cage = value; } }
    float scale = 0.2f;
    private bool _inMenu = false;
    public bool InMenu { set { _inMenu = value; } }
    // Start is called before the first frame update
    private void Awake()
    {
        Cage = GameObject.FindGameObjectWithTag("Cage");
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_inMenu)
        {
            var mousewheel = Input.GetAxis("Mouse ScrollWheel");
            if (mousewheel > 0f)
            {
                transform.localPosition = new Vector3(0, 0, transform.localPosition.z + mousewheel);
            }
            else
            {
                transform.localPosition = new Vector3(0, 0, transform.localPosition.z + mousewheel);
            }
            if (Input.GetKeyDown("r"))
            {
                transform.localPosition = new Vector3(0, 0, _cage.transform.lossyScale.z);
            }
            if (Input.GetKey("e"))
            {
                transform.localPosition = new Vector3(0, 0, transform.localPosition.z + 0.3f);
            }
            if (Input.GetKey("q"))
            {
                transform.localPosition = new Vector3(0, 0, transform.localPosition.z - 0.3f);
            }
        }
    }

    void mouseScrolls() {
        
    }
    
}
