using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Mosframe;

public class FurnitureSelectionUIView : ViewBase<FurnitureSelectionUIComp>
{
    public override string viewID => ViewPrefabName.FurnitureSelectionUI;
    CustomTabGroup mTabGroup;
    //DynamicScrollView mScrollView;
    SimpleScroller<CustomizeListItemComp> mScroller;
    Image mMask;
    int mDisplayType;
    List<FurnitureDisplayData> mDataList;
    List<CustomizeListItemComp> mListItem;
    bool needShowAni;
    int curFurnId = -1;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        windowAnimTime = contentPane.uiAnimator.GetClipLength("commonBagUI_show");

        mMask = contentPane.img_mask;
        contentPane.btn_close.ButtonClickTween(btn_closeOnClick);
        mTabGroup = new CustomTabGroup(contentPane.tg_catagoryList);
        mTabGroup.onTabSelected += onTabSelected;
        EventTriggerListener.Get(mMask.gameObject).onClick += onMaskClick;
        contentPane.gameObject.SetActiveFalse();
        mDisplayType = (int)kFurnitureDisplayType.ShelfAndTrunk;
        initScroller();
    }



    void onMaskClick(GameObject go)
    {
        //hide();
        btn_closeOnClick();
    }

    void initScroller()
    {
        var sr = contentPane.sr_itemlist.GetComponent<ScrollRect>();
        var item = contentPane.trans_scrollItem.GetComponent<CustomizeListItemComp>();
        var itemRt = item.GetComponent<RectTransform>();
        if (FGUI.inst.isLandscape)
        {
            mScroller = new SimpleScrollerCustomize(sr, item, 1, itemRt.sizeDelta.x, itemRt.sizeDelta.y, 10);
        }
        else
        {
            mScroller = new SimpleScrollerCustomize(sr, item, 3, itemRt.sizeDelta.x, itemRt.sizeDelta.y, 20);
        }
        mScroller.setItemRenderer(scrollItemRenderer);
        mScroller.setItemLifeCycle((obj) =>
        {
            var etl = EventTriggerListener.Get(obj.gameObject);
            etl.enablePass = true;
            // etl.onClick += itemOnClick;
            etl.onDrag += itemOnDrag;
            etl.onDown += itemOnPointerDown;
            etl.onUp += itemOnPointerUp;
        },
        (obj) =>
        {
            EventTriggerListener.Get(obj.gameObject).clear();
        });
    }

    GameObject currSeletItemDown = null;
    void itemOnPointerDown(GameObject go)
    {
        currSeletItemDown = go;
    }
    void itemOnPointerUp(GameObject go)
    {
        if (currSeletItemDown == go)
        {
            itemOnClick(currSeletItemDown);
            currSeletItemDown = null;
        }
    }
    void itemOnDrag(GameObject go, Vector2 data)
    {
        if (currSeletItemDown != null)
        {
            currSeletItemDown = null;
        }
    }
    protected override void onShown()
    {
        base.onShown();

        onTabSelected(mDisplayType);
        if (FGUI.inst != null && FGUI.inst.isLandscape)
        {
            contentPane.sr_itemlist.vertical = false;
            contentPane.sr_itemlist.horizontal = true;
        }
        else
        {
            contentPane.sr_itemlist.vertical = true;
            contentPane.sr_itemlist.horizontal = false;
        }
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            if ((K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
                contentPane.sr_itemlist.vertical = false;
        }
    }

    public void showTargetType(kFurnitureDisplayType type, int curFurnId)
    {
        this.curFurnId = curFurnId;
        contentPane.tg_catagoryList[(int)type].isOn = false;
        contentPane.tg_catagoryList[(int)type].isOn = true;
        //if (type != kFurnitureDisplayType.ShelfAndTrunk)
        //{

        //}
        //onTabSelected((int)type);
    }


    protected override void DoShowAnimation()
    {
        needShowAni = true;
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");

        contentPane.maskObj.SetActiveTrue();

        GameTimer.inst.AddTimer(mDataList.Count <= 9 ? mDataList.Count * 0.02f + 0.28f : 0.46f, 1, () =>
        {
            contentPane.maskObj.SetActiveFalse();
            needShowAni = false;
        }); //播放动画禁止滑动
    }

    protected override void DoHideAnimation()
    {

        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("commonBagUI_hide");

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });

    }

    public override void shiftIn()
    {
        base.shiftIn();
        onTabSelected(mDisplayType);
    }

    private void onTabSelected(int idx)
    {
        mDisplayType = idx;
        EventController.inst.TriggerEvent<kFurnitureDisplayType>(GameEventType.ShopDesignEvent.FurnitureSelection_TabSelectd, (kFurnitureDisplayType)mDisplayType);
    }

    public void refreshData(List<FurnitureDisplayData> list)
    {
        mDataList = list;
        refreshScroll();
    }

    void refreshScroll()
    {
        mScroller.setCount(mDataList.Count);
        if (curFurnId != -1)
        {
            for (int i = 0; i < mDataList.Count; i++)
            {
                if (mDataList[i].id == curFurnId)
                {
                    mScroller.setItemByIndex(i);
                    break;
                }
            }
        }
    }

    void scrollItemRenderer(int idx, CustomizeListItemComp item)
    {
        item.reSetData();

        var data = mDataList[idx];
        var cfg = data.cfg;
        item.name = cfg.id.ToString();
        item.img_icon.SetSprite(cfg.atlas, cfg.icon);
        item.img_bottomBg.enabled = true;
        item.txt_title.text = LanguageManager.inst.GetValueByKey(cfg.name) + "";
        item.img_subTypeIcon.SetSprite("shopdesign_atlas", StaticConstants.furnitureSubTypeIconNames[cfg.type_1]);

        item.txt_lockVipText.enabled = false;
        item.vipImg.enabled = false;
        item.buffObj.SetActive(false);

        item.giftBgIcon.gameObject.SetActive(false);

        if ((kFurnitureDisplayType)mDisplayType == kFurnitureDisplayType.ResourceBin || (kFurnitureDisplayType)mDisplayType == kFurnitureDisplayType.ShelfAndTrunk)
        {
            item.img_level.gameObject.SetActiveTrue();
            item.txt_level.text = data.level + "";
        }
        else
        {
            item.img_level.gameObject.SetActiveFalse();
        }

        item.index = idx;
        //未解锁
        uint playerLv = UserDataProxy.inst.playerData.level;

        item.img_palette.enabled = false;
        //item.img_palette.enabled = mDisplayType == (int)kFurnitureDisplayType.ShelfAndTrunk;

        ResourceBinUpgradeConfig resUpCfg = null;
        if ((kFurnitureDisplayType)mDisplayType == kFurnitureDisplayType.ResourceBin)
        {
            resUpCfg = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(cfg.type_2, 1);
        }


        if (cfg.unlock_lv > playerLv && data.storeNum <= 0)
        {
            item.unlock.gameObject.SetActiveTrue();
            item.img_num.gameObject.SetActiveFalse();
            item.txt_type.gameObject.SetActiveFalse();
            item.txt_cost.gameObject.SetActiveFalse();
            item.VarietyIconsTf.gameObject.SetActiveFalse();
            EventTriggerListener.Get(item.gameObject).enabled = false;
            item.img_bg.SetSprite("__common_1", "zhizuo_wupindi1");
            item.img_titleBg.SetSprite("__common_1", "zhizuo_mingzidihui");
            item.img_iconBg.SetSprite("__common_1", "cktb_wupinkuang");
            item.text_unlock.text = LanguageManager.inst.GetValueByKey("解锁等级：") + cfg.unlock_lv;
            item.slider_unlock.gameObject.SetActiveTrue();
            item.slider_unlock.value = (float)playerLv / cfg.unlock_lv;
            item.txt_unlockSlider.text = playerLv + "/" + cfg.unlock_lv;

            GUIHelper.SetSingleUIGray(item.img_iconBg.transform, true);
        }
        else if ((kFurnitureDisplayType)mDisplayType == kFurnitureDisplayType.ResourceBin && resUpCfg != null && UserDataProxy.inst.GetBuildingData(resUpCfg.build_id) != null
            && (UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).state == (int)EBuildState.EB_Lock || UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).level < resUpCfg.build_level)) //资源篮 并且未解锁
        {
            EventTriggerListener.Get(item.gameObject).enabled = true;
            item.unlock.gameObject.SetActiveTrue();
            item.img_num.gameObject.SetActiveFalse();
            item.txt_type.gameObject.SetActiveFalse();
            item.txt_cost.gameObject.SetActiveFalse();
            item.VarietyIconsTf.gameObject.SetActiveFalse();
            item.txt_sellOut.gameObject.SetActiveFalse();
            item.img_bg.SetSprite("__common_1", "zhizuo_wupindi1");
            item.img_titleBg.SetSprite("__common_1", "zhizuo_mingzidihui");
            item.img_iconBg.SetSprite("__common_1", "cktb_wupinkuang");
            item.text_unlock.text = LanguageManager.inst.GetValueByKey("解锁需求");
            item.slider_unlock.gameObject.SetActiveFalse();
            item.txt_unlockSlider.text = LanguageManager.inst.GetValueByKey(UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).config.name) + resUpCfg.build_level + LanguageManager.inst.GetValueByKey("级");
            item.txt_storeNum.gameObject.SetActive(false);

            GUIHelper.SetSingleUIGray(item.img_iconBg.transform, true);
        }
        else
        {
            item.unlock.gameObject.SetActiveFalse();
            GUIHelper.SetSingleUIGray(item.img_iconBg.transform, false);


            var isStorable = UserDataProxy.inst.CheckStorableType(cfg.type_1);

            if (isStorable)
            {
                var guideInfo = GuideDataProxy.inst.CurInfo;
                if ((kTileGroupType)cfg.type_1 == kTileGroupType.Shelf)
                {
                    item.txt_type.gameObject.SetActiveFalse();
                    item.VarietyIconsTf.gameObject.SetActiveTrue();

                    List<int> types = ShelfUpgradeConfigManager.inst.GetShelfImgTypes(cfg.type_2, false);

                    for (int i = 0; i < item.img_contentTypeList.Length; i++)
                    {
                        if (i < types.Count)
                        {
                            item.img_contentTypeList[i].transform.parent.gameObject.SetActive(true);


                            EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(types[i]);
                            item.img_contentTypeList[i].SetSprite(classcfg.Atlas, classcfg.icon);
                        }
                        else
                        {
                            item.img_contentTypeList[i].transform.parent.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    item.VarietyIconsTf.gameObject.SetActiveFalse();
                    item.txt_type.gameObject.SetActiveTrue();

                    switch ((kTileGroupType)cfg.type_1)
                    {

                        case kTileGroupType.Trunk:
                            item.txt_type.text = LanguageManager.inst.GetValueByKey("仓库");
                            TrunkUpgradeConfig trunkUpgradeConfig = TrunkUpgradeConfigManager.inst.getConfig(data.level);
                            item.img_mark.SetSprite("shopdesign_atlas", "jiaju_neirong");
                            item.txt_mark.text = trunkUpgradeConfig.space + "";
                            break;
                        case kTileGroupType.ResourceBin:
                            item.txt_type.text = LanguageManager.inst.GetValueByKey("资源");
                            ResourceBinUpgradeConfig subCfg = ResourceBinUpgradeConfigManager.inst.getConfigByType(cfg.type_2, data.level);

                            item.img_mark.SetSprite("item_atlas", "icon_ziyuan" + subCfg.type);
                            item.txt_mark.text = subCfg.store + "";

                            //if (!guideInfo.isAllOver)
                            //{
                            //    //Logger.error("资源栏" + guideInfo.m_curCfg.btn_name);
                            //    if (((K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick) && (guideInfo.m_curCfg.btn_name == "0" || guideInfo.m_curCfg.btn_name == "btn_furniture"))
                            //    {
                            //        if (cfg.id == int.Parse(guideInfo.m_curCfg.conditon_param_1))
                            //        {
                            //            if (needShowAni)
                            //                needSet = true;
                            //            else
                            //            {
                            //                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETMASKTARGET, item.transform);
                            //            }
                            //        }
                            //    }
                            //}
                            break;
                        default:
                            break;
                    }

                }



                int placeTotal = data.cfg.placement_num;
                int placedNum = data.placedNum;// UserDataProxy.inst.GetFuritureCount(data.id);
                int storeNum = data.storeNum;
                item.txt_storeNum.gameObject.SetActive(storeNum > 0);

                item.img_bottomBg.enabled = storeNum == 0;
                if (storeNum == 0)
                {
                    item.img_bg.SetSprite("__common_1", "zhizuo_wupindi");
                    item.img_titleBg.SetSprite("__common_1", "zhizuo_mingzidi");
                    item.img_iconBg.SetSprite("__common_1", "cktb_wupinkuang");
                }
                else
                {
                    item.img_bg.SetSprite("shopdesign_atlas", "jiaju_zjinbianditu");
                    item.img_titleBg.SetSprite("__common_1", "zhizuo_mingzidi1");
                    item.img_iconBg.SetSprite("shopdesign_atlas", "jiaju_zjinbianditu1");
                }

                item.txt_storeNum.text = "" + storeNum;
                item.txt_cost.text = string.Format("{0:N0}", data.costNum);
                item.img_cost.SetSprite("__common_1", data.cfg.cost_type == 1 ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai");


                if (placeTotal != -1)//不是无上限的
                {
                    item.img_num.gameObject.SetActiveTrue();
                    item.txt_num.text = placedNum + "/" + placeTotal;

                    EventTriggerListener.Get(item.gameObject).enabled = placedNum < placeTotal || storeNum > 0;

                    if (placedNum == placeTotal && storeNum <= 0)
                    {
                        item.img_bg.SetSprite("__common_1", "zhizuo_wupindi1");
                        item.img_titleBg.SetSprite("__common_1", "zhizuo_mingzidihui");
                        item.img_iconBg.SetSprite("__common_1", "cktb_wupinkuang");
                    }
                    item.txt_sellOut.gameObject.SetActive(placedNum == placeTotal && storeNum <= 0);
                    item.txt_cost.gameObject.SetActive(placedNum != placeTotal && storeNum <= 0);
                    GUIHelper.SetSingleUIGray(item.img_iconBg.transform, placedNum == placeTotal && storeNum <= 0);

                    //item.txt_cost.gameObject.SetActive(data.level >= 6 ? false : placedNum != placeTotal);
                }
                else
                {
                    //item.img_cost.gameObject.SetActiveTrue();
                    item.txt_cost.gameObject.SetActive(storeNum == 0);
                    item.txt_sellOut.gameObject.SetActiveFalse();
                    EventTriggerListener.Get(item.gameObject).enabled = true;
                    item.img_num.gameObject.SetActive(placedNum != 0);
                    item.txt_num.text = placedNum + "";
                }

                if (!guideInfo.isAllOver)
                {
                    if (((K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick))
                    {
                        if (UserDataProxy.inst.playerData.designFreeCount > 0)
                        {
                            if (cfg.id == int.Parse(guideInfo.m_curCfg.conditon_param_1))
                            {
                                item.txt_cost.gameObject.SetActive(false);
                                item.txt_storeNum.gameObject.SetActive(true);
                                item.txt_storeNum.text = LanguageManager.inst.GetValueByKey("免费");
                            }
                        }
                    }
                }

                if (GuideManager.inst.isInTriggerGuide)
                {
                    var triggerData = HotfixBridge.inst.GetTriggerData();
                    if (triggerData.triggerType == 3 && cfg.id == triggerData.triggerCondition)
                    {
                        item.txt_cost.gameObject.SetActive(false);
                        item.txt_storeNum.gameObject.SetActive(true);
                        item.txt_storeNum.text = LanguageManager.inst.GetValueByKey("免费");
                    }
                }
            }
            else
            {
                //家具类


                item.txt_sellOut.gameObject.SetActive(false);
                item.img_num.gameObject.SetActive(false);
                item.VarietyIconsTf.gameObject.SetActiveFalse();
                item.txt_type.gameObject.SetActiveTrue();

                EventTriggerListener.Get(item.gameObject).enabled = true;
                GUIHelper.SetUIGray(item.transform, false);
                int allNum = UserDataProxy.inst.GetFuritureCount(data.id);
                int storeNum = data.storeNum;

                item.img_bottomBg.enabled = storeNum == 0;
                if (storeNum == 0)
                {
                    item.img_bg.SetSprite("__common_1", "zhizuo_wupindi");
                    item.img_titleBg.SetSprite("__common_1", "zhizuo_mingzidi");
                    item.img_iconBg.SetSprite("__common_1", "cktb_wupinkuang");
                }
                else
                {
                    item.img_bg.SetSprite("shopdesign_atlas", "jiaju_zjinbianditu");
                    item.img_titleBg.SetSprite("__common_1", "zhizuo_mingzidi1");
                    item.img_iconBg.SetSprite("shopdesign_atlas", "jiaju_zjinbianditu1");
                }

                int placeTotal = data.cfg.placement_num;
                int placedNum = data.placedNum;

                item.txt_storeNum.gameObject.SetActive(storeNum > 0);
                item.txt_storeNum.text = "" + storeNum;

                if (placeTotal != -1)//不是无上限的
                {
                    item.img_num.gameObject.SetActiveTrue();
                    item.txt_num.text = placedNum + "/" + placeTotal;

                    EventTriggerListener.Get(item.gameObject).enabled = placedNum < placeTotal || storeNum > 0;

                    if (placedNum == placeTotal && storeNum <= 0)
                    {
                        item.img_bg.SetSprite("__common_1", "zhizuo_wupindi1");
                        item.img_titleBg.SetSprite("__common_1", "zhizuo_mingzidihui");
                        item.img_iconBg.SetSprite("__common_1", "cktb_wupinkuang");
                    }
                    item.txt_sellOut.gameObject.SetActive(placedNum == placeTotal && storeNum <= 0);
                    item.txt_cost.gameObject.SetActive(placedNum != placeTotal && storeNum <= 0);
                    GUIHelper.SetSingleUIGray(item.img_iconBg.transform, placedNum == placeTotal && storeNum <= 0);

                }
                else
                {
                    item.img_num.gameObject.SetActive(allNum != 0);
                    item.txt_num.text = allNum + "";

                    item.txt_cost.gameObject.SetActive(storeNum == 0);

                    item.img_cost.SetSprite("__common_1", data.cfg.cost_type == 1 ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai");
                    item.txt_cost.text = string.Format("{0:N0}", data.costNum);
                    item.index = idx;
                }

                if ((kTileGroupType)cfg.type_1 == kTileGroupType.Carpet)
                {
                    item.txt_type.text = LanguageManager.inst.GetValueByKey("大小");
                    item.img_mark.SetSprite("shopdesign_atlas", "jiaju_bianjikongjian");
                    item.txt_mark.text = cfg.width + "×" + cfg.height;
                }
                else
                {

                    item.txt_type.text = "";
                    item.img_mark.iconImage.enabled = false;
                    item.txt_mark.text = "";


                    if (cfg.buff_ids != null && cfg.buff_ids.Length > 0)
                    {
                        item.buffObj.SetActive(true);
                        for (int i = 0; i < item.buffTextList.Count; i++)
                        {
                            if (i < cfg.buff_ids.Length)
                            {
                                item.buffTextList[i].gameObject.SetActive(true);
                                var buffCfg = FurnitureBuffConfigManager.inst.GetConfig(cfg.buff_ids[i]);
                                if (buffCfg != null)
                                    item.buffTextList[i].text = LanguageManager.inst.GetValueByKey(buffCfg.buff_txt, buffCfg.getEffectVal().ToString());
                            }
                            else
                            {
                                item.buffTextList[i].gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        if (cfg.energy > 0)
                        {
                            item.txt_type.text = LanguageManager.inst.GetValueByKey("能量");
                            item.img_mark.SetSprite("__common_1", "zhuejiemian_tili");
                            item.txt_mark.text = cfg.energy + "";
                        }
                    }

                }


                if (cfg.cost_type == 3)
                {
                    item.txt_cost.gameObject.SetActiveFalse();
                    item.txt_lockVipText.enabled = true;
                    if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
                    {
                        item.vipImg.enabled = true;
                        item.txt_lockVipText.text = LanguageManager.inst.GetValueByKey("购买贵宾特权获取");
                    }
                    else
                    {
                        item.vipImg.enabled = false;
                        if (cfg.cost_num > UserDataProxy.inst.playerData.vipLevel)
                        {
                            EventTriggerListener.Get(item.gameObject).enabled = false;
                            item.txt_lockVipText.enabled = true;
                            item.txt_lockVipText.text = LanguageManager.inst.GetValueByKey("特权卡等级达到{0}级可解锁", cfg.cost_num.ToString());
                        }
                        else
                        {
                            EventTriggerListener.Get(item.gameObject).enabled = true;
                            item.txt_lockVipText.enabled = false;
                        }
                    }
                }
                else if (cfg.cost_type == 4)//礼包
                {
                    item.txt_cost.gameObject.SetActiveFalse();

                    if (allNum <= 0)
                    {
                        if (HotfixBridge.inst.GetDirectPurchaseDataById(cfg.cost_num, out DirectPurchaseData directPurchaseData))
                        {
                            item.giftBgIcon.gameObject.SetActive(true);
                            item.giftBgIcon.SetSprite(directPurchaseData.bgIconAtlas, directPurchaseData.bgIcon);
                            item.giftIcon.SetSprite(directPurchaseData.iconAtlas, directPurchaseData.icon);
                        }
                        else
                        {
                            item.giftBgIcon.gameObject.SetActive(false);

                        }
                    }
                    else
                    {
                        item.giftBgIcon.gameObject.SetActive(false);
                    }

                }


                var guideInfo = GuideDataProxy.inst.CurInfo;

                if (!guideInfo.isAllOver)
                {
                    if (((K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick))
                    {
                        if (UserDataProxy.inst.playerData.designFreeCount > 0)
                        {
                            if (cfg.id == int.Parse(guideInfo.m_curCfg.conditon_param_1))
                            {
                                item.txt_cost.gameObject.SetActive(false);
                                item.txt_storeNum.gameObject.SetActive(true);
                                item.txt_storeNum.text = LanguageManager.inst.GetValueByKey("免费");
                            }
                        }
                    }
                }

                if (GuideManager.inst.isInTriggerGuide)
                {
                    var triggerData = HotfixBridge.inst.GetTriggerData();
                    if (triggerData.triggerType == 3 && cfg.id == triggerData.triggerCondition)
                    {
                        item.txt_cost.gameObject.SetActive(false);
                        item.txt_storeNum.gameObject.SetActive(true);
                        item.txt_storeNum.text = LanguageManager.inst.GetValueByKey("免费");
                    }
                }
            }

        }


        LayoutRebuilder.ForceRebuildLayoutImmediate(item.txt_type.rectTransform);

        if (idx < 9 && needShowAni)
        {
            item.showAnim(idx);
        }

    }

    void itemOnClick(GameObject go)
    {

        var item = go.GetComponent<CustomizeListItemComp>();
        var data = mDataList[item.index];
        //DoTweenUtil.ClickTween(item.transform as RectTransform, () =>
        //{
        FurnitureConfig cfg = FurnitureConfigManager.inst.getConfig(data.id);

        if (cfg.cost_type == 3)
        {
            if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_BuyVipUI", 0);
                return;
            }
        }
        else if (cfg.cost_type == 4 && data.storeNum <= 0)
        {
            HotfixBridge.inst.TriggerLuaEvent("CSCallLua_ShowGiftDeatilUI", cfg.cost_num);
            return;
        }



        if (!itemclickEnd)
        {
            itemclickEnd = true;
            if ((kFurnitureDisplayType)mDisplayType == kFurnitureDisplayType.ResourceBin)
            {
                var resUpCfg = FurnitureUpgradeConfigManager.inst.GetResourceBinUpgradeConfig(cfg.type_2, 1);

                if (resUpCfg != null && UserDataProxy.inst.GetBuildingData(resUpCfg.build_id) != null && (UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).state == (int)EBuildState.EB_Lock
                || UserDataProxy.inst.GetBuildingData(resUpCfg.build_id).level < resUpCfg.build_level))
                {
                    EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, resUpCfg.build_id);
                    itemclickEnd = false;
                    return;
                }
            }

            if (cfg.type_1 == 7 || cfg.type_1 == 8 || cfg.type_1 == 9)
            {
                bool full = UserDataProxy.inst.shopData.indoorMapFurniture() >= UserDataProxy.inst.shopData.furnitureLimit();
                if (full)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("扩建您的店铺，获得更多空间。"), Color.red);

                    if (UserDataProxy.inst.shopData.shopLevel == StaticConstants.shopMap_MaxLevel)
                    {
                        //已经到达最高级别
                        itemclickEnd = false;
                        return;
                    }
                    //格子不足
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 25);
                    itemclickEnd = false;
                    return;
                }
            }

            GUIManager.HideView<FurnitureSelectionUIView>();
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.Create_Furniture, data);
            HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_Create_Furniture", data);
        }
        // });
    }
    bool itemclickEnd = false;
    protected override void onHide()
    {

        itemclickEnd = false;
        curFurnId = -1;
        base.onHide();
    }
    string getTitleIcon()
    {
        return "";
    }

    int getMarkNum()
    {
        return 0;
    }
    string getMarkIcon()
    {
        return "";
    }

    string getCostIcon()
    {
        return "";
    }

    void itemPicked(int idx)
    {
        //var data = mDataList[idx];
        // EventController.inst.TriggerEvent<int, int>(GameEventType.ShopDesignEvent.Create_Furniture, data.id, data.level);
        //hide();
    }

    void btn_closeOnClick()
    {

        GUIManager.HideView<FurnitureSelectionUIView>();
        //contentPane.dgAnim.hide();
    }

    protected override void beforeDispose()
    {
        clear();
    }

    void clear()
    {
        mScroller.clear();
        mTabGroup.clear();
        mTabGroup.onTabSelected -= onTabSelected;
    }
}

