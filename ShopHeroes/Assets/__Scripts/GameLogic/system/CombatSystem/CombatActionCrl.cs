using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillAction
{
    public List<FighterClr> FighterList;
    //public HeroSkillShowConfig config;
    public int skillId;
    public int buffid;
    public CombatAction action;
    public bool isLoading = false;
    public bool isEnd = false;
    public BattleActionType actionType;
    private FighterClr fromFighter = null;
    private FighterClr targetFighter = null;
    private EquipAction equipAction;
    private HeroSkillConfig heroSkillCfg;
    public System.Action actionEnd;

    public CombatSkillsConfig new_SkillShowCfg;
    public float speedRatio = 6;
    private void PlayVfx(int vfxid, Vector3 targetPos, bool isLeft)
    {
        EffectManager.inst.Spawn(vfxid, targetPos, (gamevfx) =>
      {
          var rot = Vector3.zero;
          rot.y = isLeft ? 0f : 180f;
          gamevfx.gameObject.SetActive(true);
          gamevfx.transform.Rotate(rot);
          gamevfx.transform.position = targetPos;
          gamevfx.setSpeedRatio(speedRatio);
      });
    }
    public void PlayAction()
    {
        //处理数据
        if (action.actionFrom != null)
        {
            fromFighter = FighterList.Find(item => item.key == action.actionFrom.key);
            if (fromFighter != null)
            {
                equipAction = fromFighter.currAction;
            }
            //目标
            if (action.actionTarget.Count > 0)//判断是单体攻击 
            {
                CombatTarget fTarget = action.actionTarget[0];
                targetFighter = FighterList.Find(item => item.key == fTarget.key);
            }
            else
            {
                targetFighter = null;
            }

        }

        if (actionType == BattleActionType.skill)
        {
            // config = HeroSkillShowConfigManager.inst.GetConfig(action.skillId);
            // if (config != null)
            // {
            heroSkillCfg = heroSkillConfigManager.inst.GetConfig(skillId);
            // }
            // else
            // {
            //     //////////////////////////////////// 临时添加
            //     config = HeroSkillShowConfigManager.inst.GetConfig(10001);
            //     heroSkillCfg = heroSkillConfigManager.inst.GetConfig(10001);
            //     ////////////////////////////////////
            // }

            new_SkillShowCfg = HeroSkillShowConfigManager.inst.GetCombatSkillsConfig(fromFighter.equipType, action.skillId);
            if (new_SkillShowCfg == null)
            {
                Debug.LogError("找不到对应的技能表现配置 skillID = " + action.skillId);
                _actionEnd = true;
                return;
            }
        }

        _actionEnd = false;
        GameTimer.inst.StartCoroutine(_Play());
    }
    bool _actionEnd = false;
    public bool IsActionEnd
    {
        get { return _actionEnd; }
    }

    public void Dis()
    {
        GameTimer.inst.StopCoroutine(_Play());
        _actionEnd = true;
    }

    public void CreateSkillFx()
    {
        //释放技能
    }

    //子弹效果
    private bullet currbullet;
    public void CreateBullet(int bulletid)
    {
        //释放子弹
        EffectManager.inst.Spawn(bulletid, fromFighter.attkTF.position, (fx) =>
        {
            currbullet = fx.gameObject.GetComponent<bullet>();
            if (currbullet != null)
            {
                VFXConfig bulletvfxCfg = VFXConfigManager.inst.GetConfig(fx.vfxid);
                currbullet.startFighter = fromFighter;
                currbullet.transform.position = fromFighter.attkTF.position;
                currbullet.targetFighter = targetFighter;
                currbullet.lifeTime = bulletvfxCfg.time * 0.001f / speedRatio;
                currbullet.isleft = fromFighter.isLeft;
                currbullet.shot();
            }
            fx.gameObject.SetActive(true);
            fx.onSpawn();
        });

    }
    IEnumerator _Play()
    {
        yield return null;
        switch (actionType)
        {
            case BattleActionType.skill:
                {
                    if (heroSkillCfg == null) break;
                    Logger.log("使用技能 skillID = " + action.skillId);
                    /////////////////////////////////////////////////////////////////////
                    //判断是否有触发者
                    if (fromFighter != null)
                    {
                        //第一步 判断是否是怒气技能
                        if (heroSkillCfg.classification == 2)
                        {
                            //爆发怒气
                            fromFighter.UserAngerSkill();
                            //同时设置层级
                            fromFighter.setLayerOrder(2);
                            List<CombatTarget> targets = action.actionTarget;
                            for (int index = 0; index < targets.Count; index++)
                            {
                                FighterClr _targetFighter = FighterList.Find(item => item.key == targets[index].key);
                                if (_targetFighter != null)
                                {
                                    _targetFighter.setLayerOrder(2);
                                }
                            }
                            yield return null;
                            //前摇特效
                            if (new_SkillShowCfg.anger_skill_prepare_effect_id > 0)
                            {
                                var vfxcfg = VFXConfigManager.inst.GetConfig(new_SkillShowCfg.anger_skill_prepare_effect_id);
                                if (vfxcfg != null)
                                    fromFighter.ChargeVfx(vfxcfg, new_SkillShowCfg.anger_skill_prepare_life_time * 0.001f, true); //蓄力前摇特效
                                AudioManager.inst.PlaySound(new_SkillShowCfg.anger_skill_prepare_audio_id);
                            }
                            yield return new WaitForSeconds(new_SkillShowCfg.anger_skill_prepare_next_time * 0.001f / speedRatio);
                        }
                        else
                        {
                            //非怒气技能的 前摇 效果
                            if (new_SkillShowCfg.anger_skill_prepare_effect_id > 0)
                            {
                                var vfxcfg = VFXConfigManager.inst.GetConfig(new_SkillShowCfg.anger_skill_prepare_effect_id);
                                if (vfxcfg != null)
                                    fromFighter.ChargeVfx(vfxcfg, new_SkillShowCfg.anger_skill_prepare_life_time * 0.001f, true); //蓄力前摇特效
                                AudioManager.inst.PlaySound(new_SkillShowCfg.anger_skill_prepare_audio_id);
                                yield return new WaitForSeconds(new_SkillShowCfg.anger_skill_prepare_next_time * 0.001f / speedRatio);
                            }
                        }

                        //判断是否需要近身
                        if (equipAction != null && heroSkillCfg.priority == 1 && equipAction.if_close_combat == 1)
                        {
                            //向目标移动
                            if (targetFighter != null)
                            {
                                //移动目标不为空
                                Vector3 targetpos = targetFighter.startPos;
                                targetpos.x += targetFighter.isLeft ? 0.8f : -0.8f;
                                fromFighter.MoveToTarget(equipAction.act_run, targetpos, .3f / speedRatio);
                            }
                            else
                            {
                                //目标为空不移动
                            }
                            while (fromFighter.ismoveing)
                            {
                                yield return 0;
                            }
                        }
                    }
                    //第二步攻击（可能多次攻击）
                    //for (int i = 0; i < config.trigger_time; i++)
                    {
                        //判断是否是技能
                        var isSkill = heroSkillCfg.priority != 1;
                        if (equipAction != null && fromFighter != null)
                        {
                            VFXConfig vfxcfg;
                            // //动作
                            // if (isSkill)
                            // {

                            // }
                            // else
                            // {
                            //     if (equipAction.audio_attack > 0)
                            //     {
                            //         AudioManager.inst.PlaySound(equipAction.audio_attack);
                            //     }
                            //     fromFighter.PlayAnim(equipAction.act_attack, true);
                            //     if (equipAction.common_attack_time > 0)
                            //         yield return new WaitForSeconds(equipAction.common_attack_time * 0.001f / speedRatio);
                            //     vfxcfg = VFXConfigManager.inst.GetConfig(equipAction.vfx_attack);
                            // }
                            //
                            if (isSkill)
                            {
                                fromFighter.PlayAnim(equipAction.act_skill, true);
                                if (equipAction.skill_attack_time > 0)
                                    yield return new WaitForSeconds(equipAction.skill_attack_time * 0.001f / speedRatio);
                            }
                            else
                            {
                                fromFighter.PlayAnim(equipAction.act_attack, true);
                                if (equipAction.common_attack_time > 0)
                                    yield return new WaitForSeconds(equipAction.common_attack_time * 0.001f / speedRatio);
                            }
                            vfxcfg = VFXConfigManager.inst.GetConfig(new_SkillShowCfg.skill_attack_effect_id);
                            if (vfxcfg != null)
                            {
                                fromFighter.ShowAttackFx(vfxcfg);
                            }
                            else
                            {
                                Debug.LogError("找不到攻击特效 ID = " + new_SkillShowCfg.skill_attack_effect_id);
                            }
                            if (new_SkillShowCfg.skill_attack_audio > 0)
                            {
                                AudioManager.inst.PlaySound(new_SkillShowCfg.skill_attack_audio);
                            }
                            //if (equipAction.attack_effect_time > 0)
                            //    yield return new WaitForSeconds(equipAction.attack_effect_time * 0.001f / speedRatio);
                            //攻击 怒气
                            if (fromFighter != null)
                                fromFighter.AngerChange(action.actionFrom.anger);
                        }
                        //第三步 创建子弹(飞行特效)
                        if (new_SkillShowCfg.skill_attack_bullet > 0)
                            CreateBullet(new_SkillShowCfg.skill_attack_bullet);
                        if (new_SkillShowCfg.skill_attack_trigger_audio > 0)
                            AudioManager.inst.PlaySound(new_SkillShowCfg.skill_attack_trigger_audio);
                        if (new_SkillShowCfg.skill_attack_bullet_next_time > 0)
                            yield return new WaitForSeconds(new_SkillShowCfg.skill_attack_bullet_next_time * 0.001f / speedRatio);
                        //定点特效
                        if (new_SkillShowCfg.skill_attack_trigger_effect_id > 0)
                        {
                            //定点特效 在场景中心播放。特效制作可做偏移。（根据左右镜像）
                            PlayVfx(new_SkillShowCfg.skill_attack_trigger_effect_id, Vector3.zero, fromFighter.isLeft);
                        }
                        if (new_SkillShowCfg.skill_attack_trigger_next_time > 0)
                            yield return new WaitForSeconds(new_SkillShowCfg.skill_attack_trigger_next_time * 0.001f / speedRatio);
                        ////第四步 目标表现

                        //受击者
                        List<CombatTarget> targets = action.actionTarget;
                        Logger.log(equipAction.act_skill.ToString() + "技能，受击目标数量：" + targets.Count);
                        for (int index = 0; index < targets.Count; index++)
                        {
                            var target = targets[index];
                            Logger.log("受击目标：" + target.key);
                            FighterClr _targetFighter = FighterList.Find(item => item.key == target.key);
                            if (_targetFighter != null)
                            {
                                // if (i < target.effectList.Count)
                                if (target.effectList.Count > 0)
                                {
                                    CombatTargetEffect cte = target.effectList[0];
                                    var vfxcfg = VFXConfigManager.inst.GetConfig(new_SkillShowCfg.skill_attack_hit_effect_id);
                                    if (vfxcfg != null)
                                    {
                                        _targetFighter.ShowHitFx(vfxcfg);
                                    }
                                    else
                                    {
                                        Debug.LogError($"{new_SkillShowCfg.skill_attack_hit_effect_id}：未找到特效");
                                    }
                                    // if (i == 0)
                                    {
                                        _targetFighter.AngerChange(target.anger);
                                    }
                                    if (new_SkillShowCfg.skill_attack_hit_audio > 0)
                                    {
                                        AudioManager.inst.PlaySound(new_SkillShowCfg.skill_attack_hit_audio);
                                    }
                                    _targetFighter.Damage(cte.hp, cte.hited, cte.critical == 1);
                                }
                            }
                            else
                            {
                                Logger.log("没有找到受击目标：" + target.key);
                            }
                            foreach (int buffid in target.addBuffList)
                            {
                                _targetFighter.AddBuffer(buffid);
                                //if (!_targetFighter.HaveBuff(buffid))
                                //{


                                //}
                            }
                            foreach (int buffid in target.removeBuffList)
                            {
                                if (_targetFighter.HaveBuff(buffid))
                                {
                                    _targetFighter.RemoveBuffer(buffid);
                                }
                            }
                        }
                        //  if (config.trigger_time_space > 0)
                        // yield return new WaitForSeconds(config.trigger_time_space * 0.001f / speedRatio);
                    }

                    if (fromFighter != null)
                    {
                        while (!fromFighter.isIdle)
                        {
                            yield return null;
                        }
                        fromFighter.EndAttack();
                    }
                    yield return new WaitForSeconds(.5f);
                }
                yield return new WaitForSeconds(.5f / speedRatio);
                break;
            case BattleActionType.addBuff:
                {
                    //直接完成
                    //增加 buff目标
                    List<CombatTarget> targets = action.actionTarget;
                    targets.ForEach((target) =>
                    {
                        FighterClr _targetFighter = FighterList.Find(item => item.key == target.key);
                        foreach (int buffid in target.addBuffList)
                        {
                            _targetFighter.AddBuffer(buffid);
                            if (!_targetFighter.HaveBuff(buffid))
                            {
                                for (int i = 0; i < target.effectList.Count; i++)
                                {
                                    CombatTargetEffect cte = target.effectList[i];
                                    _targetFighter.Damage(cte.hp, cte.hited, cte.critical == 1);
                                }
                            }
                        }
                    });
                }
                yield return new WaitForSeconds(.5f / speedRatio);
                break;
            case BattleActionType.removeBuff:
                {
                    //直接完成
                    //减少 buff目标
                    List<CombatTarget> targets = action.actionTarget;
                    targets.ForEach((target) =>
                    {
                        FighterClr _targetFighter = FighterList.Find(item => item.key == target.key);
                        foreach (int buffid in target.removeBuffList)
                        {
                            if (_targetFighter.HaveBuff(buffid))
                            {
                                _targetFighter.RemoveBuffer(buffid);
                                for (int i = 0; i < target.effectList.Count; i++)
                                {
                                    CombatTargetEffect cte = target.effectList[i];
                                    _targetFighter.Damage(cte.hp, cte.hited, cte.critical == 1);
                                }
                            }
                        }
                    });
                }
                yield return new WaitForSeconds(.5f / speedRatio);
                break;
            case BattleActionType.buff:
                {
                    //直接完成
                    //buff目标
                    List<CombatTarget> targets = action.actionTarget;
                    for (int index = 0; index < targets.Count; index++)
                    {
                        var target = targets[index];
                        FighterClr _targetFighter = FighterList.Find(item => item.key == target.key);
                        if (_targetFighter != null)
                        {
                            for (int i = 0; i < target.effectList.Count; i++)
                            {
                                CombatTargetEffect cte = target.effectList[i];
                                _targetFighter.Damage(cte.hp, cte.hited, cte.critical == 1);
                            }
                        }
                    }
                }
                yield return new WaitForSeconds(.1f / speedRatio);
                break;
        }

        //恢复人物层级
        if (heroSkillCfg != null && heroSkillCfg.classification == 2)
        {
            if (fromFighter != null)
                fromFighter.setLayerOrder(0);
            List<CombatTarget> targets = action.actionTarget;
            for (int index = 0; index < targets.Count; index++)
            {
                FighterClr _targetFighter = FighterList.Find(item => item.key == targets[index].key);
                if (_targetFighter != null)
                {
                    _targetFighter.setLayerOrder(0);
                }
            }
        }
        _actionEnd = true;
    }
}

