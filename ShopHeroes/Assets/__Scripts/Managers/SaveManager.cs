using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
/// <summary>
/// 数据存储
/// </summary>
public class SaveManager : TSingletonHotfix<SaveManager>
{
    static bool debug = true;

    public static void Save<T>(T data, string path)
    {
        FileUtils.serializeData(data, path);
    }

    public static bool Load<T>(string path, ref T data)
    {
        try
        {
            if (File.Exists(path))
            {
                data = (T)FileUtils.deserializeData(path);
                return true;
            }
            return false;
        }
        catch (System.Exception e)
        {
            Logger.log(e.Message);
            return false;
        }
    }

    public static void SaveData<T>(T data, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.Serialize(fs, data);
            if (debug)
                Debug.Log("Data Saved");
        }
    }

    public static bool LoadData<T>(string path, ref T data)
    {
        try
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    data = (T)formatter.Deserialize(fs);

                    if (debug)
                        Debug.Log("Data Loaded");
                }

                return true;
            }
            return false;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }


    public static void SaveJson<T>(T data, string path)
    {
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    public static bool LoadJson<T>(string path, ref T data)
    {
        try
        {
            if (File.Exists(path))
            {
                data = JsonUtility.FromJson<T>(File.ReadAllText(path));
                return true;
            }
            return false;
        }
        catch (System.Exception e)
        {
            Logger.log(e.Message);
            return false;
        }
    }

    public static void RemoveData(string path)
    {
        File.Delete(path);
        if (debug)
            Logger.log("Data Removed");
    }

    // public static void RemovePhotoData(int key)
    // {
    //     string path = StaticConstants.photoSavePath + key + ".png";
    //     RemoveData(path);
    // }

    //保存游戏中的临时数据 T= {key:"ddddddd"}
    public void SaveGameValue<T>(string key, T value)
    {
        if (!string.IsNullOrEmpty(AccountDataProxy.inst.account))
        {
            var str = JsonUtility.ToJson(value, true);
            PlayerPrefs.SetString(AccountDataProxy.inst.account + "_" + key, str);
        }
    }

    //返回值bool为是否包含此key
    public bool GetGameValue<T>(string key, out T result)
    {
        if (!string.IsNullOrEmpty(AccountDataProxy.inst.account))
        {
            string str = PlayerPrefs.GetString(AccountDataProxy.inst.account + "_" + key);
            if (string.IsNullOrEmpty(str))
            {
                result = default(T);
                return false;
            }
            else
            {
                T data = JsonUtility.FromJson<T>(str);
                result = data;

                return true;
            }
        }
        result = default(T);
        return false;
    }

    //=======================================================================================================//
    public bool HasKey(string key, bool bindinguser = true)
    {
        string k = key;
        if (bindinguser && AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            k = k + AccountDataProxy.inst.userId;
        }
        return PlayerPrefs.HasKey(k);
    }
    public void SaveInt(string key, int value, bool bindinguser = true)
    {
        string k = key;
        if (bindinguser && AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            k = k + AccountDataProxy.inst.userId;
        }
        PlayerPrefs.SetInt(k, value);
    }

    public int GetInt(string key, bool bindinguser = true)
    {
        if (!HasKey(key)) return 0;
        string k = key;
        if (bindinguser && AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            k = k + AccountDataProxy.inst.userId;
        }
        return PlayerPrefs.GetInt(k);
    }
    public void SaveString(string key, string value, bool bindinguser = true)
    {
        string k = key;
        if (bindinguser && AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            k = k + AccountDataProxy.inst.userId;
        }
        PlayerPrefs.SetString(k, value);
    }

    public string GetString(string key, bool bindinguser = true)
    {
        if (!HasKey(key)) return string.Empty;
        string k = key;
        if (bindinguser && AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            k = k + AccountDataProxy.inst.userId;
        }
        return PlayerPrefs.GetString(k);
    }

    public void SaveFloat(string key, float value, bool bindinguser = true)
    {
        string k = key;
        if (bindinguser && AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            k = k + AccountDataProxy.inst.userId;
        }
        PlayerPrefs.SetFloat(k, value);
    }

    public float GetFloat(string key, bool bindinguser = true)
    {
        if (!HasKey(key)) return 0;
        string k = key;
        if (bindinguser && AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            k = k + AccountDataProxy.inst.userId;
        }
        return PlayerPrefs.GetFloat(k);
    }

    public void DeleteKey(string key, bool bindinguser = true)
    {
        if (!HasKey(key)) return;
        string k = key;
        if (bindinguser && AccountDataProxy.inst != null && AccountDataProxy.inst.isLogined)
        {
            k = k + AccountDataProxy.inst.userId;
        }
        PlayerPrefs.DeleteKey(k);
    }
    //=========================================================================================================================
}

