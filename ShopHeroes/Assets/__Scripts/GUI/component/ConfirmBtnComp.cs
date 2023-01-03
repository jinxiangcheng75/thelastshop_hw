using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBtnComp : MonoBehaviour
{
    public Button immediatelyUpgradeBtn;
    public Button confirmUpgradeBtn;

    private void Start()
    {
        immediatelyUpgradeBtn.onClick.AddListener(() =>
        {
            immediatelyUpgradeBtn.gameObject.SetActiveFalse();
            confirmUpgradeBtn.gameObject.SetActiveTrue();
        });
    }

    private void OnEnable()
    {
        immediatelyUpgradeBtn.gameObject.SetActiveTrue();
        confirmUpgradeBtn.gameObject.SetActiveFalse();
    }
}
