using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using System;

public class SpineTestUI : ViewBase<SpineTestUIComp>
{
    public override string sortingLayerName => "window";
    public override string viewID => ViewPrefabName.SpineTestUI;

    private DressUpSystem man, woman;

    public int sexIndex;

    private bool modelToggleIsOn;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = false;
        contentPane.closeBtn.onClick.AddListener(hide);

        CharacterManager.inst.GetCharacterByModel<DressUpSystem>(30001, callback: (man) =>
        {
            this.man = man;
            this.man.SetSortingAndOrderLayer("window", 1);
            contentPane.aniDropDown.ClearOptions();
            var animations = this.man.Skeleton.Data.Animations;

            animations.ForEach((item) =>
            {
                contentPane.aniDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData(item.Name));

                var animBtnObj = GameObject.Instantiate(contentPane.animBtnPfb.gameObject, contentPane.content);

                Button animBtn = animBtnObj.GetComponent<Button>();
                Text text = animBtnObj.GetComponentInChildren<Text>();

                animBtn.name = text.text = item.Name;
                animBtn.onClick.AddListener(() => Play(item.Name));
            });
        });

        CharacterManager.inst.GetCharacterByModel<DressUpSystem>(40001, callback: (woman) =>
        {
            this.woman = woman;
            this.woman.SetSortingAndOrderLayer("window", 1);
            isInit = true;
        });

        contentPane.aniDropDown.onValueChanged.AddListener((index) =>
        {
            string aniName = contentPane.aniDropDown.options[index].text;
            Play(aniName);

        });

        contentPane.idle_1Btn.onClick.AddListener(() => Play("idle_1"));
        contentPane.idle_2Btn.onClick.AddListener(() => Play("idle_2"));
        contentPane.walkBtn.onClick.AddListener(() => Play("walk"));
        contentPane.happyBtn.onClick.AddListener(() => Play("happy"));

        contentPane.dressUpBtn.onClick.AddListener(onDressUpBtnClick);

        contentPane.toggleGroup.OnSelectedIndexValueChange = OnSelectedValueChange;

        contentPane.modelToggle.onValueChanged.AddListener((isOn) =>
        {
            modelToggleIsOn = isOn;
        });


        contentPane.attToAnotherSlotBtn.onClick.AddListener(onAttToAnotherSlotBtnClick);
        contentPane.slotRotBtn.onClick.AddListener(onSlotRotBtnClick);
        contentPane.slotPosBtn.onClick.AddListener(onSlotPosBtnClick);
        contentPane.slotScaleBtn.onClick.AddListener(onSlotScaleBtnClick);

        contentPane.leftBtn.onClick.AddListener(() => changeDirection(true));
        contentPane.rightBtn.onClick.AddListener(() => changeDirection(false));


    }

    private void changeDirection(bool isLeft)
    {
        RoleDirectionType direction = isLeft ? RoleDirectionType.Left : RoleDirectionType.Right;
        man?.SetDirection(direction);
        woman?.SetDirection(direction);
    }

    protected override void onShown()
    {
        base.onShown();
        this.man?.SetUIPosition(contentPane.manTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
        this.man?.Play("idle_1", true);
        this.woman?.SetUIPosition(contentPane.womanTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
        this.woman?.Play("idle_1", true);
        contentPane.modelToggle.isOn = false;
        modelToggleIsOn = false;

        contentPane.attToAnotherSlotInput.text = "woman_shield_front woman_shield_front2";
        contentPane.slotRotInput.text = "woman_shield_front2 -150";
        contentPane.slotPosInput.text = "woman_shield_front2 1.4,5";
        contentPane.slotScaleInput.text = "woman_shield_front2 1,1";

    }

    public override void DefineTextFont()
    {
    }

    void bothShow()
    {
        (contentPane.manTf as RectTransform).anchoredPosition = Vector3.left * 200 + Vector3.down * 75;
        (contentPane.womanTf as RectTransform).anchoredPosition = Vector3.right * 300 + Vector3.down * 75;
        this.man?.SetUIPosition(contentPane.manTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
        this.woman?.SetUIPosition(contentPane.womanTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
    }

    void justGrilShow()
    {
        (contentPane.womanTf as RectTransform).anchoredPosition = Vector3.down * 75;
        this.woman?.SetUIPosition(contentPane.womanTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
        this.man?.SetActive(false);
    }

    void justBoyShow()
    {
        (contentPane.manTf as RectTransform).anchoredPosition = Vector3.down * 75;
        this.man?.SetUIPosition(contentPane.manTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1);
        this.woman?.SetActive(false);
    }

    private void OnSelectedValueChange(int index)
    {
        AudioManager.inst.PlaySound(11);
        this.sexIndex = index;
        switch (index)
        {
            case 0: bothShow(); break;
            case 1: justGrilShow(); break;
            case 2: justBoyShow(); break;
        }
    }


    private void onDressUpBtnClick()
    {
        int dressId;
        if (int.TryParse(contentPane.equipInpuField.text, out dressId))
        {
            if (modelToggleIsOn)
            {
                if (sexIndex != 1)
                {
                    CharacterManager.inst.ReSetCharacterByModel(this.man, dressId, repackedCallback: (system) =>
                      {
                          system.Repacked("");
                      });
                }
                if (sexIndex != 2)
                {
                    CharacterManager.inst.ReSetCharacterByModel(this.woman, dressId, repackedCallback: (system) =>
                    {
                        system.Repacked("");
                    });
                }
            }
            else
            {
                var dressCfg = dressconfigManager.inst.GetConfig(dressId);

                if (dressCfg != null)
                {
                    if (sexIndex != 1) this.man.SwitchClothingByCfg(dressCfg);
                    if (sexIndex != 2) this.woman.SwitchClothingByCfg(dressCfg);
                }
                else
                {
                    Logger.error("未找到对应dressid的换装配置 dressid：" + dressId);
                }
            }
        }
    }

    private void Play(string animName)
    {
        if (sexIndex != 1) this.man.Play(animName, true);
        if (sexIndex != 2) this.woman.Play(animName, true);
    }


    void onAttToAnotherSlotBtnClick()
    {
        string inputStr = contentPane.attToAnotherSlotInput.text.Trim();

        string[] strs = inputStr.Split(' ');

        if (strs.Length > 1)
        {
            string slotName1 = strs[0];
            string slotName2 = strs[1];

            if (sexIndex != 1) this.man.AttToAnotherSlot(slotName1, slotName2);
            if (sexIndex != 2) this.woman.AttToAnotherSlot(slotName1, slotName2);
        }
        else
        {
            Logger.error("格式不正确 请重试  示例  woman_pistol_front_r weapon_onehand");
            contentPane.attToAnotherSlotInput.text = "";
        }
    }

    void onSlotRotBtnClick()
    {
        string inputStr = contentPane.slotRotInput.text.Trim();

        string[] strs = inputStr.Split(' ');

        string slotName = strs[0];
        int slotRotVal = 0;

        if (strs.Length > 1 && int.TryParse(strs[1], out slotRotVal))
        {
            if (sexIndex != 1) this.man.SetSlotRot(slotName, slotRotVal);
            if (sexIndex != 2) this.woman.SetSlotRot(slotName, slotRotVal);
        }
        else
        {
            Logger.error("格式不正确 请重试  示例  weapon_onehand 55");
            contentPane.slotRotInput.text = "";
        }

    }


    void onSlotPosBtnClick()
    {
        string inputStr = contentPane.slotPosInput.text.Trim();

        string[] strs = inputStr.Split(' ');

        string slotName = strs[0];

        bool result = true;

        if (strs.Length > 1)
        {
            Vector2 pos = new Vector2();

            string[] posStrs = strs[1].Split(',');

            if (posStrs.Length > 1)
            {
                if (float.TryParse(posStrs[0], out float posX))
                {
                    pos.x = posX;
                }
                else
                {
                    result = false;
                }

                if (float.TryParse(posStrs[1], out float posY))
                {
                    pos.y = posY;
                }
                else
                {
                    result = false;
                }


                if (result)
                {
                    if (sexIndex != 1) this.man.SetSlotPos(slotName, pos);
                    if (sexIndex != 2) this.woman.SetSlotPos(slotName, pos);
                }
            }

        }
        else
        {
            result = false;
        }


        if (!result)
        {
            Logger.error("格式不正确 请重试  示例  weapon_onehand 66,77");
        }
    }

    void onSlotScaleBtnClick()
    {
        string inputStr = contentPane.slotScaleInput.text.Trim();

        string[] strs = inputStr.Split(' ');

        string slotName = strs[0];

        bool result = true;

        if (strs.Length > 1)
        {
            Vector2 scale = new Vector2();

            string[] posStrs = strs[1].Split(',');

            if (posStrs.Length > 1)
            {
                if (float.TryParse(posStrs[0], out float scaleX))
                {
                    scale.x = scaleX;
                }
                else
                {
                    result = false;
                }

                if (float.TryParse(posStrs[1], out float scaleY))
                {
                    scale.y = scaleY;
                }
                else
                {
                    result = false;
                }

                if (result)
                {
                    if (sexIndex != 1) this.man.SetSlotScale(slotName, scale);
                    if (sexIndex != 2) this.woman.SetSlotScale(slotName, scale);
                }
            }

        }
        else
        {
            result = false;
        }


        if (!result)
        {
            Logger.error("格式不正确 请重试  示例  weapon_onehand 66,77");
        }
    }



}
