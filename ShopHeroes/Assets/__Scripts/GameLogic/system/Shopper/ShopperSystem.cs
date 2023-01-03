using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class ShopperSystem : BaseSystem
{
    ShopperUIView shopperUIView;
    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.ShopperEvent.SHOPPER_GETSHOPPERLIST, GetShopperData);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_SHOPPERDATACHANGE, OnShopperStateChange);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_START_CHECKOUT, OpenShopperBuyUI);
        EventController.inst.AddListener(GameEventType.ShopperEvent.SHOPPERDATA_GETEND, onShopperDataGetEnd);
        EventController.inst.AddListener<int, string>(GameEventType.ShopperEvent.SHOPPER_CHANGE_TAGETEQUIP, ShopperTargetChange);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_START_CHAT, ShopperChat);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_START_DISCOUNT, ShopperDiscount);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_START_DOUBLE, ShopperDouble);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_CHECKOUT, ShopperCheckout);
        EventController.inst.AddListener<int, bool>(GameEventType.ShopperEvent.SHOPPER_REFUSE, ShopperRefuse);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_CANCEL, CancelUI);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_CHANGE_NEXT, NextShopper);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_CHANGE_LAST, LastShopper);
        EventController.inst.AddListener<int, string, int>(GameEventType.ShopperEvent.SHOPPER_BUY_CONFIRM, ShopperConfirm);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_SETREFUSEUID, setRefuseShopperUid);
        EventController.inst.AddListener<int, int, int>(GameEventType.ShopperEvent.SHOPPER_LookOrnamental, shopperLookOrnamental);

        EventController.inst.AddListener(GameEventType.ShopperEvent.SHOPPER_UPDATEBYENERGY, UpdateShopperDataOnEnergyChanged);


        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Chat_Cmd, OnShopperChatResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Discount_Cmd, OnShopperDiscountResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Double_Cmd, OnShopperDoubleResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Refuse_Cmd, OnShopperRefuseResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Coming_Cmd, OnShopperComingResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Checkout_Cmd, OnShopperCheckoutResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Queue_Cmd, OnShopperQueueResp);
        Helper.AddNetworkRespListener(MsgType.Response_Shopper_Recommend_Cmd, onShopperRecommend);
    }

    protected override void RemoveListeners()
    {
        GameTimer.inst.RemoveTimer(timerId);
        EventController.inst.RemoveListener(GameEventType.ShopperEvent.SHOPPER_GETSHOPPERLIST, GetShopperData);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_SHOPPERDATACHANGE, OnShopperStateChange);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_START_CHECKOUT, OpenShopperBuyUI);
        EventController.inst.RemoveListener<int, string>(GameEventType.ShopperEvent.SHOPPER_CHANGE_TAGETEQUIP, ShopperTargetChange);
        EventController.inst.RemoveListener(GameEventType.ShopperEvent.SHOPPERDATA_GETEND, onShopperDataGetEnd);
        EventController.inst.RemoveListener<int, string, int>(GameEventType.ShopperEvent.SHOPPER_BUY_CONFIRM, ShopperConfirm);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_START_CHAT, ShopperChat);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_START_DISCOUNT, ShopperDiscount);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_START_DOUBLE, ShopperDouble);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_CHECKOUT, ShopperCheckout);
        EventController.inst.RemoveListener<int, bool>(GameEventType.ShopperEvent.SHOPPER_REFUSE, ShopperRefuse);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_CANCEL, CancelUI);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_CHANGE_NEXT, NextShopper);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_CHANGE_LAST, LastShopper);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_SETREFUSEUID, setRefuseShopperUid);
        EventController.inst.RemoveListener<int, int, int>(GameEventType.ShopperEvent.SHOPPER_LookOrnamental, shopperLookOrnamental);
        EventController.inst.RemoveListener(GameEventType.ShopperEvent.SHOPPER_UPDATEBYENERGY, UpdateShopperDataOnEnergyChanged);
    }

    int timerId = 0;
    protected override void OnInit()
    {
        timerId = GameTimer.inst.AddTimer(1, OnUpdate);
    }

    public override void ReInitSystem()
    {
        mDataReady = false;
        timerId = GameTimer.inst.AddTimer(1, OnUpdate);
    }

    //推荐
    private void onShopperRecommend(HttpMsgRspdBase msg)
    {
        Response_Shopper_Recommend data = (Response_Shopper_Recommend)msg;
        if (data.errorCode == 0)
        {
            //刷新头顶气泡
            //EventController.inst.TriggerEvent<int>(GameEventType.ShopperEvent.SHOPPER_ReShowPopupCheckOut,shopperUid);
        }
    }
    //顾客刷新循环 (3秒一次)
    private int requestDistance = 3;
    private float currtime = 0;
    bool mDataReady;
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!mDataReady)
            return;
        if (CSGameStart.Inst == null || !CSGameStart.Inst.onApplicationFocus)
            return;

        currtime += 1;
        if (ManagerBinder.inst.mGameState == kGameState.Shop)  //在商店内刷新顾客
        {
            if (currtime > requestDistance)
            {
                //if (ShopperDataProxy.inst.ShopperCount() <= 2)
                UpdateShopper();
                currtime = 0;
            }
        }
    }
    /////////////////////////
    //取消界面
    private void CancelUI(int shopperuid)
    {
        closeUI();
        D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
        //EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_SHOWALLANIM);
    }

    //打开顾客买卖
    private void OpenShopperBuyUI(int shopperuid)
    {
        var shopperData = ShopperDataProxy.inst.GetShopperData(shopperuid);
        if (shopperData != null)
        {
            if (shopperData.data.shopperState != 99) return;
        }

        var shopperList = IndoorRoleSystem.inst.GetAllShopperList();

        for (int i = 0; i < shopperList.Count; i++)
        {
            var shopper = shopperList[i];
            //shopperList[i].SetSpBubbleAlpha(0.5f, 0.35f);
            if (!shopper.isTalking)
            {
                shopperList[i].HidePopup();
            }
        }

        //IndoorMapEditSys.inst.Shopkeeper.HideAcheivementBubble();
        HotfixBridge.inst.TriggerLuaEvent("SETSHOPKEEPERACHEIVEMENTSTATE", false);

        shopperUIView = GUIManager.OpenView<ShopperUIView>(view =>
        {
            view.SetShopperData(shopperuid);
        });

    }


    void onShopperDataGetEnd()
    {
        mDataReady = true;
    }
    // 售卖切换顾客
    private void sellChangeShopper(int shopperuid)
    {

        List<int> ids = ShopperDataProxy.inst.GetShopperIdList();
        bool judgeResult = CheckShopperListIsAllLeave(ids);
        if (!judgeResult)
        {
            D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
            FGUI.inst.setGlobalMaskActice(false);
            //EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_SHOWALLANIM);
            closeUI();
            return;
        }

        var index = ids.IndexOf(shopperuid);
        index = CheckChangeShopperStateIsQueue(true, ids, index);

        //if (shopperUIView != null)
        {


            if (UserDataProxy.inst.playerData.level >= (int)WorldParConfigManager.inst.GetConfig(118).parameters)
            {
                var cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.shopperDeal);
                Vector3 position = IndoorRoleSystem.inst.GetShopperByUid(ids[index]).transform.position + new Vector3(cfg.X_revise, cfg.Y_revise, 0);
                D2DragCamera.inst.LookToPosition(position, true, cfg.zoom2 / 100f * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1));
                // shopperUIView.show((v) => shopperUIView.SetShopperData(ids[index]));
                shopperUIView = GUIManager.OpenView<ShopperUIView>(view =>
                {
                    view.SetShopperDataWithAnim(ids[index]);
                });
            }
            else
            {
                closeUI();
                D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
            }
        }

    }
    // 拒绝切换顾客
    private void refuseChangeShopper(int shopperuid)
    {
        List<int> ids = ShopperDataProxy.inst.GetShopperIdList();
        bool judgeResult = CheckShopperListIsAllLeave(ids);
        if (!judgeResult)
        {
            D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
            FGUI.inst.setGlobalMaskActice(false);
            closeUI();
            return;
        }

        var index = ids.IndexOf(shopperuid);
        index = CheckChangeShopperStateIsQueue(true, ids, index);

        //  if (shopperUIView != null)
        {



            if (UserDataProxy.inst.playerData.level >= (int)WorldParConfigManager.inst.GetConfig(118).parameters)
            {
                var cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.shopperDeal);
                Vector3 position = IndoorRoleSystem.inst.GetShopperByUid(ids[index]).transform.position + new Vector3(cfg.X_revise, cfg.Y_revise, 0);
                D2DragCamera.inst.LookToPosition(position, true, cfg.zoom2 / 100f * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1));
                shopperUIView = GUIManager.OpenView<ShopperUIView>(view =>
                {
                    view.SetShopperDataWithAnim(ids[index]);
                });
            }
            else
            {
                closeUI();
                D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
            }
        }
    }

    //IEnumerator NextShopper(int shopperUid, Action callback)
    //{

    //    Shopper shopper = IndoorMapEditSys.inst.GetShopperActor(shopperUid);
    //    if (shopper == null)
    //    {
    //        callback?.Invoke();
    //    }
    //    else
    //    {
    //        ActorAI ai = shopper.gameObject.GetComponent<ActorAI>();

    //        if (ai == null)
    //        {
    //            callback?.Invoke();
    //        }
    //        else
    //        {
    //            while (ai.readyLeaveType != EAIReadyToLeave.none)
    //            {
    //                yield return null;
    //            }

    //            callback?.Invoke();
    //        }
    //    }
    //}

    //关闭页面
    private void closeUI()
    {
        GUIManager.HideView<ShopperUIView>();
        shopperUIView = null;
    }

    private bool CheckShopperListIsAllLeave(List<int> ids)
    {
        if (IndoorMapEditSys.inst == null) return false;

        for (int i = 0; i < ids.Count; i++)
        {
            Shopper shopper = IndoorRoleSystem.inst.GetShopperByUid(ids[i]);
            if (shopper != null)
            {
                if (shopper.shopperData.data.shopperState == 99 && shopper.GetCurState() == MachineShopperState.queuing)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int CheckChangeShopperStateIsQueue(bool isAdd, List<int> ids, int curIndex)
    {
        int num = isAdd ? 1 : -1;
        int startIndex = curIndex;
        while (true)
        {
            curIndex += num;
            if (isAdd)
            {
                if (curIndex >= ids.Count)
                    curIndex = 0;
            }
            else
            {
                if (curIndex < 0)
                    curIndex = ids.Count - 1;
            }


            Shopper shopper = IndoorRoleSystem.inst.GetShopperByUid(ids[curIndex]);
            if (shopper != null)
            {
                if (shopper.shopperData.data.shopperState == 99 && shopper.GetCurState() == MachineShopperState.queuing)
                {
                    return curIndex;
                }
            }

            if (curIndex == startIndex)
                return startIndex;
        }
    }

    //前一个 顾客
    private void LastShopper(int shopperuid)
    {
        List<int> ids = ShopperDataProxy.inst.GetShopperIdList();
        var index = ids.IndexOf(shopperuid);
        index = CheckChangeShopperStateIsQueue(false, ids, index);
        //if ((--index) < 0)
        //{
        //    index = ids.Count - 1;
        //}
        if (shopperUIView != null && shopperUIView.isShowing)
        {
            //Shopper lastShopper = IndoorMapEditSys.inst.GetShopperActor(ids[index]);

            CameraMoveConfig cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.shopperDeal);
            Vector3 position = IndoorRoleSystem.inst.GetShopperByUid(ids[index]).transform.position + new Vector3(cfg.X_revise, cfg.Y_revise, 0);
            D2DragCamera.inst.LookToPosition(position, true, cfg.zoom2 / 100f * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1), ea: DG.Tweening.Ease.OutCubic);
            shopperUIView.SetShopperDataWithAnim(ids[index], true);

        }
    }
    //下一个顾客
    private void NextShopper(int shopperuid)
    {
        List<int> ids = ShopperDataProxy.inst.GetShopperIdList();
        var index = ids.IndexOf(shopperuid);
        index = CheckChangeShopperStateIsQueue(true, ids, index);

        if (shopperUIView != null && shopperUIView.isShowing)
        {
            //Shopper nextShopper = IndoorMapEditSys.inst.GetShopperActor(ids[index]);

            CameraMoveConfig cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.shopperDeal);
            Vector3 position = IndoorRoleSystem.inst.GetShopperByUid(ids[index]).transform.position + new Vector3(cfg.X_revise, cfg.Y_revise, 0);
            D2DragCamera.inst.LookToPosition(position, true, cfg.zoom2 / 100f * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1), ea: DG.Tweening.Ease.OutCubic);
            shopperUIView.SetShopperDataWithAnim(ids[index], true);

        }
    }

    private void setRefuseShopperUid(int uid)
    {
        curId = uid;
    }

    //客户 观赏装饰
    private void shopperLookOrnamental(int shopperUid, int furnitureUid, int petUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Energy()
            {
                shopperUid = shopperUid,
                furnitureUid = furnitureUid,
                petUid = petUid,
            }
        });
    }


    //客户 状态变化
    private void OnShopperStateChange(int uid)
    {
        if (shopperUIView != null && shopperUIView.isShowing)
        {
            shopperUIView.UpdateInfo(uid);
        }
    }

    private void UpdateShopperDataOnEnergyChanged()
    {
        if (shopperUIView != null && shopperUIView.isShowing)
        {
            shopperUIView.UpdateInfo();
        }
    }
    #region  服务器返回

    //刷新新顾客
    private void OnShopperComingResp(HttpMsgRspdBase msg)
    {
        Response_Shopper_Coming data = (Response_Shopper_Coming)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
        }
    }
    // 闲聊
    private void OnShopperChatResp(HttpMsgRspdBase msg)
    {
        Response_Shopper_Chat data = (Response_Shopper_Chat)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            if (data.chatSuccess == 0)
            {
                AudioManager.inst.PlaySound(137);
            }
            else
            {
                AudioManager.inst.PlaySound(136);
            }
        }
    }
    //折扣
    private void OnShopperDiscountResp(HttpMsgRspdBase msg)
    {
        Response_Shopper_Discount data = (Response_Shopper_Discount)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {

        }
    }
    //加价
    private void OnShopperDoubleResp(HttpMsgRspdBase msg)
    {
        Response_Shopper_Double data = (Response_Shopper_Double)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {

        }
    }
    public int curId;
    //拒绝
    private void OnShopperRefuseResp(HttpMsgRspdBase msg)
    {
        Response_Shopper_Refuse data = (Response_Shopper_Refuse)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //closeUI();

        }
    }

    void showCheckOutAwardUI(List<OneRewardItem> rewardList)
    {
        if (rewardList.Count > 1)
        {
            List<CommonRewardData> tempList = new List<CommonRewardData>();
            for (int i = 0; i < rewardList.Count; i++)
            {
                if ((ItemType)rewardList[i].itemType != ItemType.Glod)
                {
                    CommonRewardData tempData = new CommonRewardData(rewardList[i].itemId, rewardList[i].count, rewardList[i].quality, rewardList[i].itemType);
                    tempList.Add(tempData);
                }
            }

            if (tempList.Count > 1)
            {
                EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONREWARD_SETINFO, tempList);
            }
            else if (tempList.Count == 1)
            {
                CommonRewardData commonRewardData = tempList[0];

                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", commonRewardData.itemType == 26 ? commonRewardData.rewardId : 0, commonRewardData.itemType == 26 ? 0 : commonRewardData.rewardId, commonRewardData.count));
            }
        }
        else if (rewardList.Count == 1)
        {
            if ((ItemType)rewardList[0].itemType != ItemType.Glod)
            {
                OneRewardItem oneRewardItem = rewardList[0];

                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", oneRewardItem.itemType == 26 ? oneRewardItem.itemId : 0, oneRewardItem.itemType == 26 ? 0 : oneRewardItem.itemId, oneRewardItem.count));
            }
        }
    }

    //结算
    private void OnShopperCheckoutResp(HttpMsgRspdBase msg)
    {
        Response_Shopper_Checkout data = (Response_Shopper_Checkout)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //closeUI();

            //Logger.error("顾客结算来了 3");

            //FGUI.inst.showGlobalMask(3);//开启透明遮罩

            var shopperData = ShopperDataProxy.inst.GetShopperData(data.shopperUid);

            if (shopperData != null && (shopperData.data.shopperType == (int)EShopperType.Buy || shopperData.data.shopperType == (int)EShopperType.Sell))
            {
                sellChangeShopper(data.shopperUid);
            }
            else
            {
                closeUI();
                D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
            }

            if (IndoorMapEditSys.inst.shelfEquipToShopperHandlers.ContainsKey(data.shopperUid))//货架的装备飞过来
            {
                Response_Shopper_Checkout delay_data = data;

                IndoorMapEditSys.inst.shelfEquipToShopperHandlers[delay_data.shopperUid] = () =>
                {
                    if (IndoorMap.inst == null) return;

                    //顾客买到装备
                    if (delay_data.shopperEquipId > 0)
                    {
                        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_ChangeEquip, delay_data.shopperUid, delay_data.shopperEquipId);
                    }

                    showCheckOutAwardUI(data.rewardList);

                    //sellChangeShopper(data.shopperUid);
                };
            }
            else//直接换装进行结算一系列行为
            {
                if (IndoorMap.inst == null) return;

                //顾客买到装备
                if (data.shopperEquipId > 0)
                {
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_ChangeEquip, data.shopperUid, data.shopperEquipId);
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_SellItem, data.shopperUid);
                }

                showCheckOutAwardUI(data.rewardList);

                //sellChangeShopper(data.shopperUid);
            }
        }
    }
    //确认有无装备
    private void OnShopperQueueResp(HttpMsgRspdBase msg)
    {
        var data = msg as Response_Shopper_Queue;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            EventController.inst.TriggerEvent<int, string>(GameEventType.ShopperEvent.SHOPPER_BUY_CONFIRM_RESULT, data.shopperUid, data.equipUid);
        }
    }
    #endregion
    #region  请求服务器
    private void ShopperTargetChange(int shopperuid, string equipuid)
    {
        EquipItem eitem = ItemBagProxy.inst.GetEquipItem(equipuid);
        if (eitem == null) return;
        if (UserDataProxy.inst.playerData.energy >= eitem.equipConfig.equipQualityConfig.recommend_energy)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Shopper_Recommend()
                {
                    shopperUid = shopperuid,
                    equipUid = equipuid
                }
            });
        }
        else
        {
            //体力不足
        }
    }
    //请求商店内顾客数据
    private void GetShopperData()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Data()
        });
    }

    //刷新顾客是否有新的顾客来
    private void UpdateShopper()
    {
        //if (ShopperDataProxy.inst.GetShopperList().Count < 1)
        //{
        //    GetShopperData();
        //}
        //else
        //{

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Coming()
            {
                shopperUidList = ShopperDataProxy.inst.GetNotLeaveShopperUidList(),
            }
        });
        //}
    }

    //闲聊
    private void ShopperChat(int uid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Chat()
            {
                shopperUid = uid
            }
        });
    }

    //打折
    private void ShopperDiscount(int uid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Discount()
            {
                shopperUid = uid
            }
        });
    }
    //加价
    private void ShopperDouble(int uid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Double()
            {
                shopperUid = uid
            }
        });

        var shopperdata = ShopperDataProxy.inst.GetShopperData(uid);
        if (shopperdata != null)
        {
            shopperdata.setPriceDouble();
            OnShopperStateChange(uid);
        }
    }

    //结账
    private void ShopperCheckout(int uid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Checkout()
            {
                shopperUid = uid
            }
        });
    }
    //拒绝
    private void ShopperRefuse(int uid, bool isAuto)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Refuse()
            {
                shopperUid = uid
            }
        });

        if (isAuto && curId != uid) return;

        shopperUIView = GUIManager.GetWindow<ShopperUIView>();
        if (shopperUIView != null && shopperUIView.isShowing)
        {
            var shopperData = ShopperDataProxy.inst.GetShopperData(curId);

            if (shopperData != null && (shopperData.data.shopperType == (int)EShopperType.Buy || shopperData.data.shopperType == (int)EShopperType.Sell))
            {
                refuseChangeShopper(curId);
            }
            else
            {
                closeUI();
                D2DragCamera.inst.RestorePositionAndOrthgraphicSize();
            }
        }
    }
    //确认有无装备
    private void ShopperConfirm(int uid, string equipUid, int equipId)
    {

        //Logger.error("_________shopperid :" + uid + "   前端发给后端最终想要的装备uid :" + equipUid  + "    前端发给后端最终想要的装备id :" + equipId);

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Shopper_Queue()
            {
                shopperUid = uid,
                equipUid = equipUid,
                equipId = equipId,
            }
        });
    }
    #endregion
}
