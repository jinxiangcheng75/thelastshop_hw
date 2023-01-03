using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;

public class GuideTriggerData
{
    public int id;
    public GuideTriggerConditionConfigData cfg;
    public bool isTrigger;

    public GuideTriggerData(int id, int isTrigger)
    {
        this.id = id;
        this.isTrigger = isTrigger == 1;
        cfg = GuideTriggerConditionConfigManagaer.inst.GetConfig(id);
    }
}

public class GuideDataProxy : TSingletonHotfix<GuideDataProxy>, IDataModelProx
{
    private GuideInfo m_curInfo = new GuideInfo();
    private bool isInit = false;
    public Dictionary<int, NpcController> npc = new Dictionary<int, NpcController>();
    private Dictionary<int, GuideTriggerData> guideTriggerDic = new Dictionary<int, GuideTriggerData>();
    public List<SpiderWeb> spiders = new List<SpiderWeb>();
    public float needWaitTime = 0;
    public bool taskIsShowing;
    public int curTargetId;
    public string curMakeSlotName;

    public bool isGetNetworkd = false;
    public GuideInfo CurInfo
    {
        get { return m_curInfo; }
    }

    public void Clear()
    {
        npc.Clear();
        guideTriggerDic.Clear();
        spiders.Clear();
        m_curInfo.Clear();
    }

    public void Init()
    {
        //m_curInfo = new GuideInfo();

        Helper.AddNetworkRespListener(MsgType.Response_User_SetGuide_Cmd, GetSetGuideData);
        Helper.AddNetworkRespListener(MsgType.Response_User_SkipGuide_Cmd, GetSkipGuideData);
    }

    public void setGuideData(int guideId)
    {
        //m_curInfo.m_curGroup = 40;
        //m_curInfo.m_curIndex = 1;
        //m_curInfo.m_curCfg = GuideConfigManager.inst.GetConfigByGroupAndIndex(m_curInfo.m_curGroup, m_curInfo.m_curIndex);
        //EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SETGUIDE, m_curInfo.m_curCfg.id);

        //EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SKIPGUIDE);



        // 0 - 没有全完成 1 - 全部领取完成

        if (PlayerPrefs.HasKey(UserDataProxy.inst.playerData.userUid + "SevenDayState"))
        {
            if (PlayerPrefs.GetInt(UserDataProxy.inst.playerData.userUid + "SevenDayState") == 0)
            {
                EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYCHECK);
            }
            else
            {
                SevenDayGoalDataProxy.inst.isAllOver = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt(UserDataProxy.inst.playerData.userUid + "SevenDayState", 0);
            EventController.inst.TriggerEvent(GameEventType.SevenDayGoalEvent.REQUEST_SEVENDAYCHECK);
        }

        var worldParCfg = WorldParConfigManager.inst.GetConfig(129);
        if (worldParCfg != null && worldParCfg.parameters == 0)
        {
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.REQUEST_SKIPGUIDE);

            m_curInfo.setGuideData(99999);
            return;
        }


