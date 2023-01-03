using DG.Tweening;
using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipListUIView : ViewBase<equipBagComp>
{
    public override string viewID => ViewPrefabName.EquipMakeUI;
    public override int showType => (int)ViewShowType.pullUp;
    private Dictionary<int, ResComp> resitemcomps = new Dictionary<int, ResComp>();
    const int itemCountPerRow = 3;
    public int soltHarderId = 0;

    private int currMainTypeSelected = 0;
    private int[] subTypeState = { 1, 1, 1, 1 }; //二级分类，选择对象类型

    private int[][] subTypeGroup = new int[][] //大分类 与子分类关系
    {
        new int[]{1,2,3,4,5,8,6,7},
        new int[]{9,10,11,12,13,14,15,16,17},
        new int[]{18,19,20,21,22,23},
    };
    List<EquipData> currEquipList;
    List<int> notUnlockEquip;
    private int listItemCount = 0;
    //刷新子分类标签
    List<EquipSubTypeitem> subtypeList = new List<EquipSubTypeitem>();

    bool needShowAni;
    public int curHighLightEqiupDrawingId = -1;

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        windowAnimTime = contentPane.uiAnimator.GetClipLength("commonBagUI_show");
        subtypeList.Clear();
        for (int i = 0; i < 23; i++)
        {
            var newGO = GameObject.Instantiate(contentPane.subTypeItem, contentPane.subTypeTF) as EquipSubTypeitem;
            subtypeList.Add(newGO);
            newGO.OnSelect = SubTypeTableOnSelect;
            newGO.gameObject.SetActive(false);
        }
        contentPane.subTypeItem.gameObject.SetActive(false);

        InitEquipList();
        contentPane.closeBtn.ButtonClickTween(() =>
        {
            hide();
        });
        contentPane.resListParent.gameObject.SetActive(false);
        if (!FGUI.inst.isLandscape)
            contentPane.mainTypeTF.SetToggleSize(new Vector2(320, 124), new Vector2(246, 124));
        else
            contentPane.mainTypeTF.SetToggleSize(new Vector2(160, 151), new Vector2(160, 151));
        contentPane.mainTypeTF.OnSelectedIndexValueChange = mainTypeSelectedChange; //大分类改变

        contentPane.allTypeToggle.onValueChanged.AddListener((value) =>
        {
            //AudioManager.inst.PlaySound(11);
            for (int i = 0; i < contentPane.allTypeToggle.graphic.transform.childCount; i++)
            {
                contentPane.allTypeToggle.graphic.transform.GetChild(i).gameObject.SetActive(value);
            }

            if (value)
                SubTypeTableOnSelect(1);
        });
        contentPane.collectToggle.onValueChanged.AddListener((value) =>
        {
            //AudioManager.inst.PlaySound(11);
            for (int i = 0; i < contentPane.collectToggle.graphic.transform.childCount; i++)
            {
                contentPane.collectToggle.graphic.transform.GetChild(i).gameObject.SetActive(value);
            }

            if (value)
            {
                contentPane.collectCheckObj.SetActive(true);
                SubTypeTableOnSelect(2);
            }
            else
            {
                contentPane.collectCheckObj.SetActive(false);
            }
        });
        contentPane.activity_workerGameToggle.onValueChanged.AddListener((value) =>
        {
            //AudioManager.inst.PlaySound(11);
            for (int i = 0; i < contentPane.activity_workerGameToggle.graphic.transform.childCount; i++)
            {
                contentPane.activity_workerGameToggle.graphic.transform.GetChild(i).gameObject.SetActive(value);
            }

            if (value)
                SubTypeTableOnSelect(3);
        });
        contentPane.subTypeTF.transform.GetComponent<ToggleGroupEX>().StartMethod();
    }
    //当标签被选择
    private void SubTypeTableOnSelect(int index)
    {

        if (subTypeState[currMainTypeSelected] != index)
        {
            AudioManager.inst.PlaySound(11);
            needShowAni = false;

            subTypeState[currMainTypeSelected] = index;
            //刷新列表
            updateEquipList();
        }

    }

    private void mainTypeSelectedChange(int index)
    {
        if (currMainTypeSelected != index)
        {
            AudioManager.inst.PlaySound(11);
            needShowAni = false;

            currMainTypeSelected = index;
            updateTypePanle();
            updateEquipList();
        }
    }


    public void SetUIInfo()
    {
        contentPane.contentSizeFitter.enabled = true;

        showInfo();
        updateTopTypeGroup();
        UpdateUnoinBuffInfo();
    }

    public void updateTopTypeGroup()
    {
        bool typetablevisble = true;
        var cfg = WorldParConfigManager.inst.GetConfig(163);
        var value_1 = cfg == null ? 0 : (int)cfg.parameters;
        if (UserDataProxy.inst.playerData.level < value_1)
        {
            cfg = WorldParConfigManager.inst.GetConfig(162);
            var value_2 = cfg == null ? 0 : (int)cfg.parameters;
            if (EquipDataProxy.inst.GetEquipDatas().Count < value_2)
            {
                typetablevisble = false;
            }
        }
        else
        {
            typetablevisble = true;
        }
        if (typetablevisble)
        {
            contentPane.mainTypeTF.gameObject.SetActive(true);
            contentPane.subTypeTF.gameObject.SetActive(true);
            contentPane.subTypeTF1.gameObject.SetActive(true);
            if (!FGUI.inst.isLandscape)
            {
                var _rectTF = contentPane.loopListView.GetComponent<RectTransform>();
                _rectTF.sizeDelta = new Vector2(_rectTF.sizeDelta.x, -453.5f);
            }
        }
        else
        {
            contentPane.mainTypeTF.gameObject.SetActive(false);
            contentPane.subTypeTF.gameObject.SetActive(false);
            contentPane.subTypeTF1.gameObject.SetActive(false);
            if (!FGUI.inst.isLandscape)
            {
                var _rectTF = contentPane.loopListView.GetComponent<RectTransform>();
                _rectTF.sizeDelta = new Vector2(_rectTF.sizeDelta.x, -237f);
            }
        }
    }
    public void UpdateUnoinBuffInfo()
    {
        foreach (var item in contentPane.unionBuffItems)
        {
            item.UpdateData();
        }
    }

    protected override void onHide()
    {
        base.onHide();

        curHighLightEqiupDrawingId = -1;
        if (HideAnimationTimerId > 0)
            GameTimer.inst.RemoveTimer(HideAnimationTimerId);
    }


    void animateOpResItems()
    {
        contentPane.gridLayoutGroup.gameObject.SetActiveTrue();
        var hlayout = contentPane.gridLayoutGroup;
        contentPane.contentSizeFitter.enabled = false;//播放动画时关闭contentSizeFitter

        for (int i = 0; i < contentPane.resList.Length; i++)
        {
            var resItem = contentPane.resList[i];
            var graphics = resItem.transform.GetComponentsInChildren<Graphic>();

            for (int k = 0; k < graphics.Length; k++)
            {
                graphics[k].DOFade(1, 0.5f).From(0.3f);
            }
        }

        DOTween.To(() => hlayout.spacing, v => hlayout.spacing = v, Vector2.zero, 0.5f).From(Vector2.left * 100).SetEase(Ease.OutCubic);
    }

    protected override void DoShowAnimation()
    {
        //needShowAni = true;
        base.DoShowAnimation();

        //contentPane.uiAnimator.CrossFade("show", 0f);
        //contentPane.uiAnimator.Update(0f);
        //contentPane.uiAnimator.Play("show");

        //contentPane.gridLayoutGroup.gameObject.SetActiveFalse();
        //contentPane.maskObj.SetActiveTrue();
        //GameTimer.inst.AddTimer(0.28f, 1, () =>
        //{
        //    animateOpResItems();
        //});

        //var count = currEquipList == null ? 0 : notUnlockEquip != null ? notUnlockEquip.Count + currEquipList.Count : currEquipList.Count;

        //GameTimer.inst.AddTimer(count <= 9 ? count * 0.02f + 0.28f : 0.46f, 1, () =>
        // {
        //     contentPane.maskObj.SetActiveFalse();
        //     needShowAni = false;
        // }); //播放动画禁止滑动

        contentPane.resListParent.gameObject.SetActive(true);
    }
    int HideAnimationTimerId = 0;
    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("commonBagUI_hide");

        HideAnimationTimerId = GameTimer.inst.AddTimer(animLength, 1, () =>
       {
           contentPane.uiAnimator.CrossFade("null", 0f);
           contentPane.uiAnimator.Update(0f);
           this.HideView();
       });
    }

    public void updateResListUI(int itemid)
    {
        RefreshResProductionBar(itemid);
    }

    public void UpdateUIInfo()
    {
        showInfo();
    }
    protected void showInfo()
    {
        if (!GuideDataProxy.inst.CurInfo.isAllOver)
            contentPane.mainTypeTF.OnEnableMethod(0);
        else
            contentPane.mainTypeTF.OnEnableMethod(currMainTypeSelected);

        // 列表
        updateEquipList();

        updateTypePanle();

        //资源
        updateResPlane();
    }


    //subTypeGroup
    private void updateTypePanle()
    {

        subtypeList.ForEach(item =>
        {
            item.gameObject.SetActive(false);

        });

        if (currMainTypeSelected == 0)
        {
            //总分类
            contentPane.allTypeToggle.gameObject.SetActive(true);
            contentPane.collectToggle.gameObject.SetActive(true);
            bool activity_workerGameToggleActive = HotfixBridge.inst.GetActivity_WorkerGameFlag();
            contentPane.activity_workerGameToggle.gameObject.SetActive(activity_workerGameToggleActive);
            string activity_workerGameName = HotfixBridge.inst.GetActivity_WorkerGameStr(EOperatingActivityStringType.Name);
            if (LanguageManager.inst.curType == LanguageType.ENGLISH)
            {
                activity_workerGameName = activity_workerGameName.Length >= 8 ? activity_workerGameName.Substring(0, 8) : activity_workerGameName;
            }
            else
            {
                activity_workerGameName = activity_workerGameName.Length >= 4 ? activity_workerGameName.Substring(0, 4) : activity_workerGameName;
            }
            contentPane.activity_workerGameNameTx_1.text = contentPane.activity_workerGameNameTx_2.text = activity_workerGameName;

            if (subTypeState[0] == 1) //最近
            {
                contentPane.allTypeToggle.isOn = true;
            }
            else if (subTypeState[0] == 2) //收藏
            {
                contentPane.collectToggle.isOn = true;
            }
            else if (subTypeState[0] == 3) //巧匠大赛
            {
                if (activity_workerGameToggleActive)
                {
                    if (!contentPane.activity_workerGameToggle.isOn)
                    {
                        contentPane.activity_workerGameToggle.isOn = true;
                    }
                }
                else
                {
                    if (contentPane.activity_workerGameToggle.isOn)
                    {
                        contentPane.allTypeToggle.isOn = true;
                    }
                }
            }

            contentPane.nullItem.SetActive(false);
        }
        else
        {
            contentPane.allTypeToggle.gameObject.SetActive(false);
            contentPane.collectToggle.gameObject.SetActive(false);
            contentPane.activity_workerGameToggle.gameObject.SetActive(false);
            for (int i = 0; i < subTypeGroup[currMainTypeSelected - 1].Length; i++)
            {
                EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(subTypeGroup[currMainTypeSelected - 1][i]);
                EquipSubTypeitem item = subtypeList[i];
                item.gameObject.SetActive(true);
                item.type = (EquipSubType)classcfg.id;
                item.index = i + 1;
                item.Icon.SetSprite(classcfg.Atlas, classcfg.icon);
                string[] arr = classcfg.icon.Split('2');
                item.typeIcon.SetSprite(classcfg.Atlas, arr[0] + "1");
            }
            subtypeList[subTypeState[currMainTypeSelected] - 1].setSelect();
        }
    }
    //刷新 资源列表
    private void updateResPlane()
    {
        resitemcomps.Clear();
        var resitems = ItemBagProxy.inst.GetItemsByType(ItemType.Material);
        if (resitems != null)
        {
            if (!GameSettingManager.inst.needShowUIAnim) contentPane.resListParent.gameObject.SetActive(true);
            int index = 0;
            for (int i = 0; i < resitems.Count; i++)
            {
                Item item = resitems[i];
                contentPane.resList[i].gameObject.SetActive(false);
                ResourceProduction rp = UserDataProxy.inst.GetResProduction(item.ID);
                if (rp != null && rp.isActivate)
                {
                    ResComp resitem = contentPane.resList[index];
                    resitem.setIcon(item.itemConfig.atlas, item.itemConfig.icon);
                    resitem.count.text = item.count.ToString();
                    resitemcomps.Add(item.ID, resitem);
                    resitem.gameObject.SetActive(true);
                    RefreshResProductionBar(item.ID);
                    index++;
                }
            }
        }
        else
        {
            contentPane.resListParent.gameObject.SetActive(false);
        }

    }

    //刷新装备
    private void updateEquipList()
    {
        if (FGUI.inst.isLandscape)
        {
            contentPane.scrollRect.horizontal = true;
        }
        else
        {
            contentPane.scrollRect.vertical = true;
        }

        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            if ((K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
            {
                if (FGUI.inst.isLandscape)
                {
                    contentPane.scrollRect.horizontal = false;
                }
                else
                {
                    contentPane.scrollRect.vertical = false;
                }
            }
        }
        if (currMainTypeSelected == 0)
        {
            if (subTypeState[0] == 1) //最近
            {
                currEquipList = EquipDataProxy.inst.GetEquipDatas();
                currEquipList.Sort((x, y) => -x.lastMakeTime.CompareTo(y.lastMakeTime));//总列表按制作时间排序 降序
            }
            else if (subTypeState[0] == 2) //收藏
            {
                currEquipList = EquipDataProxy.inst.GetFavoriteEquipDatas();
                currEquipList.Sort((x, y) => -x.lastMakeTime.CompareTo(y.lastMakeTime));//总列表按制作时间排序 降序
            }
            else if (subTypeState[0] == 3) //巧匠大赛
            {
                currEquipList = EquipDataProxy.inst.GetActivity_WorkerGameAddRateEquipDatas();
                currEquipList.Sort((x, y) => -HotfixBridge.inst.GetActivity_WorkerGameEquipMakeIntegralByDrawingId(x.equipDrawingId).CompareTo(HotfixBridge.inst.GetActivity_WorkerGameEquipMakeIntegralByDrawingId(y.equipDrawingId)));//总列表按制作增加分数 降序
            }


            notUnlockEquip = null;

            if (curHighLightEqiupDrawingId != -1)
            {
                for (int i = 0; i < currEquipList.Count; i++)
                {
                    if (currEquipList[i].equipDrawingId == curHighLightEqiupDrawingId)
                    {
                        contentPane.loopListView.scrollByItemIndex(i);
                        break;
                    }
                }
            }
        }
        else  //分类
        {
            //可以激活和可以制作的列表
            var List = EquipDataProxy.inst.GetEquipDatas(currMainTypeSelected);
            currEquipList = List.FindAll(item => item.subType == subTypeGroup[currMainTypeSelected - 1][subTypeState[currMainTypeSelected] - 1]);
            if (curHighLightEqiupDrawingId != -1)
            {
                for (int i = 0; i < currEquipList.Count; i++)
                {
                    if (currEquipList[i].equipDrawingId == curHighLightEqiupDrawingId)
                    {
                        contentPane.loopListView.scrollByItemIndex(i);
                        break;
                    }
                }
            }
            //可以解锁的列表
            notUnlockEquip = EquipDataProxy.inst.GetNotUnLockEquips(subTypeGroup[currMainTypeSelected - 1][subTypeState[currMainTypeSelected] - 1]);

            currEquipList.Sort((x, y) => -x.level.CompareTo(y.level)); //根据品阶排序


            if (notUnlockEquip.Count == 0 && currEquipList.Count == 0) //判断空的Item数量显示没有物品可以打造
            {
                //Debug.LogError(currEquipList.Count + "Item的数量");
                //for (int i = 0; i < currEquipList.Count; i++)
                //{
                //    //if(currEquipList[i])
                //}
                contentPane.nullItem.SetActive(true);
                //获得当前分类中一阶的图纸

            }
            else
            {
                contentPane.nullItem.SetActive(false);

            }
        }
        contentPane.loopListView.ScrollToTop();
        SetListItemTotalCount(currEquipList == null ? 0 : notUnlockEquip != null ? notUnlockEquip.Count + currEquipList.Count : currEquipList.Count);
    }

    public void RefreshResProductionBar(int itemId)
    {
        if (resitemcomps.ContainsKey(itemId))
        {
            ResourceProduction rp = UserDataProxy.inst.GetResProduction(itemId);
            if (rp != null && rp.isActivate)
            {
                bool full = ItemBagProxy.inst.resItemCount(itemId) >= rp.countLimit;
                resitemcomps[itemId].gameObject.SetActive(true);
                resitemcomps[itemId].RefreshMakeBar(full, (float)rp.duration, (float)rp.time);
                resitemcomps[itemId].count.text = ItemBagProxy.inst.resItemCount(itemId).ToString();
                resitemcomps[itemId].count.color = rp.countLimit == 0 ? GUIHelper.GetColorByColorHex("fd4f4f") : (full ? GUIHelper.GetColorByColorHex("52fb66") : Color.white);
            }
            else
            {
                resitemcomps[itemId].gameObject.SetActive(false);
            }
        }
        contentPane.loopListView.updateListItemInfo();
    }

    public void RefreshListItemsInfo()
    {
        contentPane.loopListView.updateListItemInfo();
    }


    public void InitEquipList()
    {
        currEquipList = EquipDataProxy.inst.GetEquipDatas(0);
        listItemCount = currEquipList.Count;

        int count = listItemCount / itemCountPerRow;
        if (listItemCount % itemCountPerRow > 0)
        {
            count++;
        }

        if (contentPane.loopListView != null)
        {
            contentPane.loopListView.itemRenderer = this.listitemRenderer;
            contentPane.loopListView.itemUpdateInfo = this.listitemUpdateInfo;
            contentPane.loopListView.activeFalse = this.activeFalseRenderer;
            //contentPane.loopListView.totalItemCount = count;
            contentPane.loopListView.scrollByItemIndex(0);
        }
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList item = (BtnList)obj;
        if (index == -1) index = item.index;
        for (int i = 0; i < itemCountPerRow; ++i)
        {
            int itemIndex = index * itemCountPerRow + i;
            if (itemIndex >= listItemCount)
            {
                item.buttonList[i].gameObject.name = "item";
                item.buttonList[i].gameObject.SetActive(false);
                continue;
            }
            int count = notUnlockEquip != null ? notUnlockEquip.Count + currEquipList.Count : currEquipList.Count;
            if (itemIndex < count)
            {
                item.buttonList[i].gameObject.SetActive(true); //添加
                var equipInfoListItem = item.buttonList[i].GetComponent<EquipMakeListItem>();
                //获取data  赋值  
                setBagItemData(itemIndex, equipInfoListItem, false);
                equipInfoListItem.itemOnclick = ListItemOnclick;
            }
            else
            {
                item.buttonList[i].gameObject.name = "item";
                item.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void listitemUpdateInfo(int index, IDynamicScrollViewItem obj)
    {
        BtnList item = (BtnList)obj;
        if (index == -1) index = item.index;
        for (int i = 0; i < itemCountPerRow; ++i)
        {
            int itemIndex = index * itemCountPerRow + i;
            if (itemIndex >= listItemCount)
            {
                item.buttonList[i].gameObject.name = "item";
                item.buttonList[i].gameObject.SetActive(false);
                continue;
            }
            int count = notUnlockEquip != null ? notUnlockEquip.Count + currEquipList.Count : currEquipList.Count;
            if (itemIndex < count)
            {
                item.buttonList[i].gameObject.SetActive(true); //添加
                var equipInfoListItem = item.buttonList[i].GetComponent<EquipMakeListItem>();
                //获取data  只刷新 
                setBagItemData(itemIndex, equipInfoListItem, true);
                equipInfoListItem.itemOnclick = ListItemOnclick;
            }
            else
            {
                item.buttonList[i].gameObject.name = "item";
                item.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void activeFalseRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList item = (BtnList)obj;
        for (int i = 0; i < itemCountPerRow; ++i)
        {
            item.buttonList[i].gameObject.name = "item";
        }
    }
    private void ListItemOnclick(int equipid)
    {
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUI, equipid, currEquipList);
    }
    void setBagItemData(int itemIndex, EquipMakeListItem item, bool isRefresh)
    {
        int equipDrawingId = 0;

        if (notUnlockEquip == null)
        {
            equipDrawingId = currEquipList[itemIndex].equipDrawingId;
            if (!isRefresh) item.setData(currEquipList[itemIndex], itemIndex, needShowAni, curHighLightEqiupDrawingId);
            else item.refreshData(currEquipList[itemIndex]);
        }
        else
        {
            if (itemIndex >= notUnlockEquip.Count)
            {
                var _index = itemIndex - notUnlockEquip.Count;
                equipDrawingId = currEquipList[_index].equipDrawingId;
                if (!isRefresh) item.setData(currEquipList[_index], itemIndex, needShowAni, curHighLightEqiupDrawingId);
                else item.refreshData(currEquipList[_index]);
            }
            else
            {
                if (!isRefresh)
                {
                    equipDrawingId = notUnlockEquip[itemIndex];
                    item.setNotUnLockDate(notUnlockEquip[itemIndex], itemIndex, needShowAni);
                }
            }
        }

        //lua侧 刷新
        LuaListItem luaListItem = item.GetComponent<LuaListItem>();

        if (luaListItem != null)
        {
            luaListItem.SetData(equipDrawingId);
        }

    }

    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        int count1 = listItemCount / itemCountPerRow;
        if (listItemCount % itemCountPerRow > 0)
        {
            count1++;
        }
        contentPane.loopListView.totalItemCount = count1;

        if (curHighLightEqiupDrawingId != -1)
        {
            for (int i = 0; i < currEquipList.Count; i++)
            {
                if (currEquipList[i].equipDrawingId == curHighLightEqiupDrawingId)
                {
                    contentPane.loopListView.scrollByItemIndex(i / itemCountPerRow);
                    break;
                }
            }
        }
    }

    public void setShowListSubtype(int type)
    {
        for (int i = 0; i < subTypeGroup.Length; i++)
        {
            for (int j = 0; j < subTypeGroup[i].Length; j++)
            {
                if (subTypeGroup[i][j] == type)
                {
                    currMainTypeSelected = i + 1;
                    subTypeState[currMainTypeSelected] = j + 1;
                    break;
                }
            }
        }
    }

    public void setMainToggle()
    {
        currMainTypeSelected = 0;
        subTypeState[0] = 1;
    }

}
