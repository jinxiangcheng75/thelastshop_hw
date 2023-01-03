using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GNewTask : MonoBehaviour
{
    public GUIIcon icon;
    public Button selfBtn;
    public Text contentText;
    public Text btnText;
    public Transform moveTrans;
    public Canvas c;
    public Material liuguang;
    bool isOk;
    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            if (isOk)
            {
                var nextGuide = GuideConfigManager.inst.GetConfig(GuideDataProxy.inst.CurInfo.m_curCfg.next_id);
                if ((K_Guide_Type)nextGuide.guide_type == K_Guide_Type.EmptyOperation)
                {
                    nextGuide = GuideConfigManager.inst.GetConfig(nextGuide.next_id);
                }
                if ((K_Guide_Type)nextGuide.guide_type == K_Guide_Type.NPCCreat)
                {
                    nextGuide = GuideConfigManager.inst.GetConfig(nextGuide.next_id);
                }
                if (nextGuide != null && ((K_Guide_Type)nextGuide.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)nextGuide.guide_type == K_Guide_Type.TipsAndRestrictClick || (K_Guide_Type)nextGuide.guide_type == K_Guide_Type.FullScreenDialog))
                {
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
                }
                hideGNewTask();
            }
        });
    }

    public void showGNewTask(bool isOk)
    {
        this.isOk = isOk;
        if (!gameObject.activeSelf)
        {
            float movePosX = FGUI.inst.isLandscape ? -600 : -185;
            moveTrans.DOLocalMoveX(movePosX, 0.5f).From(-900).OnStart(() =>
            {
                gameObject.SetActive(true);
            });
        }

        var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
        var taskCfg = GuideTaskConfigManager.inst.GetConfig(cfg.task_id);
        if (taskCfg != null)
        {
            contentText.text = LanguageManager.inst.GetValueByKey(taskCfg.desc);
        }
        selfBtn.gameObject.GetComponent<Image>().material = isOk ? liuguang : null;
        if (!isOk)
        {
            c.sortingLayerName = "window";
            c.sortingOrder = 0;
            //selfBtn.GetComponent<Image>().color = Color.white;
            //btnText.text = LanguageManager.inst.GetValueByKey("未完成");
        }
        else
        {
            c.sortingLayerName = "popup_2";
            c.sortingOrder = 1;
            //selfBtn.GetComponent<Image>().color = Color.green;
            //btnText.text = LanguageManager.inst.GetValueByKey("完成");
        }

    }

    public void hideGNewTask()
    {
        gameObject.SetActive(false);
    }
}
