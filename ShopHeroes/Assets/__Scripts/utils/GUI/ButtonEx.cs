using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonEx : MonoBehaviour, IPointerClickHandler
{
    public static float globalCoolTime = 0.7f;
    public float clickDistance = 0.5f;

    public int ButtonClickSoundId = 8;

    public bool isNeedCallBtnBack = false; //lua那边的按钮点击响应会被OnPointerClick吞掉


    private double lastClickTime = 0;
    bool inCoolTime = false;
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_button.interactable)
        {
            if (ButtonClickSoundId > 0)
                AudioManager.inst.PlaySound(ButtonClickSoundId);

            if (clickDistance <= 0) return;

            lastClickTime = 0;
            inCoolTime = true;
            _button.interactable = false;

            // 额外添加
            if (isNeedCallBtnBack && _button.enabled)
            {
                _button.onClick.Invoke();
            }
        }
    }

    void Update()
    {
        if (inCoolTime && !_button.interactable)
        {
            if (lastClickTime >= clickDistance)
            {
                _button.interactable = true;
                inCoolTime = false;
                lastClickTime = -6666;
            }
            else
            {
                lastClickTime += Time.deltaTime;
            }
        }
    }
}
