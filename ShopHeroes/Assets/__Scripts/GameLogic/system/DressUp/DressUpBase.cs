using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoleDirectionType
{
    Positive, // 前
    Negative, // 后
    Left, // 左
    Right, // 右
    max,
}

//换装父级
[RequireComponent(typeof(IAnimationStateComponent), typeof(ISkeletonComponent))]
public class DressUpBase : MonoBehaviour
{
    protected IAnimationStateComponent _IAnimationstateComp;
    protected ISkeletonComponent _ISkeletonComp;

    protected bool isInit, initRepacked;
    protected TrackEntry curTrackEntry; //当前动画信息

    //身上穿的
    protected Dictionary<int, int> _facadeDressDic; //外观
    protected Dictionary<int, int> _fashionOrEquipDressDic; //时装or装备

    protected List<int> _defaultDressList;

    protected string nudeBodyAssetPath;

    public EGender gender;//性别 
    public RoleDress curDress;//角色装扮
    public RoleDirectionType direction = RoleDirectionType.max;//朝向

    //合批shader路径
    protected virtual string repackedShaderPath
    {
        get { return ""; }
    }

    private bool isError;

    public virtual SkeletonDataAsset SkeletonDataAsset
    {
        get { return _ISkeletonComp.SkeletonDataAsset; }
        set { }
    }

    public virtual Skeleton Skeleton
    {
        get { return _ISkeletonComp.Skeleton; }
        set { }
    }

    public virtual float skeletonAlpha
    {
        get;
        set;
    }

    public virtual Color skeletonColor
    {
        get;
        set;
    }

    public bool EqualsNudeAsset(SkeletonDataAsset asset)
    {
        return _ISkeletonComp.SkeletonDataAsset == null ? false : _ISkeletonComp.SkeletonDataAsset == asset;
    }

    #region inDressing
    /// <summary>
    /// 是否处于换装过程中
    /// </summary>
    public bool isInDressing
    {
        get;
        protected set;
    }

    protected List<int> overallDresingIds;
    protected int inDressingCount;
    protected Action pubOverAllClothingEndHandler;//提供给外面整体换装完毕的回调
    protected Action overallClothingEndHandle;//整体换装完毕后的回调

    private int _iterationID = -202020;

    private void CheckInDressing()
    {
        if (isInDressing)
        {
            inDressingCount++;

            if (inDressingCount == overallDresingIds.Count)
            {
                if (initRepacked)
                {
                    initRepacked = false;
                    Repacked();

                    GameTimer.inst.AddTimerFrame(1, 1, () =>
                    {
                        if (this != null)
                        {
                            pubOverAllClothingEndHandler?.Invoke();
                            gameObject.SetActive(true);
                        }
                    });
                }
                else
                {
                    pubOverAllClothingEndHandler?.Invoke();
                    overallClothingEndHandle?.Invoke();
                }

                isInDressing = false;
            }
        }
    }
    #endregion

    protected virtual void Awake()
    {
        _IAnimationstateComp = GetComponent<IAnimationStateComponent>();
        _ISkeletonComp = GetComponent<ISkeletonComponent>();
        _facadeDressDic = new Dictionary<int, int>();
        _fashionOrEquipDressDic = new Dictionary<int, int>();
        _defaultDressList = new List<int>();
        curDress = new RoleDress();

        if (_IAnimationstateComp == null || _ISkeletonComp == null)
        {
            Logger.error("换装系统初始化有问题~~~~");
            isError = true;
            isInit = false;
        }
    }

    public virtual void Init(EGender gender, string nudeBodyAssetPath, List<int> defaultDress = null, bool initRepacked = true, Action repackedCallback = null)
    {
        if (isError) return;

        if (runtimeMaterial != null) Destroy(runtimeMaterial);
        if (runtimeAtlas != null) Destroy(runtimeAtlas);

        this.gender = gender;
        this.nudeBodyAssetPath = nudeBodyAssetPath;
        isInit = true;
        _iterationID++;

        AnimationSpeed = 1;

        //初始装扮不为null 
        if (defaultDress != null && defaultDress.Count != 0)
        {
            _defaultDressList = new List<int>(defaultDress);

            this.initRepacked = initRepacked;
            OverallClothing(_defaultDressList, callback: repackedCallback);

            if (initRepacked) gameObject.SetActive(false);
        }
        else
        {
            repackedCallback?.Invoke();
            isInDressing = false;
        }
    }

