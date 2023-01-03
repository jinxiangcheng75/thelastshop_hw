using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionHotItem : MonoBehaviour, IDynamicScrollViewItem
{
    public GUIIcon unionIcon;
    //public Text unionLvTx;
    public Text unionNameTx;
    public Text memeberNumTx;
    public Text lowestLvTx;
    public Button joinBtn;

    UnionSimpleData _data;


    private void Start()
    {
        joinBtn.ButtonClickTween(onJoinBtnClick);
    }

    public void SetData(UnionSimpleData data)
    {
        _data = data;

        gameObject.SetActive(true);

        unionNameTx.text = data.unionName;
        //unionLvTx.text = data.unionLevel.ToString();
        memeberNumTx.text = LanguageManager.inst.GetValueByKey("成员数") + "<color=#ffc21d>" + data.memberNum + "/" + data.memberNumLimit + "</color>";
        lowestLvTx.text = LanguageManager.inst.GetValueByKey("最低等级{0}", data.enterLevel.ToString());
    }

    private void onJoinBtnClick()
    {
        if (UserDataProxy.inst.playerData.unionName != "")
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您当前已拥有联盟"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }

        if (_data.memberNum >= _data.memberNumLimit)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("该联盟已满员，请加入其它联盟！"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }

        EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_MSGBOX_ENTER, _data.unionId, _data.unionName);
    }


    int _index;
    public void onUpdateItem(int index)
    {
        _index = index;
    }
}
