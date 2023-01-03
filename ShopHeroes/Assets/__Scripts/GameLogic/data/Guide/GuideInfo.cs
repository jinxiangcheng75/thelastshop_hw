using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideInfo
{
    public bool isAllOver;
    public int m_curGroup;
    public int m_curIndex;

    public bool isClickTarget;
    public bool isDialogFinish;
    public bool isArriveTarget;
    public bool isCreatFinish;

    public int val = 0;

    public GuideConfigData m_curCfg;
    public void completeCheck()
    {
        if (m_curCfg != null)
        {
            if (m_curCfg.next_id != 0)
            {
                m_curCfg = GuideConfigManager.inst.GetConfig(m_curCfg.next_id);
                if (m_curCfg != null)
                {
                    m_curGroup = m_curCfg.sort;
                    m_curIndex = m_curCfg.index;
                    val = 0;
                }
                else
                {
                    Logger.error("没有id为" + m_curCfg.next_id + "的数据");
                    EventController.inst.TriggerEvent(GameEventType.GuideEvent.REALHIDEGUIDEUI);
                    isAllOver = true;
                }
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.REALHIDEGUIDEUI);
                isAllOver = true;
            }
        }
    }

    public void setGuideData(int guideId)
    {
        m_curCfg = GuideConfigManager.inst.GetConfig(guideId);
        if (m_curCfg != null)
        {
            if (m_curCfg.next_id == 0)
            {
                EventController.inst.TriggerEvent(GameEventType.GuideEvent.REALHIDEGUIDEUI);
                isAllOver = true;
            }
            m_curGroup = m_curCfg.sort;
            m_curIndex = m_curCfg.index;
        }
    }

    public bool JudgeIsFinishById(int id)
    {
        var cfg = GuideConfigManager.inst.GetConfig(id);
        if (cfg != null)
        {
            if (m_curGroup > cfg.sort)
                return true;
            else if (m_curGroup == cfg.sort && m_curIndex >= cfg.index)
                return true;
        }
        return false;
    }

    public void Clear()
    {
        isAllOver = false;
        m_curGroup = 0;
        m_curIndex = 0;

        isClickTarget = false;
        isDialogFinish = false;
        isArriveTarget = false;
        isCreatFinish = false;

        val = 0;

        m_curCfg = null;
    }
}
