using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class MakeSlot : MonoBehaviour
{
    public GUIIcon icon;
    public Text timeText;
    public Image coolBg;
    public GameObject liuguang;
    [HideInInspector]
    public int slotId = 0; //制造槽位 序列
    [HideInInspector]
    //int _slotIndex;
    public int slotIndex;//制作槽位所在位置
    [HideInInspector]
    //int _slotGroup;
    public int slotGroup;//制作槽位所在组
    //{
    //    get
    //    {
    //        return _slotGroup;
    //    }
    //    set
    //    {
    //        _slotGroup = value;
    //        //#if UNITY_EDITOR
    //        //            Logger.error("槽位下标为" + _slotIndex + "的制作槽   设置组为 " + _slotGroup);
    //        //#endif

    //    }
    //}

    public Func<bool> checkCanClickHandler;
    private Button getBtn
    {
        get
        {
            return gameObject.GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        }
    }

    [Header("动画")]
    public GameObject animObj;
    public GameObject slotDiLiuguangObj;

    public Animator BtnATor; //当前按钮动画
    [Header("----------------------------------")]
    private double endTime; //结束时间计算倒计时
    private int currEquipID;
    private bool makeing = false;
    public int makeState = 0;

    public int CurrEquipId
    {
        get { return currEquipID; }
        private set { }
    }

    Tween curTween;


    private void Start()
    {
        liuguang.SetActive(makeState == 2);
        getBtn.onClick.AddListener(slotOnclick);
    }

    private void slotOnclick()
    {
        if (checkCanClickHandler != null && !checkCanClickHandler()) return;
        if (makeState == 0)
        {
            //打开面板选择制作装备
            //  
            // EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_PRODUCTION_SELECT, slotId);
        }
        else if (makeState == 2)
        {
            //收取装备
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver && GuideDataProxy.inst.CurInfo.m_curCfg.btn_name != "1")
            {
                return;
            }
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver && gameObject.name != GuideDataProxy.inst.curMakeSlotName)
            {
                return;
            }
            AudioManager.inst.PlaySound(41);
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_MAKED, slotId);
            // itemBg.gameObject.SetActive(true);
        }
        else if (makeState == 1)
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver && ((K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick)) return;
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.UIHandle_SHOW_MAKINGSLOTINFO, slotId);
        }
    }
    public void ReInit(int id)
    {
        slotId = id;
        makeing = false;
        currEquipID = 0;
        //icon.sprite = "制作图标";
        coolBg.fillAmount = 1;
        timeText.color = GUIHelper.GetColorByColorHex("FFFFFF");
        timeText.text = LanguageManager.inst.GetValueByKey("制作");
        DOTween.Kill(timeText.rectTransform);
        timeText.transform.localScale = Vector3.one;
        makeState = 0;
        icon.clear();
        coolBg.fillAmount = 0;
    }

    //制作装备
    /// <param name="equipId">装备id</param>
    /// <param name="colTime">剩余完成时间</param>
    int timerId = 0;
    float makeTime = 0;
    public void StartMakeequip(int _equipDrawingId, double mackTime, double _totalTime)
    {
        //if (makeing) return;
        clearTimer();
        makeing = true;
        currEquipID = _equipDrawingId;
        endTime = mackTime;
        if (icon == null || timeText == null) return;
        makeTime = (float)_totalTime;
        EquipDrawingsConfig equipdrawings = EquipConfigManager.inst.GetEquipDrawingsCfg((int)_equipDrawingId);
        icon.SetSprite(equipdrawings.atlas, equipdrawings.icon);
        if (endTime == 0)
        {
            timeText.text = "";
        }
        else
        {
            timeText.text = TimeUtils.timeSpanStrip((int)endTime);
        }
        timeText.color = GUIHelper.GetColorByColorHex("FFFFFF");
        timeText.fontSize = 32;
        if (makeTime > 0)
        {
            coolBg.fillAmount = 1 - (float)endTime / makeTime;
            currentvalue = coolBg.fillAmount;
        }
        time = GameTimer.inst.serverNow;
    }
    int mEquipDrawingId = 0;
    public void UpdateState(int _state, int _equipDrawingId, double _maketime, double _totalTime)
    {

        mEquipDrawingId = _equipDrawingId;
        EquipDrawingsConfig equipdrawings = EquipConfigManager.inst.GetEquipDrawingsCfg(_equipDrawingId);

        if (_maketime <= 0 && _state != 0 && _state != 2)
        {
            _state = 2;
        }

        if (_state == 2 && makeState == 1)
        {
            if (BtnATor != null)
            {
                BtnATor.SetTrigger("MakeSlotJump");
            }
        }

        makeState = _state;
        if (_state == 0)
        {
            liuguang.SetActive(false);
            slotDiLiuguangObj.SetActive(false);
            clearTimer();
            ReInit(slotId);
        }
        else if (_state == 1)
        {
            liuguang.SetActive(false);
            slotDiLiuguangObj.SetActive(true);

            //制作中
            StartMakeequip(_equipDrawingId, _maketime, _totalTime);
        }
        else if (_state == 2)
        {
            clearTimer();
            liuguang.SetActive(true);
            slotDiLiuguangObj.SetActive(false);
            curTween?.Kill(true);
            timeText.color = GUIHelper.GetColorByColorHex("FFD907");
            timeText.text = LanguageManager.inst.GetValueByKey("领取");
            timeText.fontSize = 38;
            timeText.rectTransform.DOScale(1, 0.5f).From(1.3f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
            icon.SetSprite(equipdrawings.atlas, equipdrawings.icon);
            coolBg.fillAmount = 1;
        }

        //lua侧 刷新
        LuaListItem luaListItem = transform.GetComponent<LuaListItem>();

        if (luaListItem != null)
        {
            luaListItem.SetData(slotId);
        }

    }


    double time = 0;
    void Update()
    {
        if (makeState != 1 || makeing == false) return;
        if (GameTimer.inst.serverNow - time >= 1)
        {
            endTime -= GameTimer.inst.serverNow - time;
            time = GameTimer.inst.serverNow;
            updateTime();
        }
    }
    private void updateTime()
    {
        // if (makeState != 1 || makeing == false) return;
        // endTime -= 1f;
        if (endTime <= 0)
        {
            curTween?.Kill(true);
            timeText.text = "1" + LanguageManager.inst.GetValueByKey("秒");
            MakeEnd();
        }
        else
        {
            timeText.text = TimeUtils.timeSpanStrip((int)endTime);
            var value = 1 - (float)endTime / makeTime;
            curTween = DOTween.To(() => currentvalue, x => currentvalue = x, value, 1f).SetEase(Ease.Linear);
            curTween.onUpdate = CoolBarUpdate;
        }
    }
    private float currentvalue;
    private void CoolBarUpdate()
    {
        if (makeState != 1 || (makeing == false && coolBg.fillAmount != 1))
        {
            coolBg.fillAmount = 0;
        }
        else
            coolBg.fillAmount = currentvalue;
    }
    public void MakeEnd()
    {
        //结束发消息
        makeing = false;
        //coolBg.fillAmount = 1;
        ////消息回调
        //canGet = true;
        //timeText.text = "收取！";
        UpdateState(2, mEquipDrawingId, 0, 0);
    }

    void clearTimer()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;

        }
        makeing = false;
    }
    void OnDestroy()
    {
        clearTimer();
    }
    //public void

    public void SetSelfBtnInteractable(bool interactable)
    {
        getBtn.interactable = interactable;
    }

    ///动画
    public void SetAnimProgress(float progress)
    {
        Graphic[] graphics = animObj.GetComponentsInChildren<Graphic>();

        animObj.transform.localScale = Vector3.one * Mathf.Lerp(0.7f, 1f, progress);

        for (int i = 0; i < graphics.Length; i++)
        {
            Color temp = graphics[i].color;
            temp.a = Mathf.Lerp(0.5f, 1f, progress);
            graphics[i].color = temp;
        }
    }



}
