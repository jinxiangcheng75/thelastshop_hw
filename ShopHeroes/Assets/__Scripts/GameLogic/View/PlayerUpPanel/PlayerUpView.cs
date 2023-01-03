using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpItemData
{
    /// <summary>
    /// 市场等级 0
    /// 英雄栏位 1
    /// 贸易栏位 2
    /// 制作栏位 3
    /// 店铺扩建限制 4
    /// 冒险栏位 5
    /// 勇士id 6
    /// 工匠id 7
    /// 英雄职业 8
    /// 新家具 9
    /// 新装饰 10
    /// </summary>
    public int mainType; // 1 - 10
    public int mainValue;
    public int[] subtypes;
}


public class PlayerUpView : ViewBase<PlayerUpComp>
{
    public override string viewID => ViewPrefabName.PlayerLevelUpUI;
    public override string sortingLayerName => "window";

    private List<PlayerUpItem> _playerUpItemList;

    private ShopkeeperUpconfig _config;
    private List<PlayerUpItemData> datas;


    private void ParseConfig()
    {
        datas = new List<PlayerUpItemData>();

        if (_config.market_level != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 0;
            data.mainValue = _config.market_level;
            datas.Add(data);
        }

        if (_config.hero_slot != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 1;
            data.mainValue = _config.hero_slot;
            datas.Add(data);
        }

        if (_config.trade != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 2;
            data.mainValue = _config.trade;
            datas.Add(data);
        }

        if (_config.maker_slot != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 3;
            data.mainValue = _config.maker_slot;
            datas.Add(data);
        }

        if (_config.shopSize_slot != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 4;
            data.mainValue = _config.shopSize_slot;
            datas.Add(data);
        }

        if (_config.adventur_slot != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 5;
            data.mainValue = _config.adventur_slot;
            datas.Add(data);
        }

        if (_config.warrior_id != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 6;
            data.mainValue = _config.warrior_id;
            datas.Add(data);
        }

        if (_config.worker_id != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 7;
            data.mainValue = _config.worker_id;
            datas.Add(data);
        }

        if (_config.hero_occupation[0] != -1)
        {
            PlayerUpItemData data = new PlayerUpItemData();
            data.mainType = 8;
            data.mainValue = -1;
            data.subtypes = _config.hero_occupation;
            datas.Add(data);
        }

        if (_config.furniture[0] != -1)
        {
            for (int i = 0; i < _config.furniture.Length; i++)
            {
                PlayerUpItemData data = new PlayerUpItemData();
                data.mainType = 9;
                data.mainValue = _config.furniture[i];
                data.subtypes = _config.furniture;
                datas.Add(data);
            }
        }

        if (_config.decoration[0] != -1)
        {
            for (int i = 0; i < _config.decoration.Length; i++)
            {
                PlayerUpItemData data = new PlayerUpItemData();
                data.mainType = 10;
                data.mainValue = _config.decoration[i];
                data.subtypes = _config.decoration;
                datas.Add(data);
            }
        }

    }

    protected override void onInit()
    {
        base.onInit();

        contentPane.continueBtn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.HIDEUI_PLAYERUPUI);
            //GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ArriveLevel, 0);
        });
        _playerUpItemList = new List<PlayerUpItem>();

        //生成小Item 最多五只
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = GameObject.Instantiate(contentPane.resItem.gameObject, contentPane.content, false);

            PlayerUpItem item = obj.GetComponent<PlayerUpItem>();

            _playerUpItemList.Add(item);
            item.Clear();
        }

        contentPane.vfxCanvns.sortingLayerName = "window";
    }

    protected override void onShown()
    {
        base.onShown();
        AudioManager.inst.PlaySound(69);
        if (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)
            GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ArriveLevel, 0);
        contentPane.lvTipText.text = LanguageManager.inst.GetValueByKey("恭喜，您已经达到等级{0}!", UserDataProxy.inst.playerData.level.ToString());
        contentPane.lvText.text = UserDataProxy.inst.playerData.level + "";

        contentPane.vfxCanvns.sortingOrder = _uiCanvas.sortingOrder + 1;


        _config = ShopkeeperUpconfigManager.inst.GetConfig(UserDataProxy.inst.playerData.level);
        ParseConfig();

        for (int i = 0; i < _playerUpItemList.Count; i++)
        {

            if (i < datas.Count)
            {
                _playerUpItemList[i].SetData(datas[i], i);
            }
            else
            {
                _playerUpItemList[i].Clear();
            }
        }

        ShopkeeperDataProxy.inst.curRole.SetUIPosition(contentPane.shopKeeperTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
        string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)ShopkeeperDataProxy.inst.curRole.gender, (int)kIndoorRoleActionType.normal_standby);
        ShopkeeperDataProxy.inst.curRole.Play(idleAnimationName, true);
        GUIHelper.setRandererSortinglayer(_uiCanvas.transform, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 2);
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
        var clipInfo = contentPane.uiAnimator.GetClipLength("show");
        //ShopkeeperDataProxy.inst.curRole.Fade(1, 0.15f).From(0);
        //DoTweenUtil.Fade_0_To_a_All(contentPane.topBg, 1, 0.5f);
        //DoTweenUtil.Fade_0_To_a_All(contentPane.continueBtn.transform, 1, 0.2f, clipInfo);
    }

    protected override void DoHideAnimation()
    {
        HideView();
    }

    protected override void onHide()
    {
        //
        ShopkeeperDataProxy.inst.curRole.transform.parent = null;
        ShopkeeperDataProxy.inst.curRole.SetActive(false);
        ShopkeeperDataProxy.inst.curRole.transform.position = Vector3.right * 1000;
        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
    }

}
