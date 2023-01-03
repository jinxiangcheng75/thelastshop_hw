using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMove : MonoBehaviour
{
   // public List<GameObject> bgGos;
    private Transform _transForm;

    Vector3 movespeed;
    // Start is called before the first frame update
    void Start()
    {
        movespeed = new Vector3(-1f, 0.5f, 0) * 1.3f;
        _transForm = transform;
    }

    // Update is called once per frame
    void Update()
    {
        

        if(_transForm.transform.localPosition.x < -4096.0f)
        {
            _transForm.localPosition = Vector3.zero;
        }   
        else
        {
            _transForm.Translate(movespeed * Time.deltaTime);
        }
     }
}
