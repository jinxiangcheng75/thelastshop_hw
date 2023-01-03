using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFRotate : MonoBehaviour
{
    public Transform rotateTF;
    public float rotSpeed = 5;

    void Start()
    {
        if (rotateTF == null)
        {
            this.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        rotateTF.Rotate(0, 0, -rotSpeed * Time.deltaTime);
    }
}
