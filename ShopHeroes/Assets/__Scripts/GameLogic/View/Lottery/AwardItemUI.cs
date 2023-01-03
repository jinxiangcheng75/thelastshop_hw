using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class AwardItemUI : MonoBehaviour, IPointerClickHandler
{
    public GUIIcon bgIcon;
    public GUIIcon icon;
    public Image selectObj;
    public Text numText;
    public Text qualityText;
    public GameObject highObj;
    public GameObject grayObj;
    public GUIIcon stateIcon;
    public GameObject gouObj;
    private LotteryData data;

    CommonRewardData commonData;

    public void InitData(LotteryData data)
    {
        this.data = data;
        commonData = new CommonRewardData(data.itemId, data.itemNum, data.rarity, data.type);
        if (data.itemConfig == null) return;
        gameObject.SetActive(true);
        highObj.SetActive(false);
        icon.SetSprite(data.itemConfig.atlas, data.itemConfig.icon);
        numText.text = "x" + AbbreviationUtility.AbbreviateNumber(data.itemNum, 2);
        bgIcon.gameObject.SetActive(data.rarity != 1);
        if (data.rarity == 2)
        {
            qualityText.text = LanguageManager.inst.GetValueByKey("稀有");
            bgIcon.SetSprite("lottery_atlas", "zhuanpan_xiyou");
        }
        else if (data.rarity == 3)
        {
            qualityText.text = LanguageManager.inst.GetValueByKey("超稀有");
            bgIcon.SetSprite("lottery_atlas", "zhuanpan_chuanqi");
        }
        if ((JackpotDataState)data.prizeState == JackpotDataState.BeenGet)
        {
            icon.GetComponent<Image>().color = Color.gray;
            gouObj.SetActive(true);
            grayObj.SetActive(true);
            stateIcon.SetSprite("lottery_atlas", "zhuanpan_wupindihui");
        }
        else
        {
            icon.GetComponent<Image>().color = Color.white;
            gouObj.SetActive(false);
            grayObj.SetActive(false);
            stateIcon.SetSprite("lottery_atlas", "zhuanpan_wupindi");
        }
    }

    public void SetHighLight(System.Action timeComplete = null)
    {
        highObj.SetActive(true);
        stateIcon.SetSprite("lottery_atlas", "zhuanpan_wupindiliang");

        GameTimer.inst.AddTimer(0.5f, 1, () =>
          {
              timeComplete?.Invoke();
          });
    }

    public void SetGray()
    {

    }

    public void ClearData()
    {
        data = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, commonData, icon.transform);
    }
}
