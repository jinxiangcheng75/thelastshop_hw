using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffer
{
    public int buffId;
    public GameEffect gameEffect;
    public BuffConfig buffConfig;
    public int targetKey; //目标对象key
    private Transform targetTF;
    public float vfxLifeTime;
    public void InitBuffInfo(int id, int key, Transform parent)
    {
        targetKey = key;
        targetTF = parent;
        buffId = id;
        buffConfig = BuffConfigManager.inst.GetConfig(id);
        Logger.log($"目标{key}增加BUFF{id}");
        if (buffConfig != null)
        {
            showFx();
        }
        else
        {
            Logger.log("找不到BUFF配置");
        }
    }

    bool Loop = true;
    void showFx()
    {
        //特效效果
        if (buffConfig.effect > 0)
        {
            vfxLifeTime = -1;
            VFXConfig vfxCfg = VFXConfigManager.inst.GetConfig(buffConfig.effect);
            if (vfxCfg != null)
            {
                if (!vfxCfg.isLoop)
                {
                    vfxLifeTime = (float)vfxCfg.time * 0.001f;
                }
                Loop = vfxCfg.isLoop;

                if (vfxCfg != null)
                {
                    ManagerBinder.inst.Asset.InstantiatePrefabAsync(vfxCfg.vfxname, (newgo) =>
                    {
                        gameEffect = newgo.GetComponent<GameEffect>() ?? newgo.AddComponent<GameEffect>();
                        gameEffect.vfxid = vfxCfg.id;
                        newgo.SetActive(true);
                        gameEffect.setSpeedRatio(speedRatio);
                        gameEffect.transform.SetParent(targetTF, false);
                        gameEffect.transform.localPosition = Vector3.zero;
                    });
                }
            }
        }
        //图标
    }
    float speedRatio = 1;
    public void SpeedRatio(float ratio)
    {
        speedRatio = ratio;
        if (gameEffect != null)
        {
            gameEffect.setSpeedRatio(speedRatio);
        }
    }

    public void EndVFX()
    {
        if (gameEffect != null)
        {
            gameEffect.Die();
        }
        gameEffect = null;


        targetKey = -1;
        targetTF = null;
        buffId = -1;
        buffConfig = null;
    }
}
