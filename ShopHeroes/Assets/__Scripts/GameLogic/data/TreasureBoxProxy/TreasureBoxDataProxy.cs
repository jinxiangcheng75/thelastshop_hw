using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class TreasureBoxData : IComparable<TreasureBoxData>
{
    public int boxId; // 宝箱组id
    public int boxItemId;
    public double count; // 宝箱数量
    public int keyId;
    public double keyCount; // 钥匙数量
    public int costGem; // 开启消耗钻石数量
    public Dictionary<int, ExclusiveItemData> exclusiveDic = new Dictionary<int, ExclusiveItemData>();
    public List<ExclusiveItemData> items
    {
        get
        {
            return exclusiveDic.Values.ToList();
        }
    }

    public void setData(TreasureBox _data)
    {
        boxId = _data.boxGroupId;
        boxItemId = _data.boxItemId;
        count = _data.boxCount;
        keyId = _data.keyId;
        keyCount = _data.keyCount;
        costGem = _data.costGem;

        for (int i = 0; i < _data.exclusiveData.Count; i++)
        {
            int index = i;
            if (exclusiveDic.ContainsKey(_data.exclusiveData[index].itemId))
            {
                ExclusiveItemData value;
                if (exclusiveDic.TryGetValue(_data.exclusiveData[index].itemId, out value))
                {
                    value.setData(_data.exclusiveData[index]);
                };
            }
            else
            {
                ExclusiveItemData data = new ExclusiveItemData();
                data.setData(_data.exclusiveData[index]);
                exclusiveDic.Add(data.itemId, data);
            }
        }
    }

    public void setData(Item boxItem, List<ExclusiveItemData> list = null)
    {
        var cfg = TreasureBoxConfigManager.inst.GetConfigByBoxItemId(boxItem.ID);
        boxId = cfg.box_group;
        boxItemId = boxItem.ID;
        count = (int)boxItem.count;
        keyId = cfg.key_item_id;
        keyCount = (int)ItemBagProxy.inst.GetItem(keyId).count;
        costGem = cfg.cost_num;

        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int index = i;
                items.Add(list[index]);
            }
        }
    }

    public int GetUnlockItemCount()
    {
        return items.FindAll(t => t.isUnlock == 1).Count;
    }

    public int CompareTo(TreasureBoxData other)
    {
        if (this.keyCount < other.keyCount)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}

public class ExclusiveItemData
{
    public int itemId;
    public int isUnlock; // 0 - 未解锁 1 - 解锁

    public void setData(ExclusiveData data)
    {
        itemId = data.itemId;
        isUnlock = data.isUnlock;
    }
}

public class TreasureBoxDataProxy : TSingletonHotfix<TreasureBoxDataProxy>, IDataModelProx
{
    private Dictionary<int, TreasureBoxData> boxDic;
    public bool boxIsInit;
    public int newBoxGroupId;
    public bool isBackToTown = true;
    public bool isOpening = false;
    public List<TreasureBoxData> boxList
    {
        get
        {
            var list = boxDic.Values.ToList()/*.FindAll(t => t.keyCount > 0 || t.count > 0)*/;
            return list;
        }
    }

    public bool hasBox
    {
        get
        {
            if (boxList.Count <= 0) return false;
            foreach (var item in boxList)
            {
                if (item.count > 0)
                    return true;
            }

            return false;
        }
    }
    public void Clear()
    {
        if (boxDic != null) boxDic.Clear();
        boxDic = null;
    }

    public void Init()
    {
        boxDic = new Dictionary<int, TreasureBoxData>();

        Helper.AddNetworkRespListener(MsgType.Response_TreasureBox_Data_Cmd, GetTreausreBoxData);
        Helper.AddNetworkRespListener(MsgType.Response_TreasureBox_Open_Cmd, GetOpenTreasureBoxData);
    }

