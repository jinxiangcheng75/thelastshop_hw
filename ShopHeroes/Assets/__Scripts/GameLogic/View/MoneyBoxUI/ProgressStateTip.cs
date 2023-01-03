using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressStateTip : MonoBehaviour
{
    //public Image currBg;
    public Image icon;
    public Image bigIcon;
    public Image finishImage;
    private int state = 0;  //0 当前未开始 1 当前进行中 2 当前待领取 3 已完成 4 不是当前未开始
    public int State
    {
        get
        {
            return state;
        }
        set
        {
            //  if (state != value)
            //  {
            state = value;
            //currBg.enabled = state != 3 && state != 4;
            bigIcon.enabled = state == 1 || state == 2;
            icon.enabled = state == 0 || state == 4;
            finishImage.enabled = state == 3;
            if (state == 0 || state == 4)
            {
                icon.color = GUIHelper.GetColorByColorHex("#808080");
            }
            else
            {
                icon.color = Color.white;
            }
            //  }
        }
    }

}
