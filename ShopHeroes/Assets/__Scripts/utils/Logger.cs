using UnityEngine;
using System.Collections;
using System.IO;

[System.Flags]
public enum E_OS_TYPE
{
    WINDOWS = 1 << 0,
    LINUX = 1 << 1,
    ANDROID = 1 << 2,
    IOS = 1 << 3,
}

[System.Flags]
public enum E_LOG_TAG : int
{
    NONE = 0,
    ACTOR = 1 << 0,
    AIREACT = 1 << 1,
    GAME_STATE = 1 << 2,
    BATTLE = 1 << 3,
    LEVEL = 1 << 4,
    HUD = 1 << 5,
    CONFIG = 1 << 6,
    UI = 1 << 7,
    NETWORK = 1 << 8,
    AUDIO = 1 << 9,
    BUFF = 1 << 10,
    ALL = 1 << 11 - 1,
}

public sealed class Logger
{

    private static string LOG_PATH = "log.txt";
#if UNITY_EDITOR
    public static bool ENABLED = true;
#else
    public static bool ENABLED = false;
#endif
    private static string m_logCache = string.Empty;
    private const int CACHE_LENGTH = 4096;
    private static bool IS_MOBILE = false;

#if UNITY_EDITOR
    private static E_LOG_TAG m_ignoreTag = E_LOG_TAG.ALL | (~(E_LOG_TAG.BATTLE));//E_LOG_TAG.ACTOR | E_LOG_TAG.AIREACT;
    public static void setOpenTag(E_LOG_TAG openTag)
    {
        m_ignoreTag = E_LOG_TAG.ALL | (~openTag);
    }
#else
    private static E_LOG_TAG m_ignoreTag = E_LOG_TAG.ALL;
#endif

    public static void init()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            IS_MOBILE = true;
        LOG_PATH = "log.txt";
    }

    public static void log(string msg, string color = "#ffffff")
    {
        if (ENABLED == false)
            return;
        Debug.Log(string.Format("<color={1}>{0}</color>", msg, color));
    }

    public static void info(string msg)
    {
        if (ENABLED == false)
            return;
        Debug.Log(msg);
    }

    public static void info(string msg, E_LOG_TAG tag)
    {
        if (ENABLED == false)
            return;
        if ((tag & m_ignoreTag) != E_LOG_TAG.NONE)
            return;
        Debug.Log("[" + tag.ToString() + "]:" + msg);
    }

    public static void warning(string msg)
    {
        Debug.LogWarning(msg);
    }

    public static void error(string msg)
    {
        if (IS_MOBILE)
            warning(msg);
        else
            Debug.LogError(msg);
    }

    public static void logException(System.Exception ex)
    {
        Debug.LogException(ex);
    }

    public static void filed(string msg)
    {
        m_logCache += msg;
        if (m_logCache.Length > CACHE_LENGTH)
        {
            FileStream fs = null;
            try
            {
                fs = File.Open(LOG_PATH, FileMode.Append);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(m_logCache);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }
            catch (System.Exception ex)
            {
                Logger.error(ex.StackTrace);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            m_logCache = string.Empty;
        }
    }

    //#if UNITY_EDITOR
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogCallInfo()
    {
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
        System.Diagnostics.StackFrame sf = st.GetFrame(1);
        var callInfo = sf.GetMethod().Name.ToString();

        Debug.Log("[" + callInfo + "] called");
    }
    //#endif
}
