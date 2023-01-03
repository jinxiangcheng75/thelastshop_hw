using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//商店场景界面。
public class GameShopUIMediator : BaseSystem
{
    MenuUIView menuUIView;

    TopPlayerInfoView topPlayerInfoView;
    TopSubUIView subTopView;
    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.SHOWUI_SHOPSCENE, ShowMainUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_SHOPSCENE, HideMainUI);

        EventController.inst.AddListener(GameEventType.MAINUI_SHIFTOUT, MainUIShiftOut);
        EventController.inst.AddListener(GameEventType.MAINUI_SHIFTIN, MainUIShiftIn);

        EventController.inst.AddListener(GameEventType.SHOWUI_SHOWTOPINFOUI, ShowTopPlayerInfo);

        EventController.inst.AddListener(GameEventType.BagEvent.BAG_DATA_UPDATE, updateResUI);

        EventController.inst.AddListener<int>(GameEventType.BagEvent.BAG_RES_UPDATE, OnResItemChange);  //资源道具 数量发生改变

        EventController.inst.AddListener<ESlotAnimType>(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, updateEquipMakeSlots);
        EventController.inst.AddListener(GameEventType.ProductionEvent.UIHanlde_RefreshMakeBtnState, updateEquipMakeBtnState);
        EventController.inst.AddListener<long, long, long>(GameEventType.Activity_WorkerGameCoin_Fly, flyActivityWorkerGameCoin);
        EventController.inst.AddListener<long, long, long>(GameEventType.UnionCoin_FLY, flyUnionCoin);
        EventController.inst.AddListener<long, long, long>(GameEventType.ENERGY_FLY, flyEnergy);
        EventController.inst.AddListener<long, long, long>(GameEventType.GOLD_FLY, flyGold);
        EventController.inst.AddListener<long>(GameEventType.GEM_FLY, flyGem);

        EventController.inst.AddListener(GameEventType.MoneyBoxEvent.MONEYBOX_ONDATAUPDATE, updateMoneyBoxData);
        EventController.inst.AddListener<long, long>(GameEventType.ItemChangeEvent.GOLDNUM_ADD, ChangeGoldNum);
        EventController.inst.AddListener<long, long>(GameEventType.ItemChangeEvent.GEMNUM_ADD, ChangeGemNum);
        EventController.inst.AddListener<long, long>(GameEventType.ItemChangeEvent.ENERGYNUM_ADD, ChangeEnergyNumIncrease);
        EventController.inst.AddListener<long, long>(GameEventType.ItemChangeEvent.ENERGYNUM_REDUCE, ChangeEnergyNumReduce);
        EventController.inst.AddListener(GameEventType.ItemChangeEvent.ENERGYLIMITNUM_CHANGE, ChangeEnergyLimitNum);
        EventController.inst.AddListener<long, long>(GameEventType.ItemChangeEvent.SELF_UNIONCOIN, ChangeSelfUnionCoinNum);
        EventController.inst.AddListener<long, long>(GameEventType.ItemChangeEvent.UNION_UNIONCOIN, ChangeUnionCoinNum);
        EventController.inst.AddListener<long, long>(GameEventType.ItemChangeEvent.ACTIVITY_WORKERGAME_COIN, ChangeActivityWorkerGameCoinNum);

        EventController.inst.AddListener<ShelfChange_Equip_SFX_Data>(GameEventType.FurnitureDisplayEvent.ShelfChange_Equip_SFX, ShelfChangeEquipSFX);
        EventController.inst.AddListener<int, string, int>(GameEventType.EquipEvent.SET_EQUIPFLY, SetEquipFly);

        EventController.inst.AddListener(GameEventType.EquipEvent.SET_GUIDETARGET, setGuideTarget);

        EventController.inst.AddListener(GameEventType.EmailEvent.SHOWUI_RefreshTaskRedPoint, refreshMainUIRedPoint);

        EventController.inst.AddListener(GameEventType.REFRESHMAINUIREDPOINT, refreshMainUIRedPoint);

        EventController.inst.AddListener(GameEventType.SHOWUI_SUBTOP, showSubTop);
        EventController.inst.AddListener(GameEventType.MenuEvent.REFRESHMAINUIPAYGIFTBTNS, refreshMainUIPayGiftBtns);

        EventController.inst.AddListener<TopPlayerShowType>(GameEventType.MenuEvent.SETTOPBTNSTATE, setTopBtnState);

        EventController.inst.AddListener(GameEventType.MenuEvent.REFRESHONLINEREWARDBTNS, refreshMainUIOnlineBtns);
        EventController.inst.AddListener(GameEventType.MenuEvent.REFRESHREFUGEBTN, refreshRefugeBtn);
        EventController.inst.AddListener<bool>(GameEventType.MenuEvent.SETREFUGEBTNSTATE, setRefugeBtnState);

        EventController.inst.AddListener<long>(GameEventType.USERDATA_EXPCHANGE, PlayerExpChange);
        EventController.inst.AddListener(GameEventType.MenuEvent.REFRESHLUXURYBTN, refreshLuxuryBtn);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_SHOPSCENE, ShowMainUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_SHOPSCENE, HideMainUI);

        EventController.inst.RemoveListener(GameEventType.MAINUI_SHIFTOUT, MainUIShiftOut);
        EventController.inst.RemoveListener(GameEventType.MAINUI_SHIFTIN, MainUIShiftIn);

        EventController.inst.RemoveListener(GameEventType.BagEvent.BAG_DATA_UPDATE, updateResUI);
        EventController.inst.RemoveListener<int>(GameEventType.BagEvent.BAG_RES_UPDATE, OnResItemChange);

        EventController.inst.RemoveListener<ESlotAnimType>(GameEventType.ProductionEvent.UPDATEUI_EQUIP_MAKESLOT, updateEquipMakeSlots);
        EventController.inst.RemoveListener(GameEventType.ProductionEvent.UIHanlde_RefreshMakeBtnState, updateEquipMakeBtnState);

        EventController.inst.RemoveListener<long, long, long>(GameEventType.UnionCoin_FLY, flyUnionCoin);
        EventController.inst.RemoveListener<long, long, long>(GameEventType.ENERGY_FLY, flyEnergy);
        EventController.inst.RemoveListener<long, long, long>(GameEventType.GOLD_FLY, flyGold);
        EventController.inst.RemoveListener<long>(GameEventType.GEM_FLY, flyGem);
        EventController.inst.RemoveListener(GameEventType.MoneyBoxEvent.MONEYBOX_ONDATAUPDATE, updateMoneyBoxData);
        EventController.inst.RemoveListener<long, long>(GameEventType.ItemChangeEvent.GOLDNUM_ADD, ChangeGoldNum);
        EventController.inst.RemoveListener<long, long>(GameEventType.ItemChangeEvent.GEMNUM_ADD, ChangeGemNum);
        EventController.inst.RemoveListener<long, long>(GameEventType.ItemChangeEvent.ENERGYNUM_ADD, ChangeEnergyNumIncrease);
        EventController.inst.RemoveListener<long, long>(GameEventType.ItemChangeEvent.ENERGYNUM_REDUCE, ChangeEnergyNumReduce);
        EventController.inst.RemoveListener(GameEventType.ItemChangeEvent.ENERGYLIMITNUM_CHANGE, ChangeEnergyLimitNum);
        EventController.inst.RemoveListener<long, long>(GameEventType.ItemChangeEvent.SELF_UNIONCOIN, ChangeSelfUnionCoinNum);
        EventController.inst.RemoveListener<long, long>(GameEventType.ItemChangeEvent.UNION_UNIONCOIN, ChangeUnionCoinNum);
        EventController.inst.RemoveListener<long, long>(GameEventType.ItemChangeEvent.ACTIVITY_WORKERGAME_COIN, ChangeActivityWorkerGameCoinNum);

        EventController.inst.RemoveListener<ShelfChange_Equip_SFX_Data>(GameEventType.FurnitureDisplayEvent.ShelfChange_Equip_SFX, ShelfChangeEquipSFX);
        EventController.inst.RemoveListener<int, string, int>(GameEventType.EquipEvent.SET_EQUIPFLY, SetEquipFly);

        EventController.inst.RemoveListener(GameEventType.EquipEvent.SET_GUIDETARGET, setGuideTarget);

        EventController.inst.RemoveListener(GameEventType.EmailEvent.SHOWUI_RefreshTaskRedPoint, refreshMainUIRedPoint);


        EventController.inst.RemoveListener(GameEventType.REFRESHMAINUIREDPOINT, refreshMainUIRedPoint);
        EventController.inst.RemoveListener(GameEventType.MenuEvent.REFRESHREFUGEBTN, refreshRefugeBtn);
        EventController.inst.RemoveListener<bool>(GameEventType.MenuEvent.SETREFUGEBTNSTATE, setRefugeBtnState);

        EventController.inst.RemoveListener(GameEventType.SHOWUI_SUBTOP, showSubTop);
        EventController.inst.RemoveListener(GameEventType.MenuEvent.REFRESHMAINUIPAYGIFTBTNS, refreshMainUIPayGiftBtns);

        EventController.inst.RemoveListener<TopPlayerShowType>(GameEventType.MenuEvent.SETTOPBTNSTATE, setTopBtnState);

        EventController.inst.RemoveListener(GameEventType.MenuEvent.REFRESHONLINEREWARDBTNS, refreshMainUIOnlineBtns);

        EventController.inst.RemoveListener<long>(GameEventType.USERDATA_EXPCHANGE, PlayerExpChange);
        EventController.inst.RemoveListener(GameEventType.MenuEvent.REFRESHLUXURYBTN, refreshLuxuryBtn);
    }

    void showSubTop()
    {
        if (!ReceiveAwardUIMediator.ismsguishow)
            subTopView = GUIManager.OpenView<TopSubUIView>();
    }

    void setGuideTarget()
    {
        if (menuUIView != null && menuUIView.isShowing) menuUIView.setTarget();
    }

    void refreshMainUIRedPoint()
    {
        if (menuUIView != null && menuUIView.isShowing) menuUIView.RefreshRedPoint();
    }

    private void ShelfChangeEquipSFX(ShelfChange_Equip_SFX_Data shelfChange_Equip_SFX_Data)
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.PlayEquipFly(shelfChange_Equip_SFX_Data);
        }
    }

    private void SetEquipFly(int slotId, string equipUid, int toStoreBasket)
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.SetEquipFly(slotId, equipUid, toStoreBasket);
        }
    }

    private void setTopBtnState(TopPlayerShowType type)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            GUIManager.curLuaTopPlayerShowType = type;
            topPlayerInfoView.shiftIn();
        }
        else
        {
            GUIManager.OpenView<TopPlayerInfoView>((view) =>
            {
                topPlayerInfoView = view;
            });
        }
    }

    private void updateMoneyBoxData()
    {
        //储蓄罐先隐藏
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateMoneyBoxData();
        }
        else if (menuUIView != null && menuUIView.isShowing)
        {
            if (topPlayerInfoView != null)
                topPlayerInfoView.updateMoneyBoxData();
        }
        else
        {
            var shopperUI = GUIManager.GetWindow<ShopperUIView>();
            if (shopperUI != null && shopperUI.isShowing)
            {
                if (topPlayerInfoView != null)
                {
                    topPlayerInfoView.updateMoneyBoxData();
                }
            }
        }
    }

    private void MainUIShiftOut()
    {
        menuUIView = GUIManager.GetWindow<MenuUIView>();
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.shiftOut();
        }
    }

    private void MainUIShiftIn()
    {
        menuUIView = GUIManager.GetWindow<MenuUIView>();
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.shiftIn();
        }
    }

    private void HideMainUI()
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.hide();
        }
    }
    private void ShowMainUI()
    {
        GUIManager.OpenView<MenuUIView>((view) =>
        {
            menuUIView = view;
        });
    }

    private void ShowTopPlayerInfo()
    {
        if (topPlayerInfoView == null || !topPlayerInfoView.isShowing)
        {
            GUIManager.OpenView<TopPlayerInfoView>((view) =>
            {
                topPlayerInfoView = view;
            });
        }
    }
    private void refreshMainUIPayGiftBtns()
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.RefreshPayGiftBtns();
        }
    }

    private void refreshMainUIOnlineBtns()
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.RefreshOnlineRewardBtns();
        }
    }

    private void refreshRefugeBtn()
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.RefreshRefugeBtn();
        }
    }

    private void setRefugeBtnState(bool state)
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.SetRefugeBtnState(state);
        }
    }

    private void refreshLuxuryBtn()
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.RefreshLuxuryItem();
        }
    }

    private void updateEquipMakeBtnState()
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.RefreshMakeBtnState();
        }
    }

    private void updateEquipMakeSlots(ESlotAnimType animType)
    {
        if (menuUIView != null && menuUIView.isShowing)
        {
            menuUIView.updateMakeSlots(animType);
        }
    }
    //更新资源ui显示
    private void updateResUI()
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateResUI();
        }
    }

    private void PlayerExpChange(long addexp)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.showExpChange(addexp);
        }
    }
    private void flyActivityWorkerGameCoin(long changeNum, long oldNum, long newNum)
    {
        if (topPlayerInfoView != null)
        {
            topPlayerInfoView.AddFlyActivityWorkerGameCoin(changeNum, oldNum, newNum);
        }
    }

    private void flyUnionCoin(long changeNum, long oldNum, long newNum)
    {
        if (topPlayerInfoView != null)
        {
            topPlayerInfoView.AddFlyUnionCoin(changeNum, oldNum, newNum);
        }
    }

    private void flyEnergy(long changeNum, long oldNum, long newNum)
    {
        if (topPlayerInfoView != null)
        {
            topPlayerInfoView.AddFlyEnergy(changeNum, oldNum, newNum);
        }
    }

    private void flyGold(long changeNum, long oldNum, long newNum)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.AddFlyGold(changeNum, oldNum, newNum);
        }
    }

    private void flyGem(long number)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.AddFlyGem(number);
        }
    }

    private void OnResItemChange(int itemId)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateResUI();
        }
    }

    private void ChangeGoldNum(long oldVal, long newVal)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateItemNum(oldVal, newVal, 1);
        }
    }

    private void ChangeGemNum(long oldVal, long newVal)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateItemNum(oldVal, newVal, 2);
        }
    }

    private void ChangeEnergyLimitNum()
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateEnergyLimitNum();
        }
    }

    private void ChangeEnergyNumIncrease(long oldVal, long newVal)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateItemNum(oldVal, newVal, 3);
        }
    }

    private void ChangeEnergyNumReduce(long oldVal, long newVal)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateItemNum(oldVal, newVal, 3);
        }
    }

    private void ChangeSelfUnionCoinNum(long oldVal, long newVal)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateItemNum(oldVal, newVal, 4);
        }
    }

    private void ChangeUnionCoinNum(long oldVal, long newVal)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateItemNum(oldVal, newVal, 5);
        }
    }

    private void ChangeActivityWorkerGameCoinNum(long oldVal, long newVal)
    {
        if (topPlayerInfoView != null && topPlayerInfoView.isShowing)
        {
            topPlayerInfoView.updateItemNum(oldVal, newVal, 6);
        }
    }

}
