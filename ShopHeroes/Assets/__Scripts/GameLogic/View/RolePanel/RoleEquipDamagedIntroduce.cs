using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleEquipDamagedIntroduce : MonoBehaviour
{
    public GUIIcon typeIcon;
    public Text nameText;
    public List<RoleEquipItem> allEquip;
    public Transform headParent;
    public Text lastFightingText;
    public Text curFightingText;
    public Button bgBtn;
    public RectTransform bgRect;
    public RectTransform moveObjTrans;
    GraphicDressUpSystem graphicDressUp;

    private void Awake()
    {
        bgBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void setData(Transform pos, int equipId)
    {
        var propertyList = EquipConfigManager.inst.GetEquipAllPropertyList(equipId);
        for (int i = 0; i < allEquip.Count; i++)
        {
            int index = i;
            if (propertyList[index] > 0)
            {
                allEquip[index].gameObject.SetActive(true);
                allEquip[index].valText.text = propertyList[index].ToString();
            }
            else
            {
                allEquip[index].gameObject.SetActive(false);
            }
        }
        var cfg = EquipConfigManager.inst.GetEquipInfoConfig(equipId);
        var classCfg = EquipConfigManager.inst.GetEquipTypeByID(cfg.equipDrawingsConfig.sub_type);
        typeIcon.SetSprite(classCfg.Atlas, classCfg.icon);
        nameText.text = cfg.name;
        typeIcon.iconImage.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[cfg.equipQualityConfig.quality - 1]);
        nameText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[cfg.equipQualityConfig.quality - 1]);
        setIntroducePos(pos);
    }

    private void setIntroducePos(Transform trans)
    {
        Vector3 screenPoint = FGUI.inst.uiCamera.WorldToScreenPoint(trans.position);
        Vector2 anchorePos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), screenPoint, FGUI.inst.uiCamera, out anchorePos);
        bgRect.localScale = new Vector3(-1, -1, 1);
        anchorePos += new Vector2(136, 188);
        float screenWidth = FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.width / 2;
        float bgWidth = bgRect.rect.width / 2;

        if (anchorePos.x < bgWidth - screenWidth)
        {
            bgRect.localScale = new Vector3(-1, -1, 1);
            anchorePos = new Vector2(bgWidth - screenWidth, anchorePos.y);
        }
        else if (anchorePos.x > screenWidth - bgWidth)
        {
            bgRect.localScale = new Vector3(1, -1, 1);
            anchorePos -= new Vector2(76 + bgWidth, 0);
        }

        moveObjTrans.anchoredPosition = anchorePos;
    }

    public void setInfoData(HeroInfo data)
    {
        var propertyList = EquipConfigManager.inst.GetEquipAllPropertyList(data.brokenEquip.equipId);
        for (int i = 0; i < allEquip.Count; i++)
        {
            int index = i;
            if (propertyList[index] > 0)
            {
                allEquip[index].gameObject.SetActive(true);
                allEquip[index].valText.text = propertyList[index].ToString();
            }
            else
            {
                allEquip[index].gameObject.SetActive(false);
            }
        }
        var cfg = EquipConfigManager.inst.GetEquipInfoConfig(data.brokenEquip.equipId);
        var classCfg = EquipConfigManager.inst.GetEquipTypeByID(cfg.equipDrawingsConfig.sub_type);
        typeIcon.SetSprite(classCfg.Atlas, classCfg.icon);
        nameText.text = cfg.name;
        typeIcon.iconImage.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[cfg.equipQualityConfig.quality - 1]);
        nameText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[cfg.equipQualityConfig.quality - 1]);
        var lastHeroData = ExploreDataProxy.inst.GetLastHeroDataByUid(data.heroUid);
        lastFightingText.text = lastHeroData.fightingNum.ToString();
        RoleHeroData tempData = new RoleHeroData();
        tempData.setData(data);
        curFightingText.text = tempData.fightingNum.ToString();
        setHeroHeadIcon(tempData);
    }

    private void setHeroHeadIcon(RoleHeroData data)
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender, callback: system =>
             {
                 graphicDressUp = system;
                 system.transform.SetParent(headParent);
                 system.transform.localScale = Vector3.one * 0.5f;
                 system.transform.localPosition = new Vector3(0, 310, 0);
             });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(graphicDressUp, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), data.GetHeadDressIds(), (EGender)data.gender);
        }
    }
}