    protected bool CheckSex()
    {
        return gender == EGender.Male;
    }

    //刷新当前方向
    public void ReSetDirection()
    {
        SetDirection(direction);
        continueCurAni();
    }

    void continueCurAni()
    {
        if (curTrackEntry != null)
        {
            Play(curTrackEntry.Animation.Name, curTrackEntry.Loop, curTrackEntry.TimeScale, curTrackEntry.TrackTime, curTrackEntry.IsComplete ? null : curTrackEntry.GetCurCompleteHanler());
        }
    }


    /// <summary>
    /// 改变方向
    /// </summary>
    /// <param name="direct">方向</param>
    public void SetDirection(RoleDirectionType direct)
    {
        if (!isInit)
        {
            Logger.error("尚未初始化，无法转换方向" + "  gameObject.name : " + gameObject.name);
            return;
        }

        direction = direct;

        //spine模型偏移
        switch (direct)
        {
            case RoleDirectionType.Positive:
            case RoleDirectionType.Negative:
                break;
            case RoleDirectionType.Left:
                Skeleton.ScaleX = 1;
                break;
            case RoleDirectionType.Right:
                Skeleton.ScaleX = -1;
                break;
        }
    }

    #region 换装

    //集体换装前
    protected void onOverallClothingStart()
    {
        isInDressing = true;
        inDressingCount = 0;
        overallClothingEndHandle = null;
    }

    /// <summary>
    /// 整体换装
    /// </summary>
    /// <param name="ids">换装表对应ID</param>
    public void OverallClothing(List<int> ids, Action callback = null)
    {
        onOverallClothingStart();
        pubOverAllClothingEndHandler = callback;
        overallDresingIds = ids;

        for (int i = ids.Count - 1; i >= 0; i--)
        {
            dressconfig cfg = dressconfigManager.inst.GetConfig(ids[i]);
            if (cfg == null)
            {
                Logger.error("~~~~~~没有对应的dressId   dressid = " + ids[i]);
                _defaultDressList.Remove(ids[i]);
                continue;
            }

            SwitchClothingByCfg(cfg);

        }
    }

    /// <summary>
    /// 根据装备Id换装
    /// </summary>
    /// <param name="equipId">对应equip表ID</param>
    public void SwitchClothingByEquipId(int equipId)
    {
        var equipCfg = EquipConfigManager.inst.GetEquipQualityConfig(equipId);

        if (equipCfg != null)
        {
            int dressId = equipCfg.dressId;
            dressconfig dressconfig = dressconfigManager.inst.GetConfig(dressId);

            SwitchClothingByCfg(dressconfig);
        }
        else
        {
            Logger.error("~~~~~~~~~换装未找到对应装备 equipId = " + equipId);
        }
    }

    /// <summary>
    /// 单个部位换装
    /// </summary>
    /// <param name="cfg"></param>
    public void SwitchClothingByCfg(dressconfig cfg)
    {
        if (cfg == null) return;

        string assetPath = cfg.atlas;

        if (string.IsNullOrEmpty(assetPath))
        {
            string slotName = CheckSex() ? cfg.slot_man : cfg.slot_woman;

            if (!string.IsNullOrEmpty(cfg.val))//皮肤、发型、胡子、妆颜、眼睛  颜色
            {
                if (isInDressing && !initRepacked)
                    overallClothingEndHandle += () => changeClotheColor(slotName, cfg.val);
                else
                    changeClotheColor(slotName, cfg.val);
            }
            else if (cfg.name.StartsWith("无"))//摘除
            {

            }

            CheckDress(cfg);
            CheckInDressing();
        }
        else
        {
            //_equipDressDic[cfg.type_2] = cfg.id;
            if (!needChangeDress(cfg))
            {
                CheckInDressing();
                return;
            }

            switchClothingByAssetPath(assetPath, cfg.type_1, _iterationID, () => { CheckDress(cfg); });
        }
    }

