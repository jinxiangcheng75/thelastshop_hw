using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingCostConfig
{
    public int id;
    public int click_interval_min;
    public int click_interval_max;
    public int grade_A_gold;
    public int grade_B_gold;
    public int grade_C_gold;
    public int grade_A_diamond;
    public int grade_B_diamond;
    public int grade_C_diamond;


    public ulong GetGrade_Gold(int costGrade)
    {
        int grade_gold = 0;
        switch (costGrade)
        {
            case 1: grade_gold = grade_A_gold; break;
            case 2: grade_gold = grade_B_gold; break;
            case 3: grade_gold = grade_C_gold; break;
            default:
                break;
        }

        return (ulong)grade_gold;
    }

    public ulong GetGrade_Diamond(int costGrade)
    {
        int grade_diamond = 0;
        switch (costGrade)
        {
            case 1: grade_diamond = grade_A_diamond; break;
            case 2: grade_diamond = grade_B_diamond; break;
            case 3: grade_diamond = grade_C_diamond; break;
            default:
                break;
        }

        return (ulong)grade_diamond;
    }

}


public class BuildingCostConfigManager : TSingletonHotfix<BuildingCostConfigManager>, IConfigManager
{
    public Dictionary<int, BuildingCostConfig> buildingCostDic = new Dictionary<int, BuildingCostConfig>();

    public const string CONFIG_NAME = "architecture_cost";

    public void ReLoadCSVConfig()
    {
        strategyDic.Clear();
        buildingCostDic.Clear();
        InitCSVConfig();
    }
    public void InitCSVConfig()
    {
        List<BuildingCostConfig> buildingCostArr = CSVParser.GetConfigsFromCache<BuildingCostConfig>
            (CONFIG_NAME, CSVParser.STRING_SPLIT);

        foreach (var sc in buildingCostArr)
        {
            if (sc.id <= 0) continue;
            buildingCostDic.Add(sc.id, sc);
        }

        strategyDic = new Dictionary<int, ICalInvestCost>()
       {
       {1,new By1To10(1)},
       {2,new By11To20(2)},
       {3,new By21To30(3)},
       {4,new By31To50(4)},
       {5,new By51To80(5)},
       {6,new By81To100(6)},
       {7,new By101To140(7)},
       {8,new By141To180(8)},
       {9,new By181To250(9)},
       {10,new By251To310(10)},
       {11,new By311To400(11)},
       {12,new By401To500(12)},
       {13,new By501To630(13)},
       {14,new By631To750(14)},
       {15,new By751To850(15)},
       {16,new By851To950(16)},
       {17,new By951To1040(17)},
       {18,new By1041To1140(18)},
       {19,new By1141To1235(19)},
       };

    }

    public BuildingCostConfig[] GetAllConfig()
    {
        return buildingCostDic.Values.ToArray();
    }

    public BuildingCostConfig GetConfig(int key)
    {
        if (buildingCostDic.ContainsKey(key))
        {
            return buildingCostDic[key];
        }
        return null;
    }

    private Dictionary<int, ICalInvestCost> strategyDic;
    private ulong goldResult, gemResult;

    private ICalInvestCost getCurCalInvestCostStrategy(int startCount)
    {
        int index = 0;

        foreach (var item in buildingCostDic.Values)
        {
            if (startCount >= item.click_interval_min)
            {
                index++;
            }
            else
            {
                break;
            }
        }

        return strategyDic[index > 19 ? 19 : index];
    }

    public ulong GetInvestCost(int startCount, int investCount, int type, out ulong gemCost)
    {
        goldResult = 0;
        gemResult = 0;
        CalInvestCostStrategy investCostStrategy = new CalInvestCostStrategy();
        investCostStrategy.SetCalInvestCostStrategy(getCurCalInvestCostStrategy(startCount));
        investCostStrategy.calCost(startCount, investCount, type);
        gemCost = gemResult;
        return goldResult;
    }



    interface ICalInvestCost
    {
        void calCost(int startCount, int investCount, int type);
    }

    class CalInvestCostStrategy
    {
        ICalInvestCost curStrategy;

        public void SetCalInvestCostStrategy(ICalInvestCost strategy)
        {
            curStrategy = strategy;
        }

        public void calCost(int startCount, int investCount, int type)
        {
            curStrategy.calCost(startCount, investCount, type);
        }

    }

    abstract class CalAssist : ICalInvestCost
    {
        protected int level;
        private BuildingCostConfig config;
        int curLevelMin;
        int curLevelMax;

        public CalAssist(int level)
        {
            this.level = level;
            config = inst.GetConfig(level);
            curLevelMin = config.click_interval_min;
            curLevelMax = config.click_interval_max;
        }

        public void calCost(int startCount, int investCount, int type)
        {
            ulong unitGold = config.GetGrade_Gold(type);
            ulong unitGem = config.GetGrade_Diamond(type);

            int count = startCount + investCount < curLevelMax ? investCount : curLevelMax - startCount;

            inst.goldResult += (ulong)count * unitGold;
            inst.gemResult += (ulong)count * unitGem;

            if (startCount + investCount > curLevelMax)
            {
                CalInvestCostStrategy costStrategy = new CalInvestCostStrategy();
                costStrategy.SetCalInvestCostStrategy(inst.strategyDic[level + 1]);
                costStrategy.calCost(curLevelMax, investCount - count, type);
            }
        }
    }

    class By1To10 : CalAssist
    {
        public By1To10(int level) : base(level) { }
    }

    class By11To20 : CalAssist
    {
        public By11To20(int level) : base(level) { }
    }

    class By21To30 : CalAssist
    {
        public By21To30(int level) : base(level) { }
    }

    class By31To50 : CalAssist
    {
        public By31To50(int level) : base(level) { }
    }

    class By51To80 : CalAssist
    {
        public By51To80(int level) : base(level) { }
    }

    class By81To100 : CalAssist
    {
        public By81To100(int level) : base(level) { }
    }

    class By101To140 : CalAssist
    {
        public By101To140(int level) : base(level) { }
    }

    class By141To180 : CalAssist
    {
        public By141To180(int level) : base(level) { }

    }

    class By181To250 : CalAssist
    {
        public By181To250(int level) : base(level) { }
    }

    class By251To310 : CalAssist
    {
        public By251To310(int level) : base(level) { }
    }

    class By311To400 : CalAssist
    {
        public By311To400(int level) : base(level) { }
    }

    class By401To500 : CalAssist
    {
        public By401To500(int level) : base(level) { }
    }

    class By501To630 : CalAssist
    {
        public By501To630(int level) : base(level) { }
    }

    class By631To750 : CalAssist
    {
        public By631To750(int level) : base(level) { }
    }

    class By751To850 : CalAssist
    {
        public By751To850(int level) : base(level) { }
    }

    class By851To950 : CalAssist
    {
        public By851To950(int level) : base(level) { }

    }

    class By951To1040 : CalAssist
    {
        public By951To1040(int level) : base(level) { }

    }

    class By1041To1140 : CalAssist
    {
        public By1041To1140(int level) : base(level) { }
    }

    class By1141To1235 : CalAssist
    {
        public By1141To1235(int level) : base(level) { }
    }


}

