using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAdvancedOrLockEquipTipsData 
{
    public int type;
    public int equipId;
    public string title;
    public string content;
}

public class UseAdvancedEquipTipsMediator : BaseSystem
{
    protected override void AddListeners()
    {
        EventController.inst.AddListener<System.Action, List<int>,string>(GameEventType.UseAdvancedEquip, onUseAdvancedEquip);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<System.Action, List<int>, string>(GameEventType.UseAdvancedEquip, onUseAdvancedEquip);
    }

    List<int> currEquipList = new List<int>();
    int agreeCount = 0;
    System.Action okCallBack;
    private void onUseAdvancedEquip(System.Action callback, List<int> equipList, string itemIntroduceTx)
    {
        currEquipList = equipList;
        agreeCount = equipList.Count;
        okCallBack = callback;
        showView(currEquipList[0], itemIntroduceTx);
    }

    private void onUIBack(bool value,string itemIntroduceTx)
    {
        if (value)
        {
            agreeCount--;
            if (agreeCount <= 0)
            {
                //同意消耗
                if (okCallBack != null)
                {
                    okCallBack.Invoke();
                }
                okCallBack = null;
                hideView();
            }
            else
            {
                showView(currEquipList[currEquipList.Count - agreeCount], itemIntroduceTx);
            }
        }
        else
        {
            hideView();
            //取消了
        }
    }

    private void showView(int equipid,string itemIntroduceTx)
    {
        GUIManager.OpenView<UseAdvancedEquipView>((view) =>
        {
            view.backEvent = onUIBack;
            view.setCurrEquipInfo(equipid, itemIntroduceTx);
        });
    }

    private void hideView()
    {
        GUIManager.HideView<UseAdvancedEquipView>();
        currEquipList = null;
        agreeCount = 0;

    }
}
