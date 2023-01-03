using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesLightTimeAnimi : MonoBehaviour
{
    [Header("一天时间周期(分钟)")]
    public float oneDayTimeLen;
    private Animator _animator;
    private int state = -1;
    private float updatetime = 0;
    private int musicId = -1;
    void Awake()
    {
        _animator = GetComponent<Animator>();

    }
    void Start()
    {
        var _time = Mathf.FloorToInt((System.DateTime.Now.Minute % oneDayTimeLen) / oneDayTimeLen * 24.0f);
        state = _time / 6;
        var _state = state + 1 >= 4 ? 0 : state + 1;
        _animator.enabled = true;
        _animator.speed = 1;
        _animator.Play(0, 0, 0.25f * _state);
        _animator.speed = 0;
        musicId = state <= 1 ? 1 : 2;
        AudioManager.inst.PlayMusic(musicId);
    }

    void updateLightState()
    {
        var _time = Mathf.FloorToInt((System.DateTime.Now.Minute % oneDayTimeLen) / oneDayTimeLen * 24.0f);
        if (state != _time / 6)
        {
            state = _time / 6;
            playanim(0.25f * state);

            int _musicId = state <= 1 ? 1 : 2;
            if (musicId != _musicId)
            {
                musicId = _musicId;
                AudioManager.inst.PlayMusic(musicId);
            }
        }
    }
    void Update()
    {
        if (_animator == null || !_animator.enabled) return;
        if (_animator.speed == 1)
        {
            playtime += Time.deltaTime;
            if (playtime >= changeTime)
            {
                stop();
            }
        }
        else
        {
            updatetime += Time.deltaTime;
            if (updatetime > 10)
            {
                updateLightState();
                updatetime = 0;
            }
        }
    }
    private float changeTime = 6.0f;
    private float playtime = 0;
    //timesub = 0, 0.25, 0.5 , 0.75
    public void playanim(float timesub)
    {
        playtime = 0;
        //_animator.enabled = true;
        _animator.speed = 1;
        _animator.Play(0, 0, timesub);
    }
    public void stop()
    {
        if (_animator != null)
        {
            _animator.speed = 0;
        }
    }
}
