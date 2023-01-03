using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DeserializeSlot
{
    public int index;//下标
    public string name;//槽位名称
    public string attachmentName;//附件名称
}

public class DeserializeDataAsset
{
    public string aa_address;//aa包地址
    public float scale;//dataAsset大小
    public List<DeserializeSlot> desSlots;//槽位数据

}

//角色管理器（实例显示）
public class CharacterManager : TSingletonHotfix<CharacterManager>
{
    private Dictionary<string, SkeletonDataAsset> _assetPool;//spine SkeletonDataAsset对象池
    private Dictionary<string, int> _assetDependNumDic;//spine SkeletonDataAsset对象引用计数池

    private int checkClearAssetPoolTimer;
    private bool isClealAssetDepending; //正在清理缓存

    private Dictionary<string, DeserializeDataAsset> _aseeDeserializeDic;//spine assets反序列化池子

    private List<string> _needCacheAddreessList; //spine 需要缓存的asset address

    private const float characterScale = 0.14f;

    void deserialize()//反序列化
    {
        string depKey = "spineDependency";

        ManagerBinder.inst.Asset.loadMiscAsset<TextAsset>(depKey, (depJson) =>
        {
            string jsonContent = depJson.text;

            IList<object> datas = MiniJSON.Json.Deserialize(jsonContent) as IList<object>;

            foreach (Dictionary<string, object> item in datas)
            {
                DeserializeDataAsset deserializeData = new DeserializeDataAsset();

                deserializeData.aa_address = item["address"] as string;

                deserializeData.scale = float.Parse(item["assetScale"].ToString());

                deserializeData.desSlots = new List<DeserializeSlot>();

                var dic = item["slotsData"] as Dictionary<string, System.Object>;

                foreach (var it in dic)
                {
                    DeserializeSlot slot = new DeserializeSlot();
                    slot.index = int.Parse(it.Key);

                    var slotData = it.Value as Dictionary<string, object>;

                    slot.name = slotData["name"] as string;
                    slot.attachmentName = slotData["attachmentName"] as string;

                    deserializeData.desSlots.Add(slot);
                }

                _aseeDeserializeDic.Add(deserializeData.aa_address, deserializeData);
            }

            // ManagerBinder.inst.Asset.unloadMiscAsset(depKey, depJson);

            Logger.log("spine资源反序列化完毕");
        });

        //要缓存的spineAssetAddresses
        string cacheAddressesKey = "spineNeedCacheAddresses";

        ManagerBinder.inst.Asset.loadMiscAsset<TextAsset>(cacheAddressesKey, (cacheAddressesJson) =>
        {
            string jsonContent = cacheAddressesJson.text;
            var arr = MiniJSON.Json.Deserialize(jsonContent) as IList<object>;

            foreach (var item in arr)
            {
                _needCacheAddreessList.Add(item as string);
            }

            // ManagerBinder.inst.Asset.unloadMiscAsset(cacheAddressesKey, cacheAddressesJson);
        });

    }

    protected override void init()
    {
        _assetPool = new Dictionary<string, SkeletonDataAsset>();
        _assetDependNumDic = new Dictionary<string, int>();
        _aseeDeserializeDic = new Dictionary<string, DeserializeDataAsset>();
        _needCacheAddreessList = new List<string>();

        deserialize();
    }

    public string GetPeopleShapeNudeSpinePath(EGender gender) //性别 人形裸模
    {
        string spinePath = "";

        spinePath = gender == EGender.Male ? "SkeletonDataman" : "SkeletonDatawoman";

        return spinePath;
    }

    void checkClearAssetPool()
    {
        if (checkClearAssetPoolTimer == 0)
        {
            checkClearAssetPoolTimer = GameTimer.inst.AddTimer(0.5f, 1, ClearDataAssetCache);
        }
        else
        {
            GameTimer.inst.RemoveTimer(checkClearAssetPoolTimer);
            checkClearAssetPoolTimer = GameTimer.inst.AddTimer(0.5f, 1, ClearDataAssetCache);
        }
    }

    //获取asset
    public void GetSkeletonDataAsset(string assetPath, Action<SkeletonDataAsset> callBackHandler)
    {
        if (_assetPool.ContainsKey(assetPath))
        {
            if (!_needCacheAddreessList.Contains(assetPath)) //如果不是恒定缓存 计数++
            {
                _assetDependNumDic[assetPath]++;
            }

            callBackHandler?.Invoke(_assetPool[assetPath]);

            checkClearAssetPool();

        }
        else
        {
            if (FGUI.inst != null)
            {
                FGUI.inst.StartCoroutine(getSkeletonDataAsset(assetPath, callBackHandler));
            }
        }
    }

