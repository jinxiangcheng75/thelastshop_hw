using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum EAIReadyToLeave
{
    none,
    success,
    fail,
}

public enum MachineShopperState
{
    none = -1,
    toInDoor,//进门前
    ramble, //闲逛
    strollToCounter,//闲逛后去柜台
    strollToLeave,//闲逛后离开
    toCounter,//去柜台
    queuing, //在排队
    readyToLeave,//准备离开
    leave,//离开

}

public enum ShopperRambleType
{
    none,

    //观赏类 
    look_1 = StaticConstants.noneWeightBase + 1,
    look_2,
    look_3,

    //有货
    have_ramble_1 = StaticConstants.haveWeightBase + 1,
    have_ramble_2,
    have_ramble_3,

    //无货
    notHave_ramble_1 = StaticConstants.notHaveWeightBase + 1,

    //宠物
    pet_ramble_1 = StaticConstants.petWeightBase + 1,
    pet_ramble_2,


}

public class TempData_ShopperPos //临时数据 顾客位置
{
    public int shopperUid;
    public Vector3Int cellPos;
}

public class Shopper_PathData
{
    public int type; // 0 家具 1 装饰 2宠物
    public int id_param;//对应id

    public Shopper_PathData(int type, int id_param)
    {
        this.type = type;
        this.id_param = id_param;
    }

}

public class Shopper : IndoorRole
{
    [HideInInspector]
    public int shopperUid;
    [HideInInspector]
    public ShopperData shopperData;
    [HideInInspector]
    public bool isBargaining = false;
    [HideInInspector]
    public EAIReadyToLeave readyLeaveType = EAIReadyToLeave.none;

    #region 闲逛流程相关参数
    [HideInInspector]
    public ShopperRambleType rambleType = ShopperRambleType.none;
    [HideInInspector]
    public int rambleTargetShelfUid = -1;//闲逛目标货架的uid 没有则为-1
    [HideInInspector]
    public string rambleTargetEquipUid = "";//闲逛时锁定的目标装备Uid
    public int rambleTargetEquipId = 0;//闲逛时锁定的目标装备id
    [HideInInspector]
    public int rambleTalkeCount = 0; //闲逛状态说话次数
    [HideInInspector]
    public int rambleTalkeCountLimit = -1; //闲逛状态说话次数上限 -1表示无限次
    [HideInInspector]
    public int rambleTalkPointByDecor = 100; //闲逛状态看装饰的说话概率
    [HideInInspector]
    public int rambleTalkPointByShelf = 100; //闲逛状态看货架的说话概率
    [HideInInspector]
    public int rambleTalkPointByPet = 100; //闲逛状态看宠物的说话概率
    [HideInInspector]
    public int moveEndTalkPoint = 0; //闲逛结束时说话概率
    [HideInInspector]
    public TalkConditionType moveEndTalkType = TalkConditionType.none; //闲逛结束时的talk类型
    [HideInInspector]
    public int rambleCanMovePathNodeNum = 0; //闲逛可到达的路径点数量

    #endregion

    [HideInInspector]
    public Vector3Int moveTargetPos; //寻路的目标地点

    StateMachine _machine;
    bool init;

    [HideInInspector]
    public bool isBubbleAlpha;

    [SerializeField]
    public GameObject iconBgObj; //图片下方阴影

    bool hidePopupCheckout = false;

    //是否在室内
    public bool isInDoor
    {
        get
        {
            var size = UserDataProxy.inst.shopData.size;
            var pos = currCellPos;
            pos.x -= StaticConstants.IndoorOffsetX;
            if (pos.x >= size.xMin && pos.x <= size.xMax && pos.y >= size.yMin && pos.y <= size.yMax)
            { return true; }
            else
            { return false; }
        }
    }

    protected override void Init()
    {
        base.Init();

        _machine = new StateMachine();

        _machine.Init(new List<IState> {
            new ShopperToInDoorState(this),
            new ShopperRambleState(this),
            new ShopperToCounterState(this),
            new ShopperQueuingState(this),
            new ShopperReadyToLeaveState(this),
            new ShopperLeaveState(this),
        });
    }

    public float autoCheckLeaveTime = 2f;
    float timer;

    private void Update()
    {
        if (!init) return;

        if (_machine != null)
        {
            _machine.OnUpdate();

            if (GetCurState() != MachineShopperState.readyToLeave && GetCurState() != MachineShopperState.leave) //如果不是离开和准备离开 就计时判断
            {
                if (shopperData.data.shopperState == 100)//离开 但没有离开
                {
                    timer += Time.deltaTime;

                    if (timer >= autoCheckLeaveTime)
                    {
                        timer = 0;
                        SetState(MachineShopperState.leave);
                    }

                }
            }
        }

    }

    public MachineShopperState GetCurState()
    {
        if (_machine != null) return (MachineShopperState)_machine.GetCurState();

        return MachineShopperState.none;
    }


