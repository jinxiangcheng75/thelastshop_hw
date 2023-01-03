using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 扩建面板    #陆泓屹
/// </summary>

//与后端数据映射后的数据
public class ExtensionData
{
    //解锁顺序
    public byte sequence;
    //等级需求
    public ushort level;
    //图标
    public string icon;
    //消耗金币
    public uint money;
    //消耗钻石
    public byte diamond;
    //消耗时间
    public uint time;
    //尺寸_X
    public byte size_x;
    //尺寸_Y
    public byte size_y;
    //家具数
    public byte furniture;
}

//从CSV表中解析的数据
public class ExtensionConfig
{
    //解锁顺序
    public int sequence;
    //等级需求
    public int shopkeeper_level;
    //图标
    public string icon;
    //消耗金币
    public long money;
    //消耗钻石
    public int diamond;
    //开始加速的钻石数
    public int start_diamond;
    //加速的钻石与秒的比例：n秒/钻
    public int rate;
    //消耗时间
    public int time;
    //尺寸_X
    public int size_x;
    //尺寸_Y
    public int size_y;
    //家具数
    public int furniture;
    //地图尺寸
    public int cell_minx;
    public int cell_maxx;
    public int cell_miny;
    public int cell_maxy;
}

public class ExtensionConfigManager : TSingletonHotfix<ExtensionConfigManager>, IConfigManager
{
    public Dictionary<int, ExtensionConfig> cfgDic = new Dictionary<int, ExtensionConfig>();
    public const string CONFIG_NAME = "shop_size";

    public void InitCSVConfig()
    {
        List<ExtensionConfig> scArray = CSVParser.GetConfigsFromCache<ExtensionConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);
        foreach (var sc in scArray)
        {
            if (sc.sequence < 0) continue;
            cfgDic.Add(sc.sequence, sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgDic.Clear();
        InitCSVConfig();
    }
    public ExtensionConfig GetExtensionConfig(int sequence_id)
    {
        if (cfgDic.ContainsKey(sequence_id))
        {
            return cfgDic[sequence_id];
        }
        return null;
    }

    public int GetExtensionLvMax()
    {
        return cfgDic.Count;
    }
}
