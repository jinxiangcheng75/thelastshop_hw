using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    public List<GameObject> spiders = new List<GameObject>();
    public int index = 0;

    public void setSpiderClear(int index)
    {
        index = index - 1;
        index = index >= spiders.Count ? spiders.Count - 1 : index;

        spiders[index].SetActiveFalse();
    }

    public void setAllActiveTrue()
    {
        for (int i = 0; i < spiders.Count; i++)
        {
            spiders[i].SetActiveTrue();
        }
    }

    public void setAllActiveFalse()
    {
        for (int i = 0; i < spiders.Count; i++)
        {
            spiders[i].SetActiveFalse();
        }
    }
}
