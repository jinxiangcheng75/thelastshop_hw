using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerInfoPanelView : ViewBase<PlayerInfoPanelComp>
{
    public override string viewID => ViewPrefabName.PlayerInfoPanel;
    public override string sortingLayerName => "window";

    PlayerInfoData _data;

    public string curUserId { get { return _data == null ? string.Empty : _data.userUid; } }


    DressUpSystem dressUpSystem;
    DressUpSystem petDressUpSystem;

    string[] animStrs = { "call", "idle", "play", "rest" };

    bool hasPet;
    int timerId_petAnim;
    bool impeachPresidentTimeEnough = false;


    //GraphicDressUpSystem headGraphicSystem;

    protected override void onInit()
    {
        base.onInit();

        InitPanelUI();

        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_User_DressList()
        });
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");
        var clipInfo = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");
        GameTimer.inst.AddTimer(clipInfo, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });
    }

    private void InitPanelUI()
    {
        contentPane.headBtn.onClick.AddListener(showShopkeeperPanel);
        contentPane.closeBtn.ButtonClickTween(CloseRoleInfoPanel);
        contentPane.headCircleBtn.ButtonClickTween(showShopkeeperPanel);
        contentPane.changeNameBtn.ButtonClickTween(ChangeName);
        contentPane.setMemberJobBtn.ButtonClickTween(onSetMemberJobBtnClick);
        contentPane.customBtn.ButtonClickTween(showShopkeeperPanel);

        //
        contentPane.expTf.gameObject.SetActive(true);
        contentPane.visitTF.gameObject.SetActive(false);
        contentPane.VisitBtn.onClick.AddListener(() =>
        {

            GUIManager.BackMainView();
            GameTimer.inst.AddTimer(1, 1, () =>
            {
                //拜访
                EventController.inst.TriggerEvent(GameEventType.VisitShopEvent.VISIT_ENTER_SHOP, _data.userUid);
            });
            contentPane.VisitBtn.onClick.RemoveAllListeners();
            //HotfixBridge.inst.ChangeState(new StateTransition(kGameState.VisitShop, true));
        });

        contentPane.vipBtn.ButtonClickTween(() =>
        {
            HotfixBridge.inst.TriggerLuaEvent("ShowUI_BuyVipUI", 0);
        });

        WorldParConfig worldParConfig = WorldParConfigManager.inst.GetConfig(352);
        if (worldParConfig != null)
        {
            contentPane.impeachGemCostTx.text = ((int)worldParConfig.parameters).ToString();
        }

        contentPane.impeachPresidentBtn.ButtonClickTween(onBtn_impeachPresidentClick);

    }

    private void onBtn_impeachPresidentClick()
    {

        if (!impeachPresidentTimeEnough)
        {

            int impeachPresidentTime = 72 * 60 * 60; //默认72小时
            var worldParConfig = WorldParConfigManager.inst.GetConfig(351);
            if (worldParConfig != null) impeachPresidentTime = (int)worldParConfig.parameters;

            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("会长离线时间超过{0}小时方可弹劾", (impeachPresidentTime / 60 / 60).ToString()), GUIHelper.GetColorByColorHex("FF2828"));

        }
        else
        {

            if (!contentPane.impeachPresidentGemAffirmObj.activeSelf)
            {
                contentPane.impeachPresidentGemAffirmObj.SetActive(true);
            }
            else
            {
                var worldParCfg = WorldParConfigManager.inst.GetConfig(352);

                if (worldParCfg != null && UserDataProxy.inst.playerData.gem < worldParCfg.parameters)
                {
                    HotfixBridge.inst.TriggerLuaEvent("ShowUI_GameHintUI", 31, null, (int)worldParCfg.parameters - UserDataProxy.inst.playerData.gem);
                }
                else
                {
                    //发送弹劾协议
                    HotfixBridge.inst.TriggerLuaEvent("Request_ImpeachPresident", _data.userUid);
                }

            }

        }

    }

    private void CloseRoleInfoPanel()
    {
        //AudioManager.inst.PlaySound(11);
        hide();
    }



    void showShopkeeperPanel()
    {
        if (_data == null || _data.userUid == UserDataProxy.inst.playerData.userUid) //自己
        {
            if (UserDataProxy.inst.playerData.level < WorldParConfigManager.inst.GetConfig(404).parameters)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("{0}级解锁自定义装扮功能", WorldParConfigManager.inst.GetConfig(404).parameters.ToString()), GUIHelper.GetColorByColorHex("FF2828"));
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_SHOPKEEPERPANEL);
            }
        }
    }

    private void ChangeName()
    {

        EventController.inst.TriggerEvent(GameEventType.SHOWUI_CHANGENAME);
    }

    void onSetMemberJobBtnClick()
    {
        if (UserDataProxy.inst.playerData.unionId == _data.unionData.unionId) //自己公会的
        {
            if (UserDataProxy.inst.playerData.memberJob <= _data.unionData.memberJob)
            {
                EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("您没有更改这些设置的权限"), GUIHelper.GetColorByColorHex("FF2828"));
            }
            else
            {
                EventController.inst.TriggerEvent(GameEventType.UnionEvent.SHOWUI_UNIONMEMBERSETTING, _data.userUid, _data.unionData.memberJob);
            }
        }
    }

    void setSelfExpInfo()
    {
        contentPane.playerStateObj.SetActive(false);
        contentPane.expTf.gameObject.SetActive(true);
        contentPane.levelSliderText.text = UserDataProxy.inst.playerData.CurrExp.ToString() + "/" + UserDataProxy.inst.playerData.MaxExp.ToString();
        contentPane.levelSlider.maxValue = UserDataProxy.inst.playerData.MaxExp;
        contentPane.levelSlider.value = Mathf.Max(UserDataProxy.inst.playerData.MaxExp * 0.05f, UserDataProxy.inst.playerData.CurrExp);
    }

    string getUnionJobTx(EUnionJob eUnionJob)
    {
        string result = "";
        switch (eUnionJob)
        {
            case EUnionJob.Common:
                result = "成员";
                break;
            case EUnionJob.Manager:
                result = "管理员";
                break;
            case EUnionJob.President:
                result = "会长";
                break;
        }

        return LanguageManager.inst.GetValueByKey(result);
    }

    int vipTimerId;
    public void ShowSelfInfo()
    {
        _data = null;
        var data = UserDataProxy.inst.playerData;

        contentPane.impeachPresidentBtn.gameObject.SetActive(false);

        contentPane.vipBtn.interactable = true;

        contentPane.vipBtn.gameObject.SetActiveTrue();
        GUIHelper.SetSingleUIGray(contentPane.vipBtn.transform, (K_Vip_State)data.vipState != K_Vip_State.Vip);

        contentPane.vipText.enabled = true;
        if ((K_Vip_State)data.vipState != K_Vip_State.Vip)
        {
            contentPane.vipText.text = LanguageManager.inst.GetValueByKey("未开通");
            contentPane.vipText.color = Color.white;
        }
        else
        {
            contentPane.vipText.color = GUIHelper.GetColorByColorHex("#ffdf2d");
            int vipRemainTime = HotfixBridge.inst.GetVipRemainTime();

            contentPane.vipText.text = TimeUtils.timeSpanStrip(vipRemainTime);

            if (vipTimerId > 0)
            {
                GameTimer.inst.RemoveTimer(vipTimerId);
                vipTimerId = 0;
            }

            vipTimerId = GameTimer.inst.AddTimer(1, vipRemainTime, () =>
              {
                  if (vipRemainTime <= 0)
                  {
                      contentPane.vipText.text = LanguageManager.inst.GetValueByKey("未开通");
                      GameTimer.inst.RemoveTimer(vipTimerId);
                      vipTimerId = 0;
                  }
                  else
                  {
                      vipRemainTime -= 1;
                      contentPane.vipText.text = TimeUtils.timeSpanStrip(vipRemainTime);
                  }
              });
        }

        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(data.playerName);
        contentPane.uidText.text = "#" + data.userUid;
        //contentPane.nameTxBg.sizeDelta = new Vector2(Mathf.Max(388, contentPane.nameText.preferredWidth + 118), 86);
        contentPane.levelText.text = data.level.ToString();

        contentPane.changeNameBtn.gameObject.SetActive(true);
        contentPane.selfObj.gameObject.SetActive(true);
        contentPane.unionMemberObj.gameObject.SetActive(false);
        contentPane.visitTF.gameObject.SetActive(false);

        setSelfExpInfo();

        if (!data.hasUnion) //没有公会
        {
            contentPane.unionInfoObj.SetActive(false);
        }
        else
        {
            contentPane.unionInfoObj.SetActive(true);

            contentPane.unionLvTx.text = UserDataProxy.inst.UnionLevel.ToString();
            contentPane.unionNameTx.text = data.unionName;
            contentPane.unionJobTx.text = getUnionJobTx((EUnionJob)data.memberJob);
        }

        contentPane.worthTx.text = data.worth.ToString("N0");
        contentPane.investTx.text = data.invest.ToString("N0");
        contentPane.helpTx.text = data.unionHelpCount.ToString("N0");
        contentPane.rewardTx.text = data.unionTaskCrownCount.ToString("N0");
        contentPane.monsterTx.text = data.masterCount.ToString("N0");


        initRoleTrans();
    }

    public void RefreshUIData(PlayerInfoData data)
    {
        _data = data;

        contentPane.impeachPresidentBtn.gameObject.SetActive(false);

        contentPane.nameText.text = LanguageManager.inst.GetValueByKey(data.userData.nickName);
        contentPane.uidText.text = "#" + data.userUid;
        //contentPane.nameTxBg.sizeDelta = new Vector2(Mathf.Max(388, contentPane.nameText.preferredWidth + 118), 86);
        contentPane.levelText.text = data.userData.level.ToString();

        GUIHelper.SetSingleUIGray(contentPane.vipBtn.transform, (K_Vip_State)data.vipInfo.state != K_Vip_State.Vip);

        contentPane.playerStateObj.SetActive(data.userUid != UserDataProxy.inst.playerData.userUid);

        //经验
        if (data.userUid == UserDataProxy.inst.playerData.userUid) //为自己
        {
            contentPane.vipText.enabled = true;
            contentPane.vipBtn.interactable = true;
            contentPane.changeNameBtn.gameObject.SetActive(true);
            contentPane.selfObj.gameObject.SetActive(true);
            contentPane.unionMemberObj.gameObject.SetActive(false);
            contentPane.visitTF.gameObject.SetActive(false);
            if ((K_Vip_State)data.vipInfo.state != K_Vip_State.Vip)
            {
                contentPane.vipText.text = LanguageManager.inst.GetValueByKey("未开通");
                contentPane.vipText.color = Color.white;
            }
            else
            {
                contentPane.vipText.color = GUIHelper.GetColorByColorHex("#ffdf2d");
                int vipRemainTime = HotfixBridge.inst.GetVipRemainTime();

                contentPane.vipText.text = TimeUtils.timeSpanStrip(vipRemainTime);

                if (vipTimerId > 0)
                {
                    GameTimer.inst.RemoveTimer(vipTimerId);
                    vipTimerId = 0;
                }

                vipTimerId = GameTimer.inst.AddTimer(1, vipRemainTime, () =>
                {
                    if (vipRemainTime <= 0)
                    {
                        contentPane.vipText.text = LanguageManager.inst.GetValueByKey("到期");
                        GameTimer.inst.RemoveTimer(vipTimerId);
                        vipTimerId = 0;
                    }
                    else
                    {
                        vipRemainTime -= 1;
                        contentPane.vipText.text = TimeUtils.timeSpanStrip(vipRemainTime);
                    }
                });
            }

            setSelfExpInfo();

        }
        else if (data.unionData.unionId != "" && data.unionData.unionId == UserDataProxy.inst.playerData.unionId)//为公会成员
        {
            contentPane.vipText.enabled = false;
            contentPane.vipBtn.interactable = false;
            contentPane.expTf.gameObject.SetActive(false);
            contentPane.selfObj.gameObject.SetActive(false);
            contentPane.unionMemberObj.gameObject.SetActive(true);
            contentPane.changeNameBtn.gameObject.SetActive(false);
            contentPane.visitTF.gameObject.SetActive(ManagerBinder.inst.mGameState == kGameState.VisitShop ? false : true);
            contentPane.playerStateText.text = data.lastActiveTime <= 0 ? LanguageManager.inst.GetValueByKey("在线") : TimeUtils.pasttimeSpanStrip(data.lastActiveTime);

            //弹劾
            int impeachPresidentTime = 72 * 60 * 60; //默认72小时
            var worldParConfig = WorldParConfigManager.inst.GetConfig(351);
            if (worldParConfig != null) impeachPresidentTime = (int)worldParConfig.parameters;

            if (data.unionData.memberJob == (int)EUnionJob.President) //是会长 就显示弹劾按钮
            {
                impeachPresidentTimeEnough = data.lastActiveTime >= impeachPresidentTime;
                contentPane.impeachPresidentBtn.gameObject.SetActive(true);
                GUIHelper.SetUIGray(contentPane.impeachPresidentBtn.transform, !impeachPresidentTimeEnough);
            }

        }
        else //路人
        {
            contentPane.vipText.enabled = false;
            contentPane.vipBtn.interactable = false;
            contentPane.expTf.gameObject.SetActive(false);
            contentPane.changeNameBtn.gameObject.SetActive(false);
            contentPane.unionMemberObj.gameObject.SetActive(false);
            contentPane.selfObj.gameObject.SetActive(false);
            contentPane.visitTF.gameObject.SetActive(ManagerBinder.inst.mGameState == kGameState.VisitShop ? false : true);

            contentPane.playerStateText.text = data.lastActiveTime <= 0 ? LanguageManager.inst.GetValueByKey("在线") : TimeUtils.pasttimeSpanStrip(data.lastActiveTime);
        }

        //公会
        if (data.unionData.unionId == "")
        {
            contentPane.unionInfoObj.SetActive(false);
        }
        else
        {
            contentPane.unionInfoObj.SetActive(true);

            contentPane.unionLvTx.text = data.unionData.unionLevel.ToString();
            contentPane.unionNameTx.text = data.unionData.unionName;
            contentPane.unionJobTx.text = getUnionJobTx((EUnionJob)data.unionData.memberJob);
        }


        //个人
        contentPane.worthTx.text = data.userData.worth.ToString("N0");
        contentPane.investTx.text = data.userData.invest.ToString("N0");
        contentPane.helpTx.text = data.userData.unionHelpCount.ToString("N0");
        contentPane.rewardTx.text = data.userData.unionTaskCrownCount.ToString("N0");
        contentPane.monsterTx.text = data.userData.masterCount.ToString("N0");


        initRoleTrans();
    }

    ////刷新公会职位
    //public void RefreshUnionMemeberJob(string userId, UnionMember unionData) 
    //{
    //    if (_data.userUid != userId) return;

    //    if (unionData.unionId == "") //没有公会
    //    {
    //        contentPane.unionInfoObj.SetActive(false);
    //    }
    //    else
    //    {
    //        contentPane.unionInfoObj.SetActive(true);

    //        contentPane.unionLvTx.text = unionData.unionLevel.ToString();
    //        contentPane.unionNameTx.text = unionData.unionName;
    //        contentPane.unionJobTx.text = getUnionJobTx((EUnionJob)unionData.);
    //    }

    //}

    private void initRoleTrans()
    {
        RoleDress roleDress = null;
        EGender gender = EGender.Female;

        int petModelId = -1;

        if (_data == null) //自己
        {
            roleDress = UserDataProxy.inst.playerData.userDress;
            gender = (EGender)UserDataProxy.inst.playerData.gender;

            if (UserDataProxy.inst.playerData.hasMainPet)
            {
                petModelId = PetDataProxy.inst.GetPetDataByPetUid(UserDataProxy.inst.playerData.mainPetUid).petCfg.model;
            }

        }
        else
        {
            roleDress = _data.userData.userDress;
            gender = (EGender)_data.userData.gender;

            if (_data.petInfo.petId != 0)
            {
                petModelId = PetConfigManager.inst.GetConfig(_data.petInfo.petId).model;
            }

        }

        if (dressUpSystem == null)
        {
            CharacterManager.inst.GetCharacter<DressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath(gender), SpineUtils.RoleDressToUintList(roleDress), gender, callback: (system) =>
            {
                dressUpSystem = system;
                dressUpSystem.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
                dressUpSystem.SetDirection(RoleDirectionType.Left);
                string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)dressUpSystem.gender, (int)kIndoorRoleActionType.normal_standby);
                dressUpSystem.Play(idleAnimationName, true);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(dressUpSystem, CharacterManager.inst.GetPeopleShapeNudeSpinePath(gender), SpineUtils.RoleDressToUintList(roleDress), gender);
            dressUpSystem.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
            string idleAnimationName = IndoorRoleActionConfigManager.inst.GetRandomAction((int)dressUpSystem.gender, (int)kIndoorRoleActionType.normal_standby);
            dressUpSystem.Play(idleAnimationName, true);
        }


        if (petModelId == -1)
        {
            hasPet = false;
            if (petDressUpSystem != null) petDressUpSystem.SetActive(false);
            contentPane.roleTrans.localPosition = Vector3.zero;
        }
        else
        {
            contentPane.roleTrans.localPosition = Vector3.left * 150;

            if (petDressUpSystem == null)
            {
                CharacterManager.inst.GetCharacterByModel<DressUpSystem>(petModelId, callback: (system) =>
                {
                    petDressUpSystem = system;
                    petDressUpSystem.SetUIPosition(contentPane.petTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
                    petDressUpSystem.SetDirection(RoleDirectionType.Left);
                    petDressUpSystem.Play("idle", true);
                    hasPet = true;
                });
            }
            else
            {
                CharacterManager.inst.ReSetCharacterByModel(petDressUpSystem, petModelId);
                petDressUpSystem.SetUIPosition(contentPane.petTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
                petDressUpSystem.Play("idle", true);
                hasPet = true;
            }
        }


        //if (headGraphicSystem == null)
        //{
        //    CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath(gender), SpineUtils.RoleDressToHeadDressIdList(roleDress), gender, callback: (system) =>
        //    {
        //        headGraphicSystem = system;
        //        system.transform.SetParent(contentPane.headParent);
        //        system.transform.localScale = Vector3.one * 0.42f;
        //        system.transform.localPosition = Vector3.down * 274f;
        //    });
        //}
        //else
        //{
        //    CharacterManager.inst.ReSetCharacter(headGraphicSystem, CharacterManager.inst.GetPeopleShapeNudeSpinePath(gender), SpineUtils.RoleDressToHeadDressIdList(roleDress), gender);
        //}
    }

    public override void shiftIn()
    {
        base.shiftIn();
        if (_data != null)
        {
            if (_data.userUid == UserDataProxy.inst.playerData.userUid)
            {
                ShowSelfInfo();
            }
            else
            {
                RefreshUIData(_data);
            }
        }
        else
        {
            ShowSelfInfo();
        }
    }

    protected override void onShown()
    {
        base.onShown();

        timerId_petAnim = GameTimer.inst.AddTimer(8, () =>
        {

            if (hasPet)
            {
                petDressUpSystem.Play(animStrs[Random.Range(0, animStrs.Length)], completeDele: (t) =>
                  {
                      petDressUpSystem.Play("idle", true);
                  });
            }

        });

    }

    protected override void onHide()
    {
        base.onHide();

        impeachPresidentTimeEnough = false;
        contentPane.impeachPresidentGemAffirmObj.SetActive(false);
        contentPane.impeachPresidentBtn.gameObject.SetActive(false);

        if (timerId_petAnim != 0)
        {
            GameTimer.inst.RemoveTimer(timerId_petAnim);
            timerId_petAnim = 0;
        }

        if (vipTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(vipTimerId);
            vipTimerId = 0;
        }
    }
}
