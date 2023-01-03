using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//战斗战报数据 
public class CombatDataProxy : TSingletonHotfix<CombatDataProxy>, IDataModelProx
{
    private CombatReport combatReport; //当前战斗数据
    public void Clear()
    {
    }

    public void Init()
    {
        Helper.AddNetworkRespListener(MsgType.Response_Gm_Command_Cmd, CombatInfoUpdate);
    }
    //战斗数据
    private List<FighterClr> allFighterList = new List<FighterClr>();  // 
    //==================================================================================================================
    //获取战报
    private void GetComBatReport()
    {

    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //收到战报
    private void CombatInfoUpdate(HttpMsgRspdBase msg)
    {
        Response_Gm_Command data = (Response_Gm_Command)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            combatReport = data.testReport;
        }
    }


}