    //摘除单个槽位的附件
    public void TakeOutSlotAttachment(string slotName)
    {
        Slot slot = getSlotByName(slotName);

        if (slot != null)
        {
            slot.Attachment = null;
        }
        else
        {
#if UNITY_EDITOR
            Logger.error("[摘除]没有找到对应槽位 :" + slotName);
#endif
        }
    }

    //摘除对应asset包含的所有槽位
    protected void TakeOutClothingByAsset(SkeletonDataAsset dataAsset)
    {
        SkeletonData skeletonData = dataAsset.GetSkeletonData(true);

        foreach (var item in skeletonData.Slots)
        {
            TakeOutSlotAttachment(item.Name);
        }
    }

    protected void TakeOutClothingByDeserializeAsset(DeserializeDataAsset deserializeDataAsset)
    {
        foreach (var item in deserializeDataAsset.desSlots)
        {
            TakeOutSlotAttachment(item.name);
        }
    }

    /// <summary>
    /// 根据asset地址摘除
    /// </summary>
    /// <param name="assetPath">asset地址</param>
    /// <param name="callBack">换装后的回调</param>
    public void TakeOutClothingByAssetPath(string assetPath, int type_1)
    {
        if (type_1 == 3)
        {
            string sex = gender == EGender.Male ? "man" : "woman";
            assetPath += sex;
        }

        DeserializeDataAsset deserializeDataAsset = CharacterManager.inst.GetDeserializeDataAsset(assetPath);

        if (deserializeDataAsset == null)
        {
            CharacterManager.inst.GetSkeletonDataAsset(assetPath, (asset) =>
            {
                if (this == null) return;

                TakeOutClothingByAsset(asset);
            });
        }
        else
        {
            TakeOutClothingByDeserializeAsset(deserializeDataAsset);
        }
    }

    protected void changeSlotAttachment(Slot slot, Attachment attachment)
    {
        if (isInDressing && !initRepacked) //正在换装且不是初始创建
        {
            overallClothingEndHandle += () =>
            {
                slot.Attachment = attachment;
            };
        }
        else
        {
            slot.Attachment = attachment;
        }
    }

    //替换对应asset包含的所有槽位
    protected void SwitchClothingByAsset(SkeletonDataAsset dataAsset/*, bool needCopyAttFromSelf*/)
    {
        SkeletonData skeletonData = dataAsset.GetSkeletonData(true);

        //string skinName = "";
        //Skin skin = skeletonData.FindSkin(skinName);
        Skin skin = skeletonData.DefaultSkin; //默认皮肤

        if (skin == null)
        {
            Logger.error("未找到该皮肤" + skeletonData.DefaultSkin);
            return;
        }


        foreach (var item in skeletonData.Slots)
        {
            Slot slot = getSlotByName(item.Name);

            Attachment attachment = skin.GetAttachment(item.Index, item.AttachmentName.IfNullThenEmpty());

            if (attachment == null)
            {
#if UNITY_EDITOR
                Logger.error("[替换]该槽位上未找到对应附件   _资源名称：  " + dataAsset.name + "  _槽位名称：  " + item.Name + "  _附件名称：  " + item.AttachmentName.IfNullThenEmpty() + "  _性别：" + gender.ToString());
#endif
                return;
            }

            if (slot != null)
            {
                changeSlotAttachment(slot, attachment);
            }
            else
            {
#if UNITY_EDITOR
                Logger.error("[替换]裸模没有找到对应槽位 : " + item.Name + "   _资源名称:" + dataAsset.name + "  _性别：" + gender.ToString());
#endif
            }
        }

    }

    /// <summary>
    /// 根据asset地址替换
    /// </summary>
    /// <param name="assetPath">asset地址</param>
    protected void switchClothingByAssetPath(string assetPath, int type_1, int iterationID, Action callBack = null)
    {

        if (type_1 == 3)
        {
            string sex = gender == EGender.Male ? "man" : "woman";
            assetPath += sex;
        }

        string path = assetPath;

        CharacterManager.inst.GetSkeletonDataAsset(path, (asset) =>
        {
            if (this == null) return;
            if (_iterationID != iterationID) return; //版本不一致 不做更换

            callBack?.Invoke();
            SwitchClothingByAsset(asset);
            CheckInDressing();
        });
    }

