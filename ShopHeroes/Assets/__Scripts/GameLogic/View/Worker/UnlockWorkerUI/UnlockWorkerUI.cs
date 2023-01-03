using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWorkerUI : ViewBase<UnlockWorekrUIComp>
{

    public override string sortingLayerName => "popup";
    public override string viewID => ViewPrefabName.UnlockWorkerUI;

    GraphicDressUpSystem graphicDressUpSystem;

    protected override void onInit()
    {
        contentPane.continueBtn.onClick.AddListener(onContinueBtnClick);
    }

    public void SetData(int workerId)
    {
        WorkerConfig workerCfg = WorkerConfigManager.inst.GetConfig(workerId);

        if (graphicDressUpSystem == null)
        {
            CharacterManager.inst.GetCharacterByModel<GraphicDressUpSystem>(workerCfg.model, callback: (sys) =>
            {
                graphicDressUpSystem = sys;
                graphicDressUpSystem.transform.SetParent(contentPane.workerTf);
                graphicDressUpSystem.transform.localScale = Vector3.one;
                graphicDressUpSystem.transform.localPosition = Vector3.zero;
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUpSystem.gender, (int)kIndoorRoleActionType.normal_standby);
                graphicDressUpSystem.Play(idleAnimationName, true);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByModel(graphicDressUpSystem, workerCfg.model);
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUpSystem.gender, (int)kIndoorRoleActionType.normal_standby);
            graphicDressUpSystem.Play(idleAnimationName, true);
        }


        //contentPane.workerPicIcon.SetSprite(StaticConstants.guideAtlas, workerCfg.pic, "", true);
        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(workerCfg.name);
        contentPane.typeText.text = LanguageManager.inst.GetValueByKey(workerCfg.profession);
        contentPane.contentText.text = LanguageManager.inst.GetValueByKey(workerCfg.desc);

        int[] itemDatas = workerCfg.equipment_id;

        for (int i = 0; i < contentPane.unlockItems.Count; i++)
        {
            if (i >= itemDatas.Length)
            {
                contentPane.unlockItems[i].SetActiveFalse();
            }
            else
            {
                contentPane.unlockItems[i].SetActiveTrue();
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(itemDatas[i]);
                contentPane.unlockItemIcons[i].SetSprite(equipCfg.atlas, equipCfg.icon);

                contentPane.unlockItemBtns[i].onClick.RemoveAllListeners();
                contentPane.unlockItemBtns[i].onClick.AddListener(() =>
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_EQUIPINFOUI, equipCfg.id, new List<EquipData>());
                });
            }
        }

    }

    void onContinueBtnClick()
    {
        //AudioManager.inst.PlaySound(62);
        hide();
    }

    protected override void onHide()
    {
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
    }


}