    IEnumerator getSkeletonDataAsset(string assetPath, Action<SkeletonDataAsset> callBackHandler)
    {
        while (isClealAssetDepending)
        {
            yield return null;
        }

        ManagerBinder.inst.Asset.loadMiscAsset<SkeletonDataAsset>(assetPath, (asset) =>
        {
            callBackHandler?.Invoke(asset);

            if (!_assetPool.ContainsKey(assetPath))
            {
                _assetPool.Add(assetPath, asset);

                if (!_needCacheAddreessList.Contains(assetPath))
                {
                    _assetDependNumDic.Add(assetPath, 1);
                }
            }

            checkClearAssetPool();

        });
    }

    //卸载asset
    public void UnLoadSkeletonDataAsset(string assetPath)
    {

        if (_assetPool.ContainsKey(assetPath))
        {
            if (_assetDependNumDic.ContainsKey(assetPath))
            {
                _assetDependNumDic[assetPath]--;
            }
        }

    }

    public void UnLoadAllSkeletonDataAsset() 
    {
        isClealAssetDepending = true;

        if (FGUI.inst != null) FGUI.inst.StopCoroutine(clearDataAssetCache());

        foreach (var key in _assetPool.Keys)
        {
            ManagerBinder.inst.Asset.unloadMiscAsset(key, _assetPool[key]);
        }

        _assetPool.Clear();
        _assetDependNumDic.Clear();

        isClealAssetDepending = false;

    }

    void ClearDataAssetCache()
    {
        if (_assetPool.Count < 50)
        {
            checkClearAssetPoolTimer = 0;
            return;
        }

        Logger.log("清理spine资源缓存中,,,,,,,,,,,,");

        if (FGUI.inst != null)
        {
            FGUI.inst.StartCoroutine(clearDataAssetCache());
        }
    }

    IEnumerator clearDataAssetCache()
    {
        isClealAssetDepending = true;

        List<string> moveKeys = new List<string>();

        foreach (string key in _assetDependNumDic.Keys)
        {
            if (_assetDependNumDic[key] <= 0) //无引用次数
            {
                if (_assetPool.ContainsKey(key))
                {
                    ManagerBinder.inst.Asset.unloadMiscAsset(key, _assetPool[key]);
                    _assetPool.Remove(key);
                }

                moveKeys.Add(key);
            }

        }

        for (int i = 0; i < moveKeys.Count; i++)
        {
            _assetDependNumDic.Remove(moveKeys[i]);
        }

        for (int i = 0; i < 10; i++)
        {
            yield return null;
        }

        Spine.Unity.AttachmentTools.AtlasUtilities.ClearCache();
        checkClearAssetPoolTimer = 0;
        isClealAssetDepending = false;
    }


    //获取asset关联slots数据
    public DeserializeDataAsset GetDeserializeDataAsset(string assetPath)
    {
        if (_aseeDeserializeDic.ContainsKey(assetPath))
        {
            return _aseeDeserializeDic[assetPath];
        }

        return null;
    }

    // 同步创建spine角色脚本
    public T CreatRuntimeAssetsAndGameObjectSync<T>(SkeletonDataAsset asset) where T : DressUpBase
    {
        T system = null;

        if (typeof(T) == typeof(DressUpSystem))
        {
            // 代码获取spine导出资源
            SkeletonAnimation runtimeSkeletonAnimation = SkeletonAnimation.NewSkeletonAnimationGameObject(asset);
            system = runtimeSkeletonAnimation.gameObject.AddComponent<T>();
        }
        else if (typeof(T) == typeof(GraphicDressUpSystem))
        {
            // 代码获取spine导出资源
            SkeletonGraphic runtimeSkeletonAnimation = SkeletonGraphic.NewSkeletonGraphicGameObject(asset, null, new Material(Shader.Find("Spine/SkeletonGraphic")));
            system = runtimeSkeletonAnimation.gameObject.AddComponent<T>();
        }

        return system;
    }

    //创建spine角色脚本
    private void CreatRuntimeAssetsAndGameObject<T>(string assetPath, Action<T> callBackHandler = null) where T : DressUpBase
    {
        GetSkeletonDataAsset(assetPath, (skeletonData) =>
        {
            T dressSystem = CreatRuntimeAssetsAndGameObjectSync<T>(skeletonData);
            callBackHandler?.Invoke(dressSystem);
        });
    }

    //装备ID列表转换为DressId列表
    public List<int> EquipIdsToDressIds(List<int> equipIdList)
    {
        List<int> result = new List<int>();


        if (equipIdList != null)
        {
            for (int i = 0; i < equipIdList.Count; i++)
            {
                var equipCfg = EquipConfigManager.inst.GetEquipQualityConfig(equipIdList[i]);

                if (equipCfg == null)
                {
                    Logger.error("未找到该装备    装备id : " + equipIdList[i]);
                }
                else
                {
                    if (equipCfg.dressId == 0)
                    {
                        Logger.log("装备id为" + equipIdList[i] + "的equip表暂未配置dressId");
                        continue;
                    }

                    result.Add(equipCfg.dressId);
                }
            }
        }

        return result;
    }


