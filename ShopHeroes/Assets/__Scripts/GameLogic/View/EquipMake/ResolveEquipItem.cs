using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolveEquipItem : MonoBehaviour
{
    public GUIIcon resIcon;
    public Text countTx;
    public Text addTx;

    public void SetData(int itemID, int addCount, int count)
    {
        gameObject.SetActiveTrue();
        var item = ItemBagProxy.inst.GetItem(itemID);

        resIcon.SetSprite(item.itemConfig.atlas, item.itemConfig.icon);
        countTx.text = item.count + "/" + UserDataProxy.inst.GetResCountLimit(itemID);
        addTx.text = "+" + (addCount * count);
    }

    public void Clear()
    {
        gameObject.SetActiveFalse();
    }

}
