using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPosition
{
    public int index;
    public int x;
    public int y;
    public int dir;
}
public class IndoorGridMapClr : TSingletonHotfix<IndoorGridMapClr>, IConfigManager
{
    public int[,] maxIndoorGrid;   //最大室内网格数据  value：开放等级
    public int[,] outdoorGrid;

    List<CellPosition> floorPosList = new List<CellPosition>();
    List<CellPosition> wallPosList = new List<CellPosition>();
    //public List<IndoorData.ShopDesignItem> allFloorList = new List<IndoorData.ShopDesignItem>();
    public void InitCSVConfig()
    {
        maxIndoorGrid = new int[StaticConstants.IndoorMaxX, StaticConstants.IndoorMaxY];
        outdoorGrid = new int[6, 36];
        // allFloorList.Clear();
        Logger.log("====================初始化地图网格========================");
        string mapcsv = CSVParser.GetCSV("indoormap");
        if (mapcsv == null)
        {
            Logger.error("CSV cache not found : " + "indoormap");
            return;
        }
        string[] lines = null;
        if (mapcsv.IndexOf(CSVParser.WIN_LINEFEED) >= 0)
        {
            lines = mapcsv.Split(CSVParser.RNSTR_SPLIT, System.StringSplitOptions.None);
        }
        else
        {
            lines = mapcsv.Split(CSVParser.NLINE_SPLIT, System.StringSplitOptions.None);
        }
        for (int i = 3; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            if (string.IsNullOrEmpty(values[0])) continue;
            int x = int.Parse(values[0]);
            for (int j = 1; j < values.Length; j++)
            {
                if (!string.IsNullOrEmpty(values[j]))
                {
                    int value = int.Parse(values[j]);
                    if (x > 100)
                    {
                        outdoorGrid[x - 101, j - 1] = value;
                    }
                    else
                    {
                        maxIndoorGrid[x, j - 1] = value;
                    }
                }
            }
        }
        Logger.log("===========地图网格块初始化完成============");
        ///初始化所有地砖
        // int countx = (StaticConstants.IndoorMaxX + 1) / 4;
        // int county = (StaticConstants.IndoorMaxY + 1) / 4;
        // for (int x = 0; x < countx; x++)
        //     for (int y = 0; y < county; y++)
        //     {
        //         IndoorData.ShopDesignItem floor = new IndoorData.ShopDesignItem(11001, (int)kTileGroupType.Floor, 0, x * 4, y * 4, 4, 4, 0, 0, 0, 0, 0);
        //         floor.param_1 = maxIndoorGrid[floor.x, floor.y];  //开启等级
        //         floor.state = 3;
        //         allFloorList.Add(floor);
        //     }
    }
    //
    public void ReLoadCSVConfig()
    {
        floorPosList.Clear();
        wallPosList.Clear();
        InitCSVConfig();
    }
    public void ResetMapGrid(int maplevel)
    {

    }
    public List<CellPosition> GetfloorPosList(int maplevel)
    {
        RectInt size = new RectInt();
        if (GetCurrIndoorSize(maplevel, ref size))
        {
            floorPosList.Clear();
            for (int x = size.xMin; x < size.xMax; x += 2)
                for (int y = size.yMin; y < size.yMax; y += 2)
                {
                    var _floorPos = new CellPosition();
                    _floorPos.index = x * StaticConstants.IndoorMaxX + y * StaticConstants.IndoorMaxY;
                    _floorPos.x = x;
                    _floorPos.y = y;
                    _floorPos.dir = 0;
                    floorPosList.Add(_floorPos);
                }
            return floorPosList;
        }
        return null;
    }
    public List<CellPosition> GetWallPosList(int maplevel)
    {
        RectInt size = new RectInt();
        if (GetCurrIndoorSize(maplevel, ref size))
        {
            wallPosList.Clear();
            int index = 0;

            int xmax = size.xMax + 1;
            int ymax = size.yMax + 1;

            for (int x = size.xMin; x < size.xMax; x += 2)
            {
                var _wallPos = new CellPosition();
                _wallPos.index = index;
                _wallPos.x = x;
                _wallPos.y = ymax;
                _wallPos.dir = 0;
                wallPosList.Add(_wallPos);
                index++;
            }
            for (int y = size.yMin; y < size.yMax; y += 2)
            {
                var _wallPos = new CellPosition();
                _wallPos.index = index;
                _wallPos.x = xmax;
                _wallPos.y = y;
                _wallPos.dir = 1;
                wallPosList.Add(_wallPos);
                index++;
            }
            return wallPosList;
        }
        return null;
    }
    public bool GetCurrIndoorSize(int level, ref RectInt size)
    {
        // RectInt size = new RectInt(0, 0, 0, 0);
        ExtensionConfig cfg = ExtensionConfigManager.inst.GetExtensionConfig(level);
        if (cfg != null)
        {
            size.xMin = cfg.cell_minx;
            size.yMin = cfg.cell_miny;
            size.xMax = cfg.cell_maxx;
            size.yMax = cfg.cell_maxy;
            return true;
        }
        return false;
    }
}
