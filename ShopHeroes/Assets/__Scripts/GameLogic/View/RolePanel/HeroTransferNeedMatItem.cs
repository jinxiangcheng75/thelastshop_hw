using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroTransferNeedMatItem : MonoBehaviour
{

    public Button btn_tip;
    public GUIIcon icon_mat;
    public Text tx_count;

    CommonRewardData _rewardData;

    private void Start()
    {
        btn_tip.ButtonClickTween(() =>
        {
            if (_rewardData != null)
            {
                EventController.inst.TriggerEvent(GameEventType.CommonEvent.COMMONTIPS_SETINFO, _rewardData, btn_tip.transform);
            }
        });
    }

    public void SetData(HeroProfessionNeedMatData data)
    {
        var item = ItemBagProxy.inst.GetItem(data.itemId);
        _rewardData = new CommonRewardData(item.itemConfig.id, 1, item.itemConfig.property, item.itemConfig.type);
        icon_mat.SetSprite(item.itemConfig.atlas, item.itemConfig.icon);

        string colorStr = item.count >= data.needItemCount ? "#6bff3e" : "#ff0000";
        tx_count.text = "<Color=" + colorStr + ">" + item.count + "</Color>/" + data.needItemCount;
    }

}
