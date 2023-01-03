using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyTaskBoxAwardDesPanel : ViewBase<DailyTaskBoxAwardDesPanelComp>
{

    public override string viewID => ViewPrefabName.DailyTaskBoxAwardDesPanel;
    public override string sortingLayerName => "popup";
    public override int showType => (int)ViewShowType.normal;


    ActiveRewardBoxData _data;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        //contentPane.randomAwardBgBtn.onClick.AddListener(onRandomAwardBgBtnClick);
        //contentPane.randomAwardBtn.ButtonClickTween(onRandomAwardBtnClick);
    }

    private void onRandomAwardBtnClick()
    {
        contentPane.randomAwardBgBtn.gameObject.SetActiveTrue();
        contentPane.ramdomAwardShowObj.gameObject.SetActiveTrue();
    }

    private void onRandomAwardBgBtnClick()
    {
        contentPane.randomAwardBgBtn.gameObject.SetActiveFalse();
        contentPane.ramdomAwardShowObj.gameObject.SetActiveFalse();
    }

    public void SetData(RectTransform boxTf, int activeTaskId)
    {
        Vector2 v2 = GUIHelper.GetFGuiCameraUIPointByWorldPos(boxTf.position);
        Vector2 pos = new Vector3(v2.x + 120, v2.y + 206f, 0);

        float screenWidth = FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.width / 2;
        float bgWidth = contentPane.boxBgTf.rect.width / 2;


        if (pos.x < -screenWidth + bgWidth)
        {
            contentPane.boxBgTf.localScale = new Vector3(-1, -1, 1);
            pos = new Vector2(-screenWidth + bgWidth, pos.y);
        }
        else if (pos.x > screenWidth - bgWidth)
        {
            contentPane.boxBgTf.localScale = new Vector3(1, -1, 1);
            //pos = new Vector2(screenWidth - bgWidth, pos.y);
            pos = new Vector2(v2.x - 120, pos.y);
        }
        else
        {
            contentPane.boxBgTf.localScale = new Vector3(-1, -1, 1);
        }

        contentPane.boxDesTf.anchoredPosition = pos;

        _data = UserDataProxy.inst.GetActiveRewardBoxData(activeTaskId);
        var itemCfg = ItemconfigManager.inst.GetConfig(_data.config.reward1_id);
        if (itemCfg != null)
        {
            contentPane.awardIcon_1.SetSprite(itemCfg.atlas, itemCfg.icon);
            contentPane.tx_awardCount_1.text = "X" + _data.config.reward1_num.ToString();
            contentPane.tx_awardName_1.text = LanguageManager.inst.GetValueByKey(itemCfg.name);
        }
        itemCfg = ItemconfigManager.inst.GetConfig(_data.config.reward2_id);
        if (itemCfg != null)
        {
            contentPane.awardIcon_2.SetSprite(itemCfg.atlas, itemCfg.icon);
            contentPane.tx_awardCount_2.text = "X" + _data.config.reward2_num.ToString();
            contentPane.tx_awardName_2.text = LanguageManager.inst.GetValueByKey(itemCfg.name);
        }

        //string content = "";

        //for (int i = 0; i < _data.randomAwards.Count; i++)
        //{
        //    ActiveRewardData data = _data.randomAwards[i];
        //    content += LanguageManager.inst.GetValueByKey("奖品") + (i + 1) + " " + data.itemConfig.name + "    " + LanguageManager.inst.GetValueByKey("概率") + (data.probability * 100).ToString("f2") + "%" + "\n";
        //}

        //contentPane.ramdomAwardShowTx.text = content;
        //Vector2 size = (contentPane.ramdomAwardShowObj.transform as RectTransform).sizeDelta;
        //size.y = contentPane.ramdomAwardShowTx.preferredHeight + 40f;
        //(contentPane.ramdomAwardShowObj.transform as RectTransform).sizeDelta = size;
    }

    protected override void onHide()
    {
        //onRandomAwardBgBtnClick();
    }


}
