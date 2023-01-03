using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjList : MonoBehaviour, IDynamicScrollViewItem
{
    public List<GameObject> objList;

    public int index = 0;
    public void onUpdateItem(int _index)
    {
        index = _index;
    }

}
