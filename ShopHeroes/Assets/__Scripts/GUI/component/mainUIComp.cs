using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class mainUIComp : MonoBehaviour
{

    [Header("-底部按钮-")]
    //制作
    public Button makeBtn;
    public Transform makeSlotListContent;
    public GameObject makeSlotItemGO;

    public Image addImg;
    public Text idleSlotNumTx;
    //仓库
    public Button openBagBtn;
    //城市
    public Button cityBtn;
    public GameObject city_redPoint;
    //任务
    public Button taskBtn;
    public GameObject task_redPoint;
    //英雄
    public Button heroBtn;
    public Text heroIdleCount;
    public GameObject heroIdleBg;
    public GameObject workerRedPoint;
    //设计
    public Button btn_design;
    //聊天
    public Button btn_Chat;
    public GameObject chatBtnRedPoint;

    [Header("顶部")]
    //全服buff
    public GlobalBuffItem[] globalBuffItems;
    //豪华度
    public LuaListItem luxuryItem;

    [Header("-左侧按钮-")]
    //排行
    public Button btn_totalRank;
    //福利
    public Button btn_welfare;
    //绑定
    public Button bindingBtn;
    public GUIIcon bindingIcon;
    public Text bindingText;
    //切换屏幕
    public Button btn_landscape;
    //爬塔
    public Button refugeBtn;
    public Text refugeTimeText;


    [Header("右侧按钮")]
    public VerticalLayoutGroup rightBtnsVLG_row1;
    //商城
    public Button btn_mall;
    public GameObject redPoint_mall;
    //直购礼包
    public LuaBehaviour commonGiftParent_luaBehaviour;
    //在线领奖
    public LuaBehaviour onlineReward_luaBehaviour;

    [Header("anim")]
    public RectTransform leftAnimTf;//left
    public RectTransform rightAnimTf; //right

    //bottom
    public RectTransform bottomAniTf;
    //cityBtn
    public RectTransform bottomCityBtnImg;
    public Text bottomCityBtnTx;
    //heroBtn
    public RectTransform bottomHeroBtnImg;
    public Text bottomHeroBtnTx;
    //taskBtn
    public RectTransform bottomTaskBtnImg;
    public Text bottomTaskBtnTx;
    //bagBtn
    public RectTransform bottomBagBtnImg;
    public Text bottomBagBtnTx;
    //design
    public RectTransform bottomDesignBtnImg;
    public Text bottomDesignBtnTx;

    //makeSlot
    public RectTransform makeBtnTf;

    public OverrideScrollRect slotSR;
    public RectTransform[] makeSlotSigns;

    public GameObject allParentObj;


#if  UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GUIManager.OpenView<SpineTestUI>();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            //Award_AboutVal triggerData = new Award_AboutVal() { type = ReceiveInfoUIType.StarUpEffectTrigger_return, val = 2024 };
            //EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, triggerData);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //Award_AboutVal triggerData = new Award_AboutVal() { type = ReceiveInfoUIType.StarUpEffectTrigger_double, val = 10074 };
            //EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, triggerData);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            //Award_AboutVal triggerData = new Award_AboutVal() { type = ReceiveInfoUIType.StarUpEffectTrigger_super, val = 10074 };
            //EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, triggerData);
            HotfixBridge.inst.TriggerLuaEvent("AddRaceLampTip", 6, 50009, "大二臂", "176170");
        }
    }
#endif
}
