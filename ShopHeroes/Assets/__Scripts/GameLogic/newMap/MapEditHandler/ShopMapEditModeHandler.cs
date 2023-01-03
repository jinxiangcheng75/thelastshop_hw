using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class IndoorMapEditSys
{
    public bool isDesigning
    {
        get { return shopDesignMode != DesignMode.normal && shopDesignMode != DesignMode.LookPetHouse; }
    }
    public DesignMode shopDesignMode = DesignMode.normal;
    private void AddListeners_EditMode()
    {
        // EventController.inst.AddListener(GameEventType.ShopDesignEvent.Enter_EditMode, EnterDesigning);
        // EventController.inst.AddListener(GameEventType.ShopDesignEvent.Exit_EditMode, ExitDesigning);
        EventController.inst.AddListener(GameEventType.ExtensionEvent.EXTENSION_CALLTAO_SHOPUPGRADE, onShopUpdate);
        EventController.inst.AddListener(GameEventType.ExtensionEvent.EXTENSION_SHOPREFRESH, shopUpgradeRefresh);
        EventController.inst.AddListener<DesignMode, int>(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignModeChange);
        EventController.inst.AddListener<IndoorData.ShopDesignItem>(GameEventType.ShopDesignEvent.LOOKPETHOUSE, lookPetHouse);
        EventController.inst.AddListener(GameEventType.ShopDesignEvent.EXITPETHOUSE, exitLookPetHouse);

        EventController.inst.AddListener(GameEventType.ShopDesignEvent.Set_DesignChangedFlag, setDesignChangedFlag);

    }

    private void RemoveListeners_EditMode()
    {
        // EventController.inst.RemoveListener(GameEventType.ShopDesignEvent.Enter_EditMode, EnterDesigning);
        // EventController.inst.RemoveListener(GameEventType.ShopDesignEvent.Exit_EditMode, ExitDesigning);
        EventController.inst.RemoveListener(GameEventType.ExtensionEvent.EXTENSION_CALLTAO_SHOPUPGRADE, onShopUpdate);
        EventController.inst.RemoveListener(GameEventType.ExtensionEvent.EXTENSION_SHOPREFRESH, shopUpgradeRefresh);
        EventController.inst.RemoveListener<DesignMode, int>(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignModeChange);
        EventController.inst.RemoveListener<IndoorData.ShopDesignItem>(GameEventType.ShopDesignEvent.LOOKPETHOUSE, lookPetHouse);
        EventController.inst.RemoveListener(GameEventType.ShopDesignEvent.EXITPETHOUSE, exitLookPetHouse);

        EventController.inst.RemoveListener(GameEventType.ShopDesignEvent.Set_DesignChangedFlag, setDesignChangedFlag);

    }


    public void EnterDesigning()
    {
        return;
        //显示网格
        IndoorMap.inst.MapGridLineVisible(true);
        //隐藏所有Role
        HideRole();
        //显示设计界面
    }
    public void ExitDesigning()
    {
        IndoorMap.inst.MapGridLineVisible(false);
        ShowRole();
        OnFurnituresRelease();
        //  EventController.inst.TriggerEvent(GameEventType.HIDEUI_SHOPDESIGNUI);
    }
    public void shopUpgradeRefresh()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_ShopRefresh() { }
        });
    }
    public void shopUpgradeFinish()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Design_ShopFinish() { }
        });
    }

    bool isRoleVisible = true;
    void HideRole()
    {
        if (!isRoleVisible) return;
        isRoleVisible = false;

        //if (shopKeeper != null) shopKeeper.SetVisible(false);
        HotfixBridge.inst.TriggerLuaEvent("SETVISIBLE", false);

        if (IndoorRoleSystem.inst != null)
        {
            IndoorRoleSystem.inst.SetRolesVisible(false);
        }

    }

    void ShowRole()
    {
        if (isRoleVisible) return;
        isRoleVisible = true;

        //if (shopKeeper != null) shopKeeper.SetVisible(true);
        HotfixBridge.inst.TriggerLuaEvent("SETVISIBLE", true);

        if (IndoorRoleSystem.inst != null)
        {
            IndoorRoleSystem.inst.SetRolesVisible(true);

        }
    }

    void HideFurniture()
    {
        IndoorMap.inst.SetFurnituresVisible(false);
    }
    void ShowFurniture()
    {
        IndoorMap.inst.SetFurnituresVisible(true);
    }

    void HidePethouseFeedTips() 
    {
        IndoorMap.inst.HidePethouseFeedTips();
    }

    void onShopUpdate()
    {
        UserDataProxy.inst.requestFloor();
        UserDataProxy.inst.requestWall();
        Logger.log("店铺扩建刷新：");
        IndoorMap.inst.ShopUpdataRefreshFloor();
        if (IndoorEnvironmentObjVisibleClr.inst != null)
        {
            IndoorEnvironmentObjVisibleClr.inst.UpdateObjVisible();
        }
    }


    bool Flag_designChanged;//室内格局变化，顾客位置刷新监测

    void setDesignChangedFlag()
    {
        if (shopDesignMode == DesignMode.normal) return;
        Flag_designChanged = true;
    }

    void DesignModeChange(DesignMode mode, int parameter)
    {
        // if (shopDesignMode == DesignMode.normal && shopDesignMode == mode) return;
        // if (shopDesignMode == mode) return;
        switch (shopDesignMode)
        {
            case DesignMode.FloorEdit:  //场景地板编辑
                {
                    clearFloorEditMode();
                }
                break;
            case DesignMode.WallEdit:
                {
                    ExitWallEditMode();
                }
                break;
        }
        shopDesignMode = mode;

        switch (mode)
        {
            case DesignMode.normal:
                {
                    //隐藏网格
                    IndoorMap.inst.MapGridLineVisible(false);

                    //取消当前选择
                    OnFurnituresRelease();
                    //关闭当前设计界面
                    //DesignViewRefresh();
                    HideDesignView();

                    //判断角色是否需要刷新
                    if (Flag_designChanged)
                    {
                        Flag_designChanged = false;
                        if (IndoorRoleSystem.inst != null) IndoorRoleSystem.inst.AllMovedShopperRefreshPos();
                    }

                    //显示角色
                    ShowRole();
                    //
                    ShowFurniture();
                }
                break;
            case DesignMode.modeSelection://编辑类型 选择
                {
                    //显示网格
                    IndoorMap.inst.MapGridLineVisible(true);
                    //隐藏宠物小家头顶信息
                    HidePethouseFeedTips();
                    //隐藏所有Role
                    HideRole();
                    //取消当前选择
                    OnFurnituresRelease();
                    //显示设计界面
                    DesignViewRefresh();

                }
                break;
            case DesignMode.FurnitureEdit://场景家具编辑
                {
                    //显示网格
                    IndoorMap.inst.MapGridLineVisible(true);
                    //隐藏宠物小家头顶信息
                    HidePethouseFeedTips();
                    //隐藏所有Role
                    HideRole();
                    //显示设计界面
                    DesignViewRefresh();
                }
                break;
            case DesignMode.FloorEdit:  //场景地板编辑
                {
                    var size = UserDataProxy.inst.GetIndoorSize();
                    size.x += StaticConstants.IndoorOffsetX;
                    EnterFloorEditMode(parameter, new RectInt(size.xMin, size.yMin, 2, 2));
                }
                break;
            case DesignMode.WallEdit:   //场景墙纸编辑
                {
                    EnterWallEditMode(parameter);
                }
                break;
        }
        if (shopDesignMode == DesignMode.normal)
        {
            //打开主线任务
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETTIMERRESET, true);
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REFRESHTASKDATA, false);
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_GuideTask");
        }
        else
        {
            //关闭主线任务
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.HIDEMAINLINEUI);
            HotfixBridge.inst.TriggerLuaEvent("HideUI_GuideTask");
        }
        EventController.inst.TriggerEvent(GameEventType.SHOW_FurnitureNumLimit, shopDesignMode != DesignMode.normal);
    }


    DressUpSystem petHouse_shopkeeper = null;
    DressUpSystem petHouse_pet = null;

    private RoleDirectionType getDir(Vector3 from, Vector3 to)
    {
        Vector3 span = to - from;

        if (span.y > 0)
        {
            return RoleDirectionType.Left;
        }
        else if (span.y < 0)
        {
            return RoleDirectionType.Right;
        }
        else
        {
            //Logger.error("观赏宠物小家 店主与宠物位置的y一致" + span.ToString());
            return span.x > 0 ? RoleDirectionType.Right : RoleDirectionType.Left;
        }

    }



    void getPetHouseCharacterPos(Furniture furniture, out Vector3Int shopkeeperPos, out Vector3Int petPos)
    {
        var freePoses = furniture.GetFreePos();

        shopkeeperPos = Vector3Int.zero;
        petPos = Vector3Int.zero;


        //原方案
        //if (freePoses.Count >= 2)
        //{
        //    petHouse_shopkeeperPos = freePoses[0];
        //    petHouse_petPos = freePoses[1];
        //}
        //else
        //{
        //    Logger.error("这里的问题？？？？");
        //}

        //店主仍然为此逻辑
        if (freePoses.Count > 0)
        {
            shopkeeperPos = freePoses[0];
        }
        //宠物位置直接为小家位置
        petPos = MapUtils.IndoorCellposToMapCellPos(furniture.cellpos);

    }

    public void ShowPethouseCharacter(Furniture furniture, bool shopkeeperNeedChg = true)
    {

        var petData = PetDataProxy.inst.GetPetDataByFurnitureUid(furniture.uid);

        if (PetDataProxy.inst.GetPethouseNeedFeedTips(petData.petUid))
        {
            PetDataProxy.inst.SetPethouseNeedFeedTipsFlag(petData.petUid, false);
        }

        getPetHouseCharacterPos(furniture, out Vector3Int petHouse_shopkeeperPos, out Vector3Int petHouse_petPos);

        RoleDress roleDress = UserDataProxy.inst.playerData.userDress;
        EGender gender = (EGender)UserDataProxy.inst.playerData.gender;


        if (petHouse_shopkeeper == null)
        {
            CharacterManager.inst.GetCharacter<DressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath(gender), SpineUtils.RoleDressToUintList(roleDress), gender, callback: (system) =>
            {
                petHouse_shopkeeper = system;
                petHouse_shopkeeper.gameObject.name = "petHouse_shopkeeper";
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)petHouse_shopkeeper.gender, (int)kIndoorRoleActionType.normal_standby);
                petHouse_shopkeeper.Play(idleAnimationName, true);


                Vector3 pos = MapUtils.CellPosToCenterPos(petHouse_shopkeeperPos);
                petHouse_shopkeeper.transform.position = pos;
                petHouse_shopkeeper.SetSortingAndOrderLayer("map_Actor", MapUtils.GetTileMapOrder(petHouse_shopkeeper.transform.position.y - 0.2f, petHouse_shopkeeper.transform.position.x, 1, 1));
                petHouse_shopkeeper.SetDirection(getDir(petHouse_shopkeeperPos, petHouse_petPos));
            });
        }
        else
        {
            if (shopkeeperNeedChg)
            {
                CharacterManager.inst.ReSetCharacter(petHouse_shopkeeper, CharacterManager.inst.GetPeopleShapeNudeSpinePath(gender), SpineUtils.RoleDressToUintList(roleDress), gender, repackedCallback: (system) =>
                {
                    petHouse_shopkeeper.SetActive(true);
                    Vector3 pos = MapUtils.CellPosToCenterPos(petHouse_shopkeeperPos);
                    petHouse_shopkeeper.transform.position = pos;
                    petHouse_shopkeeper.SetSortingAndOrderLayer("map_Actor", MapUtils.GetTileMapOrder(petHouse_shopkeeper.transform.position.y - 0.2f, petHouse_shopkeeper.transform.position.x, 1, 1));
                    petHouse_shopkeeper.SetDirection(getDir(petHouse_shopkeeperPos, petHouse_petPos));
                });
            }
        }

        if (petHouse_pet == null)
        {
            CharacterManager.inst.GetCharacterByModel<DressUpSystem>(petData.petCfg.model, callback: (system) =>
            {
                petHouse_pet = system;
                petHouse_pet.gameObject.name = "petHouse_pet";
                petHouse_pet.Play("rest", completeDele: (a) =>
                {
                    petHouse_pet.Play("idle", true);
                });


                Vector3 pos = MapUtils.CellPosToCenterPos(petHouse_petPos);
                petHouse_pet.transform.position = pos;
                petHouse_pet.SetSortingAndOrderLayer("map_Actor", MapUtils.GetTileMapOrder(petHouse_pet.transform.position.y - 0.2f, petHouse_pet.transform.position.x, 1, 1));
                petHouse_pet.SetDirection(getDir(petHouse_petPos, petHouse_shopkeeperPos));
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByModel(petHouse_pet, petData.petCfg.model, repackedCallback: (system) =>
            {
                petHouse_pet.SetActive(true);
                Vector3 pos = MapUtils.CellPosToCenterPos(petHouse_petPos);
                petHouse_pet.transform.position = pos;
                petHouse_pet.SetSortingAndOrderLayer("map_Actor", MapUtils.GetTileMapOrder(petHouse_pet.transform.position.y - 0.2f, petHouse_pet.transform.position.x, 1, 1));
                petHouse_pet.SetDirection(getDir(petHouse_petPos, petHouse_shopkeeperPos));
                petHouse_pet.Play("rest", completeDele: (a) =>
                {
                    petHouse_pet.Play("idle", true);
                });
            });
        }

    }

    void hidePethouseCharacter()
    {
        if (petHouse_shopkeeper != null)
        {
            petHouse_shopkeeper.SetActive(false);
        }

        if (petHouse_pet != null)
        {
            petHouse_pet.SetActive(false);
        }

    }

    void lookPetHouse(IndoorData.ShopDesignItem designItem)
    {
        shopDesignMode = DesignMode.LookPetHouse;
        HotfixBridge.inst.TriggerLuaEvent("HideUI_GuideTask");
        IndoorMap.inst.MapGridLineVisible(false);
        OnFurnituresRelease();
        HideRole();
        HideFurniture();

        //只保留这个家具 临时创建店主和宠物 其他都隐藏
        if (IndoorMap.inst.GetFurnituresByUid(designItem.uid, out Furniture furniture))
        {
            furniture.gameObject.SetActive(true);
            //移动镜头看过来
            D2DragCamera.inst.LookToPosition(furniture.transform.position, GUIManager.GetViewIsShowingByViewID("PetHouseDesignUI"), D2DragCamera.inst.minZoom);

            //lua 打开界面
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_PetHouseDesignUI", PetDataProxy.inst.GetPetDataByFurnitureUid(designItem.uid));

            //显示临时店主和宠物
            ShowPethouseCharacter(furniture);
        }

    }

    void exitLookPetHouse()
    {
        shopDesignMode = DesignMode.normal;
        HotfixBridge.inst.TriggerLuaEvent("HideUI_GuideTask");
        ShowRole();
        ShowFurniture();

        HideDesignView();

        //lua 关闭界面
        HotfixBridge.inst.TriggerLuaEvent("HideUI_PetHouseDesignUI");

        D2DragCamera.inst.RestorePositionAndOrthgraphicSize();

        //隐藏临时店主和宠物
        hidePethouseCharacter();

        //显示主线任务
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.SETTIMERRESET, true);
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REFRESHTASKDATA, false);
    }

}
