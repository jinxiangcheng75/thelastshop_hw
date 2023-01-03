using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipStarUpProgress : MonoBehaviour
{

    public GUIIcon icon_effect;
    public Image img_on;
    public Image img_off;
    public Text tx_effect;
    public Text tx_des;

    public void SetData(starUpProgressItemInfo info, bool onOff)
    {
        img_on.enabled = onOff;
        img_off.enabled = !onOff;
        icon_effect.SetSprite(info.atlas, info.iconName, needSetNativeSize: true);

        //if (info.type == ReceiveInfoUIType.StarUpEffectTrigger_super) //超凡装备不展示概率
        //{
        //    tx_effect.text = "";
        //}
        //else
        //{
        //    tx_effect.text = "+" + (info.value / 100) + "%";
        //}

        tx_effect.text = LanguageManager.inst.GetValueByKey(info.title);
        tx_des.text = LanguageManager.inst.GetValueByKey(info.des);
        tx_des.color = GUIHelper.GetColorByColorHex(onOff ? "#FFFFFF" : "#E0B884");

    }

}