    /// <summary>
    /// 获取角色[模型表]
    /// </summary>
    /// <param name="modelId">对应 outward_database表中的id</param>
    public void GetCharacterByModel<T>(int modelId, float scale = characterScale, bool initRepacked = true, Action<T> callback = null, Action<T> repackedCallback = null) where T : DressUpBase
    {
        var cfg = CharacterModelConfigManager.inst.GetConfig(modelId);
        if (cfg == null)
        {
            Logger.error("未找到对应模型 ： 模型id ： " + modelId);
            cfg = CharacterModelConfigManager.inst.GetAllConfig()[0];
        }

        GetCharacter(cfg.model_path, dressIds: cfg.ToDressIds(), cfg.GetGender(), scale, initRepacked, callback, repackedCallback);
    }

    /// <summary>
    /// 获取角色 [英雄]
    /// </summary>
    public void GetCharacterByHero<T>(EGender gender, List<int> equipIdList = null, List<int> dressIdList = null, float scale = characterScale, bool initRepacked = true, Action<T> callback = null, Action<T> repackedCallback = null) where T : DressUpBase
    {

        List<int> dressIds = new List<int>();
        List<int> equipToDressIds = new List<int>();
        if (equipIdList != null) equipToDressIds = EquipIdsToDressIds(equipIdList);

        dressIds.AddRange(dressIdList ?? new List<int>());
        dressIds.AddRange(equipToDressIds);

        GetCharacter(GetPeopleShapeNudeSpinePath(gender), dressIds, gender, scale, initRepacked, callback, repackedCallback);
    }


    /// <summary>
    /// 获取角色[通用]
    /// </summary>
    public void GetCharacter<T>(string spinePath, List<int> dressIds, EGender gender = EGender.Male, float scale = characterScale, bool initRepacked = true, Action<T> callback = null, Action<T> repackedCallback = null) where T : DressUpBase
    {
        CreatRuntimeAssetsAndGameObject<T>(spinePath, (system) =>
        {
            if (system is GraphicDressUpSystem)
            {
                repackedCallback += (t) =>
                {
                    t.SetSlotAlpha(gender == EGender.Male ? "m_eye_close" : "w_eye_close", 0);
                };
            }

            system.Init(gender, spinePath, dressIds, initRepacked, () => repackedCallback?.Invoke(system));
            system.SetDirection(RoleDirectionType.Left);
            system.transform.localScale = Vector3.one * scale;
            callback?.Invoke(system);
        });
    }

    /// <summary>
    /// 刷新角色[模型表]
    /// </summary>
    public void ReSetCharacterByModel(DressUpBase system, int modelId, bool initRepacked = true, Action<DressUpBase> repackedCallback = null)
    {
        var cfg = CharacterModelConfigManager.inst.GetConfig(modelId);
        if (cfg == null)
        {
            Logger.error("未找到对应模型 ： 模型id ： " + modelId);
            cfg = CharacterModelConfigManager.inst.GetAllConfig()[0];
        }

        List<int> defaultDress = cfg.ToDressIds();

        ReSetCharacter(system, cfg.model_path, defaultDress, cfg.GetGender(), initRepacked, repackedCallback);
    }

    /// <summary>
    /// 刷新角色[英雄]
    /// </summary>
    /// <param name="equipIdList">装备列表id</param>
    /// <param name="dressIdList">除装备dressId以外的列表</param>
    public void ReSetCharacterByHero(DressUpBase system, EGender gender, List<int> equipIdList = null, List<int> dressIdList = null, bool initRepacked = true, Action<DressUpBase> repackedCallback = null)
    {
        List<int> defaultDress = dressIdList;
        List<int> equipToDressIds = new List<int>();

        if (equipIdList != null) equipToDressIds = EquipIdsToDressIds(equipIdList);
        defaultDress.AddRange(equipToDressIds);

        ReSetCharacter(system, GetPeopleShapeNudeSpinePath(gender), defaultDress, gender, initRepacked, repackedCallback);
    }


    /// <summary>
    /// 刷新角色[通用]
    /// </summary>
    public void ReSetCharacter(DressUpBase system, string spinePath, List<int> dressIdList = null, EGender gender = EGender.Male, bool initRepacked = true, Action<DressUpBase> repackedCallback = null)
    {
        if (system == null)
        {
            Logger.error("传入system为空");
            return;
        }

        if (system is GraphicDressUpSystem)
        {
            repackedCallback += (t) =>
            {
                t.SetSlotAlpha(gender == EGender.Male ? "m_eye_close" : "w_eye_close", 0);
            };
        }

        system.Clear();

        GetSkeletonDataAsset(spinePath, (asset) =>
        {
            if (system.EqualsNudeAsset(asset))
            {
                system.Init(gender, spinePath, dressIdList, initRepacked, () => repackedCallback?.Invoke(system));
            }
            else
            {
                system.ReLoadSkeletonDataAssetSync(asset);
                system.Init(gender, spinePath, dressIdList, initRepacked, () => repackedCallback?.Invoke(system));
            }

            system.ReSetDirection();
        });
    }

}
