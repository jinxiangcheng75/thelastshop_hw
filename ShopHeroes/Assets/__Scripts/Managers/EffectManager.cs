using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏特效管理
/// </summary>
public class EffectManager : TSingletonHotfix<EffectManager>
{
    private Dictionary<uint, List<GameEffect>> readyObjects = new Dictionary<uint, List<GameEffect>>();
    private List<GameEffect> occupiedObjects = new List<GameEffect>();
    private Transform vFXPOOLRoot;
    public EffectManager()
    {
        //初始化
        readyObjects.Clear();
        occupiedObjects.Clear();
        foreach (uint key in VFXConfigManager.inst.VFXCfgs.Keys)
        {
            readyObjects.Add(key, new List<GameEffect>());
            occupiedObjects.Clear();
        }
        if (vFXPOOLRoot == null)
        {
            vFXPOOLRoot = new GameObject("POOL:VFXObject").transform;
            vFXPOOLRoot.position = Vector3.zero;
            GameObject.DontDestroyOnLoad(vFXPOOLRoot.gameObject);
        }
    }

    //ui上显示特效接口
    public GameEffect SpawnUIVFX(int vfxID, Canvas uiCanvas, Transform parent, bool loop = false, float life = 1f, Vector3 pos = new Vector3())
    {
        // GameEffect ge = Spawn(vfxID, loop, life, pos);
        // ge.transform.parent = parent;
        // ge.transform.localPosition = pos;
        // GUIHelper.setRandererSortinglayer(parent, uiCanvas.sortingLayerName, uiCanvas.sortingOrder);
        // return ge;
        return null;
    }

    //特效创建借口，返回特效自己挂到需要的节点上。
    public void Spawn(int vfxID, Vector3 pos, System.Action<GameEffect> callback = null)
    {
        if (!VFXConfigManager.inst.CheckForExistingTemplate(vfxID))
        {
            Debug.LogWarning("没有找到（" + vfxID.ToString() + "）特效的配置");
            return;
        }

        CreatEffect(vfxID, (_effect) =>
        {
            if (callback != null)
            {
                callback.Invoke(_effect);
            }
            else
            {
                _effect.gameObject.SetActive(true);
                _effect.transform.localPosition = pos;
                _effect.transform.rotation = Quaternion.identity;
                //playEffect(_effect, pos);
            }

        });
    }
    private void playEffect(GameEffect effect, Vector3 pos = new Vector3())
    {
        if (effect == null) return;


        // effect.onSpawn();
        //  occupiedObjects.Add(effect);
    }

    private void CreatEffect(int id, System.Action<GameEffect> callback)
    {
        VFXConfig cfg = VFXConfigManager.inst.GetConfig(id);
        if (cfg != null)
        {
            string vfxpath = cfg.vfxname;
            //资源加载
            // var newgo = GameObject.Instantiate(Resources.Load<GameObject>(vfxpath), vFXPOOLRoot);
            ManagerBinder.inst.Asset.InstantiatePrefabAsync(cfg.vfxname, (newgo) =>
            {
                GameEffect effect = newgo.GetComponent<GameEffect>() ?? newgo.AddComponent<GameEffect>();
                effect.vfxid = id;
                newgo.SetActive(false);
                callback(effect);
            });
        }
    }
    public bool Despawn(GameEffect effect)
    {
        GameObject.Destroy(effect.gameObject);
        return true;
    }
}
