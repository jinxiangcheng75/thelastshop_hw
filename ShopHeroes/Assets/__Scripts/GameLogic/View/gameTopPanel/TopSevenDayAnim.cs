using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TopSevenDayAnim : MonoBehaviour
{
    public GUIIcon bgIcon;
    public GUIIcon icon;
    public Slider slider;
    public Text scheduleText;
    public Button selfBtn;
    public Text contentText;
    public GameObject finishObj;

    public float startPos, endPos;
    private bool isPlayAnim;
    float timer;
    SevenDayGoalSingle curData;
    RectTransform rectTrans;

    bool needNextMsg = false;

    private void Awake()
    {
        if(selfBtn != null)
        {
            selfBtn.onClick.AddListener(() =>
            {
                if (curData != null)
                {
                    //EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.SEVENDAY_JUMP, curData.id);
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_WelfareUI", 1, curData.id);
                }
            });
        }
        
        rectTrans = transform as RectTransform;
    }

    public void SetSevenDayTaskData(SevenDayGoalSingle data)
    {
        curData = data;

        bool isDoing = data.state == ESevenDayTaskState.Doing;
        bgIcon.SetSprite("topplayer_atlas", isDoing ? "zhuejiemian_qiritishi" : "zhuejiemian_qiritishi1");
        var cfg = SevenDayTaskConfigManger.inst.GetConfig(data.id);
        icon.SetSprite(cfg.type_atlas, cfg.type_icon);
        if (!isDoing)
        {
            icon.SetSprite("topplayer_atlas", "gou");
            finishObj.SetActive(true);
            slider.gameObject.SetActive(false);
        }
        else
        {
            finishObj.SetActive(false);
            slider.gameObject.SetActive(true);
        }

        slider.maxValue = data.limit;
        if (data.process >= data.limit)
        {
            slider.value = data.limit;
            scheduleText.text = data.limit + "/" + data.limit;
        }
        else
        {
            slider.value = data.process;
            scheduleText.text = data.process + "/" + data.limit;
        }
        contentText.text = SevenDayGoalDataProxy.inst.setTaskDescByType(data);
        gameObject.SetActive(true);
        needNextMsg = false;
        playAnim();
    }

    public void PlayAnimShow()
    {
        gameObject.SetActive(true);
        needNextMsg = true;
        playAnim();
    }

    private void playAnim()
    {
        gameObject.SetActive(true);
        if (isPlayAnim)
        {
            rectTrans.anchoredPosition3D = new Vector3(rectTrans.anchoredPosition3D.x, startPos, rectTrans.anchoredPosition3D.z);
            gameObject.SetActive(false);
            isPlayAnim = false;
            playAnim();
            return;
        }
        rectTrans.DOAnchorPos3DY(endPos, 0.3f).From(startPos).OnStart(() =>
         {
             isPlayAnim = true;
         });
    }

    private void Update()
    {
        if (isPlayAnim)
        {
            timer += Time.deltaTime;

            if (timer >= 2)
            {
                timer = 0;
                rectTrans.DOAnchorPos3DY(startPos, 0.3f).From(endPos).OnComplete(() =>
                {
                    isPlayAnim = false;
                    gameObject.SetActive(false);
                    if (needNextMsg)
                    {
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
                    }
                });
            }
        }
    }
}
