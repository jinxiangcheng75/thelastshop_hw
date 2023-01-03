using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapAICtrl : MonoBehaviour
{

    public Transform actor_node;

    private Vector3[] paths;

    //private DressUpSystem system;

    private int ranPathNumber;

    private Animator anim;

    private Tween tween;

    public void Init(Transform path_parent, bool randomOrigin)
    {

        ranPathNumber = -1;

        if (anim == null)
        {
            var seqFreameObj = Instantiate(Random.Range(0, 100) > 50 ? CityMap.inst.AI_man : CityMap.inst.AI_woman);
            anim = seqFreameObj.GetComponent<Animator>();

            seqFreameObj.transform.SetParent(actor_node);
            seqFreameObj.transform.localPosition = Vector3.zero;
            setPaths(path_parent, randomOrigin);
            //anim.gameObject.SetActiveTrue();
        }
        else
        {
            anim.runtimeAnimatorController = (Random.Range(0, 100) > 50 ? CityMap.inst.AI_man : CityMap.inst.AI_woman).GetComponent<Animator>().runtimeAnimatorController;
            setPaths(path_parent, randomOrigin);
            //anim.gameObject.SetActiveTrue();
        }

        //if (this.system == null)
        //{
        //    CharacterManager.inst.GetCharacter(Random.Range(0,100) > 50 ? 30001 : 40001, 0.035f, (system) =>
        //    {
        //        this.system = system;
        //        this.system.transform.SetParent(actor_node);
        //        this.system.transform.localPosition = Vector3.zero;
        //        this.system.SetSortingAndOrderLayer("map_Actor", 0);
        //        setPaths(path_parent);
        //    });
        //}
        //else
        //{
        //    this.system.SetActive(true);
        //    setPaths(path_parent);
        //}
    }


    private void setPaths(Transform root, bool randomOrigin)
    {
        float totalDis = 0;

        if (randomOrigin)
        {
            if (root.childCount <= 5)
            {
                onComplete();
                return;
            }

            //随机初始地点
            int num = Random.Range(3, root.childCount - 2);

            ranPathNumber = int.Parse(root.name) * 1000 + num;
            //筛除重合的人物
            if (MapAIManager.inst.randPool.Contains(ranPathNumber))
            {
                onComplete();
                return;
            }
            else
            {
                MapAIManager.inst.randPool.Add(ranPathNumber);
            }

            paths = new Vector3[root.childCount - num];

            for (int i = num; i < root.childCount; i++)
            {
                Vector3 pos = root.GetChild(i).position;
                int index = i - num;
                paths[index] = pos;
                if (index != 0) totalDis += Vector3.Distance(paths[index - 1], paths[index]);
            }
        }
        else
        {
            paths = new Vector3[root.childCount];

            int index = 0;
            foreach (Transform item in root)
            {
                paths[index] = item.position;
                if (index != 0) totalDis += Vector3.Distance(paths[index - 1], paths[index]);
                index++;
            }
        }


        move(totalDis);

    }

    private void move(float totalDis)
    {
        transform.position = paths[0];

        getScaleX(paths[0], paths[1]);

        float time = totalDis * Random.Range(4.5f, 5.5f);
        Logger.log("[城市游客]需要移动" + time + "秒完成该路径");

        tween = transform.DOPath(paths, time, PathType.Linear, PathMode.Ignore).SetEase(Ease.Linear).OnStart(() =>
        {
            //system.Play("walk", true);
            anim.gameObject.SetActiveTrue();
        });

        tween.onWaypointChange = onWaypointChange;

        GameObject obj = anim.gameObject;
        tween.onComplete = onComplete;
    }

    //到达下一个目的地
    private void onWaypointChange(int curIndex)
    {
        if (curIndex < paths.Length - 1)
        {
            Vector3 lastPos = paths[curIndex];
            Vector3 nextPos = paths[curIndex + 1];
            //system.SetDirection(getDir(lastPos, nextPos));

            Vector3 scale = anim.transform.localScale;
            scale.x = getScaleX(lastPos, nextPos);
            anim.transform.localScale = scale;
        }
    }

    //走完路径
    private void onComplete()
    {
        //system.SetActive(false);
        if (ranPathNumber != -1)
        {
            MapAIManager.inst.randPool.Remove(ranPathNumber);
        }
        anim.gameObject.SetActiveFalse();
        MapAIManager.inst.RecycleMapAI(this);
    }

    RoleDirectionType getDir(Vector3 from, Vector3 to)
    {
        Vector3 span = to - from;
        if (span.x > 0)
        {
            return RoleDirectionType.Right;
        }
        else
        {
            return RoleDirectionType.Left;
        }
    }

    int getScaleX(Vector3 from, Vector3 to)
    {
        Vector3 span = to - from;
        if (span.x > 0)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    private void OnDestroy()
    {
        if (tween != null) tween.Kill();
    }

}
