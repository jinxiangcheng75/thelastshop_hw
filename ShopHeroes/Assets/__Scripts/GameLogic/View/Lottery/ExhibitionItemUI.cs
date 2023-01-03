using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExhibitionItemUI : MonoBehaviour, IDynamicScrollViewItem
{
    public Text nameText;
    public Text typeText;
    public Text contentText;
    public int index = 0;

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    public void setData(Recording infoText, bool isWorld)
    {
        if (infoText == null) return;
        if (isWorld)
            nameText.text = LanguageManager.inst.GetValueByKey(infoText.name);
        else
            nameText.text = LanguageManager.inst.GetValueByKey(UserDataProxy.inst.playerData.playerName);
        itemConfig tempData = ItemconfigManager.inst.GetConfig(infoText.recordingContent);
        if (tempData == null)
        {
            Logger.error("转盘的记录itemId在表里不匹配");
            return;
        }
        contentText.text = LanguageManager.inst.GetValueByKey(tempData.name) + "*" + infoText.count;
        //contentText.color = GUIHelper.GetColorByColorHex(StaticConstants.LotteryQualityColor[infoText.quality]);
    }
}
