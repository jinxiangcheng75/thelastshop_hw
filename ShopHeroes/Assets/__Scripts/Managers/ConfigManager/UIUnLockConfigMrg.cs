using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUnLockConfig
{
    public int id;
    public int type;
    public int unlock_num;
    public int showtype;
    public string ui_prefab;
    public string ui_btn;
}
public class UIUnLockConfigMrg : TSingletonHotfix<UIUnLockConfigMrg>, IConfigManager
{
    public const string CONFIG_NAME = "ui_unlock";
    private List<UIUnLockConfig> cfgList = new List<UIUnLockConfig>();

    public void InitCSVConfig()
    {
        List<UIUnLockConfig> resList = CSVParser.GetConfigsFromCache<UIUnLockConfig>(CONFIG_NAME, CSVParser.STRING_SPLIT);
        foreach (var sc in resList)
        {
            if (sc.id <= 0) continue;
            cfgList.Add(sc);
        }
    }
    public void ReLoadCSVConfig()
    {
        cfgList.Clear();

        InitCSVConfig();
    }
    public List<UIUnLockConfig> unlockCfgList
    {
        get { return cfgList; }
    }

    public List<UIUnLockConfig> getConfigsByUIName(string name)
    {
        return cfgList.FindAll(item => item.ui_prefab == name);
    }

    private Dictionary<string, bool> btnInteractable = new Dictionary<string, bool>();

    public void setInteractableValue(string name, bool value)
    {
        if (btnInteractable.ContainsKey(name))
        {
            btnInteractable[name] = value;
        }
        else
        {
            btnInteractable.Add(name, value);
        }
    }
    public bool havsKey(string name)
    {
        return btnInteractable.ContainsKey(name);
    }

    public bool HasBtnMatchedCfg(string name) 
    {
        var cfg = cfgList.Find(icfg => icfg.ui_btn == name);

        return cfg != null;
    }

    public bool GetBtnInteractable(string name)
    {
        var cfg = cfgList.Find(icfg => icfg.ui_btn == name);
        if (cfg != null)
        {
            var term = checkTerm(cfg);
            if (btnInteractable.ContainsKey(name))
            {
                btnInteractable[name] = term;
                return btnInteractable[name];
            }
            else
            {
                setInteractableValue(name, term);
                return term;
            }
        }
        return true;
    }

    public void updateAll()
    {
        foreach (var cfg in cfgList)
        {
            if (cfg != null)
            {
                var term = checkTerm(cfg);
                if (btnInteractable.ContainsKey(cfg.ui_btn))
                {
                    btnInteractable[cfg.ui_btn] = term;
                }
                else
                {
                    setInteractableValue(cfg.ui_btn, term);
                }
            }
        }
    }
    public bool checkTerm(UIUnLockConfig cfg)
    {
        switch (cfg.type)
        {
            case 1: //引导
                //return true;
                return GuideDataProxy.inst.CurInfo.JudgeIsFinishById(cfg.unlock_num);

            case 2: //店主等级
                uint level = UserDataProxy.inst.playerData.level;
                if (cfg.unlock_num <= level)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case 3:
                if (cfg.unlock_num < UserDataProxy.inst.currMainTaskGroup)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        return false;
    }
}
