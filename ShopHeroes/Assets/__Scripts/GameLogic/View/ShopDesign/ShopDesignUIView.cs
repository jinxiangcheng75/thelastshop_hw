using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using Mosframe;
using com.poptiger.events;
using System;

public class ShopDesignUIView : ViewBase<ShopDesignUIComp>
{
    public override string viewID => ViewPrefabName.ShopDesignUI;
    public override int showType => (int)ViewShowType.normal;
    public override string sortingLayerName => "window";
    enum kPickTypeOp
    {
        PickTrunk,//cancel, skin, upgrade, storage, edit,
        PickBin,//cancel, upgrade, resource, edit
        PickShelf,//cancel, skins, upgrade, content, edit
        PickCounter,//cancel, skin, upgrade, edit
        PickDecor,//cancel, edit,
        PickPetHouse,//cancel, edit, puppy
        PickCarpet,//cancel, rotate, store, confirm
        Num,
    }
    enum kEditTypeOp
    {
        EditUpgradable,//cancel, rotate, store, upgrade, confirm
        EditDecor,//cancel, rotate, store, confirm
        Num,
    }
    enum kCreateTypeOp
    {
        CreateFurniture,//cancel, rotate, confirm_price
        CreateFloor,//back, applyall, apply
        CreateWall,//back, applyall, apply
        None,
    }

    //Button[] mEditBtns;
    Button[] mOpBtns;
    Button[] mCreateBtns;
    Button[][] mPickOpBtnList;
    Button[][] mEditOpBtnList;
    Button[][] mCreateOpBtnList;
    kPickTypeOp mPickMode;
    kEditTypeOp mEditMode;
    kCreateTypeOp mCreateMode;
    kDesignMode mDesignMode;

    IndoorData.ShopDesignItem mSelectItem;

    //animater
    Tween hideOpBtnsTween;
    Tween hideItemTitleTween;

