using System.Collections.Generic;
using UnityEngine;

public class CageHandler : MonoBehaviour
{
    public GameObject prefabCollider;
    private Vector3 defaultSize = new Vector3(15, 37.5f, 15);
    private List<Vector3> coords = new List<Vector3>();

    private void Start()
    {
        GetColliderCoordinates();
        CreateColliders();
        this.transform.localScale = defaultSize;
    }

    private void Update()
    {
        //foreach (var item in coords)
        //{
        //    Vector3 dir = item - transform.position;
        //    Debug.DrawRay(transform.position, dir, Color.yellow);
        //}
    }

    private void GetColliderCoordinates()
    {
        for (int i = 0; i < 20; i++)
        {
            Mathtools.MathTools tools = new Mathtools.MathTools();

            float v = (360 / 20) * (i + 0.5f);
            float x = Mathf.Cos(tools.DegreeToRadian(v));
            float y = Mathf.Sin(tools.DegreeToRadian(v));

            coords.Add(new Vector3(x, this.gameObject.transform.position.y, y));
        }
    }

    private void CreateColliders()
    {
        for(int i = 0; i < 20; i++)
        {
            GameObject obj = Instantiate(prefabCollider, coords[i], Quaternion.identity, transform);
            obj.transform.localScale = new Vector3(0.3f, 0.5f, 0.15f); //new Vector3(0.275f, 0.5f, 0.15f);
            obj.transform.LookAt(this.transform);
            obj.transform.tag = "Net";
        }
    }
}
