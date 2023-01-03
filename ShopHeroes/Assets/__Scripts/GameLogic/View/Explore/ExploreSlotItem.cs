using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExploreSlotItem : MonoBehaviour
{
    public Image refugeBgImg;
    public Image refugeCoolBg;
    public Image coolBg;
    public Button selfBtn;
    public GUIIcon icon;
    public GUIIcon refugeIcon;
    public Text timeText;
    private int index;
    kExploreSlotState slotState = kExploreSlotState.max;
    ExploreSlotData data;
    int timerId;

    [HideInInspector]
    public int slotIndex;//副本槽位所在位置
    [HideInInspector]
    public int slotGroup;//副本槽位所在组

    public liuguangcrl liuguangcrl;
    public GameObject animObj;
    private void Awake()
    {
        selfBtn.onClick.AddListener(onSlotBtnClick);
    }

    private void onSlotBtnClick()
    {
        if (slotState == kExploreSlotState.Exploring)
        {
            if (data == null) return;
            if (data.slotType != 2)
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.ROLEADVENTUREBYSLOT_SHOWUI, data.slotId);
            else
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_RefugeAdventure", data.slotId);
        }
        else if (slotState == kExploreSlotState.Finish)
        {
            if(timeText != null)
            {
                DOTween.Kill(timeText.rectTransform);
                timeText.transform.localScale = Vector3.one;
            }
            if (data == null) return;
            if (data.slotType != 2)
                EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREEND, index);
            else
                HotfixBridge.inst.TriggerLuaEvent("Request_RefugeEnd", data.slotId);
            data = null;
        }
    }

    public void InitData(int slotId)
    {
        index = slotId;
        timeText.text = LanguageManager.inst.GetValueByKey("冒险");
        icon.clear();
        refugeIcon.clear();
    }

    public void UpdateState(ExploreSlotData _data)
    {
        data = _data;
        if (data.exploreState == 0)
        {
            InitData(data.slotId);
            return;
        }
        index = data.slotId;
        slotState = (kExploreSlotState)data.exploreState;
        var tempData = ExploreInstanceConfigManager.inst.GetConfig(data.exploreId);
        //icon.SetSprite("", tempData.instance_icon);
        refugeBgImg.enabled = data.slotType == 2;
        if (data.slotType != 2)
        {
            coolBg.enabled = true;
            refugeCoolBg.enabled = false;
            refugeBgImg.enabled = false;
            icon.gameObject.SetActive(true);
            refugeIcon.gameObject.SetActive(false);
            var group = ExploreDataProxy.inst.GetGroupDataByGroupId(tempData.instance_group);
            setIconData(tempData, group);
        }
        else
        {
            coolBg.enabled = false;
            refugeCoolBg.enabled = true;
            refugeBgImg.enabled = true;
            icon.gameObject.SetActive(false);
            refugeIcon.gameObject.SetActive(true);
            setRefugeIcon();
        }
        if (data.exploreState == 1)
        {
            liuguangcrl.enabled = false;
            timeText.fontSize = 32;
            timeText.color = Color.white;

            coolBg.fillAmount = 1 - (float)data.exploringRemainTime / data.exploreTotalTime;
            refugeCoolBg.fillAmount = 1 - (float)data.exploringRemainTime / data.exploreTotalTime;

            StartExplore();
        }
        else if (data.exploreState == 2)
        {
            if (timerId > 0)
            {
                GameTimer.inst.RemoveTimer(timerId);
                timerId = 0;
            }
            timeText.text = LanguageManager.inst.GetValueByKey("就绪");
            timeText.fontSize = 38;
            timeText.color = GUIHelper.GetColorByColorHex("#ffd907");
            liuguangcrl.enabled = true;
            coolBg.fillAmount = 1;
            refugeCoolBg.fillAmount = 1;
            timeText.rectTransform.DOScale(1, 0.5f).From(1.3f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void setRefugeIcon()
    {
        refugeIcon.SetSprite(ExploreDataProxy.inst.refugeAtlas, ExploreDataProxy.inst.refugeIcon);
    }

    private void setIconData(ExploreInstanceConfigData tempData, ExploreGroup group)
    {
        if (data.exploreType == 4)
        {
            var monster = MonsterConfigManager.inst.GetConfig(tempData.boss_id);
            icon.SetSprite(monster.monster_atlas, monster.monster_icon);
        }
        else
        {
            var itemInfo = group.explores[data.exploreType - 1];
            var item = ItemconfigManager.inst.GetConfig(itemInfo.id);
            icon.SetSprite(item.atlas, item.icon);
        }
    }

    private void StartExplore()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
        if (timerId == 0)
        {
            timeText.text = TimeUtils.timeSpanStrip(data.exploringRemainTime);
            timerId = GameTimer.inst.AddTimer(1, () =>
             {
                 if (data.exploringRemainTime <= 0)
                 {
                     timeText.text = "0";
                     GameTimer.inst.RemoveTimer(timerId);
                     timerId = 0;
                 }
                 else
                 {
                     coolBg.fillAmount = 1 - (float)data.exploringRemainTime / data.exploreTotalTime;
                     refugeCoolBg.fillAmount = 1 - (float)data.exploringRemainTime / data.exploreTotalTime;
                     timeText.text = TimeUtils.timeSpanStrip(data.exploringRemainTime);
                 }
             });
        }
    }

    public void SetSelfBtnInteractable(bool interactable)
    {
        selfBtn.interactable = interactable;
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

    public void ClearTime()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }

    private void OnDisable()
    {

    }
}
