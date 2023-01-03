using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//处理装备 统计相关、 制造数据 
public class EquipDataProxy : TSingletonHotfix<EquipDataProxy>, IDataModelProx
{
    //当前可激活和制造的装备数据 （制造列表使用）
    //private Dictionary<int, EquipData> EquipMakerDataList = new Dictionary<int, EquipData>();
    private List<EquipData> EquipDataList = new List<EquipData>();
    private List<int> NotUnlockEquipList = new List<int>();

    //新解锁装备列表
    private List<int> NewEquip = new List<int>();
    public int equipSubTypeCount = 0;
    public bool lackResIsShowing = false;
    public void Clear()
    {
        EquipDataList.Clear();
        NotUnlockEquipList.Clear();
        NewEquip.Clear();
        ClearMakeSlots();
    }

    public void ClearMakeSlots() 
    {
        foreach (var slot in _makeSlots)
        {
            slot.CancelWork();
        }
        _makeSlots.Clear();
    }

    public void SetNewEquipDrawing(int drawingid)
    {
        NewEquip.Add(drawingid);
    }
    public bool hasNewEquipDrawing()
    {
        return NewEquip.Count > 0;
    }

    public List<int> GetAllNewDrawings()
    {
        return NewEquip;
    }

    public void ClearNewEquip()
    {
        NewEquip.Clear();
    }
    public bool RemoveNewDrawing(int drawingid)
    {
        if (NewEquip.Count <= 0 || NewEquip.IndexOf(drawingid) == -1) return false;
        //NewEquip.Remove(drawingid);
        return true;
    }
    //制造队列
    private List<EquipMakerSlot> _makeSlots = new List<EquipMakerSlot>();

    public int slotId;
    public string toequipUid;
    public int toStoreBasket;
    public bool needWait;
    public SpriteRenderer spriteRender;
    public bool onOrOff;
    public string equipUid;
    public int isFromSlotOrBox;
    public EquipData GetEquipData(int _equipDrawingId)
    {
        return EquipDataList.Find(item => item.equipDrawingId == _equipDrawingId);
    }
    public List<EquipMakerSlot> equipSlotList
    {
        get
        {
            return _makeSlots;
        }
        private set { _makeSlots = value; }
    }

    public EquipMakerSlot GetMakeSlot(int slotid)
    {
        return equipSlotList.Find(i => i.slotId == slotid);
    }

    public int GetIdleEquipMakeSlot(int ignoreid = -1)
    {
        int id = -1;
        foreach (var item in equipSlotList)
        {
            if (id == -1 && item.makeState == 0)
            {
                if (ignoreid != item.slotId)
                {
                    id = item.slotId;
                    return id;
                }
            }
        }
        return id;
    }

    public void MaskeSlotsSort()
    {
        _makeSlots.Sort((a, b) =>
        {
            if (a.makeState > b.makeState)
            {
                return -1;
            }
            else if (a.makeState < b.makeState)
            {
                return 1;
            }
            else
            {
                if (a.makeState == 1)
                {
                    return a.endTime.CompareTo(b.endTime);
                }
                else
                {
                    return -1;
                }
            }
        });
    }

    public List<int> GetAllWorkEquipMakeSlot()
    {
        List<int> result = new List<int>();

        //MaskeSlotsSort();

        _makeSlots.ForEach(item =>
        {
            if (item.makeState != 0)
            {
                result.Add(item.slotId);
            }
        });
        return result;
    }

    public int GetIdleEquipMakeSlotNum()
    {
        int result = 0;
        _makeSlots.ForEach(item =>
        {
            if (item.makeState == 0) result++;
        });

        return result;
    }

    public EquipMakerSlot AddMakeSlot(int slotid)
    {
        var makerslot = new EquipMakerSlot(slotid);
        _makeSlots.Add(makerslot);
        return makerslot;
    }

    public int mskeSlotCount
    {
        get { return _makeSlots.Count; }
    }
    public EquipMakerSlot GetMakeSlotByIndex(int index)
    {
        return index >= mskeSlotCount ? null : _makeSlots[index];
    }

    public EquipMakerSlot GetFreeMakeSlot()
    {
        foreach (var slot in equipSlotList)
        {
            if (slot.makeState == 0)
            {
                return slot;
            }
        }
        return null;
    }

    //public EquipMakerSlot GetMakerSlot
    public void Init()
    {
    }



