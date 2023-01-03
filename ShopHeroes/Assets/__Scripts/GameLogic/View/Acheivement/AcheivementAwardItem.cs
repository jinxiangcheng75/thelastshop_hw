using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcheivementAwardItem : MonoBehaviour
{
    public Button selfBtn;
    public GUIIcon icon;
    public Text numText;
    public Button canAwardBtn;
    public GameObject gouObj;

    AcheivementRoadConfigData data;
    K_Acheivement_AwardState roadState = K_Acheivement_AwardState.None;
    private void Awake()
    {
        selfBtn.ButtonClickTween(onButtonClick);
        canAwardBtn.ButtonClickTween(onButtonClick);
    }

    public void setData(AcheivementRoadConfigData cfgData)
    {
        data = cfgData;
        numText.text = data.need_point.ToString();
    }

    public void refreshData()
    {
        var roadData = AcheivementDataProxy.inst.GetAcheivementRoadDataById(data.id);
        if (roadData == null) return;
        roadState = (K_Acheivement_AwardState)((int)roadData.state);
        selfBtn.gameObject.SetActive(roadState != K_Acheivement_AwardState.CanReward);
        canAwardBtn.gameObject.SetActive(roadState == K_Acheivement_AwardState.CanReward);
        gouObj.SetActive(roadState == K_Acheivement_AwardState.Rewarded);
        selfBtn.image.color = roadState == K_Acheivement_AwardState.Rewarded ? GUIHelper.GetColorByColorHex("#999999") : Color.white;
    }

    public void clearData()
    {
        data = null;
    }

    private void onButtonClick()
    {
        switch (roadState)
        {
            case K_Acheivement_AwardState.CanReward:
                EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTROADAWARD, data.id);
                break;
            default:
                List<CommonRewardData> allList = new List<CommonRewardData>();
                if (data.reward1_type != -1)
                {
                    CommonRewardData tempData = new CommonRewardData(data.reward1_type, data.reward1_num, 1, data.item1_type);
                    allList.Add(tempData);
                }
                if (data.reward2_type != -1)
                {
                    CommonRewardData tempData = new CommonRewardData(data.reward2_type, data.reward2_num, 1,data.item2_type);
                    allList.Add(tempData);
                }
                EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONMORETIPS_SETINFO, allList, selfBtn.transform);
                break;
        }
    }
}
