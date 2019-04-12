using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //COPYRIGHT MICHAEL NIELSEN & ANDREAS ANDERSEN
    public class MathTools : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

            //Test af metoderne 
            //Vector3 vec1 = new Vector3(0, 0, 0);
            //Vector3 vec2 = new Vector3(3, 4, 1);
            //Vector3 vec3 = new Vector3(1, 1, 1);
            //GetDistanceBetweenVectors(vec1, vec2);
            //GetAngleBetweenVectors(vec2, vec3);
            //Debug.Log(GetOpposingCatheter(GetAngleBetweenVectors(vec2, vec3), GetDistanceBetweenVectors(vec2, vec3))); 
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public float GetDistanceBetweenVectors(Vector3 vec1, Vector3 vec2)
        {
            float xDifference = (vec2.x - vec1.x);
            float yDifference = (vec2.y - vec1.y);
            float zDifference = (vec2.z - vec1.z);
            float distanceBetweenVectors = Mathf.Sqrt(xDifference * xDifference + yDifference * yDifference + zDifference * zDifference);

            //Debug.Log(distanceBetweenVectors);
            return distanceBetweenVectors;
        }

        public float GetAngleBetweenVectors(Vector3 vec1, Vector3 vec2)
        {
            float dotProduct = (vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z);
            float vec1Length = Mathf.Sqrt(Mathf.Pow(vec1.x, 2) + Mathf.Pow(vec1.y, 2) + Mathf.Pow(vec1.z, 2));
            float vec2Length = Mathf.Sqrt(Mathf.Pow(vec2.x, 2) + Mathf.Pow(vec2.y, 2) + Mathf.Pow(vec2.z, 2));
            
            //Debug.Log(RadianToDegree(Mathf.Acos(dotProduct / (vec1Length * vec2Length))));
            return RadianToDegree(Mathf.Acos(dotProduct / (vec1Length * vec2Length)));
        }

        public float GetOpposingCatheter(float angle, float length) => Mathf.Sin(DegreeToRadian(angle)) * length;
        
        public float RadianToDegree(float angle)
        {
            //Debug.Log("RadianToDegree: " + angle * (180 / Mathf.PI));
            return angle * (180 / Mathf.PI);
        }
        public float DegreeToRadian(float angle)
        {
            //Debug.Log("DegreeToRadian: " + Mathf.PI * angle / 180);
            return Mathf.PI * angle / 180;
        }

    }
