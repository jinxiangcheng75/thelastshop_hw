using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GTask : MonoBehaviour
{
    public GameObject bgObj;
    RectTransform bgRect;
    public GUIIcon icon;
    public Text contentText;
    int remainCount = 0;
    string taskContent;
    string param_1;
    string param_2;
    string param_3;

    private void Awake()
    {
        if (bgObj == null) return;
        bgRect = bgObj.GetComponent<RectTransform>();
    }

    public void showGTask()
    {
        GuideDataProxy.inst.taskIsShowing = true;
        gameObject.SetActiveTrue();

        var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
        if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.Task || (K_Guide_Type)cfg.guide_type == K_Guide_Type.WeakGuideAndTask)
        {
            var workerCfg = WorkerConfigManager.inst.GetConfig(cfg.character_id);

            icon.SetSprite(StaticConstants.roleHeadIconAtlasName, workerCfg.icon);
            if (cfg.conditon_param_5.Contains("{0}") && !cfg.conditon_param_5.Contains("{1}"))
            {
                contentText.text = LanguageManager.inst.GetValueByKey(cfg.conditon_param_5, cfg.end_param.ToString());
            }
            else if (cfg.conditon_param_5.Contains("{0}") && cfg.conditon_param_5.Contains("{1}") && cfg.conditon_param_5.Contains("{2}"))
            {
                GuideDataProxy.inst.curTargetId = int.Parse(cfg.conditon_param_2);
                var equipData = EquipConfigManager.inst.GetEquipDrawingsCfg(int.Parse(cfg.conditon_param_2));
                var equipData2 = EquipConfigManager.inst.GetEquipDrawingsCfg(int.Parse(cfg.conditon_param_3));
                remainCount = int.Parse(cfg.conditon_param_1);
                taskContent = cfg.conditon_param_5;
                param_1 = equipData.name;
                param_2 = equipData2.name;
                contentText.text = LanguageManager.inst.GetValueByKey(cfg.conditon_param_5, LanguageManager.inst.GetValueByKey(cfg.conditon_param_1), LanguageManager.inst.GetValueByKey(equipData.name), LanguageManager.inst.GetValueByKey(equipData2.name));
               
            }
            //GuideManager.inst.GuideManager_OnNextGuide();
        }
        else
        {
            var tempCfg = GuideConfigManager.inst.GetConfig(3102);
            var workerCfg = WorkerConfigManager.inst.GetConfig(tempCfg.character_id);

            icon.SetSprite(StaticConstants.roleHeadIconAtlasName, workerCfg.icon);
            GuideDataProxy.inst.curTargetId = int.Parse(tempCfg.conditon_param_2);
            var equipData = EquipConfigManager.inst.GetEquipDrawingsCfg(int.Parse(tempCfg.conditon_param_2));
            var equipData2 = EquipConfigManager.inst.GetEquipDrawingsCfg(int.Parse(tempCfg.conditon_param_3));
            remainCount = int.Parse(tempCfg.conditon_param_1) - EquipDataProxy.inst.GetEquipData(int.Parse(tempCfg.conditon_param_2)).beenMake;
            taskContent = tempCfg.conditon_param_5;
            param_1 = equipData.name;
            param_2 = equipData2.name;
            contentText.text = LanguageManager.inst.GetValueByKey(tempCfg.conditon_param_5, LanguageManager.inst.GetValueByKey(remainCount.ToString()), LanguageManager.inst.GetValueByKey(equipData.name), LanguageManager.inst.GetValueByKey(equipData2.name));
        }
    }

    public void refreshTaskData()
    {
        if (taskContent != null)
        {
            remainCount--;
            remainCount = Mathf.Clamp(remainCount, 0, remainCount);

            contentText.text = LanguageManager.inst.GetValueByKey(taskContent, LanguageManager.inst.GetValueByKey(remainCount.ToString()), LanguageManager.inst.GetValueByKey(param_1), LanguageManager.inst.GetValueByKey(param_2));
        }
    }

    public void hideGTask()
    {
        GuideDataProxy.inst.taskIsShowing = false;
        GuideDataProxy.inst.curTargetId = 0;
        gameObject.SetActiveFalse();
    }

    float timer;
    bool isReStart = true;
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 8 && isReStart)
        {
            isReStart = false;
            bgRect.DOAnchorPosX(550, 1).OnComplete(() =>
             {
                 bgRect.DOAnchorPosX(225, 1).SetDelay(5).OnComplete(() =>
                  {
                      timer = 0;
                      isReStart = true;
                  });
             });
        }
    }
}
