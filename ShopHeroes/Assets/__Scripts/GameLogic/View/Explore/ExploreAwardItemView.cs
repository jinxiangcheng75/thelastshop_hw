using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExploreAwardItemView : MonoBehaviour
{
    public Image vipImg;
    public Image lockImg;
    public Image huangguanImg;
    public GUIIcon icon;
    public Text numText;
    public Button selfBtn;
    public GameObject limitObj;
    public GameObject changeObj;
    public Text changeMoneyText;
    OneRewardItem data;
    CommonRewardData commonData;
    bool isVip;
    private void Awake()
    {
        selfBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, commonData, selfBtn.transform);
        });
    }

    public void setData(OneRewardItem award, bool isVip, int index)
    {
        data = award;
        commonData = new CommonRewardData(data.itemId, data.count, data.quality, data.itemType);
        limitObj.SetActive(false);
        changeObj.SetActive(false);
        this.isVip = isVip;
        vipImg.enabled = isVip;
        lockImg.enabled = isVip && (K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip;
        huangguanImg.enabled = isVip && (K_Vip_State)UserDataProxy.inst.playerData.vipState == K_Vip_State.Vip;
        if ((ItemType)award.itemType == ItemType.EquipmentDrawing || (ItemType)award.itemType == ItemType.Equip)
        {
            var equip = EquipConfigManager.inst.GetEquipInfoConfig(award.itemId);
            icon.SetSprite(equip.equipDrawingsConfig.atlas, equip.equipDrawingsConfig.icon, StaticConstants.qualityTxtColor[equip.equipQualityConfig.quality - 1]);
        }
        else
        {
            var item = ItemconfigManager.inst.GetConfig(award.itemId);
            icon.SetSprite(item.atlas, item.icon);
        }

        numText.text = award.count.ToString();

        setTweenAnimation(index);
    }

    public void RecoveryData()
    {
        bool isChange = data.count != data.recycledCount1;
        if (isVip && (K_Vip_State)UserDataProxy.inst.playerData.vipState != K_Vip_State.Vip) return;
        if (isChange)
        {
            limitObj.SetActive(true);
            GameTimer.inst.AddTimer(0.5f, 1, () =>
              {
                  if (this == null) return;
                  limitObj.SetActive(false);
                  changeObj.SetActive(true);
                  long moneyCount = 0;
                  DOTween.To(() => moneyCount, a => moneyCount = a, data.recycledCount2, 0.6f).OnUpdate(() =>
                    {
                        if (this == null) return;
                        changeMoneyText.text = "x" + moneyCount;
                    });
                  long itemCount = data.count;
                  DOTween.To(() => itemCount, a => itemCount = a, data.recycledCount1, 0.6f).OnUpdate(() =>
                    {
                        if (this == null) return;
                        numText.text = "x" + itemCount;
                    }).OnComplete(() =>
                    {
                        if (this == null) return;
                        limitObj.SetActive(true);
                        limitObj.GetComponent<Text>().DOFade(1, 1f).From(0).SetLoops(8,LoopType.Yoyo);
                        //DoTweenUtil.Fade_0_To_a_All(limitObj.transform, 1, 0.3f);
                    });
              });
        }
    }

    private void setTweenAnimation(int index)
    {
        GameTimer.inst.AddTimer(0.2f + 0.1f * index, 1, () =>
          {
              if (this == null) return;
              transform.DOScale(new Vector3(1, 1, 1), 0.2f).From(new Vector3(0f, 0f, 0f)).OnStart(() =>
              {
                  if (this == null) return;
                  gameObject.SetActive(true);
              });
          });
    }
}
