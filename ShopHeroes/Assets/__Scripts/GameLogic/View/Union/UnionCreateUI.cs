using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.InputField;

public class UnionCreateUI : ViewBase<UnionCreateUIComp>
{

    public override string viewID => ViewPrefabName.UnionCreateUI;
    public override string sortingLayerName => "window";


    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.goldBtn.ButtonClickTween(() => onBtnClick(0, goleCost));
        contentPane.gemBtn.ButtonClickTween(() =>
        {
            if (contentPane.gemAffirmObj.activeSelf)
            {
                onBtnClick(1, gemCost);
            }
            else
            {
                contentPane.gemAffirmObj.SetActive(true);
                contentPane.gemTip.text = LanguageManager.inst.GetValueByKey("确定");
            }
        });

        contentPane.enterLeftBtn.onClick.AddListener(() => rangeIndex -= 1);
        contentPane.enterRightBtn.onClick.AddListener(() => rangeIndex += 1);
        contentPane.lvLeftBtn.onClick.AddListener(() => enterlevel -= 1);
        contentPane.lvRightBtn.onClick.AddListener(() => enterlevel += 1);
        contentPane.lowestLvField.onEndEdit.AddListener(onLowestLvFieldEndEdit);
        contentPane.investField.onEndEdit.AddListener(onInvestFieldEndEdit);

        goleCost = (int)WorldParConfigManager.inst.GetConfig(130).parameters;
        gemCost = (int)WorldParConfigManager.inst.GetConfig(131).parameters;

        contentPane.goldTx.text = goleCost.ToString("N0");
        contentPane.gemTx.text = gemCost.ToString("N0");
    }

    protected override void onShown()
    {
        base.onShown();
        //AudioManager.inst.PlaySound(10);
        contentPane.nameField.text = "";
        contentPane.investField.text = 0.ToString();
        contentPane.gemAffirmObj.SetActive(false);

        rangeIndex = 0;
        enterlevel = 1;
        joinInvsetVal = 0;

        contentPane.goldTx.color = UserDataProxy.inst.playerData.gold < goleCost ? GUIHelper.GetColorByColorHex("FD4F4F") : Color.white;
        //contentPane.gemTx.color = UserDataProxy.inst.playerData.gem < gemCost ? GUIHelper.GetColorByColorHex("FD4F4F") : Color.white;
    }

    protected override void onHide()
    {
        base.onHide();
        //AudioManager.inst.PlaySound(11);
    }

    private void onLowestLvFieldEndEdit(string lowestLvVal)
    {
        enterlevel = Mathf.Clamp(int.Parse(lowestLvVal), 1, StaticConstants.UnionEnterLvMax);
    }

    long joinInvsetVal = 0;

    private void onInvestFieldEndEdit(string investVal)
    {
        investVal = investVal.Replace(",", "");

        if (long.TryParse(investVal, out long val))
        {
            if (val < 0)
            {
                val = -val;
            }
            joinInvsetVal = val;
            contentPane.investField.contentType = InputField.ContentType.Standard;
            contentPane.investField.lineType = LineType.MultiLineNewline;
            contentPane.investField.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            contentPane.investField.text = joinInvsetVal.ToString("N0");
        }
        else
        {
            if (investVal.Length > 18)
            {
                onInvestFieldEndEdit(long.MaxValue.ToString());
            }
            else
            {
                joinInvsetVal = 0;
                contentPane.investField.text = 0.ToString();
            }

        }
    }


    int _rangeIndex;

    int rangeIndex
    {
        get
        {
            return _rangeIndex;
        }
        set
        {

            if (value == StaticConstants.unionRange.Length)
            {
                _rangeIndex = 0;
            }
            else if (value == -1)
            {
                _rangeIndex = StaticConstants.unionRange.Length - 1;
            }
            else
            {
                _rangeIndex = value;
            }

            contentPane.enterTx.text = LanguageManager.inst.GetValueByKey(StaticConstants.unionRange[_rangeIndex]);

        }
    }

    int _enterlevel;

    int enterlevel
    {
        get
        {
            return _enterlevel;
        }
        set
        {

            if (value >= StaticConstants.UnionEnterLvMax + 1)
            {
                _enterlevel = 1;
            }
            else if (value <= 0)
            {
                _enterlevel = StaticConstants.UnionEnterLvMax;
            }
            else
            {
                _enterlevel = value;
            }
            contentPane.lowestLvField.text = _enterlevel.ToString();
        }
    }

    int goleCost, gemCost;



    private void onBtnClick(int moneyType, int moneyNum)
    {
        string str = contentPane.nameField.text.Trim();

        int strLen = Helper.GetStringRealLen(str);

        if (strLen < 2 || strLen > contentPane.nameField.characterLimit)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("联盟名称长度需要处于2与14字符之间"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }

        string outStr;
        if (WordFilter.inst.filter(str, out outStr, check_only: true))
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称中包含敏感内容"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }


        if (moneyType == 0)
        {
            if (UserDataProxy.inst.playerData.gold < moneyNum)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
        }
        else
        {
            if (UserDataProxy.inst.playerData.gem < moneyNum)
            {
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, moneyNum - UserDataProxy.inst.playerData.gem);
                return;
            }
        }

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_CREATE, new Request_createUnionClientData() { unionName = str, enterSetting = (EUnionEnter)rangeIndex, enterLevel = enterlevel, useGem = moneyType, enterInvest = joinInvsetVal });
    }

}
