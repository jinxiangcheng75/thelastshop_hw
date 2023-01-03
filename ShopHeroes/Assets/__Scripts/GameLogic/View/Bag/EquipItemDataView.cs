using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipItemDataView : ViewBase<EquipItemDataComp>
{
    public override string viewID => ViewPrefabName.EquipItemDataUI;

    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;
        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.leftBtn.ButtonClickTween(() =>
        {
            string frontequip = GetFrontEquipItem(equip.itemUid);
            //前一个
            if (!string.IsNullOrEmpty(frontequip))
                setEquipItem(ItemBagProxy.inst.GetEquipItem(frontequip), currItemList, true);
        });
        contentPane.rightBtn.ButtonClickTween(() =>
        {
            string nextequip = GetNextEquipItem(equip.itemUid);
            //下一个
            if (!string.IsNullOrEmpty(nextequip))
                setEquipItem(ItemBagProxy.inst.GetEquipItem(nextequip), currItemList, true);
        });

        contentPane.lockBtn.onValueChanged.AddListener((value) =>
        {
            if (string.IsNullOrEmpty(equip.itemUid)) return;
            if (equip.isLock != value)
            {
                //发送物品锁定
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_LOCK, equip.itemUid, value);
            }
        });
        contentPane.removeBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(equip.itemUid)) return;
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPRESOLVEUI, equip);

        });

        contentPane.drawingBtn.onClick.AddListener(() =>
        {
            EquipData data = EquipDataProxy.inst.GetEquipData(equip.ID);
            if (data == null) return;
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUI, equip.ID, new List<EquipData>());
        });

        contentPane.canWearHeroBtn.onClick.AddListener(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_CanWearEquipTip", contentPane.canWearHeroBtn.transform, equip.equipConfig.equipDrawingsConfig.sub_type);
        });
    }

    protected override void onShown()
    {
        base.onShown();
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
        equip = null;
        currItemList = null;
    }

    void animateShowUI(bool justTop = false)
    {

        contentPane.topAnimator.CrossFade("show", 0f);
        contentPane.topAnimator.Update(0f);
        contentPane.topAnimator.Play("show");
        if (!justTop)
        {
            contentPane.windowAnimator.CrossFade("show", 0f);
            contentPane.windowAnimator.Update(0f);
            contentPane.windowAnimator.Play("show");
        }


        //Graphic[] topGraphics = contentPane.topAnimator.GetComponentsInChildren<Graphic>();
        //foreach (var item in topGraphics) item.FadeFromTransparentTween(1, 0.4f);
        //foreach (var item in contentPane.btns) item.FadeFromTransparentTween(1, 0.4f);

        //if (!justTop)
        //{
        //    Graphic[] windowGraphics = contentPane.windowAnimator.GetComponentsInChildren<Graphic>();
        //    foreach (var item in windowGraphics) item.FadeFromTransparentTween(1, 0.5f);

        //    contentPane.mask.FadeFromTransparentTween(0.5f, 0.5f);
        //}
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
        animateShowUI();
    }

    protected override void DoHideAnimation()
    {
        contentPane.topAnimator.Play("hide");
        contentPane.windowAnimator.Play("hide");

        float animLength = contentPane.windowAnimator.GetClipLength("window_hide");

        //if (equip.quality > 1) contentPane.equipIcon.iconImage.material = null;//将描边取消


        //Graphic[] topGraphics = contentPane.topAnimator.GetComponentsInChildren<Graphic>();

        //foreach (var item in topGraphics) item.FadeTransparentTween(item.color.a, animLength);


        //foreach (var item in contentPane.btns) item.FadeTransparentTween(item.color.a, animLength);

        //Graphic[] windowGraphics = contentPane.windowAnimator.GetComponentsInChildren<Graphic>();
        //foreach (var item in windowGraphics) item.FadeTransparentTween(item.color.a, animLength);

        //contentPane.mask.FadeTransparentTween(0.5f, animLength);

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.topAnimator.CrossFade("null", 0f);
            contentPane.topAnimator.Update(0f);
            contentPane.windowAnimator.CrossFade("null", 0f);
            contentPane.windowAnimator.Update(0f);
            this.HideView();
        });
    }

    public string GetFrontEquipItem(string uid)
    {
        if (currItemList == null || currItemList.Count <= 1) return "";
        int index = currItemList.FindIndex(item => item.itemUid == uid);
        if (index <= 0)
        {
            return currItemList[currItemList.Count - 1].itemUid;
        }
        else
        {
            return currItemList[index - 1].itemUid;
        }
    }

    public string GetNextEquipItem(string uid)
    {
        if (currItemList == null || currItemList.Count <= 1) return "";
        int index = currItemList.FindIndex(item => item.itemUid == uid);
        if (index >= currItemList.Count - 1)
        {
            return currItemList[0].itemUid;
        }
        else
        {
            return currItemList[index + 1].itemUid;
        }
    }

    EquipItem equip; //当前item
    List<EquipItem> currItemList;

    public void UpdateList(string itemUid)
    {
        if (itemUid != equip.itemUid) return;
        setEquipItem(ItemBagProxy.inst.GetEquipItem(itemUid), currItemList);
    }

    public void setEquipItem(EquipItem item, List<EquipItem> equipitems, bool isSwitch = false)
    {
        if (item == null)
        {
            hide();
            return;
        }

        if (GameSettingManager.inst.needShowUIAnim && isSwitch)
        {
            contentPane.topAnimator.CrossFade("null", 0f);
            contentPane.topAnimator.Update(0f);
            animateShowUI(true);
        }

        equip = item;
        currItemList = equipitems;
        EquipData data = EquipDataProxy.inst.GetEquipData(equip.ID);
        contentPane.img_drawing.enabled = data != null;
        contentPane.removeBtn.gameObject.SetActive(!string.IsNullOrEmpty(item.itemUid));
        contentPane.leftBtn.gameObject.SetActive(currItemList != null && currItemList.Count > 1);
        contentPane.rightBtn.gameObject.SetActive(currItemList != null && currItemList.Count > 1);
        contentPane.equipNameTx.text = equip.equipConfig.name;
        //contentPane.equipNameTx.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[equip.quality - 1]);

        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(equip.equipConfig.equipDrawingsConfig.sub_type);
        contentPane.equipSubTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);

        contentPane.equipLevelTx.text = equip.equipConfig.equipDrawingsConfig.level.ToString();
        contentPane.equipSubTypeTx.text = LanguageManager.inst.GetValueByKey("{0}阶{1}", equip.equipConfig.equipDrawingsConfig.level.ToString(), LanguageManager.inst.GetValueByKey(EquipConfigManager.inst.GetEquipTypeByID(equip.equipConfig.equipDrawingsConfig.sub_type).name));
        contentPane.priceText.text = equip.sellPrice.ToString("N0");
        contentPane.desTx.text = LanguageManager.inst.GetValueByKey(equip.equipConfig.equipDrawingsConfig.desc);
        contentPane.equipIconBg.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[equip.quality - 1]);
        GUIHelper.showQualiyIcon(contentPane.equipIconBg.GetComponent<RectTransform>(), item.quality, 780f);
        contentPane.qualityText.text = "<color=" + StaticConstants.qualityTxtColor[equip.quality - 1] + ">" + LanguageManager.inst.GetValueByKey(StaticConstants.qualityNames[equip.quality - 1]) + "</color>";
        //contentPane.qualityText.text = $"<color={StaticConstants.qualityColor[equip.quality - 1]}>{StaticConstants.qualityNames[equip.quality - 1]}品质</color>";
        contentPane.qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityicon[equip.quality - 1]);
        contentPane.inBoxNumberTx.text = (equip.count - equip.onShelfCount).ToString();
        contentPane.inShelfNumberTx.text = equip.onShelfCount.ToString();
        contentPane.lockBtn.isOn = equip.isLock;
        string qcolor = equip.quality > 1 ? StaticConstants.qualityColor[equip.quality - 1] : "";
        contentPane.equipIcon.SetSpriteURL(equip.equipConfig.equipDrawingsConfig.big_icon, qcolor, true);

        setEquipCanWearInfo();
        setEquipProperty();
    }

    void setEquipCanWearInfo()
    {
        KeyValuePair<string, string>[] heroProfessionArr = ItemBagProxy.inst.GetCanWearHeroInfosByEquip(equip.equipConfig.equipDrawingsConfig.sub_type, equip.equipConfig.equipDrawingsConfig.level, out int canWearFloorLv);

        contentPane.tx_canWearFloorLv.text = LanguageManager.inst.GetValueByKey("穿戴等级：") + canWearFloorLv;

        for (int i = 0; i < contentPane.canWearHeroProfessionIcons.objList.Count; i++)
        {
            GameObject obj = contentPane.canWearHeroProfessionIcons.objList[i];

            if (i < heroProfessionArr.Length)
            {
                obj.SetActive(true);
                obj.GetComponent<GUIIcon>().SetSprite(heroProfessionArr[i].Key, heroProfessionArr[i].Value);
            }
            else
            {
                obj.SetActive(false);
            }

        }

    }

    private void setEquipProperty()
    {
        var propertyList = EquipConfigManager.inst.GetEquipAllPropertyList(equip.equipid);
        int valCount = 0;
        for (int i = 0; i < contentPane.allProperty.Count; i++)
        {
            int index = i;
            if (propertyList[index] > 0)
            {
                contentPane.allProperty[index].gameObject.SetActive(true);
                contentPane.allProperty[index].valText.text = "+" + propertyList[index].ToString();
                valCount++;
            }
            else
            {
                contentPane.allProperty[index].gameObject.SetActive(false);
            }
        }

        if (valCount <= 4)
        {
            contentPane.contentSize.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
            contentPane.contentSize.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contentPane.contentSize.transform.parent.GetComponent<RectTransform>().rect.height);
        }
        else
        {
            contentPane.contentSize.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
        }
    }

}
