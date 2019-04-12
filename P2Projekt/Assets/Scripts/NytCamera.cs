using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NytCamera : MonoBehaviour
{

    Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 0.0f);
    float cameraSpeed = 0.4f;
    // Start is called before the first frame update
    public Camera hovedKamera;

    void Start()
    {
          // Michael er flot
    }

    // Update is called once per frame
    void Update()
    {

        if (true)
        {
            // HEJ
            Debug.Log("HejMedDig");
        }
        if (Input.GetKey("w")) {
            transform.Translate(new Vector3(0,0,-cameraSpeed), Space.Self);
        }
        if (Input.GetKey("a"))
        {
            transform.Translate(new Vector3(cameraSpeed, 0, 0), Space.Self);
        }
        if (Input.GetKey("d"))
        {
            transform.Translate(new Vector3(-cameraSpeed, 0, 0), Space.Self);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(new Vector3(0, 0, cameraSpeed), Space.Self);
        }



        if (Input.GetKey("u"))
        {
            transform.Rotate(0, 0, 40 * Time.deltaTime);
        }
        if (Input.GetKey("o"))
        {
            transform.Rotate(0, 0, -40 * Time.deltaTime);
        }
        if (Input.GetKey("i"))
        {
            transform.Rotate(-40 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("k"))
        {
            transform.Rotate(40 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("j"))
        {
            transform.Rotate(0, -40 * Time.deltaTime, 0);
        }
        if (Input.GetKey("l"))
        {
            transform.Rotate(0, 40 * Time.deltaTime, 0);
        }


        if (Input.GetKeyDown("1")) {
            hovedKamera.enabled = true;
        }

        if (Input.GetKeyDown("2"))
        {
            hovedKamera.enabled = false;
        }
    }
}
