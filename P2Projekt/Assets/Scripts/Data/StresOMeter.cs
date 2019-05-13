using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StresOMeter : MonoBehaviour
{
    //private RectTransform.Edge bottom = RectTransform.Edge.Bottom;
    //RectTransform myRectangle;
    //private void Start()
    //{
    //    myRectangle = this.GetComponent<RectTransform>();
    //}
    public void UpdateStressOMeter(float sizeOfMeter) {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(30, sizeOfMeter);           
    }
}
