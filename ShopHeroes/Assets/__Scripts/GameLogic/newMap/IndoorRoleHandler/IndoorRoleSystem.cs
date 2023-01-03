using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//室内所有角色管理
public partial class IndoorRoleSystem
{
    public static IndoorRoleSystem inst;

    public IndoorRoleSystem()
    {
        inst = this;
        XLuaManager.inst.HotfixRaw(GetType().Name, this);
    }


    public void EnterSystem()
    {
        AddListeners_Shopper();
        AddListeners_StreetDrop();
        AddListeners_Pet();
        AddListeners_CanLockWorker();
    }


    public void ExitSystem()
    {

        RemoveListeners_Shopper();
        RemoveListeners_StreetDrop();
        RemoveListeners_Pet();
        RemoveListeners_CanLockWorker();

        clearRole();
    }


    void clearRole()
    {

        //清理顾客
        foreach (var shopper in shopperDic.Values)
        {
            if (shopper != null) shopper.Clear();
        }
        shopperDic.Clear();

        //清理宠物
        foreach (var pet in petRoleDic.Values)
        {
            if (pet != null) GameObject.Destroy(pet.gameObject);
        }
        petRoleDic.Clear();

        //暂停路人
        foreach (var item in passerbyDic.Values)
        {
            item.Pause();
        }

        //清理垃圾
        foreach (var item in streetDopDic.Values)
        {
            if (item != null) GameObject.Destroy(item.gameObject);
        }
        streetDopDic.Clear();

        //清理可招募工匠
        if (indoorCanLockWorker != null)
        {
            GameObject.Destroy(indoorCanLockWorker.gameObject);
        }

    }


    public void SetRolesVisible(bool visible)
    {

        //顾客
        foreach (var shopper in shopperDic.Values)
        {
            if (shopper != null && shopper.gameObject != null)
                shopper.SetVisible(visible);
        }

        //宠物
        foreach (var pet in petRoleDic.Values)
        {
            if (pet != null && pet.gameObject != null)
                pet.SetVisible(visible);
        }

        //可招募工匠
        if (indoorCanLockWorker != null && indoorCanLockWorker.gameObject != null && getCanLockWorkerId() != -1)
        {
            indoorCanLockWorker.SetVisible(visible);
        }

    }

    public void SetShopperHeadTipsVisible(bool visible) 
    {

        //顾客
        foreach (var shopper in shopperDic.Values)
        {
            if (shopper != null && shopper.gameObject != null)
                shopper.Attacher.SetVisible(visible);
        }

    }

}
