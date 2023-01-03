using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingSite : MonoBehaviour
{
    public Transform sand_1;
    public Transform sand_2;
    public Transform sand_3;
    public Transform accessories;
    private InputEventListener mouseListener;

    public Transform tipTF;

    bool canClick = false;
    void Start()
    {
        mouseListener = gameObject.GetComponentInChildren<InputEventListener>();
        if (mouseListener != null)
        {
            mouseListener.OnClick = (mousepos) =>
            {
                onClick();
            };
        }

        canClick = UserDataProxy.inst.shopData.shopLevel < StaticConstants.shopMap_MaxLevel && WorldParConfigManager.inst.GetConfig(117).parameters <= UserDataProxy.inst.playerData.level;
        EventController.inst.AddListener(GameEventType.UIUnlock.SHOP_ONLVUP, onShopLevelUp);
        // setSize(UserDataProxy.inst.shopData.size.xMax);

    }

    void onShopLevelUp()
    {
        if (UserDataProxy.inst.shopData.shopLevel >= StaticConstants.shopMap_MaxLevel)
        {
            this.gameObject.SetActive(false);
        }
        if (tipTF != null && tipTF.gameObject != null)
        {
            canClick = UserDataProxy.inst.shopData.shopLevel < StaticConstants.shopMap_MaxLevel && WorldParConfigManager.inst.GetConfig(117).parameters <= UserDataProxy.inst.playerData.level;
            EDesignState state = (EDesignState)UserDataProxy.inst.shopData.currentState;
            tipTF.gameObject.SetActive(state == EDesignState.Idle && canClick);
        }
    }
    protected bool onmouseDown = false;
    protected bool Draging = false;
    protected Vector3 lastMousePos;
    public void MouseUp(Vector3 mousepos)
    {
        if (onmouseDown && !Draging)
        {
            //点击
            onClick();
        }
        onmouseDown = false;
        Draging = false;
    }

    public void MouseDown(Vector3 mousepos)
    {
#if UNITY_EDITOR
        if (Input.touchCount <= 0) return;
#endif
        if (GUIHelper.isPointerOnUI()) return;
        onmouseDown = true;
        Draging = false;
        lastMousePos = mousepos;
    }
    public void Drag(Vector3 mousepos)
    {
        if (onmouseDown == false) return;
        var dis = Vector3.Distance(mousepos, lastMousePos);
        if (dis > 10)
        {
            Draging = true;
        }
    }

    void onClick()
    {
        if (!canClick) return;
        EDesignState state = (EDesignState)UserDataProxy.inst.shopData.currentState;
        switch (state)
        {
            case EDesignState.Idle:
                if (UserDataProxy.inst.shopData.shopLevel == StaticConstants.shopMap_MaxLevel)
                {
                    //已经到达最高级别
                    canClick = false;
                    break;
                }
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENSIONPANEL);
                break;
                // case EDesignState.Upgrading:
                //     EventController.inst.TriggerEvent(GameEventType.SHOWUI_EXTENDINGPANEL);
                //     break;
                // case EDesignState.Finished:
                //     IndoorMapEditSys.inst.shopUpgradeFinish();
                //     break;
        }
    }
    List<Transform> diList = new List<Transform>();
    public void setSize(int Maxx)
    {
        int offset_x = ((Maxx + 3) - 8) / 2;
        for (int i = 4; i < Maxx; i += 2)
        {
            if (diList.Find(tf => tf.name == "d_" + i.ToString()) == null)
            {
                var newTF = Instantiate(sand_2, sand_2.parent);
                Vector3 pos = MapUtils.CellPosToWorldPos(new Vector3Int(i, 0, 0));
                newTF.localPosition = pos;
                newTF.name = "d_" + i;
                diList.Add(newTF);
            }
        }
        sand_3.localPosition = MapUtils.CellPosToWorldPos(new Vector3Int(Maxx, 0, 0));
        //上层
        accessories.localPosition = MapUtils.CellPosToWorldPos(new Vector3Int(offset_x, 0, 0));
        EDesignState state = (EDesignState)UserDataProxy.inst.shopData.currentState;

        tipTF.gameObject.SetActive(state == EDesignState.Idle && canClick);
    }
}
