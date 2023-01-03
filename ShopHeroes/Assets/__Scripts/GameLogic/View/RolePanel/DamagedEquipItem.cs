using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagedEquipItem : MonoBehaviour
{
    public GUIIcon bgIcon;
    public GUIIcon icon;
    public Text classText;
    public Button selfBtn;
    public GameObject superNormalObj;

    public int equipId;

    private void Awake()
    {
        selfBtn.ButtonClickTween(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.SETDAMAGEDEQUPINTRODUCE, transform, equipId);
        });
    }

    public void setData(int equipId)
    {
        this.equipId = equipId;

        var cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipId);
        classText.text = cfg.equipDrawingsConfig.level.ToString();
        bgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleEquipQualityIconName[cfg.equipQualityConfig.quality - 1]);
        icon.SetSprite(cfg.equipDrawingsConfig.atlas, cfg.equipDrawingsConfig.icon);
        superNormalObj.SetActive(cfg.equipQualityConfig.quality > StaticConstants.SuperEquipBaseQuality);
    }
}
