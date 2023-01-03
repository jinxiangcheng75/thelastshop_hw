using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using Mosframe;

public class CreatRoleView : ViewBase<CreatRoleComp>
{
    public override string viewID => ViewPrefabName.CreatRolePanel;
    public override string sortingLayerName => "window";

    RoleSubItemComp lastItem;
    List<RoleSubTypeData> needDatas = new List<RoleSubTypeData>();

    EGender curGender = EGender.Male;
    FacadeType curType = FacadeType.ModelColor;
    // 创建角色 男性女性
    private DressUpSystem roleMan;
    private DressUpSystem roleWoman;
    private DressUpSystem curRole;
    int index = -1;

    private string creatName;
    protected override void onInit()
    {
        base.onInit();

        InitData();
        AddUIEvent();
        CreatRoleObj();
        sexSelectChange(EGender.Male);
    }

    protected override void onShown()
    {
        base.onShown();

        roleMan.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 1);
        roleWoman.SetUIPosition(contentPane.roleTrans, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder - 1);
        roleWoman.Play("idle_1", true);

        SwitchSex();

        FGUI.inst.StartCoroutine(MoveToComplete());
    }

    private void InitData()
    {
        contentPane.subTypeGroup.OnSelectedIndexValueChange = subTypeSelectChange;
        contentPane.scrollView.itemRenderer = listitemRenderer;
        contentPane.scrollView.itemUpdateInfo = listitemRenderer;
        contentPane.scrollView.scrollByItemIndex(0);
    }

    int listItemCount = 0;
    private void listitemRenderer(int index, IDynamicScrollViewItem obj)
    {
        BtnList itemScript = (BtnList)obj;
        for (int i = 0; i < 4; ++i)
        {
            int itemIndex = index * 4 + i;
            var item = itemScript.buttonList[i].GetComponent<RoleSubItemComp>();
            item.clearAnim();
            if (itemIndex >= listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
                continue;
            }

            if (itemIndex < listItemCount)
            {
                itemScript.buttonList[i].gameObject.SetActive(true);

                item.InitData(needDatas[itemIndex], SmallItemClickHandler, itemIndex);
            }
            else
            {
                itemScript.buttonList[i].gameObject.SetActive(false);
            }
        }
    }

    private void SmallItemClickHandler(RoleSubItemComp item, bool isAuto)
    {
        if (lastItem != null && item.Data.config.type_2 == lastItem.Data.config.type_2)
        {
            lastItem.isSelect.SetActive(false);
            lastItem.highLightObj.SetActive(false);
            CreatRoleProxy.inst.ChangeDataList(curGender, lastItem.Data.config.id, false);
        }

        lastItem = new RoleSubItemComp();
        lastItem = item;

        if (isAuto)
        {
            //item.btnAnim.Play("click");
            float animTime = item.btnAnim.GetClipLength("ShopkeeperItem_click") + item.btnAnim.GetClipLength("ShopkeeperItem_ClickUp");
            GameTimer.inst.AddTimer(animTime * 0.5f, 1, () =>
            {
                item.isSelect.SetActive(true);
                item.highLightObj.SetActive(true);
                CreatRoleProxy.inst.ChangeDataList(curGender, item.Data.config.id, true);
                curRole.SwitchClothingByCfg(item.Data.config);
            });
            GameTimer.inst.AddTimer(animTime, 1, () =>
            {
                item.btnAnim.Play("Normal");
            });
        }
        else
        {
            item.isSelect.SetActive(true);
            item.highLightObj.SetActive(true);
            CreatRoleProxy.inst.ChangeDataList(curGender, item.Data.config.id, true);
            curRole.SwitchClothingByCfg(item.Data.config);
        }
    }

    private void SetListItemTotalCount(int count)
    {
        listItemCount = count;
        if (listItemCount < 0)
        {
            listItemCount = 0;
        }
        int count1 = listItemCount / 4;
        if (listItemCount % 4 > 0)
        {
            count1++;
        }
        contentPane.scrollView.totalItemCount = count1;
    }


    EGender lastGender;
    private void subTypeSelectChange(int index)
    {
        //if (contentPane.subTypeGroup.NotNeedInvokeAction)
        //{
        //    contentPane.subTypeGroup.NotNeedInvokeAction = false;
        //    return;
        //}

        var toggle = contentPane.subTypeGroup.togglesBtn[index];

        int selectedIndex = toggle.GetComponent<ItemSubType>().subType;

        if (selectedIndex == this.index && lastGender == curGender) return;

        this.index = selectedIndex;


        curType = (FacadeType)this.index;

        contentPane.titleText.text = LanguageManager.inst.GetValueByKey(StaticConstants.types[(uint)curType - 1]);
        needDatas = CreatRoleProxy.inst.GetCurSexTypeSubDatas(curType, curGender);
        lastGender = curGender;

        SetListItemTotalCount(needDatas.Count);
        contentPane.scrollView.ScrollToTop();
    }

    private void sexSelectChange(EGender gender)
    {
        curGender = gender;
        SwitchSex();
        contentPane.maleSelect.SetActive(curGender == EGender.Male);
        contentPane.femaleSelect.SetActive(curGender == EGender.Female);
        contentPane.sexIcon.SetSprite("dressup_atlas", curGender == EGender.Male ? "huanzhuang_xuanzenan" : "huanzhuang_xuanzenv");
        lastItem = null;
    }

    // 切换性别
    private void SwitchSex()
    {
        roleMan.SetActive(curGender == EGender.Male);
        roleWoman.SetActive(curGender == EGender.Female);
        curRole = curGender == EGender.Male ? roleMan : roleWoman;
    }

    private void CreatRoleObj()
    {
        var shopkeeperManDefaultDresses = CharacterModelConfigManager.inst.GetConfig(10001).ToDressIds();
        var shopkeeperWomanDefaultDresses = CharacterModelConfigManager.inst.GetConfig(20001).ToDressIds();

        roleMan = CharacterManager.inst.CreatRuntimeAssetsAndGameObjectSync<DressUpSystem>(contentPane.shopkeeperManAss);
        roleMan.Init(EGender.Male, CharacterManager.inst.GetPeopleShapeNudeSpinePath(EGender.Male), shopkeeperManDefaultDresses);
        roleMan.gameObject.name = "Man";
        roleMan.SetDirection(RoleDirectionType.Right);
        roleMan.SetActive(false);

        roleWoman = CharacterManager.inst.CreatRuntimeAssetsAndGameObjectSync<DressUpSystem>(contentPane.shopkeeperWomanAss);
        roleWoman.Init(EGender.Female, CharacterManager.inst.GetPeopleShapeNudeSpinePath(EGender.Female), shopkeeperWomanDefaultDresses);
        roleWoman.gameObject.name = "Woman";
        roleWoman.SetDirection(RoleDirectionType.Right);
        roleWoman.SetActive(false);


        //男性
        for (int i = 0; i < shopkeeperManDefaultDresses.Count; i++)
        {
            CreatRoleProxy.inst.ChangeDataList(EGender.Male, shopkeeperManDefaultDresses[i], true);
        }


        //女性
        for (int i = 0; i < shopkeeperWomanDefaultDresses.Count; i++)
        {
            CreatRoleProxy.inst.ChangeDataList(EGender.Female, shopkeeperWomanDefaultDresses[i], true);
        }
    }

    private void AddUIEvent()
    {
        // 窗口
        contentPane.randomButton.onClick.AddListener(() =>
        {
            PlatformManager.inst.GameHandleEventLog("RandomSelect", "");
            float animTime = contentPane.randomButton.GetComponent<Animator>().GetClipLength("Pressed");
            GameTimer.inst.AddTimer(animTime * 1.25f, 1, () =>
            {
                RandomClothe();
            });
        });
        contentPane.exteriorButton.onClick.AddListener(() =>
        {
            PlatformManager.inst.GameHandleEventLog("Customize", "");
            float animTime = contentPane.exteriorButton.GetComponent<Animator>().GetClipLength("Pressed");
            GameTimer.inst.AddTimer(animTime * 1.25f, 1, () =>
            {
                OpenPopupPanel();
                if (GameSettingManager.inst.needShowUIAnim)
                    contentPane.proAnimator.Play("show");
            });
        });
        contentPane.startGameButton.onClick.AddListener(() =>
        {
            PlatformManager.inst.GameHandleEventLog("Play", "");
            float animTime = contentPane.startGameButton.GetComponent<Animator>().GetClipLength("Pressed");
            GameTimer.inst.AddTimer(animTime * 1.25f, 1, () =>
            {
                EnterCreatName();
            });
        });
        contentPane.maleButton.onClick.AddListener(() =>
        {
            float animTime = contentPane.maleButton.GetComponent<Animator>().GetClipLength("Pressed");
            GameTimer.inst.AddTimer(animTime * 1.25f, 1, () =>
            {
                sexSelectChange(EGender.Male);
            });
        });
        contentPane.femaleButton.onClick.AddListener(() =>
        {
            float animTime = contentPane.femaleButton.GetComponent<Animator>().GetClipLength("Pressed");
            GameTimer.inst.AddTimer(animTime * 1.25f, 1, () =>
            {
                sexSelectChange(EGender.Female);
            });
        });

        // 关闭外观按钮
        contentPane.closeButton.onClick.AddListener(() =>
        {
            float animTime = contentPane.closeButton.GetComponent<Animator>().GetClipLength("Pressed");
            GameTimer.inst.AddTimer(animTime * 1.25f, 1, () =>
            {
                if (GameSettingManager.inst.needShowUIAnim)
                {
                    contentPane.proAnimator.Play("hide");
                    var animInfo = contentPane.proAnimator.GetClipLength("ShopkeeperSub_hide");
                    GameTimer.inst.AddTimer(animInfo, 1, () =>
                    {
                        closeCreatRolePanel();
                    });
                }
                else
                    closeCreatRolePanel();
            });
        });

        // 输入姓名
        contentPane.nameCloseButton.onClick.AddListener(() =>
        {
            float animTime = contentPane.nameCloseButton.GetComponent<Animator>().GetClipLength("Pressed");
            GameTimer.inst.AddTimer(animTime * 1.25f, 1, () =>
            {
                if (GameSettingManager.inst.needShowUIAnim)
                {
                    contentPane.creatNameAnimator.Play("hide");
                    float hideTime = contentPane.creatNameAnimator.GetClipLength("common_popUpUI_hide");
                    GameTimer.inst.AddTimer(hideTime, 1, () =>
                    {
                        contentPane.nameInput.text = "";
                        contentPane.creatNameObj.SetActive(false);
                        curRole.Play(UnityEngine.Random.Range(0, 2) == 0 ? "idle_1" : "idle_2", true);
                    });
                }
                else
                {
                    contentPane.nameInput.text = "";
                    contentPane.creatNameObj.SetActive(false);
                    curRole.Play(UnityEngine.Random.Range(0, 2) == 0 ? "idle_1" : "idle_2", true);
                }
            });
        });

        contentPane.nameCloseBtn.ButtonClickTween(() =>
        {
            if (GameSettingManager.inst.needShowUIAnim)
            {
                contentPane.creatNameAnimator.Play("hide");
                float hideTime = contentPane.creatNameAnimator.GetClipLength("common_popUpUI_hide");
                GameTimer.inst.AddTimer(hideTime, 1, () =>
                {
                    contentPane.nameInput.text = "";
                    contentPane.creatNameObj.SetActive(false);
                    curRole.Play(UnityEngine.Random.Range(0, 2) == 0 ? "idle_1" : "idle_2", true);
                });
            }
            else
            {
                contentPane.nameInput.text = "";
                contentPane.creatNameObj.SetActive(false);
                curRole.Play(UnityEngine.Random.Range(0, 2) == 0 ? "idle_1" : "idle_2", true);
            }
        });

        contentPane.nameRandomBtn.onClick.AddListener(onNameRandomBtnClick);

        contentPane.nameEnterButton.onClick.AddListener(() =>
        {
            PlatformManager.inst.GameHandleEventLog("Role", "");
            float animTime = contentPane.nameEnterButton.GetComponent<Animator>().GetClipLength("Pressed");
            GameTimer.inst.AddTimer(animTime * 1.25f, 1, () =>
            {
                EnterGame();
            });
        });

        contentPane.nameInput.onValueChanged.AddListener((str) =>
        {
            for (int i = 0; i < CreatRoleProxy.inst.sensitiveWords.Length; i++)
            {
                if (str.Contains(CreatRoleProxy.inst.sensitiveWords[i].ToString()))
                {
                    int index = str.IndexOf(CreatRoleProxy.inst.sensitiveWords[i]);
                    str = str.Remove(index);
                    contentPane.nameInput.text = str;
                }
            }
        });
    }

    private void OpenPopupPanel()
    {
        contentPane.pupopObj.SetActive(true);

        BtnStateChange(false);

        for (int i = 0; i < contentPane.subTypeGroup.togglesBtn.Count; i++)
        {
            contentPane.subTypeGroup.togglesBtn[i].gameObject.SetActive(true);
        }
        int startIndex = 0;
        int endIndex = 0;
        int initIndex = 0;
        if (curGender == EGender.Male)
        {
            startIndex = 6;
            endIndex = 12;
            initIndex = 0;
        }
        else
        {
            startIndex = 0;
            endIndex = 6;
            initIndex = 6;
        }

        for (int i = startIndex; i < endIndex; i++)
        {
            contentPane.subTypeGroup.togglesBtn[i].gameObject.SetActive(false);
            contentPane.subTypeGroup.togglesBtn[i].isOn = false;
        }

        contentPane.subTypeGroup.selectedIndex = initIndex;
        subTypeSelectChange(initIndex);
    }

    private void onNameRandomBtnClick()
    {
        contentPane.nameInput.text = LanguageManager.inst.GetValueByKey(NameConfigManager.inst.GetRandomName(curGender));
    }

    // 进入游戏 进行网络消息发送
    private void EnterGame()
    {
        creatName = contentPane.nameInput.text;
        if (string.IsNullOrEmpty(creatName))
        {
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称不能为空"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else if (WordFilter.inst.filter(contentPane.nameInput.text, out creatName))
        {
            contentPane.nameInput.text = "";
            EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("名称中包含敏感词汇！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
        else
        {
            BtnStateChange(false);
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_User_Create()
                {
                    nickName = creatName,
                    gender = (int)curGender,
                    userDress = curRole.curDress
                }
            });
            contentPane.creatNameObj.SetActive(false);
        }
    }

    private void RandomClothe()
    {
        if (curRole.isInDressing) return; //正在换装 不做操作

        List<int> randomDressIds = new List<int>();

        for (int i = 0; i < StaticConstants.roleCreatIndex.Length; i++)
        {
            randomDressIds.Add(CreatRoleProxy.inst.RandomGetTypeData((FacadeType)StaticConstants.roleCreatIndex[i], curGender).config.id);
        }

        CreatRoleProxy.inst.InitSelectData(curGender);

        randomDressIds.AddRange(CharacterModelConfigManager.inst.GetConfig(curGender == EGender.Male ? 10001 : 20001).ToFacadeDressIds());

        for (int i = 0; i < randomDressIds.Count; i++)
        {
            CreatRoleProxy.inst.ChangeDataList(curGender, randomDressIds[i], true);
        }

        curRole.OverallClothing(randomDressIds);
    }

    private void EnterCreatName()
    {
        contentPane.creatNameObj.SetActive(true);
        onNameRandomBtnClick();
        if (GameSettingManager.inst.needShowUIAnim)
            contentPane.creatNameAnimator.Play("show");
        curRole.SetAnimationSpeed(0);
    }

    private void closeCreatRolePanel()
    {
        contentPane.pupopObj.SetActive(false);
        BtnStateChange(true);
    }

    private void BtnStateChange(bool state)
    {
        contentPane.maleButton.gameObject.SetActive(state);
        contentPane.femaleButton.gameObject.SetActive(state);
        contentPane.randomButton.enabled = state;
        contentPane.exteriorButton.enabled = state;
        contentPane.startGameButton.enabled = state;
        GUIHelper.SetUIGray(contentPane.startGameButton.transform, !state);
    }

    IEnumerator MoveToComplete()
    {
        roleMan.Play("walk", true);
        BtnStateChange(false);
        while (Vector3.Distance(contentPane.moveToTrans.position, contentPane.roleTrans.position) >= 0.05f)
        {
            if (roleMan.isInDressing) yield return null;

            contentPane.roleTrans.position = Vector3.MoveTowards(contentPane.roleTrans.position, contentPane.moveToTrans.position, Time.deltaTime * 2f);
            yield return null;
        }

        roleMan.Play(UnityEngine.Random.Range(0, 1) == 0 ? "idle_1" : "idle_2", true);
        BtnStateChange(true);
    }

    protected override void onHide()
    {
        if (roleMan != null)
            GameObject.Destroy(roleMan.gameObject);
        if (roleWoman != null)
            GameObject.Destroy(roleWoman.gameObject);
        roleMan = null;
        roleWoman = null;
        curRole = null;
    }

    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.windowAnimator.CrossFade("show", 0f);
        contentPane.windowAnimator.Update(0f);
        contentPane.windowAnimator.Play("show");
        var animInfo = contentPane.windowAnimator.GetCurrentAnimatorStateInfo(0);
        GameTimer.inst.AddTimer(animInfo.length, 1, () =>
        {
            contentPane.windowAnimator.enabled = false;
        });
    }
}