    public void updateEquipInfo(EquipInfo info, bool isnew = false)
    {
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(info.equipDrawingId);
        if (cfg == null) return;
        if (EquipDataList.FindAll(item => item.subType == cfg.sub_type).Count <= 0)
        {
            equipSubTypeCount++;
        }

        // EquipInfoList[info.equipId] = info;
        EquipData _equipData = GetEquipData(info.equipDrawingId);

        if (_equipData == null)
        {
            _equipData = new EquipData();
            EquipDataList.Add(_equipData);
        }
        if (isnew)
        {
            _equipData.isNew = isnew;
        }

        UpdateMackAddition(ref _equipData, info.equipDrawingId, info);
        // //总列表按制作时间排序 降序
        // EquipDataList.Sort((x, y) => -x.lastMakeTime.CompareTo(y.lastMakeTime));
    }

    public List<EquipData> GetEquipDatas(int type = 0)
    {
        switch (type)
        {
            case 0:
                return EquipDataList;
            case 1:
                {
                    return EquipDataList.FindAll(d => d.mainType == 1);
                }
            case 2:
                {
                    return EquipDataList.FindAll(d => d.mainType == 2);
                }
            case 3:
                {
                    return EquipDataList.FindAll(d => d.mainType == 3 || d.mainType == 4);
                }
        }
        return null;
    }

    public List<EquipData> GetEquipDatasBySubTypes(List<int> subTypes, int eqiupLevel)
    {
        return EquipDataList.FindAll(t =>
        {
            if (t.level >= eqiupLevel)
            {
                return subTypes.Contains(t.subType);
            }
            else
            {
                return false;
            }

        });
    }

    public List<EquipData> GetFavoriteEquipDatas()
    {
        return EquipDataList.FindAll(item => item.favorite == 1);
    }

    public List<EquipData> GetActivity_WorkerGameAddRateEquipDatas() 
    {
        return EquipDataList.FindAll(item => HotfixBridge.inst.GetActivity_WorkerGameEquipCanAddRateByDrawingId(item.equipDrawingId));
    }

    public List<EquipData> GetCompareEquipDatas()
    {
        var data = EquipDataProxy.inst.GetEquipDatas();
        data.Sort((x, y) => -x.lastMakeTime.CompareTo(y.lastMakeTime));
        return data;
    }

