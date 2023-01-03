using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OpenTreasureBoxUIView : ViewBase<OpenTreasureBoxUICom>
{
    public override string viewID => ViewPrefabName.OpenTreasureBoxUI;
    public override int showType => (int)ViewShowType.normal;
    public override string sortingLayerName => "window";
    TreasureBoxData data;
    Tween tween;

    protected override void onInit()
    {
        base.onInit();
        topResPanelType = TopPlayerShowType.noRoleAndSettingAndEnergy;
        isShowResPanel = true;

        AddUIEvent();
    }

    public override void shiftIn()
    {
        base.shiftIn();
        OpenTreasureBoxPanel();
        //if (data != null && data.boxId != 0)
        //setData(data.boxId);
    }

    private void AddUIEvent()
    {
        contentPane.useByKeyBtn.onClick.AddListener(() =>
        {
            buyBtnClick(0);
        });
        contentPane.useByGemBtn.onClick.AddListener(() =>
        {
            if (UserDataProxy.inst.playerData.gem >= data.costGem)
            {
                if (contentPane.sureAgainObj.enabled)
                {
                    contentPane.sureAgainObj.enabled = false;
                    buyBtnClick(1);
                }
                else
                {
                    contentPane.sureAgainObj.enabled = true;
                }
            }
            else
            {
                buyBtnClick(1);
            }
        });
        contentPane.leftBtn.onClick.AddListener(() =>
        {
            setData(TreasureBoxDataProxy.inst.GetLastBox(data));
        });
        contentPane.rightBtn.onClick.AddListener(() =>
        {
            setData(TreasureBoxDataProxy.inst.GetNextBox(data));
        });
        contentPane.chanceBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.BOXINFO_SHOWUI, kTreasureBoxInfoType.Chance, data.boxItemId);
        });
        contentPane.selectBoxBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ExploreEvent.EXPLOREUSEITEM_SHOWUI, 100);
        });
        contentPane.infoBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.BOXINFO_SHOWUI, kTreasureBoxInfoType.Exclusive, data.boxItemId);
        });
        contentPane.cityBtn.onClick.AddListener(() =>
        {
            if (TreasureBoxDataProxy.inst.isBackToTown)
                HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Town, true));
            else
            {
                TreasureBoxDataProxy.inst.isBackToTown = true;
                HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Shop, true));
            }
        });
    }

    public void setData(int boxId)
    {
        if (boxId == -1) return;
        contentPane.sureAgainObj.enabled = false;
        contentPane.useByKeyBtn.interactable = false;
        contentPane.useByGemBtn.interactable = false;
        data = TreasureBoxDataProxy.inst.GetDataByBoxId(boxId);
        if (data == null)
        {
            //Logger.error("没有boxId是" + boxId + "的宝箱数据");
            return;
        }

        if (data.count <= 0)
        {
            GUIHelper.SetUIGray(contentPane.useByKeyBtn.transform, true);
            GUIHelper.SetUIGray(contentPane.useByGemBtn.transform, true);
        }
        else
        {
            GUIHelper.SetUIGray(contentPane.useByKeyBtn.transform, false);
            GUIHelper.SetUIGray(contentPane.useByGemBtn.transform, false);
        }

        TBoxManager.inst.setBox(boxId - 50001, () =>
         {
             setBtnState();
         });
        var keyItemCfg = ItemconfigManager.inst.GetConfig(data.keyId);
        itemConfig cfg = ItemconfigManager.inst.GetConfig(data.boxItemId);
        contentPane.boxIcon.SetSprite(cfg.atlas, cfg.icon);
        contentPane.keyIcon.SetSprite(keyItemCfg.atlas, keyItemCfg.icon);
        contentPane.keyNumText.text = data.keyCount + "/1";
        contentPane.keyNumText.color = data.keyCount >= 1 ? Color.white : Color.red;
        contentPane.gemNumText.text = data.costGem.ToString();
        //contentPane.gemNumText.color = UserDataProxy.inst.playerData.gem >= data.costGem ? Color.white : Color.red;
        contentPane.unlockNumText.text = data.GetUnlockItemCount() + "/" + data.items.Count;
        contentPane.curBoxNumText.text = data.count.ToString();

        setTweenAnim();
    }

    private void setBtnState()
    {
        if (contentPane == null) return;
        if (data.count <= 0)
        {
            contentPane.useByKeyBtn.interactable = false;
            contentPane.useByGemBtn.interactable = false;
        }
        else
        {
            contentPane.useByKeyBtn.interactable = true;
            contentPane.useByGemBtn.interactable = true;
        }
    }

    private void setTweenAnim()
    {
        if (tween != null)
        {
            return;
        }
        int timer = 0;
        tween = DOTween.To(() => timer, x => timer = x, 3, 2).OnStepComplete(() =>
        {
            if (contentPane.tipsObj != null)
            {
                if (contentPane.tipsObj.activeSelf)
                {
                    contentPane.tipsObj.transform.DOScale(0, 0.5f).From(1).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        if (contentPane.tipsObj != null)
                            contentPane.tipsObj.SetActive(false);
                    });
                }
                else
                {
                    contentPane.tipsObj.transform.DOScale(1, 0.5f).From(0).SetEase(Ease.OutBack).OnStart(() =>
                    {
                        if (contentPane.tipsObj != null)
                            contentPane.tipsObj.SetActive(true);
                    });
                }
            }
        }).SetLoops(-1);
    }

    public void OpenTreasureBoxPanel()
    {
        bool hasBox = TreasureBoxDataProxy.inst.hasBox;
        var boxList = TreasureBoxDataProxy.inst.boxList;

        contentPane.chanceBtn.gameObject.SetActive(hasBox);
        contentPane.leftBtn.gameObject.SetActive(hasBox);
        contentPane.rightBtn.gameObject.SetActive(hasBox);
        contentPane.useByGemBtn.gameObject.SetActive(hasBox);
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
        {
            contentPane.useByGemBtn.gameObject.SetActive(false);
        }
        contentPane.useByKeyBtn.gameObject.SetActive(hasBox);
        contentPane.infoBtn.gameObject.SetActive(hasBox);
        contentPane.boxNumObj.SetActive(hasBox);
        contentPane.hasBoxObj.enabled = hasBox;
        contentPane.hasBoxObj.color = hasBox ? Color.white : GUIHelper.GetColorByColorHex("#7E7E7E");
        contentPane.noBoxTipsObj.enabled = !hasBox;
        if (hasBox)
        {
            contentPane.selectBoxText.text = LanguageManager.inst.GetValueByKey("选择宝箱");
        }
        else
        {
            contentPane.boxIcon.SetSprite("tbox_atlas", "zhuejiemian_baoxiang");
            TBoxManager.inst.setCurFalse();
            contentPane.selectBoxText.text = LanguageManager.inst.GetValueByKey("没有宝箱");
            return;
        }

        if (TreasureBoxDataProxy.inst.newBoxGroupId != 0)
        {
            setData(TreasureBoxDataProxy.inst.newBoxGroupId);
            TreasureBoxDataProxy.inst.newBoxGroupId = 0;
        }
        else
        {
            if (data != null && data.count > 0)
                setData(data.boxItemId);
            else
            {
                var targetData = boxList.Find(t => t.count > 0);
                if (targetData != null)
                    setData(boxList.Find(t => t.count > 0).boxItemId);
            }

        }
    }

    private void buyBtnClick(int costType)
    {
        if (costType == 0)
        {
            if (data.keyCount <= 0)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您没有适合此宝箱的钥匙"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }
        }
        else if (costType == 1)
        {
            if (UserDataProxy.inst.playerData.gem < data.costGem)
            {
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不够"), GUIHelper.GetColorByColorHex("FF2828"));
                HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, data.costGem - UserDataProxy.inst.playerData.gem);
                return;
            }
        }

        FGUI.inst.showGlobalMask(0.5f);
        EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.REQUEST_OPENTREASUREBOX, data.boxId, costType);
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {
        contentPane.sureAgainObj.enabled = false;
        if (tween != null)
        {
            tween.Kill(true);
            tween = null;
        }
    }
}
