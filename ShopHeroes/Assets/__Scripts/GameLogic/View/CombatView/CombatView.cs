using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CombatView : ViewBase<CombatComp>
{
    public override string viewID => ViewPrefabName.CombatUI;
    public override string sortingLayerName => "window";

    private Dictionary<int, FighterCard> cardList = new Dictionary<int, FighterCard>();
    int sumWave = 0;
    int startWave = 0;
    int enemySumCount = 0;
    int enemyRemainCount = 0;
    int endWave = 0;
    int endRemainCount = 0;
    int curBattleType = 1;
    protected override void onInit()
    {
        isShowResPanel = false;
        base.onInit();
        contentPane.exitBtn.gameObject.SetActive(false);
        contentPane.exitBtn.onClick.AddListener(exitbtnOnclick);
        contentPane.speedUpBtn.onClick.AddListener(onspeedUpClick);
        // for (int i = 0; i < contentPane.fighterCards.Length; i++)
        // {
        //     contentPane.fighterCards[i].heroIcon.ClearTexture();
        //     contentPane.fighterCards[i].gameObject.SetActive(false);
        // }
    }

    public void setRound(int index)
    {
        contentPane.roundText.text = index + "/" + (int)WorldParConfigManager.inst.GetConfig(701).parameters;
    }
    public void setState(int state) // 1 - explore 2 - ruins
    {
        curBattleType = state;
        contentPane.ruinsObj.SetActive(state == 2);
        if (state == 2)
        {
            if (!FGUI.inst.isLandscape)
            {
                contentPane.leftTf.anchorMax = new Vector2(0, 1);
                contentPane.leftTf.anchorMin = new Vector2(0, 1);

                contentPane.leftTf.anchoredPosition = new Vector2(176, contentPane.leftTf.anchoredPosition.y);
            }

            var data = HotfixBridge.inst.GetRuinsBattleData();
            var bossCfg = MonsterConfigManager.inst.GetConfig(data.triggerType);

            if (bossCfg != null)
            {
                contentPane.ruinsBossIcon.SetSprite(bossCfg.monster_atlas, bossCfg.monster_icon);
            }

            enemySumCount = data.triggerCondition;
            enemyRemainCount = data.triggerVal;
            contentPane.ruinsScheduleText.text = Mathf.CeilToInt((float)(data.triggerCondition - data.triggerVal) / data.triggerCondition * 100) + "%";
            contentPane.ruinsSlider.maxValue = data.triggerCondition;
            contentPane.ruinsSlider.value = data.triggerCondition - data.triggerVal;
            //Logger.error("输出 初始 怪物总数" + enemySumCount + "      剩余数量" + enemyRemainCount);
            if (data.position != null)
            {
                var str = data.position.Split('/');
                if (str.Length >= 1)
                {
                    startWave = int.Parse(str[0]);
                }
                if (str.Length >= 2)
                {
                    sumWave = int.Parse(str[1]);
                }
                if (str.Length >= 3)
                {
                    endWave = int.Parse(str[2]);
                }
                if (str.Length >= 4)
                {
                    endRemainCount = int.Parse(str[3]);
                }
            }
            contentPane.ruinsWaveText.text = LanguageManager.inst.GetValueByKey("{0}波", (startWave + "/" + sumWave));
            //Logger.error("输出 初始 总波数" + sumWave + "        开始波数" + startWave);
        }
        else if (state == 1)
        {
            if (!FGUI.inst.isLandscape)
            {
                contentPane.leftTf.anchorMax = new Vector2(0.5f, 1);
                contentPane.leftTf.anchorMin = new Vector2(0.5f, 1);

                contentPane.leftTf.anchoredPosition = new Vector2(0, contentPane.leftTf.anchoredPosition.y);
            }
        }
    }

    public void refreshState(int curType, int curVal) // curType = 1 死亡怪物数量 curType = 2 波数
    {
        if (curBattleType != 2) return;
        if (curType == 1)
        {
            enemyRemainCount -= curVal;
            contentPane.ruinsSlider.value = enemySumCount - enemyRemainCount;
            contentPane.ruinsScheduleText.text = Mathf.CeilToInt((float)(enemySumCount - enemyRemainCount) / enemySumCount * 100) + "%";
            //Logger.error("输出 刷新 怪物总数量" + enemySumCount + "       剩余怪物" + enemyRemainCount);
        }
        else if (curType == 2)
        {
            contentPane.ruinsWaveText.text = LanguageManager.inst.GetValueByKey("{0}波", ((startWave + curVal - 1) + "/" + sumWave));
            //Logger.error("输出 刷新 总波数" + sumWave + "        当前波数" + (startWave + curVal - 1));
        }
    }

    public void endState(bool isFinish)
    {
        if (curBattleType != 2) return;

        if (endWave < startWave)
        {
            contentPane.ruinsWaveText.text = LanguageManager.inst.GetValueByKey("{0}波", (sumWave + "/" + sumWave));
            contentPane.ruinsSlider.value = enemySumCount;
            contentPane.ruinsScheduleText.text = "100%";

            //Logger.error("输出 结束 总波数" + sumWave + "     当前波数" + sumWave);
            //Logger.error("输出 结束 怪物总数" + enemySumCount + "       剩余怪物数量0");
        }
        else
        {
            if (endWave == startWave)
            {
                endWave = isFinish ? sumWave : endWave;
                endRemainCount = isFinish ? 0 : endRemainCount;
            }

            contentPane.ruinsWaveText.text = LanguageManager.inst.GetValueByKey("{0}波", (endWave + "/" + sumWave));
            contentPane.ruinsSlider.value = enemySumCount - endRemainCount;
            contentPane.ruinsScheduleText.text = Mathf.CeilToInt((float)(enemySumCount - endRemainCount) / enemySumCount * 100) + "%";

            //Logger.error("输出 结束2 总波数" + sumWave + "     当前波数" + endWave);
            //Logger.error("输出 结束2 怪物总数" + enemySumCount + "       剩余怪物数量" + endRemainCount);
        }
    }

    private void onspeedUpClick()
    {
        float speed = GameSettingManager.combatPlaySpeed;
        contentPane.speed_x1_Obj.SetActive(speed == GameSettingManager.combatDefaultSpeed * 3.0f);
        contentPane.speed_x3_Obj.SetActive(speed != GameSettingManager.combatDefaultSpeed * 3.0f);
        //contentPane.speedText.text = speed == 3.0f ? "3X" : "1X";
        EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_PLAYSPEED_CHANGE, speed == GameSettingManager.combatDefaultSpeed * 3.0f ? GameSettingManager.combatDefaultSpeed * 1.0f : GameSettingManager.combatDefaultSpeed * 3.0f);
    }
    private void exitbtnOnclick()
    {
        EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_USE_SKIP);
    }
    protected override void onShown()
    {
        EventController.inst.TriggerEvent(GameEventType.UI_TOPTESPANEL_ShiftOut, true);
        contentPane.exitBtn.gameObject.SetActive(false);
        float speed = GameSettingManager.combatPlaySpeed;
        contentPane.speed_x1_Obj.SetActive(speed != GameSettingManager.combatDefaultSpeed * 3.0f);
        contentPane.speed_x3_Obj.SetActive(speed == GameSettingManager.combatDefaultSpeed * 3.0f);
        //contentPane.speedText.text = speed == GameSettingManager.combatDefaultSpeed * 3.0f ? "1X" : "3X";
        contentPane.roundText.text = "";
        contentPane.speedUpBtn.gameObject.SetActive(false);
        contentPane.CardsTF.anchoredPosition = new Vector2(0, -400f);
    }

    public void showBtn()
    {
        contentPane.exitBtn.gameObject.SetActive(true);
        contentPane.speedUpBtn.gameObject.SetActive(true);
    }

    public void BattleWin()
    {
        contentPane.exitBtn.gameObject.SetActive(true);
    }
    public void shownExitBtn()
    {
        contentPane.exitBtn.gameObject.SetActive(true);
    }
    public void BattleFalied()
    {
        contentPane.exitBtn.gameObject.SetActive(true);
    }
    protected override void onHide()
    {
        // for (int i = 0; i < contentPane.fighterCards.Length; i++)
        // {
        //     //contentPane.fighterCards[i].heroIcon.ClearTexture();
        //     //   contentPane.fighterCards[i].gameObject.SetActive(false);
        // }
        curBattleType = 1;
        sumWave = 0;
        startWave = 0;
        enemySumCount = 0;
        enemyRemainCount = 0;
        endRemainCount = 0;
        endWave = 0;
        if (TakePhoto.inst != null)
        {
            TakePhoto.inst.clearTexture2d();
        }
    }
    public void InitFighter(List<CombatFighter> attackers)
    {
        cardList.Clear();
        for (int i = 0; i < contentPane.fighterCards.Length; i++)
        {
            var newCard = contentPane.fighterCards[i];
            if (newCard != null)
            {
                // newCard._angerAnim.SetTrigger("reset");
                // newCard._angerAnim.Play("nvqi_0", 0, 0);
                // newCard._angerAnim.speed = GameSettingManager.combatPlaySpeed;
                if (i < attackers.Count)
                {
                    if (attackers[i] == null)
                    {
                        Debug.LogError($"战斗数据异常，attackers[{i}]为空!");
                        continue;
                    }
                    newCard.professionIconBgIcon.gameObject.SetActive(true);
                    //newCard.gameObject.SetActive(true);
                    newCard.fighterkey = attackers[i].key;
                    newCard.nameText.text = LanguageManager.inst.GetValueByKey(attackers[i].name);
                    newCard.hpSlider.maxValue = attackers[i].hp;
                    newCard.hpSlider.value = attackers[i].currentHp;
                    newCard.hpText.text = $"{attackers[i].hp}/{attackers[i].currentHp}";
                    newCard.angerSlider.maxValue = attackers[i].anger;
                    newCard.angerSlider.value = attackers[i].currentAnger;
                    newCard.angerText.text = $"{attackers[i].anger}/{attackers[i].currentAnger}";
                    // newCard.heroIcon.setTexture(999000 + attackers[i].key, attackers[i].sex, SpineUtils.RoleDressToUintList(attackers[i].roleDress), attackers[i].equips);
                    newCard.showHeroHeadIcon(attackers[i].sex, SpineUtils.RoleDressToUintList(attackers[i].roleDress), attackers[i].equips);
                    newCard.levelText.text = attackers[i].level.ToString();
                    newCard.NoneMaskTF.gameObject.SetActive(false);
                    newCard.LevelTf.gameObject.SetActive(true);
                    int index = RoleDataProxy.inst.ReturnRarityByAptitude(attackers[i].aptitude);
                    newCard.iconBG.sprite = newCard.iconBGs[index];
                    newCard.iconFrame.sprite = newCard.iconFrames[index];
                    newCard.lvBg.sprite = newCard.lvBG[index];
                    HeroProfessionConfigData herocfg = HeroProfessionConfigManager.inst.GetConfig(attackers[i].job);
                    newCard.professionIconBgIcon.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[herocfg.type - 1]);
                    newCard.professionIcon.SetSprite(herocfg.atlas, herocfg.ocp_icon);
                    cardList.Add(newCard.fighterkey, newCard);
                    newCard.DeathTextTF.gameObject.SetActive(false);
                    newCard.setAngerBarValue(0);
                }
                else
                {
                    newCard.fighterkey = -1;
                    newCard.nameText.text = "";
                    newCard.hpSlider.value = 0;
                    newCard.angerSlider.value = 0;
                    newCard.levelText.text = "";
                    //newCard.heroIcon.ClearTexture();
                    newCard.NoneMaskTF.gameObject.SetActive(true);
                    newCard.LevelTf.gameObject.SetActive(false);
                    newCard.DeathTextTF.gameObject.SetActive(false);
                    newCard.professionIconBgIcon.gameObject.SetActive(false);
                    newCard.setAngerBarValue(0);
                }
                GUIHelper.SetUIGray(newCard.transform, false);
            }
            else
            {
                Debug.LogError($"战斗界面异常，fighterCards[{i}]为空!");
            }
        }
        shiftIn();
    }
    public void updateHp(int key, int chp)
    {
        if (cardList.ContainsKey(key))
        {
            var card = cardList[key];
            card.hpSlider.DOValue(chp, 0.5f);
            card.hpText.text = $"{chp}/{card.hpSlider.maxValue}";
            if (chp <= 0)
            {
                GUIHelper.SetUIGray(card.transform, true);
                card.DeathTextTF.gameObject.SetActive(true);
                card.setAngerBarValue(0);
            }
            else
            {
                card.DeathTextTF.gameObject.SetActive(false);
            }
        }
    }

    FighterCard currActionCard = null;
    public void currFighterCard(int key)
    {
        cardList.TryGetValue(key, out currActionCard);
        if (currActionCard != null)
        {
            // currActionCard.transform.DOLocalMoveY()
        }
    }

    public void updateAnger(int key, int chp)
    {

        if (cardList.ContainsKey(key))
        {
            var card = cardList[key];

            card.angerSlider.DOValue(chp, 0.5f);
            card.angerText.text = $"{chp}/{card.hpSlider.maxValue}";
            card.setAngerBarValue(chp / card.angerSlider.maxValue);
            if (card.angerSlider.value >= card.angerSlider.maxValue)
            {
                return;
            }
            else
            {
                if (chp >= card.angerSlider.maxValue)
                {
                    //怒气爆满
                    // card._angerAnim.SetTrigger("ready");
                    // card._angerAnim.speed = GameSettingManager.combatPlaySpeed;
                    card._SkillVfx.gameObject.SetActive(true);
                    if (card._SkillVfx.AnimationState != null)
                        card._SkillVfx.AnimationState.SetAnimation(0, "energy_full", true);
                    card.setAngerBarValue(1f);
                }
            }

        }
    }

    public void UseAngerSkill(int key)
    {
        if (cardList.ContainsKey(key))
        {
            var card = cardList[key];
            //怒气技能释放
            //  card.animHeroIcon.texture = card.heroIcon._taregetTexture.texture;
            // card._angerAnim.SetTrigger("fire");
            // card._angerAnim.speed = GameSettingManager.combatPlaySpeed;
            card._SkillVfx.gameObject.SetActive(true);
            if (card._SkillVfx.AnimationState != null)
            {
                var trackEntry = card._SkillVfx.AnimationState.SetAnimation(0, "skill_prepare", false);
                trackEntry.Complete += (track) =>
                {
                    card.setAngerBarValue(0f);
                    card._SkillVfx.gameObject.SetActive(false);
                };
            }
        }
    }

    public override void shiftIn()
    {
        if (contentPane.CardsTF != null)
        {
            contentPane.CardsTF.DOAnchorPosY(0f, 0.5f);
        }
    }

    protected override void DoHideAnimation()
    {
        if (contentPane.CardsTF != null)
        {
            contentPane.CardsTF.DOAnchorPosY(-450f, 0.2f).onComplete = () =>
            {
                base.DoHideAnimation();
            };
        }
        else
        {
            base.DoHideAnimation();
        }
    }
    public override void shiftOut()
    {
        if (contentPane.CardsTF != null)
        {
            contentPane.CardsTF.DOAnchorPosY(-450f, 0.2f);
        }
    }
}
