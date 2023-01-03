using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.InputField;

public class UnionSetSettingUI : ViewBase<UnionSetSettingUIComp>
{
    public override string viewID => ViewPrefabName.UnionSetSettingUI;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.enterLeftBtn.onClick.AddListener(() => { PlayButtonClickSound(12); rangeIndex -= 1; });
        contentPane.enterRightBtn.onClick.AddListener(() => { PlayButtonClickSound(12); rangeIndex += 1; });
        contentPane.lvLeftBtn.onClick.AddListener(() => { PlayButtonClickSound(12); enterlevel -= 1; });
        contentPane.lvRightBtn.onClick.AddListener(() => { PlayButtonClickSound(12); enterlevel += 1; });

        contentPane.lowestLvField.onEndEdit.AddListener(onLowestLvFieldEndEdit);
        contentPane.investField.onEndEdit.AddListener(onInvestFieldEndEdit);

        contentPane.cancelBtn.ButtonClickTween(hide);
        contentPane.confirmBtn.ButtonClickTween(onConfirmBtnClick);
    }

    private void PlayButtonClickSound(int soundId)
    {
        AudioManager.inst.PlaySound(soundId);
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
        rangeIndex = UserDataProxy.inst.unionDetailInfo.enterSetting;
        enterlevel = UserDataProxy.inst.unionDetailInfo.enterLevel;
        joinInvsetVal = UserDataProxy.inst.unionDetailInfo.enterInvest;
        contentPane.nameTx.text = UserDataProxy.inst.unionDetailInfo.unionName;
        contentPane.nameField.text = "";
        onInvestFieldEndEdit(UserDataProxy.inst.unionDetailInfo.enterInvest.ToString());
        contentPane.lowestLvField.text = UserDataProxy.inst.unionDetailInfo.enterLevel.ToString();
    }

    protected override void onHide()
    {
        base.onHide();
        //AudioManager.inst.PlaySound(11);
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


    void onConfirmBtnClick()
    {
        //string str = contentPane.nameField.text.Trim();

        //int strLen = Helper.GetStringRealLen(str);

        //if (strLen < 2 || strLen > contentPane.nameField.characterLimit)
        //{
        //    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("联盟名称长度需要处于2与14字符之间"), GUIHelper.GetColorByColorHex("FF2828"));
        //    return;
        //}


        //string outStr;
        //if (WordFilter.inst.filter(str, out outStr, check_only: true))
        //{
        //    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称中包含敏感内容"), GUIHelper.GetColorByColorHex("FF2828"));
        //    return;
        //}

        string str = string.Empty;

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_SETINFO, str, rangeIndex, enterlevel, joinInvsetVal);
    }

    private void onLowestLvFieldEndEdit(string lowestLvVal)
    {
        enterlevel = Mathf.Clamp(int.Parse(lowestLvVal), 1, StaticConstants.UnionEnterLvMax);
    }


    long joinInvsetVal;

    private void onInvestFieldEndEdit(string investVal)
    {
        investVal = investVal.Replace(",","");

        if (long.TryParse(investVal, out long val))
        {
            if (val < 0)
            {
                val = -val;
            }
            joinInvsetVal = val;
            contentPane.investField.contentType = UnityEngine.UI.InputField.ContentType.Standard;
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


    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }


}
