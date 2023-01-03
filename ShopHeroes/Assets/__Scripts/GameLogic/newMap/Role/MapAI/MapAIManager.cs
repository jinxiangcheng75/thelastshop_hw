using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAIManager : SingletonMono<MapAIManager>
{

    public Transform actors_parent;

    private Transform[] paths;

    private const float REFRESHTIME_LOW = 5;
    private const float REFRESHTIME_HIGH = 8;
    private const float STARTPEOPLE_NUM = 10;

    private float timer;
    private float refreshTime;

    private System.Random rand;

    public List<int> randPool;

    Stack<MapAICtrl> actorPool;

    private void Start()
    {
        paths = new Transform[transform.childCount];

        int index = 0;
        foreach (Transform item in transform)
        {
            paths[index] = item;
            index++;
        }

        rand = new System.Random(Helper.GetRandomSeed());
        actorPool = new Stack<MapAICtrl>();

        startPeopleShow();
    }

    void startPeopleShow()
    {
        for (int i = 0; i < STARTPEOPLE_NUM; i++)
        {
            GameObject ai_Actor = GameObject.Instantiate(CityMap.inst.AI_actor);
            ai_Actor.transform.SetParent(actors_parent, true);
            MapAICtrl actor_ctrl = ai_Actor.GetComponent<MapAICtrl>();
            actor_ctrl.Init(getRandomPath(), true);
        }

        randPool.Clear();
    }

    private void Update()
    {
        if (ManagerBinder.inst.mGameState == kGameState.Town)
        {
            if (refreshTime == 0)
            {
                refreshTime = (float)(rand.NextDouble() * (REFRESHTIME_HIGH - REFRESHTIME_LOW) + REFRESHTIME_LOW);
                Logger.log("[城市]下一只城市游客出现的时间为 : " + refreshTime + "秒");
                return;
            }

            timer += Time.deltaTime;

            if (timer >= refreshTime)
            {
                timer = refreshTime = 0;

                if (actorPool.Count > 0)
                {
                    MapAICtrl actor_ctrl = actorPool.Pop();
                    actor_ctrl.Init(getRandomPath(), Random.Range(0, 100) > 50 ? true : false);
                }
                else
                {
                    GameObject ai_Actor = GameObject.Instantiate(CityMap.inst.AI_actor);
                    ai_Actor.transform.SetParent(actors_parent, true);
                    MapAICtrl actor_ctrl = ai_Actor.GetComponent<MapAICtrl>();
                    actor_ctrl.Init(getRandomPath(), Random.Range(0, 100) > 50 ? true : false);
                }
            }

        }

    }


    private Transform getRandomPath()
    {
        int index = rand.Next(0, paths.Length);
        Logger.log("[城市]所要去的路径为  第" + index + "条");
        return paths[index];
    }

    public void RecycleMapAI(MapAICtrl ai_actor)
    {
        actorPool.Push(ai_actor);
    }

}
