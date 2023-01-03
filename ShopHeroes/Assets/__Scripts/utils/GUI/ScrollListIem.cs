using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ScrollListIem : UIBehaviour, Mosframe.IDynamicScrollViewItem
{
    public int index;
    public System.Action<int, Mosframe.IDynamicScrollViewItem> clickCallback;
    public void onUpdateItem(int index)
    {

    }
    public void AddItemOnClickEvent(Button button, System.Action<int, Mosframe.IDynamicScrollViewItem> _clickCallback)
    {
        if (button == null) return;
        clickCallback = _clickCallback;
        if (!button.onClick.IsNull())
        {
            button.onClick.RemoveListener(ItemOnClick);
        }
        button.onClick.AddListener(ItemOnClick);
    }
    void ItemOnClick()
    {
        clickCallback?.Invoke(index, this);
    }
}
