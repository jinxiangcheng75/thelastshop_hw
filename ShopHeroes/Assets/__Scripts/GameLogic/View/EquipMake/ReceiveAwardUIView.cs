using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ReceiveAwardUIView : ViewBase<ReceiveAwardUIComp>
{
    public override string viewID => ViewPrefabName.ReceiveAwardUI;
    public override string sortingLayerName => "popup";
    public override int showType => (int)ViewShowType.normal;
    private ReceiveInfoUIType currType;  //当前类型
    protected override void onInit()
    {
        base.onInit();
        contentPane.drawingStarUp.btn_continue.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        });
    }

    protected override void onShown()
    {
        closeCurrUIPanel();
        FGUI.inst.setGlobalMaskActice(false);
        if (curritem != null)
        {
            SetShowInfo(curritem);
        }
    }

    //public override void shiftIn()
    //{
    //    animReset();
    //    setAnim(currType);
    //}

    public void closeCurrUIPanel()
    {
        contentPane.getQualityItemUI.gameObject.SetActive(false);
        contentPane.getItemUI.gameObject.SetActive(false);
        contentPane.newBlueprint.gameObject.SetActive(false);
        contentPane.drawingUp.gameObject.SetActive(false);
        contentPane.activateDrawingUI.gameObject.SetActive(false);
        contentPane.drawingStarUp.gameObject.SetActive(false);
    }

    void setAnim(ReceiveInfoUIType infoType)
    {

        var guideInfo = GuideDataProxy.inst.CurInfo;

        switch (infoType)
        {
            case ReceiveInfoUIType.AdvacedEquip:
                contentPane.getQualityItemUI.gameObject.SetActive(true);
                ///动画
                if (GameSettingManager.inst.needShowUIAnim)
                {
                    contentPane.getQualityItemUI.uiAnimator.CrossFade("show", 0f);
                    contentPane.getQualityItemUI.uiAnimator.Update(0f);
                    contentPane.getQualityItemUI.uiAnimator.Play("show");
                }
                break;
            case ReceiveInfoUIType.GetItem:
                contentPane.getItemUI.gameObject.SetActive(true);
                ///动画
                if (GameSettingManager.inst.needShowUIAnim)
                {
                    contentPane.getItemUI.uiAnimator.CrossFade("show", 0f);
                    contentPane.getItemUI.uiAnimator.Update(0f);
                    contentPane.getItemUI.uiAnimator.Play("show");
                    if (!guideInfo.isAllOver && (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.GetItemPanel)
                    {
                        float timeLen = contentPane.activateDrawingUI.uiAnimator.GetClipLength("show");
                        GameTimer.inst.AddTimer(timeLen, 1, () =>
                        {
                            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, contentPane.getItemUI.okBtn.transform, 140, false);
                        });
                    }
                }
                else
                {
                    if (!guideInfo.isAllOver && (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.GetItemPanel)
                        EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, contentPane.getItemUI.okBtn.transform, 140, false);
                }
                break;
            case ReceiveInfoUIType.UnLockDrawing:
                contentPane.newBlueprint.gameObject.SetActive(true);
                if (GameSettingManager.inst.needShowUIAnim)
                {
                    contentPane.newBlueprint.uiAnimator.CrossFade("show", 0f);
                    contentPane.newBlueprint.uiAnimator.Update(0f);
                    contentPane.newBlueprint.uiAnimator.Play("show");
                    //contentPane.newBlueprint.studyBtn.transform.Fade_0_To_a_All(1, 0.2f, 0.5f);
                    //contentPane.newBlueprint.okBtn.transform.Fade_0_To_a_All(1, 0.2f, 0.5f);
                    //contentPane.newBlueprint.guideBtn.transform.Fade_0_To_a_All(1, 0.2f, 0.5f);
                }
                break;
            case ReceiveInfoUIType.DrawingUpLv:
                contentPane.drawingUp.gameObject.SetActive(true);
                ///动画
                if (GameSettingManager.inst.needShowUIAnim)
                {
                    contentPane.drawingUp.uiAnimator.CrossFade("show", 0f);
                    contentPane.drawingUp.uiAnimator.Update(0f);
                    contentPane.drawingUp.uiAnimator.Play("show");
                    //contentPane.drawingUp.milestoneIcon.transform.Fade_0_To_a_All(1, 0.3f, 0.15f);
                }
                break;
            case ReceiveInfoUIType.ActivateDrawing:
                contentPane.activateDrawingUI.gameObject.SetActive(true);
                ///动画
                if (GameSettingManager.inst.needShowUIAnim)
                {
                    contentPane.activateDrawingUI.uiAnimator.CrossFade("show", 0f);
                    contentPane.activateDrawingUI.uiAnimator.Update(0f);
                    contentPane.activateDrawingUI.uiAnimator.Play("show");
                    if (!guideInfo.isAllOver && (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.ResearchEquip)
                    {
                        float timeLen = contentPane.activateDrawingUI.uiAnimator.GetClipLength("show");
                        GameTimer.inst.AddTimer(timeLen, 1, () =>
                        {
                            EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, contentPane.activateDrawingUI.okBtn.transform, 140, false);
                        });
                    }
                }
                else
                {
                    if (!guideInfo.isAllOver && (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.ResearchEquip)
                        EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETFINGERPOS, contentPane.activateDrawingUI.okBtn.transform, 140, false);
                }
                break;
        }

    }

    void animReset()
    {
        contentPane.getQualityItemUI.uiAnimator.CrossFade("null", 0f);
        contentPane.getQualityItemUI.uiAnimator.Update(0f);

        contentPane.getItemUI.uiAnimator.CrossFade("null", 0f);
        contentPane.getItemUI.uiAnimator.Update(0f);

        contentPane.drawingUp.uiAnimator.CrossFade("null", 0f);
        contentPane.drawingUp.uiAnimator.Update(0f);

        contentPane.activateDrawingUI.uiAnimator.CrossFade("null", 0f);
        contentPane.activateDrawingUI.uiAnimator.Update(0f);

        contentPane.newBlueprint.uiAnimator.CrossFade("null", 0f);
        contentPane.newBlueprint.uiAnimator.Update(0f);
    }
    queueItem curritem;
    public void SetShowInfo(queueItem item)
    {
        if (!isShowing)
        {
            curritem = item;
            return;
        }
        curritem = null;
        animReset();
        currType = item.type;
        if (currType == ReceiveInfoUIType.AdvacedEquip)
        {
            UpdateInfo(item.equipuid, item.extend);
        }
        else if (currType == ReceiveInfoUIType.GetItem)
        {
            setItemInfo(item.itemid, item.equipid, item.count);
        }
        else if (currType == ReceiveInfoUIType.UnLockDrawing)
        {
            SetNewDrawingsInfo(item.equipid);
        }
        else if (currType == ReceiveInfoUIType.DrawingUpLv)
        {
            DrawingLvUP(item.equipid);
        }
        else if (currType == ReceiveInfoUIType.ActivateDrawing)
        {
            showActivateDrawingInfo(item.equipid);
        }
        else if (currType == ReceiveInfoUIType.DrawingStarUp)
        {
            equipStarUp(item.equipid);
        }
        setAnim(currType);

    }
    #region 高品质物品
    private string currEquipuid;
    public void UpdateInfo(string _equipuid, int extend)
    {
        AudioManager.inst.PlaySound(131);
        currEquipuid = _equipuid;
        EquipItem item = ItemBagProxy.inst.GetEquipItem(_equipuid);
        contentPane.getQualityItemUI.canWearHeroBtn.onClick.RemoveAllListeners();
        if (item == null)
        {
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
            return;
        }
        if (item.quality == 1)
            contentPane.getQualityItemUI.qualityFx.gameObject.SetActive(false);
        else
        {
            //qualityColorSprict
            contentPane.getQualityItemUI.qualityFx.gameObject.SetActive(true);
            var particlesyslist = contentPane.getQualityItemUI.qualityFx.GetComponentsInChildren<ParticleSystem>(true);
            var qualityColor = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[item.quality - 1]);
            foreach (var particle in particlesyslist)
            {
                particle.startColor = qualityColor;
                particle.Play();
            }
            //contentPane.getQualityItemUI.qualityFx.SetSprite("StaticIcon", StaticConstants.qualityColorSprict[item.quality - 1]);
        }
        //contentPane.getQualityItemUI.itemicon.SetSprite(item.equipConfig.equipDrawingsConfig.atlas, item.equipConfig.equipDrawingsConfig.icon, StaticConstants.qualityColor[item.quality - 1]);
        contentPane.getQualityItemUI.itemicon.SetSpriteURL(item.equipConfig.equipDrawingsConfig.big_icon, StaticConstants.qualityTxtColor[item.quality - 1], true);

        //contentPane.getQualityItemUI.costObj.SetActive(UserDataProxy.inst.playerData.equipImproveFreeCount <= 0);
        //contentPane.getQualityItemUI.freeObj.SetActive(UserDataProxy.inst.playerData.equipImproveFreeCount > 0);

        contentPane.getQualityItemUI.itemNameText.text = item.equipConfig.name;
        contentPane.getQualityItemUI.itemNameText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[item.equipConfig.equipQualityConfig.quality - 1]);
        contentPane.getQualityItemUI.qualityIcon.SetSprite("StaticIcon", StaticConstants.qualityicon[item.quality - 1]);
        contentPane.getQualityItemUI.priceText.text = GUIHelper.GetMoneyStr(item.sellPrice);
        contentPane.getQualityItemUI.levelText.text = item.equipConfig.equipDrawingsConfig.level.ToString();

        int quality = item.quality;

        if (item.quality > StaticConstants.SuperEquipBaseQuality)
        {
            quality -= StaticConstants.SuperEquipBaseQuality;
        }

        if (extend == 1 || quality >= 5)
        {
            Vector2 pos = contentPane.getQualityItemUI.okBtnAnimTf.anchoredPosition;
            pos.x = 0;
            contentPane.getQualityItemUI.okBtnAnimTf.anchoredPosition = pos;

            //最高
            contentPane.getQualityItemUI.upBtn.gameObject.SetActive(false);
            var apont = contentPane.getQualityItemUI.okBtn.GetComponent<RectTransform>().anchoredPosition;
            contentPane.getQualityItemUI.okBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, apont.y);
        }
        else
        {
            Vector2 pos = contentPane.getQualityItemUI.okBtnAnimTf.anchoredPosition;
            pos.x = -210;
            contentPane.getQualityItemUI.okBtnAnimTf.anchoredPosition = pos;

            contentPane.getQualityItemUI.upBtn.gameObject.SetActive(true);
            var apont = contentPane.getQualityItemUI.okBtn.GetComponent<RectTransform>().anchoredPosition;
            if (GameSettingManager.inst.needShowUIAnim)
                contentPane.getQualityItemUI.okBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-250, apont.y);
            contentPane.getQualityItemUI.nextQualitytext.text = LanguageManager.inst.GetValueByKey("升级至") + " " + "<color=" + StaticConstants.qualityTxtColor[item.quality] + ">" + LanguageManager.inst.GetValueByKey(StaticConstants.qualityNames[item.quality]) + "</color>";
            //contentPane.getQualityItemUI.nextQualitytext.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[item.quality]);
            contentPane.getQualityItemUI.needGemText.text = UserDataProxy.inst.playerData.equipImproveFreeCount > 0 ? LanguageManager.inst.GetValueByKey("免费") : item.equipConfig.equipQualityConfig.improve_quality_diamond.ToString();

            //contentPane.getQualityItemUI.needGemText.color = UserDataProxy.inst.playerData.equipImproveFreeCount > 0 ? Color.white : item.equipConfig.equipQualityConfig.improve_quality_diamond > UserDataProxy.inst.playerData.gem ? Color.red : Color.white;

            contentPane.getQualityItemUI.upBtn.onClick.AddListener(() =>
            {
                if (UserDataProxy.inst.playerData.equipImproveFreeCount <= 0 && UserDataProxy.inst.playerData.gem < item.equipConfig.equipQualityConfig.improve_quality_diamond)
                {
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, item.equipConfig.equipQualityConfig.improve_quality_diamond - UserDataProxy.inst.playerData.gem);
                    return;
                }
                contentPane.getQualityItemUI.upBtn.onClick.RemoveAllListeners();
                contentPane.getQualityItemUI.okBtn.onClick.RemoveAllListeners();
                contentPane.getQualityItemUI.canWearHeroBtn.onClick.RemoveAllListeners();
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.Equip_IMPROVING_QUALITY, currEquipuid);
            });
        }
        //if (extend == 1)
        EquipDataProxy.inst.needWait = true;
        contentPane.getQualityItemUI.okBtn.onClick.AddListener(() =>
        {
            contentPane.getQualityItemUI.upBtn.onClick.RemoveAllListeners();
            contentPane.getQualityItemUI.okBtn.onClick.RemoveAllListeners();
            contentPane.getQualityItemUI.canWearHeroBtn.onClick.RemoveAllListeners();
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        });
        contentPane.getQualityItemUI.canWearHeroBtn.onClick.AddListener(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_CanWearEquipTip", contentPane.getQualityItemUI.canWearHeroBtn.transform, item.equipConfig.equipDrawingsConfig.sub_type);
        });
        setEquipProperty();
        setEquipCanWearInfo(item.equipConfig.equipDrawingsConfig);
    }

    private void setEquipProperty()
    {
        var propertyList = EquipConfigManager.inst.GetEquipAllPropertyList(ItemBagProxy.inst.GetEquipItem(currEquipuid).equipid);
        int valCount = 0;
        for (int i = 0; i < contentPane.getQualityItemUI.allProperty.Count; i++)
        {
            int index = i;
            if (propertyList[index] > 0)
            {
                contentPane.getQualityItemUI.allProperty[index].gameObject.SetActive(true);
                contentPane.getQualityItemUI.allProperty[index].valText.text = "+" + propertyList[index].ToString();
                valCount++;
            }
            else
            {
                contentPane.getQualityItemUI.allProperty[index].gameObject.SetActive(false);
            }
        }
    }

    void setEquipCanWearInfo(EquipDrawingsConfig drawingsCfg)
    {
        KeyValuePair<string, string>[] heroProfessionArr = ItemBagProxy.inst.GetCanWearHeroInfosByEquip(drawingsCfg.sub_type, drawingsCfg.level, out int canWearFloorLv);

        contentPane.getQualityItemUI.tx_canWearFloorLv.text = LanguageManager.inst.GetValueByKey("穿戴等级：") + canWearFloorLv;

        for (int i = 0; i < contentPane.getQualityItemUI.canWearHeroProfessionIcons.objList.Count; i++)
        {
            GameObject obj = contentPane.getQualityItemUI.canWearHeroProfessionIcons.objList[i];

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

    #endregion
    #region  获得物品
    public void setItemInfo(int id, int equipid, long count)
    {
        AudioManager.inst.PlaySound(131);
        if (equipid != 0)
        {
            var equipConfig = EquipConfigManager.inst.GetEquipInfoConfig(equipid);

            contentPane.getItemUI.qualityFX.transform.DORotate(new Vector3(0, 0, -360 * 4), 20).SetEase(Ease.Linear).SetLoops(-1);
            var particlesyslist = contentPane.getQualityItemUI.qualityFx.GetComponentsInChildren<ParticleSystem>(true);
            var qualityColor = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[equipConfig.equipQualityConfig.quality - 1]);
            foreach (var particle in particlesyslist)
            {
                particle.startColor = qualityColor;
                particle.Play();
            }

            //装备类
            contentPane.getItemUI.desText.text = equipConfig.name + $" x{ count}";
            //contentPane.getItemUI.desText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityColor[item.quality - 1]);
            contentPane.getItemUI.qualityText.enabled = true;
            contentPane.getItemUI.qualityText.text = LanguageManager.inst.GetValueByKey(StaticConstants.qualityNames[equipConfig.equipQualityConfig.quality - 1]);
            contentPane.getItemUI.qualityText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[equipConfig.equipQualityConfig.quality - 1]);
            //contentPane.getItemUI.nameText.text = LanguageManager.inst.GetValueByKey(item.equipConfig.equipDrawingsConfig.name);
            //contentPane.getItemUI.numberText.text = "x" + count;
            contentPane.getItemUI.itemIcon.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.one * 512;
            contentPane.getItemUI.itemIcon.SetSpriteURL(equipConfig.equipDrawingsConfig.big_icon, StaticConstants.qualityTxtColor[equipConfig.equipQualityConfig.quality - 1], true);
        }
        else
        {
            //道具类
            Item item = ItemBagProxy.inst.GetItem(id);
            contentPane.getItemUI.desText.text = LanguageManager.inst.GetValueByKey(item.itemConfig.name) + $" x{ count}";
            //contentPane.getItemUI.desText.color = Color.white;
            contentPane.getItemUI.qualityText.enabled = false;
            //contentPane.getItemUI.nameText.text = LanguageManager.inst.GetValueByKey(item.itemConfig.name);
            //contentPane.getItemUI.numberText.text = "x" + count;
            contentPane.getItemUI.itemIcon.gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.one * 256;

            if (item.itemConfig.type == (int)ItemType.Furniture || item.itemConfig.type == (int)ItemType.Craftsman || item.itemConfig.type == (int)ItemType.ShopkeeperDress)
            {
                contentPane.getItemUI.itemIcon.SetSprite(item.itemConfig.atlas, item.itemConfig.icon);
            }
            else
            {
                contentPane.getItemUI.itemIcon.SetSpriteURL(item.itemConfig.icon);
            }

            contentPane.getItemUI.qualityFX.transform.DORotate(new Vector3(0, 0, -360 * 4), 20).SetEase(Ease.Linear).SetLoops(-1);
            var particlesyslist = contentPane.getQualityItemUI.qualityFx.GetComponentsInChildren<ParticleSystem>(true);
            var qualityColor = GUIHelper.GetColorByColorHex("F1DD3C");
            foreach (var particle in particlesyslist)
            {
                particle.startColor = qualityColor;
                particle.Play();
            }

        }



        var guideInfo = GuideDataProxy.inst.CurInfo;

        contentPane.getItemUI.okBtn.onClick.AddListener(() =>
        {
            contentPane.getItemUI.okBtn.onClick.RemoveAllListeners();
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);

            if (!guideInfo.isAllOver && (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.GetItemPanel)
            {
                guideInfo.isClickTarget = true;
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.FINGERACTIVEFALSE);
                GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickTarget, 0);
            }
        });
    }
    #endregion
    #region 图纸升级
    public void DrawingLvUP(int equipdrawingid)
    {
        AudioManager.inst.PlaySound(18);
        currEquipId = equipdrawingid;
        EquipData data = EquipDataProxy.inst.GetEquipData(equipdrawingid);
        EquipDrawingsConfig drawingscfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipdrawingid);
        contentPane.drawingUp.equipIcon.SetSpriteURL(drawingscfg.big_icon);
        progressItemInfo info = data.progresInfoList[data.progressLevel - 1];
        contentPane.drawingUp.milestoneDec.text = LanguageManager.inst.GetValueByKey(info.dec);
        GUIHelper.SetMilestonesIconText(info, ref contentPane.drawingUp.milestoneIcon, ref contentPane.drawingUp.valueTx);

        var img = contentPane.drawingUp.milestoneIcon.GetComponent<Image>();
        img.rectTransform.sizeDelta = Vector2.one * 128;

        bool isOver = data.progressLevel >= 5;

        contentPane.drawingUp.tx_title.text = isOver ? LanguageManager.inst.GetValueByKey("图纸升级！") : LanguageManager.inst.GetValueByKey("图纸已精通！");
        contentPane.drawingUp.img_master.enabled = isOver;
        //if (info.type < 2)
        //{
        //    img.rectTransform.sizeDelta /= 2;
        //}

        contentPane.drawingUp.okBtn.onClick.AddListener(() =>
        {
            contentPane.drawingUp.okBtn.onClick.RemoveAllListeners();
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        });
    }
    #endregion
    #region 新解锁图纸
    int currEquipId = 0;
    public void SetNewDrawingsInfo(int equipdrawingid)
    {
        AudioManager.inst.PlaySound(131);
        currEquipId = equipdrawingid;
        contentPane.newBlueprint.gameObject.SetActive(true);
        EquipData data = EquipDataProxy.inst.GetEquipData(equipdrawingid);
        EquipDrawingsConfig drawingscfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipdrawingid);
        contentPane.newBlueprint.nameText.text = LanguageManager.inst.GetValueByKey(drawingscfg.name);
        contentPane.newBlueprint.icon.SetSpriteURL(drawingscfg.big_icon);
        //contentPane.newBlueprint.currchipCountTx.text = UserDataProxy.inst.playerData.drawing.ToString();

        long drawingItemCount = UserDataProxy.inst.playerData.drawing;
        if (drawingscfg.activate_drawing > drawingItemCount)
            contentPane.newBlueprint.needChipCountTx.text = "<Color=#FF0000>" + drawingItemCount + "</Color>" + "/" + drawingscfg.activate_drawing;
        else
            contentPane.newBlueprint.needChipCountTx.text = "<Color=#4CF0FF>" + drawingItemCount + "</Color>" + "/" + drawingscfg.activate_drawing;
        //contentPane.newBlueprint.needChipCountTx.text =  drawingscfg.activate_drawing.ToString();
        //contentPane.newBlueprint.needChipCountTx.color = drawingscfg.activate_drawing > UserDataProxy.inst.playerData.drawing ? GUIHelper.GetColorByColorHex("FF0000") : GUIHelper.GetColorByColorHex("FFFFFF");

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            contentPane.newBlueprint.okBtn.gameObject.SetActive(false);
            contentPane.newBlueprint.studyBtn.gameObject.SetActive(false);
            contentPane.newBlueprint.guideBtn.gameObject.SetActive(true);
        }
        else
        {
            contentPane.newBlueprint.okBtn.gameObject.SetActive(true);
            contentPane.newBlueprint.studyBtn.gameObject.SetActive(true);
            contentPane.newBlueprint.guideBtn.gameObject.SetActive(false);
        }

        contentPane.newBlueprint.okBtn.onClick.AddListener(() =>
        {
            contentPane.newBlueprint.okBtn.onClick.RemoveAllListeners();
            contentPane.newBlueprint.studyBtn.onClick.RemoveAllListeners();
            contentPane.newBlueprint.guideBtn.onClick.RemoveAllListeners();
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        });
        contentPane.newBlueprint.studyBtn.onClick.AddListener(() =>
        {
            EquipDrawingsConfig _cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(currEquipId);
            if (_cfg.activate_drawing <= UserDataProxy.inst.playerData.drawing)
            {
                contentPane.newBlueprint.okBtn.onClick.RemoveAllListeners();
                contentPane.newBlueprint.studyBtn.onClick.RemoveAllListeners();
                contentPane.newBlueprint.guideBtn.onClick.RemoveAllListeners();
                EventController.inst.RemoveListener(GameEventType.EquipEvent.EQUIP_SHOWREFRESH, ToMsgNext);
                EventController.inst.AddListener(GameEventType.EquipEvent.EQUIP_SHOWREFRESH, ToMsgNext);
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_UNLOCKEQUIP, currEquipId);
            }
            else
            {
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 26);
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您的图纸碎片不足"), GUIHelper.GetColorByColorHex("FF2828"));
            }
        });
        contentPane.newBlueprint.guideBtn.onClick.AddListener(() =>
        {
            contentPane.newBlueprint.okBtn.onClick.RemoveAllListeners();
            contentPane.newBlueprint.studyBtn.onClick.RemoveAllListeners();
            contentPane.newBlueprint.guideBtn.onClick.RemoveAllListeners();
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        });

    }

    private void ToMsgNext()
    {
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);

        EventController.inst.RemoveListener(GameEventType.EquipEvent.EQUIP_SHOWREFRESH, ToMsgNext);
    }
    #endregion
    #region  已激活
    public void showActivateDrawingInfo(int equipdrawingid)
    {
        AudioManager.inst.PlaySound(67);
        currEquipId = equipdrawingid;
        EquipData equipdata = EquipDataProxy.inst.GetEquipData(equipdrawingid);
        var guideInfo = GuideDataProxy.inst.CurInfo;
        if (equipdata != null)
        {
            EquipDrawingsConfig drawingscfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipdrawingid);
            contentPane.activateDrawingUI.equipIcon.SetSpriteURL(drawingscfg.big_icon);
            contentPane.activateDrawingUI.equipNameTx.text = LanguageManager.inst.GetValueByKey(drawingscfg.name);

            contentPane.activateDrawingUI.okBtn.onClick.AddListener(() =>
            {
                contentPane.activateDrawingUI.okBtn.onClick.RemoveAllListeners();
                contentPane.activateDrawingUI.infoBtn.onClick.RemoveAllListeners();
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);

                if (!guideInfo.isAllOver && (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.ResearchEquip)
                {
                    guideInfo.isClickTarget = true;
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.FINGERACTIVEFALSE);
                    GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickTarget, 0);
                }
            });
            contentPane.activateDrawingUI.infoBtn.onClick.AddListener(() =>
            {
                if (GuideDataProxy.inst == null || GuideDataProxy.inst.CurInfo == null || !GuideDataProxy.inst.CurInfo.isAllOver) return;
                contentPane.activateDrawingUI.okBtn.onClick.RemoveAllListeners();
                contentPane.activateDrawingUI.infoBtn.onClick.RemoveAllListeners();
                EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
                if (guideInfo.isAllOver && (K_Guide_Type)guideInfo.m_curCfg.guide_type != K_Guide_Type.ResearchEquip)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUI, currEquipId, new List<EquipData>());
                }
            });
        }
        else
        {
            Logger.warning($"为找到{equipdrawingid}对应的图纸信息");
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
        }

        //LayoutRebuilder.ForceRebuildLayoutImmediate(contentPane.activateDrawingUI.centerRect);
    }
    #endregion

    #region 图纸升星
    private void equipStarUp(int equipDrawingId)
    {
        AudioManager.inst.PlaySound(131);
        currEquipId = equipDrawingId;
        contentPane.drawingStarUp.gameObject.SetActive(true);
        EquipData data = EquipDataProxy.inst.GetEquipData(equipDrawingId);
        EquipDrawingsConfig drawingscfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipDrawingId);
        contentPane.drawingStarUp.icon_item.SetSpriteURL(drawingscfg.big_icon);
        //contentPane.drawingStarUp.obj_bottom.SetActive(false);
        for (int i = 0; i < contentPane.drawingStarUp.starIcons.Length; i++)
        {
            contentPane.drawingStarUp.starIcons[i].SetSprite("PlayerUp_atlas", data.starLevel > i ? "feixu_xingxing2" : "feixu_xingxing1");
        }
        //contentPane.drawingStarUp.obj_stars.SetActive(true);
        var curStarUpProgressItemInfo = drawingscfg.GetStarUpProgressItemInfos()[data.starLevel - 1];

        contentPane.drawingStarUp.tx_starUpTitle.text = LanguageManager.inst.GetValueByKey(curStarUpProgressItemInfo.des);
        contentPane.drawingStarUp.icon_starUpEffect.SetSprite(curStarUpProgressItemInfo.atlas, curStarUpProgressItemInfo.iconName, needSetNativeSize: true);
        contentPane.drawingStarUp.tx_starUpValue.text = LanguageManager.inst.GetValueByKey(curStarUpProgressItemInfo.title); //"+" + (curStarUpProgressItemInfo.value / 100) + "%";

        //GameTimer.inst.AddTimer(1f, 1, () => 
        //{
        //    contentPane.drawingStarUp.obj_stars.SetActive(false);
        //    contentPane.drawingStarUp.obj_bottom.SetActive(true);
        //});

    }

    #endregion

}