    protected override void onInit()
    {
        isShowResPanel = true;
        windowAnimTime = 0.4f;
        //edit
        contentPane.btn_furniture.ButtonClickTween(() =>
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver)
            {
                var guideCfg = GuideDataProxy.inst.CurInfo.m_curCfg;
                if (((K_Guide_Type)guideCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)guideCfg.guide_type == K_Guide_Type.TipsAndRestrictClick) && (guideCfg.btn_name == "0" || guideCfg.btn_name == "btn_furniture"))
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TARGETFURN, Int16.Parse(guideCfg.conditon_param_2), -1);
                }
                else
                {
                    dispatchEvent(GameEventType.SHOWUI_FURNITUREUI);
                }
            }
            else
            {
                dispatchEvent(GameEventType.SHOWUI_FURNITUREUI);
            }
            //dispatchEvent(GameEventType.SHOWUI_FURNITUREUI);
        });
        contentPane.btn_customize.ButtonClickTween(() =>
        {
            dispatchEvent(GameEventType.SHOWUI_CUSTOMIZEUI);
        });
        contentPane.btn_edit_pets.ButtonClickTween(() =>
        {
            if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(170).parameters)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主达到{0}级可解锁，可以通过售卖装备升级", WorldParConfigManager.inst.GetConfig(170).parameters.ToString()), GUIHelper.GetColorByColorHex("FFD907"));
                return;
            }
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_MainPetUI");
        });

        contentPane.btn_done.ButtonClickTween(btn_doneOnClick);
        contentPane.btn_expand.ButtonClickTween(btn_expandOnClick);
        //select
        contentPane.btn_edit.ButtonClickTween(btn_editOnClick);
        contentPane.btn_op_pets.ButtonClickTween(btn_op_petsOnClick);
        contentPane.btn_storage.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_BAGUI);
        });
        contentPane.btn_resource.ButtonClickTween(btn_resourceOnClick);
        //op
        contentPane.btn_cancel.ButtonClickTween(btn_cancelOnClick);
        contentPane.btn_skin.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_SKINUI);
        });
        contentPane.btn_rotate.ButtonClickTween(() =>
          {
              AudioManager.inst.PlaySound(22);
              EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.ROTATE_ITEM);
          }
        );

        contentPane.create_btn_rotate.ButtonClickTween(() => EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.ROTATE_ITEM));

        contentPane.btn_store.ButtonClickTween(btn_storeOnClick);
        contentPane.btn_content.ButtonClickTween(btn_contentOnClick);
        contentPane.btn_upgrade.ButtonClickTween(btn_upgradeOnClick);
        contentPane.btn_confirm.ButtonClickTween(btn_confirmOnClick);
        contentPane.btn_back.ButtonClickTween(btn_backOnClick);
        contentPane.btn_apply.ButtonClickTween(btn_applyOnClick);
        contentPane.btn_applyAll.ButtonClickTween(btn_applyAllOnClick);
        //contentPane.emptyMask.onClick.AddListener(btn_emptyOnClick);
        mOpBtns = contentPane.trans_op.GetComponentsInChildren<Button>(true);
        mCreateBtns = contentPane.create_op.GetComponentsInChildren<Button>(true);
        initModeList();
    }

    void initModeList()
    {
        //pick
        var ml = new Button[(int)kPickTypeOp.Num][];
        ml[(int)kPickTypeOp.PickTrunk] = new Button[] { contentPane.btn_cancel, contentPane.btn_upgrade, contentPane.btn_storage, contentPane.btn_edit };
        ml[(int)kPickTypeOp.PickBin] = new Button[] { contentPane.btn_cancel, contentPane.btn_upgrade, contentPane.btn_resource, contentPane.btn_edit };
        ml[(int)kPickTypeOp.PickShelf] = new Button[] { contentPane.btn_cancel, contentPane.btn_upgrade, contentPane.btn_content, contentPane.btn_edit };
        ml[(int)kPickTypeOp.PickCounter] = new Button[] { contentPane.btn_cancel, contentPane.btn_upgrade, contentPane.btn_edit };
        ml[(int)kPickTypeOp.PickDecor] = new Button[] { contentPane.btn_cancel, contentPane.btn_edit };
        ml[(int)kPickTypeOp.PickPetHouse] = new Button[] { contentPane.btn_cancel, contentPane.btn_edit, contentPane.btn_op_pets };
        ml[(int)kPickTypeOp.PickCarpet] = new Button[] { contentPane.btn_cancel, contentPane.btn_rotate, contentPane.btn_store, contentPane.btn_confirm };
        mPickOpBtnList = ml;

        //edit
        ml = new Button[(int)kEditTypeOp.Num][];
        ml[(int)kEditTypeOp.EditUpgradable] = new Button[] { contentPane.btn_cancel, contentPane.btn_rotate, contentPane.btn_store, contentPane.btn_upgrade, contentPane.btn_confirm };
        ml[(int)kEditTypeOp.EditDecor] = new Button[] { contentPane.btn_cancel, contentPane.btn_rotate, contentPane.btn_store, contentPane.btn_confirm };
        mEditOpBtnList = ml;

        //create
        ml = new Button[(int)kCreateTypeOp.None][];
        ml[(int)kCreateTypeOp.CreateFurniture] = new Button[] { contentPane.btn_cancel, contentPane.btn_rotate, contentPane.btn_confirm };
        ml[(int)kCreateTypeOp.CreateFloor] = new Button[] { contentPane.btn_back, contentPane.btn_applyAll, contentPane.btn_apply };
        ml[(int)kCreateTypeOp.CreateWall] = new Button[] { contentPane.btn_back, contentPane.create_btn_rotate, contentPane.btn_applyAll, contentPane.btn_apply };
        mCreateOpBtnList = ml;
        mCreateMode = kCreateTypeOp.None;
    }


    void addListeners()
    {
        var e = EventController.inst;
        e.AddListener<FurnitureDisplayData>(GameEventType.ShopDesignEvent.Create_Furniture, onCreateFurniture);
        e.AddListener<int>(GameEventType.ShopDesignEvent.Create_Customize, onCreateCustomize);
        e.AddListener(GameEventType.ShopDesignEvent.Create_Failed_Space, onCreateFailedSpace);
        e.AddListener<bool>(GameEventType.ShopDesignEvent.Furniture_CANAPPLY, CanApply);
    }

    void removeListeners()
    {
        var e = EventController.inst;
        e.RemoveListener<FurnitureDisplayData>(GameEventType.ShopDesignEvent.Create_Furniture, onCreateFurniture);
        e.RemoveListener<int>(GameEventType.ShopDesignEvent.Create_Customize, onCreateCustomize);
        e.RemoveListener(GameEventType.ShopDesignEvent.Create_Failed_Space, onCreateFailedSpace);
        e.RemoveListener<bool>(GameEventType.ShopDesignEvent.Furniture_CANAPPLY, CanApply);
    }

    void onCreateFurniture(FurnitureDisplayData uiDisplayData)
    {
        IndoorData.ShopDesignItem data = UserDataProxy.inst.GetFuriture(uiDisplayData.uid);
        mDesignMode = kDesignMode.Create;
        var cfg = uiDisplayData.cfg;
        contentPane.txt_itemTitle.text = LanguageManager.inst.GetValueByKey(cfg.name.IfNullThenEmpty());
        mCreateMode = kCreateTypeOp.CreateFurniture;
        animateHideEditButtons();
        int idx = (int)mCreateMode;
        showOpButtons(idx, mCreateOpBtnList[idx], true);
        contentPane.txt_itemLevel.text = data == null ? "1" : "" + data.level;
        if (data != null)
        {
            mSelectItem = data;
            contentPane.img_confirmType.gameObject.SetActiveFalse();
            contentPane.txt_confirm.gameObject.SetActiveFalse();
            contentPane.txt_affirm.gameObject.SetActiveTrue();
        }
        else
        {
            contentPane.txt_affirm.gameObject.SetActiveFalse();
            contentPane.img_confirmType.SetSprite("__common_1", cfg.cost_type == 1 ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai");
            contentPane.img_confirmType.gameObject.SetActiveTrue();
            contentPane.txt_confirm.gameObject.SetActiveTrue();
            contentPane.txt_confirm.text = uiDisplayData.costNum.ToString("N0");
            var guideInfo = GuideDataProxy.inst.CurInfo;
            if (!guideInfo.isAllOver)
            {
                if ((K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
                {
                    if (UserDataProxy.inst.playerData.designFreeCount > 0)
                    {
                        if (cfg.id == int.Parse(guideInfo.m_curCfg.conditon_param_1))
                        {
                            contentPane.txt_confirm.text = LanguageManager.inst.GetValueByKey("免费");
                        }
                    }
                }
            }

            confirmbtnMeet = buyCheck(cfg.cost_type, (int)uiDisplayData.costNum, (int)uiDisplayData.costNum, cfg.unlock_lv);
            if (GuideManager.inst.isInTriggerGuide)
            {
                var triggerData = HotfixBridge.inst.GetTriggerData();

                if (triggerData.triggerType == 3 && cfg.id == triggerData.triggerCondition)
                {
                    contentPane.txt_confirm.text = LanguageManager.inst.GetValueByKey("免费");
                    confirmbtnMeet = true;
                }
            }
            if (!confirmbtnMeet)
            {
                GUIHelper.SetUIGray(contentPane.btn_confirm.transform, true);
                contentPane.btn_confirm.interactable = false;
            }
        }

        setBuffDes(cfg);

    }

    void onCreateCustomize(int id)
    {
        mDesignMode = kDesignMode.Create;
        var cfg = FurnitureConfigManager.inst.getConfig(id);
        if (cfg.type_1 == (int)kTileGroupType.Floor)
        {
            mCreateMode = kCreateTypeOp.CreateFloor;
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.FloorEdit, id);
        }
        else
        {
            mCreateMode = kCreateTypeOp.CreateWall;
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.WallEdit, id);
        }
        animateHideEditButtons();
        int idx = (int)mCreateMode;
        showOpButtons(idx, mCreateOpBtnList[idx], true);
    }

    void onCreateFailedSpace()
    {
        EventController.inst.TriggerEvent<string>(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("没有足够的空间"));
    }

    protected override void onShown()
    {
        addListeners();
        Refresh();
        RefreshUIUnlock();
    }

    protected override void onHide()
    {
        contentPane.trans_op.gameObject.SetActiveFalse();
        mDesignMode = kDesignMode.None;
        mCreateMode = kCreateTypeOp.None;
        removeListeners();
    }

    protected override void DoHideAnimation()
    {
        animateHideEditButtons();
        animateHideOpButtons(HideView);
    }

    bool confirmbtnMeet = true;

    void CanApply(bool can)
    {
        GUIHelper.SetUIGray(contentPane.btn_apply.transform, !can);
        contentPane.btn_apply.interactable = can;
        if (confirmbtnMeet)
        {
            GUIHelper.SetUIGray(contentPane.btn_confirm.transform, !can);
            contentPane.btn_confirm.interactable = can;
        }
        else
        {
            GUIHelper.SetUIGray(contentPane.btn_confirm.transform, true);
            contentPane.btn_confirm.interactable = false;
        }
    }
    void animateShowEditButtons(float delay = 0.05f)
    {
        if (contentPane.trans_edit.gameObject.activeSelf) return;
        if (!GameSettingManager.inst.needShowUIAnim)
        {
            contentPane.trans_edit.gameObject.SetActiveTrue();
            contentPane.trans_edit.anchoredPosition = new Vector2(0, 0);
            return;
        }
        contentPane.trans_edit.gameObject.SetActiveTrue();
        contentPane.trans_edit.DOAnchorPos3DY(0, 0);

        contentPane.btn_done.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack).SetDelay(delay == 0 ? 0 : 0.1f);
        float leftMovePosX = FGUI.inst.isLandscape ? 120 : -410;
        float rightMovePosX = FGUI.inst.isLandscape ? -120 : 344;
        (contentPane.btn_edit_pets.transform as RectTransform).DOAnchorPos3D(new Vector3(leftMovePosX, 506), 0.4f).From(new Vector3(-1000, -200)).SetEase(Ease.OutBack).SetDelay(delay);
        (contentPane.btn_furniture.transform as RectTransform).DOAnchorPos3D(new Vector3(leftMovePosX, 330), 0.4f).From(new Vector3(-1000, -200)).SetEase(Ease.OutBack).SetDelay(delay);
        (contentPane.btn_customize.transform as RectTransform).DOAnchorPos3D(new Vector3(leftMovePosX, 154), 0.4f).From(new Vector3(-1000, -200)).SetEase(Ease.OutBack).SetDelay(delay);
        (contentPane.btn_expand.transform as RectTransform).DOAnchorPos3D(new Vector3(rightMovePosX, 142), 0.4f).From(new Vector3(1000, -200)).SetEase(Ease.OutBack).SetDelay(delay);


        //DOTween.Kill(contentPane.trans_edit, true);
        //contentPane.trans_edit.DOAnchorPos3DY(0, 0.25f).From(-500f).SetEase(Ease.OutCubic).OnStart(() =>
        //{
        //    contentPane.trans_edit.gameObject.SetActiveTrue();
        //});
    }


    void animateHideEditButtons(bool needAnim = true)
    {
        if (!GameSettingManager.inst.needShowUIAnim)
        {
            contentPane.trans_edit.gameObject.SetActiveFalse();
            return;
        }
        if (!needAnim)
        {
            if (!GameSettingManager.inst.needShowUIAnim)
            {
                contentPane.trans_edit.gameObject.SetActiveFalse();
                return;
            }
        }

        contentPane.trans_edit.DOAnchorPos3DY(-500f, 0.1f).From(0f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            contentPane.trans_edit.gameObject.SetActiveFalse();
        });
    }

    void showOpButtons(int modeIdx, Button[] modeBtns, bool needAnim)
    {
        confirmbtnMeet = true;
        //Logger.log("___" + mCreateMode.ToString() + "    needAnim : " + needAnim);

        if (mCreateMode == kCreateTypeOp.None || mCreateMode == kCreateTypeOp.CreateFurniture)
        {
            contentPane.create_op.gameObject.SetActiveFalse();

            for (int i = 0; i < mOpBtns.Length; i++)
            {
                var btn = mOpBtns[i];
                btn.gameObject.SetActiveFalse();
                //if (btn == contentPane.btn_store) Logger.error("我隐藏了");
            }
            contentPane.trans_op.gameObject.SetActiveTrue();
            for (int i = 0; i < modeBtns.Length; i++)
            {
                var btn = modeBtns[i];
                if (UIUnLockConfigMrg.inst.GetBtnInteractable(btn.name))
                {
                    btn.gameObject.SetActiveTrue();
                }
                //if (btn == contentPane.btn_store) Logger.error("我又打开了");
            }
            if (needAnim) animateOpButtons();
        }
        else
        {
            contentPane.trans_op.gameObject.SetActiveFalse();


            for (int i = 0; i < mCreateBtns.Length; i++)
            {
                var btn = mCreateBtns[i];
                btn.gameObject.SetActiveFalse();
            }

            contentPane.create_op.gameObject.SetActiveTrue();

            for (int i = 0; i < modeBtns.Length; i++)
            {
                var btn = modeBtns[i];
                if (UIUnLockConfigMrg.inst.GetBtnInteractable(btn.name))
                {
                    btn.gameObject.SetActiveTrue();
                }
            }

            if (needAnim) animateCreateBtns();
        }
    }

    void animateOpButtons()
    {
        if (!GameSettingManager.inst.needShowUIAnim)
        {
            contentPane.trans_op.anchoredPosition = new Vector2(0, 0)/*.From(-150f).SetEase(Ease.OutBack)*/;
            contentPane.hlayout_op.spacing = 0;
            return;
        }

        //var postion = contentPane.trans_op.anchoredPosition3D;
        //postion.y = 0;
        //contentPane.trans_op.anchoredPosition3D = postion;

        var hlayout = contentPane.hlayout_op;
        contentPane.trans_op.DOAnchorPos3DY(0, 0.15f).From(-150f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DOTween.To(() => hlayout.spacing, v => hlayout.spacing = v, 4f, 0.2f).From(-144f).SetEase(Ease.OutBack);
        }).OnStart(() => contentPane.hlayout_op.spacing = -144).SetDelay(mCreateMode == kCreateTypeOp.CreateFurniture ? 0.5f : 0);

    }

    void animateCreateBtns()
    {
        (contentPane.btn_back.transform as RectTransform).DOAnchorPos3DX(150, 0.2f).From(540).SetEase(Ease.OutBack).SetDelay(0.5f);
        (contentPane.create_btn_rotate.transform as RectTransform).DOAnchorPos3DX(318, 0.2f).From(540).SetEase(Ease.OutBack).SetDelay(0.5f);
        (contentPane.btn_applyAll.transform as RectTransform).DOAnchorPos3DX(-308, 0.2f).From(-540).SetEase(Ease.OutBack).SetDelay(0.5f);
        (contentPane.btn_apply.transform as RectTransform).DOAnchorPos3DX(-140, 0.2f).From(-540).SetEase(Ease.OutBack).SetDelay(0.5f);
    }

    void animateHideOpBtns(TweenCallback callback = null)
    {
        contentPane.trans_op.gameObject.SetActiveFalse();
        callback?.Invoke();
        //contentPane.trans_op.DOAnchorPos3DY(-150f, 0.1f).From(0).SetEase(Ease.OutCubic).OnComplete(() =>
        //{
        //    contentPane.trans_op.gameObject.SetActiveFalse();
        //    callback?.Invoke();
        //});
    }

    void animateHideOpButtons(TweenCallback callback = null)
    {
        var hlayout = contentPane.hlayout_op;

        if (GameSettingManager.inst.needShowUIAnim)
        {
            // hideOpBtnsTween = DOTween.To(() => hlayout.spacing, v => hlayout.spacing = v, -144f, 0.1f).From(12f).SetEase(Ease.OutCubic).OnComplete(() =>
            //{
            //    animateHideOpBtns(callback);
            //});

            for (int i = 0; i < mOpBtns.Length; i++)
            {
                var btn = mOpBtns[i];
                btn.transform.DOScale(0f, 0.2f).From(1f).SetEase(Ease.OutCubic).OnComplete(() =>
                 {
                     btn.transform.localScale = Vector3.one;
                 });
            }

            hideItemTitleTween?.Kill(true);

            hideItemTitleTween = contentPane.go_itemTitle.transform.DOScale(0f, 0.2f).From(1f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                contentPane.go_itemTitle.transform.localScale = Vector3.one;
                contentPane.trans_op.gameObject.SetActiveFalse();
                callback?.Invoke();
            });
        }
        else
        {
            animateHideOpBtns(callback);
        }
    }

    public void onPickItem(int uid)
    {
        hideItemTitleTween?.Kill(true);
        IndoorData.ShopDesignItem grid = UserDataProxy.inst.GetFuriture(uid);
        mSelectItem = grid;
        var groupType = grid.type;
        var newPick = checkFirstPick();

        if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.isDesigning)
        {
            //editModeSelect((kTileGroupType)groupType);
        }
        else
        {
            pickModeSelect((kTileGroupType)groupType, newPick);
        }

        showItemTitle();
    }
    public void UpdateTitle(int uid)
    {
        onPickItem(uid);
    }
    void showItemTitle()
    {
        var cfg = FurnitureConfigManager.inst.getConfig(mSelectItem.id);
        contentPane.txt_itemTitle.text = LanguageManager.inst.GetValueByKey(cfg.name.IfNullThenEmpty());
        var editOp = toEditOp((kTileGroupType)mSelectItem.type);
        contentPane.img_itemTitle.gameObject.SetActive(editOp == kEditTypeOp.EditUpgradable);
        contentPane.txt_itemLevel.text = mSelectItem.level + "";
        //if (mCreateMode == kCreateTypeOp.CreateFurniture)
        //{
        contentPane.img_confirmType.gameObject.SetActiveFalse();
        contentPane.txt_confirm.gameObject.SetActiveFalse();
        contentPane.txt_affirm.gameObject.SetActiveTrue();
        //}
        if (mSelectItem.type == (int)kTileGroupType.ResourceBin)    //资源篮
        {
            setResourceStatus(cfg.type_1, cfg.type_2, mSelectItem.level);
        }
        else if (mSelectItem.type == (int)kTileGroupType.PetHouse) //宠物小家
        {
            contentPane.txt_op_pets.text = PetDataProxy.inst.GetPetDataByFurnitureUid(mSelectItem.uid).petInfo.petName;
        }

        reSetContentSliderVal();

        setBuffDes(cfg);
    }

    void setBuffDes(FurnitureConfig cfg)
    {
        if (cfg.buff_ids == null || cfg.buff_ids.Length <= 0)
        {
            for (int i = 0; i < contentPane.txt_itemBuffDeses.Length; i++)
            {
                contentPane.txt_itemBuffDeses[i].gameObject.SetActiveFalse();
            }

            contentPane.vlGroup_itemTitle.padding.left = contentPane.img_itemTitle.gameObject.activeSelf ? 40 : 0;

            return;
        }

        for (int i = 0; i < contentPane.txt_itemBuffDeses.Length; i++)
        {
            if (i < cfg.buff_ids.Length)
            {
                contentPane.txt_itemBuffDeses[i].gameObject.SetActive(true);
                var buffCfg = FurnitureBuffConfigManager.inst.GetConfig(cfg.buff_ids[i]);
                if (buffCfg != null)
                    contentPane.txt_itemBuffDeses[i].text = LanguageManager.inst.GetValueByKey(buffCfg.type_title, buffCfg.getEffectVal().ToString());
            }
            else
            {
                contentPane.txt_itemBuffDeses[i].gameObject.SetActive(false);
            }
        }

        contentPane.vlGroup_itemTitle.padding.left = 0;
        //LayoutRebuilder.ForceRebuildLayoutImmediate(contentPane.go_itemTitle.transform as RectTransform);
    }

    public void reSetContentSliderVal()
    {
        if (mSelectItem == null || mSelectItem.uid == 0 || mSelectItem.type != (int)kTileGroupType.Shelf)
        {
            return;
        }

        var cfg = ShelfUpgradeConfigManager.inst.getConfigByType(mSelectItem.config.type_2, mSelectItem.level);
        contentPane.slider_content.value = (float)mSelectItem.equipList.Count / cfg.store;
    }

    bool res_isFull;
    int res_itemId;
    void setResourceStatus(int type1, int type2, int level)
    {
        var upCfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(type2, level);
        kResourceBinType tp = (kResourceBinType)type2;

        Item itm = ItemBagProxy.inst.GetItem(upCfg.item_id);
        res_itemId = itm.ID;
        Logger.info("setResourceStatus :" + itm.itemConfig.atlas + " icon:" + itm.itemConfig.icon);
        contentPane.img_resIcon.SetSprite(itm.itemConfig.atlas, itm.itemConfig.icon);
        contentPane.txt_resName.text = LanguageManager.inst.GetValueByKey(itm.itemConfig.name);
        contentPane.txt_resNum.text = itm.count + "/" + UserDataProxy.inst.GetResCountLimit(upCfg.item_id);
        res_isFull = itm.count >= UserDataProxy.inst.GetResCountLimit(upCfg.item_id);

    }

    bool checkFirstPick()
    {
        if (mDesignMode == kDesignMode.None)
        {
            mDesignMode = kDesignMode.Pick;
            //EventController.inst.TriggerEvent(GameEventType.HIDEUI_SHOPSCENE);
            return true;
        }
        return false;
    }

    void pickModeSelect(kTileGroupType groupType, bool newPick)
    {
        var pickOp = toPickOp(groupType);
        animateHideEditButtons();
        mPickMode = pickOp;
        showOpButtons((int)pickOp, mPickOpBtnList[(int)pickOp], newPick);
    }

    void editModeSelect(kTileGroupType groupType)
    {
        bool editBtnsActive = contentPane.trans_edit.gameObject.activeSelf;
        animateHideEditButtons(false);
        mEditMode = toEditOp(groupType);
        showOpButtons((int)mEditMode, mEditOpBtnList[(int)mEditMode], IndoorMapEditSys.inst.isClickFunriture && !editBtnsActive ? false : true);
    }

    void createModeSelect(kTileGroupType groupType)
    {
        mCreateMode = kCreateTypeOp.CreateFloor;
        showOpButtons((int)mCreateMode, mCreateOpBtnList[(int)mCreateMode], false);
    }

    public void onReleaseItem()
    {

    }

    //进入编辑模式
    void btn_editOnClick()
    {
        mDesignMode = kDesignMode.Edit;
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.FurnitureEdit, IndoorMap.tempItemUid);
    }

    void btn_cancelOnClick()
    {
        //取消 商店物品操作 并退出设计模式
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.normal, IndoorMap.tempItemUid);
    }

    //完成
    void btn_doneOnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.normal, IndoorMap.tempItemUid);
    }

    void btn_op_petsOnClick()
    {
        //进入宠物小家界面
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.LOOKPETHOUSE, mSelectItem);
    }

    public void showEditMenus()
    {
        contentPane.create_op.gameObject.SetActiveFalse();
        if (contentPane.trans_op.gameObject.activeSelf)
        {
            hideOpBtnsTween?.Kill(true);
            animateHideOpButtons(() => animateShowEditButtons(0));
        }
        else
        {
            animateShowEditButtons();
        }
    }

    void btn_storeOnClick()
    {
        if (mSelectItem.type == 6)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("柜台不可被回收"), GUIHelper.GetColorByColorHex("FFD907"));
            return;
        }
        else if (mSelectItem.state != 0)
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("不能存放正在升级的家具"), GUIHelper.GetColorByColorHex("FFD907"));
            return;
        }

        IndoorMapEditSys.inst.storeItem(mSelectItem.uid, (int)mSelectItem.type);
    }

    void btn_contentOnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHELFCONTENTUI);
    }

    void btn_upgradeOnClick()
    {
        switch ((EDesignState)mSelectItem.state)
        {
            case EDesignState.Idle:
                if (GuideManager.inst.isInTriggerGuide && HotfixBridge.inst.GetTriggerData().triggerVal == 9999) return;
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_UPGRADEPANEL, mSelectItem);
                break;
            case EDesignState.Upgrading:
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHELFUPGRADINGUI, mSelectItem);
                break;
            case EDesignState.Finished:
                EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Furniture_Upgrading_Finish, mSelectItem.uid);
                break;
        }
    }

    void btn_confirmOnClick()
    {
        if (mCreateMode != kCreateTypeOp.None)
        {
            mCreateMode = kCreateTypeOp.None;
        }

        IndoorMapEditSys.inst.isClickFunriture = false;
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Apply, false);
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
    }

    void btn_expandOnClick()
    {
        EDesignState state = (EDesignState)UserDataProxy.inst.shopData.currentState;
        switch (state)
        {
            case EDesignState.Idle:
                if (UserDataProxy.inst.shopData.shopLevel == StaticConstants.shopMap_MaxLevel)
                {
                    //已经到达最高级别
                    //Hidebtn_expand();
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("已达到最大面积不可扩建"), GUIHelper.GetColorByColorHex("FF2828"));
                    break;
                }
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENSIONPANEL);
                break;
            case EDesignState.Upgrading:
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENDINGPANEL);
                break;
            case EDesignState.Finished:
                IndoorMapEditSys.inst.shopUpgradeFinish();
                break;
        }
    }

    void btn_resourceOnClick()
    {
        if (!res_isFull)
        {
            EventController.inst.TriggerEvent(GameEventType.BagEvent.ShopDesign_Resource_BuyProduction, res_itemId);
        }
    }

    bool spaceCheck(int type1, int level)
    {
        var extCfg = ExtensionConfigManager.inst.GetExtensionConfig(UserDataProxy.inst.shopData.shopLevel);
        int num = 1;
        num += UserDataProxy.inst.GetEntitys(kTileGroupType.Shelf).Count;
        num += UserDataProxy.inst.GetEntitys(kTileGroupType.ResourceBin).Count;
        num += UserDataProxy.inst.GetEntitys(kTileGroupType.Trunk).Count;
        if (num > extCfg.furniture)
            return false;
        return true;
    }

    bool buyCheck(int costType, int needGold, int needGem, int needLevel)
    {
        if (GuideManager.inst.isInTriggerGuide)
        {
            return true;
        }
        if (costType == 1) //金币
        {
            if (UserDataProxy.inst.playerData.gold < needGold && UserDataProxy.inst.playerData.designFreeCount <= 0)
            {
                //金币不足
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("#FF2828"));
                return false;
            }
            if (UserDataProxy.inst.playerData.level < needLevel)
            {
                //等级不足
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("#FF2828"));
                return false;
            }
        }
        else if (costType == 2) //钻石
        {
            if (UserDataProxy.inst.playerData.gem < needGem)
            {
                //钻石不足
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不够"), GUIHelper.GetColorByColorHex("#FF2828"));
                return false;
            }
        }
        return true;
    }

    //返回按钮
    void btn_backOnClick()
    {
        mCreateMode = kCreateTypeOp.None;
        IndoorMapEditSys.inst.isClickFunriture = false;
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
        if (mCreateMode == kCreateTypeOp.CreateFurniture)
        {
            dispatchEvent(GameEventType.SHOWUI_FURNITUREUI);
        }
        else
        {
            dispatchEvent(GameEventType.SHOWUI_CUSTOMIZEUI);
        }
    }
    //应用
    void btn_applyOnClick()
    {
        IndoorMapEditSys.inst.isClickFunriture = false;
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Apply, false);
        // EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.EDITMODE_CHANGE, DesignMode.modeSelection, IndoorMap.tempItemUid);
    }

    void btn_applyAllOnClick()
    {
        IndoorMapEditSys.inst.isClickFunriture = false;
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Apply, true);

    }

    void btn_emptyOnClick()
    {
        //EventTriggerListener.Get(contentPane.emptyMask.gameObject).OnPointerClick(contentPane.)
        //btn_doneOnClick();
    }

    void dispatchEvent(string str)
    {
        EventController.inst.TriggerEvent(str);
    }

    void dispatchEvent(string str, int type)
    {
        EventController.inst.TriggerEvent(str, type);
    }

    public override void shiftIn()
    {
        base.shiftIn();
        if (contentPane.trans_op.gameObject.activeSelf && mDesignMode != kDesignMode.Create)
        {
            Refresh();
        }
        RefreshUIUnlock();
    }

    kPickTypeOp toPickOp(kTileGroupType groupType)
    {
        switch (groupType)
        {
            case kTileGroupType.Carpet:
                return kPickTypeOp.PickCarpet;
            case kTileGroupType.WallFurniture:
                return kPickTypeOp.PickDecor;
            case kTileGroupType.Furniture:
                return kPickTypeOp.PickDecor;
            case kTileGroupType.Counter:
                return kPickTypeOp.PickCounter;
            case kTileGroupType.Shelf:
                return kPickTypeOp.PickShelf;
            case kTileGroupType.Trunk:
                return kPickTypeOp.PickTrunk;
            case kTileGroupType.ResourceBin:
                return kPickTypeOp.PickBin;
            case kTileGroupType.OutdoorFurniture:
                return kPickTypeOp.PickDecor;
            case kTileGroupType.PetHouse:
                return kPickTypeOp.PickPetHouse;
            default:
                return kPickTypeOp.PickDecor;
        }
    }

    kEditTypeOp toEditOp(kTileGroupType groupType)
    {
        switch (groupType)
        {
            case kTileGroupType.Carpet:
                return kEditTypeOp.EditDecor;
            case kTileGroupType.WallFurniture:
                return kEditTypeOp.EditDecor;
            case kTileGroupType.Furniture:
                return kEditTypeOp.EditDecor;
            case kTileGroupType.Counter:
                return kEditTypeOp.EditUpgradable;
            case kTileGroupType.Shelf:
                return kEditTypeOp.EditUpgradable;
            case kTileGroupType.Trunk:
                return kEditTypeOp.EditUpgradable;
            case kTileGroupType.ResourceBin:
                return kEditTypeOp.EditUpgradable;
            case kTileGroupType.OutdoorFurniture:
                return kEditTypeOp.EditDecor;
            case kTileGroupType.PetHouse:
                return kEditTypeOp.EditDecor;
        }
        return kEditTypeOp.EditDecor;
    }

    kCreateTypeOp toCreateOp(kTileGroupType groupType)
    {
        switch (groupType)
        {
            case kTileGroupType.Wall:
                return kCreateTypeOp.CreateWall;
            case kTileGroupType.Floor:
                return kCreateTypeOp.CreateFloor;
        }
        return kCreateTypeOp.CreateFurniture;
    }

    #region z
    /////////////////////////////////////////////////////////////////////////////////////////////////
    public void Refresh()
    {
        switch (IndoorMapEditSys.inst.shopDesignMode)
        {
            case DesignMode.normal:
                {
                    int uid = 0;
                    if (IndoorMapEditSys.inst != null)
                        uid = IndoorMapEditSys.inst.currEntityUid;
                    if (uid > 0)
                    {
                        onPickItem(uid);
                    }
                    else
                    {
                        this.hide();
                    }
                }
                break;
            case DesignMode.modeSelection://编辑类型 选择
                {
                    int uid = IndoorMapEditSys.inst.currEntityUid;
                    if (IndoorMapEditSys.inst.isClickFunriture && uid != IndoorMap.tempItemUid && uid != -1)
                    {
                        onPickItem(uid);
                    }
                    else
                    {
                        IndoorMapEditSys.inst.shopDesignMode = DesignMode.FurnitureEdit;
                        mCreateMode = kCreateTypeOp.None;
                        showEditMenus();
                    }
                }
                break;
            case DesignMode.FurnitureEdit://场景家具编辑
                {
                    if (IndoorMapEditSys.inst != null)
                    {
                        mSelectItem = UserDataProxy.inst.GetFuriture(IndoorMapEditSys.inst.currEntityUid);
                        if (mSelectItem == null)
                        {
                            return;
                        }
                        mEditMode = toEditOp((kTileGroupType)mSelectItem.type);
                        int idx = (int)mEditMode;
                        animateHideEditButtons(false);
                        showOpButtons(idx, mEditOpBtnList[idx], !contentPane.trans_op.gameObject.activeSelf);
                        int uid = IndoorMapEditSys.inst.currEntityUid;
                        onPickItem(uid);
                    }
                }
                break;
            case DesignMode.FloorEdit:  //场景地板编辑
                break;
            case DesignMode.WallEdit:   //场景墙纸编辑
                break;
        }
    }

    private void RefreshUIUnlock()
    {
        GUIHelper.SetUIGray(contentPane.btn_edit_pets.transform, UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(170).parameters);
    }

    List<Button> CurrHandlerbtnList = new List<Button>();

    public void checkHandlerList()
    {

    }
    /////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion
}

