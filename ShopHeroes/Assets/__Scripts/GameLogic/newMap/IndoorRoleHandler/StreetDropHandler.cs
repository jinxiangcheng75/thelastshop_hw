using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//街道掉落物（美元 金块）
public partial class IndoorRoleSystem
{
    Dictionary<int, Vector3Int> passerbyPosCacheDic = new Dictionary<int, Vector3Int>();//街道路人位置缓存

    Dictionary<int, Passerby> passerbyDic = new Dictionary<int, Passerby>();
    Dictionary<int, StreetDrop> streetDopDic = new Dictionary<int, StreetDrop>();

    bool passerbyInit = false;

    void AddListeners_StreetDrop()
    {
        StreetDropInit();

        var e = EventController.inst;
        e.AddListener<int>(GameEventType.StreetDropEvent.STREETDROP_DATAREFRESH, rubbishRefresh);
        e.AddListener<int>(GameEventType.StreetDropEvent.STREETDROP_DEAL, dealStreetDrop);
        e.AddListener(GameEventType.StreetDropEvent.PASSBY_INITCREATE, initCreatePasserBy);


        //-------------------------------------------------------------------------------------------------------------------
        e.AddListener(GameEventType.StreetDropEvent.STREETDROP_REQUEST_REFRESH, request_RubbishRefresh);
        e.AddListener<int>(GameEventType.StreetDropEvent.STREETDROP_REQUEST_CLAIMED, request_RubbishClaimed);

    }

    void RemoveListeners_StreetDrop()
    {
        var e = EventController.inst;
        e.RemoveListener<int>(GameEventType.StreetDropEvent.STREETDROP_DATAREFRESH, rubbishRefresh);
        e.RemoveListener<int>(GameEventType.StreetDropEvent.STREETDROP_DEAL, dealStreetDrop);
        e.RemoveListener(GameEventType.StreetDropEvent.PASSBY_INITCREATE, initCreatePasserBy);


        //-------------------------------------------------------------------------------------------------------------------
        e.RemoveListener(GameEventType.StreetDropEvent.STREETDROP_REQUEST_REFRESH, request_RubbishRefresh);
        e.RemoveListener<int>(GameEventType.StreetDropEvent.STREETDROP_REQUEST_CLAIMED, request_RubbishClaimed);

    }

    void initCreatePasserBy()
    {
        if (UserDataProxy.inst.playerData.level < (int)WorldParConfigManager.inst.GetConfig(1501).parameters) return;

        if (passerbyInit) return;

        GameTimer.inst.AddTimer(2, 1, () =>
        {
            if (ManagerBinder.inst.mGameState != kGameState.Shop)
            {
                return;
            }

            int num = (int)WorldParConfigManager.inst.GetConfig(1302).parameters;


            List<StreetRoleOriPosConfig> cfgList = new List<StreetRoleOriPosConfig>(StreetRolePosConfigManager.inst.GetGetPassbyConfigs());
            List<StreetRoleOriPosConfig> ranList = new List<StreetRoleOriPosConfig>();

            for (int i = 0; i < num; i++)
            {
                int index = UnityEngine.Random.Range(0, cfgList.Count);
                ranList.Add(cfgList[index]);
                cfgList.RemoveAt(index);
            }

            foreach (var cfg in ranList)
            {
                var passerby = PasserByPoolMgr.inst.GetPassBy(new PasserbyData(cfg));
                AddPasserby(passerby);
            }

        });

        passerbyInit = true;

    }

    void initCreateStreetDrop()
    {
        var list = StreetDropDataProxy.inst.GetAllStreetDropData();

        foreach (var streetDropData in list)
        {
            if (streetDropData.state == StreetDropState.waitPick && !streetDopDic.ContainsKey(streetDropData.uid))
            {
                createStreetDropItemImmediate(streetDropData);
            }
        }
    }


    bool isStreetDropInit;
    public void StreetDropInit()
    {
        if (UserDataProxy.inst.playerData.level < (int)WorldParConfigManager.inst.GetConfig(1501).parameters) return;

        if (!isStreetDropInit)
        {
            foreach (var item in passerbyDic.Values)
            {
                RemovePasserby(item.passerbyUid);
            }

            var list = StreetDropDataProxy.inst.GetAllStreetDropData();
            foreach (var item in list)
            {
                item.state = StreetDropState.waitPick;
            }
        }

        isStreetDropInit = true;

        if (passerbyTimer != 0)
        {
            GameTimer.inst.RemoveTimer(passerbyTimer);
            passerbyTimer = 0;
        }

        if (passerbyTimer != 0)
        {
            GameTimer.inst.RemoveTimer(passerbyTimer);
            passerbyTimer = 0;
        }

        passerbyRefreshTime = UnityEngine.Random.Range(WorldParConfigManager.inst.GetConfig(1301).parameters, WorldParConfigManager.inst.GetConfig(1301).parameters + 10f);
        passerbyTimer = GameTimer.inst.AddTimer(passerbyRefreshTime, 1, passerbyTimerMethod);

        resumePasserbys();
        initCreateStreetDrop();
    }

