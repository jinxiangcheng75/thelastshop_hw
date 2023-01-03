using UnityEngine;
using System.Collections;

public enum kPrefKey {
    PatchVersion,
    NewInstalled,
    CatalogName,
    NeedPlayUIAnim, //0 关闭 1 开启
}

public class PrefUtils {
    static string[] PrefKeyStrings = new string[] {
        kPrefKey.PatchVersion.ToString(),
        kPrefKey.NewInstalled.ToString(),
        kPrefKey.CatalogName.ToString(),
        kPrefKey.NeedPlayUIAnim.ToString(),
    };
    public static void setInt (kPrefKey key, int val) {
        PlayerPrefs.SetInt(PrefKeyStrings[(int)key], val);
        PlayerPrefs.Save();
    }

    public static int getInt (kPrefKey key, int defaultVal = -1) {
        return PlayerPrefs.GetInt(PrefKeyStrings[(int)key], defaultVal);
    }

    public static void setString (kPrefKey key, string str) {
        PlayerPrefs.SetString(PrefKeyStrings[(int)key], str);
        PlayerPrefs.Save();
    }

    public static string getString (kPrefKey key, string defaultVal = null) {
        return PlayerPrefs.GetString(PrefKeyStrings[(int)key], defaultVal);
    }

    public static float getFloat(kPrefKey key, float defaultVal = -1)
    {
        return PlayerPrefs.GetFloat(PrefKeyStrings[(int)key], defaultVal);
    }

    public static void setFloat(kPrefKey key, float val)
    {
        PlayerPrefs.SetFloat(PrefKeyStrings[(int)key], val);
        PlayerPrefs.Save();
    }

    public static void del (kPrefKey key) {
        PlayerPrefs.DeleteKey(PrefKeyStrings[(int)key]);
        PlayerPrefs.Save();
    }

    public static void clearAll () {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
