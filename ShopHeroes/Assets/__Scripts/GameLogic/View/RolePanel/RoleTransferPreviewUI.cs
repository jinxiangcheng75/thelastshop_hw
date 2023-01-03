using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleTransferPreviewUI : ViewBase<RoleTransferPreviewComp>
{
    public override string viewID => ViewPrefabName.RoleTransferPreview;
    public override string sortingLayerName => "popup";

    int listItemCount = 0;
    List<HeroProfessionConfigData> canTransferList;
    protected override void onInit()
    {
        base.onInit();

        topResPanelType = TopPlayerShowType.noSettingAndEnergy;
        isShowResPanel = true;

        contentPane.closeBtn.ButtonClickTween(hide);

        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;
        contentPane.scrollView.scrollByItemIndex(0);
        canTransferList = new List<HeroProfessionConfigData>();
    }

    private void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }

        contentPane.scrollView.totalItemCount = count;
    }

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        RoleTransferPreviewItem item = (RoleTransferPreviewItem)obj;
        if (index >= listItemCount)
        {
            item.gameObject.SetActive(false);
        }

        if (index < listItemCount)
        {
            item.gameObject.SetActive(true);
            item.setData(canTransferList[index]);
        }
        else
        {
            item.gameObject.SetActive(false);
        }
    }

    public void setData(int heroId)
    {
        var cfg = HeroProfessionConfigManager.inst.GetConfig(heroId);
        canTransferList = HeroProfessionConfigManager.inst.GetTransferData(heroId, (int)WorldParConfigManager.inst.GetConfig(272).parameters);

        contentPane.typeIcon.SetSprite(cfg.atlas, cfg.ocp_icon);
        contentPane.typeText.text = LanguageManager.inst.GetValueByKey(cfg.name);
        contentPane.descText.text = LanguageManager.inst.GetValueByKey(cfg.ocp_story);

        var skillCfg = HeroSkillShowConfigManager.inst.GetConfig(cfg.id_skill1);
        contentPane.skillIcon.SetSprite(skillCfg.skill_atlas, skillCfg.skill_icon);

        if (canTransferList.Count <= 0)
        {
            contentPane.isMaxObj.SetActive(true);
            return;
        }
        contentPane.isMaxObj.SetActive(false);
        SetListItemTotalCount(canTransferList.Count);
    }

    protected override void onShown()
    {
        AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        AudioManager.inst.PlaySound(11);
    }
}
