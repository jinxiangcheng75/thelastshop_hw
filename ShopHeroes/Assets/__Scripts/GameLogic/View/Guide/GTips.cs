using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GTips : MonoBehaviour
{
    public GUIIcon icon;
    public Text contentText;

    public Transform modelPos;
    public RectTransform tipsBgRect;

    GraphicDressUpSystem graphicDressUp;

    int animIndex = 0;

    private void Awake()
    {
        //icon.GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    GuideDataProxy.inst.CurInfo.isDialogFinish = true;
        //    GuideManager.inst.GuideManager_OnNextGuide();
        //});
    }

    public void showGTips()
    {
        gameObject.SetActiveTrue();
        var cfg = GuideDataProxy.inst.CurInfo;
        var workerCfg = ArtisanNPCConfigManager.inst.GetConfig(cfg.m_curCfg.character_id);

        //icon.SetSprite(StaticConstants.guideAtlas, workerCfg.pic, "", true);
        contentText.text = LanguageManager.inst.GetValueByKey(cfg.m_curCfg.conditon_param_5);

        if (cfg.m_curCfg.conditon_param_3 != null)
        {
            if (!FGUI.inst.isLandscape)
            {
                int anchorePosY = 360;
                if (int.TryParse(cfg.m_curCfg.conditon_param_3, out anchorePosY))
                {
                    tipsBgRect.anchoredPosition = new Vector2(0, -anchorePosY);
                }
            }
            else
            {
                tipsBgRect.anchoredPosition = new Vector2(0, -190);
            }
        }

        creatNPCModel(workerCfg.model);
    }

    private void creatNPCModel(int modelId)
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacterByModel<GraphicDressUpSystem>(modelId, callback: (dressUpSystem) =>
            {
                graphicDressUp = dressUpSystem;
                graphicDressUp.transform.SetParent(modelPos.transform);
                graphicDressUp.transform.localScale = Vector3.one * 0.5f;
                graphicDressUp.transform.localPosition = new Vector3(0, 95, 0);
                setRandomAnim();
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByModel(graphicDressUp, modelId);
            setRandomAnim();
        }
    }

    private void setRandomAnim()
    {
        string randomAnimName = "";
        if (animIndex % 2 == 0)
        {
            randomAnimName = "idle_sp_1";
        }
        else
        {
            randomAnimName = "idle_2";
        }

        animIndex++;

        if (graphicDressUp != null)
        {
            graphicDressUp.Play(randomAnimName, completeDele: (t) =>
              {
                  string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.normal_standby);
                  graphicDressUp.Play(idleAnimationName, true);
              });
        }
    }

    public void hideGTips()
    {
        gameObject.SetActiveFalse();
        string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.normal_standby);
        graphicDressUp.Play(idleAnimationName, true);
    }
}
