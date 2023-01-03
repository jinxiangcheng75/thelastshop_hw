using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TreasureBoxCompleteView : ViewBase<TreasureBoxCompleteCom>
{
    public override string viewID => ViewPrefabName.TreasureBoxComplete;
    public override int showType => (int)ViewShowType.normal;
    public override string sortingLayerName => "window";
    int boxGroupId;
    List<OneRewardItem> items;
    int index = 0;
    List<int> equipDrawingIdList = new List<int>();
    protected override void onInit()
    {
        base.onInit();

        items = new List<OneRewardItem>();
        contentPane.collectBtn.onClick.AddListener(() =>
        {
            hide();
            if (equipDrawingIdList.Count > 0)
            {
                for (int i = 0; i < equipDrawingIdList.Count; i++)
                {
                    int index = i;
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.ActivateDrawing, "", equipDrawingIdList[index], 0, 1));
                }

                equipDrawingIdList.Clear();
            }
        });
    }

    protected override void onShown()
    {

    }

    public void setData(int boxId, List<OneRewardItem> items)
    {
        boxGroupId = boxId;
        this.items = items;
        var boxData = TreasureBoxDataProxy.inst.GetDataByBoxId(boxId);
        var boxCfg = ItemconfigManager.inst.GetConfig(boxData.boxItemId);
        contentPane.boxIcon.SetSpriteURL(boxCfg.icon);
        index = 0;
        contentPane.completeObj.SetActive(false);
        contentPane.popupObj.gameObject.SetActive(false);

        equipDrawingIdList.Clear();

        TBoxManager.inst.playOpenAnim(() =>
        {
            if (items.Count > 0)
            {
                setSinglePopupAwardAnim(items[index].itemId, items[index].quality, items[index].itemType);
            }
        }, () =>
         {
             if (FGUI.inst.isLandscape)
             {
                 Camera.main.transform.DOMoveY(13.41f, 0.3f);
                 Camera.main.DOOrthoSize(3f, 0.3f);
             }
             else
             {
                 Camera.main.transform.DOMoveY(18.65f, 0.3f);
                 Camera.main.DOOrthoSize(8.3f, 0.3f);
             }
         });

        contentPane.effectObj.sortingLayerName = _uiCanvas.sortingLayerName;
        contentPane.effectObj.sortingOrder = _uiCanvas.sortingOrder - 1;
    }

    private void setSinglePopupAwardAnim(int itemId, int rarity, int itemType)
    {
        AudioManager.inst.PlaySound(100);
        contentPane.popupObj.gameObject.SetActive(false);
        contentPane.popupRect.anchoredPosition = new Vector2(0, -204);
        var tempItemCfg = ItemconfigManager.inst.GetConfig(itemId);

        if (itemType == 16)
        {
            if(tempItemCfg != null)
            {
                contentPane.popupNameText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.name);
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(tempItemCfg.effect);
                contentPane.popupObj.SetSpriteURL(equipCfg.big_icon);
            }
            else
            {
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(itemId);
                if(equipCfg != null)
                {
                    contentPane.popupNameText.text = LanguageManager.inst.GetValueByKey(equipCfg.name);
                    contentPane.popupObj.SetSpriteURL(equipCfg.big_icon);
                }
                else
                {
                    contentPane.popupNameText.text = "";
                }
            }
            
        }
        else if (itemType == 26)
        {
            var qualityCfg = EquipConfigManager.inst.GetEquipDrawingsCfgByEquipId(itemId);
            EquipQualityConfig eqcfg = EquipConfigManager.inst.GetEquipQualityConfig(itemId);
            contentPane.popupObj.SetSpriteURL(qualityCfg.big_icon, StaticConstants.qualityTxtColor[eqcfg.quality - 1]);
            contentPane.popupNameText.text = LanguageManager.inst.GetValueByKey(qualityCfg.name);
        }
        else
        {
            if(tempItemCfg != null)
            {
                contentPane.popupNameText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.name);
                contentPane.popupObj.SetSpriteURL(tempItemCfg.icon);
            }
            else
            {
                contentPane.popupNameText.text = "";
            }
        }

        contentPane.tipsObj.gameObject.SetActive(rarity != 1);
        if (rarity == 2)
        {
            contentPane.tipsText.text = LanguageManager.inst.GetValueByKey("稀有");
            contentPane.tipsText.color = GUIHelper.GetColorByColorHex("#f77dff");
        }
        else if (rarity == 3)
        {
            contentPane.tipsText.text = LanguageManager.inst.GetValueByKey("专属图纸");
            contentPane.tipsText.color = GUIHelper.GetColorByColorHex("#ffc12c");
        }
        float tempAnchoreY = FGUI.inst.isLandscape ? 200 : 222;
        GameTimer.inst.AddTimer(0.05f, 1, () =>
          {
              contentPane.effectObj.gameObject.SetActive(true);
          });
        contentPane.popupRect.DOAnchorPos(new Vector2(0, tempAnchoreY), 0.2f);
        contentPane.popupObj.transform.DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack).OnStart(() =>
         {
             contentPane.popupObj.gameObject.SetActive(true);
             TBoxManager.inst.playAnim("shake");
         }).OnComplete(() =>
         {
             GameTimer.inst.AddTimer(0.35f, 1, () =>
             {
                 contentPane.popupNameText.DOFade(0, 0.2f).SetDelay(0.2f);
                 contentPane.tipsObj.DOFade(0, 0.2f).SetDelay(0.2f);
                 contentPane.tipsText.DOFade(0, 0.2f).SetDelay(0.2f);
                 contentPane.popupObj.iconImage.DOFade(0, 0.2f).OnComplete(() =>
                 {
                     contentPane.effectObj.gameObject.SetActive(false);

                     contentPane.popupObj.gameObject.SetActive(false);
                     contentPane.popupObj.iconImage.color = new Color(contentPane.popupObj.iconImage.color.r, contentPane.popupObj.iconImage.color.g, contentPane.popupObj.iconImage.color.b, 1);
                     contentPane.tipsObj.color = new Color(contentPane.tipsObj.color.r, contentPane.tipsObj.color.g, contentPane.tipsObj.color.b, 1);
                     contentPane.tipsText.color = new Color(contentPane.tipsText.color.r, contentPane.tipsText.color.g, contentPane.tipsText.color.b, 1);
                     contentPane.popupNameText.color = new Color(contentPane.popupNameText.color.r, contentPane.popupNameText.color.g, contentPane.popupNameText.color.b, 1);
                     index += 1;
                     if (index < items.Count)
                     {
                         setSinglePopupAwardAnim(items[index].itemId, items[index].quality, items[index].itemType);
                     }
                     else
                     {
                         TBoxManager.inst.CloseEffect();
                         AnimComplete();
                     }
                 }).SetDelay(0.2f);
             });
         });
    }

    private void AnimComplete()
    {
        TBoxManager.inst.setCurFalse();
        contentPane.completeObj.SetActive(true);
        for (int i = 0; i < contentPane.allReward.Count; i++)
        {
            int itemIndex = i;
            if (itemIndex < items.Count)
            {
                contentPane.allReward[itemIndex].transform.localScale = new Vector3(0, 0, 0);
                contentPane.allReward[itemIndex].gameObject.SetActive(true);
                contentPane.allReward[itemIndex].setData(items[itemIndex], itemIndex);
                var cfg = ItemconfigManager.inst.GetConfig(items[itemIndex].itemId);
                if (cfg != null && (ItemType)cfg.type == ItemType.EquipmentDrawing)
                {
                    equipDrawingIdList.Add(cfg.effect);
                }
            }
            else
            {
                contentPane.allReward[itemIndex].gameObject.SetActive(false);
            }
        }

        GameTimer.inst.AddTimer(0.2f, 1, () =>
          {
              //contentPane.horizontal.enabled = false;
              //contentPane.sizeFitter.enabled = false;

              for (int i = 0; i < items.Count; i++)
              {
                  int itemIndex = i;
                  contentPane.allReward[itemIndex].setAnimation(itemIndex);
              }
          });

        float tempOrthographicSize = FGUI.inst.isLandscape ? 3.7f : 9.53f;
        Camera.main.orthographicSize = tempOrthographicSize;
        Camera.main.transform.position = new Vector3(0.34f, 12.8f, 0);
    }

    protected override void onHide()
    {
        for (int i = 0; i < contentPane.allReward.Count; i++)
        {
            contentPane.allReward[i].clearData();
        }
        contentPane.completeObj.SetActive(false);
    }
}
