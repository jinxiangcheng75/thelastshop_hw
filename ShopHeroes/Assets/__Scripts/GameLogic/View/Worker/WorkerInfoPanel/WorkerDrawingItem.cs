using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerDrawingItem : MonoBehaviour
{
    public GUIIcon icon;
    public GUIIcon subTypeIcon;
    public Text lvText;
    public Text nameText;
    public ContentSizeFitter sizeFitter;


    public Action onClickHandle;


    private void Start()
    {
        var selfBtn = GetComponent<Button>();
        selfBtn.ButtonClickTween(() => 
        {
            onClickHandle?.Invoke();
        });
    }

    public void setData(EquipDrawingsConfig equipDrawCfg)
    {
        icon.SetSprite(equipDrawCfg.atlas, equipDrawCfg.icon);

        EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(equipDrawCfg.sub_type);
        subTypeIcon.SetSprite(classcfg.Atlas, classcfg.icon);

        lvText.text = equipDrawCfg.level.ToString();
        nameText.text = LanguageManager.inst.GetValueByKey(equipDrawCfg.name);

        if (nameText.preferredWidth <= 150f)
        {
            nameText.resizeTextForBestFit = false;
            sizeFitter.enabled = true;
        }
        else
        {
            sizeFitter.enabled = false;
            nameText.resizeTextForBestFit = true;
        }
    }
}
