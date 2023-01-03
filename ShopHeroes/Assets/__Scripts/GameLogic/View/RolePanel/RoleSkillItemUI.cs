using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleSkillItemUI : MonoBehaviour
{
    public GUIIcon skillIcon;
    public GameObject nonactivatedObj;

    private HeroSkillShowConfig skillInfo;

    public void setData(HeroSkillShowConfig skillData)
    {
        skillInfo = skillData;
        //levelText.text = "Lv" + skillData.level;
        skillIcon.gameObject.SetActive(true);
        skillIcon.SetSprite(skillInfo.skill_atlas, skillInfo.skill_icon);
    }

    public void clearData()
    {
        skillInfo = null;
        skillIcon.gameObject.SetActive(false);
        //skillBtn.onClick.RemoveAllListeners();
    }
}