    private void GetTreausreBoxData(HttpMsgRspdBase msg)
    {
        Response_TreasureBox_Data data = (Response_TreasureBox_Data)msg;

        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;

        for (int i = 0; i < data.treasureBox.Count; i++)
        {
            int index = i;
            AddBoxData(data.treasureBox[index]);
        }

        //boxDic = (from temp in boxDic orderby temp.Value.keyCount descending select temp).ToDictionary(pair => pair.Key, pair => pair.Value);
        boxIsInit = true;
    }

    private void GetOpenTreasureBoxData(HttpMsgRspdBase msg)
    {
        Response_TreasureBox_Open data = (Response_TreasureBox_Open)msg;

        if ((EErrorCode)data.errorCode != EErrorCode.EEC_Success) return;

        AddBoxData(data.treasureBox);
        isOpening = false;
        //boxDic = (from temp in boxDic orderby temp.Value.keyCount descending select temp).ToDictionary(pair => pair.Key, pair => pair.Value);
        PlatformManager.inst.GameHandleEventLog("Open_chest", "");
        EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.BOXCOMPLETE_SHOWUI, data.rewardItemList, data.treasureBox.boxItemId);
    }

    private void AddBoxData(TreasureBox boxData)
    {
        if (boxDic.ContainsKey(boxData.boxItemId))
        {
            boxDic[boxData.boxItemId].setData(boxData);
        }
        else
        {
            TreasureBoxData tempData = new TreasureBoxData();
            tempData.setData(boxData);
            boxDic.Add(tempData.boxItemId, tempData);
            if (boxData.boxCount > 0 && boxData.keyCount > 0 && newBoxGroupId == 0) newBoxGroupId = boxData.boxItemId;
        }
    }

    public void AddBoxData(Item item)
    {
        if (boxIsInit)
        {
            bool isHave = false;
            if (item.itemConfig.type == 8)
            {
                foreach (var boxData in boxDic.Values)
                {
                    if (boxData.boxItemId == item.ID)
                    {
                        isHave = true;
                        if (item.count > boxData.count && boxData.keyCount > 0)
                        {
                            newBoxGroupId = boxData.boxItemId;
                        }
                        boxData.count = item.count;
                        break;
                    }
                }
            }
            else if (item.itemConfig.type == 9)
            {
                foreach (var boxData in boxDic.Values)
                {
                    if (boxData.keyId == item.ID)
                    {
                        isHave = true;
                        if (item.count > boxData.keyCount && boxData.count > 0)
                        {
                            newBoxGroupId = boxData.boxItemId;
                        }
                        boxData.keyCount = item.count;
                        break;
                    }
                }

                //if (!isHave)
                //{
                //    isHave = true;
                //    TreasureBoxData newBox = new TreasureBoxData();
                //    newBox.boxId = item.ID - StaticConstants.
                //}
            }
            if (!isHave)
            {
                EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.REQUEST_TREASUREBOXDATA);
            }
        }
        //if (isInit)
        //    EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.REQUEST_TREASUREBOXDATA);
    }

    public TreasureBoxData GetDataByBoxId(int boxId)
    {
        if (boxDic.ContainsKey(boxId))
            return boxDic[boxId];

        //Logger.error("没有组id为" + boxId + "的宝箱数据");
        return null;
    }

    public int GetNextBox(TreasureBoxData curBox)
    {
        var notEmptyList = boxList.FindAll(t => t.count > 0);
        int index = notEmptyList.IndexOf(curBox);

        index += 1;
        if (index == notEmptyList.Count) index = 0;
        if (notEmptyList.Count <= 0) return -1;

        return notEmptyList[index].boxItemId;
    }

    public int GetLastBox(TreasureBoxData curBox)
    {
        var notEmptyList = boxList.FindAll(t => t.count > 0);
        int index = notEmptyList.IndexOf(curBox);

        index -= 1;
        if (index == -1) index = notEmptyList.Count - 1;
        if (notEmptyList.Count <= 0) return -1;

        return notEmptyList[index].boxItemId;
    }
}
