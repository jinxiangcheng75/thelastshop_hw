using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public class FighterClr
{
    string fighterPrefabUrl = "fighter";
    public static FighterClr CreatFighter(CombatFighter fighterData, Transform sitpoint, bool isLeft)
    {
        var fighter = new FighterClr(fighterData, sitpoint, isLeft);
        return fighter;
    }
    public int key;
    public int site;
    private GameObject fighterObject;
    public Transform headTipTF;
    public Transform entityTF;
    public Transform attkTF;
    public Transform hitTF;
    public Vector3 startPos;  //初始坐标
    private CombatFighter fighter;
    public bool isLeft;
    private FollowBloodstrip healthTip;
    protected DressUpSystem dressUpClr;
    public EquipAction currAction;
    public bool isDeath;
    public int currHp = 0;
    public int currAnger = 0;
    public bool isMonster = false;

    public int equipType = 1;
    //*********************************************************
    private Dictionary<int, int> buffsList = new Dictionary<int, int>();


    //*********************************************************
    public FighterClr(CombatFighter fighterData, Transform sitpoint, bool isleft)
    {
        site = fighterData.site;
        key = fighterData.key;
        fighter = fighterData;
        isLeft = isleft;
        currHp = fighterData.currentHp;
        isMonster = fighterData.isMonster == 1;

        /////////////////////////////////////////////////////////////////////////
        if (fighterData.isMonster == 1)
        {
            MonsterConfigData monster = MonsterConfigManager.inst.GetConfig(fighterData.job);
            currAction = EquipActionConfigManager.inst.GetCfg(monster.action_id);
            equipType = monster.equip_type;
        }
        else
        {
            currAction = EquipActionConfigManager.inst.GetCfg(fighterData.weapon);
            if (currAction == null)//999999
                currAction = EquipActionConfigManager.inst.GetCfg(999999);
            var equipTz = EquipConfigManager.inst.GetEquipDrawingsCfg(fighterData.weapon);
            if (equipTz != null)
            {
                equipType = equipTz.sub_type;
            }
            else
            {
                equipType = 11; //空手
            }
        }

        //////////////////////////////////////////////////////////////////////////
        ManagerBinder.inst.Asset.loadPrefabAsync(fighterPrefabUrl, sitpoint.parent, (obj) =>
         {
             fighterObject = obj;
             startPos = sitpoint.position;
             headTipTF = fighterObject.transform.Find("HeadNode");
             entityTF = fighterObject.transform.Find("EntityNode");
             attkTF = fighterObject.transform.Find("atkNode");
             hitTF = fighterObject.transform.Find("HitNode");

             fighterObject.transform.localPosition = sitpoint.localPosition + ((isleft ? Vector3.left : Vector3.right) * 6);

             //创建HUD
             healthTip = FGUI.inst.CreaterHealthTip(key, headTipTF);
             healthTip.SetInitInfo(isleft, fighter.hp, fighter.currentHp, fighter.anger, fighter.currentAnger, fighter.level, fighter.job);
             isDeath = false;
             fighterObject.SetActive(true);
             if (isMonster)
             {
                 MonsterConfigData monster = MonsterConfigManager.inst.GetConfig(fighterData.job);
                 CharacterManager.inst.GetCharacterByModel<DressUpSystem>(monster.monster_model, 0.12f, callback: characterloadend);
             }
             else
             {
                 CharacterManager.inst.GetCharacterByHero<DressUpSystem>((EGender)fighter.sex, fighterData.equips, SpineUtils.RoleDressToUintList(fighter.roleDress), 0.12f, callback: characterloadend);
             }
         });
    }

    void characterloadend(DressUpSystem dressUpSystem)
    {
        dressUpClr = dressUpSystem;
        var go = dressUpSystem.gameObject;
        go.transform.SetParent(entityTF);
        go.transform.localPosition = Vector3.zero;
        dressUpClr.SetSortingAndOrderLayer("map_Actor", 0);
        dressUpClr.SetDirection(isLeft ? RoleDirectionType.Right : RoleDirectionType.Left);
        AnimationSpeed(_speedRatio);
        if (dressUpClr != null && currAction != null)
        {
            dressUpClr.Play(currAction.act_combat_standby, true);
            isIdle = true;
        }
        enterArena();
    }

    public void setLayerOrder(int order)
    {
        dressUpClr.SetSortingAndOrderLayer("map_Actor", order);
    }
    public bool ismoveing = false;
    string currMoveAnimName = "";
    public void MoveToTarget(string moveanim, Vector3 targetPos, float duration)
    {
        ismoveing = true;
        currMoveAnimName = moveanim;
        if (dressUpClr != null && currAction != null)
        {
            dressUpClr.Play(moveanim, true);
        }
        if (fighterObject != null)
        {
            fighterObject.transform.DOLocalMove(targetPos, duration).onComplete = moveEnd;
        }
    }

    public void moveEnd()
    {
        //移动结束
        if (dressUpClr != null && currAction != null)
        {
            dressUpClr.Play(currAction.act_combat_standby, true);
            dressUpClr.SetDirection(isLeft ? RoleDirectionType.Right : RoleDirectionType.Left);
        }
        ismoveing = false;
    }
    public bool isIdle = true;
    public void PlayAnim(string name, bool backIdle)
    {
        if (dressUpClr != null && currAction != null)
        {
            if (name != currAction.act_combat_standby)
            {
                isIdle = false;
            }
            if (backIdle)
            {
                dressUpClr.Play(name, completeDele: t =>
               {
                   dressUpClr.Play(currAction.act_combat_standby, true);
                   isIdle = true;
               });
                // dressUpClr.PlayThenIdle(name, currAction.act_combat_standby);
            }
            else
            {
                dressUpClr.Play(name);
            }
        }
    }

    public void AnimationSpeed(float scale)
    {
        if (dressUpClr != null)
        {
            dressUpClr.SetAnimationSpeed(scale);
        }
    }

    public void ShowAttackFx(VFXConfig fxcfg)
    {
        playVfx(fxcfg);
    }

    public void ShowHitFx(VFXConfig fxcfg)
    {
        playVfx(fxcfg);
    }

    void playVfx(VFXConfig fxcfg)
    {

        EffectManager.inst.Spawn(fxcfg.id, entityTF.position, (gamevfx) =>
                    {
                        gamevfx.transform.SetParent(fighterObject.transform, false);
                        var rot = Vector3.zero;
                        // rot.y *= isLeft ? 0f : 180f;
                        gamevfx.transform.localRotation = Quaternion.Euler(0, isLeft ? 0f : 180f, 0);
                        gamevfx.transform.localPosition = Vector3.zero;
                        gamevfx.setSpeedRatio(speedRatio);
                        gamevfx.gameObject.SetActive(true);
                    });
    }

    private GameObject chargeVfxGo;
    public void ChargeVfx(VFXConfig fxcfg, float life, bool show)
    {
        if (show)
        {
            EffectManager.inst.Spawn(fxcfg.id, entityTF.position, (gamevfx) =>
                        {
                            gamevfx.transform.SetParent(fighterObject.transform, false);
                            var rot = Vector3.zero;
                            // rot.y *= isLeft ? 0f : 180f;
                            gamevfx.transform.localRotation = Quaternion.Euler(0, isLeft ? 0f : 180f, 0);
                            gamevfx.transform.localPosition = Vector3.zero;
                            gamevfx.setSpeedRatio(speedRatio);
                            gamevfx.gameObject.SetActive(true);
                            chargeVfxGo = gamevfx.gameObject;
                            GameObject.Destroy(chargeVfxGo, life);
                        });
        }
        else
        {
            if (chargeVfxGo != null)
            {
                GameObject.Destroy(chargeVfxGo);
                chargeVfxGo = null;
            }
        }
    }
    public void DeletSelf()
    {
        if (fighterObject != null)
        {
            GameObject.Destroy(fighterObject);
        }
        if (healthTip != null)
        {
            GameObject.Destroy(healthTip.gameObject);
        }
    }

    public void AngerChange(int anger)
    {
        if (anger != 0)
        {
            this.currAnger += anger;
            this.currAnger = Mathf.Max(0, this.currAnger);
            healthTip.AngerChange(currAnger);
            EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_VIEW_HPUPDATE, key);
        }
    }
    public void UserAngerSkill()
    {
        this.currAnger = 0;
        EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_USE_ANGER, key);
    }
    public void Damage(float damage, int hited, bool critical = false)
    {
        //if (isDeath) return;
        if (hited == 1)
        {
            Logger.log("受击");
            this.currHp += (int)damage;
            if (fighter.hp <= this.currHp)
            {
                this.currHp = fighter.hp;
            }

            healthTip.hpChange((int)damage, false, critical);

            if (damage < 0 && dressUpClr != null && currAction != null)
            {
                isIdle = false;
                dressUpClr.Play(currAction.act_hit, completeDele: t =>
                {
                    dressUpClr.Play(currAction.act_combat_standby, true);
                    isIdle = true;
                });

                //  dressUpClr.PlayThenIdle(currAction.act_hit, currAction.act_combat_standby, speedRatio);
            }
        }
        else
        {
            Logger.log("闪避");
            healthTip.hpChange(0, true, false);
        }
        if (this.currHp <= 0)
        {
            this.currHp = 0;
            GameTimer.inst.AddTimer(0.8f / speedRatio, 1, Death);
        }
        EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_VIEW_HPUPDATE, key);
    }
    //欢呼
    public void Cheer()
    {
        if (dressUpClr != null && currAction != null)
        {
            isIdle = false;
            dressUpClr.Play(currAction.act_victory, completeDele: t =>
            {
                dressUpClr.Play(currAction.act_combat_standby, true);
                isIdle = true;
            });
            //  dressUpClr.PlayThenIdle(currAction.act_victory, currAction.act_combat_standby, speedRatio);
        }
    }

    //攻击结束
    public void EndAttack()
    {
        ismoveing = true;
        Logger.log("我攻击结束");
        if (Vector3.Distance(startPos, fighterObject.transform.localPosition) > 0.2f)
        {
            if (dressUpClr != null && currAction != null)
            {
                dressUpClr.Play(currMoveAnimName, true);
            }
            if (dressUpClr != null && fighterObject != null)
            {
                dressUpClr.SetDirection(!isLeft ? RoleDirectionType.Right : RoleDirectionType.Left);
                fighterObject.transform.DOLocalMove(startPos, 0.3f / speedRatio).onComplete = moveEnd;
            }
        }
    }

    public void enterArena()
    {
        if (isDeath) return;
        if (dressUpClr != null && currAction != null)
        {
            dressUpClr.Play(currAction.act_run, true);
        }
        fighterObject.transform.DOLocalMove(startPos, 0.3f / speedRatio).onComplete = moveEnd;
    }
    public void Death()
    {
        if (isDeath) return;
        //
        isDeath = true;
        this.currHp = 0;
        if (healthTip.gameObject != null) healthTip.gameObject.SetActive(false);
        EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_VIEW_HPUPDATE, key);
        if (dressUpClr != null && currAction != null)
        {
            dressUpClr.Play(currAction.act_failed, false, completeDele: t =>
          {
              if (!string.IsNullOrEmpty(currAction.act_failed_standby))
                  dressUpClr.Play(currAction.act_failed_standby, true);
          });
        }
        if (isMonster)
        {
            var combatView = GUIManager.GetWindow<CombatView>();
            if (combatView != null && combatView.isShowing)
            {
                combatView.refreshState(1, 1);
            }
        }
    }
    //--------------------------------------------------------------------------------------------------
    private Dictionary<int, Buffer> buffVfxList = new Dictionary<int, Buffer>();
    public bool HaveBuff(int buffid)
    {
        return buffsList.ContainsKey(buffid);
    }
    public void AddBuffer(int buffid)
    {
        if (!buffsList.ContainsKey(buffid))
        {
            buffsList.Add(buffid, 1);
            //播放buff特效
            PlayBuff(buffid);
        }
        else
        {
            buffsList[buffid]++;
        }
    }

    public void RemoveBuffer(int buffid)
    {
        if (buffsList.ContainsKey(buffid))
        {
            buffsList[buffid]--;
            if (buffsList[buffid] <= 0)
            {
                EndBuff(buffid);
                buffsList.Remove(buffid);
                //消除buff图标
            }
        }
    }

    private void PlayBuff(int id)
    {
        if (healthTip == null) return;
        var buf = new Buffer();
        buf.InitBuffInfo(id, key, entityTF);
        //if (buf.buffConfig.flow_text == 1)
        //    healthTip.showStateText(buf.buffConfig.flow_name);
        if (buf.vfxLifeTime > 0)
        {
            buffsList.Remove(id);
        }
        else
        {
            buffVfxList.Add(id, buf);
        }

    }
    private void EndBuff(int id)
    {
        if (buffVfxList.ContainsKey(id))
        {
            var fxGo = buffVfxList[id];
            fxGo.EndVFX();
            buffVfxList.Remove(id);
        }
    }
    //----------------------------------------------------------------------------------------------------

    private float _speedRatio = 1;
    public float speedRatio
    {
        get { return _speedRatio; }
        set
        {
            if (_speedRatio != value)
            {
                _speedRatio = value;
                //动作倍速
                if (dressUpClr != null)
                {
                    dressUpClr.SetAnimationSpeed(value);
                }
                //身上BUFF 速度
                foreach (Buffer buf in buffVfxList.Values)
                {
                    buf.SpeedRatio(value);
                }
            }
        }
    }
}

