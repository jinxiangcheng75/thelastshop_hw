using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleRestingItemUI : MonoBehaviour
{
    public Image headIcon;
    public GUIIcon typeBgIcon;
    public GUIIcon typeIcon;
    public GUIIcon intelligenceBgIcon;
    public Text levelText;
    public Text timeText;

    private RoleHeroData data;
    int timerId;

    //头像
    public Transform headParent;
    GraphicDressUpSystem graphicDressUp;

    public void setData(RoleHeroData _data)
    {
        data = _data;
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(data.intelligence);
        levelText.text = data.level.ToString();
        //headIcon.sprite = RoleDataProxy.inst.GetSprite(StaticConstants.rolePhotoOffset + data.uid);
        setHeroHeadIcon();
        typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[data.config.type - 1]);
        typeIcon.SetSprite(data.config.atlas, data.config.ocp_icon);
        intelligenceBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        if (timerId == 0)
        {
            if (data.remainTime > 0)
                timeText.text = TimeUtils.timeSpanStrip(data.remainTime);
            else
                timeText.text = "1" + LanguageManager.inst.GetValueByKey("秒");
            timerId = GameTimer.inst.AddTimer(1, () =>
             {
                 if (data.remainTime <= 0)
                 {
                     timeText.text = "1" + LanguageManager.inst.GetValueByKey("秒");
                     data = RoleDataProxy.inst.GetHeroDataByUid(data.uid);
                     if (data.currentState == 0)
                     {
                         EventController.inst.TriggerEvent(GameEventType.RoleEvent.ALLROLERESTING_SHOWUI);
                         GameTimer.inst.RemoveTimer(timerId);
                         timerId = 0;
                     }
                 }
                 else
                 {
                     timeText.text = TimeUtils.timeSpanStrip(data.remainTime);
                 }
             });
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

    public void clearData()
    {
        if (timerId > 0)
        {
            GameTimer.inst.RemoveTimer(timerId);
            timerId = 0;
        }
    }
}
