using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EquipInfoUIView : ViewBase<EquipInfoUIComp>
{
    public override string viewID => ViewPrefabName.EquipInfoUI;

    public override string sortingLayerName => "popup";

    private int _curIndex = 0;
    private EquipData currData;
    public int sub_type;
    private List<EquipData> dataList = null;
    private int _curNeedStarUpItemNum = 0;

    protected override void onInit()
    {
        isShowResPanel = true;

        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.favoriteBtn.onValueChanged.AddListener((value) =>
        {
            AudioManager.inst.PlaySound(8);
            if ((currData.favorite == 1) != value)
            {
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_FAVORITE, currData.equipDrawingId, value);
            }
        });

        contentPane.leftBtn.ButtonClickTween(() =>
        {
            EquipData frontequip = GetFrontEquipItem(currData.equipDrawingId);
            //前一个
            if (frontequip != null)
                ShowInfo(frontequip, dataList);
        });
        contentPane.rightBtn.ButtonClickTween(() =>
        {
            EquipData nextequip = GetNextEquipItem(currData.equipDrawingId);
            //下一个
            if (nextequip != null)
                ShowInfo(nextequip, dataList);
        });

        contentPane.infoMakeButton.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_START, currData.equipDrawingId);
        });

        contentPane.infoUnlockButton.ButtonClickTween(() =>
        {
            EquipDrawingsConfig drawingsCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(currData.equipDrawingId);

            if (currData.equipDrawingId > 0)
            {
                if (UserDataProxy.inst.playerData.drawing < drawingsCfg.activate_drawing)
                {
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 26);
                }
                else
                {
                    EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_UNLOCKEQUIP, currData.equipDrawingId);
                    hide();
                }
            }
        });

        contentPane.progressMakeButton.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_START, currData.equipDrawingId);
        });

        contentPane.btn_starUp.ButtonClickTween(onStarUpBtnClick);

        contentPane.briefInfoBtn.ButtonClickTween(onBriefInfoBtnClick);
        contentPane.briefInfoBgBtn.onClick.AddListener(onBriefBgClick);

        contentPane.canWearHeroBtn.onClick.AddListener(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_CanWearEquipTip", contentPane.canWearHeroBtn.transform, sub_type);
        });

        contentPane.toggleGroup.OnSelectedIndexValueChange = onSelectedIndexValueChange;
        contentPane.toggleGroup.SetToggleSize(new Vector2(370, 120), new Vector2(340, 120));

        //升星材料固定itemId
        itemConfig itemConfig = ItemconfigManager.inst.GetConfig(StaticConstants.EquipStarUpItemId);
        if (itemConfig != null)
        {
            contentPane.useItemIcon.SetSprite(itemConfig.atlas, itemConfig.icon);
        }

    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.topAnimator.CrossFade("show", 0f);
        contentPane.topAnimator.Update(0f);
        contentPane.topAnimator.Play("show");

        contentPane.windowAnimator.CrossFade("show", 0f);
        contentPane.windowAnimator.Update(0f);
        contentPane.windowAnimator.Play("show");

        //contentPane.leftBtn.GetComponent<Graphic>().color = new Color(1, 1, 1, 0);
        //contentPane.rightBtn.GetComponent<Graphic>().color = new Color(1, 1, 1, 0);
        //contentPane.mask.color = new Color(1, 1, 1, 0.6f);

        //GameTimer.inst.AddTimer(0.2f, 1, () =>
        //{
        //    contentPane.leftBtn.GetComponent<Graphic>().FadeFromTransparentTween(1f, 0.5f);
        //    contentPane.rightBtn.GetComponent<Graphic>().FadeFromTransparentTween(1f, 0.5f);
        //    contentPane.mask.FadeFromTransparentTween(0.97f, 0.3f);
        //});
    }

    protected override void DoHideAnimation()
    {
        contentPane.topAnimator.Play("hide");
        contentPane.windowAnimator.Play("hide");


        float animLength = contentPane.windowAnimator.GetClipLength("window_hide");

        //Graphic[] topGraphics = contentPane.topAnimator.transform.parent.GetComponentsInChildren<Graphic>();
        //foreach (var item in topGraphics) item.FadeTransparentTween(1, animLength);
        //contentPane.mask.FadeTransparentTween(0.97f, animLength);


        GameTimer.inst.AddTimer(animLength - 0.01f, 1, () =>
        {
            contentPane.topAnimator.CrossFade("null", 0f);
            contentPane.topAnimator.Update(0f);
            contentPane.windowAnimator.CrossFade("null", 0f);
            contentPane.windowAnimator.Update(0f);
            this.HideView();
        });
    }

    protected override void onHide()
    {
        base.onHide();
        //AudioManager.inst.PlaySound(11);
        currData = null;
        dataList = null;
        _curNeedStarUpItemNum = 0;

        contentPane.briefInfoBgBtn.gameObject.SetActiveFalse();
        contentPane.briefInfoObj.SetActiveFalse();
    }

    private void onSelectedIndexValueChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        _curIndex = index;

        foreach (var item in contentPane.toggleLinkObjs)
        {
            item.SetActive(false);
        }

        contentPane.toggleLinkObjs[index].SetActive(true);
    }

    public EquipData GetFrontEquipItem(int id)
    {
        if (dataList == null || dataList.Count <= 1) return null;
        int index = dataList.FindIndex(item => item.equipDrawingId == id);
        if (index <= 0)
        {
            return dataList[dataList.Count - 1];
        }
        else
        {
            return dataList[index - 1];
        }
    }

    public EquipData GetNextEquipItem(int id)
    {
        if (dataList == null || dataList.Count <= 1) return null;
        int index = dataList.FindIndex(item => item.equipDrawingId == id);
        if (index >= dataList.Count - 1)
        {
            return dataList[0];
        }
        else
        {
            return dataList[index + 1];
        }
    }

    void setEquipBaseInfo(EquipDrawingsConfig drawingsCfg)
    {
        contentPane.equiptyName.text = LanguageManager.inst.GetValueByKey(drawingsCfg.name);
        contentPane.equiptyLevel.text = drawingsCfg.level.ToString();
        contentPane.equipIcon.SetSpriteURL(drawingsCfg.big_icon);

        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(drawingsCfg.sub_type);
        contentPane.equipTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);
        contentPane.equipSubTypeText.text = drawingsCfg.level + LanguageManager.inst.GetValueByKey("阶") + LanguageManager.inst.GetValueByKey(EquipConfigManager.inst.GetEquipTypeByID(drawingsCfg.sub_type).name);
        //contentPane.priceText.text = Mathf.FloorToInt((currData.sellPrice * currData.sellAddition)).ToString();
        contentPane.shuomingTx.text = LanguageManager.inst.GetValueByKey(drawingsCfg.desc);

    }

    //信息面板
    void setInfoPanel(EquipDrawingsConfig drawingsCfg)
    {
        // 信息面板
        for (int i = 0; i < contentPane.needWorkerList.Count; i++)
        {
            needWorker item = contentPane.needWorkerList[i];

            if (i < drawingsCfg.artisan_id.Length)
            {
                item.gameObject.SetActive(true);
                var workerCfg = WorkerConfigManager.inst.GetConfig(drawingsCfg.artisan_id[i]);
                if (workerCfg != null)
                    item.workerIcon.SetSprite("portrait_atlas", workerCfg.icon);
                item.workerLv.text = LanguageManager.inst.GetValueByKey("{0}级", drawingsCfg.artisan_lv[i].ToString());
                var worker = RoleDataProxy.inst.GetWorker(workerCfg.id);
                item.workerLv.color = worker.state == EWorkerState.Unlock && worker.level >= drawingsCfg.artisan_lv[i] ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("cc2201");
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

    }

    //所需资源
    void setResInfo(EquipDrawingsConfig drawingsCfg)
    {
        foreach (var obj in contentPane.needResList)
        {
            obj.gameObject.SetActive(false);
        }

        var needRes = drawingsCfg.GetNeedMaterialsInfos();

        int i;

        for (i = 0; i < needRes.Length; i++)
        {
            needMaterialsInfo info = needRes[i];
            if (info.type == 0)
            {
                itemConfig rescfg = ItemconfigManager.inst.GetConfig(info.needId);
                contentPane.needResList[i].gameObject.SetActive(true);
                contentPane.needResList[i].setData(rescfg.atlas, rescfg.icon, info.needCount, 0, ItemBagProxy.inst.resItemCount(info.needId) >= info.needCount);
            }
        }
        if (drawingsCfg.component1_type > 0)
        {
            if (drawingsCfg.component1_type == 1) //装备
            {
                EquipConfig equipcfg = EquipConfigManager.inst.GetEquipInfoConfig(drawingsCfg.component1_id);
                if (equipcfg != null)
                {
                    contentPane.needResList[i].gameObject.SetActive(true);
                    contentPane.needResList[i].setData(equipcfg.equipDrawingsConfig.atlas, equipcfg.equipDrawingsConfig.icon, ItemBagProxy.inst.getEquipNumberBySuperQuip(drawingsCfg.component1_id), drawingsCfg.component1_num, ItemBagProxy.inst.getEquipNumberBySuperQuip(drawingsCfg.component1_id) >= drawingsCfg.component1_num, equipcfg.equipQualityConfig.quality);
                }
            }
            else if (drawingsCfg.component1_type == 2)    //  特殊资源
            {
                itemConfig rescfg = ItemconfigManager.inst.GetConfig(drawingsCfg.component1_id);
                contentPane.needResList[i].gameObject.SetActive(true);
                contentPane.needResList[i].setData(rescfg.atlas, rescfg.icon, (int)ItemBagProxy.inst.resItemCount(drawingsCfg.component1_id), drawingsCfg.component1_num, ItemBagProxy.inst.resItemCount(drawingsCfg.component1_id) >= drawingsCfg.component1_num);
            }
        }
        i++;
        if (drawingsCfg.component2_type > 0)
        {
            if (drawingsCfg.component2_type == 1) //装备
            {
                EquipConfig equipcfg = EquipConfigManager.inst.GetEquipInfoConfig(drawingsCfg.component2_id);
                if (equipcfg != null)
                {
                    contentPane.needResList[i].gameObject.SetActive(true);
                    contentPane.needResList[i].setData(equipcfg.equipDrawingsConfig.atlas, equipcfg.equipDrawingsConfig.icon, ItemBagProxy.inst.getEquipNumberBySuperQuip(drawingsCfg.component2_id), drawingsCfg.component2_num, ItemBagProxy.inst.getEquipNumberBySuperQuip(drawingsCfg.component2_id) >= drawingsCfg.component2_num, equipcfg.equipQualityConfig.quality);
                }
            }
            else if (drawingsCfg.component2_type == 2)    //  特殊资源
            {
                itemConfig rescfg = ItemconfigManager.inst.GetConfig(drawingsCfg.component2_id);
                contentPane.needResList[i].gameObject.SetActive(true);
                contentPane.needResList[i].setData(rescfg.atlas, rescfg.icon, (int)ItemBagProxy.inst.resItemCount(drawingsCfg.component2_id), drawingsCfg.component2_num, ItemBagProxy.inst.resItemCount(drawingsCfg.component2_id) >= drawingsCfg.component2_num);
            }
        }
    }

    //进度
    void setProgressInfo(int progressLevel, int beenMake, progressItemInfo[] progressItemInfoArr)
    {
        int index = 0;
        //进度面板
        foreach (var jt in contentPane.jiantou)
        {
            if (index < progressLevel)
            {
                jt.isOn = true;
            }
            else
            {
                jt.isOn = false;
            }
            index++;
        }

        for (int k = 0; k < 5; k++)
        {
            float progress = progressLevel > k ? 1 : 0;
            if (progressLevel == k)
            {
                int fcount = k > 0 ? progressItemInfoArr[k - 1].exp : 0;
                progress = (float)(beenMake - fcount) / (float)(progressItemInfoArr[k].exp - fcount);
            }
            contentPane.equipInfoPlaneList[k].setInfo(k, progressItemInfoArr[k], progress);
        }

    }

    public void ShowInfo(int equipDrawingId)
    {
        //lua侧 刷新
        LuaListItem luaListItem = contentPane.GetComponent<LuaListItem>();

        if (luaListItem != null)
        {
            luaListItem.SetData(equipDrawingId);
        }

        contentPane.toggleGroup.OnEnableMethod(_curIndex);

        contentPane.leftBtn.gameObject.SetActive(false);
        contentPane.rightBtn.gameObject.SetActive(false);
        contentPane.infoMakeButton.gameObject.SetActive(false);
        contentPane.progressMakeButton.gameObject.SetActive(false);
        contentPane.infoUnlockButton.gameObject.SetActive(false);
        contentPane.infoCoolTimeTx.gameObject.SetActive(false);

        EquipData data = EquipDataProxy.inst.GetEquipData(equipDrawingId);
        contentPane.tip_needLearn.enabled = data == null;


        EquipDrawingsConfig drawingsCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipDrawingId);
        sub_type = drawingsCfg.sub_type;

        setEquipBaseInfo(drawingsCfg);

        contentPane.favoriteBtn.gameObject.SetActive(false);

        setInfoPanel(drawingsCfg);
        // 信息小面板
        EquipQualityConfig cfg = EquipConfigManager.inst.GetEquipQualityConfig(equipDrawingId, 1);
        setBriefPanelInfo(cfg.price_gold, equipDrawingId, cfg.id);
        setResInfo(drawingsCfg);

        setProgressInfo(0, 0, drawingsCfg.GetProgressItemInfos());
        setStarUpInfo(false, 0, drawingsCfg.GetStarUpProgressItemInfos());

        setEquipCanWearInfo(drawingsCfg);
    }

    //设置升星信息
    void setStarUpInfo(bool isActive, int starUpProgressLv, starUpProgressItemInfo[] starUpProgressItemInfos)
    {

        //进度
        for (int i = 0; i < contentPane.starUpToggles.Length; i++)
        {
            var toggle = contentPane.starUpToggles[i];
            if (i < starUpProgressLv)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
        }

        for (int k = 0; k < 3; k++)
        {
            contentPane.equipStarUpProgresses[k].SetData(starUpProgressItemInfos[k], starUpProgressLv > k);
            contentPane.starIcons[k].SetSprite("equipMakeUI_atlas", starUpProgressLv >= k ? "yingxiong_xingcheng" : "yingxiong_xingcheng1");

            if (starUpProgressLv == k) //当前要升星的阶段
            {
                _curNeedStarUpItemNum = starUpProgressItemInfos[k].needNum;
                Item item = ItemBagProxy.inst.GetItem(StaticConstants.EquipStarUpItemId);
                int curNum = 0;
                if (item != null) curNum = (int)item.count;
                string colorStr = curNum >= _curNeedStarUpItemNum ? "#ffffff" : "#ff2828";
                contentPane.tx_needStarUpItemNum.text = "<color=" + colorStr + ">" + curNum + "</color><color=#f39b10>/" + _curNeedStarUpItemNum + "</color>";
            }
        }

        contentPane.btn_starUp.gameObject.SetActive(isActive);

        if (starUpProgressLv >= 3)
        {
            contentPane.btn_starUp.gameObject.SetActive(false);
            contentPane.tip_starUpOver.enabled = true;
        }
        else
        {
            contentPane.tip_starUpOver.enabled = false;
        }

    }

    void onStarUpBtnClick()
    {
        Item item = ItemBagProxy.inst.GetItem(StaticConstants.EquipStarUpItemId);
        int curNum = 0;
        if (item != null) curNum = (int)item.count;

        if (curNum >= _curNeedStarUpItemNum)
        {
            EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_STARUP, currData.equipDrawingId);
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("升星道具不足"), GUIHelper.GetColorByColorHex("FF2828"));
        }
    }

    public void ShowInfo(EquipData data, List<EquipData> datalist)
    {
        //lua侧 刷新
        LuaListItem luaListItem = contentPane.GetComponent<LuaListItem>();

        if (luaListItem != null)
        {
            luaListItem.SetData(data.equipDrawingId);
        }

        dataList = datalist;
        currData = data;
        sub_type = data.subType;
        contentPane.toggleGroup.OnEnableMethod(_curIndex);

        contentPane.leftBtn.gameObject.SetActive(dataList != null && dataList.Count > 1);
        contentPane.rightBtn.gameObject.SetActive(dataList != null && dataList.Count > 1);
        EquipDrawingsConfig drawingsCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(currData.equipDrawingId);
        EquipData equipinfodata = EquipDataProxy.inst.GetEquipData(currData.equipDrawingId);

        contentPane.infoMakeButton.gameObject.SetActive(equipinfodata == null || equipinfodata.equipState == 2);
        contentPane.infoUnlockButton.gameObject.SetActive(equipinfodata == null || equipinfodata.equipState == 1);
        contentPane.progressMakeButton.gameObject.SetActive(equipinfodata == null || equipinfodata.equipState == 2);
        contentPane.tip_needLearn.enabled = false;

        string colorStr = UserDataProxy.inst.playerData.drawing >= drawingsCfg.activate_drawing ? "4cf0ff" : "#fd4f4f";
        contentPane.infoUnlockCountText.text = "<Color=#" + colorStr + ">" + UserDataProxy.inst.playerData.drawing + "</Color>" + "/" + drawingsCfg.activate_drawing;

        double maketime = equipinfodata.makeTime; //目前直接拿后端下发的时间 后端时间是计算完毕之后的

        //int subTime = 0;
        //float val = 0;

        ////工匠加成
        //foreach (var id in drawingsCfg.artisan_id)
        //{
        //    var worker = RoleDataProxy.inst.GetWorker(id);
        //    if (worker != null)
        //    {
        //        val += (worker.addSpeed * 0.01f);
        //    }
        //}
        //subTime += Mathf.RoundToInt((float)maketime * val);

        ////---家具加成
        ////小类型
        //val = FurnitureBuffMgr.inst.GetBuffValByType(FurnitureBuffType.make_subTypeSpeedUp, currData.subType);
        //subTime += Mathf.RoundToInt((float)maketime * val);

        ////全部类型
        //val = FurnitureBuffMgr.inst.GetBuffValByType(FurnitureBuffType.sell_allPriceUp);
        //subTime += Mathf.RoundToInt((float)maketime * val);
        ////---

        ////公会buff加成
        //var make_equipSpeedUpUnionBuff = UserDataProxy.inst.GetUnionBuffData(EUnionScienceType.AccelSkill);
        //if (make_equipSpeedUpUnionBuff != null)
        //{
        //    val = make_equipSpeedUpUnionBuff.config.add2_num / 100f;
        //    subTime += Mathf.RoundToInt((float)maketime * val);
        //}

        ////全服buff加成
        //var make_equipSpeedUpBuff = GlobalBuffDataProxy.inst.GetGlobalBuffData(GlobalBuffType.make_equipSpeedUp);
        //if (make_equipSpeedUpBuff != null)
        //{
        //    val = make_equipSpeedUpBuff.buffInfo.buffParam / 100f;
        //    subTime += Mathf.RoundToInt((float)maketime * val);
        //}

        contentPane.infoCoolTimeTx.text = contentPane.progressCoolTimeTx.text = TimeUtils.timeSpanStrip((int)maketime/* - subTime*/, false);

        setEquipBaseInfo(drawingsCfg);

        foreach (var obj in contentPane.needResList)
        {
            obj.gameObject.SetActive(false);
        }

        contentPane.favoriteBtn.gameObject.SetActive(true);
        contentPane.favoriteBtn.isOn = data.favorite == 1;

        setInfoPanel(drawingsCfg);
        // 信息小面板
        EquipQualityConfig cfg = EquipConfigManager.inst.GetEquipQualityConfig(currData.equipDrawingId, 1);
        setBriefPanelInfo(currData.sellPrice, currData.equipDrawingId, cfg.id);
        setResInfo(drawingsCfg);

        setProgressInfo(data.progressLevel, data.beenMake, drawingsCfg.GetProgressItemInfos());
        setStarUpInfo(data.equipState == 2, data.starLevel, drawingsCfg.GetStarUpProgressItemInfos());

        setEquipCanWearInfo(drawingsCfg);
    }

    void setEquipCanWearInfo(EquipDrawingsConfig drawingsCfg)
    {
        KeyValuePair<string, string>[] heroProfessionArr = ItemBagProxy.inst.GetCanWearHeroInfosByEquip(drawingsCfg.sub_type, drawingsCfg.level, out int canWearFloorLv);
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

    public void RefreshStarUpInfo()
    {
        if (currData != null)
        {
            EquipDrawingsConfig drawingsCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(currData.equipDrawingId);
            setStarUpInfo(true, currData.starLevel, drawingsCfg.GetStarUpProgressItemInfos());
        }
    }

    #region 装备信息简略面板

    void onBriefInfoBtnClick()
    {
        contentPane.briefInfoObj.SetActiveTrue();
        contentPane.briefInfoBgBtn.gameObject.SetActiveTrue();
    }

    void onBriefBgClick()
    {
        contentPane.briefInfoObj.SetActiveFalse();
        contentPane.briefInfoBgBtn.gameObject.SetActiveFalse();
    }

    void setBriefPanelInfo(int sellPrice, int equipDrawingId, int equipQualityId)
    {
        //先来个金币
        contentPane.goldTx.text = sellPrice.ToString("N0");

        //然后是装备属性
        var propertyList = EquipConfigManager.inst.GetEquipAllPropertyList(equipQualityId);
        for (int i = 0; i < contentPane.equipPropertyItems.Count; i++)
        {
            int index = i;
            if (propertyList[index] > 0)
            {
                contentPane.equipPropertyItems[index].gameObject.SetActive(true);
                contentPane.equipPropertyItems[index].valText.text = propertyList[index].ToString();
            }
            else
            {
                contentPane.equipPropertyItems[index].gameObject.SetActive(false);
            }
        }

        //仓库里的数量
        contentPane.briefStoreTx.text = ItemBagProxy.inst.getEquipAllNumber(equipDrawingId).ToString();
    }

    #endregion

}
