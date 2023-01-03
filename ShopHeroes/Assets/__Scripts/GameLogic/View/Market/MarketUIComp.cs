using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketUIComp : MonoBehaviour
{
    public Button closeBtn;

    [Header("摊位列表")]
    public List<BoothItemComp> boothItems;

    [Header("底部")]

    public Button buyMarketBtn;
    public Button sellMarketBtn;

}
