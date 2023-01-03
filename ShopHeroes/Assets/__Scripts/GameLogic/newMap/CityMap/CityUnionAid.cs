using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityUnionAid : InputEventListener
{
    private UnionAidHUD hud;


    private void Awake()
    {
        EventController.inst.AddListener(GameEventType.UnionEvent.UNION_MEMBERHELPLISTREFRESH, updateHudInfo);
    }


    public override bool TOnClick(Vector3 mousepos)
    {
        if (hud != null && hud.gameObject.activeSelf)
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_UnionAidUIView");
        }

        return true;
    }

    void Start()
    {
        updateHudInfo();
    }

    void updateHudInfo()
    {
        if (null == this) return;

        if (hud == null)
            hud = FGUI.inst.CreateUnionAidHUD(this.transform);
        if (hud != null)
        {
            hud.CanShowUnionAid();
            hud.SetFont();
        }
    }




    private void OnDestroy()
    {
        EventController.inst.RemoveListener(GameEventType.UnionEvent.UNION_MEMBERHELPLISTREFRESH, updateHudInfo);
    }


}
