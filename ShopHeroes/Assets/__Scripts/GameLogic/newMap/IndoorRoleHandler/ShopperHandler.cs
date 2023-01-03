using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class IndoorRoleSystem
{
    Dictionary<int, Shopper> shopperDic = new Dictionary<int, Shopper>();

    Stack<int> _shopperStack = new Stack<int>();
    bool shopperIsReleased = false;
    int timerId = 0;
    float curTime;

    private void AddListeners_Shopper()
    {
        EventController.inst.AddListener(GameEventType.ShopperEvent.SHOPPERDATA_GETEND, UpdateStartShopper);   //刷新初始顾客
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_COMING_NEW, NewShopperComing); //新顾客
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_COMING_REPECT, repeatShopperCheck); //检查重复且是离开状态的顾客
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_REMOVE, ShopperRemove);

        EventController.inst.AddListener<int, EAIReadyToLeave>(GameEventType.ShopperEvent.Shopper_ChangeAIstate, ShopperChangeAIReadyToLeaveState);
        EventController.inst.AddListener<int, int>(GameEventType.ShopperEvent.SHOPPER_ChangeEquip, ShopperChangeEquip);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_SellItem, ShopperSellItem);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_Bargaining, ShopperBargaining);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_StopBargaining, ShopperStopBargaining);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_SHOPPERDATACHANGE, OnShopperDataChange);
        EventController.inst.AddListener(GameEventType.ShopperEvent.SHOPPER_SHOWALLANIM, ShopperShowAllAnim);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_HIDEBUBBLE, ShopperHideBubble);
        EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_ReShowPopupCheckOut, ShopperShowPopupCheckOut);

    }
    private void RemoveListeners_Shopper()
    {
        EventController.inst.RemoveListener(GameEventType.ShopperEvent.SHOPPERDATA_GETEND, UpdateStartShopper);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_COMING_NEW, NewShopperComing);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_COMING_REPECT, repeatShopperCheck); //检查重复且是离开状态的顾客
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_REMOVE, ShopperRemove);
        EventController.inst.RemoveListener<int, EAIReadyToLeave>(GameEventType.ShopperEvent.Shopper_ChangeAIstate, ShopperChangeAIReadyToLeaveState);
        EventController.inst.RemoveListener<int, int>(GameEventType.ShopperEvent.SHOPPER_ChangeEquip, ShopperChangeEquip);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_SellItem, ShopperSellItem);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_Bargaining, ShopperBargaining);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_StopBargaining, ShopperStopBargaining);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_SHOPPERDATACHANGE, OnShopperDataChange);
        EventController.inst.RemoveListener(GameEventType.ShopperEvent.SHOPPER_SHOWALLANIM, ShopperShowAllAnim);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_HIDEBUBBLE, ShopperHideBubble);
        EventController.inst.RemoveListener<int>(GameEventType.ShopperEvent.SHOPPER_ReShowPopupCheckOut, ShopperShowPopupCheckOut);

        GameTimer.inst.RemoveTimer(timerId);
        timerId = 0;
    }



    //初始化顾客列表
    void UpdateStartShopper()
    {
        FGUI.inst.StartCoroutine(initShopperList());
        timerId = GameTimer.inst.AddTimer(1, checkShopperRelease);
    }

    void addWaitShopper(int uid)
    {
        if (!_shopperStack.Contains(uid))
        {

            var data = ShopperDataProxy.inst.GetShopperData(uid);
            if (data.data.shopperComeType == (int)EShopperComeType.GuideTask)
            {
                //Logger.error("剧情顾客来了 停止此时的顾客释放列表  剧情顾客uid: " + uid);
                shopperIsReleased = false;
                curTime = WorldParConfigManager.inst.GetConfig(8201).parameters;
            }
            _shopperStack.Push(uid);
        }
    }

    void checkShopperRelease()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && GuideDataProxy.inst.CurInfo.isAllOver && !shopperIsReleased && _shopperStack.Count > 0)
        {
            if (ManagerBinder.inst != null && ManagerBinder.inst.mGameState == kGameState.Shop)  //在商店内刷新顾客
            {
                curTime++;

                if (curTime >= WorldParConfigManager.inst.GetConfig(8201).parameters)
                {
                    curTime = 0;
                    FGUI.inst.StartCoroutine(releaseWaitShoppers());
                }
            }
        }

    }

    IEnumerator releaseWaitShoppers()
    {
        if (!shopperIsReleased)
        {
            List<int> shopperUids = new List<int>();

            while (_shopperStack.Count > 0)
            {
                shopperUids.Add(_shopperStack.Pop());
            }

            if (shopperUids.Count > 0)
            {
                shopperUids.Reverse();
                shopperIsReleased = true;

                //Logger.error("开始释放缓存顾客列表");

                for (int i = 0; i < shopperUids.Count; i++)
                {
                    if (ManagerBinder.inst != null && (ManagerBinder.inst.mGameState != kGameState.Shop || !shopperIsReleased))
                    {

                        for (int k = shopperUids.Count - 1; k >= i; k--)
                        {
                            _shopperStack.Push(shopperUids[k]);
                            //Logger.error("剧情顾客强制插入，将尚未释放完的顾客列表加回缓存,,  " + shopperUids[k]);
                        }

                        break;
                    }

                    var data = ShopperDataProxy.inst.GetShopperData(shopperUids[i]);

                    if (data == null)
                    {
                        continue;
                    }

                    //Logger.error("释放缓存顾客列表中 顾客uid: " + data.data.shopperUid);


                    //如果是五倍商人来了 就让宠物去瞅他
                    if (data.data.shopperType == (int)EShopperType.HighPriceBuy)
                    {
                        EventController.inst.TriggerEvent(GameEventType.PetCompEvent.PET_GOTODOORWAY);
                    }

                    if (!shopperDic.ContainsKey(data.data.shopperUid))
                    {
                        Shopper newShopper = IndoorMap.inst.CreateShopper(data, true);
                        if(newShopper != null) shopperDic.Add(data.data.shopperUid, newShopper);
                    }

                    yield return new WaitForSeconds(Random.Range(WorldParConfigManager.inst.GetConfig(8202).parameters, WorldParConfigManager.inst.GetConfig(8203).parameters));
                }

                shopperIsReleased = false;
                //Logger.error("释放缓存顾客列表完毕");

            }
        }
    }

    IEnumerator initShopperList()
    {
        var customers = ShopperDataProxy.inst.GetShopperList();
        var queuqeingList = new List<ShopperData>();
        var newList = new List<ShopperData>();

        for (int i = 0; i < customers.Count; i++)
        {
            var cus = customers[i];
            bool isQueuing = cus.data.shopperState == (int)EShopperState.Queuing;

            if (isQueuing)
            {
                queuqeingList.Add(cus);
            }
            else
            {
                newList.Add(cus);
            }
        }

        for (int i = 0; i < queuqeingList.Count; i++)
        {
            var cus = queuqeingList[i];
            var shopper = IndoorMap.inst.CreateShopper(cus, false);

            if (shopperDic.ContainsKey(cus.data.shopperUid))
            {
                if (shopperDic[cus.data.shopperUid] != null)
                {
                    shopperDic[cus.data.shopperUid].DestroySelf();
                    shopperDic.Remove(cus.data.shopperUid);
                }
            }

            if(shopper != null) shopperDic.Add(cus.data.shopperUid, shopper);

            for (int k = 0; k < 3; k++)
            {
                yield return null;
            }
        }

        if (queuqeingList.Count == 0)
        {
            //EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.SHOPPEALLNEW);
            HotfixBridge.inst.TriggerLuaEvent("SHOPPEALLNEW");
        }

        for (int k = 0; k < newList.Count; k++)
        {
            var cus = newList[k];

            if (PlayerPrefs.GetString(AccountDataProxy.inst.account + "_" + cus.data.shopperUid + "_shopperPos", "-1") != "-1") //说明本地有缓存
            {
                cus.isCacheRamble = true;
                var shopper = IndoorMap.inst.CreateShopper(cus, true);

                if (shopperDic.ContainsKey(cus.data.shopperUid))
                {
                    if (shopperDic[cus.data.shopperUid] != null)
                    {
                        shopperDic[cus.data.shopperUid].DestroySelf();
                        shopperDic.Remove(cus.data.shopperUid);
                    }
                }

                if (shopper != null) shopperDic.Add(cus.data.shopperUid, shopper);

                for (int l = 0; l < 3; l++)
                {
                    yield return null;
                }

            }
            else
            {
                _shopperStack.Push(cus.data.shopperUid);
            }

        }

        FGUI.inst.StartCoroutine(releaseWaitShoppers());
    }


    public Shopper GetShopperByUid(int uid)
    {
        if (shopperDic.ContainsKey(uid))
        {
            return shopperDic[uid];
        }

        return null;
    }

    public List<Shopper> GetAllShopperList()
    {
        return shopperDic.Values.ToList();
    }

    public void AllMovedShopperRefreshPos()
    {
        //IndoorMapEditSys.inst.Shopkeeper.RefreshCurCellPos();
        HotfixBridge.inst.TriggerLuaEvent("REFRESHSHOPKEEPERCURCELLPOS");

        foreach (var shopper in GetAllShopperList())
        {
            if (shopper != null && shopper.GetCurState() == MachineShopperState.queuing)
            {
                shopper.RefreshCurCellPos(false);
            }
        }
    }


    //加入队列 延迟生成处理
    void NewShopperComing(int uid)
    {
        var data = ShopperDataProxy.inst.GetShopperData(uid);

        if (shopperDic.ContainsKey(uid)) //判断是否有重复的存在 拒绝让他离开不立即删除
        {
            shopperLeave(uid);
        }
        else
        {
            if (data.data.isGuide == 1) //新手引导 直接创建生成 
            {
                if (ManagerBinder.inst.mGameState != kGameState.Shop)
                {
                    return;
                }

                //如果是五倍商人来了 就让宠物去瞅他
                if (data.data.shopperType == (int)EShopperType.HighPriceBuy)
                {
                    EventController.inst.TriggerEvent(GameEventType.PetCompEvent.PET_GOTODOORWAY);
                }

                Shopper newShopper = IndoorMap.inst.CreateShopper(data, true);
                if(newShopper != null) shopperDic.Add(data.data.shopperUid, newShopper);
            }
            else
            {
                addWaitShopper(uid);
            }
        }

    }

    void repeatShopperCheck(int uid)
    {
        if (shopperDic.ContainsKey(uid)) //判断是否有重复的存在  立即删除
        {
            shopperDic[uid].DestroySelf();
        }
    }

    private void OnShopperDataChange(int shopperUid)
    {
        if (shopperDic.TryGetValue(shopperUid, out Shopper newShopper))
        {
            newShopper.OnDataChange();
        }
    }

    void ShopperRemove(int uid)
    {
        if (shopperDic.ContainsKey(uid))
        {
            shopperDic.Remove(uid);
        }
    }

    void shopperLeave(int uid)
    {
        if (shopperDic.TryGetValue(uid, out Shopper newShopper))
        {
            newShopper.SetState(MachineShopperState.leave);
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_REFUSE, uid, false);
        }
    }

    //顾客准备离开状态改变
    void ShopperChangeAIReadyToLeaveState(int uid, EAIReadyToLeave type)
    {
        if (shopperDic.TryGetValue(uid, out Shopper newShopper))
        {
            newShopper.readyLeaveType = type;
        }
    }

    //顾客换装
    void ShopperChangeEquip(int uid, int equipid)
    {
        if (shopperDic.TryGetValue(uid, out Shopper newShopper))
        {
            newShopper.ChangeEquip(equipid);
        }
    }

    //顾客卖出物品
    void ShopperSellItem(int uid)
    {
        if (shopperDic.TryGetValue(uid, out Shopper newShopper))
        {
            newShopper.SellItem();
        }
    }

    //动画打开所有顾客气泡
    void ShopperShowAllAnim()
    {
        foreach (Shopper shopper in GetAllShopperList())
        {
            //shopper.SetSpBubbleAlpha(1f, 0.35f);
            shopper.ShowPopupCheckOut();
        }

        //IndoorMapEditSys.inst.Shopkeeper.ShowAcheivementBubble();
        HotfixBridge.inst.TriggerLuaEvent("SETSHOPKEEPERACHEIVEMENTSTATE",true);

    }

    //刷新单个顾客的气泡内容
    void ShopperShowPopupCheckOut(int uid)
    {
        if (shopperDic.TryGetValue(uid, out Shopper newShopper))
        {
            newShopper.ShowPopupCheckOut();
        }
    }

    //动画关闭单个顾客气泡
    void ShopperHideBubble(int uid)
    {
        if (shopperDic.TryGetValue(uid, out Shopper newShopper))
        {
            newShopper.HidePopup();
        }
    }

    //顾客讨价还价
    void ShopperBargaining(int uid)
    {
        if (shopperDic.TryGetValue(uid, out Shopper shopper))
        {
            shopper.Bargaining();
        }
    }

    //顾客停止讨价还价
    void ShopperStopBargaining(int uid)
    {
        if (shopperDic.TryGetValue(uid, out Shopper shopper))
        {
            shopper.StopBargaining();
        }
    }

}