    //换色
    protected void changeClotheColor(string slotName, string color)
    {
        if (string.IsNullOrEmpty(slotName)) return;


        Color tempColor;
        string[] slotNames = slotName.Split('/');

        for (int i = 0; i < slotNames.Length; i++)
        {
            var slot = getSlotByName(slotNames[i]);

            if (slot != null)
            {
                if (ColorUtility.TryParseHtmlString(color, out tempColor)) slot.SetColor(tempColor);
            }
            else
            {
#if UNITY_EDITOR
                Logger.error("[换色]裸模没有找到对应槽位 : " + slotName);
#endif
            }
        }
    }

    protected Slot getSlotByName(string slotName)
    {
        if (Skeleton == null)
        {
            return null;
        }

        Slot slot = Skeleton.FindSlot(slotName);

        //if (slot == null)
        //{
        //    Logger.error("未找到对应槽位：  槽位名称： " + slotName);
        //}

        return slot;
    }

    bool needChangeDress(dressconfig cfg)
    {
        Dictionary<int, int> _tempDressDic = null;

        switch (cfg.type_1)
        {
            case 1:
                _tempDressDic = _facadeDressDic;
                break;
            case 2:
            case 3:
                _tempDressDic = _fashionOrEquipDressDic;
                break;
        }

        if (!_tempDressDic.ContainsKey(cfg.type_2))
        {
            return true;
        }
        else
        {
            dressconfig clearCfg = dressconfigManager.inst.GetConfig(_tempDressDic[cfg.type_2]);

            if (!string.IsNullOrEmpty(clearCfg.atlas) && clearCfg.id == cfg.id)
            {
                return false;
            }

        }

        return true;
    }

    public void CheckDress(dressconfig cfg)
    {
        #region 清理上一次同类型的装扮
        Dictionary<int, int> _tempDressDic = null;

        switch (cfg.type_1)
        {
            case 1:
                _tempDressDic = _facadeDressDic;
                break;
            case 2:
            case 3:
                _tempDressDic = _fashionOrEquipDressDic;
                break;
        }


        if (!_tempDressDic.ContainsKey(cfg.type_2))
        {
            _tempDressDic.Add(cfg.type_2, cfg.id);
        }
        else
        {
            //清除上一个装备的槽位
            dressconfig clearCfg = dressconfigManager.inst.GetConfig(_tempDressDic[cfg.type_2]);

            if (!string.IsNullOrEmpty(clearCfg.atlas) && clearCfg.id != cfg.id)
            {
                unLoadSkeletonDataAsset(clearCfg.id);

                if (isInDressing && !initRepacked)
                    overallClothingEndHandle += () =>
                    TakeOutClothingByAssetPath(clearCfg.atlas, clearCfg.type_1);
                else
                    TakeOutClothingByAssetPath(clearCfg.atlas, clearCfg.type_1);
            }

            _tempDressDic[cfg.type_2] = cfg.id;
        }

        #endregion

        if (cfg.type_2 == (int)FacadeType.ModelColor) curDress.modelColor = cfg.id;
        else if (cfg.type_2 == (int)FacadeType.Hair) curDress.hair = cfg.id;
        else if (cfg.type_2 == (int)FacadeType.HairColor) curDress.hairColor = cfg.id;
        else if (cfg.type_2 == (int)FacadeType.Face) curDress.face = cfg.id;
        else if (cfg.type_2 == (int)FacadeType.FaceColor) curDress.faceColor = cfg.id;
        else if (cfg.type_2 == (int)FacadeType.EyesColor) curDress.eyesColor = cfg.id;

        //时装
        else if (cfg.type_2 == (int)FashionType.Clothe) curDress.upper = cfg.id;
        else if (cfg.type_2 == (int)FashionType.Pants) curDress.lower = cfg.id;
        else if (cfg.type_2 == (int)FashionType.HeadHat) curDress.headHat = cfg.id;
        else if (cfg.type_2 == (int)FashionType.Shoes) curDress.shoes = cfg.id;
        else if (cfg.type_2 == (int)FashionType.Weapon) curDress.weapon = cfg.id;
    }

