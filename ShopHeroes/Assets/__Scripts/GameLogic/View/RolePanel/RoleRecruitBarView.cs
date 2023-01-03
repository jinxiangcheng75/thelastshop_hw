using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleRecruitBarView : ViewBase<RoleRecruitBarComp>
{
    public override string viewID => ViewPrefabName.RoleRecruitBarPanel;
    public override string sortingLayerName => "window";
    int timerId;
    List<int> animTimerIds = new List<int>();
    //bool isRefreshing = false;
    const string PLAYERPREFS_ANIM_KEY = "RoleRecruitAnimSkipFlag";

    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.noRoleAndSettingAndEnergy;
        isShowResPanel = true;
        AddUIEvent();

        float tempSize = FGUI.inst.uiRootTF.sizeDelta.x > FGUI.inst.uiRootTF.sizeDelta.y ? FGUI.inst.uiRootTF.sizeDelta.x + 100 : FGUI.inst.uiRootTF.sizeDelta.y + 100;
        contentPane.bgRect.sizeDelta = Vector2.one * tempSize;
    }

    public void InitRecruitHeroData()
    {
        RoleDataProxy rdp = RoleDataProxy.inst;
        HotfixBridge.inst.TriggerLuaEvent("RefreshUI_RoleRecruitBar");
        contentPane.refreshGemObj.enabled = rdp.costType == EGoldOrGem.Gem;
        //contentPane.refreshGoldObj.enabled = rdp.costType == EGoldOrGem.Gold;
        if (WorldParConfigManager.inst.GetConfig(304).parameters - RoleDataProxy.inst.refreshCount == 0)
            contentPane.probebilityText.text = LanguageManager.inst.GetValueByKey("下次必出<color=#f36cf9>SR</color>");
        else
            contentPane.probebilityText.text = LanguageManager.inst.GetValueByKey("再刷新<color=#72e75b>{0}</color>次必出<color=#f36cf9>SR</color>", (WorldParConfigManager.inst.GetConfig(304).parameters - RoleDataProxy.inst.refreshCount).ToString());
        contentPane.probebilitySlider.maxValue = WorldParConfigManager.inst.GetConfig(304).parameters;
        contentPane.probebilitySlider.value = Mathf.Max(WorldParConfigManager.inst.GetConfig(304).parameters * 0.05f, RoleDataProxy.inst.refreshCount);
        contentPane.probebilitySliderTx.text = RoleDataProxy.inst.refreshCount + "/" + WorldParConfigManager.inst.GetConfig(304).parameters;

        if (rdp.costValue > 0)
        {
            if (!RoleDataProxy.inst.recruitIsRefreshing)
                setRecruitHeroItem();
            contentPane.probebilityText.gameObject.SetActive(true);
            contentPane.refreshBtn.gameObject.SetActive(true);
            contentPane.freeRefreshBtn.gameObject.SetActive(false);

            var gemIcon = contentPane.refreshGemObj.GetComponent<GUIIcon>();
            var itemCfg = ItemconfigManager.inst.GetConfig(10002);
            if(itemCfg != null)
            {
                gemIcon.iconImage.enabled = true;
                gemIcon.SetSprite(itemCfg.atlas, itemCfg.icon);
            }
            else
            {
                gemIcon.iconImage.enabled = false;
            }
            //contentPane.timeText.gameObject.SetActive(true);
            contentPane.diamondText.text = rdp.costValue.ToString();
            //contentPane.diamondText.color = rdp.costValue > UserDataProxy.inst.playerData.gem ? GUIHelper.GetColorByColorHex("FF0000") : GUIHelper.GetColorByColorHex("FFFFFF");
        }
        else
        {
            if (!RoleDataProxy.inst.recruitIsRefreshing)
                setRecruitFreeItem();
            contentPane.probebilityText.gameObject.SetActive(false);
            //contentPane.windowObj.SetActive(false);
            RoleDataProxy.inst.costType = 0;
            contentPane.refreshBtn.gameObject.SetActive(false);
            contentPane.freeRefreshBtn.gameObject.SetActive(true);
            if (RoleDataProxy.inst.recruitIsNew)
            {
                contentPane.redPoint.SetActive(true);
                RoleDataProxy.inst.recruitIsNew = false;
            }
            contentPane.timeText.gameObject.SetActive(false);
        }

        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        if (RoleDataProxy.inst.nextRefreshTime > 0)
        {
            contentPane.timeText.gameObject.SetActive(true);
            contentPane.timeText.text = TimeUtils.timeSpan3Str(RoleDataProxy.inst.nextRefreshTime);
            timerId = GameTimer.inst.AddTimer(1, rdp.nextRefreshTime, RefreshTimeContent);
        }
        else
        {
            contentPane.timeText.gameObject.SetActive(false);
        }

    }

    public void setRecruitHeroItem()
    {
        List<RoleRecruitData> tempList = RoleDataProxy.inst.recruitList;
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int index = i;
            RoleRecruitData tempData = RoleDataProxy.inst.GetCurIndexRecruitData(index);

            if (tempData == null)
            {
                contentPane.allHeroes[index].setFreeItem();
            }
            else
            {
                contentPane.allHeroes[index].setData(tempData, index, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
                contentPane.allHeroes[index].recruitClickHandler = ClickRecruitBtnHandler;
            }
        }
    }

    public void setRecruitFreeItem()
    {
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int index = i;
            contentPane.allHeroes[index].setFreeItem();
        }
    }

    private void ClickRecruitBtnHandler(RoleRecruitData heroData, int index)
    {
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLERECRUITSUB_SHOWUI, heroData, index);
    }

    private void RefreshTimeContent()
    {
        if (RoleDataProxy.inst.nextRefreshTime <= 0)
        {
            //EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RECRUITLIST);
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
        else
        {
            contentPane.timeText.text = TimeUtils.timeSpan3Str(RoleDataProxy.inst.nextRefreshTime);
        }
    }

    private void AddUIEvent()
    {
        contentPane.closeBtn.ButtonClickTween(() =>
        {
            if (RoleDataProxy.inst.recruitIsRefreshing)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("正在刷新卡牌中"), GUIHelper.GetColorByColorHex("FFFFFF"));
                return;
            }
            hide();
        });

        contentPane.btn_probabilityPublic.ButtonClickTween(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowProbabilityPublic", 0);
        });

        contentPane.refreshBtn.ButtonClickTween(() =>
        {
            clickRefreshMethod((int)RoleDataProxy.inst.costType);

        });
        contentPane.freeRefreshBtn.ButtonClickTween(() =>
        {
            clickRefreshMethod(0);
        });

        contentPane.Toggle_skipAnim.onValueChanged.AddListener((isOn) =>
        {
            SaveManager.inst.SaveInt(PLAYERPREFS_ANIM_KEY, isOn ? 1 : 0);
        });

    }

    private void clickRefreshMethod(int costType)
    {
        if (RoleDataProxy.inst.recruitIsRefreshing)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("正在刷新卡牌中"), GUIHelper.GetColorByColorHex("FFFFFF"));
            return;
        }

        if (costType == 0 || costType == 1)
        {
            var list = RoleDataProxy.inst.recruitList;
            for (int i = 0; i < list.Count; i++)
            {
                var curData = list[i];
                if (RoleDataProxy.inst.ReturnRarityByAptitude(curData.intelligence) >= 3 && curData.recruitState == 0)
                {
                    EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OKCANCLE_MSGBOX, "当前有高品质英雄尚未招募，确认是否继续刷新？", () =>
                    {
                        EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RECRUITREFRESH, costType);
                    });
                    return;
                }
            }
            RoleDataProxy.inst.recruitIsRefreshing = true;
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RECRUITREFRESH, costType);
        }
        else
        {
            if (UserDataProxy.inst.playerData.gem >= RoleDataProxy.inst.costValue)
            {
                var list = RoleDataProxy.inst.recruitList;
                for (int i = 0; i < list.Count; i++)
                {
                    var curData = list[i];
                    if (RoleDataProxy.inst.ReturnRarityByAptitude(curData.intelligence) >= 3 && curData.recruitState == 0)
                    {
                        EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OKCANCLE_MSGBOX, "当前有高品质英雄尚未招募，确认是否继续刷新？", () =>
                        {
                            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RECRUITREFRESH, costType);
                        });
                        return;
                    }
                }

                RoleDataProxy.inst.recruitIsRefreshing = true;
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RECRUITREFRESH, costType);
                //if (contentPane.sureAgainObj.activeSelf)
                //{
                //    isRefreshing = true;
                //    contentPane.sureAgainObj.SetActive(false);
                //    EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RECRUITREFRESH, costType);
                //}
                //else
                //    contentPane.sureAgainObj.SetActive(true);
            }
            else
            {
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, RoleDataProxy.inst.costValue - UserDataProxy.inst.playerData.gem);
            }
        }

        if (RoleDataProxy.inst.recruitIsRefreshing)
        {
            if (contentPane.Toggle_skipAnim.isOn)
            {
               // RoleDataProxy.inst.recruitIsRefreshing = false;//网络回调中直接刷新
            }
            else
            {
               // doCardsAnim();
            }
        }
    }

    public void doCardsAnim()
    {
        AudioManager.inst.PlaySound(109);
        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            int index = i;

            int animTimerId = GameTimer.inst.AddTimer(index * 0.4f, 1, () =>
            {
                if (index == contentPane.allHeroes.Count - 1)
                    contentPane.allHeroes[index].setRefreshAnim(() =>
                    {
                        RoleDataProxy.inst.recruitIsRefreshing = false;
                        RoleRecruitData tempData = RoleDataProxy.inst.GetCurIndexRecruitData(index);
                        contentPane.allHeroes[index].setData(tempData, index, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
                        contentPane.allHeroes[index].recruitClickHandler = ClickRecruitBtnHandler;
                    });
                else
                {
                    contentPane.allHeroes[index].setRefreshAnim(() =>
                    {
                        RoleRecruitData tempData = RoleDataProxy.inst.GetCurIndexRecruitData(index);
                        contentPane.allHeroes[index].setData(tempData, index, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
                        contentPane.allHeroes[index].recruitClickHandler = ClickRecruitBtnHandler;
                    });
                }
            });

            animTimerIds.Add(animTimerId);

        }

    }

    void skipCardsAnim()
    {
        if (!RoleDataProxy.inst.recruitIsRefreshing)
        {
            return;
        }

        for (int i = 0; i < animTimerIds.Count; i++)
        {
            GameTimer.inst.RemoveTimer(animTimerIds[i]);
        }
        animTimerIds.Clear();

        for (int i = 0; i < contentPane.allHeroes.Count; i++)
        {
            var heroCardItem = contentPane.allHeroes[i];
            heroCardItem.ClearAnim();
            RoleRecruitData tempData = RoleDataProxy.inst.GetCurIndexRecruitData(i);
            heroCardItem.setData(tempData, i, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
            heroCardItem.recruitClickHandler = ClickRecruitBtnHandler;
        }

        RoleDataProxy.inst.recruitIsRefreshing = false;

    }


    protected override void onShown()
    {
        int flag = 0;

        if (SaveManager.inst.HasKey(PLAYERPREFS_ANIM_KEY))
        {
            flag = SaveManager.inst.GetInt(PLAYERPREFS_ANIM_KEY);
        }
        else
        {
            SaveManager.inst.SaveInt(PLAYERPREFS_ANIM_KEY, 0);
        }

        contentPane.Toggle_skipAnim.isOn = flag == 1;

        //InitRecruitHeroData();

        //contentPane.recruitObj.GetComponent<Canvas>().sortingOrder = _uiCanvas.sortingOrder + 2;
    }

    protected override void onHide()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }

        animTimerIds.Clear();

    }
}
