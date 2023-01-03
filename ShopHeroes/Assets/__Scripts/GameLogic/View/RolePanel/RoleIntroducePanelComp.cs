using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleIntroducePanelComp : MonoBehaviour
{
    public Button closeBtn;
    public Text nameText;
    public Text desText;
    public Transform introduceBg;
    public RectTransform bgRect;

    public Transform talentObj;
    public RectTransform talentRect;
    public Text talentNameText;
    public Text talentEntryText;
    public ContentSizeFitter talentEntryFitter;
    public Text talentDesText;
    public ContentSizeFitter talentDesFitter;
    public GUIIcon talentIcon;
}

public class RoleIntroducePanelView : ViewBase<RoleIntroducePanelComp>
{
    public override string viewID => ViewPrefabName.RoleIntroducePanel;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.onClick.AddListener(hide);
        contentPane.introduceBg.gameObject.SetActive(false);
    }

    public void SetTextAndVectorPos(Transform pos, HeroSkillShowConfig skillCfg)
    {
        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(skillCfg.skill_name);
        contentPane.desText.text = LanguageManager.inst.GetValueByKey(skillCfg.skill_dec);

        contentPane.bgRect.sizeDelta = new Vector2(536, contentPane.nameText.rectTransform.sizeDelta.y + contentPane.desText.preferredHeight + 40/*那个小尖尖*/);

        SetVecrtorPos(pos);
        contentPane.introduceBg.gameObject.SetActive(true);
    }

    public void SetTalentData(Transform pos, HeroTalentDataBase talentCfg)
    {
        contentPane.talentNameText.text = LanguageManager.inst.GetValueByKey(talentCfg.name);
        contentPane.talentEntryText.text = HeroTalentDBConfigManager.inst.GetStrByTalentId(talentCfg.id);
        contentPane.talentNameText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[talentCfg.quality - 1]);
        contentPane.talentEntryText.color = GUIHelper.GetColorByColorHex(StaticConstants.qualityTxtColor[talentCfg.quality - 1]);
        var cfg = HeroSkillShowConfigManager.inst.GetConfig(talentCfg.skill_id);
        contentPane.talentIcon.SetSprite(cfg.skill_atlas, cfg.skill_icon);
        var talentSkillCfg = HeroSkillShowConfigManager.inst.GetConfig(talentCfg.skill_id);
        contentPane.talentDesText.text = LanguageManager.inst.GetValueByKey(talentSkillCfg.skill_dec);
        contentPane.talentRect.sizeDelta = new Vector2(536, contentPane.nameText.rectTransform.sizeDelta.y + contentPane.talentEntryText.preferredHeight + contentPane.talentDesText.preferredHeight + 60);

        Vector3 v3 = Vector3.zero;
        Vector2 v2;
        v3 = FGUI.inst.uiCamera.WorldToScreenPoint(pos.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), v3, FGUI.inst.uiCamera, out v2);
        v3 = new Vector3(v2.x, v2.y, 0);
        v3 += new Vector3(288, pos.GetComponent<RectTransform>().sizeDelta.y / 2 + contentPane.talentRect.sizeDelta.y / 2, 0);
        float screenWidth = FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.width / 2;
        float bgWidth = contentPane.talentRect.rect.width / 2;
        contentPane.talentObj.GetComponent<RectTransform>().anchoredPosition = v3;

        contentPane.talentObj.gameObject.SetActive(true);
    }

    private void SetVecrtorPos(Transform pos)
    {
        Vector3 v3 = Vector3.zero;
        Vector2 v2;
        contentPane.bgRect.localScale = new Vector3(1, 1, 1);
        v3 = FGUI.inst.uiCamera.WorldToScreenPoint(pos.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), v3, FGUI.inst.uiCamera, out v2);
        v2 += new Vector2(288, pos.GetComponent<RectTransform>().sizeDelta.y / 2 + contentPane.bgRect.sizeDelta.y / 2 - 226);
        float screenWidth = FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.width / 2;
        float bgWidth = contentPane.bgRect.rect.width / 2;
        if (v2.x < bgWidth - screenWidth)
        {
            v2 = new Vector2(bgWidth - screenWidth, v2.y);
        }
        else if (v2.x > screenWidth - bgWidth)
        {
            contentPane.bgRect.localScale = new Vector3(-1, 1, 1);
            v2 = new Vector2(v2.x - 425, v2.y);
        }

        contentPane.introduceBg.GetComponent<RectTransform>().anchoredPosition = v2;
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {
        contentPane.introduceBg.gameObject.SetActive(false);
        contentPane.talentObj.gameObject.SetActive(false);
    }
}
