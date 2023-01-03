using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Core;

public enum TopPlayerShowType
{
    none = 0,
    all = 1,
    noSetting = 2,  //没有设置 角色按钮不能点击
    noSettingAndEnergy = 3, //没有设置和能量 角色按钮不能点击
    rotaryTable = 4,    //有转盘数量没有能量和设置 角色按钮不能点击
    noEnergy = 5,   //没有能量 角色按钮能点击
    noRoleAndSettingAndEnergy = 6, //没有角色按钮和设置按钮和能量
    selfUnionToken = 7,//没有能量 公会积分 有个人公会币 没有设置 角色按钮不能点击
    //转盘数量（做到该功能添加）
    lottery = 8,//没有角色按钮 没有能量 没有新币 没有设置
    roleExchange = 9,//没有能量 没有金条 没有设置
    activity_workerGame = 10,//没有能量 公会积分 个人公会币 没有设置 角色按钮不能点击 有巧匠大赛活动币
    luxury = 11,//豪华度
    noSettingAndCantClickGem = 12,//没设置不能点击商城
    ignore = 99,//忽视 继承上一个界面的设置
    //
}
public class TopPlayerInfoView : ViewBase<TopPlayerinfoPanel>
{
    public override string viewID => ViewPrefabName.TopPlayerInfoPanel;

    private bool IsInitGAndG;

    public override int showType => (int)ViewShowType.normal;

    Dictionary<int, TopPlayerAniItem> workerAniItems;
    Stack<TopPlayerAniItem> animItemPool;

    List<string> cantClickViewName = new List<string>() { ViewPrefabName.ShopkeeperSubPanel, "GiftDetailUI" };

    protected override void onInit()
    {
        base.onInit();
        EventController.inst.AddListener<bool>(GameEventType.UI_TOPTESPANEL_ShiftOut, (value) =>
        {
            if (contentObject == null) return;
            if (value)
            {
                shiftOut();
            }
            else
            {
                shiftIn();
            }
        });
        contentPane.shopkeeperBtn.onClick.AddListener(onShopkeeperBtnClick);
        contentPane.gameSettingBtn.onClick.AddListener(() =>
        {
            if (GuideDataProxy.inst.CurInfo.isAllOver)
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SUBTOP);
        });

