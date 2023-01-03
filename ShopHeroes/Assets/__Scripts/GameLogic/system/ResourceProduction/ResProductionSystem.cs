using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResProductionSystem : BaseSystem
{
    private int productionTimerId = -1;
    private ShopDesignUIView _shopDesignView;
    protected override void AddListeners()
    {

        EventController.inst.AddListener(GameEventType.ProductionEvent.GET_RES_PRODUCTIONLIST, GetResourceProductionList);
        EventController.inst.AddListener(GameEventType.ProductionEvent.UIREFRESH_UPDATEPRODUCTIONTIME, UpdateProductionRefresh);
        EventController.inst.AddListener<int>(GameEventType.ProductionEvent.UIREFRESH_PRODUCTIONREFRESHCOM, OnResourceProductionRefreshCom);

        EventController.inst.AddListener(GameEventType.ProductionEvent.SyncUpdateProductionData, UpdateProductionTime);

    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.ProductionEvent.GET_RES_PRODUCTIONLIST, GetResourceProductionList);
        EventController.inst.RemoveListener(GameEventType.ProductionEvent.UIREFRESH_UPDATEPRODUCTIONTIME, UpdateProductionRefresh);
        EventController.inst.RemoveListener<int>(GameEventType.ProductionEvent.UIREFRESH_PRODUCTIONREFRESHCOM, OnResourceProductionRefreshCom);

        EventController.inst.RemoveListener(GameEventType.ProductionEvent.SyncUpdateProductionData, UpdateProductionTime);

        // if (productionTimerId != -1)
        // {
        //     GameTimer.inst.RemoveTimer(productionTimerId);
        // }
        GameTimer.inst.StopCoroutine("ResUpdate");
        resupdateend = true;
    }

    private void UpdateProductionRefresh()
    {
        //开始
        if (productionTimerId == -1)
        {
            resupdateend = false;
            // _WaitForSeconds = new WaitForSeconds(1);
            GameTimer.inst.StartCoroutine(ResUpdate());
            productionTimerId = 1;
            //productionTimerId = GameTimer.inst.AddTimer(1, UpdateProductionTime);
        }
    }
    // WaitForSeconds _WaitForSeconds;
    public bool resupdateend = false;
    IEnumerator ResUpdate()
    {
        while (!resupdateend)
        {
            UpdateProductionTime();
            yield return new WaitForSeconds(1);
        }
    }

    private void UpdateProductionTime()
    {
        if (ManagerBinder.inst.mGameState == kGameState.Login) return;
        foreach (var production in UserDataProxy.inst.resSlotList.Values)
        {
            if (!production.isActivate) continue;
            if (production.duration <= -1) continue;
            if (ItemBagProxy.inst.resItemCount(production.resItemId) >= production.countLimit) continue;
            if (production.lastCollectTime == 0) production.time = 0;
            if (production.time >= production.duration)
            {
                //发送消息
                Item resitem = ItemBagProxy.inst.GetItem(production.resItemId);
                var targetcount = resitem.count + Mathf.FloorToInt((float)(production.time / production.duration));
                //production.time = 0;
                if (targetcount >= production.countLimit)
                {
                    targetcount = production.countLimit;
                    production.lastCollectTime = 0;
                    ItemBagProxy.inst.updateItemNum(production.resItemId, (long)targetcount);
                    NetworkEvent.SendRequest(new NetworkRequestWrapper()
                    {
                        req = new Request_Resource_ProductionRefresh()
                        {
                            itemId = (int)production.resItemId
                        }
                    });
                }
                else
                {
                    ItemBagProxy.inst.updateItemNum(production.resItemId, (long)targetcount);
                    production.time = 0;
                }
                OnResourceProductionRefreshCom(resitem.ID);
                EventController.inst.TriggerEvent(GameEventType.ProductionEvent.RES_PRODUCTIONLIST_REFRESHUI, resitem.ID);
            }
            //production.time += 1;
        }

    }


    private void GetResourceProductionList()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Resource_ProductionList()
        });
    }

    private void OnResourceProductionRefreshCom(int itemId)
    {
        //if (_shopDesignView == null)
        //    _shopDesignView = GUIManager.GetWindow<ShopDesignUIView>();
        //if (_shopDesignView != null && _shopDesignView.isShowing)
        //{
        //    var furnCfg = UserDataProxy.inst.GetFuriture(IndoorMapEditSys.inst.currEntityUid);
        //    if (furnCfg != null)
        //    {
        //        var upCfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(furnCfg.config.type_2, furnCfg.level);

        //        if (upCfg == null)
        //        {
        //            Logger.log("资源篮升级配置表未找到   家具类型2 ：" + furnCfg.config.type_2 + "家具等级 ：" + furnCfg.level);
        //            return;
        //        }

        //        if (upCfg.item_id == itemId)
        //        {
        //            _shopDesignView.onPickItem(IndoorMapEditSys.inst.currEntityUid);
        //        }
        //    }
        //}

        HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_OnResourceProductionRefreshCom", itemId);

    }
}
