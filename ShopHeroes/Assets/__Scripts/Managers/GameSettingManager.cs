using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//本地游戏设置
public class GameSettingManager : TSingletonHotfix<GameSettingManager>
{
    public static string appVersion = "";
    public static string resVersion = "";
    public static float combatDefaultSpeed = 1;
    public static float combatPlaySpeed = 1; //战报播放速度
    public static int HandleEventState = 0;  //界面操作上報開關
    public static bool ProtocolEncryption = true;//协议加密
    protected override void init()
    {

    }
    public bool needShowUIAnim
    {
        get
        {
            int result = -1;
            if (SaveManager.inst.GetInt(kPrefKey.NeedPlayUIAnim.ToString()) == 0)
            {
                SaveManager.inst.SaveInt(kPrefKey.NeedPlayUIAnim.ToString(),1);
                result = 1;
            }
            else
            {
                result = SaveManager.inst.GetInt(kPrefKey.NeedPlayUIAnim.ToString());
            }

            return result == 1;
        }
    }

    public void SetNeedShowUIAnim(bool needShow)
    {
        SaveManager.inst.SaveInt(kPrefKey.NeedPlayUIAnim.ToString(), needShow ? 1 : -1);
    }

    //是否为ipad
    public bool IsIpad() 
    {
        string a = SystemInfo.deviceModel.ToLower().Trim();
        Logger.log("芋圆 aaaa：  " + SystemInfo.deviceName + "   bbbb： " + a);
        if (a.StartsWith("ipad"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
