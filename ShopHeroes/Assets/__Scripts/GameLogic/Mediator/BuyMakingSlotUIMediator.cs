using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyMakingSlotUIMediator : BaseSystem
{
    private BuyMakingSlotView buyMakingSlotView;
    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.SHOWUI_BuyMakingSlot, openUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_BuyMakingSlot, closeUI);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_BuyMakingSlot, openUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_BuyMakingSlot, closeUI);
    }


    private void openUI()
    {
        //if (buyMakingSlotView == null)
        //{
        //    buyMakingSlotView = new BuyMakingSlotView();
        //}
        //var cfg = FieldConfigManager.inst.GetFieldConfig(3, EquipDataProxy.inst.mskeSlotCount + 1);
        //if (cfg != null)
        //{
        GUIManager.OpenView<BuyMakingSlotView>((view) =>
        {
            buyMakingSlotView = view;
        });
        //}
        //buyMakingSlotView.show();
    }
    private void closeUI()
    {
        GUIManager.HideView<BuyMakingSlotView>();
        //if (buyMakingSlotView != null && buyMakingSlotView.isShowing)
        //{
        //    buyMakingSlotView.hide();
        //}
    }
}
