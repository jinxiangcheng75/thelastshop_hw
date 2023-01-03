using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnionListItem : MonoBehaviour, IDynamicScrollViewItem
{
    public GUIIcon unionIcon;
    public Image bgImg;
    //public Text unionLvTx;
    public Text unionNameTx;
    public Text lowestLvTips;
    public Text memeberNumTips;
    private Button selfBtn;

    UnionSimpleData _data;

    public void SetData(UnionSimpleData data, string ifVal, int index)
    {
        _data = data;

        bgImg.enabled = index % 2 == 0;

        //unionLvTx.text = data.unionLevel.ToString();
        unionNameTx.text = ifVal + (ifVal.Length < _data.unionName.Length ? "<color=#7d6e6e>" + _data.unionName.Substring(ifVal.Length) + "</color>" : "");
        lowestLvTips.text = LanguageManager.inst.GetValueByKey("最低等级") + _data.enterLevel;
        memeberNumTips.text = _data.memberNum + "/" + _data.memberNumLimit + LanguageManager.inst.GetValueByKey("名成员");
    }

    private void Start()
    {
        selfBtn = GetComponent<Button>();
        if (selfBtn) selfBtn.onClick.AddListener(onSelfBtnClick);

    }

    private void onSelfBtnClick()
    {
        if (_data.unionId == UserDataProxy.inst.playerData.unionId)
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONINFO);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.UnionEvent.UNION_REQUEST_DATA, _data.unionId);
        }
    }

    int _index;
    public void onUpdateItem(int index)
    {
        _index = index;
    }
}
