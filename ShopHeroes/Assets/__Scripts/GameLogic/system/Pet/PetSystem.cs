using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSystem : BaseSystem
{
    protected override void AddListeners()
    {

        var e = EventController.inst;


        e.AddListener<bool, bool, int>(GameEventType.PetCompEvent.PET_TURNPAGE, petTurnPage);
        //-------------------------------------------------------------------------------------------------------------------
        e.AddListener(GameEventType.PetCompEvent.REQUEST_PET_INFO, request_getPetInfo);
        e.AddListener<int>(GameEventType.PetCompEvent.REQUEST_PET_UPDATEINFO, request_updatePetInfo);

    }

    protected override void RemoveListeners()
    {

        var e = EventController.inst;


        e.RemoveListener<bool, bool, int>(GameEventType.PetCompEvent.PET_TURNPAGE, petTurnPage);
        //-------------------------------------------------------------------------------------------------------------------
        e.RemoveListener(GameEventType.PetCompEvent.REQUEST_PET_INFO, request_getPetInfo);
        e.RemoveListener<int>(GameEventType.PetCompEvent.REQUEST_PET_UPDATEINFO, request_updatePetInfo);

    }

    void petTurnPage(bool containsHide, bool isLeft, int petUid)
    {
        var list = PetDataProxy.inst.GetHasPetDatas();
        if (!containsHide) list = list.FindAll(t => t.petInfo.petState != (int)EPetState.Store);


        int index = list.IndexOf(PetDataProxy.inst.GetPetDataByPetUid(petUid));

        index = isLeft ? index - 1 : index + 1;

        if (index == -1)
        {
            index = list.Count - 1;
        }
        else if (index == list.Count)
        {
            index = 0;
        }


        var pageData = list[index];

        if (containsHide)
        {
            //详情界面
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_PetDetailUI", pageData);
        }
        else
        {
            //观看宠物小家
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.LOOKPETHOUSE, UserDataProxy.inst.GetFuriture(pageData.petInfo.furnitureUid));
        }

    }


    void request_getPetInfo()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_PetInfo()
            {

            }
        });
    }

    void request_updatePetInfo(int petUid)
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_PetInfoUpdate()
            {
                petUid = petUid,
            }
        });
    }

}
