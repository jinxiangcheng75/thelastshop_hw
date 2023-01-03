using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//宠物相关
public partial class IndoorRoleSystem
{

    //宠物实例表现
    Dictionary<int, Pet> petRoleDic = new Dictionary<int, Pet>();


    void AddListeners_Pet()
    {
        var e = EventController.inst;



        e.AddListener(GameEventType.PetCompEvent.PETDATA_GETEND, updateStartPets);
        e.AddListener<int>(GameEventType.PetCompEvent.PET_ONDATACHANGE, petDataChangeHandler);
        e.AddListener(GameEventType.PetCompEvent.PET_GOTODOORWAY, petGotoDoorway);
        e.AddListener(GameEventType.PetCompEvent.PET_LEAVEDOORWAY, petLeaveDoorway);


    }

    void RemoveListeners_Pet()
    {
        var e = EventController.inst;

        //-------------------------------------------------------------------------------------------------------------------
        e.RemoveListener(GameEventType.PetCompEvent.PETDATA_GETEND, updateStartPets);
        e.RemoveListener<int>(GameEventType.PetCompEvent.PET_ONDATACHANGE, petDataChangeHandler);
        e.RemoveListener(GameEventType.PetCompEvent.PET_GOTODOORWAY, petGotoDoorway);
        e.RemoveListener(GameEventType.PetCompEvent.PET_LEAVEDOORWAY, petLeaveDoorway);



    }

    void updateStartPets()
    {

        if (ManagerBinder.inst.mGameState != kGameState.Shop) return;  //不在店铺

        FGUI.inst.StartCoroutine(initPets());
    }

    IEnumerator initPets() 
    {
        var pets = PetDataProxy.inst.GetAllPetDatas();

        for (int i = 0; i < pets.Count; i++)
        {
            var petData = pets[i];

            if (petData.petInfo.petState != (int)EPetState.Idle) //不是在店铺的状态
            {
                continue;
            }

            var pet = IndoorMap.inst.CreatePet(petData);

            if (petRoleDic.ContainsKey(petData.petUid))
            {
                if (petRoleDic[petData.petUid] != null)
                {
                    petRoleDic[petData.petUid].DestroySelf();
                    petRoleDic.Remove(petData.petUid);
                }
            }

            petRoleDic.Add(petData.petUid, pet);

            for (int k = 0; k < 3; k++)
            {
                yield return null;
            }

        }
    }

    void petDataChangeHandler(int petUid)
    {
        var petData = PetDataProxy.inst.GetPetDataByPetUid(petUid);


        if (petData.petInfo.petState == (int)EPetState.Idle) //判断场景中有没有 
        {
            if (!petRoleDic.ContainsKey(petUid)) //没有生成一个
            {
                var pet = IndoorMap.inst.CreatePet(PetDataProxy.inst.GetPetDataByPetUid(petUid));
                petRoleDic.Add(petUid, pet);
            }
            else //有则刷新外观
            {
                petRoleDic[petUid].SetData(petData);
            }
        }
        else if (petData.petInfo.petState == (int)EPetState.Store) //判断场景中有没有 
        {
            if (petRoleDic.ContainsKey(petUid)) // 有删除
            {
                if (petRoleDic[petData.petUid] != null)
                {
                    AudioManager.inst.PlaySound(14);
                    petRoleDic[petData.petUid].DestroySelf();
                }
                petRoleDic.Remove(petData.petUid);
            }
        }

    }

    public Pet GetPetByUid(int uid)
    {
        if (petRoleDic.ContainsKey(uid))
        {
            return petRoleDic[uid];
        }

        return null;
    }

    public List<Pet> GetAllPetList()
    {
        return petRoleDic.Values.ToList();
    }


    void petGotoDoorway()
    {
        var list = GetAllPetList().FindAll(t => t.GetCurState() == MachinePetState.ramble);

        if (list.Count > 0)
        {
            var pet = list.GetRandomElement();
            list.GetRandomElement().SetState(MachinePetState.stayDoorway);
            pet.StayBySeeCount = 0;
        }

    }

    void petLeaveDoorway()
    {
        foreach (var item in GetAllPetList())
        {
            if (item.GetCurState() == MachinePetState.stayDoorway)
            {
                item.SetState(MachinePetState.ramble);
            }
        }

    }

}
