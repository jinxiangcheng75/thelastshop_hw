using Mosframe;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerInfoView : ViewBase<WorkerInfoComp>
{
    public override string viewID => ViewPrefabName.WorkerInfoUI;
    public override string sortingLayerName => "window";

    private WorkerData _data;
    private int _index;
    private DressUpSystem workerDressSys;

    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;

        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.superList.itemRenderer = this.listitemRenderer;
        contentPane.superList.itemUpdateInfo = this.listitemRenderer;
        contentPane.superList.scrollByItemIndex(0);

        contentPane.toggleGroup.OnSelectedIndexValueChange = OnSelectedValueChange;

        contentPane.leftBtn.ButtonClickTween(() => turnPage(true));
        contentPane.rightBtn.ButtonClickTween(() => turnPage(false));

        contentPane.investBtn.ButtonClickTween(onInvestBtnClick);

        contentPane.expInfoBtn.ButtonClickTween(() =>
        {
            contentPane.expTipsCloseBtn.gameObject.SetActive(true);
            contentPane.expTipsObj.SetActive(true);
        });

        contentPane.expTipsCloseBtn.onClick.AddListener(() =>
        {
            contentPane.expTipsCloseBtn.gameObject.SetActive(false);
            contentPane.expTipsObj.SetActive(false);
        });

    }

    public void Init(WorkerData data)
    {
        _data = data;

        contentPane.headIcon.SetSprite(StaticConstants.roleHeadIconAtlasName, _data.config.icon);
        contentPane.typeIcon.SetSprite("worker_atlas", data.config.profession_icon);
        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(_data.config.name);
        contentPane.lvText.text = LanguageManager.inst.GetValueByKey("等级") + _data.level;
        contentPane.expSlider.value = Mathf.Max(0.05f, (float)_data.exp / _data.maxExp);
        contentPane.expTx.text = _data.exp + "/" + _data.maxExp /*+ "(" + Mathf.FloorToInt((float)_data.exp / _data.maxExp * 100) + "%)"*/;

        contentPane.toggleGroup.OnEnableMethod(_index);

        if (workerDressSys == null)
        {
            CharacterManager.inst.GetCharacterByModel<DressUpSystem>(_data.config.model, callback: (sys) =>
            {
                workerDressSys = sys;
                workerDressSys.SetUIPosition(contentPane.workerTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1, 0.8f);
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)workerDressSys.gender, (int)kIndoorRoleActionType.normal_standby);
                workerDressSys.Play(idleAnimationName, true);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByModel(workerDressSys, _data.config.model);
            workerDressSys.SetUIPosition(contentPane.workerTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1, 0.8f);
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)workerDressSys.gender, (int)kIndoorRoleActionType.normal_standby);
            workerDressSys.Play(idleAnimationName, true);
        }

        bool active = RoleDataProxy.inst.WorkerList.FindAll(t => t.state == EWorkerState.Unlock).Count > 1;
        contentPane.leftBtn.gameObject.SetActive(active);
        contentPane.rightBtn.gameObject.SetActive(active);
    }

    void turnPage(bool isLeft)
    {
        Init(RoleDataProxy.inst.GetNearWorkerData(_data, isLeft));
    }

    private void onInvestBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.BUILDING_ONCLICK, _data.config.connect_build_id);
    }

    private void OnSelectedValueChange(int index)
    {
        if (_data == null) return;
        AudioManager.inst.PlaySound(11);
        _index = index;

        if (index == 0)
        {
            onInfoState();
        }
        else
        {
            onDrawingState();
        }
    }

    private void onInfoState()
    {
        contentPane.drawingUI.SetActive(false);
        contentPane.infoUI.SetActive(true);


        contentPane.professionText.text = LanguageManager.inst.GetValueByKey(_data.config.profession);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPane.professionText.rectTransform);


        contentPane.descText.text = LanguageManager.inst.GetValueByKey(_data.config.desc);
        contentPane.curSpeedTx.text = "+" + _data.addSpeed + "%";
        contentPane.nextSpeedTx.text = "+" + _data.nextSpeed + "%";

        CityBuildingData buildingData = UserDataProxy.inst.GetBuildingData(_data.config.connect_build_id);
        contentPane.investBtn.interactable = buildingData != null;
        if (buildingData == null)
        {
            Logger.error("未找到对应建筑物的信息  建筑物ID ： " + _data.config.connect_build_id);
            GUIHelper.SetUIGray(contentPane.investBtn.transform, true);
        }
        else
        {
            GUIHelper.SetUIGray(contentPane.investBtn.transform, false);
            contentPane.investBtnTx.text = LanguageManager.inst.GetValueByKey(buildingData.config.name);
        }

    }

    int listItemCount = 0;
    private List<EquipData> equips;
    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < 4; ++i)
        {
            int itemIndex = index * 4 + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < equips.Count)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                WorkerDrawingItem item = itemScript.buttonList[i].GetComponent<WorkerDrawingItem>();
                item.setData(EquipConfigManager.inst.GetEquipDrawingsCfg(equips[itemIndex].equipDrawingId));
                item.onClickHandle = () => itemOnClickHandle(equips[itemIndex].equipDrawingId);
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void itemOnClickHandle(int equipDrawingId)
    {
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUI, equipDrawingId, equips);
    }

    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        if (listItemCount > equips.Count)
        {
            listItemCount = equips.Count;
        }
        int count1 = listItemCount / 4;
        if (listItemCount % 4 > 0)
        {
            count1++;
        }
        contentPane.superList.totalItemCount = count1;
    }

    private void onDrawingState()
    {
        contentPane.infoUI.SetActive(false);
        contentPane.drawingUI.SetActive(true);


        equips = EquipDataProxy.inst.GetEquipsByWorkerId(_data.id);


        if (equips.Count > 0)
        {
            SetListItemTotalCount(equips.Count);
        }
        else
        {
            contentPane.superList.totalItemCount = 0;
        }

        contentPane.superList.refresh();
    }

    protected override void onShown()
    {
        base.onShown();
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        base.onHide();
        contentPane.expTipsCloseBtn.gameObject.SetActive(false);
        contentPane.expTipsObj.SetActive(false);
        //AudioManager.inst.PlaySound(11);
    }
}
