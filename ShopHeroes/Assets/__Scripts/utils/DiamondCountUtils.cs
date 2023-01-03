using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiamondCountUtils
{

    /*
       【家具升级、店铺扩建的加速金条花费】
       时间x = 小时数*60 + 分钟数 + 秒/60（ 四舍五入，最小取值1分钟）
       示例：花费 = 18 + 0.046*x
       公式：花费金条=c1+c2*x

       向上取整，最小花费y2(示例：10金条)
     */


    public static int GetFurnitureUpgradeDiamonds(int secs)
    {
        int x = Mathf.Max(1, (int)(secs / 60.0f));

        int leastCost = (int)WorldParConfigManager.inst.GetConfig(431).parameters;

        return Mathf.Max(leastCost, Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(429).parameters + WorldParConfigManager.inst.GetConfig(430).parameters * x));
    }


    /*
    
        【装备制造与冒险加速金条花费】
        时间x = 小时数*60 + 分钟数 + 秒/60（四舍五入，最小取值1分钟）
        x >= 600，
        示例：花费=0.021*x + 60.3 + 1.095
        公式：花费金条=a1*x+a2+a3

        x<600，
        示例：花费= 9.195 + 0.192* x - 0.00014* x^2
        公式：花费金条=b1+b2*x-b3*x^b4

        四舍五入，最小花费金条y1（示例：10金条）
     */

    public static int GetExploreOrMakeEquipUpgradeDiamonds(int secTime)
    {
        int x = Mathf.Max(1, (int)(secTime / 60.0f));

        int result = 0;

        int leastCost = (int)WorldParConfigManager.inst.GetConfig(428).parameters;

        if (x >= WorldParConfigManager.inst.GetConfig(420).parameters)
        {
            result = Mathf.Max(leastCost, Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(421).parameters * x + WorldParConfigManager.inst.GetConfig(422).parameters + WorldParConfigManager.inst.GetConfig(423).parameters));
        }
        else
        {
            result = Mathf.Max(leastCost, Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(424).parameters + WorldParConfigManager.inst.GetConfig(425).parameters * x - WorldParConfigManager.inst.GetConfig(426).parameters * Mathf.Pow(x, (int)WorldParConfigManager.inst.GetConfig(427).parameters)));
        }

        return result;
    }

    /*
        【英雄恢复加速的金条花费】
        时间x = 小时数*60 + 分钟数 + 秒/60（ 四舍五入，最小取值1分钟）
        示例：花费 = 1.02429 + 0.126446 x^0.8（x^0.8是x的0.8次方）
        公式：花费=d1+d2*x^d3

        四舍五入，最小花费y3(示例：1金条）
     */
    public static int GetHeroRestingFastCost(int secTime)
    {
        int x = Mathf.Max(1, (int)(secTime / 60.0f));
        return Mathf.Max((int)WorldParConfigManager.inst.GetConfig(435).parameters, Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(432).parameters + WorldParConfigManager.inst.GetConfig(433).parameters * Mathf.Pow(x, WorldParConfigManager.inst.GetConfig(434).parameters)));
    }


    /*
        【装备制造材料补充花费金条计算】
        花费金条数=0.07*补充材料数量+12
        公式：花费金条=Z1*X+Z2
       （向上取整，补充材料数量为该材料存储上限-当前拥有该材料数量）
        Z1与Z2取world_par的440/441id
     */

    public static int GetEquipMakeMaterialsReFullCost(int materialNum)
    {
        return Mathf.CeilToInt(WorldParConfigManager.inst.GetConfig(440).parameters * materialNum + WorldParConfigManager.inst.GetConfig(441).parameters);
    }



}