    public void SetState(MachineShopperState state, StateInfo Info = null)
    {
        if (!init) return;

        if (_machine != null) _machine.SetState((int)state, Info);
    }


    [HideInInspector]
    public bool isNew;
    public void SetData(ShopperData data, bool isNew)
    {

        shopperData = data;
        this.isNew = isNew;
        shopperUid = data.data.shopperUid;

        if (data.GetEquips().Count > 0)
        {
            CharacterManager.inst.GetCharacterByHero<DressUpSystem>(data.getGender(), data.GetEquips(), SpineUtils.RoleDressToUintList(data.data.roleDress), callback: (dressUpSystem) =>
            {
                _character = dressUpSystem;

                StartCoroutine(placeWeapon());
            });
        }
        else
        {
            ArtisanNPCConfigData npcCfg = ArtisanNPCConfigManager.inst.GetConfig(data.data.shopperId);

            if (npcCfg == null)
            {
                Logger.error("给定顾客数据没有装备信息，且npc表没找到该id shopperId : " + data.data.shopperId);
            }

            int modelId = npcCfg == null ? 1 : npcCfg.model;

            CharacterManager.inst.GetCharacterByModel<DressUpSystem>(modelId, callback: (dressUpSystem) =>
            {
                _character = dressUpSystem;

                onCharacterCreated();
            });
        }

        if (IndoorMapEditSys.inst != null && (IndoorMapEditSys.inst.shopDesignMode != DesignMode.normal))
        {
            SetVisible(false);
        }

    }

    /// <summary>
    /// 初始生成时 将武器放到背后或腰间  将盾牌放在背后
    /// </summary>
    IEnumerator placeWeapon()
    {
        while (_character.isInDressing)
        {
            yield return null;
        }

        int weaponEquipType2 = shopperData.GetInitWeaponEquipType_2();

        if (weaponEquipType2 != -1)//携带了武器
        {
            EGender gender = shopperData.getGender();
            string weaponSlotName = gender == EGender.Male ? StaticConstants.man_weapon_slotName : StaticConstants.woman_weapon_slotName;
            string placeWeaponSlotName = SpineUtils.GetPlaceWeaponSlotName(gender, (EquipSubType)weaponEquipType2, out string temp);

            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(weaponEquipType2);
            if (classcfg != null)
            {
                float slotRot = gender == EGender.Male ? classcfg.m_rotation_angle : classcfg.g_rotation_angle;
                _character.AttToAnotherSlot(weaponSlotName, placeWeaponSlotName, slotRot, classcfg.GetSlotPos(gender), classcfg.GetSlotScale(gender));
            }
        }

        var cfg = shopperData.GetInitShieldEquipCfg();

        if (cfg != null)//携带了盾牌
        {
            EGender gender = shopperData.getGender();
            string shieldSlotName = gender == EGender.Male ? StaticConstants.man_shield_slotName : StaticConstants.woman_shield_slotName;
            string placeShieldSlotName = SpineUtils.GetPlaceWeaponSlotName(gender, EquipSubType.shield, out string temp);

            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID((int)EquipSubType.shield);
            if (classcfg != null)
            {
                float slotRot = gender == EGender.Male ? classcfg.m_rotation_angle : classcfg.g_rotation_angle;
                _character.AttToAnotherSlot(shieldSlotName, placeShieldSlotName, slotRot, classcfg.GetSlotPos(gender), classcfg.GetSlotScale(gender));
            }
        }


        onCharacterCreated();
    }

    void onCharacterCreated()
    {

        gameObject.name = "Shopper" + shopperUid;
        var go = _character.gameObject;
        go.transform.SetParent(_attacher.actorParent);
        go.transform.localPosition = Vector3.zero;

        SetBubbleClickHandler(onShopperClick);
        UpdateSortingOrder();
        init = true;

        SetState(isNew ? MachineShopperState.toInDoor : MachineShopperState.queuing);

        //if (!isNew) EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.CHANGESTATETOFREE);
    }

    public void OnDataChange()
    {
        if (shopperData.isAutoLeave || isNew)
        {
            if (shopperData.data.shopperState == (int)EShopperState.Queuing) //去柜台
            {
                SetState(MachineShopperState.toCounter);
            }
            else if (shopperData.data.shopperState == (int)EShopperState.Leaving) //离开
            {
                SetState(MachineShopperState.readyToLeave);
            }
        }

    }

    public bool CheckHasTypeHandler()
    {
        return shopperData.data.shopperType <= (int)EShopperType.SellMultiple;
    }


