using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
public class GameStateShop : IStateBase
{
    IndoorMapEditSys indoorMapEditSys;
    public void onEnter(IStateTransition transition)
    {
        //获取背包、装备、资源生产、装备生产数据
        EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_GET_DATA);
        EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_UPDATEINFO);
        //加载场景
        startLoadScene = false;
        GameTimer.inst.StartCoroutine(loadIndoorMap());
        //showActor
        //获取顾客列表
        //EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_GETSHOPPERLIST);
        //showUI
        //获取储蓄罐数据
        EventController.inst.TriggerEvent(GameEventType.MoneyBoxEvent.MONEYBOX_REQUEST_DATA);
        //获取角色数据
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_ROLEDATA);
        // 获取招募数据
        EventController.inst.TriggerEvent(GameEventType.RoleEvent.REQUEST_RECRUITLIST);
        // 获取宝箱数据
        //EventController.inst.TriggerEvent(GameEventType.TreasureBoxEvent.REQUEST_TREASUREBOXDATA);
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLOREGROUPDATA);
        EventController.inst.TriggerEvent(GameEventType.ExploreEvent.REQUEST_EXPLORESLOTDATA);
        // 获取每日奖励数据
        EventController.inst.TriggerEvent(GameEventType.ActivityEvent.REQUEST_DAILYGIFTLIST);

        //获取城市建筑物数据 工匠与之关联 提前获取
        if (!UserDataProxy.inst.isInit) EventController.inst.TriggerEvent(GameEventType.CityBuildingEvent.CITYBUILDING_GET_DATA);
        //获取邮件数据
        EventController.inst.TriggerEvent(GameEventType.EmailEvent.EMAIL_REQUEST_DATA);
        // 获取成就数据
        EventController.inst.TriggerEvent(GameEventType.AcheivementEvent.REQUEST_ACHEIVEMENTCHECK);

        var preloadBgtf = FGUI.inst.uiRootTF.Find("preloadBg");
        if (preloadBgtf != null)
            GameObject.Destroy(preloadBgtf.gameObject);
    }
    bool startLoadScene = false;
    IEnumerator loadIndoorMap()
    {
        if (!startLoadScene)
        {
            startLoadScene = true;
            //加载场景(Add)
            AsyncOperationHandle operation = ManagerBinder.inst.mSceneMgr.loadScene("IndoorScene", "IndoorEnvironment");//SceneManager.LoadSceneAsync("IndoorScene", LoadSceneMode.Additive);
            while (!operation.IsDone)
            {
                yield return null;
            }
            yield return null;
            //SceneManager.LoadSceneAsync("IndoorEnvironment", LoadSceneMode.Additive);

            // FGUI.inst.StartExcessAnimation(false, false);
            //进入商店系统
            if (indoorMapEditSys == null)
            {
                indoorMapEditSys = new IndoorMapEditSys();
            }
            AudioManager.inst.PlayMusic(2);
            indoorMapEditSys.EnterSystem();
        }
    }
    public void onExit()
    {
        if (IndoorEnvironmentObjVisibleClr.inst != null)
        {
            IndoorEnvironmentObjVisibleClr.inst.setVisible(false);
        }
        startLoadScene = false;
        GameTimer.inst.StopCoroutine(loadIndoorMap());
        if (indoorMapEditSys != null)
        {
            indoorMapEditSys.ExitSystem();
        }
        EventController.inst.TriggerEvent(GameEventType.HIDEUI_SHOPSCENE);

        SceneManager.UnloadSceneAsync("IndoorScene");
        SceneManager.UnloadSceneAsync("IndoorEnvironment");

    }

    public void onReset()
    {

    }
    public kGameState getState()
    {
        return kGameState.Shop;
    }
}
