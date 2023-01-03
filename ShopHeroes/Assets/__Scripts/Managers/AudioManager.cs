using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>
{
    [HideInInspector]
    public float globalVolume = 1;

    [HideInInspector]
    public float musicVolume = 1;
    private string currMusicName;

    [HideInInspector]
    public float soundVolume = 1;

    private bool globalPause = false;
    private bool musicPaused = false;

    public override void init()
    {
        base.init();
        DontDestroyOnLoad(gameObject);

        //读取本地设置
        if (PlayerPrefs.HasKey("musicMute"))
        {
            StopMusic(PlayerPrefs.GetInt("musicMute") == 1);
        }
        if (PlayerPrefs.HasKey("soundMute"))
        {
            SoundMute(PlayerPrefs.GetInt("soundMute") == 1);
        }
        if (PlayerPrefs.HasKey("globalVolume"))
        {
            SetGlobalVolume(PlayerPrefs.GetFloat("globalVolume"));
        }
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        }
        if (PlayerPrefs.HasKey("soundVolume"))
        {
            SetSoundVolume(PlayerPrefs.GetFloat("soundVolume"));
        }

    }

    public void SetGlobalVolume(float value)
    {
        globalVolume = value;
        AudioController.SetGlobalVolume(globalVolume);
        //保存
        PlayerPrefs.SetFloat("globalVolume", globalVolume);
    }

    public void PausedAll(bool paused = true)
    {
        globalPause = paused;
        if (globalPause)
        {
            AudioController.PauseAll(0.1f);
        }
        else
        {
            AudioController.UnpauseAll(0.1f);
        }
    }
    #region 背景音乐 
    //播放背景音乐
    //AudioClip _cacheMusic;
    public void PlayMusic(string musicName, float volume = 1)
    {
        if (isPause) return;
        if (string.IsNullOrEmpty(musicName)) return;
        //if (currMusicName != musicName && _cacheMusic != null)
        //{
        //    ManagerBinder.inst.Asset.unloadMiscAsset<AudioClip>(currMusicName, _cacheMusic);
        //    _cacheMusic = null;
        //}
        currMusicName = musicName;
        if (musicPaused) return;

        // AudioController.
        var category = GetCustomAudioCategory("Music");
        if (!CheckAudioInCategory(musicName, category))
        {
            ManagerBinder.inst.Asset.loadMiscAsset<AudioClip>(musicName, (clip) =>
            {
                if (clip)
                {
                    //_cacheMusic = clip;
                    AudioController.AddToCategory(category, (AudioClip)clip, musicName);
                    AudioObject audioObject = AudioController.PlayMusic(musicName, musicVolume * volume);
                    if (audioObject != null)
                        audioObject.primaryAudioSource.loop = true;
                }
            });
        }
        else
        {
            AudioObject audioObject = AudioController.PlayMusic(musicName, musicVolume * volume);
            if (audioObject != null)
                audioObject.primaryAudioSource.loop = true;
        }
    }
    public void PlayMusic(int id)
    {
        var soundname = MusicConfigManager.inst.GetMusicName(id);
        if (!string.IsNullOrEmpty(soundname))
        {

            PlayMusic(soundname);
        }
    }
    /// <summary>
    /// 获取自定义Category
    /// </summary>
    /// <returns></returns>

    private static AudioCategory GetCustomAudioCategory(string gategoryName)
    {
        var category = AudioController.GetCategory(gategoryName);
        if (category == null)
            category = AudioController.NewCategory(gategoryName);
        return category;
    }

    /// <summary>
    /// 检查当前声音是否已经存在
    /// </summary>
    /// <param name="strAudio"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    private static bool CheckAudioInCategory(string strAudio, AudioCategory category)
    {
        if (category.AudioItems == null)
            return false;
        bool isExist = false;
        foreach (var item in category.AudioItems)
        {
            if (item.Name == strAudio)
            {
                isExist = true;
                break;
            }
        }
        return isExist;
    }

    //停止音乐
    public void StopMusic(bool paused = true)
    {
        musicPaused = paused;
        if (musicPaused)
        {
            AudioController.PauseMusic(0.1f);
        }
        else
        {
            if (AudioController.GetCurrentMusic() != null)
            {
                AudioController.UnpauseMusic(0.1f);
            }
            else
            {
                PlayMusic(currMusicName);
            }
        }
        //保存
        PlayerPrefs.SetInt("musicMute", musicPaused ? 1 : 0);
    }

    //调节音乐音量
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        AudioController.SetCategoryVolume("Music", musicVolume);
        //保存
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }
    #endregion

    //播放音效
    private bool soundMute = false;
    public void PlaySound(string SoundName) //下降到70%
    {
        if (isPause) return;
        if (soundMute) return;
        var category = GetCustomAudioCategory("Sound");
        if (!CheckAudioInCategory(SoundName, category))
        {
            ManagerBinder.inst.Asset.loadMiscAsset<AudioClip>(SoundName, (clip) =>
            {
                if (clip)
                {
                    AudioController.AddToCategory(category, (AudioClip)clip, SoundName);
                    AudioController.Play(SoundName, soundVolume);
                }
            });
        }
        else
        {
            AudioController.Play(SoundName, soundVolume);
        }
    }

    public void PlaySound(int id)
    {
        string soundname = MusicConfigManager.inst.GetMusicName(id);
        if (!string.IsNullOrEmpty(soundname))
        {
            PlaySound(soundname);
        }
    }
    //音效静音
    public void SoundMute(bool mute)
    {
        soundMute = mute;
        //保存
        PlayerPrefs.SetInt("soundMute", mute ? 1 : 0);
    }

    //音效音量
    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        AudioController.SetCategoryVolume("Sound", soundVolume);
        //保存
        PlayerPrefs.SetFloat("soundVolume", soundVolume);
    }



    //停止播放所有
    public void stopAll()
    {
        AudioController.StopAll();
    }

    //暂停播放游戏声音
    bool isPause = false;
   public void PauseAll( bool ispause)
    {
        isPause = ispause;
        if(ispause)
        {
            AudioController.PauseAll();
        }
        else
        {
            AudioController.UnpauseAll();
        }
    }
}
