using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerRecruitUIComp : MonoBehaviour
{
    public Transform workerTf;
    public Text nameText;
    public Text contentText;
    public Text typeText;
    public List<GameObject> unlockItems;
    public List<GUIIcon> unlockItemIcons;
    public List<Button> unlockItemBtns;
    public Button leftPageBtn;
    public Button rightPageBtn;
    public Button goldBtn;
    public Text needLvTx;
    public Text goldCostTx;
    public Button gemBtn;
    public GameObject gemConfirmObj;
    public Text gemCostTx;
    public Text orTx;
    public Text lockedTx;
    public Button otherBtn;
    public Text otherBtnTx;
    public Button closeBtn;

    public GameObject notArriveLv;
    public Text arriveLv;
}
