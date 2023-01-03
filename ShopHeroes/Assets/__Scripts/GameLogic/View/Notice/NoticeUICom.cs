using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUICom : MonoBehaviour
{
    public Button closeBtn;
    public Text titleText;
    public Text contentText;
}

public class NoticeUIView : ViewBase<NoticeUICom>
{
    public override string viewID => ViewPrefabName.ProclamationUI;
    public override string sortingLayerName => "popup";

    protected override void onInit()
    {
        contentPane.closeBtn.onClick.AddListener(hide);
    }

    protected override void onShown()
    {

    }

    protected override void onHide()
    {

    }

    public void setNoticeData(Notice notice)
    {
        contentPane.titleText.text = notice.noticeTitle;
        string desText = "";
        if (notice.noticeContent != null)
        {
            desText = notice.noticeContent.Replace("\\n", "\n");
        }
        contentPane.contentText.text = desText;
    }
}
