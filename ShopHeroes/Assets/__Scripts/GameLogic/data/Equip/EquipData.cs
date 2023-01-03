//装备信息
using System.Collections.Generic;
using UnityEngine;
using System;

public class needMaterialsInfo
{
    public int needId; //资源或者配件
    public int needCount;  //资源数量
    public int type;   // 0= 普通资源  1=装备配件 2=特殊资源
}

public class progressItemInfo
{
    public int type;        //效果了类型
    public int reward_id;   //
    public float reward_value; //效果数值
    public string dec;  //说明
    public int exp;  //制作数量要求

    public progressItemInfo(int _t, int _id, float _value, string _dec, int _exp)
    {
        type = _t;
        reward_id = _id;
        reward_value = _value;
        dec = _dec;
        exp = _exp;
    }
}

public class starUpProgressItemInfo
{
    public ReceiveInfoUIType type; //类型
    public string atlas { get; private set; }
    public string iconName { get; private set; }
    public int needNum;
    public float value; //效果数值
    public string title; //标题
    public string des;  //说明

    public starUpProgressItemInfo(int _type, float _value, int _need_num, string _des)
    {
        type = ReceiveInfoUIType.StarUpEffectTrigger_return + _type - 1;
        atlas = "equipMakeUI_atlas";
        if (_type == 1) //返还所有材料
        {
            iconName = "shengxing_cailiao";
        }
        else if (_type == 2) //有概率制作2件装备 
        {
            iconName = "shengxing_banshou";
        }
        else if (_type == 3) //有概率制作超凡装备 
        {
            iconName = "shengxing_qianghua";
        }
        needNum = _need_num;
        value = _value;

        if (!string.IsNullOrEmpty(_des))
        {
            var strs = _des.Split('|');

            if (strs.Length >= 2)
            {
                title = strs[0];
                des = strs[1];
            }
        }
       
    }

}

//装备数据
public class EquipData
{
    public int equipDrawingId;
    public int level = 1;
    public float sellAddition;
    public int mainType;    //大类型
    public int subType;    //子类型
    /// info 服务器数据
    public int equipState = 0;      //0-未解锁 1-解锁未激活 2-已激活
    public int beenMake = 0;       //已制造 个数
    public int progressLevel = 0;    //0-5  当前第几档
    public int lastMakeTime = 0;    //最近制作时间
    public int unlockTime = 0;      //解锁时间
    public int activateTime = 0;
    public int favorite = 0;    //收藏
    public int starLevel = 0; //升星阶数
    ///////////////////////////
    public int sellPrice;
    public int buyPrice;
    //生产需要的材料
    public needMaterialsInfo[] needRes;
    public needMaterialsInfo specialRes_1;
    public needMaterialsInfo specialRes_2;
    //进度效果
    public progressItemInfo[] progresInfoList;
    public int barMaxValue = 0; //当前进度最大值
    public int currBarValue = 0; //当前进度
    public double makeTime; //制作耗时
    public bool isNew = false;
    public bool isNewActivate
    {
        get
        {
            if (PlayerPrefs.HasKey($"isn{equipDrawingId}"))
            {
                return false;
            }
            return beenMake <= 0;
        }

        set
        {
            PlayerPrefs.SetInt($"isn{equipDrawingId}", value ? 1 : 0);
        }
    }
}



