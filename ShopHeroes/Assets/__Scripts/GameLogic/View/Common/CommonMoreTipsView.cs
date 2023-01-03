using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonMoreTipsView : ViewBase<CommonMoreTipsComp>
{
    public override string viewID => ViewPrefabName.CommonMoreTips;
    public override string sortingLayerName => "top";
    public override int showType => (int)ViewShowType.normal;
    List<CommonMoreTipsItem> allItems;
    int count = 0;
    protected override void onInit()
    {
        base.onInit();
        contentPane.bgBtn.onClick.AddListener(hide);
        allItems = new List<CommonMoreTipsItem>();
    }

    public void showIntroducePanel(List<CommonRewardData> allData, Transform trans)
    {
        if (allData == null) return;
        count = allData.Count;
        if (allData.Count > allItems.Count)
        {
            int differenceCount = allData.Count - allItems.Count;
            for (int i = 0; i < differenceCount; i++)
            {
                CommonMoreTipsItem go = GameObject.Instantiate(contentPane.prefabItem, contentPane.cloneTrans);
                //go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-39.5f, 60 - 60 * (allItems.Count - 1));
                allItems.Add(go);
            }
        }

        for (int i = 0; i < allItems.Count; i++)
        {
            int index = i;
            if (index < allData.Count)
            {
                allItems[index].setData(allData[index]);
            }
            else
            {
                allItems[index].clearData();
            }
        }

        setIntroduceBg(trans);
    }

    private void setIntroduceBg(Transform trans)
    {
        float bgHeight = 72 + 72 * count;
        contentPane.bgRect.sizeDelta = new Vector2(contentPane.bgRect.sizeDelta.x, bgHeight);
        var cloneRectTrans = contentPane.cloneTrans.GetComponent<RectTransform>();
        cloneRectTrans.sizeDelta = new Vector2(cloneRectTrans.sizeDelta.x, bgHeight - 38);

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
        float bgHalfHeight = contentPane.bgRect.sizeDelta.y / 2;
        float addNum = trans.GetComponent<RectTransform>().sizeDelta.y / 2 + bgHalfHeight * 0.3f;
        anchorePos += new Vector2(226, addNum);

        float bgWidth = contentPane.bgRect.rect.width / 2;
        float bgHeight = contentPane.bgRect.rect.height / 2;    //540 100 -440 -540     -11 -250
        float calculateX = 0;
        float calculateY = screenPoint.y - canvasHeight;

        if (rectTrans.anchorMax.x == 1 && rectTrans.anchorMax.y == 1 && rectTrans.anchorMin.x == 1 && rectTrans.anchorMin.y == 1)
        {
            calculateX = screenPoint.x + canvasWidth;
        }
        else
        {
            calculateX = screenPoint.x - canvasWidth;
        }

        if (/*screenPoint.x - screenWidth*/calculateX + 186 < bgWidth - canvasWidth)
        {
            contentPane.bgRect.localScale = new Vector3(-1, -1, 1);
            anchorePos = new Vector2(bgWidth - canvasWidth, anchorePos.y);
        }
        else if (/*screenPoint.x - screenWidth*/calculateX + 186 > canvasWidth - bgWidth)
        {
            contentPane.bgRect.localScale = new Vector3(1, -1, 1);
            anchorePos = new Vector2(screenPoint.x - canvasWidth - 160, anchorePos.y);
        }

        var cloneRectTrans = contentPane.cloneTrans.GetComponent<RectTransform>();
        float changeVal = contentPane.bgRect.pivot.y > 0.5f ? (contentPane.bgRect.pivot.y - 0.5f) * bgHalfHeight + contentPane.bgRect.anchoredPosition.y : (0.5f - contentPane.bgRect.pivot.y) * bgHalfHeight - contentPane.bgRect.anchoredPosition.y;
        if (calculateY + addNum + changeVal > canvasHeight - bgHeight)
        {
            contentPane.bgRect.localScale = new Vector3(contentPane.bgRect.localScale.x, 1, 1);
            anchorePos = new Vector2(anchorePos.x, screenPoint.y - canvasHeight);
            contentPane.cloneTrans.GetComponent<RectTransform>().anchoredPosition = new Vector2(cloneRectTrans.anchoredPosition.x, cloneRectTrans.sizeDelta.y - contentPane.bgRect.sizeDelta.y - 38);
        }
        else
        {
            contentPane.cloneTrans.GetComponent<RectTransform>().anchoredPosition = new Vector2(cloneRectTrans.anchoredPosition.x, cloneRectTrans.sizeDelta.y);
        }

        contentPane.moveObjTrans.anchoredPosition = anchorePos;
        //cloneRectTrans.anchoredPosition = new Vector2(cloneRectTrans.anchoredPosition.x, cloneRectTrans.sizeDelta.y);
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
