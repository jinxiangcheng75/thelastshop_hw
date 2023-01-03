using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏对象缓冲管理

public class GameObjectPoolManager : SingletonMono<GameObjectPoolManager>
{
    //对象池
    public Dictionary<string, Stack> pool = new Dictionary<string, Stack>();

    public GameObject Get(string name, Vector3 position, Quaternion rotation)
    {
        string key = name;
        GameObject getItem = null;
        if (pool.ContainsKey(key) && pool[key].Count > 0)
        {
            Stack list = pool[key];
            getItem = list.Pop() as GameObject;
            getItem.SetActive(true);

            getItem.transform.position = position;
            getItem.transform.rotation = rotation;
        }
        else
        {
            //异步加载

        }
        return getItem;
    }
    public void DestroyObj(string key)
    {
        if (pool.ContainsKey(key) && pool[key].Count > 0)
        {
            foreach (GameObject obj in pool[key])
            {
                GameObject.DestroyImmediate(obj);
            }
            pool.Remove(key);
        }
    }
    public void Return(string key, GameObject go)
    {
        if (pool.ContainsKey(key))
        {
            pool[key].Push(go);
        }
        else
        {
            pool.Add(key, new Stack());
            pool[key].Push(go);
        }
        go.SetActive(false);
    }
}
