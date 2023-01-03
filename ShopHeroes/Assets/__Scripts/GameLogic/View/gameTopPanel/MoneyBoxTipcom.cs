using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoneyBoxTipcom : MonoBehaviour
{
    public Text goldText;
    public Slider progressSlider;
    public Text progressText;
    public Button okBtn;
    int timerid = 0;
    public void setinfo()
    {
        var data = MoneyBoxDataProxy.inst.moneyBoxData;
        int cgold = data.hasBeenStored;
        int tgold = data.targetGoldCount;
        float progress = (float)cgold / (float)tgold;
        okBtn.gameObject.SetActive(progress >= 1);
        progressSlider.value = progress;
        progressText.text = Mathf.FloorToInt(progress * 100).ToString() + "%";
        goldText.text = string.Format("{0}/{1}", cgold.ToString("N0"), tgold.ToString("N0"));
        okBtn.onClick.AddListener(() =>
        {
            //EventController.inst.TriggerEvent(GameEventType.MoneyBoxEvent.MONEYBOX_SHOWUI);
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_WelfareUI", 4);
            this.gameObject.SetActive(false);
        });
        GameTimer.inst.RemoveTimer(timerid);
        timerid = GameTimer.inst.AddTimer(4f, 1, () =>
       {
           this.gameObject.SetActive(false);
       });
    }

    void OnEnable()
    {
        setinfo();
    }

    void OnDisable()
    {
        GameTimer.inst.RemoveTimer(timerid);
    }
}
