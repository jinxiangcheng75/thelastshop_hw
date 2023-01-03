using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroWearEquipView : ViewBase<HeroWearEquipComp>
{
    public override string viewID => ViewPrefabName.HeroWearEquipUI;
    public override string sortingLayerName => "window";

    int maxLevel = 0;
    protected override void onInit()
    {
        base.onInit();

        contentPane.closeInventoryBtn.onClick.AddListener(() =>
        {
            hide();
        });

        contentPane.suggestItemList.itemRenderer = this.listitemRenderer;
        //contentPane.suggestItemList.itemUpdateInfo = this.listitemRenderer;
        contentPane.suggestItemList.activeFalse = activeFalseRenderer;
        contentPane.suggestItemList.scrollByItemIndex(0);
        //contentPane.suggestItemList.totalItemCount = 0;
    }

    private int[] _typeidArray;
    private int _heroId;
    private int _equipFieldId;
    private int _onOrOff;

    protected override void onShown()
    {
        base.onShown();
        //AudioManager.inst.PlaySound(10);
    }

    public void GetItemLists(int[] typeidArray, int heroUid, int equipField, int onOrOff)
    {
        _typeidArray = typeidArray;
        _heroId = heroUid;
        _equipFieldId = equipField;
        _onOrOff = onOrOff;

        RoleHeroData curHero = RoleDataProxy.inst.GetHeroDataByUid(heroUid);
        maxLevel = heroupgradeconfigManager.inst.GetHeroUpgradeConfig(curHero.level).equip_lv;
        currOnShelfEquips = ItemBagProxy.inst.GetEquipItemsByHero(typeidArray, maxLevel);
        currOnShelfEquips.Sort((a, b) =>
        {
            return a.equipConfig.equipQualityConfig.GetEquipFightingSum(curHero.talentConfig) > b.equipConfig.equipQualityConfig.GetEquipFightingSum(curHero.talentConfig) ? -1 : 1;
        });
        if (currOnShelfEquips.Count > 0)
        {
            SetListItemTotalCount(currOnShelfEquips.Count);
        }
        else
        {
            listItemCount = 1;
            contentPane.suggestItemList.totalItemCount = 1;
        }

        contentPane.suggestItemList.refresh();
        contentPane.tx_canWearHighestEquipLv.text = maxLevel.ToString();

    }

    int listItemCount = 0;
    private List<EquipItem> currOnShelfEquips;

    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;

        for (int i = 0; i < 3; ++i)
        {
            int itemIndex = index * 3 + i;
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.name = "item";
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < currOnShelfEquips.Count + 1)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                HeroEquipItem item = itemScript.buttonList[i].GetComponent<HeroEquipItem>();
                if (itemIndex == 0) item.setMarketDara(_typeidArray, maxLevel, _equipFieldId, _heroId);
                else item.setData(currOnShelfEquips[itemIndex - 1], OnInventoryItemClick);
            }
            else
            {
                itemScript.buttonList[i].gameObject.name = "item";
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void activeFalseRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList item = (BtnList)obj;
        for (int i = 0; i < 3; ++i)
        {
            item.buttonList[i].gameObject.name = "item";
        }
    }

    private void OnInventoryItemClick(string equipUid)
    {
        // 判断当前装备阶级和英雄装备最大阶级 判断是否穿戴
        //hide();
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_WEAREQUIP, _heroId, _equipFieldId, _onOrOff, equipUid);
    }

    void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        if (listItemCount > currOnShelfEquips.Count)
        {
            listItemCount = currOnShelfEquips.Count;
        }
        listItemCount = count + 1;
        int count1 = listItemCount / 3;
        if (listItemCount % 3 > 0)
        {
            count1++;
        }
        contentPane.suggestItemList.totalItemCount = count1;
    }


    protected override void onHide()
    {
        base.onHide();
        //AudioManager.inst.PlaySound(11);
        currOnShelfEquips = null;
        MarketDataProxy.inst.Payload = string.Empty;
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
        float animTime = contentPane.uiAnimator.GetClipLength("commonBagUI_hide");
        GameTimer.inst.AddTimer(animTime * 0.85f, 1, () =>
          {
              this.HideView();
          });
    }
}
