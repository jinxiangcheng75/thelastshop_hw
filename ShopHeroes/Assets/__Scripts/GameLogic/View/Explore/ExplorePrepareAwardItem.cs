using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorePrepareAwardItem : MonoBehaviour
{
    public GUIIcon icon;
    public Text numText;
    public Text nameText;
    public GameObject nameObj;

    public void setData(int itemId, int minNum, int maxNum, int addPercent, float addition, bool isSingle)
    {
        itemConfig cfg = ItemconfigManager.inst.GetConfig(itemId);
        numText.gameObject.SetActive(true);
        icon.SetSprite(cfg.atlas, cfg.icon);
        minNum += addPercent;
        maxNum += addPercent;
        minNum = Mathf.CeilToInt(minNum * addition);
        maxNum = Mathf.CeilToInt(maxNum * addition);

        var buildPercent = UserDataProxy.inst.GetExploreDropMaterialOutputUp(itemId);
        minNum = Mathf.CeilToInt(minNum * (1 + buildPercent));
        maxNum = Mathf.CeilToInt(maxNum * (1 + buildPercent));

        var buffCfg = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.explore_dropUp);
        if (buffCfg != null)
        {
            minNum = Mathf.CeilToInt(minNum * (1 + buffCfg.buffInfo.buffParam / 100.0f));
            maxNum = Mathf.CeilToInt(maxNum * (1 + buffCfg.buffInfo.buffParam / 100.0f));
        }

        numText.text = minNum + " - " + maxNum;
        nameText.text = LanguageManager.inst.GetValueByKey(cfg.name);
        numText.gameObject.SetActive(!isSingle);
        nameObj.SetActive(isSingle);
    }

    // difficult = 1 - 简单 2 - 正常 3 - 困难
    public void setRandomData(int difficulty, int keyIconIndex)
    {
        numText.text = StaticConstants.getKeyProbability[difficulty - 1];
        icon.SetSprite("item_atlas", (60000 + keyIconIndex).ToString());
    }
}