    //路人刷新循环
    int passerbyTimer;
    float passerbyRefreshTime;
    void passerbyTimerMethod()
    {
        if (ManagerBinder.inst.mGameState == kGameState.Shop && IndoorMap.inst != null)  //在商店内刷新路人
        {
            if (passerbyDic.Count >= 3)  //上限为3个人 掉落垃圾的除外
            {
                passerbyRefreshTime = UnityEngine.Random.Range(WorldParConfigManager.inst.GetConfig(1301).parameters, WorldParConfigManager.inst.GetConfig(1301).parameters + 10f);
                passerbyTimer = GameTimer.inst.AddTimer(passerbyRefreshTime, 1, passerbyTimerMethod);
                return;
            }

            var passerby = PasserByPoolMgr.inst.GetPassBy(new PasserbyData(StreetRolePosConfigManager.inst.GetPassbyConfig()));
            AddPasserby(passerby);

            passerbyRefreshTime = UnityEngine.Random.Range(WorldParConfigManager.inst.GetConfig(1301).parameters, WorldParConfigManager.inst.GetConfig(1301).parameters + 10f);
            passerbyTimer = GameTimer.inst.AddTimer(passerbyRefreshTime, 1, passerbyTimerMethod);
        }
        else
        {
            passerbyRefreshTime = UnityEngine.Random.Range(WorldParConfigManager.inst.GetConfig(1301).parameters, WorldParConfigManager.inst.GetConfig(1301).parameters + 10f);
            passerbyTimer = GameTimer.inst.AddTimer(passerbyRefreshTime, 1, passerbyTimerMethod);
        }
    }


    private void resumePasserbys()
    {
        foreach (var item in passerbyDic.Values)
        {
            item.Resume();
        }
    }


    //请求（刷新）新的街道掉落物数据
    private void request_RubbishRefresh()
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Rubbish_Refresh()
                {

                }
            });
        }
    }

    //请求领取街道掉落物
    private void request_RubbishClaimed(int rubbishId)
    {
        if (AccountDataProxy.inst.isLogined)
        {
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Rubbish_Claimed()
                {
                    rubbishId = rubbishId,
                }
            });
        }

    }

    //街道掉落物刷新
    private void rubbishRefresh(int rubbishId)
    {
        if (UserDataProxy.inst.playerData.level < (int)WorldParConfigManager.inst.GetConfig(1501).parameters) return;


        var data = StreetDropDataProxy.inst.GetStreetDropData(rubbishId);
        if (data == null) return;


        switch (data.state)
        {
            case StreetDropState.notDrop:
                createPasserbyToDrop(data);
                break;
            case StreetDropState.waitPick:
                createStreetDropItemImmediate(data);
                break;
            case StreetDropState.alreadyGet:
                Logger.error("怎么会有已领取状态的发过来呢");
                break;
        }
    }

    public void AddPasserby(Passerby passerby)
    {
        if (!passerbyDic.ContainsKey(passerby.passerbyUid))
            passerbyDic.Add(passerby.passerbyUid, passerby);
    }

    public void RemovePasserby(int uid)
    {
        //缓存位置清除
        if (passerbyPosCacheDic.ContainsKey(uid))
        {
            passerbyPosCacheDic.Remove(uid);
        }

        //角色清除
        if (passerbyDic.ContainsKey(uid))
        {
            if (passerbyDic[uid] != null && passerbyDic[uid].gameObject != null)
            {
                //加入缓存池
                PasserByPoolMgr.inst.RecyclePasserby(passerbyDic[uid]);
            }
            passerbyDic.Remove(uid);
        }
    }

    public void AddStreetDrop(StreetDrop streetDrop)
    {
        if (streetDopDic.ContainsKey(streetDrop.data.uid))
        {
            GameObject.Destroy(streetDopDic[streetDrop.data.uid].gameObject);
            streetDopDic.Remove(streetDrop.data.uid);
        }

        streetDopDic.Add(streetDrop.data.uid, streetDrop);
    }

    void dealStreetDrop(int streetDropUid)
    {
        if (streetDopDic.ContainsKey(streetDropUid))
        {
            var streetDrop = streetDopDic[streetDropUid];

            if (streetDrop != null)
            {
                streetDrop.Clear();
                UserDataProxy.inst.DealWithAward(streetDrop.data.accessoryData);
            }

            streetDopDic.Remove(streetDropUid);
        }
    }


    void createStreetDropItemImmediate(StreetDropData data)
    {
        var streetDrop = IndoorMap.inst.CreateStreetDropItem(data);
        AddStreetDrop(streetDrop);
    }

    void createPasserbyToDrop(StreetDropData data)
    {
        var passerby = PasserByPoolMgr.inst.GetPassBy(new PasserbyData(data));
        AddPasserby(passerby);
    }

    public Vector3Int GetPasserbyOriCachePos(int passerbyUid)
    {
        if (passerbyPosCacheDic.ContainsKey(passerbyUid))
        {
            return passerbyPosCacheDic[passerbyUid];
        }

        return Vector3Int.zero;
    }

    public void SetPasserbyOriCachePos(int passerbyUid)
    {
        if (passerbyDic.TryGetValue(passerbyUid, out Passerby passerby))
        {
            if (passerby != null)
            {
                passerbyPosCacheDic[passerbyUid] = passerby.currCellPos;
            }
        }
    }

    public void DelPasserbyOriCachePos(int passerbyUid)
    {
        if (passerbyPosCacheDic.ContainsKey(passerbyUid))
        {
            passerbyPosCacheDic.Remove(passerbyUid);
        }
    }






}
