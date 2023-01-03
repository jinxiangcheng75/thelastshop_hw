using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryGetRewardComp : MonoBehaviour
{
    public Button confirmBtn;
    public Button singleConfirmBtn;
    public GUIIcon singleIcon;
    public Animator singleAnim;
    public Text singleDescText;
    public GameObject singleObj;
    public GameObject allObj;
    public Transform effectTrans;
    public List<LotteryRewardItem> allItems;
}
