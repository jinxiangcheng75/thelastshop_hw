using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mosframe;

public class ExploreInfoView : ViewBase<ExploreInfoCom>
{
    public override string viewID => ViewPrefabName.ExploreInfoUI;
    public override string sortingLayerName => "popup";
    List<ExploreInstanceLvConfigData> cfgData;
    int curLevel;
    protected override void onInit()
    {
        base.onInit();

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.scrollView.itemRenderer = listitemRenderer;
        //contentPane.scrollView.itemUpdateInfo = listitemRenderer;
        cfgData = new List<ExploreInstanceLvConfigData>();
    }

    public void setData(int groupId)
    {
        cfgData = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroup(groupId);
        ExploreGroup groupData = ExploreDataProxy.inst.GetGroupDataByGroupId(groupId);
        ExploreInstanceConfigData temp = ExploreInstanceConfigManager.inst.GetConfigByGroupId(groupId);
        ExploreInstanceLvConfigData nextCfg = ExploreInstanceLvConfigManager.inst.GetConfigDataByGroupAndLevel(groupId, groupData.groupData.level + 1);

        curLevel = groupData.groupData.level;
        contentPane.icon.SetSprite(StaticConstants.exploreAtlas, temp.instance_icon);
        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(temp.instance_name);
        contentPane.levelText.text = "Lv." + groupData.groupData.level;

        if (nextCfg == null)
        {
            contentPane.nextObj.SetActive(false);
            //Color curColor = GUIHelper.GetColorByColorHex("#FFD700");
            //contentPane.levelText.color = curColor;
            contentPane.levelSlider.maxValue = 1;
            contentPane.levelSlider.value = 1;
            //contentPane.scheduleText.color = curColor;
            contentPane.scheduleText.text = "max";
        }
        else
        {
            contentPane.nextObj.SetActive(true);
            if (nextCfg.effect_type == 1 || nextCfg.effect_type == 6)
                contentPane.nextUpObj.SetActiveTrue();
            else
                contentPane.nextUpObj.SetActiveFalse();

            if (nextCfg.effect_type == 5)
            {
                var exploreCfg = ExploreInstanceConfigManager.inst.GetConfig(nextCfg.effect_id[0]);
                if (temp.instance_group != exploreCfg.instance_group)
                {
                    contentPane.itemIcon.SetSprite(nextCfg.effect_atlas, nextCfg.effect_icon, needSetNativeSize: true);
                }
                else
                {
                    contentPane.itemIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                    contentPane.itemIcon.SetSprite(nextCfg.effect_atlas, nextCfg.effect_icon);
                }
            }
            else
            {
                contentPane.itemIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                contentPane.itemIcon.SetSprite(nextCfg.effect_atlas, nextCfg.effect_icon);
            }

            //contentPane.itemIcon.SetSprite(nextCfg.effect_atlas, nextCfg.effect_icon);
            //contentPane.levelText.color = Color.white;
            //contentPane.scheduleText.color = Color.white;
            contentPane.levelSlider.maxValue = nextCfg.need_instance_exp;
            contentPane.levelSlider.value = Mathf.Max(nextCfg.need_instance_exp * 0.05f, groupData.groupData.exp);
            contentPane.scheduleText.text = groupData.groupData.exp + "/" + nextCfg.need_instance_exp;
        }
        SetListItemTotalCount(cfgData.Count);
        //contentPane.scrollView.updateListItemInfo();
        contentPane.scrollView.scrollByItemIndex(curLevel);
    }

    int listItemCount = 0;
    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        ExploreInfoItemView item = (ExploreInfoItemView)obj;

        if (index >= listItemCount)
        {
            item.gameObject.SetActive(false);
        }

        if (index < listItemCount)
        {
            item.gameObject.SetActive(true);
            item.setData(cfgData[index], curLevel);
        }
        else
        {
            item.gameObject.SetActive(false);
        }
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

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.windowAnim.CrossFade("show", 0f);
        contentPane.windowAnim.Update(0f);
        contentPane.windowAnim.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.windowAnim.Play("hide");
        float animTime = contentPane.windowAnim.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(animTime, 1, () =>
          {
              contentPane.windowAnim.CrossFade("null", 0f);
              contentPane.windowAnim.Update(0f);
              HideView();
          });
    }
}