    #endregion

    #region 槽位更改(位置、大小、旋转、附件、透明度)
    /// <summary>
    /// 设置槽位透明度
    /// </summary>
    /// <param name="slotName">槽位名称</param>
    /// <param name="slotAlpha">槽位透明度</param>
    public void SetSlotAlpha(string slotName, float slotAlpha)
    {
        Slot slot = getSlotByName(slotName);

        if (slot != null)
        {
            Color color = slot.GetColor();
            color.a = slotAlpha;
            slot.SetColor(color);
        }
        else
        {
            if (slotName != (gender == EGender.Male ? "m_eye_close" : "w_eye_close"))
            {
                Logger.error("[设置槽位透明度]裸模没有找到对应槽位 : " + slotName);
            }
        }
    }

    /// <summary>
    /// 设置槽位旋转
    /// </summary>
    /// <param name="slotName">槽位名称</param>
    /// <param name="slotRot">槽位旋转角度</param>
    public void SetSlotRot(string slotName, float slotRot)
    {
        Slot slot = getSlotByName(slotName);

        if (slot != null)
        {
            slot.Bone.Rotation = slotRot;
        }
        else
        {
            Logger.error("[设置槽位旋转]裸模没有找到对应槽位 : " + slotName);
        }
    }

    /// <summary>
    /// 设置槽位位置
    /// </summary>
    /// <param name="slotName">槽位名称</param>
    /// <param name="pos">位置</param>
    public void SetSlotPos(string slotName, Vector2 pos)
    {
        Slot slot = getSlotByName(slotName);

        if (slot != null)
        {
            //Logger.error("该槽位之前的槽位位置为 ：" + slot.Bone.GetLocalPosition().ToString());
            slot.Bone.SetLocalPosition(pos);
        }
        else
        {
            Logger.error("[设置槽位位置]裸模没有找到对应槽位 : " + slotName);
        }
    }

    /// <summary>
    /// 设置槽位大小
    /// </summary>
    public void SetSlotScale(string slotName, Vector2 scale)
    {
        Slot slot = getSlotByName(slotName);

        if (slot != null)
        {
            //Logger.error("该槽位之前的槽位大小为 ： " + slot.Bone.ScaleX + "," + slot.Bone.ScaleY);
            slot.Bone.ScaleX = scale.x;
            slot.Bone.ScaleY = scale.y;
        }
        else
        {
            Logger.error("[设置槽位大小]裸模没有找到对应槽位 : " + slotName);
        }
    }

    /// <summary>
    /// 槽位过渡（附件转移）
    /// </summary>
    /// <param name="slot1Name">起始</param>
    /// <param name="slot2Name">终点</param>
    public void AttToAnotherSlot(string slot1Name, string slot2Name)
    {
        Slot slot1 = getSlotByName(slot1Name);
        Slot slot2 = getSlotByName(slot2Name);

        if (slot1 != null && slot2 != null)
        {
            var att = slot1.Attachment;
            slot1.Attachment = null;
            slot2.Attachment = att;
        }
        else
        {
            if (slot1 == null) Logger.error("[槽位过渡]裸模没有找到对应槽位1 : " + slot1Name);
            if (slot2 == null) Logger.error("[槽位过渡]裸模没有找到对应槽位2 : " + slot2Name);
        }

    }

    /// <summary>
    /// 槽位过渡（附件转移）
    /// </summary>
    /// <param name="slot1Name">起始</param>
    /// <param name="slot2Name">终点</param>
    /// <param name="slot2Rot">终点旋转</param>

    public void AttToAnotherSlot(string slot1Name, string slot2Name, float slot2Rot, Vector2 pos, Vector2 scale)
    {
        SetSlotRot(slot2Name, slot2Rot);
        SetSlotPos(slot2Name, pos);
        SetSlotScale(slot2Name, scale);
        AttToAnotherSlot(slot1Name, slot2Name);
    }
    #endregion

