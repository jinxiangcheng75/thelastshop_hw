using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleRecruitSubComp : MonoBehaviour
{
    public Button closeBtn;
    public Text titleText;
    public GUIIcon heroBgIcon;
    public Transform headParent;
    public GUIIcon typeBgIcon;
    public GUIIcon typeIcon;
    public GUIIcon intelligenceIcon;
    public Text intelligenceText;
    public List<GUIIcon> allEquipType;
    public Text probText;
    public Text hpText;
    public Text atkText;
    public Text defText;
    public Button recruitBtn;

    [Header("动画")]
    public Animator uiAnimator;

}

public class RoleRecruitSubView : ViewBase<RoleRecruitSubComp>
{
    public override string viewID => ViewPrefabName.RoleRecruitSubPanel;
    public override string sortingLayerName => "popup";

    RoleRecruitData curRecruitData;
    int curIndex;
    GraphicDressUpSystem graphicDressUp;

    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.recruitBtn.ButtonClickTween(() =>
        {
            RecruitSendMessage(curIndex, 10001);
        });
    }

    public void setData(RoleRecruitData heroData, int index)
    {
        curIndex = index;
        curRecruitData = heroData;
        int rarity = RoleDataProxy.inst.ReturnRarityByAptitude(heroData.intelligence);
        contentPane.heroBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleHeroBgIconName[rarity - 1]);
        contentPane.intelligenceIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.roleIntelligenceIconStr[rarity - 1]);
        contentPane.typeBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[heroData.professionCfg.type - 1]);
        contentPane.typeIcon.SetSprite(curRecruitData.professionCfg.atlas, curRecruitData.professionCfg.ocp_icon);
        contentPane.titleText.text = LanguageManager.inst.GetValueByKey(curRecruitData.professionCfg.name);

        string qualityColor = StaticConstants.roleIntelligenceColor[rarity];
        contentPane.intelligenceText.text = curRecruitData.intelligence.ToString();
        contentPane.intelligenceText.color = GUIHelper.GetColorByColorHex(qualityColor);
        contentPane.probText.enabled = (rarity == 3 || rarity == 4);
        if (rarity == 3)
            contentPane.probText.text = LanguageManager.inst.GetValueByKey("招募必出<Color=#c617ff>史诗天赋</Color>");
        else if (rarity == 4)
            contentPane.probText.text = LanguageManager.inst.GetValueByKey("招募必出<Color=#ff911c>传奇天赋</Color>");

        contentPane.hpText.text = curRecruitData.attributeConfig.hp_basic.ToString();
        contentPane.atkText.text = curRecruitData.attributeConfig.atk_basic.ToString();
        contentPane.defText.text = curRecruitData.attributeConfig.def_basic.ToString();

        setHeroHeadIcon();
        setEquipTypeIcon();
    }

    private void setEquipTypeIcon()
    {
        var cfg = HeroProfessionConfigManager.inst.GetConfig(curRecruitData.heroId);
        List<int> ids = new List<int>();
        foreach (var item in cfg.equip1)
        {
            ids.Add(item);
        }
        foreach (var item in cfg.equip2)
        {
            ids.Add(item);
        }

        for (int i = 0; i < contentPane.allEquipType.Count; i++)
        {
            int index = i;
            if (index < ids.Count)
            {
                EquipClassification tempClass = EquipConfigManager.inst.GetEquipTypeByID(ids[index]);
                contentPane.allEquipType[index].gameObject.SetActive(true);
                contentPane.allEquipType[index].SetSprite(tempClass.Atlas, tempClass.icon);
            }
            else
            {
                contentPane.allEquipType[index].gameObject.SetActive(false);
            }
        }
    }

    private void setHeroHeadIcon()
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)curRecruitData.gender), curRecruitData.GetHeadDressIds(), (EGender)curRecruitData.gender, callback: system =>
            {
                graphicDressUp = system;
                system.transform.SetParent(contentPane.headParent);
                system.transform.localScale = Vector3.one;
                system.transform.localPosition = Vector3.zero;
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(graphicDressUp, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)curRecruitData.gender), curRecruitData.GetHeadDressIds(), (EGender)curRecruitData.gender);
        }
    }

    private void RecruitSendMessage(int index, int costType)
    {
        if (RoleDataProxy.inst.enterType == 0)
        {
            if (RoleDataProxy.inst.heroFieldCount > RoleDataProxy.inst.HeroList.Count)
            {
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_BUYHERO, index, costType);
            }
            else
            {
                var cfg = FieldConfigManager.inst.GetFieldConfig(1, RoleDataProxy.inst.heroFieldCount + 1);
                if (cfg == null)
                {
                    EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("无空闲槽位"), GUIHelper.GetColorByColorHex("FFD907"));
                    return;
                }
                RoleDataProxy.inst.slotComType = 0;
                RoleDataProxy.inst.buyHeroIndex = index;
                RoleDataProxy.inst.buyHeroCostType = costType;
                EventController.inst.TriggerEvent(GameEventType.RoleEvent.BUYSLOT_SHOWUI, RoleDataProxy.inst.heroFieldCount);
            }
        }
        else if (RoleDataProxy.inst.enterType == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_BUYHERO, index, costType);
        }
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animTime = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(animTime, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    protected override void onHide()
    {

    }
}
