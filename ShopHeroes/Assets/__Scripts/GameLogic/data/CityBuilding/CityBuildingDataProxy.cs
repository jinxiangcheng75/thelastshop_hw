using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class CityBuildingDataProxy : TSingletonHotfix<CityBuildingDataProxy>, IDataModelProx
//{

//    public bool isInit;

//    List<CityBuildingData> cityBuildings;

//    public void Init()
//    {
//        cityBuildings = new List<CityBuildingData>();

//        //测试
//        //foreach (var item in BuildingConfigManager.inst.GetAllConfig())
//        //{
//        //    CityBuildingData data = new CityBuildingData();
//        //    data.SetInfo(item.id, 10, 1, 1180, 0);
//        //    //if (data.config.architecture_type == 3)
//        //    //{
//        //    //    data.SetInfo(item.id, 1, 0, 0, 0);
//        //    //}
//        //    cityBuildings.Add(data);
//        //}

//    }

//    public CityBuildingData GetBuildingData(int buildingId) 
//    {
//        return cityBuildings.Find((t) => t.buildingId == buildingId);
//    }


//    public List<CityBuildingData> GetAllScienceBuildingData(kCityBuildingType buildingType) 
//    {
//        return cityBuildings.FindAll((t) => t.config.architecture_type == (int)buildingType);
//    }

//    public void updateBuildingInfo(BuildData info)
//    {
//        CityBuildingData data = GetBuildingData(info.buildId);

//        if (data == null)
//        {
//            data = new CityBuildingData();
//            data.SetInfo(info.buildId,info.buildLevel,info.buildState,info.oneSelfCostCount,info.buildCostCount);
//            cityBuildings.Add(data);
//        }
//        else 
//        {
//            data.SetInfo(info.buildId, info.buildLevel, info.buildState, info.oneSelfCostCount, info.buildCostCount);
//        }

//    }


//    public void Clear()
//    {
//    }

//}
