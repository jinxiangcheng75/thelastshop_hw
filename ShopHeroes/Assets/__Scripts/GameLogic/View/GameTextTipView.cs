using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

public class GameTextTipView : ViewBase<TextTipcomp>
{
    public override string viewID => ViewPrefabName.TextTip;
    float delayTime = 2f;
    float upMoveTime = 0.5f;

    protected override void onInit()
    {
        topResPanelType = TopPlayerShowType.ignore;
    }

    protected override void onShown()
    {
        base.onShown();
    }

    public void ShowText(string msg, Color color)
    {
        if (!isShowing) return;


        var newGo = GameObject.Instantiate(contentPane.rect.gameObject, contentObject.transform);

        Text _text = newGo.transform.Find("Text").GetComponent<Text>();
        _text.font = LanguageManager.inst.curFont;
        _text.color = color;
        _text.text = msg;

        RectTransform rectTransform = newGo.transform as RectTransform;

        if (_text.preferredWidth > Screen.width)
        {
            var textRect = _text.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(Screen.width, textRect.sizeDelta.y);
            _text.horizontalOverflow = HorizontalWrapMode.Wrap;
            rectTransform.sizeDelta = new Vector2(Screen.width, _text.preferredHeight + 20);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(_text.preferredWidth + 120, _text.preferredHeight + 20);
        }


        newGo.SetActiveTrue();

        float endY = rectTransform.anchoredPosition3D.y + 100;

        rectTransform.DOAnchorPos3DY(endY, upMoveTime).SetDelay(delayTime).OnStart(() =>
        {
            DoTweenUtil.Fade_a_To_0_All(rectTransform, 1, upMoveTime, false);
        }).OnComplete(() => 
        {
            GameObject.Destroy(newGo);
        });

        //this.DelayHide();
    }

    int delayTimerid = -1;


    private void DelayHide()
    {
        if (delayTimerid >= 0)
        {
            GameTimer.inst.RemoveTimer(delayTimerid);
            delayTimerid = -1;
        }
        delayTimerid = GameTimer.inst.AddTimer(2, 1, () =>
        {
            hide();
        });
    }
}
