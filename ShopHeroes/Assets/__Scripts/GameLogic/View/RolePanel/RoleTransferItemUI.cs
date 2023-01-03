using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleTransferItemUI : MonoBehaviour
{
    public GUIIcon icon;
    public GUIIcon selectIcon;
    public Text selectText;
    public HeroProfessionConfigData roleData;

    public void setData(HeroProfessionConfigData roleData)
    {
        gameObject.name = roleData.id.ToString();
        this.roleData = roleData;
        selectText.text = LanguageManager.inst.GetValueByKey(roleData.name);
        icon.SetSprite(roleData.atlas, roleData.ocp_icon);
        selectIcon.SetSprite(roleData.atlas, roleData.ocp_icon);
    }
}
