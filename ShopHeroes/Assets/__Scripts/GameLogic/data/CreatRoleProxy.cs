using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoleTypeData
{
    public string atlasName;
    public string iconName;
    public string highLightName;
    public uint typeStr;

    public RoleTypeData() { }
    public RoleTypeData(string atlasName, string iconName, string highLightName, uint typeStr)
    {
        this.atlasName = atlasName;
        this.iconName = iconName;
        this.highLightName = highLightName;
        this.typeStr = typeStr;
    }
}

public class RoleSubTypeData
{
    public dressconfig config;
    public bool isSelect;

    public RoleSubTypeData() { }
    public RoleSubTypeData(bool isSelect)
    {
        this.isSelect = isSelect;
    }
    public RoleSubTypeData(bool isSelect, dressconfig config)
    {
        this.isSelect = isSelect;
        this.config = config;
    }
}

public class CreatRoleProxy : TSingletonHotfix<CreatRoleProxy>, IDataModelProx
{
    private List<RoleSubTypeData> _manList;
    private List<RoleSubTypeData> _womanList;

    public GameObject _lastItem;

    #region 固定字符串数组
    public string atlasName = "dressup_atlas";

    public string sensitiveWords;
    #endregion


    public void Init()
    {
        sensitiveWords = "[`~!#$^&*()=|{}':;',\\[\\].<>/?~！#￥……&*（）——|{}【】‘；：”“'。，、？]_-+\"";

        AwakeInitDatas();
    }

    public void InitSelectData(EGender sexType)
    {
        if (sexType == EGender.Male)
        {
            for (int i = 0; i < _manList.Count; i++)
            {
                _manList[i].isSelect = false;
            }
        }
        else
        {
            for (int i = 0; i < _womanList.Count; i++)
            {
                _womanList[i].isSelect = false;
            }
        }
    }

    private void AwakeInitDatas()
    {

        List<RoleSubTypeData> _allList = new List<RoleSubTypeData>();

        dressconfig[] datas = dressconfigManager.inst.GetAllConfig();

        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i].type_1 == 1 && datas[i].is_show == 1)
            {
                _allList.Add(new RoleSubTypeData(false, datas[i]));
            }
        }

        _manList = new List<RoleSubTypeData>(_allList.FindAll(t => (t.config.gender == 0 || t.config.gender == 1) /*&& t.config.guide == 1*/));
        _womanList = new List<RoleSubTypeData>(_allList.FindAll(t => (t.config.gender == 0 || t.config.gender == 2) /*&& t.config.guide == 1*/));

        _allList.Clear();
    }


    public void ChangeDataList(EGender gender, int id, bool iswear)
    {
        if (gender == EGender.Male)
        {
            if (_manList.Find(t => t.config.id == id) != null)
                _manList.Find(t => t.config.id == id).isSelect = iswear;
        }
        else
        {
            if (_womanList.Find(t => t.config.id == id) != null)
                _womanList.Find(t => t.config.id == id).isSelect = iswear;
        }
    }

    public List<RoleSubTypeData> GetSexListData(EGender sexType)
    {
        List<RoleSubTypeData> resultList = new List<RoleSubTypeData>();
        switch (sexType)
        {
            case EGender.Male:
                resultList = _manList;
                break;
            case EGender.Female:
                resultList = _womanList;
                break;
        }

        return resultList;
    }

    public List<RoleSubTypeData> GetCurSexTypeSubDatas(FacadeType typeStr, EGender sexStr)
    {
        List<RoleSubTypeData> resultList = GetSexListData(sexStr);
        return resultList.FindAll(t => t.config.type_2 == (uint)typeStr);
    }

    public RoleSubTypeData RandomGetTypeData(FacadeType typeStr, EGender sexStr)
    {
        List<RoleSubTypeData> resultList = GetSexListData(sexStr);
        List<RoleSubTypeData> tempList = resultList.FindAll(t => t.config.type_2 == (uint)typeStr);
        return tempList[Random.Range(0, tempList.Count)];
    }

    public void Clear()
    {
        Release();
    }
}
