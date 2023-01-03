using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupAwardItemUI : MonoBehaviour
{
    public GUIIcon icon;
    public Text numText;

    public void setData(ShowPopupData data)
    {
        if (data.itemConfig == null)
        {
            return;
        }
        icon.SetSprite(data.itemConfig.atlas, data.itemConfig.icon);
        numText.text = data.itemNum.ToString();
    }
}
