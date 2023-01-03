using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class RoleTransferPreviewItem : MonoBehaviour, IDynamicScrollViewItem
{
    public GUIIcon typeIcon;
    public Text typeText;
    public Text descText;
    public Button skillBtn;
    public GUIIcon skillIcon;
    HeroProfessionConfigData data;
    HeroSkillShowConfig curSkill;

    public int index = 0;

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    private void Awake()
    {
        skillBtn.ButtonClickTween(() =>
        {
            if (curSkill == null) return;
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.ROLEINTRODUCE_SHOWUI, skillBtn.transform, curSkill);
        });
    }

    public void setData(HeroProfessionConfigData _data)
    {
        data = _data;
        typeIcon.SetSprite(data.atlas, data.ocp_icon);
        typeText.text = LanguageManager.inst.GetValueByKey(data.name);
        descText.text = LanguageManager.inst.GetValueByKey(data.ocp_story);
        var lastCfg = HeroProfessionConfigManager.inst.GetConfig(data.pre_profession);
        if (data.id_skill1 != lastCfg.id_skill1)
        {
            curSkill = HeroSkillShowConfigManager.inst.GetConfig(data.id_skill1);
        }
        else if (data.id_skill2 != 0 && lastCfg.id_skill2 == 0)
        {
            curSkill = HeroSkillShowConfigManager.inst.GetConfig(data.id_skill2);
        }
        else if (data.id_skill3 != 0 && lastCfg.id_skill3 == 0)
        {
            curSkill = HeroSkillShowConfigManager.inst.GetConfig(data.id_skill3);
        }
        else if (data.id_skill2 != lastCfg.id_skill2)
        {
            curSkill = HeroSkillShowConfigManager.inst.GetConfig(data.id_skill2);
        }

        skillIcon.SetSprite(curSkill.skill_atlas, curSkill.skill_icon);
    }
}
