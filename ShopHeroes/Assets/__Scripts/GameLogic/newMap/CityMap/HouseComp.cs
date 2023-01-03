using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HouseComp : MonoBehaviour
{
    [SerializeField]
    private Follow2DTarget follow2DTarget;
    [SerializeField]
    private Image lvImg;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Slider lvProgress;
    [SerializeField]
    private Transform lockTF;
    [SerializeField]
    private Transform unLockTF;
    [SerializeField]
    private Text canInvestText;
    [SerializeField]
    private GameObject redPointObj;

    public InputEventListener eventListener;

    public Action unLockHandler;
    public Action lockHandler;

    public float moveBase = 110;
    public float unLockCommonMoveDis = 110;


    public Transform followTarget
    {
        get { return follow2DTarget.target; }
        set { follow2DTarget.target = value; }
    }

    public void setHouseInfo(int houseid)
    {
        var config = BuildingConfigManager.inst.GetConfig(houseid);
        if (config != null && FGUI.inst != null)
        {
            CityBuildingData buildingData = UserDataProxy.inst.GetBuildingData(houseid);
            if (buildingData != null)
            {
                canInvestText.text = LanguageManager.inst.GetValueByKey("可投资");
                GUIHelper.SetUIGrayColor(follow2DTarget.target, buildingData.state == 0 ? 0.66f : 1f);
                unLockTF.gameObject.SetActive(buildingData.state != 0);

                if (buildingData.state == 0)
                {
                    lockTF.gameObject.SetActive(true);
                    lockTF.localPosition = Vector3.up * moveBase;
                }
                else
                {
                    lockTF.gameObject.SetActive(false);
                }

                if (buildingData.state != 0)
                {
                    unLockHandler?.Invoke();
                    //this.lvText.text = buildingData.level.ToString();
                    if (buildingData.config.architecture_type == 2 && buildingData.buildingId != 2300)
                    {
                        redPointObj.SetActiveFalse();

                        unLockTF.localPosition = Vector3.up * moveBase;

                        this.nameText.text = LanguageManager.inst.GetValueByKey(config.name);
                        this.lvImg.gameObject.SetActive(false);
                        this.lvProgress.gameObject.SetActive(false);
                    }
                    else
                    {
                        unLockTF.localPosition = Vector3.up * (moveBase + unLockCommonMoveDis);

                        this.nameText.text = LanguageManager.inst.GetValueByKey(config.name) + (buildingData.buildingId == 2300 ? "" : "[Lv." + buildingData.level + "]");

                        this.lvImg.gameObject.SetActive(true);
                        this.lvProgress.gameObject.SetActive(true);

                        float progress = 0;

                        if (buildingData.buildingId == 2300)
                        {

                            EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.HUD_SCIENCEBUILDINGREFRESH);

                            var buildingList = UserDataProxy.inst.GetAllCanShowScienceBuildingData().FindAll(t => t.isNew);
                            redPointObj.SetActive(buildingList.Count > 0);

                            var list = UserDataProxy.inst.GetAllCanShowScienceBuildingData();

                            int costCount = 0;
                            int needClickCount = 0;
                            bool showCanInvestTx = false;

                            foreach (var data in list)
                            {
                                costCount += data.costCount;
                                needClickCount += data.needClickCount;

                                if (UserDataProxy.inst.playerData.level <= WorldParConfigManager.inst.GetConfig(175).parameters)
                                {
                                    ulong goldCost = BuildingCostConfigManager.inst.GetInvestCost(data.oneSelfCostCount, 1, data.config.cost_grade, out ulong gemCost);
                                    if (!showCanInvestTx) showCanInvestTx = UserDataProxy.inst.playerData.gold >= (long)goldCost;
                                }
                            }

                            progress = (float)costCount / needClickCount;
                            canInvestText.gameObject.SetActive(showCanInvestTx);
                        }
                        else
                        {
                            redPointObj.SetActiveFalse();

                            progress = (float)buildingData.costCount / buildingData.needClickCount;

                            if (UserDataProxy.inst.playerData.level <= WorldParConfigManager.inst.GetConfig(175).parameters)
                            {
                                ulong goldCost = BuildingCostConfigManager.inst.GetInvestCost(buildingData.oneSelfCostCount, 1, buildingData.config.cost_grade, out ulong gemCost);
                                canInvestText.gameObject.SetActive(UserDataProxy.inst.playerData.gold >= (long)goldCost);
                            }
                            else
                            {
                                canInvestText.gameObject.SetActiveFalse();
                            }
                        }

                        this.lvProgress.value = Mathf.Max(0.05f, progress);
                    }
                }
                else
                {
                    lockHandler?.Invoke();
                }
            }
            else
            {
                this.lvProgress.value = 0;
            }
        }
    }

    public void setFont()
    {
        this.nameText.font = LanguageManager.inst.curFont;
        this.canInvestText.font = LanguageManager.inst.curFont;
    }
}

