using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasserByPoolMgr : SingletonMono<PasserByPoolMgr>
{
    public int initNum = 10;
    private int passerbyUid;
    private bool isInit;

    Stack<Passerby> passerbyPool = new Stack<Passerby>();


    void initPasserByModels()
    {
        if (isInit || ManagerBinder.inst.mGameState != kGameState.Shop) return;
        isInit = true;

        for (int i = 0; i < initNum; i++)
        {
            Passerby passerby = IndoorMap.inst.CreatePasserby();
            passerby.transform.parent = transform;
            passerbyPool.Push(passerby);
        }
    }

    public Passerby GetPassBy(PasserbyData data)
    {
        if (ManagerBinder.inst.mGameState != kGameState.Shop) return null;

        if (!isInit) initPasserByModels();

        Passerby passerby = null;

        if (passerbyPool.Count > 0)
        {
            passerby = passerbyPool.Pop();
        }
        else
        {
            passerby = IndoorMap.inst.CreatePasserby();
            passerby.transform.parent = transform;
        }

        passerby.SetData(passerbyUid++, data);

        return passerby;
    }

    public void RecyclePasserby(Passerby passerby)
    {
        passerby.Pause();
        passerby.gameObject.transform.position = Vector3.right * 1000;
        passerbyPool.Push(passerby);
    }


}
