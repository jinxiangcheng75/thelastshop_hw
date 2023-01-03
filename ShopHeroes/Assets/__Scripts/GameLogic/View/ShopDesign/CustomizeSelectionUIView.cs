using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Mosframe;

public class CustomizeSelectionUIView : ViewBase<CustomizeSelectionUIComp>
{
    public override string viewID => ViewPrefabName.CustomizeSelectionUI;

    Image mMask;
    CustomizeSelectionUIComp mComp;
    CustomTabGroup mTabGroup;
    int mDisplayType;
    List<CustomizeDisplayData> mDataList;
    SimpleScrollerCustomize mScroller;

    bool needShowAni;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;

        mComp = contentPane;
        var c = mComp;
        mMask = c.img_mask;

        mTabGroup = new CustomTabGroup(c.tg_group);
        mTabGroup.onTabSelected += onTabSelected;

        mComp.btn_close.ButtonClickTween(btn_closeOnClick);
        EventTriggerListener.Get(mMask.gameObject).onClick += onMaskClick;
        initScroller(c);
    }

    void onMaskClick(GameObject go)
    {
        hide();
    }

    void initScroller(CustomizeSelectionUIComp c)
    {
        var sr = c.sr_itemList.GetComponent<ScrollRect>();
        var item = c.trans_scrollItem.GetComponent<CustomizeListItemComp>();
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
            //etl.onClick += itemOnClick;
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

        onTabSelected(mDisplayType);
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
        float animTime = contentPane.uiAnimator.GetClipLength("commonBagUI_hide");
        GameTimer.inst.AddTimer(animTime, 1, () =>
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
        EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.CustomizeSelection_TabSelectd, (kCustomizeDisplayType)mDisplayType);
    }

    public void refreshData(List<CustomizeDisplayData> list)
    {
        mDataList = list;
        refreshScroll();
    }
    void refreshScroll()
    {
        mScroller.setCount(mDataList.Count);
    }

    void itemOnClick(GameObject go)
    {
        var item = go.GetComponent<CustomizeListItemComp>();
        var data = mDataList[item.index];

        FurnitureConfig cfg = FurnitureConfigManager.inst.getConfig(data.id);

        bool hase = cfg.type_1 == 1 ? UserDataProxy.inst.shopData.hasWall(cfg.id) : UserDataProxy.inst.shopData.hasFloor(cfg.id);

        if (!hase)
        {
            if (cfg.cost_type == 3)
            {
                if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
                {
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_BuyVipUI", 0);
                    return;
                }
            }
            else if (cfg.cost_type == 4)
            {
                HotfixBridge.inst.TriggerLuaEvent("CSCallLua_ShowGiftDeatilUI", cfg.cost_num);
                return;
            }
        }

        //EventController.inst.TriggerEvent<int>(GameEventType.ShopDesignEvent.Create_Customize, data.cfg.id);
        HotfixBridge.inst.TriggerLuaEvent("ShopDesignUI_Create_Customize", data.cfg.id);

        hide();

    }

    void scrollItemRenderer(int idx, CustomizeListItemComp item)
    {

        item.reSetData();
        item.unlock.gameObject.SetActiveFalse();
        item.img_palette.enabled = false;
        var data = mDataList[idx];
        var cfg = data.cfg;
        item.name = cfg.id.ToString();
        item.index = idx;
        item.img_icon.SetSprite(cfg.atlas, cfg.icon);
        item.txt_title.text = LanguageManager.inst.GetValueByKey(cfg.name) + "";
        item.img_subTypeIcon.SetSprite("shopdesign_atlas", StaticConstants.furnitureSubTypeIconNames[cfg.type_1]);
        item.img_level.gameObject.SetActiveFalse();
        item.img_num.enabled = false;
        item.txt_num.text = string.Empty;
        item.txt_lockVipText.enabled = false;
        item.vipImg.enabled = false;
        item.buffObj.SetActive(false);
        item.giftBgIcon.gameObject.SetActive(false);


        bool _lock = cfg.unlock_lv > UserDataProxy.inst.playerData.level;
        EventTriggerListener.Get(item.gameObject).enabled = !_lock;
        if (!_lock)
        {
            //GUIHelper.SetUIGrayColor(item.transform, 0f);
            if (cfg.energy > 0)
            {
                item.txt_type.text = LanguageManager.inst.GetValueByKey("能量");
                item.img_mark.SetSprite("__common_1", "zhuejiemian_tili");
                item.txt_mark.text = cfg.energy + "";
            }
            else
            {
                item.txt_type.text = "";
                item.img_mark.iconImage.enabled = false;
                item.txt_mark.text = "";
            }
            bool hase = cfg.type_1 == 1 ? UserDataProxy.inst.shopData.hasWall(cfg.id) : UserDataProxy.inst.shopData.hasFloor(cfg.id);
            if (hase)
            {
                item.txt_owned.gameObject.SetActiveTrue();
                item.txt_cost.gameObject.SetActiveFalse();
            }
            else
            {
                item.txt_owned.gameObject.SetActiveFalse();
                item.txt_cost.gameObject.SetActiveFalse();

                if (cfg.cost_type == 3) //Vip
                {
                    item.txt_lockVipText.enabled = true;
                    if ((K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip)
                    {
                        item.vipImg.enabled = true;
                        //item.txt_storeNum.enabled = false;
                        item.txt_lockVipText.text = LanguageManager.inst.GetValueByKey("购买贵宾特权获取");
                    }
                    else
                    {
                        item.buffObj.SetActive(true);
                        for (int i = 0; i < cfg.buff_ids.Length; i++)
                        {
                            if (i <= 1)
                            {
                                var buffCfg = FurnitureBuffConfigManager.inst.GetConfig(cfg.buff_ids[i]);
                                item.buffTextList[i].text = LanguageManager.inst.GetValueByKey(buffCfg.buff_txt, buffCfg.getEffectVal().ToString());
                            }
                        }
                    }

                }
                else if (cfg.cost_type == 4) //礼包
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
                    item.txt_cost.gameObject.SetActiveTrue();
                    item.img_cost.SetSprite("__common_1", data.cfg.cost_type == 1 ? "zhuejiemian_meiyuan" : "zhuejiemian_jinkuai");
                    item.txt_cost.text = cfg.cost_num + "";
                }
            }
        }
        else
        {
            //GUIHelper.SetUIGrayColor(item.transform, 0.3f);
            item.unlock.gameObject.SetActiveTrue();
            item.img_num.gameObject.SetActiveFalse();
            item.txt_type.gameObject.SetActiveFalse();
            item.txt_cost.gameObject.SetActiveFalse();
            item.VarietyIconsTf.gameObject.SetActiveFalse();

            item.slider_unlock.gameObject.SetActiveTrue();

            item.text_unlock.text = LanguageManager.inst.GetValueByKey("解锁等级：") + cfg.unlock_lv;

            item.slider_unlock.maxValue = cfg.unlock_lv;
            item.slider_unlock.value = UserDataProxy.inst.playerData.level;
            item.txt_unlockSlider.text = UserDataProxy.inst.playerData.level + "/" + cfg.unlock_lv;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(item.txt_type.rectTransform);

        if (idx < 9 && needShowAni)
        {
            item.showAnim(idx);
        }

    }

    void btn_closeOnClick()
    {

        hide();
    }
}