public class SimpleScrollerCustomize : SimpleScroller<CustomizeListItemComp>
{
    public SimpleScrollerCustomize(ScrollRect sr, CustomizeListItemComp itemTemplate, int rowNum, float itemWidth, float itemHeight, float spacing)
        : base(sr, itemTemplate, rowNum, itemWidth, itemHeight, spacing) { }
}

public class SimpleScroller<T> where T : MonoBehaviour
{
    ScrollRect mScroll;
    T mItemTemplate;
    int mRowNum;
    System.Action<int, T> mItemRenderer;
    Stack<T> mPool;
    List<T> mShowedItems;
    int mMaxCount;
    float mItemWidth;
    float mItemHeight;
    float mSpacing;
    float mMinContentHeight;
    System.Action<T> mItemCreateCallback;
    System.Action<T> mItemDestroyCallback;
    public SimpleScroller(ScrollRect sr, T itemTemplate, int rowNum, float itemWidth, float itemHeight, float spacing)
    {
        mScroll = sr;
        mItemTemplate = itemTemplate;
        mRowNum = rowNum;
        mPool = new Stack<T>();
        mMaxCount = 0;
        mItemTemplate.gameObject.SetActiveFalse();
        mScroll.onValueChanged.AddListener(onScrollValueChanged);
        mItemWidth = itemWidth;
        mItemHeight = itemHeight;
        mSpacing = spacing;
        mShowedItems = new List<T>();
        mMinContentHeight = sr.content.sizeDelta.y;
    }