        if (guideId == 0)
        {
            m_curInfo.m_curGroup = 1;
            m_curInfo.m_curIndex = 1;
            m_curInfo.m_curCfg = GuideConfigManager.inst.GetConfigByGroupAndIndex(m_curInfo.m_curGroup, m_curInfo.m_curIndex);
        }
        else
        {
            m_curInfo.m_curCfg = GuideConfigManager.inst.GetConfig(guideId);
            if (m_curInfo.m_curCfg == null)
                m_curInfo.isAllOver = true;
            else
            {
                m_curInfo.m_curGroup = m_curInfo.m_curCfg.sort;
                m_curInfo.m_curIndex = m_curInfo.m_curCfg.index;
                if (!GuideDataProxy.inst.CurInfo.isAllOver && ((K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.RestrictClick || (K_Guide_Type)GuideDataProxy.inst.CurInfo.m_curCfg.guide_type == K_Guide_Type.TipsAndRestrictClick))
                {
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.WAITPREMASK, true);
                }
                if ((K_Guide_Type)m_curInfo.m_curCfg.guide_type == K_Guide_Type.End)
                {
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.REALHIDEGUIDEUI);
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDESKIPBTN);
                    m_curInfo.isAllOver = true;
                    EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REQUESTMAINLINEDATA);
                    HotfixBridge.inst.TriggerLuaEvent("REQUEST_GuideTask");
                    if (IndoorMap.inst != null && IndoorMap.inst.indoorMask != null)
                    {
                        IndoorMap.inst.indoorMask.transform.position = new Vector3(IndoorMap.inst.indoorMask.transform.position.x, IndoorMap.inst.indoorMask.transform.position.y, -10);
                    }
                }
            }
        }
    }

    //int waitTimerId = 0;
    void GetSetGuideData(HttpMsgRspdBase msg)
    {
        Response_User_SetGuide data = (Response_User_SetGuide)msg;
        EventController.inst.TriggerEvent(GameEventType.UIUnlock.GUIDE_END);
        JudgeSDKAppFlyerEvent(data.guideId);
        if (!AccountDataProxy.inst.needCreatRole/* || (AccountDataProxy.inst.NeedCreatRole && data.guideId == 101)*/)
            GuideManager.inst.waitNetworkBack();
        if (GuideManager.inst.isLoop)
        {
            GuideManager.inst.isLoop = false;
        }
        if (data.guideId == 99999)
        {
            if (m_curInfo != null)
            {
                m_curInfo.isAllOver = true;
            }
            else
            {
                m_curInfo = new GuideInfo();
                m_curInfo.isAllOver = true;
            }
            //if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.Shopkeeper != null)
            //    IndoorMapEditSys.inst.Shopkeeper.SetState((int)MachineShopkeeperState.onCounterRound);
            HotfixBridge.inst.TriggerLuaEvent("Guide_SetShopkeeperMachineState", (int)MachineShopkeeperState.onCounterRound);
            EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDESKIPBTN);
            EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTCHECK);
            EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REQUESTMAINLINEDATA);
            HotfixBridge.inst.TriggerLuaEvent("REQUEST_GuideTask");

            if (IndoorMap.inst != null && IndoorMap.inst.indoorMask != null)
            {
                IndoorMap.inst.indoorMask.transform.position = new Vector3(IndoorMap.inst.indoorMask.transform.position.x, IndoorMap.inst.indoorMask.transform.position.y, -10);
            }
        }
        // 触发式引导触发
        HotfixBridge.inst.TriggerLuaEvent("CheckGuideTrigger", 1, data.guideId);
    }

    private void GetSkipGuideData(HttpMsgRspdBase msg)
    {
        Response_User_SkipGuide data = (Response_User_SkipGuide)msg;

        if (m_curInfo != null)
            m_curInfo.setGuideData(data.guideId);
        EventController.inst.TriggerEvent(GameEventType.GuideEvent.HIDEALLSUBPANEL);
        EventController.inst.TriggerEvent(GameEventType.GuideEvent.REALHIDEGUIDEUI);
        if (IndoorMap.inst != null && IndoorMap.inst.indoorMask != null)
            //GameObject.Destroy(IndoorMap.inst.indoorMask.gameObject);
            IndoorMap.inst.indoorMask.SetActive(false);
        EventController.inst.TriggerEvent(GameEventType.UIUnlock.GUIDE_END);
        DestroyAllNpc();
        ActiveFalseAllSpider();
        GuideManager.inst.RemoveAllTime();
        //if (IndoorMapEditSys.inst.Shopkeeper != null)
        //{
        //    IndoorMapEditSys.inst.Shopkeeper.gameObject.SetActive(true);
        //    IndoorMapEditSys.inst.Shopkeeper.SetState((int)MachineShopkeeperState.onCounterRound);
        //}
        HotfixBridge.inst.TriggerLuaEvent("Guide_SetShopkeeperObjActive", true);
        HotfixBridge.inst.TriggerLuaEvent("Guide_SetShopkeeperMachineState", (int)MachineShopkeeperState.onCounterRound);
        JudgeSDKAppFlyerEvent(data.guideId);
        EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTCHECK);
        EventController.inst.TriggerEvent(GameEventType.MainlineTaskEvent.REQUESTMAINLINEDATA);
        HotfixBridge.inst.TriggerLuaEvent("REQUEST_GuideTask");

        // 触发式引导触发
        HotfixBridge.inst.TriggerLuaEvent("CheckGuideTrigger", 1, data.guideId);

        if (IndoorMap.inst != null && IndoorMap.inst.indoorMask != null)
        {
            IndoorMap.inst.indoorMask.transform.position = new Vector3(IndoorMap.inst.indoorMask.transform.position.x, IndoorMap.inst.indoorMask.transform.position.y, -10);
        }
    }

    public NpcController GetNpcById(int npcId)
    {
        if (npc.ContainsKey(npcId))
            return npc[npcId];

        return null;
    }

    public void AddNpc(NpcController npcC)
    {
        if (!npc.ContainsKey(npcC.npcId))
            npc.Add(npcC.npcId, npcC);
    }

    public void RemoveNpc(int npcId)
    {
        if (npc.ContainsKey(npcId))
        {
            npc.Remove(npcId);
        }
    }

    public void DestroyAllNpc()
    {
        foreach (var item in npc.Values)
        {
            if (item != null)
                GameObject.Destroy(item.gameObject);
        }

        npc.Clear();
    }

    public void ActiveFalseAllSpider()
    {
        for (int i = 0; i < spiders.Count; i++)
        {
            if (spiders[i] != null)
            {
                spiders[i].setAllActiveFalse();
            }
        }
    }

    private void JudgeSDKAppFlyerEvent(int guideId)
    {
        if (guideId == 103)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Start", "");
        }
        else if (guideId == 108)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_FixCounter", "");
        }
        else if (guideId == 206)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_ColdShelf", "");
        }
        else if (guideId == 504)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_William", "");
        }
        else if (guideId == 704)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Machete", "");
        }
        else if (guideId == 1004)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_IronBin", "");
        }
        else if (guideId == 1401)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Explore", "");
        }
        else if (guideId == 1507)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_ArmorShelf", "");
        }
        else if (guideId == 1704)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_1stSale", "");
        }
        else if (guideId == 2204)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Lin", "");
        }
        else if (guideId == 2507)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_LeatherBin", "");
        }
        else if (guideId == 2905)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Slot_2", "");
        }
        else if (guideId == 3304)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_QualityUp", "");
        }
        else if (guideId == 3504)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Hoodie", "");
        }
        else if (guideId == 4010)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Recruit1", "");
        }
        else if (guideId == 4201)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Recruit2", "");
        }
        else if (guideId == 4402)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Start1stQuest", "");
        }
        else if (guideId == 4405)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_1stRush", "");
        }
        else if (guideId == 4601)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Fish1stQuest", "");
        }
        else if (guideId == 4605)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_QuickHeal", "");
        }
        else if (guideId == 99999)
        {
            PlatformManager.inst.GameHandleEventLog("Guide_Finish", "");
        }
    }
}
