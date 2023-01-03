using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainLineTaskDialog : MonoBehaviour
{
    public Button bgNextBtn;
    public Transform npcTrans;
    public Text contentText;

    GraphicDressUpSystem graphicDressUp;
    string[] contents;
    int index = 0;

    float contentTextWidth;
    private void Awake()
    {
        bgNextBtn.onClick.AddListener(() =>
        {
            nextContentData();
        });

        contentTextWidth = contentText.GetComponent<RectTransform>().sizeDelta.x;
    }

    public void setData(string talkStr)
    {
        gameObject.SetActive(true);
        if (graphicDressUp == null)
        {
            creatNpcModel();
        }
        initContentData(talkStr);
    }

    private void creatNpcModel()
    {
        CharacterManager.inst.GetCharacterByModel<GraphicDressUpSystem>(60000, callback: (dressUpSystem) =>
        {
            graphicDressUp = dressUpSystem;
            graphicDressUp.transform.SetParent(npcTrans);
            graphicDressUp.transform.localScale = Vector3.one * 1.3f;
            graphicDressUp.transform.localPosition = Vector3.zero;
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.normal_standby);
            graphicDressUp.Play(idleAnimationName, true);
        });
    }

    private void initContentData(string talkStr)
    {
        contents = LanguageManager.inst.GetValueByKey(talkStr).Split('|');
        if (contents == null)
        {
            Logger.error("主线任务的对话是空的");
            return;
        }
        index = 0;
        if (contents.Length > 0)
        {
            contentText.DOText(contents[index], 0.5f).From("").OnStart(() =>
            {
                if (GUIHelper.CalculateLengthOfText(contents[index], contentText) > contentTextWidth)
                {
                    contentText.alignment = TextAnchor.MiddleLeft;
                }
                else
                {
                    contentText.alignment = TextAnchor.MiddleCenter;
                }
                string talkAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.haggle);
                graphicDressUp.Play(talkAnimationName, true);
            }).OnComplete(() =>
            {
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.normal_standby);
                graphicDressUp.Play(idleAnimationName, true);
            });
        }
    }

    private void nextContentData()
    {
        if (contents == null) return;
        index++;
        AudioManager.inst.PlaySound(62);
        if (index < contents.Length)
        {
            contentText.DOText(contents[index], 0.5f).From("").OnStart(() =>
            {
                if (GUIHelper.CalculateLengthOfText(contents[index], contentText) > contentTextWidth)
                {
                    contentText.alignment = TextAnchor.MiddleLeft;
                }
                else
                {
                    contentText.alignment = TextAnchor.MiddleCenter;
                }
                string talkAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.haggle);
                graphicDressUp.Play(talkAnimationName, true);
            }).OnComplete(() =>
            {
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.normal_standby);
                graphicDressUp.Play(idleAnimationName, true);
            });
        }
        else
        {
            index = 0;
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)graphicDressUp.gender, (int)kIndoorRoleActionType.normal_standby);
            graphicDressUp.Play(idleAnimationName, true);
            gameObject.SetActive(false);
        }
    }
}

