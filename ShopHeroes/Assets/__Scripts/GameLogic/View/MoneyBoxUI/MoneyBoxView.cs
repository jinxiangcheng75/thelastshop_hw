using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoneyBoxView : ViewBase<MoneyBoxUIComp>
{
    public override string viewID => ViewPrefabName.MoneyBoxUI;
    public override string sortingLayerName => "window";
    int updateTimeid = 0;
    protected override void onInit()
    {
        base.onInit();
        isShowResPanel = true;
        contentPane.closeBtn.onClick.AddListener(() => hide());
        contentPane.okBtn.onClick.AddListener(GetRewards);
    }

    protected override void onShown()
    {
        base.onShown();

        UpdateUI();
    }

    protected override void onHide()
    {
        base.onHide();

        if (coolTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(coolTimerId);
        }
    }

    //领取奖励
    void GetRewards()
    {
        contentPane.effectObj.SetActive(false);
        contentPane.windowTF.gameObject.SetActive(false);
        contentPane.rewardIcon.gameObject.SetActive(true);

        contentPane.rewardAnim.CrossFade("open", 0f);
        contentPane.rewardAnim.Update(0f);
        contentPane.rewardAnim.Play("open");
        AudioManager.inst.PlaySound(104);
        float animTime = contentPane.rewardAnim.GetClipLength("openMoneyBox");
        GameTimer.inst.AddTimer(animTime * 0.95f, 1, () =>
          {
              contentPane.rewardIcon.gameObject.SetActive(false);
              EventController.inst.TriggerEvent(GameEventType.MoneyBoxEvent.MONEYBOX_REQUEST_REWARDS);
          });
    }
    public void UpdateUI()
    {
        MoneyBoxData data = MoneyBoxDataProxy.inst.moneyBoxData;
        int index = 0;
        contentPane.windowTF.gameObject.SetActive(true);
        contentPane.rewardIcon.gameObject.SetActive(false);
        contentPane.slider.value = data.currOrderIndex - 1 + (float)data.hasBeenStored / data.targetGoldCount;
        if (contentPane.slider.value <= 0.05f)
        {
            contentPane.slider.value = 0.05f;
        }
        contentPane.stateTips.ForEach(tip =>
        {
            index += 1;
            if (index == data.currOrderIndex)
            {
                tip.State = data.currState;
            }
            else
            {
                tip.State = index < data.currOrderIndex ? 3 : 4;
            }
        });

        if (data.currOrderIndex >= 7)
        {
            contentPane.bgPigIcon.SetSprite("moneybox_atlas", "xiaozhu_chuxuguanjin1");
            contentPane.maskPigIcon.SetSprite("moneybox_atlas", "xiaozhu_chuxuguanjin1");
            contentPane.grayPigIcon.SetSprite("moneybox_atlas", "xiaozhu_chuxuguanjin1");
            contentPane.rewardIcon.SetSprite("moneybox_atlas", "xiaozhu_chuxuguanjin1");
        }
        else
        {
            contentPane.bgPigIcon.SetSprite("moneybox_atlas", "xiaozhu_chuxuguan1");
            contentPane.maskPigIcon.SetSprite("moneybox_atlas", "xiaozhu_chuxuguan1");
            contentPane.grayPigIcon.SetSprite("moneybox_atlas", "xiaozhu_chuxuguan1");
            contentPane.rewardIcon.SetSprite("moneybox_atlas", "xiaozhu_chuxuguan1");
        }

        contentPane.effectObj.SetActive(data.currState == 2);
        contentPane.okBtn.gameObject.SetActive(data.currState == 2);
        contentPane.inProgressNode.gameObject.SetActive(data.currState == 1);
        contentPane.nextStateTimeNode.gameObject.SetActive(data.currState == 0);
        contentPane.goldText.gameObject.SetActive(data.currState == 1);
        contentPane.progressText.gameObject.SetActive(data.currState == 1);
        if (coolTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(coolTimerId);
            coolTimerId = 0;
        }
        contentPane.progressMask.fillAmount = 1f - (float)data.hasBeenStored / (float)data.targetGoldCount;
        if (data.currState == 1)
        {
            contentPane.goldText.text = string.Format("{0}/{1}", data.hasBeenStored.ToString("N0"), data.targetGoldCount.ToString("N0"));
            if (data.targetGoldCount == 0)
            {
                contentPane.progressText.text = "0%";
            }
            else
            {
                contentPane.progressText.text = (data.hasBeenStored * 100 / data.targetGoldCount).ToString() + "%";
            }
        }
        else if (data.currState == 0)
        {
            //contentPane.coolTimeText.text = TimeUtils.timeSpanStrip(data.stateCoolTime);
            coolTimerId = GameTimer.inst.AddTimer(1, updateCoolTime);
            updateCoolTime();
        }
        if (updateTimeid > 0)
        {

            GameTimer.inst.RemoveTimer(updateTimeid);
        }
    }
    int coolTimerId = 0;
    void updateCoolTime()
    {
        if (MoneyBoxDataProxy.inst.moneyBoxData.stateCoolTime <= 0)
        {
            contentPane.coolTimeText.text = TimeUtils.timeSpanStrip(0);
            EventController.inst.TriggerEvent(GameEventType.MoneyBoxEvent.MONEYBOX_REQUEST_DATA);
        }
        else
        {
            contentPane.coolTimeText.text = TimeUtils.timeSpanStrip(MoneyBoxDataProxy.inst.moneyBoxData.stateCoolTime);
        }
    }
}
