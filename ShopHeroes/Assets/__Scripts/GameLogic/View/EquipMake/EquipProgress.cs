using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipProgress : MonoBehaviour
{
    public GUIIcon icon;
    public Text valueTx;
    public Text nameTx;
    public Image progresBar;
    public Image makeImg;
    public Image endImage;
    public int progressState = 0;
    public Text expTx;
    public RectTransform iconBg;
    private bool end = false;


    public void setInfo(int _index, progressItemInfo info, float Progress)
    {
        end = Progress >= 1;
        progresBar.fillAmount = Progress;
        expTx.text = end ? "" : info.exp.ToString();
        nameTx.text = LanguageManager.inst.GetValueByKey(info.dec);
        GUIHelper.SetMilestonesIconText(info, ref icon, ref valueTx);
        iconBg.gameObject.SetActive(info.type == 7);
        makeImg.gameObject.SetActive(!end);
        endImage.gameObject.SetActive(end);
    }
}
