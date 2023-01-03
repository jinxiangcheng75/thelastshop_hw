using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class CharacterAttacher : MonoBehaviour
{


    public Transform actorParent;

    public Collider2D collider2d;
    public InputEventListener mouseListener;
    public Transform nodeRoot;

    public Transform spRoot;
    public SpriteRenderer sp_bgIcon;
    public SpriteRenderer sp_icon;
    public SpriteRenderer sp_countBg;
    public TextMeshPro countText;
    public Animator spAnim;

    public Transform talkRoot;
    public TextMeshPro talkTx;
    public Animator talkAnim;

    [HideInInspector]
    public bool isShowTalkPop;
    [HideInInspector]
    public bool headBubbleIsShow = false;


    public Action onClickHandler;

    void Awake()
    {
        if (mouseListener != null) mouseListener.OnClick = onClick;
    }

    public void ShowSpIcon(Sprite sp, int count, bool showCountTx, bool outline, in Color outlineColor, bool needTile = false, float spScale = 1.6f)
    {
        if (null == this) return;


        bool playAnim = !spRoot.gameObject.activeSelf || spRoot.transform.localScale.x == 0;

        spRoot.gameObject.SetActive(true);
        talkRoot.gameObject.SetActive(false);

        if (playAnim && spAnim.isActiveAndEnabled)
        {
            spAnim.CrossFade("show", 0f);
            spAnim.Update(0f);
            spAnim.Play("show");
        }

        if (isTalking) StopTalk();

        headBubbleIsShow = true;

        //if (outline)
        //{
        //sp_icon.material = GUIHelper.GetOutlineMat();
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetColor("_OutlineColor", !outline ? GUIHelper.GetColorByColorHex("000000") : outlineColor);
        materialPropertyBlock.SetFloat("_Width", 0.002f);
        sp_icon.SetPropertyBlock(materialPropertyBlock);
        //}
        //else
        //{
        //    if (sp_icon.material != sp_bgIcon.material)
        //    {
        //        sp_icon.material = new Material(sp_bgIcon.material);
        //    }
        //}

        sp_icon.sprite = sp;

        if (needTile && sp != null)
        {
            Vector3 localScale = Vector3.zero;
            localScale.x = spScale / sp.bounds.size.x;
            localScale.y = spScale / sp.bounds.size.y;
            sp_icon.transform.localScale = localScale;
        }
        else
        {
            sp_icon.transform.localScale = Vector3.one * spScale;
        }

        if (showCountTx)
        {
            countText.enabled = count > 1;
            countText.text = count > 1 ? "x" + count : "";
        }
        else
        {
            countText.enabled = false;
            countText.text = "";
        }

        if (sp_countBg != null)
        {
            sp_countBg.enabled = countText.preferredWidth > 0;
            sp_countBg.size = new Vector2(countText.preferredWidth <= 0 ? 0 : countText.preferredWidth + 0.13f, 0.6f);
        }

    }

    public void SetSpBgIcon(Sprite sp)
    {
        if (this == null) return;
        sp_bgIcon.sprite = sp;
    }


    private void setTalkMsg(string msg)
    {
        if (null == this) return;

        //先去掉气泡中文字的打字机效果
        talkTx.text = msg;

        //talkTx.DOText(msg.IfNullThenEmpty(), 0.7f).From("");
    }

    private int talkMsgTimer;
    private void showTalkPop(string str)
    {
        if (null == this) return;

        spRoot.gameObject.SetActive(false);
        talkRoot.gameObject.SetActive(true);
        isShowTalkPop = true;

        if (talkAnim.isActiveAndEnabled)
        {
            talkAnim.CrossFade("show", 0f);
            talkAnim.Update(0f);
            talkAnim.Play("show");
        }
        headBubbleIsShow = true;

        //talkTx.gameObject.SetActive(false);
        //float animLength = talkAnim.GetClipLength("actorAttacher_show");

        //talkMsgTimer = GameTimer.inst.AddTimer(animLength, 1, () =>
        //{
        //    if (null == this) return;
        //    talkTx.gameObject.SetActive(true);
        //    setTalkMsg(str);
        //});
        setTalkMsg(str);

    }

    private int talkTimer;
    [HideInInspector]
    public float talkSpacing = 2f; //说话间隔 默认2f
    public bool isTalking;

    public void Talk(string msg, Action talkComplete = null)
    {
        if (string.IsNullOrEmpty(msg)) return;
        isTalking = true;

        clearTimer();
        showTalkPop(msg);

        Action _talkComplete = StopTalk;
        if (talkComplete != null) _talkComplete += talkComplete;

        talkTimer = GameTimer.inst.AddTimer(talkSpacing, 1, _talkComplete);
    }

    private string[] talkMsgs;
    private int talkIndex;
    Action _stepComplete, _endComplete;
    public void Talk(string[] msgs, Action stepComplete = null, Action endComplete = null)
    {
        if (msgs == null || msgs.Length == 0) return;
        clearTimer();

        _stepComplete = stepComplete;
        _endComplete = endComplete;

        isTalking = true;
        talkMsgs = msgs;
        talkIndex = 0;

        NextTalk();
    }

    private void NextTalk()
    {
        if (!isTalking) return;

        if (talkIndex >= talkMsgs.Length)
        {
            StopTalk();
            _endComplete?.Invoke();
            return;
        }

        string msg = talkMsgs[talkIndex];
        if (isShowTalkPop)
        {
            setTalkMsg(msg);
        }
        else
        {
            showTalkPop(msg);
            _stepComplete?.Invoke();
        }

        talkIndex++;


        talkTimer = GameTimer.inst.AddTimer(talkSpacing, 1, NextTalk);
    }

    public void StopTalk()
    {
        HidePopup();
    }

    public void HidePopup(System.Action hidePopupHandler = null)
    {
        if (null == this) return;
        if (!headBubbleIsShow)
        {
            hidePopupHandler?.Invoke();
            return;
        }

        clearTimer();

        if (isShowTalkPop)
        {
            talkTx.text = ""; //气泡字体先置空

            talkAnim.Play("hide");
            headBubbleIsShow = false;
            float animLength = talkAnim.GetClipLength("actorAttacher_hide");


            GameTimer.inst.AddTimer(animLength, 1, () =>
            {
                if (null == this) return;
                hidePopupHandler?.Invoke();
                isShowTalkPop = false;
                isTalking = false;
                talkRoot.gameObject.SetActive(false);
            });

        }
        else
        {
            spAnim.Play("hide");
            headBubbleIsShow = false;
            float animLength = talkAnim.GetClipLength("actorAttacher_hide");

            GameTimer.inst.AddTimer(animLength, 1, () =>
            {
                hidePopupHandler?.Invoke();
                if (null == this)
                {
                    spRoot.gameObject.SetActive(false);
                }
            });

        }

    }
    void onClick(Vector3 mousepos)
    {
        onClickHandler?.Invoke();
    }

    public void SetVisible(bool visible)
    {
       
        nodeRoot.gameObject.SetActive(visible);

        if (visible)
        {
            var renders = nodeRoot.GetComponentsInChildren<SpriteRenderer>();
            foreach (var item in renders)
            {
                item.DOFade(1, 0.35f).From(0);
            }
        }
        else
        {
            if (spAnim != null && spAnim.isActiveAndEnabled)
            {
                spAnim.CrossFade("null", 0f);
                spAnim.Update(0f);
                spAnim.transform.rotation = Quaternion.Euler(Vector3.zero);
            }
        }

        mouseListener.enabled = visible;
    }

    public void SetSpBubbleAlpha(float alpha, float duration)
    {
        var renders = spRoot.GetComponentsInChildren<SpriteRenderer>();

        foreach (var item in renders)
        {
            item.DOFade(alpha, duration);
        }

        countText.DOFade(alpha, duration);
    }

    private void clearTimer()
    {
        if (talkMsgTimer != 0)
        {
            GameTimer.inst.RemoveTimer(talkMsgTimer);
            talkMsgTimer = 0;
        }

        if (talkTimer != 0)
        {
            GameTimer.inst.RemoveTimer(talkTimer);
            talkTimer = 0;
        }
    }

    private void OnDestroy()
    {
        clearTimer();
    }

}
