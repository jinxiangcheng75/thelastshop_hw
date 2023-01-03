using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDestroy : MonoBehaviour
{
    public float lifeTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (lifeTime <= 0)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            lifeTime -= Time.deltaTime;
        }
    }
}
