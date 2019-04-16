using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testdieinstance : MonoBehaviour
{

    Vector3 newdir = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.x > -0.7f)
        {
            newdir = Vector3.RotateTowards(transform.forward, new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime, 2.5f);
            transform.rotation = Quaternion.LookRotation(newdir);
            //Debug.Log(transform.rotation.x);
            //transform.RotateAround(transform.position, Vector3.forward, 10 * Time.deltaTime);
        }
        else if (transform.rotation.x <= -0.7f && transform.position.y < 0)
        {
            //transform.RotateAround(transform.position, Vector3.forward, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y + 5 * Time.deltaTime, transform.position.z);
            //Debug.Log("1");
            

            //MakeOpague;
        }
        else
        {
            //Debug.Log("Er dissabled nu");
            transform.position = new Vector3(-5000.0f, -5000.0f, -5000.0f);
            this.transform.gameObject.SetActive(false);
        }
    }
}
