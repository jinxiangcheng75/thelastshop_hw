using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class InvestRankItem : MonoBehaviour, IDynamicScrollViewItem
{
    public GUIIcon rankBg;
    public GUIIcon rankIcon;
    public Text rankTx;
    public Text nameTx;
    public Text investNumTx;
    public Text costNumTx;

    int _index;
    public void onUpdateItem(int index)
    {
        _index = index;
    }

    public void SetData(BuildTopList rankData, int cost_grade)
    {
        if (_index < 3)
        {
            rankIcon.gameObject.SetActive(true);
            rankIcon.SetSprite("cityBuilding_atlas", "jiaju_paiming" + (_index + 1));
        }
        else
        {
            rankIcon.gameObject.SetActive(false);
        }

        rankTx.text = _index + 1 + "";

        if (_index % 2 == 0)
        {
            rankBg.iconImage.enabled = true;
        }
        else
        {
            rankBg.iconImage.enabled = false;
        }

        nameTx.text = LanguageManager.inst.GetValueByKey(rankData.name);
        investNumTx.text = rankData.investNum.ToString() + " " + LanguageManager.inst.GetValueByKey("投资");

        ulong goldCost, gemCost;
        goldCost = BuildingCostConfigManager.inst.GetInvestCost(0, rankData.investNum, cost_grade, out gemCost);
        costNumTx.text = goldCost.ToString("N0");
    }

}
