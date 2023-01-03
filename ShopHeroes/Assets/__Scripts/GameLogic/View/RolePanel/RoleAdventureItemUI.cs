using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleAdventureItemUI : MonoBehaviour
{
    public Image icon;
    public Slider expSlider;
    public GUIIcon bgIcon;

    private RoleHeroData data;

    //头像
    public Transform headParent;
    GraphicDressUpSystem graphicDressUp;

    public void setData(RoleHeroData roleData, heroupgradeconfig upgradeCfg)
    {
        data = roleData;
        setHeroHeadIcon();
        //icon.sprite = RoleDataProxy.inst.GetSprite(StaticConstants.rolePhotoOffset + roleData.uid);
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(roleData.intelligence);
        bgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        if (upgradeCfg == null)
        {
            expSlider.maxValue = 1;
            expSlider.value = 1;
        }
        else
        {
            expSlider.maxValue = upgradeCfg.getExp(RoleDataProxy.inst.ReturnRarityByAptitude(roleData.intelligence));
            expSlider.value = roleData.exp;
            var cityData = UserDataProxy.inst.GetBuildingData(StaticConstants.heroLvLimitHouseID);
            if (cityData != null)
            {
                if (data.level >= cityData.effectVal)
                {
                    expSlider.maxValue = 1;
                    expSlider.value = 1;
                }
            }
        }
    }

    private void setHeroHeadIcon()
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender, callback: system =>
            {
                graphicDressUp = system;
                system.transform.SetParent(headParent);
                system.transform.localScale = Vector3.one;
                system.transform.localPosition = Vector3.zero;
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(graphicDressUp, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender);
        }
    }
}
