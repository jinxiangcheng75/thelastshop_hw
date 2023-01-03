using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class IndoorMapEditSys
{
    ///新
    Shopkeeper shopKeeper;
    public Shopkeeper Shopkeeper
    {
        get { return shopKeeper; }
    }


    private void AddListeners_Shopkeeper()
    {
        EventController.inst.AddListener(GameEventType.ShopkeeperComEvent.CHANGECLOTHE, ShkpKeeperChange);
        EventController.inst.AddListener(GameEventType.ShopkeeperComEvent.ADDROUNDCOUNTERNUM, AddRoundCounterNum);
        EventController.inst.AddListener(GameEventType.ShopkeeperComEvent.SUBTRACTROUNDCOUNTERNUM, SubtractRoundCounterNum);
        EventController.inst.AddListener(GameEventType.ShopkeeperComEvent.SHOPPEALLNEW, ShopperAllNew);
        EventController.inst.AddListener(GameEventType.ShopkeeperComEvent.SHOPKEEPER_ACHEIVEMENTREFRESH, ShopkeeperAcheivementRefresh);
    }
    private void RemoveListeners_Shopkeeper()
    {
        EventController.inst.RemoveListener(GameEventType.ShopkeeperComEvent.CHANGECLOTHE, ShkpKeeperChange);
        EventController.inst.RemoveListener(GameEventType.ShopkeeperComEvent.ADDROUNDCOUNTERNUM, AddRoundCounterNum);
        EventController.inst.RemoveListener(GameEventType.ShopkeeperComEvent.SUBTRACTROUNDCOUNTERNUM, SubtractRoundCounterNum);
        EventController.inst.RemoveListener(GameEventType.ShopkeeperComEvent.SHOPPEALLNEW, ShopperAllNew);
        EventController.inst.RemoveListener(GameEventType.ShopkeeperComEvent.SHOPKEEPER_ACHEIVEMENTREFRESH, ShopkeeperAcheivementRefresh);
    }
    //创建场景店主
    void SpawnShopkeeper()
    {
        //if (shopKeeper != null)
        //{
        //    Logger.log("店主已存在！！！！");
        //    return;
        //}

        //shopKeeper = IndoorMap.inst.CreateShopkeeper();
        //if (GuideDataProxy.inst.CurInfo != null)
        //{
        //    if (!GuideDataProxy.inst.CurInfo.isAllOver && (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.NPCCreat)
        //    {
        //        shopKeeper.gameObject.SetActive(false);
        //        shopKeeper.SetState((int)MachineShopkeeperState.inGuide);
        //    }
        //}

        //var spKeeperPos = IndoorMap.inst.GetCounterOperationPos();
        //if (spKeeperPos != new Vector3Int(99999, 99999, 99999))
        //{
        //    shopKeeper.SetCellPos(spKeeperPos);
        //    shopKeeper.UpdateSortingOrder();
        //}
        //else
        //{
        //    spKeeperPos = MapUtils.IndoorCellposToMapCellPos(IndoorMap.inst.GetFreeGridPos());
        //    shopKeeper.SetCellPos(spKeeperPos);
        //    shopKeeper.UpdateSortingOrder();
        //}

        ShopkeeperAcheivementRefresh();
    }

    void ShopkeeperAcheivementRefresh()
    {

        if (AcheivementDataProxy.inst.NeedRedPoint)
        {
            Shopkeeper?.ShowAcheivementBubble();
        }
        else
        {
            Shopkeeper?.HideAcheivementBubble();
        }
    }

    void ShkpKeeperChange()
    {
        ///新
        if (shopKeeper != null)
        {
            shopKeeper.ChangeClothing();
        }
    }

    void ShopperAllNew()
    {
        ///新
        if (shopKeeper != null && shopKeeper.GetCurState() != MachineShopkeeperState.inGuide) shopKeeper.SetState((int)MachineShopkeeperState.ramble);
    }

    void AddRoundCounterNum()
    {
        ///新
        if (shopKeeper != null)
        {
            shopKeeper.roundCounterShopperNum++;
        }
    }

    void SubtractRoundCounterNum()
    {
        ///新
        if (shopKeeper != null)
        {
            shopKeeper.roundCounterShopperNum--;
        }
    }
}
