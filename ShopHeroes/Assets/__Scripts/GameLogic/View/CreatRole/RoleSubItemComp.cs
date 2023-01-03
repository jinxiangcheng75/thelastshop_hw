using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;
using UnityEngine.EventSystems;

public class RoleSubItemComp : MonoBehaviour, IDynamicScrollViewItem
{
    public Action<RoleSubItemComp, bool> clickHandle;
    public OverrideAnimatorButton selfBtn;
    public Image icon;
    public GameObject isSelect;
    public GameObject highLightObj;
    Color curColor;
    private RoleSubTypeData _data;
    int timerId = 0;
    public RoleSubTypeData Data { get { return _data; } set { } }

    public int index = 0;
    public Animator btnAnim;

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    private void Awake()
    {
        selfBtn.isItem = true;
        selfBtn.onClick.AddListener(() =>
        {
            clickHandle?.Invoke(this, true);
        });
    }

    public void InitData(RoleSubTypeData data, Action<RoleSubItemComp, bool> clickAction, int index)
    {
        clickHandle = clickAction;
        highLightObj.SetActive(data.isSelect);
        isSelect.SetActive(data.isSelect);
        ColorUtility.TryParseHtmlString("#9F5B9B", out curColor);
        gameObject.SetActive(true);
        this._data = data;

        if (!string.IsNullOrEmpty(data.config.val))
        {
            Color tempColor;
            ColorUtility.TryParseHtmlString(data.config.val, out tempColor);
            icon.GetComponent<GUIIcon>().SetSprite(CreatRoleProxy.inst.atlasName, "dianzhu_chunbaiyuan");
            icon.color = tempColor;
        }
        else if (data.config.name.StartsWith("无"))
        {
            icon.color = Color.white;
            icon.GetComponent<GUIIcon>().SetSprite(CreatRoleProxy.inst.atlasName, "huanzhuang_buxuanze");
        }
        else
        {
            string iconName = data.config.icon;

            if (iconName != "")
                icon.GetComponent<GUIIcon>().SetSprite("ClotheIcon_atlas", iconName);
            else
                icon.sprite = null;
            icon.color = Color.white;
        }

        //if (data.isSelect)
        //{
        //    highLightObj.SetActive(true);
        //    isSelect.SetActive(true);

        //    //if (CreatRoleProxy.inst._lastItem == null || data.config.type_2 != CreatRoleProxy.inst._lastItem.GetComponent<RoleSubItemComp>().Data.config.type_2)
        //    CreatRoleProxy.inst._lastItem = this.gameObject;
        //}
        //else
        //{
        //    highLightObj.SetActive(false);
        //    isSelect.SetActive(false);
        //}

        if (data.isSelect)
        {
            clickHandle?.Invoke(this, false);
        }

        if (index < 8)
            showAnim(index);
    }

    public void Clear()
    {
        _data = null;
        icon.color = Color.white;
        icon.sprite = null;
        isSelect.SetActive(false);
        gameObject.SetActive(false);
        highLightObj.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //clickHandle?.Invoke(this.gameObject);
    }

    void showAnim(int index)
    {
        btnAnim.enabled = true;
        gameObject.SetActive(false);
        timerId = GameTimer.inst.AddTimer(0.05f + 0.05f * index, 1, () =>
        {
            gameObject.SetActive(true);
            btnAnim.CrossFade("show", 0f);
            btnAnim.Update(0f);
            btnAnim.Play("show");
        });
    }

    public void clearAnim()
    {
        btnAnim.enabled = false;
        gameObject.SetActive(false);
        GameTimer.inst.RemoveTimer(timerId);
    }
}