///
//处理普通攻击和技能类型的事件
public class CombatActionCrl
{
    public Dictionary<int, skillAction> skillactionList = new Dictionary<int, skillAction>();
    public List<FighterClr> FighterList;
    public void setSpeedRatio(float ratio)
    {
        foreach (var sa in skillactionList.Values)
        {
            if (!sa.IsActionEnd)
            {
                sa.speedRatio = ratio;
            }
        }
    }


    public void PlayAction(CombatAction action)
    {
        skillAction sa = new skillAction();
        sa.actionType = (BattleActionType)action.actionType;
        sa.skillId = action.skillId;
        sa.buffid = action.buffId;
        sa.action = action;
        sa.speedRatio = GameSettingManager.combatPlaySpeed;
        sa.FighterList = FighterList;
        if (skillactionList.ContainsKey(action.actionId))
        {
            skillactionList[action.actionId].Dis();
        }
        else
        {
            skillactionList.Add(action.actionId, sa);
        }
        sa.PlayAction();
    }

    public bool ActionEnd(int actionid)
    {
        if (skillactionList.ContainsKey(actionid))
        {
            return skillactionList[actionid].IsActionEnd;
        }
        return true;
    }
    public void ExitCombat()
    {
        foreach (var action in skillactionList.Values)
        {
            if (!action.IsActionEnd)
            {
                action.Dis();
            }
        }
        skillactionList.Clear();
    }

}