    public void setItemRenderer(System.Action<int, T> itemRenderer)
    {
        mItemRenderer = itemRenderer;
    }

    public void setCount(int count)
    {
        mMaxCount = count;
        fillItems();
        refreshItems();
    }

    public void setItemByIndex(int index)
    {
        var contentPos = mScroll.content.anchoredPosition;
        if (FGUI.inst != null && !FGUI.inst.isLandscape)
        {
            float height = (index / mRowNum) * mItemHeight;
            contentPos.y = height;
        }
        else if (FGUI.inst != null && FGUI.inst.isLandscape)
        {
            float width = (index / mRowNum) * mItemWidth;
            contentPos.x = width;
        }
        mScroll.content.anchoredPosition = contentPos;
    }

    public void setItemLifeCycle(System.Action<T> createCB, System.Action<T> destroyCB)
    {
        mItemCreateCallback = createCB;
        mItemDestroyCallback = destroyCB;
    }

    void fillItems()
    {
        int num = mShowedItems.Count - mMaxCount;
        if (num < 0)
        {
            for (int i = 0; i < Mathf.Abs(num); i++)
            {
                T obj = default;
                if (mPool.Count > 0)
                {
                    obj = mPool.Pop();
                }
                else
                {
                    obj = GameObject.Instantiate<T>(mItemTemplate, mItemTemplate.transform.parent);
                    mItemCreateCallback?.Invoke(obj);
                }
                obj.gameObject.SetActiveTrue();
                mShowedItems.Add(obj);
            }
        }
        else if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                var idx = mShowedItems.Count - 1;
                var item = mShowedItems[idx];
                item.gameObject.SetActiveFalse();
                mShowedItems.RemoveAt(idx);
                mPool.Push(item);
            }
        }
        var contentPos = mScroll.content.anchoredPosition;
        var size = mScroll.content.sizeDelta;
        if (FGUI.inst != null && !FGUI.inst.isLandscape)
        {
            contentPos.y = 0;
            mScroll.content.anchoredPosition = contentPos;
            var contentHeight = (mMaxCount / mRowNum + 1) * mItemHeight;
            size.y = Mathf.Clamp(contentHeight, mMinContentHeight, contentHeight);
        }
        else if (FGUI.inst != null && FGUI.inst.isLandscape)
        {
            contentPos.x = 0;
            mScroll.content.anchoredPosition = contentPos;
            var contentWidth = (mShowedItems.Count / mRowNum + 1) * mItemWidth;
            size.x = contentWidth;
        }
        mScroll.content.sizeDelta = size;
    }

    void refreshItems()
    {
        for (int i = 0; i < mShowedItems.Count; i++)
        {
            var item = mShowedItems[i];
            var rt = item.GetComponent<RectTransform>();
            var pos = Vector2.zero;
            if (FGUI.inst != null && !FGUI.inst.isLandscape)
            {
                pos.x = (i % mRowNum) * (mItemWidth + mSpacing) + mItemWidth * rt.pivot.x;
                pos.y = -(i / mRowNum) * (mItemHeight + mSpacing) + mItemHeight * (1 - rt.pivot.y);
            }
            else if (FGUI.inst != null && FGUI.inst.isLandscape)
            {
                pos.x = (i / mRowNum) * (mItemWidth + mSpacing) + mItemWidth * rt.pivot.x;
                pos.y = -(i % mRowNum) * (mItemHeight + mSpacing) + mItemHeight * (1 - rt.pivot.y);
            }
            rt.anchoredPosition = pos;
            mItemRenderer?.Invoke(i, item);
        }
    }

    float mBottomCacheY;
    float mTopCacheY;
    void onScrollValueChanged(Vector2 val)
    {
        //0: bottom , 1: top
        //swipe up, -> 0, swipe down -> 1
        //bottom: 0, top: content.height
        //var py = mScroll.content.sizeDelta.y * val;
        //Logger.info("scroll y:" + py + " val:" + val);
        //var vy = val.y;
        //


        //

    }

    public void clear()
    {
        while (mPool.Count > 0)
        {
            var obj = mPool.Pop();
            mItemDestroyCallback?.Invoke(obj);
            GameObject.Destroy(obj.gameObject);
        }
    }
}