    #region 动画相关

    //获取动画名称
    protected string getAniFullName(string aniName)
    {
        return aniName;
    }

    /// <summary>
    /// 整体动画播放速率
    /// </summary>
    public float AnimationSpeed
    {
        get { return _IAnimationstateComp.AnimationState.TimeScale; }
        set { SetAnimationSpeed(value); }
    }

    /// <summary>
    /// 设置整体动画播放速率
    /// </summary>
    public void SetAnimationSpeed(float speed)
    {
        _IAnimationstateComp.AnimationState.TimeScale = speed;
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="aniName">动画名</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="speed">该动画播放速度(以整体动画播放速率为基础)</param>
    /// <param name="trackTime">动画播放的开始时间</param>
    /// <param name="completeDele">回调</param>
    public void Play(string aniName, bool isLoop = false, float speed = 1, float trackTime = 0, Spine.AnimationState.TrackEntryDelegate completeDele = null)
    {
        if (!isInit) return;

        if (this == null) return; //被销毁

        string animationName = getAniFullName(aniName);

        //同一动画且循环一致
        TrackEntry trackEntry = _IAnimationstateComp.AnimationState.GetCurrent(0);
        if (trackEntry != null && animationName == trackEntry.ToString() && trackEntry.Loop == isLoop)
        {
            completeDele?.Invoke(curTrackEntry);
            return;
        }

        if (Skeleton.Data.FindAnimation(animationName) == null)//未找到该动画
        {
#if UNITY_EDITOR
            Logger.error("(若是同一system模型变化 忽略) 该动画名称不存在    gameObject.name : " + gameObject.name + "   animationName : " + animationName);
#endif
            completeDele?.Invoke(curTrackEntry);
            return;
        }

        //var lastEntry =
        _IAnimationstateComp.AnimationState.SetEmptyAnimation(0, _IAnimationstateComp.AnimationState.Data.DefaultMix); //清空动画

        curTrackEntry = _IAnimationstateComp.AnimationState.SetAnimation(0, animationName, isLoop);
        curTrackEntry.TimeScale = speed;
        curTrackEntry.TrackTime = trackTime;

        curTrackEntry.Complete += completeDele;
    }

    public float GetAnimTimeByName(string animName)
    {
        var animation = Skeleton.Data.FindAnimation(animName);

        return animation == null ? 0f : animation.Duration;
    }

    public string GetRandomAnimName()
    {
        return Skeleton.Data.Animations.GetRandomElement().Name;
    }
    #endregion


    #region 合批相关
    Material runtimeMaterial;
    Texture2D runtimeAtlas;

    public void Repacked(string shaderPath)
    {
        repacked(shaderPath);
    }

    //合批
    public void Repacked()
    {
        repacked(repackedShaderPath);

        ////1. 搞个空皮肤，加进去现在身上的所有附件 [差别：需要读取默认导入的spine文件皮肤信息(且是理想的换装效果) 美术要求更繁琐 弃]
        //Skin curSkin = new Skin("curSkin");

        ////curSkin.AddSkin(_nudeAsset.GetSkeletonData(true).DefaultSkin);

        //foreach (var slot in _skeletonAnimation.skeleton.Slots)
        //{
        //    if (slot.Attachment != null)
        //    {
        //        curSkin.SetAttachment(slot.Data.Index, slot.Data.Name, slot.Attachment);
        //    }
        //}

        ////2. 搞个材质
        //var newMaterial = _skeletonRenderer.skeletonDataAsset.atlasAssets[0].PrimaryMaterial;

        ////3. 生成合批后的皮肤
        //Skin repackedSkin = curSkin.GetRepackedSkin("repacked skin", newMaterial, out runtimeMaterial, out runtimeAtlas);

        ////4. 设置为合批皮肤
        //_skeletonRenderer.skeleton.SetSkin(repackedSkin);
        //_skeletonRenderer.skeleton.UpdateCache();
        //_skeletonRenderer.skeleton.SetSlotsToSetupPose();
    }

    protected virtual void repacked(string shaderPath)
    {
        Dictionary<string, string> keys = new Dictionary<string, string>();
        List<Attachment> sourceAttachments = new List<Attachment>();
        List<Attachment> outputAttachments = new List<Attachment>();

        if (runtimeMaterial != null) Destroy(runtimeMaterial);
        if (runtimeAtlas != null) Destroy(runtimeAtlas);

        //1. 将要换的所有附件提取
        foreach (var slot in Skeleton.Slots)
        {
            if (slot.Attachment != null)
            {
                sourceAttachments.Add(slot.Attachment);
                if (!keys.ContainsKey(slot.Attachment.Name)) keys.Add(slot.Attachment.Name, slot.Data.Name);//通过附件名字与实例槽位挂钩   remarks: ^^美术资源符合此要求  同一槽位的不同导出文件的附件名称一定要保证不一致^^
                else Logger.error("[合批]重复的附件名称 ：" + slot.Attachment.Name);
            }
        }

        //2. 所有附件合并到一张图
        Shader shader = Shader.Find(shaderPath);
        var newMaterial = shader == null ? null : new Material(shader);
        AtlasUtilities.GetRepackedAttachments(sourceAttachments, outputAttachments, newMaterial, out runtimeMaterial, out runtimeAtlas);

        //3. 替换附件(通过附件名字作为key找到实例slot)
        for (int i = 0; i < outputAttachments.Count; i++)
        {
            Attachment newAtt = outputAttachments[i];
            Slot slot = getSlotByName(keys[newAtt.Name]);
            if (slot != null)
            {
                slot.Attachment = newAtt;
            }
            else
            {
                Logger.error("[合批]未通过附件名称找到对应slot   attName :" + newAtt.Name);
            }
        }

        Logger.log("~~~~~~合批结束，参与合批的附件数量： " + sourceAttachments.Count);
    }


    #endregion

    public virtual void Initialize(bool overwrite)
    {

    }

    public virtual void Clear()
    {
        isInit = false;
        isInDressing = true;

        unLoadSkeletonDataAssets();

        _defaultDressList.Clear();
        _facadeDressDic.Clear();
        _fashionOrEquipDressDic.Clear();
        curDress = new RoleDress();

        if (Skeleton != null) Skeleton.SetToSetupPose();

        Initialize(false);
    }

    protected void unLoadSkeletonDataAsset(int dressId)
    {
        dressconfig clearCfg = dressconfigManager.inst.GetConfig(dressId);

        if (!string.IsNullOrEmpty(clearCfg.atlas))
        {
            CharacterManager.inst.UnLoadSkeletonDataAsset(clearCfg.atlas);
        }
    }

    protected void unLoadSkeletonDataAssets()
    {
        foreach (var dressId in _facadeDressDic.Values)
        {
            unLoadSkeletonDataAsset(dressId);
        }

        foreach (var dressId in _fashionOrEquipDressDic.Values)
        {
            unLoadSkeletonDataAsset(dressId);
        }

        if (!string.IsNullOrEmpty(nudeBodyAssetPath)) CharacterManager.inst.UnLoadSkeletonDataAsset(nudeBodyAssetPath);
    }

    private void OnDestroy()
    {
        unLoadSkeletonDataAssets();

        if (runtimeMaterial != null) Destroy(runtimeMaterial);
        if (runtimeAtlas != null) Destroy(runtimeAtlas);
    }

    #region 测试
#if UNITY_EDITOR
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetDirection(RoleDirectionType.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetDirection(RoleDirectionType.Right);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Repacked();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, "我是一个测试语句", GUIHelper.GetColorByColorHex("FF2828"));
            //HotfixBridge.inst.TriggerLuaEvent("AddRaceLampTip",1,10006, "小松鼠",10);

            string animations = "";

            foreach (var item in Skeleton.Data.Animations)
            {
                animations += "\n" + item.Name;
            }

            Logger.log("一休一休一休   " + gameObject.name + "      animations:" + animations);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            skeletonAlpha = 0;
        }

    }
#endif
    #endregion

}
