using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GFullDialog : MonoBehaviour
{
    public RectTransform qipaoRect;
    public Transform qipaoBgTrans;
    public GUIIcon qipaoIcon;
    public GameObject arrow1;
    public GameObject arrow2;
    public Text nameText;
    public Text nameText2;
    public GameObject nameBg1;
    public GameObject nameBg2;
    public Text contentText;
    public Button nextBtn;
    public GameObject bgObj;
    public GameObject modelPos1;
    public GameObject modelPos2;
    public Image bg;

    bool canClick = false;
    string[] contents;
    int index = 0;

    int animIndex = 0;

    bool curIsNpc1;
    GraphicDressUpSystem graphicDressUp;
    GraphicDressUpSystem graphicDressUp2;
    bool isTrigger = false;

    public RectTransform btnRect;
    float contentTextWidth;

    Tween talkTween;

    public float finishTime = 0.3f;

    private void Awake()
    {
        bg = gameObject.GetComponent<Image>();

        nextBtn.onClick.AddListener(() =>
        {
            if (canClick)
                nextContent();
            else
            {
                if (talkTween != null && talkTween.IsPlaying())
                {
                    talkTween.fullPosition = finishTime;
                }
            }
        });

        contentTextWidth = contentText.GetComponent<RectTransform>().sizeDelta.x;
    }

    int workerId1 = -1;
    int workerId2 = -1;
    public void showFullDialog()
    {
        if (GuideDataProxy.inst == null) return;
        if (GuideDataProxy.inst.CurInfo == null) return;
        if (GuideDataProxy.inst.CurInfo.m_curCfg == null) return;

        if (bg != null)
        {
            bg.color = new Color(0, 0, 0, GuideDataProxy.inst.CurInfo.m_curCfg.mask_color == 0 ? 0 : GuideDataProxy.inst.CurInfo.m_curCfg.mask_color / 100.0f);
        }

        isTrigger = false;
        gameObject.SetActiveTrue();
        if (GuideDataProxy.inst.needWaitTime > 0)
        {
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, false);
            if ((K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.FullScreenDialog)
                setContentData();
            GuideDataProxy.inst.needWaitTime = 0;
        }
        else
        {
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, false);
            setContentData();
        }
    }

    private void setContentData()
    {
        AudioManager.inst.PlaySound(28);
        var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
        if (cfg == null)
        {
            return;
        }
        var workerCfg = ArtisanNPCConfigManager.inst.GetConfig(cfg.character_id);
        if (workerCfg == null)
        {
            Logger.error("workerCfg是空的   guideid = " + cfg.id + " desc = " + cfg.desc + "   npcId = " + cfg.character_id);
            return;
        }
        if (cfg.conditon_param_1 == null) return;

        if (cfg.conditon_param_2 != null)
        {
            qipaoRect.gameObject.SetActive(true);
            qipaoRect.DOScale(new Vector3(1, 1, 1), 0.5f).From(0).SetEase(Ease.OutBack);
            qipaoIcon.SetSpriteURL(cfg.conditon_param_2, needSetNativeSize: true);
        }
        else
        {
            qipaoRect.gameObject.SetActive(false);
        }

        if (int.TryParse(cfg.conditon_param_1, out int result))
        {
            if (result == 0)
            {
                //if (graphicDressUp != null)
                //    graphicDressUp.SkeletonGraphic.color = Color.white;
                //if (graphicDressUp2 != null)
                //    graphicDressUp2.SkeletonGraphic.color = GUIHelper.GetColorByColorHex("555555");
                arrow1.SetActiveTrue();
                arrow2.SetActiveFalse();
                nameBg1.SetActiveTrue();
                nameBg2.SetActiveFalse();
                modelPos1.SetActive(true);
                modelPos2.SetActive(false);

                if (cfg.conditon_param_2 != null)
                {
                    qipaoRect.anchoredPosition = new Vector2(-61, 190);
                    qipaoBgTrans.localScale = new Vector3(-1, 1, 1);
                }

                if (workerId1 != cfg.character_id)
                {
                    creatNpcModel(workerCfg.model, true);
                    workerId1 = cfg.character_id;
                    nameText.text = LanguageManager.inst.GetValueByKey(workerCfg.name);
                }
            }
            else if (result == 1)
            {
                //if (graphicDressUp != null)
                //    graphicDressUp.SkeletonGraphic.color = GUIHelper.GetColorByColorHex("555555");
                //if (graphicDressUp2 != null)
                //    graphicDressUp2.SkeletonGraphic.color = Color.white;
                arrow1.SetActiveFalse();
                arrow2.SetActiveTrue();
                nameBg2.SetActiveTrue();
                nameBg1.SetActiveFalse();
                modelPos2.SetActive(true);
                modelPos1.SetActive(false);

                if (cfg.conditon_param_2 != null)
                {
                    qipaoRect.anchoredPosition = new Vector2(61, 190);
                    qipaoBgTrans.localScale = new Vector3(1, 1, 1);
                }

                if (workerId2 != cfg.character_id)
                {
                    creatNpcModel(workerCfg.model, false);
                    workerId2 = cfg.character_id;
                    nameText2.text = LanguageManager.inst.GetValueByKey(workerCfg.name);
                }
            }
        }
        else
        {
            Logger.error("int parse error : cfg.conditon_param_1 = " + cfg.conditon_param_1);
        }

        contents = LanguageManager.inst.GetValueByKey(cfg.conditon_param_5).Split('|');
        if (contents == null)
        {
            Logger.error("解析之后的引导对话数据是空的" + "id是" + cfg.id);
            return;
        }
        index = 0;
        if (contents.Length > 0)
        {
            talkTween = contentText.DOText(contents[index], finishTime).From("").OnStart(() =>
            {
                if (GUIHelper.CalculateLengthOfText(contents[index], contentText) > contentTextWidth)
                {
                    contentText.alignment = TextAnchor.MiddleLeft;
                }
                else
                {
                    contentText.alignment = TextAnchor.MiddleCenter;
                }
                //nextBtn.interactable = false;
                canClick = false;
            }).OnComplete(() =>
            {
                //nextBtn.interactable = true;
                canClick = true;
            });
        }
    }

    private void creatNpcModel(int modelId, bool isNpc1)
    {
        curIsNpc1 = isNpc1;
        if (isNpc1)
        {
            if (graphicDressUp == null)
            {
                CharacterManager.inst.GetCharacterByModel<GraphicDressUpSystem>(modelId, callback: (dressUpSystem) =>
                {
                    graphicDressUp = dressUpSystem;
                    graphicDressUp.transform.SetParent(modelPos1.transform);
                    graphicDressUp.transform.localScale = Vector3.one * 1.3f;
                    graphicDressUp.transform.localPosition = Vector3.zero;
                    setRandomAnim(graphicDressUp);
                });
            }
            else
            {
                CharacterManager.inst.ReSetCharacterByModel(graphicDressUp, modelId);
                setRandomAnim(graphicDressUp);
            }
        }
        else
        {
            if (graphicDressUp2 == null)
            {
                CharacterManager.inst.GetCharacterByModel<GraphicDressUpSystem>(modelId, callback: (dressUpSystem) =>
                {
                    graphicDressUp2 = dressUpSystem;
                    graphicDressUp2.transform.SetParent(modelPos2.transform);
                    graphicDressUp2.transform.localScale = Vector3.one * 1.3f;
                    graphicDressUp2.transform.localPosition = Vector3.zero;
                    setRandomAnim(graphicDressUp2);
                });
            }
            else
            {
                CharacterManager.inst.ReSetCharacterByModel(graphicDressUp2, modelId);
                setRandomAnim(graphicDressUp2);
            }
        }
    }

    private void nextContent()
    {
        if (contents == null) return;
        if (ManagerBinder.inst.stateIsChanging) return;
        index++;
        AudioManager.inst.PlaySound(62);
        if (index < contents.Length)
        {
            talkTween = contentText.DOText(contents[index], finishTime).From("").OnStart(() =>
            {
                if (GUIHelper.CalculateLengthOfText(contents[index], contentText) > contentTextWidth)
                {
                    contentText.alignment = TextAnchor.MiddleLeft;
                }
                else
                {
                    contentText.alignment = TextAnchor.MiddleCenter;
                }
                //nextBtn.interactable = false;
                canClick = false;
            }).OnComplete(() =>
            {
                //nextBtn.interactable = true;
                canClick = true;
            });
        }
        else
        {
            index = 0;
            if (!isTrigger)
            {
                GuideDataProxy.inst.CurInfo.isDialogFinish = true;
                bool needNpcLeave = GuideDataProxy.inst.CurInfo.m_curCfg.conditon_param_6 != null;
                if (needNpcLeave)
                {
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.NPCLEAVE);
                }
                if (GuideDataProxy.inst.CurInfo.m_curCfg.next_id != 0)
                {
                    var cfg = GuideConfigManager.inst.GetConfig(GuideDataProxy.inst.CurInfo.m_curCfg.next_id);
                    if (cfg != null)
                    {
                        if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)cfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
                        {
                            EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
                        }
                        else if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.NPCCreat)
                        {
                            if (cfg.next_id != 0)
                            {
                                cfg = GuideConfigManager.inst.GetConfig(cfg.next_id);
                                if ((K_Guide_Type)cfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)cfg.guide_type == K_Guide_Type.TipsAndRestrictClick)
                                {
                                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
                                }
                            }
                        }
                    }
                }
                if (curIsNpc1)
                {
                    string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.normal_standby);
                    graphicDressUp.Play(idleAnimationName, true);
                }
                else
                {
                    string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp2.gender, (int)kIndoorRoleActionType.normal_standby);
                    graphicDressUp2.Play(idleAnimationName, true);
                }
                canClick = false;
                GuideManager.inst.GuideManager_OnNextGuide();
            }
        }
    }

    private void setRandomAnim(GraphicDressUpSystem thisGraphicDressUpSystem)
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

        if (thisGraphicDressUpSystem != null)
        {
            thisGraphicDressUpSystem.Play(randomAnimName, completeDele: (t) =>
            {
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)thisGraphicDressUpSystem.gender, (int)kIndoorRoleActionType.normal_standby);
                thisGraphicDressUpSystem.Play(idleAnimationName, true);
            });
        }
    }

    public void hideFullDialog()
    {
        index = 0;
        workerId1 = -1;
        workerId2 = -1;
        arrow1.SetActiveFalse();
        arrow2.SetActiveFalse();
        nameBg1.SetActiveFalse();
        nameBg2.SetActiveFalse();
        gameObject.SetActiveFalse();
        modelPos1.SetActive(false);
        modelPos2.SetActive(false);
        DOTween.Kill(talkTween, true);
    }
}
