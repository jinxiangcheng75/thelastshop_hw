using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public interface IConfigManager
{
    void InitCSVConfig();
    void ReLoadCSVConfig();
}
public class GameStatePreload : IStateBase
{
    public void onEnter(IStateTransition transition)
    {
        //EventController.inst.TriggerEvent(GameEventType.SHOWUI_LOGIN);
        //load channel
        //load patch
        //load neccessarily assets
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_LOADINGUI);

        //FGUI.inst.StartExcessAnimation(false, null);
    }

    public void onExit()
    {
        EventController.inst.TriggerEvent(GameEventType.HIDEUI_LOADINGUI);
    }

    void onAllLoaded()
    {
        initConfigs();
    }
    List<IConfigManager> managerList = new List<IConfigManager>();
    void initConfigs()
    {
        managerList.Clear();
        //managerList.Add(ChineseLanguageConfigManager.inst);
        //managerList.Add(EnglishLanguageConfigManager.inst);
        managerList.Add(LanguageConfigManager.inst);
        managerList.Add(ResourceBinUpgradeConfigManager.inst);
        managerList.Add(GameTipsConfigManager.inst);
        managerList.Add(UIUnLockConfigMrg.inst);
        managerList.Add(VFXConfigManager.inst);
        managerList.Add(ItemconfigManager.inst);
        managerList.Add(heroupgradeconfigManager.inst);
        managerList.Add(EquipConfigManager.inst);
        managerList.Add(TaskItemConfigManager.inst);
        managerList.Add(ExtensionConfigManager.inst);
        managerList.Add(dressconfigManager.inst);
        managerList.Add(CharacterModelConfigManager.inst);
        managerList.Add(FieldConfigManager.inst);
        managerList.Add(CounterUpgradeConfigManager.inst);
        managerList.Add(ShelfUpgradeConfigManager.inst);
        managerList.Add(FurnitureClassificationConfigManager.inst);
        managerList.Add(FurnitureItemiconConfigManager.inst);
        managerList.Add(TrunkUpgradeConfigManager.inst);
        managerList.Add(FurnitureConfigManager.inst);
        managerList.Add(ShopkeeperUpconfigManager.inst);
        managerList.Add(MusicConfigManager.inst);
        managerList.Add(AITalkConfigManager.inst);
        managerList.Add(ShelfDisplayConfigManager.inst);
        managerList.Add(WorkerConfigManager.inst);
        managerList.Add(WorkerUpConfigManager.inst);
        managerList.Add(MarketBoothConfigManger.inst);
        managerList.Add(MarketEquipLvManager.inst);
        managerList.Add(CumulativeRewardConfigManager.inst);
        managerList.Add(WorldParConfigManager.inst);
        managerList.Add(IndoorGridMapClr.inst);
        managerList.Add(HeroAttributeConfigManager.inst);
        managerList.Add(heroSkillConfigManager.inst);
        managerList.Add(BuildingConfigManager.inst);
        managerList.Add(HeroProfessionConfigManager.inst);
        managerList.Add(BuildingCostConfigManager.inst);
        managerList.Add(BuildingUpgradeConfigManager.inst);
        managerList.Add(ExploreInstanceConfigManager.inst);
        managerList.Add(ExploreInstanceLvConfigManager.inst);
        managerList.Add(MonsterConfigManager.inst);
        managerList.Add(EquipActionConfigManager.inst);
        managerList.Add(TreasureBoxConfigManager.inst);
        managerList.Add(BuffConfigManager.inst);
        managerList.Add(HeroSkillShowConfigManager.inst);
        managerList.Add(HeroTalentDBConfigManager.inst);
        managerList.Add(GuideConfigManager.inst);
        managerList.Add(NameConfigManager.inst);
        managerList.Add(GamePayPricecConfigManager.inst);
        //  AssetCache.LoadAllCSV();
        ManagerBinder.inst.StartGame();
        GameTimer.inst.StartCoroutine(delayChangeState());
    }

    class groupinfo
    {
        public string name;
        public List<string> keys;
    }
    class ResGroup
    {
        public List<groupinfo> groups;
    }
    IEnumerator InitGameConfigs()
    {
        // while (!AssetCache.csvsHandle.IsDone)
        // {
        //     yield return null;
        // }
        // yield return null;

        WordFilter.inst.InitCSV();

        //开始初始化configmgr
        foreach (var mgr in managerList)
        {
            mgr.InitCSVConfig();
            yield return null;
        }

        // //清理configasset
        // AssetCache.ClearAllConfigAsset();
        ManagerBinder.inst.StartGame();
        yield return delayChangeState();
    }


    IEnumerator delayChangeState()
    {
        yield return new WaitForSeconds(1);
        //GameStateEvent.inst.changeState(new StateTransition(kGameState.Login, false));
        HotfixBridge.inst.ChangeState(new StateTransition(kGameState.Login, false));
    }

    public void onReset()
    {

    }

    public kGameState getState()
    {
        return kGameState.Preload;
    }
}
