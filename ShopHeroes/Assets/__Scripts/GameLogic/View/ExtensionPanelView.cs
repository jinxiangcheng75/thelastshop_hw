using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

/// <summary>
/// 扩建面板    #陆泓屹
/// </summary>

public class ExtensionPanelView : ViewBase<ExtensionPanelComp>
{
    public override string viewID => ViewPrefabName.ExtensionPanel;
    public override int showType => (int)ViewShowType.normal;
    public override string sortingLayerName => "window";

    //店铺的最大阶段
    public int shopMaxSequence;

    private int x1;           //当前等级的x
    private int y1;           //当前等级的y
    private int x2;          //下一等级的x
    private int y2;          //下一等级的y

    //标准化后的值
    private int x3;         //当前等级的x
    private int y3;         //当前等级的y
    private int x4;         //下一等级的x
    private int y4;         //下一等级的y

    private float scaleFactor;      //缩放系数
    private float xFactor = 0.054f;
    private float yFactor = 0.027f;

    private GridLayoutGroup layoutObj;        //取content中的GridLayoutGroup组件
    private Stack<GameObject> items;          //旧区域块预制件栈
    private GameObject[] partOldImgs;         //隔离每行独立的需要替换的新的块部分
    private ExtensionConfig[] infoArray;

    protected override void onInit()
    {
        isShowResPanel = true;
        EventController.inst.TriggerEvent(GameEventType.ExtensionEvent.EXTENSION_CALLBACKS_SHOPUPGRADE);

        items = new Stack<GameObject>();

        layoutObj = contentPane.content.GetComponent<GridLayoutGroup>();

        shopMaxSequence = ExtensionConfigManager.inst.GetExtensionLvMax();
        //监听事件
        contentPane.upgradeBtn.ButtonClickTween(() =>
        {

            if (UserDataProxy.inst.playerData.level < infoArray[1].shopkeeper_level)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("店主等级不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }

            if (UserDataProxy.inst.playerData.gold < infoArray[1].money)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("新币不足"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }


            EventController.inst.TriggerEvent(GameEventType.ExtensionEvent.EXTENSION_POST_COINEXTENSION);
        });


        contentPane.upgradeImmediatelyBtn.ButtonClickTween(() =>
        {
            if (!contentPane.confirmUpgradeObj.activeSelf)
            {
                contentPane.confirmUpgradeObj.SetActive(true);
            }
            else
            {
                if (UserDataProxy.inst.playerData.gem < infoArray[1].diamond)
                {
                    //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("金条不足"), GUIHelper.GetColorByColorHex("FF2828"));
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31,null, infoArray[1].diamond - UserDataProxy.inst.playerData.gem);
                    return;
                }

                EventController.inst.TriggerEvent(GameEventType.ExtensionEvent.EXTENSION_POST_DIAMEXTENSION);
            }
        });

        contentPane.closeBtn.ButtonClickTween(() => EventController.inst.TriggerEvent(GameEventType.HIDEUI_EXTENSIONPANEL));
    }
    protected override void onShown()
    {
        AudioManager.inst.PlaySound(140);
        int shopLevel = UserDataProxy.inst.shopData.shopLevel;
        int shopState = UserDataProxy.inst.shopData.currentState;

        //这一等级与下一等级的扩建配置得添加进数组中
        infoArray = new ExtensionConfig[2];
        infoArray[0] = ExtensionConfigManager.inst.GetExtensionConfig(shopLevel);
        infoArray[1] = ExtensionConfigManager.inst.GetExtensionConfig(shopLevel + 1);

        x1 = infoArray[0].size_x;
        y1 = infoArray[0].size_y;
        x2 = infoArray[1].size_x;
        y2 = infoArray[1].size_y;

        //未在扩建时的情况
        if (shopState != 0) return;
        NormalizeSize();
        //设置缩放比例
        scaleFactor = /*x4 * y4 <= 12 ? 1 :*/ 1 - (y4 * yFactor + x4 * xFactor);
        contentPane.content.GetComponent<RectTransform>().localScale =
            new Vector3(scaleFactor, scaleFactor, 1);
        ShowPanelContent();
        InitBlocks();

        contentPane.btn_NeedLevel_Txt.text = infoArray[1].shopkeeper_level.ToString();
        contentPane.btn_NeedLevel_Txt.color = UserDataProxy.inst.playerData.level < infoArray[1].shopkeeper_level ? GUIHelper.GetColorByColorHex("FD4F4F") : GUIHelper.GetColorByColorHex("FFFFFF");

        contentPane.notArriveLv.SetActive(UserDataProxy.inst.playerData.level < infoArray[1].shopkeeper_level);
        contentPane.arriveLv.enabled = UserDataProxy.inst.playerData.level >= infoArray[1].shopkeeper_level;


        contentPane.btn_NeedCoin_Txt.text = infoArray[1].money.ToString("N0");
        contentPane.btn_NeedCoin_Txt.color = UserDataProxy.inst.playerData.gold < infoArray[1].money ? GUIHelper.GetColorByColorHex("FD4F4F") : GUIHelper.GetColorByColorHex("FFFFFF");

        contentPane.btn_NeedDiam_Txt1.text = infoArray[1].diamond.ToString("N0");
        //contentPane.btn_NeedDiam_Txt1.color = UserDataProxy.inst.playerData.gem < infoArray[1].diamond ? GUIHelper.GetColorByColorHex("FD4F4F") : GUIHelper.GetColorByColorHex("FFFFFF");


        contentPane.canUpgradeObj.SetActive(infoArray[0].sequence < shopMaxSequence);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    protected override void onHide()
    {


        contentPane.confirmUpgradeObj.SetActive(false);
        items.Clear();

        if (partOldImgs != null)
        {
            Array.Clear(partOldImgs, 0, partOldImgs.Length);
        }

        for (int i = 0; i < contentPane.content.transform.childCount; i++)
        {
            GameObject.Destroy(contentPane.content.transform.GetChild(i).gameObject);
        }
    }

    //给Panel里的组件赋值
    public void ShowPanelContent()
    {
        contentPane.area_OldCount_Txt.text = $"{x1}x{y1}";
        contentPane.area_NewCount_Txt.text = $"{x2}x{y2}";

        //家具空间Item文本组件
        contentPane.space_OldCount_Txt.text = $"{infoArray[0].furniture}";
        contentPane.space_NewCount_Txt.text = $"{infoArray[1].furniture}(+{infoArray[1].furniture - infoArray[0].furniture})";

        //升级所需时间Item文本组件
        contentPane.upgrade_NeedTime_Txt.text = $"{TimeUtils.timeSpanStrip(infoArray[1].time)}";
    }

    private void NormalizeSize()
    {
        x3 = Normalize(x1);
        y3 = Normalize(y1);
        x4 = Normalize(x2);
        y4 = Normalize(y2);
    }

    private int Normalize(int x)
    {
        return x / 2;
        // return x / 3;
    }

    private void InitBlocks()
    {
        if (x4 - x3 > 0 && y4 - y3 == 0)
        {
            //左下角生成方块
            layoutObj.startAxis = Axis.Horizontal;
            layoutObj.constraintCount = y4;

            InstantiatePfb(x4, y4);
        }
        else if (y4 - y3 > 0 && x4 - x3 == 0)
        {
            //右下角生成方块
            layoutObj.startAxis = Axis.Vertical;
            layoutObj.constraintCount = y4;

            InstantiatePfb(x4, y4);
        }
        else if (y4 - y3 > 0 && x4 - x3 > 0)
        {
            //两边都生成方块
            layoutObj.startAxis = Axis.Horizontal;
            layoutObj.constraintCount = y4;

            for (int i = x4 * y4; i > 0; i--)
            {
                GameObject item = GameObject.Instantiate(contentPane.oldPfb, contentPane.content.transform);
                items.Push(item);
            }

            for (int i = (x4 - x3) * y4; i > 0; i--)
            {
                GameObject item = items.Pop();
                item.gameObject.GetComponent<Image>().sprite = contentPane.newImg.sprite;
            }

            partOldImgs = items.ToArray();
            Array.Reverse(partOldImgs);

            for (int i = 0; i < partOldImgs.Length; i += y4)
            {
                for (int j = y3; j < y4; j++)
                {
                    partOldImgs[i + j].gameObject.GetComponent<Image>().sprite = contentPane.newImg.sprite;
                }
            }
        }
    }

    private void InstantiatePfb(int x, int y)
    {
        for (int i = x * y; i > 0; i--)
        {
            GameObject item = GameObject.Instantiate(contentPane.oldPfb, contentPane.content.transform);
            items.Push(item);
        }

        for (int i = x4 * y4 - x3 * y3; i > 0; i--)
        {
            GameObject item = items.Pop();
            item.gameObject.GetComponent<Image>().sprite = contentPane.newImg.sprite;
        }
    }
}