    /// <summary>
    /// 获得未解锁并且待解锁的装备列表
    /// </summary>
    /// <param name="type">装备小类型</param>
    /// <returns></returns>
    public List<int> GetNotUnLockEquips(int type = 0)
    {
        List<int> list = NotUnlockEquipList.FindAll(e => compare(e, type));
        if (list.Count <= 0)
        {
            List<EquipDrawingsConfig> erawingcfgs = EquipConfigManager.inst.GetEquipDrawingsConfigs(1, type);
            foreach (var cfg in erawingcfgs)
            {
                EquipData _data = GetEquipData(cfg.id);
                if (_data == null)
                {
                    list.Add(cfg.id);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// 通过工匠ID获取其具体列表
    /// </summary>
    /// <param name="workerId"></param>
    /// <returns></returns>
    public List<EquipData> GetEquipsByWorkerId(int workerId)
    {
        int workerLevel = RoleDataProxy.inst.GetWorker(workerId).level;

        var list = EquipDataList.FindAll(t =>
        {
            EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(t.equipDrawingId);
            if (cfg.artisan_id == null || cfg.artisan_id.Length == 0) return false;
            int index = Array.IndexOf(cfg.artisan_id, workerId);

            if (t.equipState != 2) return false;

            if (index != -1) return cfg.artisan_lv[index] <= workerLevel;

            return false;
        });
        list.Sort((a, b) => -a.level.CompareTo(b.level));
        return list;
    }

    private bool compare(int equipDrawingId, int subtype)
    {
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipDrawingId);
        if (cfg.sub_type == subtype)
        {
            return true;
        }
        return false;
    }
    #region equipdata

    private void UpdateMackAddition(ref EquipData eData, int _equipDrawingId, EquipInfo _info)
    {
        //基本值

        eData.equipDrawingId = _equipDrawingId;
        eData.sellAddition = 1;
        eData.equipState = _info.equipState;        //0-未解锁 1-解锁未激活 2-已激活
        eData.beenMake = _info.beenMake;            //已制造 个数
        eData.progressLevel = _info.progressLevel;  //0-5  当前第几档
        eData.starLevel = _info.starLevel;          //升星阶数
        eData.lastMakeTime = _info.lastMakeTime;    //最近制作时间
        eData.unlockTime = _info.unlockTime;        //解锁时间
        eData.activateTime = _info.activateTime;    //激活时间
        eData.favorite = _info.isFavorite;
        eData.makeTime = _info.makeNeedTime;

        if (eData.equipState != 0)
        {
            if (NotUnlockEquipList.IndexOf(_equipDrawingId) >= 0)
            {
                NotUnlockEquipList.Remove(_equipDrawingId);  //删除已经解锁的
            }
        }


        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfg(eData.equipDrawingId);
        if (cfg == null) return;
        eData.level = cfg.level;
        //eData.makeTime = cfg.production_time;
        eData.mainType = cfg.type;
        eData.subType = cfg.sub_type;
        eData.sellAddition = 1;

        var subcfg = EquipConfigManager.inst.GetEquipQualityConfig(eData.equipDrawingId, 1);
        eData.sellPrice = subcfg.price_gold;
        eData.buyPrice = subcfg.auction_price;
        //if (eData.needRes == null)
        // {
        eData.needRes = cfg.GetNeedMaterialsInfos();
        //}

        eData.specialRes_1 = new needMaterialsInfo();
        eData.specialRes_1.type = cfg.component1_type;
        eData.specialRes_1.needId = cfg.component1_id;
        eData.specialRes_1.needCount = cfg.component1_num;

        eData.specialRes_2 = new needMaterialsInfo();
        eData.specialRes_2.type = cfg.component2_type;
        eData.specialRes_2.needId = cfg.component2_id;
        eData.specialRes_2.needCount = cfg.component2_num;
        eData.barMaxValue = cfg.progress1_exp;
        eData.currBarValue = (int)_info.beenMake;

        eData.progresInfoList = cfg.GetProgressItemInfos();


        if (_info.progressLevel == 0)
        {
            eData.barMaxValue = eData.progresInfoList[0].exp;
            eData.currBarValue = (int)_info.beenMake;
        }
        else if (_info.progressLevel < 5)
        {
            eData.barMaxValue = eData.progresInfoList[_info.progressLevel].exp - eData.progresInfoList[_info.progressLevel - 1].exp;
            eData.currBarValue = _info.beenMake - eData.progresInfoList[_info.progressLevel - 1].exp;
        }

        for (int i = 0; i < eData.progresInfoList.Length; i++)
        {
            if (_info.progressLevel > i)
            {
                CheckAddition(ref eData, eData.progresInfoList[i].type, eData.progresInfoList[i].reward_id, eData.progresInfoList[i].reward_value);
            }
            else
            {
                if (eData.progresInfoList[i].type == 7)
                {
                    if (NotUnlockEquipList.IndexOf(eData.progresInfoList[i].reward_id) == -1)
                    {
                        EquipDrawingsConfig _edcfg = EquipConfigManager.inst.GetEquipDrawingsCfg(eData.progresInfoList[i].reward_id);

                        if (_edcfg == null || _edcfg.unlock_val_01 != eData.equipDrawingId)
                        {
                            Logger.error(_edcfg?.name + $"所需解锁装备{_edcfg?.unlock_val_01}与装备{eData.equipDrawingId}所能解锁装备id不匹配");
                        }
                        else
                        {
                            NotUnlockEquipList.Add(eData.progresInfoList[i].reward_id);
                        }
                    }
                }
            }
        }
    }

    private void CheckAddition(ref EquipData eData, int type, int id, float value)
    {
        switch (type)
        {
            case 1:     //耗材减少
            case 2:     //减少消耗(特殊材料)
            case 3:     //减少消耗(装备)
                {
                    for (int i = 0; i < eData.needRes.Length; i++)
                    {
                        if (eData.needRes[i].needId == id)
                        {
                            eData.needRes[i].needCount -= (int)value;
                        }
                    }
                    if (eData.specialRes_1.needId == id)
                    {
                        eData.specialRes_1.needCount -= (int)value;
                    }
                    if (eData.specialRes_2.needId == id)
                    {
                        eData.specialRes_2.needCount -= (int)value;
                    }
                }
                break;
            case 4:     //时间减少
                //eData.makeTime = (int)(eData.makeTime * ((100f - value) * 0.01f));
                break;
            case 5:     //售价提高	
                eData.sellAddition += (value - 100) * 0.01f;
                break;
            case 6:     //技能
                //????????????????????
                break;
            case 7:     //解锁新图纸
                        //?????????????????????

                break;
            case 8:     //品质提升
                        //  eData.equipSubID = id;
                        //EquipSubConfig scfg = EquipConfigManager.inst.GetEquipSubCfgBySubId(id);
                        //eData.quality = scfg.quality;
                        //eData.sellPrice = scfg.price_gold;
                break;
        }
    }
    #endregion

}