public interface IAnimHelper
{
    void popup(int type, System.Action callback);
    void popin(int type, System.Action callback);
}

public class SubUI
{
    IAnimHelper mAnimHelper;
}


public class CustomTabGroup
{
    public event EventDelegate<int> onTabSelected;
    CustomTab[] mTabList;
    int mSelectIndex;
    public CustomTabGroup(Toggle[] tabList)
    {
        mTabList = new CustomTab[tabList.Length];
        for (int i = 0; i < tabList.Length; i++)
        {
            Toggle tg = tabList[i];
            CustomTab ct = new CustomTab(i, tg);
            if (i != 0)
                ct.setState(false);
            mTabList[i] = ct;

            ct.onValueChanged += onValueChanged;
        }
        mSelectIndex = 0;
    }

    private void onValueChanged(int idx, bool val)
    {
        AudioManager.inst.PlaySound(11);

        for (int i = 0; i < mTabList.Length; i++)
        {
            var t = mTabList[i];
            if (val && i != idx)
            {
                t.setState(false);
            }
            else if (i == idx)
            {
                t.setState(true);
            }
        }
        mSelectIndex = idx;
        onTabSelected?.Invoke(idx);
    }

    public void clear()
    {
        for (int i = 0; i < mTabList.Length; i++)
        {
            CustomTab ct = mTabList[i];
            ct.onValueChanged -= onValueChanged;
            ct.clear();
        }
    }
}

public class CustomTab
{
    public System.Action<int, bool> onValueChanged;
    Toggle mTab;
    int mIndex;
    public CustomTab(int idx, Toggle tg)
    {
        mIndex = idx;
        mTab = tg;

        mTab.onValueChanged.AddListener(onTabClicked);
    }

    public void setState(bool enabled)
    {
        mTab.isOn = enabled;

        for (int i = 0; i < mTab.graphic.transform.childCount; i++)
        {
            mTab.graphic.transform.GetChild(i).gameObject.SetActive(mTab.isOn);
        }
    }

    void onTabClicked(bool val)
    {
        if (!val)
            return;
        onValueChanged?.Invoke(mIndex, val);
    }
    public void clear()
    {
        mTab.onValueChanged.RemoveListener(onTabClicked);
    }

}