    //气泡点击事件
    void onShopperClick()
    {

        //if (IndoorMapEditSys.inst.Shopkeeper == null || !IndoorMapEditSys.inst.Shopkeeper.isCanMoveToCounter)
        //{
        //    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主到不了柜台，无法交易"), GUIHelper.GetColorByColorHex("FF2828"));
        //    return;
        //}

        if (!HotfixBridge.inst.GetShopkeeperCanMoveToCounter())
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主到不了柜台，无法交易"), GUIHelper.GetColorByColorHex("FF2828"));
            return;
        }

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver && GuideDataProxy.inst.CurInfo.m_curCfg.btn_view != "ShopperUI")
        {
            if ((K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type != K_Guide_Type.RestrictShopper)
                return;
        }

        if ((shopperData.data.shopperType == (int)EShopperType.Warrior || shopperData.data.shopperType == (int)EShopperType.HighPriceBuy) && shopperData.data.shopperComeType == (int)EShopperComeType.Normal)
        {
            if (PlayerPrefs.GetInt(AccountDataProxy.inst.account + "_shopperPop" + shopperData.data.shopperUid, -1) == -1) //手动点开后可显示
            {
                PlayerPrefs.SetInt(AccountDataProxy.inst.account + "_shopperPop" + shopperData.data.shopperUid, 1);
            }
        }


        CameraMoveConfig cfg = CameraMoveConfigManager.inst.GetConfg(kCameraMoveType.shopperDeal);
        D2DragCamera.inst.LookToPosition(transform.position + new Vector3(cfg.X_revise, cfg.Y_revise, 0), false, cfg.zoom2 / 100f * (FGUI.inst.isLandscape ? (float)StaticConstants.designSceneSize.x / (float)StaticConstants.designSceneSize.y : 1));
        EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_START_CHECKOUT, shopperUid);

        HotfixBridge.inst.TriggerLuaEvent("CheckGuideTrigger", 3, shopperData.data.shopperGuideTaskId);
    }




    //气泡透明度调整
    public void SetSpBubbleAlpha(float alpha, float duration)
    {
        _attacher.SetSpBubbleAlpha(alpha, duration);
        isBubbleAlpha = alpha != 1;
    }

    private void faceToCounter()
    {
        IndoorData.ShopDesignItem counterdata = UserDataProxy.inst.GetCounter();
        if (counterdata != null)
        {
            var countpos = MapUtils.IndoorCellposToMapCellPos(new Vector3Int(counterdata.x, counterdata.y, 0));

            if (currCellPos.y >= countpos.y)
            {
                _character.SetDirection(RoleDirectionType.Right);
            }
            else
            {
                _character.SetDirection(RoleDirectionType.Left);
            }
        }
    }

    public void ShowPopupCheckOut()
    {
        if (GetCurState() == MachineShopperState.queuing /*&& shopperData.data.shopperState == 99*/ && !isBargaining && !hidePopupCheckout)
        {
            showPopupCheckout();
        }
    }


    string getSpriteBgIconName()
    {
        string icon = "";


        //判断当前气泡的颜色 顾客来买-白色 顾客来买-黄色

        bool isShopperBuy = true;

        switch ((EShopperType)shopperData.data.shopperType)
        {
            case EShopperType.Buy:
            case EShopperType.Warrior:
            case EShopperType.HighPriceBuy:
                isShopperBuy = true;
                break;
            case EShopperType.Sell:
            case EShopperType.Worker:
            case EShopperType.SellCopyItem:
            case EShopperType.SellMultiple:
                isShopperBuy = false;
                break;
        }

        icon = isShopperBuy ? "zhuejiemian_qipao2" : "zhuejiemian_qipao1";

        return icon;
    }


    //显示气泡
    bool showPopupCheckout()
    {
        if (isTalking) HidePopup();

        string iconAtlas;
        string iconName;
        faceToCounter();

        var shopperUI = GUIManager.GetWindow<ShopperUIView>();

        if ((shopperData.data.shopperType == (int)EShopperType.Warrior || shopperData.data.shopperType == (int)EShopperType.HighPriceBuy) && shopperData.data.shopperComeType == (int)EShopperComeType.Normal)
        {

            if (PlayerPrefs.GetInt(AccountDataProxy.inst.account + "_shopperPop" + shopperData.data.shopperUid, -1) == -1) //第一次打开
            {

                iconBgObj.SetActiveFalse();

                AtlasAssetHandler.inst.GetAtlasSprite("main_atlas", getSpriteBgIconName(), (gsprite) =>
                {
                    SetSpBgIcon(gsprite.sprite);
                    gsprite.release();
                });

                AtlasAssetHandler.inst.GetAtlasSprite("main_atlas", "zhuejiemian_tishitanhao", (gsprite) =>
                {
                    Color qcol = Color.white;

                    if (shopperUI == null || !shopperUI.isShowing)
                    {
                        ShowSpPop(gsprite.sprite, 1, false, false, in qcol, false, 1.6f);
                    }
                });

                //if (shopperUI != null && shopperUI.isShowing)
                //{
                //    SetSpBubbleAlpha(0.5f, 0f);
                //}

                return true;
            }
            else
            {
                iconBgObj.SetActiveTrue();
            }

        }


        var cfg = EquipConfigManager.inst.GetEquipDrawingsCfgByEquipId(shopperData.data.targetEquipId);
        if (cfg == null)
        {
            if (shopperData.data.targetItemId > 0)
            {
                var itemcfg = ItemconfigManager.inst.GetConfig(shopperData.data.targetItemId);
                if (itemcfg == null)
                {
                    Logger.error("showPopupCheckout cfg not found targetItemId:" + shopperData.data.targetItemId);
                    return false;
                }
                iconAtlas = itemcfg.atlas;
                iconName = itemcfg.icon;
            }
            else
            {
                Shopper _shopper = IndoorRoleSystem.inst.GetShopperByUid(shopperData.data.shopperUid);
                _shopper?.SetState(MachineShopperState.leave);

                EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_REFUSE, shopperData.data.shopperUid, true);
                Logger.error("showPopupCheckout cfg not found targetEquipId:" + shopperData.data.targetEquipId + "  targetItemId:" + shopperData.data.targetItemId + "    uid:" + shopperData.data.shopperUid);
                return false;
            }
        }
        else
        {
            iconAtlas = cfg.atlas;
            iconName = cfg.icon;
        }

        AtlasAssetHandler.inst.GetAtlasSprite("main_atlas", getSpriteBgIconName(), (gsprite) =>
        {
            SetSpBgIcon(gsprite.sprite);
        });

        AtlasAssetHandler.inst.GetAtlasSprite(iconAtlas, iconName, (gsp) =>
        {
            if (this == null) return;

            var quality = 0;
            EquipQualityConfig eqcfg = EquipConfigManager.inst.GetEquipQualityConfig(shopperData.data.targetEquipId);
            if (eqcfg != null)
            {
                quality = eqcfg.quality;
            }
            string qcolor = quality > 1 ? StaticConstants.qualityColor[quality - 1] : "";
            Color qcol = GUIHelper.GetColorByColorHex(qcolor);

            if (shopperUI == null || !shopperUI.isShowing)
            {
                ShowSpPop(gsp.sprite, shopperData.data.targetCount, true/* shopperData.data.shopperType == (int)EShopperType.Warrior || shopperData.data.shopperType == (int)EShopperType.SellCopyItem || shopperData.data.shopperType == (int)EShopperType.SellMultiple*/, quality > 1, in qcol);
            }

        });

        HotfixBridge.inst.TriggerLuaEvent("CheckGuideTrigger", 7, shopperData.data.shopperGuideTaskId);

        //if (shopperUI != null && shopperUI.isShowing)
        //{
        //    SetSpBubbleAlpha(0.5f, 0f);
        //}


        return true;
    }


    public void HidePopupCheckout()
    {
        hidePopupCheckout = true;
        HidePopup();
    }


    public string[] GetTalkStrs(int conditions)
    {
        string talkStr = LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(shopperData.data.shopperId, shopperData.data.shopperLevel, conditions));
        return talkStr.Split('|');
    }


    //判断是否是来买东西的
    public bool isBuy()
    {
        return shopperData.data.shopperType == (int)EShopperType.Buy;
    }

    //移动到柜台
    public bool MoveToCounter()
    {
        var endpos = IndoorMap.inst.GetCounterFrontPos();
        Stack<PathNode> movepath = IndoorMap.inst.FindPath(currCellPos, endpos);
        if (endpos != Vector3Int.zero /*&& movepath.Count > 0*/)
        {
            IndoorMap.inst.SetGridFlags(true, endpos.x - StaticConstants.IndoorOffsetX, endpos.y - StaticConstants.IndoorOffsetY, 99999999, 99999999);
            move(movepath);
            return true;
        }
        else
        {
            //Logger.error("无法前往柜台  当前位置：" + currCellPos.ToString() + "   柜台位置：" + endpos.ToString());
            return false;
        }
    }

    //离开
    public bool MoveLeave()
    {
        var endpos = IndoorMap.inst.shopperEndPosList[UnityEngine.Random.Range(0, 2)];
        Stack<PathNode> movepath = IndoorMap.inst.FindPath(currCellPos, endpos);
        if (movepath.Count > 1)
        {
            IndoorMap.inst.SetGridFlags(true, currCellPos.x - StaticConstants.IndoorOffsetX, currCellPos.y - StaticConstants.IndoorOffsetY, 99999999, 0);
            move(movepath);
            return true;
        }
        else //门口被堵住了 
        {
            movepath = IndoorMap.inst.FindPathBarrierFree(currCellPos, endpos);

            if (movepath.Count > 1)
            {
                IndoorMap.inst.SetGridFlags(true, currCellPos.x - StaticConstants.IndoorOffsetX, currCellPos.y - StaticConstants.IndoorOffsetY, 99999999, 0);
                move(movepath);

                Attacher.SetVisible(true);
                SetTalkSpacing(5);
                Talk(LanguageManager.inst.GetValueByKey("想堵住我，没门！"));
                return true;
            }
        }

        return false;
    }

    public override void Talk(string msg, Action talkComplete = null)
    {
        if (GetCurState() == MachineShopperState.ramble && rambleTalkeCountLimit != -1 && rambleTalkeCount >= rambleTalkeCountLimit) //说话有次数上限
        {
            return;
        }

        if (GetCurState() == MachineShopperState.ramble)
        {
            rambleTalkeCount++;
        }

        base.Talk(msg, talkComplete);
    }

    #region 闲逛相关逻辑
    //移动到对应家具
    public bool MoveToFurniture(int furnitureUid)
    {
        if (furnitureUid == -1) return false;

        //var endPos = IndoorMap.inst.GetFurnitureFrontPos(furnitureUid);
        //Stack<PathNode> movepath = IndoorMap.inst.FindPath(currCellPos, endPos);

        //if ((movepath.Count > 0 && endPos != Vector3Int.zero) || endPos == currCellPos)
        //{
        //    if (IndoorMap.inst.GetFurnituresByUid(furnitureUid, out Furniture f))
        //    {
        //        moveTargetPos = f.cellpos;
        //    };

        //    move(movepath);
        //    rambleCanMovePathNodeNum++;
        //    return true;
        //}

        if (IndoorMap.inst.GetFurnituresByUid(furnitureUid, out Furniture furniture))
        {
            var posList = furniture.GetFreePos();

            var ranPosList = new List<Vector3Int>();

            foreach (var item in posList)
            {
                ranPosList.Insert(UnityEngine.Random.Range(0, ranPosList.Count + 1), item);
            }

            foreach (var endPos in ranPosList)
            {
                var movepath = IndoorMap.inst.FindPath(currCellPos, endPos);

                if (movepath.Count > 0)
                {
                    moveTargetPos = furniture.cellpos;
                    move(movepath);
                    rambleCanMovePathNodeNum++;
                    return true;
                }

            }

            Logger.log(string.Format("<color=#ff0000> Error Log: {0}</color>", "____无法前往该家具 当前位置：" + currCellPos.ToString() + "   家具uid ： " + furnitureUid + "  家具当前的位置：" + furniture.cellpos.ToString() + "家具名称： " + FurnitureConfigManager.inst.getConfig(furniture.id).name +  "  顾客uid_ :" + shopperUid));

        }

        return false;
    }


    //货架是否为空的
    bool theShelfIsNull(in IndoorData.ShopDesignItem shelf)
    {
        return shelf.equipList.Count == 0;
    }

    //货架是否包含购买目标
    bool theShelfContainsTargetEquip(in IndoorData.ShopDesignItem shelf)
    {
        if (ItemBagProxy.inst.getEquipNumber(shopperData.data.targetEquipId) > 0)
        {
            var targetItem = shelf.equipList.Find(item => item.equipId == shopperData.data.targetEquipId); //shelf.equipList.Find(item => item.equipUid == shopperData.data.targetEquipUid);
            return targetItem != null;
        }

        return false;
    }


    //货架旁边
    void nearShelf(IndoorData.ShopDesignItem furniture)
    {

        bool isTargetShelf = furniture.uid == rambleTargetShelfUid;

        if (theShelfIsNull(furniture)) //为空的
        {
            if (Helper.randomResult(rambleTalkPointByShelf))//吐槽
            {
                Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(shopperData.data.shopperId, shopperData.data.shopperLevel, (int)TalkConditionType.shelfIsNull)));
                //Logger.error("为空吐槽____________ shopperUId :" + shopperUid);
            }

        }
        else
        {
            if (theShelfContainsTargetEquip(furniture)) //是否包含购买目标
            {
                if (Helper.randomResult(rambleTalkPointByShelf))//夸奖
                {
                    Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(shopperData.data.shopperId, shopperData.data.shopperLevel, (int)TalkConditionType.optGood)));
                    //Logger.error("选中包含商品 并说出夸奖____________ shopperUId :" + shopperUid);
                }

                rambleTargetEquipUid = shopperData.data.targetEquipUid;
                rambleTargetEquipId = shopperData.data.targetEquipId;
            }
            else
            {
                if (isTargetShelf)
                {

                    //Logger.error("_________shopperid :" + shopperData.data.shopperUid + "     到达了最开始确定的目标货架 货架不包含自己要的装备，前端自行筛选,,,,");


                    if (shopperData.data.shopperType == (int)EShopperType.Buy || shopperData.data.shopperType == (int)EShopperType.Sell)
                    {
                        List<int> subTypes = HeroProfessionConfigManager.inst.GetAllFieldEquipId(shopperData.data.shopperId);

                        List<ShelfEquip> canWearShelfEquipList = furniture.equipList.FindAll(item =>
                        {
                            EquipItem equip = ItemBagProxy.inst.GetEquipItem(item.equipId); //item.equipUid

                            if (equip != null)
                            {
                                return subTypes.Contains(equip.equipConfig.equipDrawingsConfig.sub_type);
                            }

                            return false;
                        });

                        if (canWearShelfEquipList.Count > 0)
                        {
                            var targetEquip = canWearShelfEquipList.GetRandomElement();
                            rambleTargetEquipUid = targetEquip.equipUid;
                            EquipItem equip = ItemBagProxy.inst.GetEquipItem(targetEquip.equipId);
                            rambleTargetEquipId = equip.equipid;

                            //Logger.error("_________shopperid :" + shopperData.data.shopperUid + "     前端自行筛选完毕,,,, 此货架包含该顾客可穿戴的装备  最终想要的装备uid： " + rambleTargetEquipUid);

                            if (Helper.randomResult(rambleTalkPointByShelf))//夸奖
                            {
                                Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(shopperData.data.shopperId, shopperData.data.shopperLevel, (int)TalkConditionType.optGood)));
                               //Logger.error("选中包含商品 并说出夸奖____________ shopperUId :" + shopperUid);
                            }

                        }
                        else
                        {
                            if (Helper.randomResult(rambleTalkPointByShelf))//吐槽
                            {
                                Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(shopperData.data.shopperId, shopperData.data.shopperLevel, (int)TalkConditionType.notOptGood)));
                                //Logger.error("（该货架剩余装备均无法穿戴）没有选中吐槽____________ shopperUId :" + shopperUid);
                            }
                        }
                    }
                }
                else
                {
                    if (Helper.randomResult(rambleTalkPointByShelf))//吐槽
                    {
                        Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(shopperData.data.shopperId, shopperData.data.shopperLevel, (int)TalkConditionType.notOptGood)));
                        //Logger.error("没有选中吐槽____________ shopperUId :" + shopperUid);
                    }
                }
            }
        }
    }

    //装饰旁边
    void nearOrnamental(IndoorData.ShopDesignItem furniture)
    {

        if (Helper.randomResult(rambleTalkPointByDecor))
        {
            Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(shopperData.data.shopperId, shopperData.data.shopperLevel, (int)TalkConditionType.lookSomething)));
        }

        //加能量
        if (shopperData.data.energyTimes > 0)
        {
            if (IndoorMap.inst.GetFurnituresByUid(furniture.uid, out Furniture furnitureEntity))
            {
                furnitureEntity.UpdateUpgradeAnim(true); //装饰不会影响到货架

                furnitureEntity.transform.DOScaleY(1.1f, 0.2f).From(1).OnComplete(() =>
                {
                    furnitureEntity.transform.DOScaleY(1, 0.2f).From(1.1f).OnComplete(() =>
                    {
                        furnitureEntity.UpdateUpgradeAnim(false);//直接关闭
                    });
                });

                EventController.inst.TriggerEvent<int, int, int>(GameEventType.ShopperEvent.SHOPPER_LookOrnamental, shopperUid, furniture.uid, 0);
            }
        }
    }

    //移动到路径点时的具体表现
    public void MoveToPathNodeBehavior(Shopper_PathData pathData)
    {
        if (pathData.type == 0) //家具
        {
            moveToFurnitureBehavior(pathData.id_param);
        }
        else if (pathData.type == 1) //装饰
        {
            moveToFurnitureBehavior(pathData.id_param);
        }
        else if (pathData.type == 2) //宠物
        {
            moveToPetBehavior(pathData.id_param);
        }
    }


    //移动到家具时的具体表现
    void moveToFurnitureBehavior(int furnitureUid)
    {

        //判断当前查询的家具
        IndoorData.ShopDesignItem furniture = UserDataProxy.inst.GetFuriture(furnitureUid);
        if (furniture != null)
        {

            if (furniture.type == (int)kTileGroupType.Shelf)//货架
            {
                nearShelf(furniture);
            }
            else if (furniture.type == (int)kTileGroupType.Furniture) //装饰
            {
                nearOrnamental(furniture);
            }
        }
    }

    //移动到宠物时的具体表现
    void moveToPetBehavior(int petId)
    {
        if (IndoorRoleSystem.inst != null)
        {
            Pet pet = IndoorRoleSystem.inst.GetPetByUid(petId);

            if (pet != null)
            {
                nearPet(pet);
            }
        }
    }

    RoleDirectionType getLookDir(Vector3 from, Vector3 to)
    {
        Vector3 span = to - from;


        if (span.y >= 0)
        {
            return RoleDirectionType.Left;
        }
        else
        {
            return span.x > 0 ? RoleDirectionType.Left : RoleDirectionType.Right;
        }

    }

    /// <param name="furnitureData">路径节点信息</param>
    /// <param name="stepCallback">到达对应家具的回调（返回家具uid）</param>
    /// <param name="successCallback">播放完动作的回调</param>
    /// <param name="failCallback">失败的回调</param>
    /// <returns></returns>
    IEnumerator toFurniture(Shopper_PathData furnitureData, Action<Shopper_PathData> stepCallback = null, Action successCallback = null, Action failCallback = null)
    {
        string lookAnimName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)Character.gender, furnitureData.type == 0 ? (int)kIndoorRoleActionType.ornamental_furniture : (int)kIndoorRoleActionType.ornamental_shelves);

        stopMove();

        if (MoveToFurniture(furnitureData.id_param))
        {
            yield return null;
            yield return null;


            while (isMoving)
            {
                yield return null;
            }

            //朝向家具
            _character.SetDirection(getLookDir(currCellPos, moveTargetPos));

            _character.Play(lookAnimName, completeDele: t =>
            {
                if (this == null) return;

                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
                _character.Play(idleAnimationName, true);

                float waitTime = WorldParConfigManager.inst.GetConfig(1105).parameters;

                GameTimer.inst.AddTimer(waitTime, 1, () =>
                {
                    if (this == null) return;
                    stepCallback?.Invoke(furnitureData);
                    successCallback?.Invoke();
                });
            });
        }
        else
        {
            failCallback?.Invoke();
        }
    }

    public bool MoveToPet(int petUid)
    {
        if (IndoorRoleSystem.inst == null) return false;

        Pet pet = IndoorRoleSystem.inst.GetPetByUid(petUid);

        if (pet == null) return false;


        var endPos = pet.GetFreePos();
        Stack<PathNode> movepath = IndoorMap.inst.FindPath(currCellPos, endPos);

        if ((movepath.Count > 0 && endPos != Vector3Int.zero) || endPos == currCellPos)
        {
            moveTargetPos = pet.currCellPos;
            move(movepath);
            rambleCanMovePathNodeNum++;
            return true;
        }

        Logger.log(string.Format("<color=#ff0000> Error Log: {0}</color>", "____无法前往该宠物 当前位置：" + currCellPos.ToString() + "   宠物Id ： " + petUid + "  宠物位置：" + endPos.ToString() + " 顾客uid_ :" + shopperUid));

        return false;

    }

    void nearPet(Pet pet)
    {
        if (Helper.randomResult(rambleTalkPointByPet))
        {
            Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(shopperData.data.shopperId, shopperData.data.shopperLevel, (int)TalkConditionType.lookPet)));
        }

        //Logger.error("观赏完宠物了~~~");
        EventController.inst.TriggerEvent<int, int, int>(GameEventType.ShopperEvent.SHOPPER_LookOrnamental, shopperUid, 0, pet.petUid);
        pet.StayBySeeCount--;
    }

    /// <param name="petData">路径数据</param>
    /// <param name="stepCallback">到达对应宠物的回调（返回宠物Ud）</param>
    /// <param name="successCallback">播放完动作的回调</param>
    /// <param name="failCallback">失败的回调</param>
    /// <returns></returns>
    IEnumerator toPet(Shopper_PathData petData, Action<Shopper_PathData> stepCallback = null, Action successCallback = null, Action failCallback = null)
    {
        string lookAnimName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)Character.gender, (int)kIndoorRoleActionType.ornamental_pets);

        stopMove();

        if (MoveToPet(petData.id_param))
        {
            //让宠物进入待观测状态
            Pet pet = IndoorRoleSystem.inst.GetPetByUid(petData.id_param);
            pet.StayBySeeCount++;

            yield return null;
            yield return null;


            while (isMoving)
            {
                yield return null;
            }

            //朝向宠物
            _character.SetDirection(getLookDir(currCellPos, moveTargetPos));

            _character.Play(lookAnimName, completeDele: t =>
            {
                if (this == null) return;
                stepCallback?.Invoke(petData);

                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
                _character.Play(idleAnimationName, true);

                float waitTime = WorldParConfigManager.inst.GetConfig(1105).parameters;
                GameTimer.inst.AddTimer(waitTime, 1, () =>
                {
                    if (this == null) return;
                    successCallback?.Invoke();
                });

                //_character.Play(lookAnimName, completeDele: d =>
                //{
                //    if (this == null) return;
                //    successCallback?.Invoke();
                //});
            });
        }
        else
        {
            failCallback?.Invoke();
        }
    }





    //按既定路线出发(若无法移动到当前目标，直接前往下一个目标)
    public void EstablishedLine(List<Shopper_PathData> pathNodes, int index = 0, Action<Shopper_PathData> stepCallback = null, Action endCallback = null)
    {

        if (index >= pathNodes.Count)
        {
            endCallback?.Invoke();
            return;
        }

        Shopper_PathData pathData = pathNodes[index];

        Logger.log("_________shopperUid: " + shopperData.data.shopperUid + "      既定路线行进中  下标 : " + index + "   路线数量 : " + pathNodes.Count + "   目标类型 " + (pathData.type == 0 || pathData.type == 1 ? "家具" : "宠物") + "   要前往的目标uid： " + pathNodes[index].id_param);

        if (pathData.type == 0 || pathData.type == 1) //家具
        {
            StartCoroutine(toFurniture(pathNodes[index], stepCallback, () => EstablishedLine(pathNodes, index + 1, stepCallback, endCallback), () => EstablishedLine(pathNodes, index + 1, stepCallback, endCallback)));
        }
        else if (pathData.type == 2) //宠物 
        {
            StartCoroutine(toPet(pathNodes[index], stepCallback, () => EstablishedLine(pathNodes, index + 1, stepCallback, endCallback), () => EstablishedLine(pathNodes, index + 1, stepCallback, endCallback)));
        }

    }

    #endregion


    #region 顾客买卖时的表现

    //讨价还价
    public void Bargaining()
    {
        if (GetCurState() == MachineShopperState.queuing /*&& shopperData.data.shopperState == 99*/)
        {
            if (_character != null)
            {
                string bargainingAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.haggle);
                _character.Play(bargainingAnimationName, true);
            }
            isBargaining = true;
        }
    }

    //停止讨价还价
    public void StopBargaining()
    {
        isBargaining = false;

        if (GetCurState() == MachineShopperState.queuing /*&& shopperData.data.shopperState == 99*/)
        {
            if (_character != null)
            {
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)_character.gender, (int)kIndoorRoleActionType.normal_standby);
                _character.Play(idleAnimationName, true);
            }
        }
    }

    public void ChangeEquip(int equipId)//买到装备
    {
        readyLeaveType = EAIReadyToLeave.success;
        SetState(MachineShopperState.readyToLeave, new ReadyToLeaveStateInfo() { equipId = equipId });
    }

    public void SellItem() //卖出物品
    {
        readyLeaveType = EAIReadyToLeave.success;
        SetState(MachineShopperState.readyToLeave);
    }

    public void ByRefuseBehavior()//被拒绝
    {
        readyLeaveType = EAIReadyToLeave.fail;
        SetState(MachineShopperState.readyToLeave);
    }

    #endregion

    bool checkCurPosIsObstacle(out TempData_ShopperPos shopperPosData)
    {
        if (SaveManager.inst.GetGameValue<TempData_ShopperPos>(shopperData.data.shopperUid + "_shopperPos", out shopperPosData)) //说明本地有缓存
        {

            int gridFlag = IndoorMap.inst.GetIndoorGridFlags(shopperPosData.cellPos.x - StaticConstants.IndoorOffsetX, shopperPosData.cellPos.y - StaticConstants.IndoorOffsetY);

            if (gridFlag == 0)
            {
                //设置一下Flag
                IndoorMap.inst.SetGridFlags(true, shopperPosData.cellPos.x - StaticConstants.IndoorOffsetX, shopperPosData.cellPos.y - StaticConstants.IndoorOffsetY, 99999999, 99999999);
                return false;
            }
            else if (gridFlag == 99999999)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        shopperPosData = null;
        return false;
    }

    bool checkCurPosIsObstacle()
    {
        int gridFlag = IndoorMap.inst.GetIndoorGridFlags(currCellPos.x - StaticConstants.IndoorOffsetX, currCellPos.y - StaticConstants.IndoorOffsetY);

        if (gridFlag == 0)
        {
            //设置一下Flag
            IndoorMap.inst.SetGridFlags(true, currCellPos.x - StaticConstants.IndoorOffsetX, currCellPos.y - StaticConstants.IndoorOffsetY, 99999999, 99999999);
            return false;
        }
        else if (gridFlag == 99999999)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void RefreshCurCellPos(bool justNowOnCounter)
    {

        if (justNowOnCounter)
        {
            if (checkCurPosIsObstacle())
            {
                Vector3Int newPos = IndoorMap.inst._FindFreeCell(MapUtils.MapCellPosToIndoorCellpos(currCellPos), 1, 1, true);
                SetCellPos(MapUtils.IndoorCellposToMapCellPos(newPos));
                UpdateSortingOrder();
                IndoorMap.inst.SetGridFlags(true, newPos.x, newPos.y, 99999999, 99999999);
            }

            SavePosCache();
        }
        else
        {
            if (checkCurPosIsObstacle(out TempData_ShopperPos shopperPosData))
            {
                Vector3Int newPos = IndoorMap.inst._FindFreeCell(MapUtils.MapCellPosToIndoorCellpos(shopperPosData.cellPos), 1, 1, true);
                SetCellPos(MapUtils.IndoorCellposToMapCellPos(newPos));
                UpdateSortingOrder();
                IndoorMap.inst.SetGridFlags(true, newPos.x, newPos.y, 99999999, 99999999);
            }

            SavePosCache();
        }

        faceToCounter();
    }

    public void SavePosCache()
    {
        SaveManager.inst.SaveGameValue(shopperUid + "_shopperPos", new TempData_ShopperPos() { shopperUid = shopperUid, cellPos = currCellPos }); //缓存位置到本地
    }

    public void ClearPosCache()
    {
        //清除本地的 顾客位置缓存
        if (PlayerPrefs.GetString(AccountDataProxy.inst.account + "_" + shopperUid + "_shopperPos", "-1") != "-1") //说明本地有缓存
        {
            PlayerPrefs.DeleteKey(AccountDataProxy.inst.account + "_" + shopperUid + "_shopperPos");
        }
    }

    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }

    public void Clear() 
    {
        if (GetCurState() == MachineShopperState.toCounter)
        {
            ClearPosCache();
        }
        else
        {
            SavePosCache();
        }
        DestroySelf();
    }

}
