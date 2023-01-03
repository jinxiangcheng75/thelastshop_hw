using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectBagItem : MonoBehaviour
{
    public ExploreIntensifyItem exploreItem;
    public TreasureBoxItem boxItem;
    /// <summary>
    /// 1 -- 副本强化 2 -- 宝箱选择
    /// </summary>
    /// <param name="type"></param>
    public void setExploreItemData(Item item, System.Action<int> clickHandler)
    {
        exploreItem.gameObject.SetActive(true);
        boxItem.gameObject.SetActive(false);

        exploreItem.setData(item, clickHandler);
    }

    public void setTreasureBoxItemData(TreasureBoxData box, System.Action<int> clickHandler)
    {
        exploreItem.gameObject.SetActive(false);
        boxItem.gameObject.SetActive(true);

        boxItem.setData(box, clickHandler);
    }
}