        contentPane.energyTipBtn.onClick.AddListener(onEnergyTipBtnClick);
        contentPane.goldTipBtn.onClick.AddListener(onGoldTipBtnClick);
        contentPane.tipMaskBtn.onClick.AddListener(onTipMaskBtnClick);
        contentPane.selfUnionTokenBtn.onClick.AddListener(onSelfUnionTipBtnClick);
        contentPane.unionCoinBtn.onClick.AddListener(onUnionTipBtnClick);
        contentPane.lvbar.value = 0;
        contentPane.addGemBtn.onClick.AddListener(() =>
        {
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver) return;

            if (!string.IsNullOrEmpty(GUIManager.GetCurrWindowViewID()))
            {
                if (cantClickViewName.Contains(GUIManager.GetCurrWindowViewID())) return;
            }
            
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_MallUI", 3);
        });

        contentPane.activity_workerGame_coinBtn.onClick.AddListener(() =>
        {
            if (contentPane.img_activity_workerGame_coinAdd.gameObject.activeSelf)
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_WorkerGameCoinGiftsUI");
            }
        });


        //动画
        workerAniItems = new Dictionary<int, TopPlayerAniItem>();
        animItemPool = new Stack<TopPlayerAniItem>();

        EventController.inst.AddListener<List<WorkerExpData>>(GameEventType.WorkerCompEvent.Worker_ExpChange, WorkerExpChange);
        EventController.inst.AddListener(GameEventType.BagEvent.Bag_inventory_numChg, InventoryNumChange);
        EventController.inst.AddListener<TaskData>(GameEventType.TaskEvent.TASK_CONTENTCHANGE, TaskContentChange);
        EventController.inst.AddListener<SevenDayGoalSingle>(GameEventType.SevenDayGoalEvent.SEVENDAY_CONTENTCHANGE, SevenDayContentChange);
        EventController.inst.AddListener(GameEventType.TaskEvent.SHOWTIP_UNIONTASK, ShowUnionTaskTip);
        EventController.inst.AddListener(GameEventType.TaskEvent.SHOWTIP_DAILYTASK, ShowDailyTaskTip);

        EventController.inst.AddListener<bool>(GameEventType.SHOW_FurnitureNumLimit, showMapCountLimit);

        LanguageManager.inst.ChangeLangeuageEvent -= UpdateShow;
        LanguageManager.inst.ChangeLangeuageEvent += UpdateShow;

        HotfixBridge.inst.TriggerLuaEvent("Check_UnionTaskTip");
        HotfixBridge.inst.TriggerLuaEvent("Check_DailyTaskTip");

    }

    int moneyBoxShowTimeid = 0;
    public void updateMoneyBoxData()
    {
        var data = MoneyBoxDataProxy.inst.moneyBoxData;
        if (data.currState == 0)
        {
            return;
        }
        if (moneyBoxShowTimeid == 0)
        {
            if (data.currState == 2)
                contentPane.moneyBoxTip.gameObject.SetActive(true);
            else
                return;
        }
        else
        {
            GameTimer.inst.RemoveTimer(moneyBoxShowTimeid);
            contentPane.moneyBoxTip.setinfo();
        }
        moneyBoxShowTimeid = GameTimer.inst.AddTimer(2, 1, () =>
            {
                if (contentPane != null)
                {
                    contentPane.moneyBoxTip.gameObject.SetActive(false);
                    moneyBoxShowTimeid = 0;
                }
            });
    }

    protected override void onShown()
    {
        base.onShown();

        UpdateRedDots();
        EventController.inst.AddListener(GameEventType.UpdateGameRedPoints, UpdateRedDots);

        updateResUI();
        if (UIUnLockConfigMrg.inst.GetBtnInteractable("Top"))
        {
            notShow(true);
        }
        else
        {
            notShow(false);
        }
        if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.isDesigning)
            showMapCountLimit(true);
        else
            showMapCountLimit(false);
    }

    protected override void onHide()
    {
        base.onHide();
        IsInitGAndG = false;
        removeAllListeners();
    }

    protected override void beforeDispose()
    {
        base.beforeDispose();

        removeAllListeners();
    }

    void removeAllListeners()
    {
        EventController.inst.RemoveListener(GameEventType.UpdateGameRedPoints, UpdateRedDots);
        EventController.inst.RemoveListener<List<WorkerExpData>>(GameEventType.WorkerCompEvent.Worker_ExpChange, WorkerExpChange);
        EventController.inst.RemoveListener(GameEventType.BagEvent.Bag_inventory_numChg, InventoryNumChange);
        EventController.inst.RemoveListener<TaskData>(GameEventType.TaskEvent.TASK_CONTENTCHANGE, TaskContentChange);
        EventController.inst.RemoveListener<SevenDayGoalSingle>(GameEventType.SevenDayGoalEvent.SEVENDAY_CONTENTCHANGE, SevenDayContentChange);
        EventController.inst.RemoveListener(GameEventType.TaskEvent.SHOWTIP_UNIONTASK, ShowUnionTaskTip);
        EventController.inst.RemoveListener(GameEventType.TaskEvent.SHOWTIP_DAILYTASK, ShowDailyTaskTip);

        EventController.inst.RemoveListener<bool>(GameEventType.SHOW_FurnitureNumLimit, showMapCountLimit);
    }

    void UpdateRedDots()
    {
        contentPane.settingRedPoint.SetActive(AcheivementDataProxy.inst.NeedRedPoint || EmailDataProxy.inst.needShowRedPoint /*|| MoneyBoxDataProxy.inst.NeedShowRedPoint*/ || (AccountDataProxy.inst.currbindingType != EBindingType.None && AccountDataProxy.inst.bindingClaimState));
    }

    public void ChangeResItemCount(int resId)
    {

    }
    public void showMapCountLimit(bool show)
    {
        if (contentPane != null && contentPane.mapCountLimit != null && contentPane.gameObject != null)
        {
            contentPane.mapCountLimit.gameObject.SetActive(show);
            if (show)
            {
                bool full = UserDataProxy.inst.shopData.indoorMapFurniture() >= UserDataProxy.inst.shopData.furnitureLimit();
                contentPane.mapCountLimit.maxValue = UserDataProxy.inst.shopData.furnitureLimit();
                contentPane.mapCountLimit.value = UserDataProxy.inst.shopData.indoorMapFurniture();
                contentPane.mapCountLimitText.text = $"{UserDataProxy.inst.shopData.indoorMapFurniture()}/{UserDataProxy.inst.shopData.furnitureLimit()}";
                contentPane.mapCountLimitText.color = full ? Color.red : Color.white;
            }
        }
    }
    int shifout = 0;
    public override void shiftOut()
    {
        if (!UIUnLockConfigMrg.inst.GetBtnInteractable("Top"))
        {
            notShow(false);
            return;
        }
        // if (shifout == 1) return;
        shifout = 1;
        if (isShowing)
        {
            //Logger.error("topView ShifoutAnim play...");
            DOTween.Kill(contentPane.animTf, true);
            if (contentPane.animTf.anchoredPosition.y != 306)
                contentPane.animTf.DOAnchorPos3DY(306, 0.5f);
        }
    }
    public override void shiftIn()
    {
        if (contentPane == null || contentPane.gameObject == null) return;
        var curViewName = HotfixBridge.inst.GetCurrWindowViewID();
        if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.isDesigning && !curViewName.Contains("Pet"))
            showMapCountLimit(true);
        else
            showMapCountLimit(false);

        //  if (shifout == 0) return;
        if (UIUnLockConfigMrg.inst.GetBtnInteractable("Top"))
        {
            notShow(true);
        }
        else
        {
            notShow(false);
            return;
        }
        shifout = 0;
        if (isShowing)
        {
            contentPane.settingRedPoint.SetActive(AcheivementDataProxy.inst.NeedRedPoint || EmailDataProxy.inst.needShowRedPoint /*|| MoneyBoxDataProxy.inst.NeedShowRedPoint*/ || (AccountDataProxy.inst.currbindingType != EBindingType.None && AccountDataProxy.inst.bindingClaimState));
            shiftInAnim();
            //updateResUI();
        }
        if (!UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.gameSettingBtn.gameObject.name))
        {
            contentPane.gameSettingBtn.gameObject.SetActive(false);
        }

        if (!UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.shopkeeperBtn.gameObject.name))
        {
            contentPane.shopkeeperBtn.gameObject.SetActive(false);
        }
        contentPane.goldTipBtn.gameObject.SetActive(UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.goldTipBtn.gameObject.name));
        contentPane.addGemBtn.gameObject.SetActive(UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.addGemBtn.gameObject.name));

        if (!string.IsNullOrEmpty(GUIManager.GetCurrWindowViewID()))
        {
            TopPlayerShowType showtype = GUIManager.curLuaTopPlayerShowType;
            if (showtype != TopPlayerShowType.none)
                SetBtnShow(showtype);
        }

        //if (GUIManager.CurrWindow != null)
        //{
        //    TopPlayerShowType showtype = GUIManager.CurrWindow.topResPanelType;
        //    if (showtype != TopPlayerShowType.none)
        //        SetBtnShow(showtype);
        //}
    }

    public void SetBtnShow(TopPlayerShowType showtype)
    {
        //按钮显示
        if (UIUnLockConfigMrg.inst != null && !string.IsNullOrEmpty(GUIManager.GetCurrWindowViewID()))
        {
            string currname = GUIManager.GetCurrWindowViewID();

            if (UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.gameSettingBtn.gameObject.name))
            {
                if (currname == ViewPrefabName.MainUI || currname == ViewPrefabName.CityUI)
                {
                    contentPane.gameSettingBtn.gameObject.SetActive(true);
                }
                else
                {
                    if (showtype == TopPlayerShowType.all || showtype == TopPlayerShowType.noEnergy)
                        contentPane.gameSettingBtn.gameObject.SetActive(true);
                    else
                        contentPane.gameSettingBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                contentPane.gameSettingBtn.gameObject.SetActive(false);
            }

            if (UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.shopkeeperBtn.gameObject.name))
            {
                if (showtype == TopPlayerShowType.noRoleAndSettingAndEnergy || showtype == TopPlayerShowType.lottery)
                {
                    contentPane.shopkeeperBtn.gameObject.SetActive(false);
                }
                else
                {
                    contentPane.shopkeeperBtn.gameObject.SetActive(true);
                    if (currname == ViewPrefabName.MainUI || currname == ViewPrefabName.CityUI)
                    {
                        contentPane.shopkeeperBtn.interactable = true;
                    }
                    else
                    {
                        if (showtype == TopPlayerShowType.all || showtype == TopPlayerShowType.noEnergy)
                        {
                            contentPane.shopkeeperBtn.interactable = true;
                        }
                        else
                        {
                            contentPane.shopkeeperBtn.interactable = false;
                        }
                    }
                }
            }
            else
            {
                contentPane.shopkeeperBtn.gameObject.SetActive(false);
            }

            if (UIUnLockConfigMrg.inst.GetBtnInteractable(contentPane.energyTipBtn.gameObject.name))
            {
                if (showtype == TopPlayerShowType.noSettingAndEnergy || showtype == TopPlayerShowType.noEnergy || showtype == TopPlayerShowType.rotaryTable || showtype == TopPlayerShowType.noRoleAndSettingAndEnergy || showtype == TopPlayerShowType.selfUnionToken || showtype == TopPlayerShowType.lottery || showtype == TopPlayerShowType.roleExchange || showtype == TopPlayerShowType.activity_workerGame || showtype == TopPlayerShowType.luxury || showtype == TopPlayerShowType.noSettingAndCantClickGem)
                {
                    contentPane.energyTipBtn.gameObject.SetActive(false);
                }
                else
                {
                    contentPane.energyTipBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                contentPane.energyTipBtn.gameObject.SetActive(false);
            }

            contentPane.goldTipBtn.gameObject.SetActive(showtype != TopPlayerShowType.lottery);
            contentPane.lotteryObj.SetActive(showtype == TopPlayerShowType.lottery);
            contentPane.luxuryObj.SetActive(showtype == TopPlayerShowType.luxury);
            contentPane.addGemBtn.gameObject.SetActive(showtype != TopPlayerShowType.roleExchange);

            //自己公会币
            if (showtype == TopPlayerShowType.selfUnionToken)
            {
                //UpdateUnionTokens();
                contentPane.selfUnionTokenBtn.gameObject.SetActive(true);
            }
            else
            {
                contentPane.selfUnionTokenBtn.gameObject.SetActive(false);
            }

            //公会积分
            if (currname == "UnionWealUI" || currname == "UnionWealUpDetailUI" || currname == ViewPrefabName.UnionTaskResultUI)
            {
                //UpdateUnionTokens();
                contentPane.unionCoinBtn.gameObject.SetActive(true);
            }
            else
            {
                contentPane.unionCoinBtn.gameObject.SetActive(false);
            }


            if (showtype == TopPlayerShowType.activity_workerGame)
            {
                contentPane.activity_workerGame_coinBtn.gameObject.SetActive(true);

                contentPane.img_activity_workerGame_coinAdd.gameObject.SetActive(currname != "WorkerGameCoinGiftsUI");
            }
            else
            {
                contentPane.activity_workerGame_coinBtn.gameObject.SetActive(false);
            }
        }
    }
    public void notShow(bool show)
    {
        if (contentObject != null)
        {
            contentObject.SetActive(show);
        }
    }
    private void shiftInAnim()
    {
        //Logger.error("topView ShiftInAnim play...");
        DOTween.Kill(contentPane.animTf, true);
        if (contentPane.animTf.anchoredPosition.y == 30) return;
        contentPane.animTf.anchoredPosition = new Vector2(contentPane.animTf.anchoredPosition.x, 30);

        if (!GameSettingManager.inst.needShowUIAnim) return;

        (contentPane.gameSettingBtn.transform as RectTransform).DOAnchorPos3DY(-78f, 0.3f).From(200f).SetEase(Ease.OutQuint);
        (contentPane.shopkeeperBtn.transform as RectTransform).DOAnchorPos3DX(-70f, 0.3f).From(-500f).SetEase(Ease.OutBack);
        (contentPane.energyTipBtn.transform as RectTransform).DOAnchorPos3DY(-27f, 0.4f).From(500f).SetEase(Ease.OutBack);
        (contentPane.goldTipBtn.transform as RectTransform).DOAnchorPos3DY(-27f, 0.5f).From(500f).SetEase(Ease.OutBack);
        (contentPane.addGemBtn.transform as RectTransform).DOAnchorPos3DY(-27f, 0.6f).From(500f).SetEase(Ease.OutBack);
        (contentPane.lotteryObj.transform as RectTransform).DOAnchorPos3DY(-27f, 0.6f).From(500f).SetEase(Ease.OutBack);
        (contentPane.luxuryObj.transform as RectTransform).DOAnchorPos3DY(-27f, 0.6f).From(500f).SetEase(Ease.OutBack);
    }

    public void UpdateShow()
    {
        updateGAndG();
    }
    public void updateGAndG()
    {
        contentPane.glodText.text = AbbreviationUtility.AbbreviateNumber(UserDataProxy.inst.playerData.gold, 2);
        contentPane.gemText.text = AbbreviationUtility.AbbreviateNumber(UserDataProxy.inst.playerData.gem, 2);

        contentPane.lotteryItemText.text = ItemBagProxy.inst.GetItem(140001).count.ToString("N0");
        contentPane.luxuryText.text = UserDataProxy.inst.playerData.prosperity.ToString();
    }

    public void UpdateUnionTokens()
    {
        contentPane.selfUnionTokenTx.text = UserDataProxy.inst.playerData.unionCoin.ToString("N0");
        contentPane.unionCoinTx.text = UserDataProxy.inst.Union_uCoin.ToString("N0");
    }

    public void UpdateActivity_workerGameCoin()
    {
        contentPane.activity_workerGame_coinTx.text = HotfixBridge.inst.GetActivity_WorkerGameCoinCount().ToString("N0");
    }

    public void updateResUI()
    {
        if (UserDataProxy.inst.playerData == null) return;
        if (!IsInitGAndG)
        {
            updateGAndG();
            UpdateUnionTokens();
            float energySliderValue = (float)UserDataProxy.inst.playerData.energy / UserDataProxy.inst.playerData.energyLimit;
            contentPane.energySlider.DOValue(energySliderValue, 0.35f);
            contentPane.energyText.text = $"{UserDataProxy.inst.playerData.energy}/{UserDataProxy.inst.playerData.energyLimit}";
            UpdateActivity_workerGameCoin();

            setCurVal(1, UserDataProxy.inst.playerData.gold);
            setCurVal(2, UserDataProxy.inst.playerData.gem);
            setCurVal(3, UserDataProxy.inst.playerData.energy);
            setCurVal(4, UserDataProxy.inst.playerData.unionCoin);
            setCurVal(5, UserDataProxy.inst.Union_uCoin);
            setCurVal(6, HotfixBridge.inst.GetActivity_WorkerGameCoinCount());

            IsInitGAndG = true;
        }
        contentPane.playerLv.text = UserDataProxy.inst.playerData.level.ToString();
        contentPane.huangguanImg.enabled = (K_Vip_State)UserDataProxy.inst.playerData.vipState == K_Vip_State.Vip;
        contentPane.vipLvBg.enabled = (K_Vip_State)UserDataProxy.inst.playerData.vipState == K_Vip_State.Vip;

        contentPane.lvbar.maxValue = 1;
        contentPane.lvbar.value = (float)Mathf.Max(UserDataProxy.inst.playerData.MaxExp * 0.05f, UserDataProxy.inst.playerData.CurrExp) / (float)UserDataProxy.inst.playerData.MaxExp;

        //contentPane.PlayerName.rectTransform.sizeDelta = sizeDelta;
        // long changeExp = contentPane.lvbar.value == 0 ? 0 : UserDataProxy.inst.playerData.CurrExp - (long)contentPane.lvbar.value;
        // contentPane.lvbar.maxValue = UserDataProxy.inst.playerData.MaxExp;
        // contentPane.lvbar.value = Mathf.Max(UserDataProxy.inst.playerData.MaxExp * 0.05f, UserDataProxy.inst.playerData.CurrExp);
        // if (changeExp > 0)
        // {
        //     DOTween.Kill(contentPane.addExpText.transform);
        //     contentPane.addExpText.transform.localScale = Vector3.zero;
        //     contentPane.addExpText.text = $"+{changeExp}EXP";
        //     contentPane.addExpText.GetComponent<RectTransform>().DOAnchorPos3DY(-12, 0.5f).From(-200).SetEase(Ease.OutQuad).OnStart(() =>
        //     {
        //         contentPane.addExpText.transform.DOScale(1, 0.3f).SetEase(Ease.OutQuad).onComplete = () =>
        //         {
        //             contentPane.addExpText.transform.DOScale(0, 0.1f).SetEase(Ease.InQuad).SetDelay(2f).onComplete = () =>
        //             {
        //                 contentPane.addExpText.text = "";
        //             };
        //         };
        //     });
        // }
    }

    public void showExpChange(long changeExp)
    {
        if (changeExp > 0)
        {
            DOTween.Kill(contentPane.addExpText.transform);
            contentPane.addExpText.transform.localScale = Vector3.zero;
            contentPane.addExpText.text = $"+{changeExp}EXP";
            contentPane.addExpText.GetComponent<RectTransform>().DOAnchorPos3DY(-12, 0.5f).From(-200).SetEase(Ease.OutQuad).OnStart(() =>
            {
                contentPane.addExpText.transform.DOScale(1, 0.3f).SetEase(Ease.OutQuad).onComplete = () =>
                {
                    contentPane.addExpText.transform.DOScale(0, 0.1f).SetEase(Ease.InQuad).SetDelay(2f).onComplete = () =>
                    {
                        contentPane.addExpText.text = "";
                    };
                };
            });
        }
    }
    public void updateEnergyLimitNum()
    {
        if (!isShowing) return;
        int energyNum = int.Parse(contentPane.energyText.text.Split('/')[0]);
        contentPane.energyText.text = energyNum + "/" + UserDataProxy.inst.playerData.energyLimit;
        float energySliderValue = (float)energyNum / UserDataProxy.inst.playerData.energyLimit;
        contentPane.energySlider.DOValue(energySliderValue, 0.2f);
    }




    //钱tween              1
    long goldCurVal;
    TweenerCore<long, long, NoOptions> goldTween;
    //钻tween              2
    long gemCurVal;
    TweenerCore<long, long, NoOptions> gemTween;
    //能量tween            3
    long energyCurVal;
    TweenerCore<long, long, NoOptions> enerygyTween;
    TweenerCore<float, float, FloatOptions> energySliderTween;
    //个人联盟币tween      4
    long selfUnionCoinCurVal;
    TweenerCore<long, long, NoOptions> selfUnionCoinTween;
    //联盟积分tween        5
    long unionCoinCurVal;
    TweenerCore<long, long, NoOptions> unoinCoinTween;
    //巧匠大赛活动币
    long activity_workerGame_coinCurVal;
    TweenerCore<long, long, NoOptions> activityWorkerGameCoinTween;


    void clearTween(int itemType)
    {
        switch (itemType)
        {
            case 1:
                if (goldTween != null)
                {
                    goldTween.Kill();
                    goldTween = null;
                }
                break;
            case 2:
                if (gemTween != null)
                {
                    gemTween.Kill();
                    gemTween = null;
                }
                break;
            case 3:
                if (enerygyTween != null)
                {
                    enerygyTween.Kill();
                    enerygyTween = null;
                }
                break;
            case 4:
                if (selfUnionCoinTween != null)
                {
                    selfUnionCoinTween.Kill();
                    selfUnionCoinTween = null;
                }
                break;
            case 5:
                if (unoinCoinTween != null)
                {
                    unoinCoinTween.Kill();
                    unoinCoinTween = null;
                }
                break;
            case 6:
                if (activityWorkerGameCoinTween != null)
                {
                    activityWorkerGameCoinTween.Kill();
                    activityWorkerGameCoinTween = null;
                }
                break;
        }
    }

    void resetTween(int itemType, TweenerCore<long, long, NoOptions> tween)
    {
        switch (itemType)
        {
            case 1: goldTween = tween; AudioManager.inst.PlaySound(151); break;
            case 2: gemTween = tween; break;
            case 3: enerygyTween = tween; break;
            case 4: selfUnionCoinTween = tween; break;
            case 5: unoinCoinTween = tween; break;
            case 6: activityWorkerGameCoinTween = tween; break;
        }

    }

    void setCurVal(int itemType, long curVal)
    {
        switch (itemType)
        {
            case 1: goldCurVal = curVal; break;
            case 2: gemCurVal = curVal; break;
            case 3: energyCurVal = curVal; break;
            case 4: selfUnionCoinCurVal = curVal; break;
            case 5: unionCoinCurVal = curVal; break;
            case 6: activity_workerGame_coinCurVal = curVal; break;
        }
    }

    long getCurVal(int itemType)
    {

        long val = 0;
        switch (itemType)
        {
            case 1: val = goldCurVal; break;
            case 2: val = gemCurVal; break;
            case 3: val = energyCurVal; break;
            case 4: val = selfUnionCoinCurVal; break;
            case 5: val = unionCoinCurVal; break;
            case 6: val = activity_workerGame_coinCurVal; break;
        }
        return val;
    }

    void checkEnergyAnimNormal(long newVal)
    {
        long curVal = getCurVal(3);

        if (curVal - newVal > 5)
        {
            curVal = newVal > 2 ? newVal - 2 : 0;

            if (energySliderTween != null)
            {
                energySliderTween.Kill();
                energySliderTween = null;
            }
            contentPane.energySlider.value = (float)curVal / UserDataProxy.inst.playerData.energyLimit;

            clearTween(3);
            contentPane.energyText.text = curVal + "/" + UserDataProxy.inst.playerData.energyLimit;
        }

    }

    public void updateItemNum(long oldVal, long newVal, int itemType)
    {
        long curVal = getCurVal(itemType);
        float rate = curVal < newVal ? 0.8f : 0.4f;

        Text tempItemText = null;
        switch (itemType)
        {
            // gold
            case 1:
                tempItemText = contentPane.glodText;
                break;
            // gem
            case 2:
                tempItemText = contentPane.gemText;
                break;
            // energy
            case 3:
                tempItemText = contentPane.energyText;
                float energySliderValue = (float)newVal / UserDataProxy.inst.playerData.energyLimit;
                if (energySliderTween != null)
                {
                    energySliderTween.Kill();
                    energySliderTween = null;
                }
                energySliderTween = contentPane.energySlider.DOValue(energySliderValue, rate);
                //contentPane.energySlider.value = (float)UserDataProxy.inst.playerData.energy / UserDataProxy.inst.playerData.energyLimit;
                break;
            // self-UnionCoin 个人联盟币
            case 4:
                tempItemText = contentPane.selfUnionTokenTx;
                break;
            // union-UnionCoin 联盟积分
            case 5:
                tempItemText = contentPane.unionCoinTx;
                break;
            //activity_workerGame_coin 巧匠大赛活动币
            case 6:
                tempItemText = contentPane.activity_workerGame_coinTx;
                break;
            default:
                break;
        }
        //int timeId = GameTimer.inst.AddTimer(rate, 1, () =>
        // {

        clearTween(itemType);

        if (curVal != newVal)
        {
            var tween = DOTween.To(() => curVal, x => curVal = x, newVal, rate);

            resetTween(itemType, tween);

            tween.OnUpdate(() =>
            {
                if (itemType == 3)
                    tempItemText.text = curVal + "/" + UserDataProxy.inst.playerData.energyLimit;
                else
                    tempItemText.text = AbbreviationUtility.AbbreviateNumber(curVal, 2);

                setCurVal(itemType, curVal);
            });
        }
        else
        {
            if (itemType == 3)
                tempItemText.text = newVal + "/" + UserDataProxy.inst.playerData.energyLimit;
            else
                tempItemText.text = AbbreviationUtility.AbbreviateNumber(newVal, 2);

            setCurVal(itemType, newVal);
        }
        //});
    }

    private void onShopkeeperBtnClick()
    {
        //AudioManager.inst.PlaySound(10);
        if (GuideDataProxy.inst.CurInfo.isAllOver)
        {
            //EventController.inst.TriggerEvent(GameEventType.HIDEUI_SHOPSCENE);
            EventController.inst.TriggerEvent(GameEventType.HIDEUI_CHANGENAME);
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_SELFROLEINFO);
        }
    }

    private void onEnergyTipBtnClick()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver) return;
        contentPane.energyTipObj.SetActive(true);
        contentPane.curOpenTipObj = contentPane.energyTipObj;
        contentPane.tipMaskBtn.gameObject.SetActive(true);
    }

    private void onSelfUnionTipBtnClick()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver) return;
        contentPane.selfUnionCoinTipObj.SetActive(true);
        contentPane.curOpenTipObj = contentPane.selfUnionCoinTipObj;
        contentPane.tipMaskBtn.gameObject.SetActive(true);
    }

    void onUnionTipBtnClick()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver) return;
        contentPane.unionCoinTipObj.SetActive(true);
        contentPane.curOpenTipObj = contentPane.unionCoinTipObj;
        contentPane.tipMaskBtn.gameObject.SetActive(true);
    }

    private void onGoldTipBtnClick()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver) return;
        contentPane.goldTipObj.SetActive(true);
        contentPane.curOpenTipObj = contentPane.goldTipObj;
        contentPane.tipMaskBtn.gameObject.SetActive(true);

        contentPane.goldTipTx.text = LanguageManager.inst.GetValueByKey("新币是店长手头的主要货币，能通过向顾客售卖物品获得。") + (UserDataProxy.inst.playerData.gold > 10000 ? "\n" + LanguageManager.inst.GetValueByKey("<size=26>您的新币：{0}</size>", UserDataProxy.inst.playerData.gold.ToString("N0")) : "");
    }

    private void onTipMaskBtnClick()
    {
        contentPane.tipMaskBtn.gameObject.SetActive(false);
        contentPane.curOpenTipObj.SetActive(false);
    }

    #region 物品栏数量变化
    public void InventoryNumChange()
    {
        int num = ItemBagProxy.inst.GetEquipInventory();
        long limit = ItemBagProxy.inst.bagCountLimit;
        contentPane.inventoryAni.SetData(ItemBagProxy.inst.GetEquipInventory() / (float)ItemBagProxy.inst.bagCountLimit, "", "", num == limit ? "<color=#ff2828>" + LanguageManager.inst.GetValueByKey("已满") + "</color>" : num + "/" + limit);
    }

    #endregion

    #region 工匠经验变化
    public void WorkerExpChange(List<WorkerExpData> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            var data = datas[i];

            WorkerData workerData = RoleDataProxy.inst.GetWorker(data.id);

            if (workerAniItems.ContainsKey(data.id))
            {
                workerAniItems[data.id].SetData(data.changeExp == 0 ? 1 : (float)workerData.exp / workerData.maxExp, StaticConstants.roleHeadIconAtlasName, workerData.config.icon, data.changeExp == 0 ? "<color=#ff2828>" + LanguageManager.inst.GetValueByKey("已满") + "</color>" : "+" + data.changeExp + LanguageManager.inst.GetValueByKey("经验"), true);
            }
            else
            {
                TopPlayerAniItem workerAniItem = null;

                if (animItemPool.Count > 0)
                {
                    workerAniItem = animItemPool.Pop();
                }
                else
                {
                    var gameObj = GameObject.Instantiate<GameObject>(contentPane.expAniItemPfb.gameObject, contentPane.expAniTf);
                    workerAniItem = gameObj.GetComponent<TopPlayerAniItem>();
                }

                (workerAniItem.transform as RectTransform).anchoredPosition = new Vector2(140, -60 - 90 * workerAniItems.Count);
                workerAniItems.Add(data.id, workerAniItem);
                workerAniItems[data.id].SetData(data.changeExp == 0 ? 1 : (float)workerData.exp / workerData.maxExp, StaticConstants.roleHeadIconAtlasName, workerData.config.icon, data.changeExp == 0 ? "<color=#ff2828>" + LanguageManager.inst.GetValueByKey("已满") + "</color>" : "+" + data.changeExp + LanguageManager.inst.GetValueByKey("经验"), false);
                workerAniItem.delAniCallBack = () =>
                {
                    animItemPool.Push(workerAniItem);
                    workerAniItems.Remove(data.id);

                    List<TopPlayerAniItem> list = workerAniItems.Values.ToList();

                    for (int k = 0; k < list.Count; k++)
                    {
                        var item = list[k];
                        Vector2 targetPos = new Vector2(140, -60 - 90 * k);
                        (item.transform as RectTransform).DOAnchorPos3D(targetPos, 0.15f);
                    }
                };
            }
        }
    }

    #endregion

    #region 任务进度变化
    public void TaskContentChange(TaskData dailyTask)
    {
        return; //任务进度变化不显示AniItem 2021.10.14 陈晨提
        var contentStr = UserDataProxy.inst.GetTaskName(dailyTask, true);
        contentPane.taskAni.SetTaskData(dailyTask.parameter_1 / (float)dailyTask.parameter_2, dailyTask.atlas, dailyTask.icon, contentStr);
    }
    #endregion

    #region 七日进度变化
    public void SevenDayContentChange(SevenDayGoalSingle data)
    {
        contentPane.sevenAnim.SetSevenDayTaskData(data);
    }
    #endregion

    #region 悬赏任务tip
    public void ShowUnionTaskTip()
    {
        contentPane.unionTaskAnim.PlayAnimShow();
    }
    #endregion

    #region 日常任务tip
    public void ShowDailyTaskTip()
    {
        contentPane.dailyTaskAnim.PlayAnimShow();
    }
    #endregion

    public void AddFlyEnergy(long changeNum, long oldNum, long newNum)
    {
        if (this == null || contentPane == null) return;

        var count = Mathf.Min(changeNum, 35);
        for (int i = 0; i < count; i++)
        {
            var newGO = GameObject.Instantiate(contentPane.flyEnergyTF.gameObject, FGUI.inst.vfxPlanel);
            newGO.SetActive(true);
            var fly = newGO.GetComponent<fly>();
            fly.tt = FGUI.inst.energyTargetTf;

            Action flyAnimEndHandler = energyIconBreatheAnim;

            if (i == 0)
            {
                long _newNum = newNum;
                long _oldNum = oldNum;
                flyAnimEndHandler += () =>
                {
                    checkEnergyAnimNormal(_newNum);
                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.ENERGYNUM_ADD, _oldNum, _newNum);
                };
            }

            fly.SetCallback(flyAnimEndHandler);
        }


        EffectManager.inst.Spawn(3002, Vector3.zero, (gamevfx) =>
        {
            gamevfx.transform.SetParent(FGUI.inst.vfxPlanel.Find("center"), true);
            gamevfx.gameObject.SetActive(true);
            gamevfx.transform.localPosition = new Vector3(0, -(FGUI.inst.uiRootTF.sizeDelta.y / 8f), 0);
            GUIHelper.setRandererSortinglayer(gamevfx.transform, "top", 100);
        });
    }


    public void AddFlyGold(long changeNum, long oldNum, long newNum)
    {
        if (this == null || contentPane == null) return;

        var count = Mathf.Max(1, Mathf.Min(changeNum / 10, 35));
        for (int i = 0; i < count; i++)
        {
            var newGO = GameObject.Instantiate(contentPane.flyGoldTF.gameObject, FGUI.inst.vfxPlanel);
            newGO.SetActive(true);
            var fly = newGO.GetComponent<fly>();
            fly.tt = FGUI.inst.goldFlyTargetTf;

            Action flyAnimEndHandler = goldIconBreatheAnim;

            if (i == 0)
            {
                long _newNum = newNum;
                long _oldNum = oldNum;
                flyAnimEndHandler += () =>
                {
                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.GOLDNUM_ADD, _oldNum, _newNum);
                };
            }

            fly.SetCallback(flyAnimEndHandler);
        }


        EffectManager.inst.Spawn(3002, Vector3.zero, (gamevfx) =>
        {
            gamevfx.transform.SetParent(FGUI.inst.vfxPlanel.Find("center"), true);
            gamevfx.gameObject.SetActive(true);
            gamevfx.transform.localPosition = new Vector3(0, -(FGUI.inst.uiRootTF.sizeDelta.y / 8f), 0);
            GUIHelper.setRandererSortinglayer(gamevfx.transform, "top", 100);
        });
    }

    public void AddFlyUnionCoin(long changeNum, long oldNum, long newNum)
    {
        if (this == null || contentPane == null) return;

        var count = Mathf.Min(changeNum, 35);
        for (int i = 0; i < count; i++)
        {
            var newGO = GameObject.Instantiate(contentPane.flyUnionCoinTF.gameObject, FGUI.inst.vfxPlanel);
            newGO.SetActive(true);
            var fly = newGO.GetComponent<fly>();
            fly.tt = FGUI.inst.energyTargetTf;

            Action flyAnimEndHandler = unionCoinBreathAnim;

            if (i == 0)
            {
                flyAnimEndHandler += () =>
                {
                    long _newNum = newNum;
                    long _oldNum = oldNum;
                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.SELF_UNIONCOIN, _oldNum, _newNum);
                };
            }

            fly.SetCallback(flyAnimEndHandler);
        }


        EffectManager.inst.Spawn(3002, Vector3.zero, (gamevfx) =>
        {
            gamevfx.transform.SetParent(FGUI.inst.vfxPlanel.Find("center"), true);
            gamevfx.gameObject.SetActive(true);
            gamevfx.transform.localPosition = new Vector3(0, -(FGUI.inst.uiRootTF.sizeDelta.y / 8f), 0);
            GUIHelper.setRandererSortinglayer(gamevfx.transform, "top", 100);
        });
    }

    public void AddFlyActivityWorkerGameCoin(long changeNum, long oldNum, long newNum)
    {
        if (this == null || contentPane == null) return;

        var count = Mathf.Min(changeNum, 35);
        for (int i = 0; i < count; i++)
        {
            var newGO = GameObject.Instantiate(contentPane.flyActivity_workerGameCoinTf.gameObject, FGUI.inst.vfxPlanel);
            newGO.SetActive(true);
            var fly = newGO.GetComponent<fly>();
            fly.tt = FGUI.inst.energyTargetTf;

            Action flyAnimEndHandler = activityWorkerGameBreathAnim;

            if (i == 0)
            {
                flyAnimEndHandler += () =>
                {
                    long _newNum = newNum;
                    long _oldNum = oldNum;
                    EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.ACTIVITY_WORKERGAME_COIN, _oldNum, _newNum);
                };
            }

            fly.SetCallback(flyAnimEndHandler);
        }


        EffectManager.inst.Spawn(3002, Vector3.zero, (gamevfx) =>
        {
            gamevfx.transform.SetParent(FGUI.inst.vfxPlanel.Find("center"), true);
            gamevfx.gameObject.SetActive(true);
            gamevfx.transform.localPosition = new Vector3(0, -(FGUI.inst.uiRootTF.sizeDelta.y / 8f), 0);
            GUIHelper.setRandererSortinglayer(gamevfx.transform, "top", 100);
        });
    }

    void energyIconBreatheAnim()
    {
        contentPane.energyTargetTf.DOScale(1.2f, 0.12f).SetLoops(2, LoopType.Yoyo).From(1);
    }


    void goldIconBreatheAnim()
    {
        contentPane.goldTargetTf.DOScale(1.2f, 0.12f).SetLoops(2, LoopType.Yoyo).From(1);
    }

    void unionCoinBreathAnim()
    {
        contentPane.unionCoinTargetTf.DOScale(1.2f, 0.12f).SetLoops(2, LoopType.Yoyo).From(1);
    }

    void activityWorkerGameBreathAnim()
    {
        contentPane.activity_workerGameCoinTf.DOScale(1.2f, 0.12f).SetLoops(2, LoopType.Yoyo).From(1);
    }


    public void AddFlyGem(long changeNum)
    {
        var count = Mathf.Min(changeNum, 15);
        for (int i = 0; i < count; i++)
        {
            var newGO = GameObject.Instantiate(contentPane.flyGemTF.gameObject, FGUI.inst.vfxPlanel);
            newGO.SetActive(true);
            var fly = newGO.GetComponent<fly>();
            fly.tt = FGUI.inst.gemFlyTargetTf;

            //if (i == 0) fly.SetCallback(() => EventController.inst.TriggerEvent(GameEventType.ItemChangeEvent.GOLDNUM_ADD, oldNum, newNum));
        }
        EffectManager.inst.Spawn(3002, Vector3.zero, (gamevfx) =>
        {
            gamevfx.transform.SetParent(FGUI.inst.vfxPlanel.Find("center"), true);
            gamevfx.gameObject.SetActive(true);
            gamevfx.transform.localPosition = new Vector3(0, -(FGUI.inst.uiRootTF.sizeDelta.y / 8f), 0);
            GUIHelper.setRandererSortinglayer(gamevfx.transform, "top", 100);
        });
    }
}
