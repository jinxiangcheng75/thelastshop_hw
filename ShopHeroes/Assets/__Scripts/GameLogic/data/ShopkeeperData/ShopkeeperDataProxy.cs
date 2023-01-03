using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopkeeperDataProxy : TSingletonHotfix<ShopkeeperDataProxy>, IDataModelProx
{
    private Dictionary<int, RoleSubTypeData> _allExteriorData;
    private Dictionary<int, RoleSubTypeData> _allFashionData;

    private List<RoleSubTypeData> _manList;
    private List<RoleSubTypeData> _manFaList;
    private List<RoleSubTypeData> _womanList;
    private List<RoleSubTypeData> _womanFaList;

    public DressUpSystem Man;
    public DressUpSystem Woman;
    public string sensitiveWords;
    public EGender curGender;
    public int buyType; // 0 - 单个 1 - 多个
    public DressUpSystem curRole
    {
        get
        {
            if ((EGender)UserDataProxy.inst.playerData.gender == EGender.Male)
            {
                return Man;
            }
            else
            {
                return Woman;
            }
        }
    }

    private dressconfig[] datas;

    private List<int> _dressList;
    public List<int> dressList
    {
        get { return _dressList; }
        set { _dressList = value; }
    }

    public RoleDress manDress
    {
        get { return Man.curDress; }
    }

    public RoleDress womanDress
    {
        get { return Woman.curDress; }
    }

    public RoleDress manFirst = new RoleDress();
    public RoleDress womanFirst = new RoleDress();

    public string atlasName = "dressup_atlas";


    public void Clear()
    {
        _allExteriorData.Clear();
        _allExteriorData = null;
        _allFashionData.Clear();
        _allFashionData = null;
        clearRole();
    }

    public void Init()
    {
        _allExteriorData = new Dictionary<int, RoleSubTypeData>();
        _allFashionData = new Dictionary<int, RoleSubTypeData>();

        // 获取已购买的时装列表
        NetworkEvent.SetCallback(MsgType.Response_User_DressList_Cmd,
        (successResp) =>
        {
            GetDressListDatas((Response_User_DressList)successResp);
        },
        (failedResp) =>
        {
            //获取装备列表失败
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("获取装备列表失败!"));
        });

        // 购买完装备之后的回调
        NetworkEvent.SetCallback(MsgType.Response_User_BuyDress_Cmd,
        (successResp) =>
        {
            GetBuyDressList((Response_User_BuyDress)successResp);
        },
        (failedResp) =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("获取购买时装列表失败!"));
        });

        // 自定义之后的网络回调
        NetworkEvent.SetCallback(MsgType.Response_User_Custom_Cmd,
        (successResp) =>
        {
            GetUserCustomData((Response_User_Custom)successResp);
        },
        (failedResp) =>
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, LanguageManager.inst.GetValueByKey("获取自定义回调失败!"));
        });

        datas = dressconfigManager.inst.GetAllConfig();

        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].type_1 == 1/* && datas[i].is_show == 1*/)
            {
                _allExteriorData.Add(datas[i].id, new RoleSubTypeData(false, datas[i]));
            }
            if (datas[i].type_1 == 2/* && datas[i].is_show == 1*/)
            {
                _allFashionData.Add(datas[i].id, new RoleSubTypeData(false, datas[i]));
            }
        }

        createRole();
    }

    void clearRole()
    {
        if (Man != null)
        {
            GameObject.Destroy(Man.gameObject);
            Man = null;
        }

        if (Woman != null)
        {
            GameObject.Destroy(Woman.gameObject);
            Woman = null;
        }
    }

    void createRole()
    {
        clearRole();
        if ((EGender)UserDataProxy.inst.playerData.gender == EGender.Male)
        {
            CharacterManager.inst.GetCharacter<DressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath(EGender.Male), SpineUtils.RoleDressToUintList(UserDataProxy.inst.playerData.userDress), EGender.Male, callback: (system) =>
            {
                Man = system;
                Man.gameObject.name = "Man";
                Man.SetActive(false);
                UnityEngine.Object.DontDestroyOnLoad(system.gameObject);
            });

            CharacterManager.inst.GetCharacter<DressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath(EGender.Female), CharacterModelConfigManager.inst.GetConfig(20001).ToDressIds(), EGender.Female, callback: (system) =>
            {
                Woman = system;
                Woman.gameObject.name = "Woman";
                Woman.SetActive(false);
                UnityEngine.Object.DontDestroyOnLoad(system.gameObject);
            });
        }
        else
        {
            CharacterManager.inst.GetCharacter<DressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath(EGender.Male), CharacterModelConfigManager.inst.GetConfig(10001).ToDressIds(), EGender.Male, callback: (system) =>
            {
                Man = system;
                Man.gameObject.name = "Man";
                Man.SetActive(false);
                UnityEngine.Object.DontDestroyOnLoad(system.gameObject);
            });

            CharacterManager.inst.GetCharacter<DressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath(EGender.Female), SpineUtils.RoleDressToUintList(UserDataProxy.inst.playerData.userDress), EGender.Female, callback: (system) =>
            {
                Woman = system;
                Woman.gameObject.name = "Woman";
                Woman.SetActive(false);
                UnityEngine.Object.DontDestroyOnLoad(system.gameObject);
            });
        }
    }

    private void GetBuyDressList(Response_User_BuyDress msg)
    {
        dressList = msg.dressList;
        for (int i = 0; i < dressList.Count; i++)
        {
            if (_allExteriorData.ContainsKey(dressList[i]))
            {
                _allExteriorData[dressList[i]].config.guide = 1;
            }
            if (_allFashionData.ContainsKey(dressList[i]))
            {
                _allFashionData[dressList[i]].config.guide = 1;
            }
        }

        RefreshSexData();
        if (buyType == 0)
        {
            EventController.inst.TriggerEvent(GameEventType.HIDEUI_SINGLEBUY);
            EventController.inst.TriggerEvent(GameEventType.DressUpEvent.BUYSINGLEDRESS);
            //EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.CHANGECLOTHE);
            HotfixBridge.inst.TriggerLuaEvent("CHANGECLOTHE");
        }
    }

    private void GetUserCustomData(Response_User_Custom msg)
    {

        if (msg.errorCode != (int)EErrorCode.EEC_Success)
        {
            return;
        }

        UserDataProxy.inst.playerData.userDress = msg.userDress;
        UserDataProxy.inst.playerData.gender = (uint)msg.gender;
        curGender = (EGender)msg.gender;
        EventController.inst.TriggerEvent(GameEventType.DressUpEvent.USERCUSTOM);
        //EventController.inst.TriggerEvent(GameEventType.ShopkeeperComEvent.CHANGECLOTHE);
        HotfixBridge.inst.TriggerLuaEvent("CHANGECLOTHE");
        InitClotheOnRole();
    }

    public void InitClotheOnRole()
    {
        if (curGender == EGender.Male)
        {
            manFirst = UserDataProxy.inst.playerData.userDress;
            womanFirst = CharacterModelConfigManager.inst.GetConfig(20001).ToRoleDress();
        }
        else
        {
            manFirst = CharacterModelConfigManager.inst.GetConfig(10001).ToRoleDress();
            womanFirst = UserDataProxy.inst.playerData.userDress;
        }


        ChangeListState(SpineUtils.RoleDressToUintList(manFirst), true);
        ChangeListState(SpineUtils.RoleDressToUintList(womanFirst), true);

        if (Man != null) Man.OverallClothing(SpineUtils.RoleDressToUintList(manFirst));
        if (Woman != null) Woman.OverallClothing(SpineUtils.RoleDressToUintList(womanFirst));
    }

    private void GetDressListDatas(Response_User_DressList msg)
    {
        if (msg.errorCode == (int)EErrorCode.EEC_Success)
        {
            dressList = msg.dressList;
            for (int i = 0; i < dressList.Count; i++)
            {
                if (_allExteriorData.ContainsKey(dressList[i]))
                {
                    _allExteriorData[dressList[i]].config.guide = 1;
                }

                if (_allFashionData.ContainsKey(dressList[i]))
                {
                    _allFashionData[dressList[i]].config.guide = 1;
                }
            }

            RefreshSexData();
        }
    }

    public void ChangeState(int id, bool iswear)
    {
        //Logger.error("DressId : " + id + "  isWear : " + iswear);

        if (_allExteriorData.ContainsKey(id))
        {
            _allExteriorData[id].isSelect = iswear;
        }

        if (_allFashionData.ContainsKey(id))
        {
            _allFashionData[id].isSelect = iswear;
        }

        //RefreshSexData();
    }

    public void ChangeListState(List<int> idList, bool iswear)
    {
        for (int i = 0; i < idList.Count; i++)
        {
            ChangeState(idList[i], iswear);
        }

    }

    public void ResetSameTypeState(List<RoleSubTypeData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].isSelect = false;
        }
    }

    private void RefreshSexData()
    {
        List<RoleSubTypeData> _allList = _allExteriorData.Values.ToList();
        _manList = _allList.FindAll(t => ((t.config.is_show == 1 || (t.config.is_show != 1 && t.config.guide == 1)) && (t.config.gender == 0 || t.config.gender == 1)));
        _womanList = _allList.FindAll(t => ((t.config.is_show == 1 || (t.config.is_show != 1 && t.config.guide == 1)) && (t.config.gender == 0 || t.config.gender == 2)));

        _allList = _allFashionData.Values.ToList();
        _manFaList = _allList.FindAll(t => ((t.config.is_show == 1 || (t.config.is_show != 1 && t.config.guide == 1)) && (t.config.gender == 0 || t.config.gender == 1)));
        _womanFaList = _allList.FindAll(t => ((t.config.is_show == 1 || (t.config.is_show != 1 && t.config.guide == 1)) && (t.config.gender == 0 || t.config.gender == 2)));
    }

    private List<int> GetCurSexDressList(EGender curSex)
    {
        RoleDress tempDress = curSex == EGender.Male ? Man.curDress : Woman.curDress;

        return SpineUtils.RoleDressToUintList(tempDress);
    }

    public bool JudgeIsAllFreeOrBuy(EGender curSex)
    {
        List<int> ids = GetCurSexDressList(curSex);

        for (int i = 0; i < ids.Count; i++)
        {
            if (_allExteriorData.ContainsKey(ids[i]))
            {
                if (_allExteriorData[ids[i]].config.guide == 0)
                    return false;
            }

            if (_allFashionData.ContainsKey(ids[i]))
            {
                if (_allFashionData[ids[i]].config.guide == 0)
                    return false;
            }
        }

        return true;
    }

    public List<RoleSubTypeData> ReturnNotBuyIdList(EGender curSex)
    {
        List<int> ids = GetCurSexDressList(curSex);
        List<RoleSubTypeData> resultList = new List<RoleSubTypeData>();

        for (int i = 0; i < ids.Count; i++)
        {
            if (_allExteriorData.ContainsKey(ids[i]))
            {
                if (_allExteriorData[ids[i]].config.guide == 0)
                    resultList.Add(new RoleSubTypeData(_allExteriorData[ids[i]].isSelect, _allExteriorData[ids[i]].config));
            }

            if (_allFashionData.ContainsKey(ids[i]))
            {
                if (_allFashionData[ids[i]].config.guide == 0)
                    resultList.Add(new RoleSubTypeData(_allFashionData[ids[i]].isSelect, _allFashionData[ids[i]].config));
            }
        }

        return resultList;
    }

    public List<RoleSubTypeData> GetCurSexTypeSubDatas(uint bigType, uint typeStr, EGender sexStr)
    {
        List<RoleSubTypeData> resultList = GetSexListData(sexStr, bigType);
        var list = resultList.FindAll(t => t.config.type_2 == typeStr);
        list.Sort(CompareByRule);
        return list;
    }

    public static int CompareByRule(RoleSubTypeData c1, RoleSubTypeData c2)
    {
        if (c1.config.guide != c2.config.guide)
        {
            return c1.config.guide > c2.config.guide ? -1 : 1;
        }
        else
        {
            if (c1.config.guide == 1)
            {
                return c1.config.id < c2.config.id ? -1 : 1;
            }
            else
            {
                if (c1.config.get_type != c2.config.get_type)
                {
                    return c1.config.get_type < c2.config.get_type ? -1 : 1;
                }
                else
                {
                    if (c1.config.get_type == 1)
                    {
                        if (c1.config.price_money == c2.config.price_money)
                        {
                            return c1.config.id < c2.config.id ? -1 : 1;
                        }
                        else
                        {
                            return c1.config.price_money < c2.config.price_money ? -1 : 1;
                        }
                    }
                    else if (c1.config.get_type == 2)
                    {
                        if (c1.config.price_diamond == c2.config.price_diamond)
                        {
                            return c1.config.id < c2.config.id ? -1 : 1;
                        }
                        else
                        {
                            return c1.config.price_diamond < c2.config.price_diamond ? -1 : 1;
                        }
                    }
                    else
                    {
                        return c1.config.id < c2.config.id ? -1 : 1;
                    }
                }
            }
        }
    }

    public List<RoleSubTypeData> GetSexListData(EGender sexType, uint bigType)
    {
        List<RoleSubTypeData> resultList = new List<RoleSubTypeData>();
        switch (sexType)
        {
            case EGender.Male:
                resultList = bigType == 1 ? _manList : _manFaList;
                break;
            case EGender.Female:
                resultList = bigType == 1 ? _womanList : _womanFaList;
                break;
            default:
                break;
        }

        return resultList;
    }

    public bool JudgeIsEquale(RoleDress last, RoleDress cur)
    {
        List<int> lastArr = SpineUtils.RoleDressToUintList(last);
        List<int> curArr = SpineUtils.RoleDressToUintList(cur);
        for (int i = 0; i < lastArr.Count; i++)
        {
            if (lastArr[i] != curArr[i])
            {
                //Logger.error("last = " + lastArr[i] + "        cur = " + curArr[i]);
                return false;
            }
        }
        return true;
    }

}
