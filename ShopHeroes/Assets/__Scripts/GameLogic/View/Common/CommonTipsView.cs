using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonTipsView : ViewBase<CommonTipsComp>
{
    public override string viewID => ViewPrefabName.CommonTips;
    public override int showType => (int)ViewShowType.normal;
    public override string sortingLayerName => "top";

    RectTransform nameRect;

    protected override void onInit()
    {
        base.onInit();
        //topResPanelType = TopPlayerShowType.none;
        //isShowResPanel = true;
        contentPane.bgBtn.onClick.AddListener(hide);
        nameRect = contentPane.nameText.GetComponent<RectTransform>();
    }

    public void showTitleContent(string title, string content, Transform trans)
    {
        contentPane.vipObj.SetActive(false);

        contentPane.nameText.text = title;
        contentPane.descText.text = content;

        float bgHeight = contentPane.nameText.preferredHeight + contentPane.descText.preferredHeight + 40;
        if (bgHeight < 240)
            contentPane.bgRect.sizeDelta = new Vector2(contentPane.bgRect.sizeDelta.x, 240);
        else
            contentPane.bgRect.sizeDelta = new Vector2(contentPane.bgRect.sizeDelta.x, bgHeight);

        //nameRect.sizeDelta = new Vector2(data.isVipItem ? 290 : 520, nameRect.sizeDelta.y);
        nameRect.sizeDelta = new Vector2(520, nameRect.sizeDelta.y);

        setIntroducePos(trans);
    }

    public void showIntroducePanel(CommonRewardData data, Transform trans)
    {
        //contentPane.tuzhiImg.enabled = false;

        if ((ItemType)data.itemType == ItemType.Craftsman)
        {
            var workerItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            WorkerConfig workerCfg = null;
            if (workerItemCfg != null)
            {
                workerCfg = WorkerConfigManager.inst.GetConfig(workerItemCfg.effect);
            }
            else
            {
                workerCfg = WorkerConfigManager.inst.GetConfig(data.rewardId);
            }

            if (workerCfg != null)
            {
                //contentPane.icon.SetSprite("portrait_atlas", workerCfg.pic);
                contentPane.nameText.text = LanguageManager.inst.GetValueByKey(workerCfg.name);
                contentPane.descText.text = LanguageManager.inst.GetValueByKey(workerCfg.desc);
            }
            else
            {
                if (workerItemCfg != null)
                {
                    contentPane.nameText.text = LanguageManager.inst.GetValueByKey(workerItemCfg.name);
                    contentPane.descText.text = LanguageManager.inst.GetValueByKey(workerItemCfg.desc);
                }
            }
        }
        else if ((ItemType)data.itemType == ItemType.Equip)
        {
            var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(data.rewardId);
            if (equipCfg != null)
            {
                //contentPane.icon.SetSprite(equipCfg.equipDrawingsConfig.atlas, equipCfg.equipDrawingsConfig.icon, StaticConstants.qualityColor[equipCfg.equipQualityConfig.quality - 1]);
                contentPane.nameText.text = equipCfg.quality_name;
                contentPane.descText.text = LanguageManager.inst.GetValueByKey(equipCfg.equipDrawingsConfig.desc);
            }
        }
        else if ((ItemType)data.itemType == ItemType.EquipmentDrawing)
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            EquipDrawingsConfig equipDrawingCfg = null;
            if (tempItemCfg != null)
            {
                equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(tempItemCfg.effect);
            }
            else
            {
                equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(data.rewardId);
            }
            if (equipDrawingCfg != null)
            {
                //contentPane.icon.SetSprite(equipDrawingCfg.atlas, equipDrawingCfg.icon);
                contentPane.nameText.text = LanguageManager.inst.GetValueByKey(equipDrawingCfg.name);
                if (tempItemCfg != null)
                    contentPane.descText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.desc);
                //contentPane.tuzhiImg.enabled = true;
            }
        }
        else if ((ItemType)data.itemType == ItemType.ShopkeeperDress)
        {
            var itemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            dressconfig dressCfg = null;
            if (itemCfg != null)
            {
                dressCfg = dressconfigManager.inst.GetConfig(itemCfg.effect);
            }
            else
            {
                dressCfg = dressconfigManager.inst.GetConfig(data.rewardId);
            }

            if (dressCfg != null)
            {
                //contentPane.icon.SetSprite("ClotheIcon_atlas", dressCfg.icon);
                contentPane.nameText.text = LanguageManager.inst.GetValueByKey(dressCfg.name);
                if (itemCfg != null)
                    contentPane.descText.text = LanguageManager.inst.GetValueByKey(itemCfg.desc);
                else
                {
                    itemCfg = ItemconfigManager.inst.GetConfigByEffectId(dressCfg.id);
                    if(itemCfg != null)
                    {
                        contentPane.descText.text = LanguageManager.inst.GetValueByKey(itemCfg.desc);
                    }
                    else
                    {
                        contentPane.descText.text = "";
                    }
                }
            }
            else
            {
                if (itemCfg != null)
                {
                    contentPane.nameText.text = LanguageManager.inst.GetValueByKey(itemCfg.name);
                    contentPane.descText.text = LanguageManager.inst.GetValueByKey(itemCfg.desc);
                }

            }
            //var dressCfg = dressconfigManager.inst.GetConfig(itemCfg.effect);
            //contentPane.icon.SetSprite("ClotheIcon_atlas", dressCfg.icon);

            //contentPane.nameText.text = LanguageManager.inst.GetValueByKey(dressCfg.name);
            //contentPane.descText.text = LanguageManager.inst.GetValueByKey(itemCfg.desc);
        }
        else if (data.itemType == 1600)
        {
            var equipDrawingCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(data.rewardId);
            if (equipDrawingCfg != null)
            {
                //contentPane.icon.SetSprite(equipDrawingCfg.atlas, equipDrawingCfg.icon);
                contentPane.nameText.text = LanguageManager.inst.GetValueByKey(equipDrawingCfg.name);
                contentPane.descText.text = LanguageManager.inst.GetValueByKey(equipDrawingCfg.desc);
                //contentPane.tuzhiImg.enabled = true;
            }
        }
        else
        {
            var tempItemCfg = ItemconfigManager.inst.GetConfig(data.rewardId);
            if (tempItemCfg != null)
            {
                //contentPane.icon.SetSprite(tempItemCfg.atlas, tempItemCfg.icon);
                contentPane.nameText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.name);
                contentPane.descText.text = LanguageManager.inst.GetValueByKey(tempItemCfg.desc);
            }
        }

        //contentPane.numText.text = data.count.ToString();
        //contentPane.numText.enabled = data.count > 1;

        float bgHeight = contentPane.nameText.preferredHeight + contentPane.descText.preferredHeight + 40;
        if (bgHeight < 240)
            contentPane.bgRect.sizeDelta = new Vector2(contentPane.bgRect.sizeDelta.x, 240);
        else
            contentPane.bgRect.sizeDelta = new Vector2(contentPane.bgRect.sizeDelta.x, bgHeight);

        contentPane.vipObj.SetActive(data.specialType == 1);
        contentPane.sevenPassObj.SetActive(data.specialType == 2);

        nameRect.sizeDelta = new Vector2(data.specialType != 0 ? 290 : 520, nameRect.sizeDelta.y);

        setIntroducePos(trans);
    }

    private void setIntroducePos(Transform trans)
    {
        Vector3 screenPoint = FGUI.inst.uiCamera.WorldToScreenPoint(trans.position);
        float canvasWidth = FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.width / 2;
        float canvasHeight = FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.height / 2;
        float screenWidth = /*FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.width*/Screen.width / 2;
        float screenHeight = /*FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.width*/Screen.height / 2;
        RectTransform rectTrans = trans as RectTransform;
        Vector2 anchorePos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), screenPoint, FGUI.inst.uiCamera, out anchorePos);
        screenPoint = new Vector3(screenPoint.x * canvasWidth / screenWidth, screenPoint.y * canvasHeight / screenHeight, screenPoint.z);
        contentPane.bgRect.localScale = new Vector3(-1, -1, 1);
        float addNum = trans.GetComponent<RectTransform>().sizeDelta.y / 2 + contentPane.bgRect.sizeDelta.y / 2 - 15;
        anchorePos += new Vector2(236, addNum);

        float bgWidth = contentPane.bgRect.rect.width / 2;    //540 100 -440 -540     -11 -250
        float bgHeight = contentPane.bgRect.rect.height / 2;    //540 100 -440 -540     -11 -250
        float calculateX = 0;
        float calculateY = screenPoint.y - canvasHeight;
        Logger.log("screen = " + screenPoint);
        //Logger.error("screenwidth = " + );
        if (rectTrans.anchorMax.x == 1 && rectTrans.anchorMax.y == 1 && rectTrans.anchorMin.x == 1 && rectTrans.anchorMin.y == 1)
        {
            calculateX = screenPoint.x + canvasWidth;
            //calculateY = screenPoint.y +
        }
        else
        {
            calculateX = screenPoint.x - canvasWidth;
        }
        //calculateX *= Screen.width / FGUI.inst.uiRootTF.GetComponent<RectTransform>().rect.width;
        Logger.log("计算出来的x是" + calculateX);
        if (calculateX /*- screenWidth*/ + 236 < bgWidth - canvasWidth)
        {
            //Logger.error("??");
            contentPane.bgRect.localScale = new Vector3(-1, -1, 1);
            anchorePos = new Vector2(bgWidth - canvasWidth, anchorePos.y);
        }
        else if (calculateX /*- screenWidth*/ + 236 > canvasWidth - bgWidth)
        {
            //Logger.error("???");
            contentPane.bgRect.localScale = new Vector3(1, -1, 1);
            anchorePos = new Vector2(screenPoint.x - canvasWidth - 236, anchorePos.y);
        }

        if (calculateY + addNum > canvasHeight - bgHeight)
        {
            contentPane.bgRect.localScale = new Vector3(contentPane.bgRect.localScale.x, 1, 1);
            anchorePos = new Vector2(anchorePos.x, screenPoint.y - canvasHeight - addNum);
        }

        contentPane.moveObjTrans.anchoredPosition = anchorePos;
    }

    protected override void onShown()
    {
        //AudioManager.inst.PlaySound(10);
    }

    protected override void onHide()
    {
        //AudioManager.inst.PlaySound(11);
    }
}
