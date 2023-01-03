using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoleHeroItemComp : MonoBehaviour
{
    public GameObject heroResObj;
    public GameObject heroFindingObj;
    public GameObject heroIdleObj;
    public Text heroLevelText;
    public GUIIcon heroTypeBgIcon;
    public GUIIcon heroTypeIcon;
    public GUIIcon qualityIcon;
    public Text heroNameText;
    public GUIIcon newintelligenceIcon;
    public Text fightingText;
    public Slider heroResSlider;
    public Text heroResTimeText;
    public Text heroFidingTimeText;
    public Button treatBtn;
    public Image canTransfer;
    public Text transferText;
    public Transform upImg;
    public List<GUIIcon> allStars;
    public GameObject noEquipObj;
    private ERoleState roleState;
    public Image redPoint;
    public Image grayImage;

    public int timerId;

    //头像
    public Transform headParent;
    GraphicDressUpSystem graphicDressUp = null;
    RoleHeroData curData;

    public void InitHeroData(RoleHeroData data)
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
        curData = data;
        roleState = (ERoleState)data.currentState;
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        string qualityColor = StaticConstants.roleIntelligenceColor[rarity];
        heroLevelText.text = data.level.ToString();
        heroNameText.text = LanguageManager.inst.GetValueByKey(data.nickName);
        heroNameText.color = GUIHelper.GetColorByColorHex(qualityColor);
        fightingText.text = data.fightingNum.ToString();

        qualityIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        newintelligenceIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleIntelligenceIconStr[rarity - 1]);

        noEquipObj.SetActive(data.equip1.equipId == 0);


        //if (RoleDataProxy.inst.heroGraphicDressDic.ContainsKey(data.uid))
        //{

        //    if (graphicDressUp != null && RoleDataProxy.inst.heroGraphicDressDic[data.uid] == graphicDressUp && graphicDressUp.transform.parent == headParent)
        //    {

        //    }
        //    else
        //    {
        //        if (graphicDressUp != null && graphicDressUp.transform.parent == headParent)
        //        {
        //            graphicDressUp.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
        //            graphicDressUp.transform.localPosition = Vector3.zero;
        //            graphicDressUp.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        //            graphicDressUp = null;
        //        }

        //        graphicDressUp = RoleDataProxy.inst.heroGraphicDressDic[data.uid];

        //        if (graphicDressUp != null)
        //        {
        //            graphicDressUp.transform.SetParent(headParent);
        //            graphicDressUp.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        //            graphicDressUp.transform.localPosition = Vector3.zero;
        //        }
        //    }

        //}
        //else
        //{
        //    if (graphicDressUp != null)
        //    {
        //        graphicDressUp.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
        //        graphicDressUp.transform.localPosition = Vector3.zero;
        //        graphicDressUp = null;
        //    }
        //}

        if (graphicDressUp != null && graphicDressUp.transform.parent == headParent)
        {
            graphicDressUp.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
            graphicDressUp.transform.localPosition = Vector3.zero;
            graphicDressUp.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            graphicDressUp = null;
        }

        RoleDataProxy.inst.resetHeroGraphicDress(data.uid, (system) =>
        {
            graphicDressUp = system;
            graphicDressUp.transform.SetParent(headParent);
            graphicDressUp.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            graphicDressUp.transform.localPosition = Vector3.zero;
        });

        heroTypeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        heroTypeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);
        setStarShow();
        setHeroState(data);
        JudgeCanTransfer();
        JudgeHasRedPoint();
    }

    private void JudgeHasRedPoint()
    {
        if(curData.hasRedPoint != 2 && curData.currentState != 2)
        {
            var bestList = RoleDataProxy.inst.GetHeroBestEquips(curData.uid);
            if (bestList.Count > 0)
            {
                redPoint.enabled = true;
                curData.hasRedPoint = 1;
            }
            else
            {
                redPoint.enabled = false;
            }
        }
        else
        {
            redPoint.enabled = false;
        }
    }

    private void JudgeCanTransfer()
    {
        List<HeroProfessionConfigData> canTransferList = HeroProfessionConfigManager.inst.GetTransferData(curData.id, curData.intelligence);
        if (canTransferList.Count > 0/* && !transferPromptIsShow*/)
        {
            for (int i = 0; i < canTransferList.Count; i++)
            {
                if (curData.level >= canTransferList[i].level_need)
                {
                    bool isArrive = true;
                    var needMatList = canTransferList[i].GetHeroProfessionNeedMatDatas();
                    for (int k = 0; k < needMatList.Count; k++)
                    {
                        Item voucherItem = ItemBagProxy.inst.GetItem(needMatList[k].itemId);
                        if (voucherItem != null)
                        {
                            if (voucherItem.count < needMatList[k].needItemCount)
                            {
                                isArrive = false;
                                break;
                            }
                        }
                        else
                        {
                            isArrive = false;
                            break;
                        }
                    }

                    if (isArrive)
                    {
                        canTransfer.enabled = true;
                        break;
                    }
                    else
                        canTransfer.enabled = false;
                }
                else
                {
                    canTransfer.enabled = false;
                }
            }
        }
        else
        {
            canTransfer.enabled = false;
        }
    }

    private void setStarShow()
    {
        for (int i = 0; i < allStars.Count; i++)
        {
            int index = i;
            if (index < curData.config.hero_grade)
            {
                allStars[index].gameObject.SetActive(true);
                allStars[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng");
            }
            else
            {
                if (index < curData.transferNumLimit)
                {
                    allStars[index].gameObject.SetActive(true);
                    allStars[index].SetSprite(StaticConstants.roleAtlasName, "yingxiong_xingcheng1");
                }
                else
                {
                    allStars[index].gameObject.SetActive(false);
                }
            }
        }
    }

    private void setHeroState(RoleHeroData data)
    {
        //grayObj.SetActive(roleState == ERoleState.Fighting);
        grayImage.enabled = roleState == ERoleState.Fighting;
        heroResObj.SetActive(roleState == ERoleState.Resting);
        heroFindingObj.SetActive(roleState == ERoleState.Fighting);
        heroIdleObj.SetActive(roleState == ERoleState.Idle);
        switch (roleState)
        {
            case ERoleState.Resting:
                treatBtn.onClick.RemoveAllListeners();
                treatBtn.ButtonClickTween(() =>
                {
                    if (GuideDataProxy.inst == null || GuideDataProxy.inst.CurInfo == null || !GuideDataProxy.inst.CurInfo.isAllOver) return;
                    EventController.inst.TriggerEvent(GameEventType.RoleEvent.SINGLEROLERESTING_SHOWUI, data, 0);
                });
                heroResSlider.maxValue = data.endTime - data.startTime;
                if (timerId == 0)
                {
                    heroResSlider.value = data.endTime - data.startTime - data.remainTime;
                    if (data.remainTime <= 0)
                        heroResTimeText.text = "1" + LanguageManager.inst.GetValueByKey("秒");
                    else
                        heroResTimeText.text = TimeUtils.timeSpanStrip(data.remainTime);
                    timerId = GameTimer.inst.AddTimer(1, () =>
                    {
                        if (data.remainTime <= 0)
                        {
                            heroResTimeText.text = "1" + LanguageManager.inst.GetValueByKey("秒");
                            data = RoleDataProxy.inst.GetHeroDataByUid(data.uid);
                            if (data.currentState == 0)
                            {
                                InitHeroData(data);
                            }
                            GameTimer.inst.RemoveTimer(timerId);
                            timerId = 0;
                        }
                        else
                        {
                            heroResSlider.value = data.endTime - data.startTime - data.remainTime;
                            heroResTimeText.text = TimeUtils.timeSpanStrip(data.remainTime);
                        }
                    });
                }

                break;
            case ERoleState.Fighting:
                if (timerId == 0)
                {
                    if (data.remainTime > 0)
                        heroFidingTimeText.text = TimeUtils.timeSpanStrip(data.remainTime);
                    else
                        heroFidingTimeText.text = LanguageManager.inst.GetValueByKey("完成");
                    timerId = GameTimer.inst.AddTimer(1, () =>
                    {
                        if (data.remainTime <= 0)
                        {
                            heroFidingTimeText.text = LanguageManager.inst.GetValueByKey("完成");
                            GameTimer.inst.RemoveTimer(timerId);
                            timerId = 0;
                        }
                        else
                        {
                            heroFidingTimeText.text = TimeUtils.timeSpanStrip(data.remainTime);
                        }
                    });
                }
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (graphicDressUp != null && graphicDressUp.transform.parent == headParent)
        {
            graphicDressUp.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
            graphicDressUp.transform.localPosition = Vector3.zero;
            graphicDressUp.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            graphicDressUp = null;
        }
    }
}
