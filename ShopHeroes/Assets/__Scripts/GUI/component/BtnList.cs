using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mosframe;

public class BtnList : UIBehaviour, IDynamicScrollViewItem
{
    public bool isBag = true;
    public List<Button> buttonList;

    public int index = 0;
    public void onUpdateItem(int _index)
    {
        index = _index;
    }

    // protected override void OnDisable()
    // {
    //     foreach (var btn in buttonList)
    //     {
    //         btn.gameObject.SetActive(false);
    //     }
    // }
}
