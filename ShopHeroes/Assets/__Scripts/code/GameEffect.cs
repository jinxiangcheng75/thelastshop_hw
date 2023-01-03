using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffect : MonoBehaviour
{
    public int vfxid;
    public bool isloop = false;
    public float lifeTime = 0;
    ParticleSystem ps;
    public bool isPlaying = false;
    private float speedRatio = 1f;
    void Start()
    {
        var cfg = VFXConfigManager.inst.GetConfig(vfxid);
        isloop = cfg.isLoop;
        lifeTime = (float)cfg.time * 0.001f;
        isPlaying = false;
        onSpawn();
    }
    public void setSpeedRatio(float ratio)
    {
        speedRatio = ratio;
        ParticleSystem[] sysList = this.GetComponentsInChildren<ParticleSystem>();
        foreach (var sys in sysList)
        {
            sys.playbackSpeed = speedRatio;
        }
    }
    private void Update()
    {
        if (isPlaying && !isloop)
        {
            lifeTime -= Time.deltaTime * speedRatio;
            if (lifeTime < -0.5f)
            {
                Die();
            }
        }
    }

    public void onSpawn()
    {
        isPlaying = true;
        ParticleSystem[] sysList = this.GetComponentsInChildren<ParticleSystem>();
        foreach (var sys in sysList)
        {
            sys.loop = isloop;
            sys.playbackSpeed = speedRatio;
        }
    }


    public void OnDestroy()
    {
        isPlaying = false;
    }
    public void Die()
    {
        EffectManager.inst.Despawn(this);
    }
}
