using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo
{
    public bool isShow;
    public string atlas;
    public string icon;
    public string itemName;
    public string oldValue;
    public string newValue;
}

public class UpgradeFinishItemComp : MonoBehaviour
{
    public GUIIcon itemIconImg;

    public Text itemName;
    public Text oldValue;
    public Text newValue;
}
