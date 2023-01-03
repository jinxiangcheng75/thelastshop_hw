using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResPathUtility
{

    public static string getstreamingAssetsPath(bool isLoad)
    {
        if (isLoad)
        {
#if UNITY_EDITOR
            return "file://" + Application.streamingAssetsPath;
#endif
#if !UNITY_EDITOR && UNITY_IPHONE
                return "file://"+Application.streamingAssetsPath;
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
                return Application.streamingAssetsPath;
#endif
        }
        else
        {
#if UNITY_EDITOR
            return Application.dataPath + "/StreamingAssets/";
#endif
#if !UNITY_EDITOR && UNITY_IPHONE
                return  Application.dataPath + "/Raw/";
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
                return "jar:file://" + Application.dataPath + "!/assets/StreamingAssets/";
#endif
        }
    }


    //缓存目录
    public static string getpersistentDataPath(bool isLoad)
    {
        if (Directory.Exists(Application.persistentDataPath + "/gamedata") == false)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/gamedata");
        }

        if (isLoad)
        {
            return "file:///" + Application.persistentDataPath + "/gamedata/";
        }
        else
        {
            return Application.persistentDataPath + "/gamedata/";
        }
    }
}
