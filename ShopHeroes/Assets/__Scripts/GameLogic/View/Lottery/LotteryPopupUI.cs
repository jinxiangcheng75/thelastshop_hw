using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LotteryPopupUI : MonoBehaviour
{
    public Action clickHandler;
    public Text titleText;
    public Button close_btn;
    public Transform allIcons;
    private string[] titles = new string[] { "累计奖励", "奖池奖励", "恭喜你抽到了", "获得累计奖励" };
    // Start is called before the first frame update
    void Start()
    {
        close_btn.onClick.AddListener(() => { clickHandler?.Invoke(); });
    }

    public void InitData(kLotteryPopupType type, List<ShowPopupData> config)
    {
        bool isLLL = type == kLotteryPopupType.Cumulative;
        gameObject.SetActive(true);
        titleText.text = titles[(int)type];

        for (int i = 0; i < allIcons.childCount; i++)
        {
            int index = i;
            if (i < config.Count)
            {
                allIcons.GetChild(index).gameObject.SetActive(true);
                allIcons.GetChild(index).GetComponent<PopupAwardItemUI>().setData(config[index]);
            }
            else
                allIcons.GetChild(index).gameObject.SetActive(false);
        }
    }
}
