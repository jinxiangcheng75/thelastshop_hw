using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleTransferUnlockItem : MonoBehaviour
{
    public GUIIcon icon;
    public Image img;

    public void setData(string atlas, string icon)
    {
        if (atlas != null)
        {
            //atlas = "Assets/GUI2D/SpriteAtlas/" + atlas;
            //img.sprite = ManagerBinder.inst.Asset.getSprite(atlas, icon);
            //img.SetNativeSize();
            this.icon.SetSprite(atlas, icon, "", true);
        }
        //this.icon.SetSprite(atlas, icon);
        //this.icon.GetComponent<Image>().SetNativeSize();
    }
}
