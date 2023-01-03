using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class HealthEffect : MonoBehaviour
{
    public GUIIcon vfxImagerTF;
    public EffectText subTextTF;
    public EffectText addTextTF;
    public EffectText artisticTF;
    public Text shanbiText;
    private Animator _animator;
    void Awake()
    {
        vfxImagerTF.gameObject.SetActive(false);
        subTextTF.gameObject.SetActive(false);
        addTextTF.gameObject.SetActive(false);
        artisticTF.gameObject.SetActive(false);
        _animator = GetComponent<Animator>();

        _animator.CrossFade("start", 0f);
        _animator.Update(0f);
        _animator.Play("start");
    }

    private string scriptName;
    private int changeHp;
    public bool isCurrentHp = false;
    public void ShowFx(string scriptname, int changehp)
    {
        scriptName = scriptname;
        changeHp = changehp;
    }

    void Start()
    {
        if (string.IsNullOrEmpty(scriptName))
        {
            if (changeHp > 0)
            {
                addTextTF._text.color = GUIHelper.GetColorByColorHex("7BFF1A");
                addTextTF._text.text = "+" + changeHp.ToString();
                addTextTF._text.fontSize = 72;
                addTextTF.gameObject.SetActive(true);
            }
            else if (changeHp < 0)
            {
                subTextTF._text.color = isCurrentHp ? GUIHelper.GetColorByColorHex("FF4040") : Color.white;
                subTextTF._text.text = (isCurrentHp ? LanguageManager.inst.GetValueByKey("暴击  ") : "") + changeHp.ToString();
                subTextTF._text.fontSize = 72;
                subTextTF.gameObject.SetActive(true);
            }
        }
        else
        {
            // if (changeHp == 0)
            {
                vfxImagerTF.gameObject.SetActive(true);
                shanbiText.text = LanguageManager.inst.GetValueByKey("未命中");
                // vfxImagerTF.SetSprite("Artistic_CN", scriptName);
            }
        }

        _animator.CrossFade("show", 0f);
        _animator.Update(0f);
        _animator.Play("show");
        this.transform.DOLocalMoveY(0, 2f);
        GameObject.Destroy(gameObject, 3f);
    }
}
