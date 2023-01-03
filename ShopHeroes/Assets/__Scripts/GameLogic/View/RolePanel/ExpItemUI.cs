using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;
using System;

public class ExpItemUI : MonoBehaviour, IDynamicScrollViewItem
{
    public Text addExpText;
    public Text numText;
    public Text nameText;
    public GUIIcon icon;
    private Button selfBtn;
    public GameObject selectObj;
    public int index = 0;
    int itemId;
    public Action<ExpItemUI, int> clickHandler;
    private void Awake()
    {
        selfBtn = gameObject.GetComponent<Button>();
        selfBtn.onClick.AddListener(() =>
        {
            selectObj.SetActive(true);
            clickHandler?.Invoke(this, itemId);
        });
    }
    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    public void setData(Item itemData)
    {
        //Logger.error(itemData.itemConfig.name + itemData.itemConfig.effect + itemData.count);
        itemId = itemData.itemConfig.id;
        selectObj.SetActive(itemId == RoleDataProxy.inst.curSelectItemId);
        addExpText.text = "+" + itemData.itemConfig.effect + "EXP";
        numText.text = itemData.count.ToString();
        nameText.text = LanguageManager.inst.GetValueByKey(itemData.itemConfig.name);
        icon.SetSprite(itemData.itemConfig.atlas, itemData.itemConfig.icon);
    }
}
