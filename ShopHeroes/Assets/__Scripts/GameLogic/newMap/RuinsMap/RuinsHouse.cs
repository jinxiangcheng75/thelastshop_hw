using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuinsHouse : InputEventListener
{
    public int houseId;
    public LuaListItem hud;
    public SpriteRenderer sprite;

    private void Awake()
    {
        if (this == null) return;

        sprite = gameObject.GetComponent<SpriteRenderer>();
    }


    public override bool TOnClick(Vector3 mousepos)
    {
        base.TOnClick(mousepos);
        AudioManager.inst.PlaySound(143);
        HotfixBridge.inst.TriggerLuaEvent("ClickEvent_House", houseId);
        return true;
    }

    public void CreatRuinsHUD(int curHouseId)
    {
        if (null == this) return;

        if (sprite != null)
        {
            if (houseId <= curHouseId)
            {
                sprite.enabled = true;
                if (houseId == curHouseId)
                {
                    Vector3 tempPos = transform.position;
                    if (RuinsMap.inst != null)
                    {
                        tempPos = RuinsMap.inst.GetClampPos(tempPos);
                    }

                    Camera.main.transform.position = tempPos;
                    sprite.color = GUIHelper.GetColorByColorHex("#ffffff");
                }
                else
                {
                    sprite.color = GUIHelper.GetColorByColorHex("#313131");
                }
            }
            else
            {
                sprite.enabled = false;
            }
        }

        if (hud == null)
        {
            hud = FGUI.inst.CreatRuinsHouseHUD(this.transform);
        }
        if (hud != null)
        {
            RuinsMap.inst.setHouseDic(houseId, hud);
            hud.SetData(houseId);
        }
    }

    public void RefreshRuinsHUD(int curHouseId)
    {

    }
}
