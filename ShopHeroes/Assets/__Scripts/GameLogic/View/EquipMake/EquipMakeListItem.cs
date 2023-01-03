using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class EquipMakeListItem : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    private int id;  //装备图纸id
    private EquipData currData;
    /// /////////////////////////////////////////////////////////////////
    [Header("------------")]
    public Button selfBtn;
    public Image bg_1;
    public Image bg_2;
    public Text tx_master;
    public Image bg_3;
    public Image iconBg_1;
    public Image iconBg_2;
    public Image iconBg_3;
    public GUIIcon equipIcon;
    public GUIIcon infoIcon;
    public GUIIcon collectIcon;
    public Image nameBg_1;
    public Image nameBg_2;
    public Image nameBg_3;
    public Text nameText;
    public GUIIcon subTypeIcon;
    public Text levelText;
    public Transform needResList;
    public needRes[] needResItem;
    public GUIIcon nextStageBgIcon;
    public GUIIcon nextStageIcon;
    public Slider targetBar;
    public Text targetBarText;
    public Text numberText;
    public GUIIcon unLockItemIcon;
    public Button equipInfoBtn;
    public GUIIcon unLockNeedIcon;
    public Text lockText;
    public Toggle favoriteBtn;

    public Transform needDrawingTf;
    public Text needDrawingcountTx;

    public GUIIcon bottomBgIcon;
    public Transform levelTf;

    public System.Action<int> itemOnclick;

    public GameObject highLightVFXObj;

    [Header("--工匠未解锁---")]
    public GUIIcon workerIcon;
    public GameObject workerLockObj;
    public Text workerLockTx;
    public Text workerLevelTx;
    [Header("--工匠等级未满足---")]
    public GameObject workerneedlevelobj;
    public GUIIcon workerIcon2;
    public Text needWorkerlevel;
    [Header("--宝箱获得图纸---")]
    public GUIIcon boxIcon;
    public GameObject boxLockObj;
    public Text boxLockTx;
    //public Text workerLevelTx;


    [Header("--转盘获得图纸---")]
    public GUIIcon drawIcon;
    public GameObject drawLockObj;
    public Text drawLockTx;
    //public Text workerLevelTx;


    [Header("--动画--")]
    public Animator animator;

    [Header("--制作队列--")]
    public GameObject makeNode;
    public Image barImage;
    public Text barText;

    [Header("--升星--")]
    public GUIIcon[] starIcons;

    [Header("--------------------------")]
    public Transform newTip;

    string itemName;
    public void init()
    {
        // selfBtn.onClick.AddListener(onClick);

        equipInfoBtn.onClick.AddListener(() =>
        {
            if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver) return;
            if (itemOnclick != null)
            {
                itemOnclick.Invoke(currData.equipDrawingId);
            }
        });

        favoriteBtn.onValueChanged.AddListener((value) =>
        {
            if (currData != null && (currData.favorite == 1) != value)
            {
                EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_FAVORITE, currData.equipDrawingId, value);
            }
        });
        EventController.inst.AddListener(GameEventType.ProductionEvent.PRODUCTIONLIST_STATE_Change, UpdateMakeState);
        EventController.inst.AddListener<int>(GameEventType.EquipEvent.EQUIP_STARUPSUCCESS, setStarUpInfo);
    }
    private void Awake()
    {
        init();
    }
    //
    #region 当前图纸制作状态 
    EquipMakerSlot currSlot = null;
    public void UpdateMakeState()
    {
        if (this.enabled == false) return;

        if (currSlot != null)
        {
            currSlot.coolTimeChange = null;
            currSlot = null;
        }
        var slotList = EquipDataProxy.inst.equipSlotList;
        List<EquipMakerSlot> rawingList = slotList.FindAll(slot => slot.equipDrawingId == this.id && slot.makeState == 1);

        if (rawingList.Count <= 0)
        {
            this.makeNode.SetActive(false);
            barText.text = "";
        }
        else
        {
            foreach (var slot in rawingList)
            {
                if (slot.makeState == 1 && slot.currTime > 0)
                {
                    if (currSlot == null)
                    {
                        currSlot = slot;
                    }
                    else
                    {
                        if (slot.currTime < currSlot.currTime)
                        {
                            currSlot = slot;
                        }
                    }
                }
            }
            if (currSlot == null || currSlot.currTime <= 0)
            {
                this.makeNode.SetActive(false);
                barText.text = "";
                return;
            }
            this.makeNode.SetActive(true);
            currSlot.coolTimeChange = timechange;
            barText.text = rawingList.Count.ToString();
            barImage.fillAmount = 1.0f - (float)(currSlot.currTime / currSlot.totalTime);
        }
    }

    private void timechange(int equipDrawingId, float ctime)
    {
        if (id != equipDrawingId) return;
        if (ctime == 0)
        {
            UpdateMakeState();
            return;
        }
        if (currSlot != null)
        {
            barImage.fillAmount = 1.0f - (float)(currSlot.currTime / currSlot.totalTime);
        }
    }
    #endregion
    //
    //设置没有解锁的
    public void setNotUnLockDate(int _equipdrawingid, int index, bool needShowAni)
    {
        highLightVFXObj.SetActiveFalse(); //高亮关闭
        if (gameObject.activeInHierarchy)
        {
            gameObject.name = _equipdrawingid.ToString();
            itemName = _equipdrawingid.ToString();
        }

        infoIcon.SetSprite("__common_1", "cangku_tishi");
        collectIcon.SetSprite("equipMakeUI_atlas", "zhizuo_xiaoshoucang1");
        GUIHelper.SetUIGray(levelTf, true);
        isShowAnimed = false;
        id = _equipdrawingid;
        bg_1.enabled = true;
        bg_2.enabled = false;
        tx_master.enabled = false;
        bg_3.enabled = false;
        iconBg_1.enabled = true;
        iconBg_2.enabled = false;
        iconBg_3.enabled = false;
        nameBg_1.enabled = true;
        nameBg_2.enabled = false;
        nameBg_3.enabled = false;
        unLockItemIcon.enabled = true;
        nextStageIcon.gameObject.SetActive(false);
        nextStageBgIcon.gameObject.SetActive(false);
        equipInfoBtn.gameObject.SetActive(false);
        needResList.gameObject.SetActive(true);
        unLockNeedIcon.gameObject.SetActive(true);
        needResList.gameObject.SetActive(false);
        needDrawingTf.gameObject.SetActive(false);
        lockText.gameObject.SetActive(true);
        makeNode.SetActive(false);
        workerLockObj.SetActive(false);
        boxLockObj.SetActive(false);
        drawLockObj.SetActive(false);
        workerneedlevelobj.SetActive(false);
        newTip.gameObject.SetActive(false);
        barText.text = "";
        foreach (var item in starIcons)
        {
            item.gameObject.SetActive(false);
        }
        //targetBar.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().sprite = ManagerBinder.inst.Asset.getSprite("NewEquipMake_atlas.spriteatlas", "zhizuo_jindut");
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(_equipdrawingid);
        nameText.text = LanguageManager.inst.GetValueByKey(cfg.name);

        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(cfg.sub_type);
        subTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);

        levelText.text = cfg.level.ToString();
        if (ItemBagProxy.inst.getEquipAllNumber(cfg.id) > 0)
        {
            numberText.text = "x" + ItemBagProxy.inst.getEquipAllNumber(cfg.id).ToString();
        }
        else
        {
            numberText.transform.parent.gameObject.SetActive(false);
        }
        equipIcon.SetSprite(cfg.atlas, cfg.icon);
        string desText = LanguageManager.inst.GetValueByKey(cfg.unlock_dec);
        desText = desText.Replace("\\n", "\n");
        lockText.text = LanguageManager.inst.GetValueByKey(desText);

        lockText.resizeTextForBestFit = true;

        currData = null;

        bottomBgIcon.gameObject.SetActive(true);
        bottomBgIcon.SetSprite("__common_1", cfg.unlock_type == 1 ? "zhizuo_lichengbeidi2" : "zhizuo_lichengbeidi1");

        if (cfg.unlock_type == 1)//获得指定工匠解锁
        {
            targetBar.gameObject.SetActive(false);
            unLockNeedIcon.gameObject.SetActive(false);
            lockText.enabled = false;
            workerLockObj.gameObject.SetActive(true);

            int workerId = Array.Find<int>(cfg.artisan_id, t => RoleDataProxy.inst.GetWorker(t).state != EWorkerState.Unlock);
            if (workerId > 0) //说明有未解锁的
            {
                workerLevelTx.transform.parent.gameObject.SetActive(false);
                WorkerConfig workerConfig = WorkerConfigManager.inst.GetConfig(workerId);
                workerIcon.SetSprite(StaticConstants.roleHeadIconAtlasName, workerConfig.icon);
                workerLockTx.text = LanguageManager.inst.GetValueByKey("招募") + LanguageManager.inst.GetValueByKey(workerConfig.name);
            }
            else //工匠等级不足
            {
                workerId = Array.Find<int>(cfg.artisan_id, t => RoleDataProxy.inst.GetWorker(t).state == EWorkerState.Unlock);
                if (workerId > 0)
                {
                    int workerLv = cfg.artisan_lv[Array.IndexOf(cfg.artisan_id, workerId)];
                    WorkerConfig workerConfig = WorkerConfigManager.inst.GetConfig(workerId);
                    workerLevelTx.transform.parent.gameObject.SetActive(true);
                    workerLevelTx.text = workerLv.ToString();
                    workerLockTx.text = LanguageManager.inst.GetValueByKey("需{0}达到{1}级", LanguageManager.inst.GetValueByKey(workerConfig.name), workerLv.ToString());
                }
                else
                {
                    Logger.error("没有未解锁的工匠，也没有等级不足的工匠，此装备的未解锁类型为 工匠解锁？？？");
                }
            }
        }
        else if (cfg.unlock_type == 2)//制作指定装备达到X数量解锁
        {
            targetBar.gameObject.SetActive(true);
            targetBar.enabled = true;
            unLockNeedIcon.enabled = true;
            lockText.enabled = true;
            targetBar.maxValue = cfg.unlock_val_02;
            EquipData edata = EquipDataProxy.inst.GetEquipData(cfg.unlock_val_01);
            if (edata != null)
            {
                targetBar.value = edata.beenMake;
                targetBarText.text = string.Format("{0}/{1}", edata.beenMake, cfg.unlock_val_02);
            }
            else
            {
                targetBar.value = 0;
                targetBarText.text = string.Format("{0}/{1}", 0, cfg.unlock_val_02);
            }
            EquipDrawingsConfig needcfg = EquipConfigManager.inst.GetEquipDrawingsCfg(cfg.unlock_val_01);
            unLockNeedIcon.SetSprite(needcfg.atlas, needcfg.icon);
        }
        else if (cfg.unlock_type == 3)//通过指定宝箱\转盘\七日\获得图纸解锁
        {
            targetBar.gameObject.SetActive(false);
            unLockNeedIcon.gameObject.SetActive(false);
            if (cfg.unlock_show_type == 3)
            {
                //宝箱
                unLockNeedIcon.enabled = false;
                lockText.enabled = true;
                boxLockObj.SetActive(true);
                var boxcfg = TreasureBoxConfigManager.inst.GetConfigByGroup(cfg.unlock_show_val);
                if (boxcfg != null)
                {
                    var boxitem = ItemconfigManager.inst.GetConfig(boxcfg.box_item_id);
                    if (boxitem != null)
                    {
                        boxIcon.SetSprite(boxitem.atlas, boxitem.icon);
                    }
                }
                boxLockTx.text = LanguageManager.inst.GetValueByKey("搜索");
            }
            else if (cfg.unlock_show_type == 4)
            {
                //转盘
                unLockNeedIcon.enabled = false;
                lockText.enabled = true;
                drawLockObj.SetActive(true);
                drawIcon.SetSprite(StaticConstants.staticAtlasName, "zhiye_renzhe");
                boxLockTx.text = LanguageManager.inst.GetValueByKey("前往");
            }
            else if (cfg.unlock_show_type == 5)
            {
                //七日
                unLockNeedIcon.enabled = false;
                lockText.enabled = true;
                drawLockObj.SetActive(true);
                drawIcon.SetSprite(StaticConstants.staticAtlasName, "zhiye_zhihuiguan");
                boxLockTx.text = LanguageManager.inst.GetValueByKey("前往");
            }
        }

        favoriteBtn.interactable = false;
        favoriteBtn.isOn = false;


        if (index < 9 && needShowAni) showAnim(index, false); //前9个


    }

    void updateRes()
    {
        if (currData == null) return;

        int workerId = -1;
        int workerLv = -1;
        //工匠
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(id);
        if (cfg != null)
        {
            if (ItemBagProxy.inst.getEquipAllNumber(cfg.id) > 0)
            {
                numberText.text = "x" + ItemBagProxy.inst.getEquipAllNumber(cfg.id).ToString();
            }
            else
            {
                numberText.transform.parent.gameObject.SetActive(false);
            }

            for (int i = 0; i < cfg.artisan_id.Length; i++)
            {
                int id = cfg.artisan_id[i];
                WorkerData data = RoleDataProxy.inst.GetWorker(id);
                if (data != null /*&& data.state == EWorkerState.Unlock*/)
                {
                    if (data.level < cfg.artisan_lv[i] || data.state != EWorkerState.Unlock)
                    {
                        workerId = id;
                        workerLv = cfg.artisan_lv[i];
                        break;
                    }
                }
            }

            if (workerId != -1)
            {
                workerneedlevelobj.SetActive(true);
                needWorkerlevel.gameObject.SetActive(true);
                needResList.gameObject.SetActive(false);
                WorkerConfig workerConfig = WorkerConfigManager.inst.GetConfig(workerId);
                workerIcon2.SetSprite(StaticConstants.roleHeadIconAtlasName, workerConfig.icon);
                needWorkerlevel.text = LanguageManager.inst.GetValueByKey("{0}级",workerLv.ToString());

                return;
            }
        }
        workerneedlevelobj.SetActive(false);
        needWorkerlevel.gameObject.SetActive(false);
        needResList.gameObject.SetActive(true);
        //材料
        if (currData.equipState != 0)
        {
            int i = 0;
            for (i = 0; i < currData.needRes.Length; i++)
            {
                needMaterialsInfo info = currData.needRes[i];
                if (info.type == 0)
                {
                    itemConfig rescfg = ItemconfigManager.inst.GetConfig(info.needId);

                    if (rescfg == null) continue;

                    needResItem[i].gameObject.SetActive(true);
                    needResItem[i].setData(rescfg.atlas, rescfg.icon, info.needCount, 0, ItemBagProxy.inst.resItemCount(info.needId) >= info.needCount);
                }
            }
            if (currData.specialRes_1.type > 0)
            {
                if (currData.specialRes_1.type == 1) //装备
                {
                    EquipConfig equipcfg = EquipConfigManager.inst.GetEquipInfoConfig(currData.specialRes_1.needId);
                    if (equipcfg != null)
                    {
                        needResItem[i].gameObject.SetActive(true);
                        needResItem[i].setData(equipcfg.equipDrawingsConfig.atlas, equipcfg.equipDrawingsConfig.icon, ItemBagProxy.inst.getEquipNumberBySuperQuip(currData.specialRes_1.needId), currData.specialRes_1.needCount, ItemBagProxy.inst.getEquipNumberBySuperQuip(currData.specialRes_1.needId) >= currData.specialRes_1.needCount, equipcfg.equipQualityConfig.quality);
                    }
                }
                else if (currData.specialRes_1.type == 2)    //  特殊资源
                {
                    itemConfig rescfg = ItemconfigManager.inst.GetConfig(currData.specialRes_1.needId);
                    needResItem[i].gameObject.SetActive(true);
                    needResItem[i].setData(rescfg.atlas, rescfg.icon, (int)ItemBagProxy.inst.resItemCount(currData.specialRes_1.needId), currData.specialRes_1.needCount, ItemBagProxy.inst.resItemCount(currData.specialRes_1.needId) >= currData.specialRes_1.needCount);
                }
            }
            i++;
            if (currData.specialRes_2.type > 0)
            {
                if (currData.specialRes_2.type == 1) //装备
                {
                    EquipConfig equipcfg = EquipConfigManager.inst.GetEquipInfoConfig(currData.specialRes_1.needId);
                    if (equipcfg != null)
                    {
                        needResItem[i].gameObject.SetActive(true);
                        needResItem[i].setData(equipcfg.equipDrawingsConfig.atlas, equipcfg.equipDrawingsConfig.icon, ItemBagProxy.inst.getEquipNumberBySuperQuip(currData.specialRes_2.needId), currData.specialRes_2.needCount, ItemBagProxy.inst.getEquipNumberBySuperQuip(currData.specialRes_2.needId) >= currData.specialRes_2.needCount, equipcfg.equipQualityConfig.quality);
                    }
                }
                else if (currData.specialRes_2.type == 2)    //  特殊资源
                {
                    itemConfig rescfg = ItemconfigManager.inst.GetConfig(currData.specialRes_2.needId);
                    needResItem[i].gameObject.SetActive(true);
                    needResItem[i].setData(rescfg.atlas, rescfg.icon, (int)ItemBagProxy.inst.resItemCount(currData.specialRes_2.needId), currData.specialRes_2.needCount, ItemBagProxy.inst.resItemCount(currData.specialRes_2.needId) >= currData.specialRes_2.needCount);
                }
            }

        }
    }

    public void refreshData(EquipData item) 
    {
        if (currData != item) return;
        updateRes();
    }

    void setStarUpInfo(int equipDrawingId) 
    {
        if (currData == null || equipDrawingId != currData.equipDrawingId) return;

        if (currData.starLevel <= 0)
        {
            foreach (var starIcon in starIcons)
            {
                starIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                var starIcon = starIcons[i];
                starIcon.gameObject.SetActive(true);
                starIcon.SetSprite("equipMakeUI_atlas", currData.starLevel > i ? "zhizuo_xingxing" : "zhizuo_xingxing1");
            }
        }
    }

    public void setData(EquipData item, int index, bool needShowAni, int curHighLightEqiupDrawingId)
    {

        if (gameObject.activeInHierarchy)
        {
            gameObject.name = item.equipDrawingId.ToString();
            itemName = item.equipDrawingId.ToString();
        }

        bool needSet = false;

        workerLockObj.SetActive(false);
        boxLockObj.SetActive(false);
        drawLockObj.SetActive(false);

        //if (!GuideDataProxy.inst.CurInfo.isAllOver)
        //{
        //    var guideInfo = GuideDataProxy.inst.CurInfo;
        //    if (((K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)guideInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick) && (guideInfo.m_curCfg.btn_name == "0" || guideInfo.m_curCfg.btn_name == "makeButton"))
        //    {
        //        if (item.equipDrawingId == int.Parse(guideInfo.m_curCfg.conditon_param_1))
        //        {
        //            if (needShowAni)
        //            {
        //                needSet = true;
        //            }
        //            else
        //                EventController.inst.TriggerEvent(GameEventType.GuideEvent.SETMASKTARGET, transform);
        //        }
        //    }
        //}

        highLightVFXObj.SetActive(item.equipDrawingId == curHighLightEqiupDrawingId); //高亮展示

        isShowAnimed = false;
        GUIHelper.SetUIGray(levelTf, false);
        newTip.gameObject.SetActive(false);
        id = item.equipDrawingId;
        currData = item;
        favoriteBtn.interactable = true;
        favoriteBtn.isOn = currData.favorite == 1;

        setStarUpInfo(currData.equipDrawingId);

        if (item.equipState == 2)
        {
            bg_1.enabled = false;
            bg_2.enabled = false;
            tx_master.enabled = false;
            bg_3.enabled = true;
            iconBg_1.enabled = false;
            iconBg_2.enabled = false;
            iconBg_3.enabled = true;
            nameBg_1.enabled = false;
            nameBg_2.enabled = false;
            nameBg_3.enabled = false;
            workerLockObj.gameObject.SetActive(false);
            lockText.text = "";
            unLockItemIcon.enabled = false;
            nextStageIcon.gameObject.SetActive(true);
            equipInfoBtn.gameObject.SetActive(true);
            needResList.gameObject.SetActive(true);
            unLockNeedIcon.gameObject.SetActive(false);
            needDrawingTf.gameObject.SetActive(false);
            targetBar.gameObject.SetActive(true);
            numberText.transform.parent.gameObject.SetActive(false);
            lockText.gameObject.SetActive(false);
            //targetBar.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().sprite = ManagerBinder.inst.Asset.getSprite("NewEquipMake_atlas.spriteatlas", "zhizuo_jindutlv");
            newTip.gameObject.SetActive(item.isNew);
            EquipDataProxy.inst.ClearNewEquip();
            // currData.isNewActivate = false;

        }
        else if (item.equipState == 1) //需要激活状态
        {
            infoIcon.SetSprite("__common_1", "cangku_tishi");
            collectIcon.SetSprite("equipMakeUI_atlas", "zhizuo_xiaoshoucang1");
            bg_1.enabled = false;
            bg_2.enabled = false;
            tx_master.enabled = false;
            bg_3.enabled = true;
            workerLockObj.gameObject.SetActive(false);
            iconBg_1.enabled = false;
            iconBg_2.enabled = false;
            iconBg_3.enabled = true;
            nameBg_1.enabled = true;
            nameBg_2.enabled = false;
            nameBg_3.enabled = false;
            newTip.gameObject.SetActive(false);
            unLockItemIcon.enabled = false;
            nextStageIcon.gameObject.SetActive(false);
            nextStageBgIcon.gameObject.SetActive(false);
            equipInfoBtn.gameObject.SetActive(true);
            needResList.gameObject.SetActive(true);
            unLockNeedIcon.gameObject.SetActive(false);
            targetBar.gameObject.SetActive(false);
            numberText.transform.parent.gameObject.SetActive(false);
            lockText.gameObject.SetActive(false);

            needDrawingTf.gameObject.SetActive(true);

            //targetBar.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().sprite = ManagerBinder.inst.Asset.getSprite("NewEquipMake_atlas.spriteatlas", "zhizuo_jindut");

        }
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(item.equipDrawingId);

        string _color = UserDataProxy.inst.playerData.drawing >= cfg.activate_drawing ? "ffffff" : "44e2ff";
        needDrawingcountTx.text = $"<color={_color}>{ UserDataProxy.inst.playerData.drawing}</color>/{cfg.activate_drawing}";  //数量
        nameText.text = LanguageManager.inst.GetValueByKey(cfg.name);

        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(cfg.sub_type);
        subTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);

        levelText.text = cfg.level.ToString();
        if (ItemBagProxy.inst.getEquipAllNumber(cfg.id) > 0)
        {
            numberText.transform.parent.gameObject.SetActive(true);
            numberText.text = "x" + ItemBagProxy.inst.getEquipAllNumber(cfg.id).ToString();
        }
        else
        {
            numberText.transform.parent.gameObject.SetActive(false);
        }
        equipIcon.SetSprite(cfg.atlas, cfg.icon);
        foreach (var resitem in needResItem)
        {
            resitem.gameObject.SetActive(false);
        }
        updateRes();

        //进度
        if (item.progressLevel >= 5)
        {
            bottomBgIcon.gameObject.SetActive(false);
            targetBar.gameObject.SetActive(false);
            nextStageIcon.gameObject.SetActive(false);
            nextStageBgIcon.gameObject.SetActive(false);
            bg_1.enabled = false;
            bg_2.enabled = true;
            tx_master.enabled = true;
            bg_3.enabled = false;
            nameBg_2.enabled = true;
            infoIcon.SetSprite("equipMakeUI_atlas", "cangku_tishi1");
            collectIcon.SetSprite("equipMakeUI_atlas", "zhizuo_xiaoshoucang");
        }
        else
        {
            nameBg_3.enabled = true;
            bottomBgIcon.gameObject.SetActive(true);
            bottomBgIcon.SetSprite("__common_1", "zhizuo_lichengbeidi1");

            if (item.equipState == 2)
            {
                targetBar.gameObject.SetActive(true);
                targetBar.maxValue = item.barMaxValue;
                targetBar.value = item.currBarValue;
                targetBarText.text = string.Format("{0}/{1}", item.currBarValue, item.barMaxValue);
                GUIHelper.SetMilestonesIconText(item.progresInfoList[item.progressLevel], ref nextStageIcon, ref lockText);

                if (item.progresInfoList[item.progressLevel].type == 7) //解锁新图纸
                {
                    EquipDrawingsConfig drawingcfg = EquipConfigManager.inst.GetEquipDrawingsCfg(item.progresInfoList[item.progressLevel].reward_id);
                    nextStageBgIcon.gameObject.SetActive(true);
                    nextStageBgIcon.SetSprite("equipMakeUI_atlas", drawingcfg.sub_type == cfg.sub_type ? "zhizuo_tuzhi1" : "zhizuo_tuzhi2");
                }
                else
                {
                    nextStageBgIcon.gameObject.SetActive(false);
                }

                infoIcon.SetSprite("__common_1", "cangku_tishi");
                collectIcon.SetSprite("equipMakeUI_atlas", "zhizuo_xiaoshoucang1");
            }
            else
            {

            }
        }
        this.makeNode.SetActive(false);
        barText.text = "";
        //------------------------
        UpdateMakeState();

        if (index < 9 && needShowAni) showAnim(index, needSet); //前9个
    }


    // private void

    //本身被点击
    public void onClick()
    {
        if (holdtime > triggerHoldTime)
        {
            holdtime = 0;
            return;
        }
        //Logger.log("点击了一件装备");
        if (currData != null)
        {
            if (currData.equipState == 2)
            {
                EventController.inst.TriggerEvent(GameEventType.ProductionEvent.EQUIP_PRODUCTIONLIST_START, id);
            }
            else if (currData.equipState == 1) //需要激活激活
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_UNLOCKDRAWINGUI, id);
            }
        }
        else
        {
            EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(id);

            if (cfg.unlock_type == 1)//获得指定工匠解锁
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_UNLOCKDRAWINGBYWORKERUI, cfg);
            }
            else if (cfg.unlock_type == 2)//制作指定装备达到X数量解锁
            {

            }
            else//通过指定宝箱获得解锁
            {
                if (cfg.unlock_show_type == 3)
                {

                }
                else if (cfg.unlock_show_type == 4)
                {

                }
                else if (cfg.unlock_show_type == 5)
                {

                }
            }


        }
    }


    //长按
    bool ishold = false;
    float holdtime = 0;
    float triggerHoldTime = 0.8f;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (currData == null || currData.equipState == 0) return;
        //Logger.log("按下" + eventData.ToString());
        holdtime = 0;
        ishold = true;
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.vfxPlanel.GetComponent<RectTransform>(), eventData.pressPosition, FGUI.inst.uiCamera, out localPoint))
        {
            FGUI.inst.holdTopTf.GetComponent<RectTransform>().anchoredPosition = localPoint;
        }
        InvokeRepeating("OnPress", 0, 0.05f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DoTweenUtil.ClickTween(transform as RectTransform, onClick);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!ishold)
        {
            CancelInvoke("OnPress");
            return;
        }
        //Logger.log("抬起");
        if (holdtime < triggerHoldTime)
        {
            //   
        }
        CancelInvoke("OnPress");
        holdtime = 0;
        FGUI.inst.holdTopTf.gameObject.SetActive(false);
    }

    private void OnPress()
    {
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null)
        {
            if (!GuideDataProxy.inst.CurInfo.isAllOver)
            {
                return;
            }
        }
        if (!ishold)
        {
            CancelInvoke("OnPress");
            return;
        }
        holdtime += 0.05f;
        if (holdtime >= 0.2f && !FGUI.inst.holdTopTf.gameObject.activeSelf)
        {
            FGUI.inst.holdTopTf.gameObject.SetActive(true);
            FGUI.inst.holdTopTf.fillAmount = 0;
        }
        if (holdtime > triggerHoldTime)
        {
            //打开页面
            CancelInvoke("OnPress");
            if (itemOnclick != null)
            {
                itemOnclick.Invoke(currData.equipDrawingId);
            }
            FGUI.inst.holdTopTf.gameObject.SetActive(false);
            return;
        }
        FGUI.inst.holdTopTf.fillAmount = holdtime * (1f / triggerHoldTime);
    }

    void OnDestroy()
    {
        if (currSlot != null)
            currSlot.coolTimeChange = null;
        EventController.inst.RemoveListener(GameEventType.ProductionEvent.PRODUCTIONLIST_STATE_Change, UpdateMakeState);
        EventController.inst.RemoveListener<int>(GameEventType.EquipEvent.EQUIP_STARUPSUCCESS, setStarUpInfo);
    }

    bool isShowAnimed;

    private void showAnim(int index, bool needSet)
    {
        if (isShowAnimed) return;
        isShowAnimed = true;

        gameObject.SetActive(false);

        GameTimer.inst.AddTimer(0.28f + 0.02f * index, 1, () =>
        {
            gameObject.SetActive(true);

            animator.CrossFade("show", 0f);
            animator.Update(0f);
            animator.Play("show");
        });

    }

}
