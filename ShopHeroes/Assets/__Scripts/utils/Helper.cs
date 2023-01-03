using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

static public class Helper
{
    public static LuaBehaviour AddLuaBehaviour(GameObject obj, string luafilename)
    {
        var behaviour = obj.AddComponent<LuaBehaviour>();
        behaviour.luaTextAssetName = luafilename;
        behaviour.setluaTxt(luafilename);
        return behaviour;
    }
    public static T DeepCopy<T>(T obj)
    {
        //如果是字符串或值类型则直接返回
        if (obj == null || obj is string || obj.GetType().IsValueType) return obj;
        object retval = Activator.CreateInstance(obj.GetType());
        System.Reflection.FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (System.Reflection.FieldInfo field in fields)
        {
            try { field.SetValue(retval, DeepCopy(field.GetValue(obj))); }
            catch { }
        }
        return (T)retval;
    }

    public static void GamePause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }

    /// 随机种子值
    public static int GetRandomSeed()
    {
        byte[] bytes = new byte[4];
        System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        rng.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }

    public static Texture2D capture(Camera camera, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0);
        rt.depth = 24;
        rt.antiAliasing = 8;
        camera.targetTexture = rt;
        camera.RenderDontRestore();
        RenderTexture.active = rt;
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
        Rect rect = new Rect(0, 0, width, height);
        texture.ReadPixels(rect, 0, 0);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return texture;
    }

    public static void AddNetworkRespListener(int msg_cmd, System.Action<HttpMsgRspdBase> successCallBack, System.Action<HttpMsgRspdBase> failCallBack = null)
    {
        NetworkEvent.SetCallback(msg_cmd,
        (successResp) =>
        {
            successCallBack((HttpMsgRspdBase)successResp);
        },
        (failedResp) =>
        {
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOX, "无法连接到服务器");
            failCallBack?.Invoke((HttpMsgRspdBase)failedResp);
        });
    }
    //获取随机数 权重数组取数组下标
    public static int getRandomValuefromweights(int[] weights)
    {
        int allnum = 0;
        foreach (int n in weights)
        {
            allnum += n;
        }

        var rand = new System.Random(GetRandomSeed());
        int r = rand.Next(0, allnum);

        allnum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] <= 0) continue;
            if (r >= allnum && r < allnum + weights[i])
            {
                return i;
            }
            allnum += weights[i];
        }

        return -1;
    }


    /// <summary>
    /// 随机概率 0%~100%  <=0% false  >=100% true
    /// </summary>
    /// <param name="point">百分点</param>
    /// <returns>true 成功   false 失败</returns>
    public static bool randomResult(int point)
    {
        if (point <= 0) return false;
        if (point >= 100) return true;

        var rand = new System.Random(GetRandomSeed());
        int r = rand.Next(0, 100);

        return r < point;
    }

    public static int[] StringParseTointArray(string str, char splitChar)
    {
        if (string.IsNullOrEmpty(str)) return null;
        string[] strarr = str.Split(new char[] { splitChar });
        int[] intArr = new int[strarr.Length];
        for (int i = 0; i < strarr.Length; i++)
        {
            intArr[i] = int.Parse(strarr[i]);
        }
        return intArr;
    }



    public static uint[] StringParseTouintArray(string str, char splitChar)
    {
        if (string.IsNullOrEmpty(str)) return null;
        string[] strarr = str.Split(new char[] { splitChar });
        uint[] intArr = new uint[strarr.Length];
        for (int i = 0; i < strarr.Length; i++)
        {
            intArr[i] = uint.Parse(strarr[i]);
        }
        return intArr;
    }

    public static float[] StringParseTofloatArray(string str, char splitChar)
    {
        if (string.IsNullOrEmpty(str)) return null;
        string[] strarr = str.Split(new char[] { splitChar });
        float[] intArr = new float[strarr.Length];
        for (int i = 0; i < strarr.Length; i++)
        {
            intArr[i] = float.Parse(strarr[i]);
        }
        return intArr;
    }
    public static string[] StringParseTostringArray(string str, char splitChar)
    {
        return str.Split(new char[] { splitChar });
    }

    //获取字符串字符长度 中文2字符
    public static int GetStringRealLen(string str)
    {

        int _placesNum = 0; // 统计字节位数

        char[] _charArray = str.ToCharArray();

        for (int i = 0; i < _charArray.Length; i++)
        {
            char _eachChar = _charArray[i];

            if (_eachChar >= 0x4e00 && _eachChar <= 0x9fa5)//判断中文字符

                _placesNum += 2;

            else if (_eachChar >= 0x0000 && _eachChar <= 0x00ff)//己2个字节判断

                _placesNum += 1;

        }

        return _placesNum;
    }

    //判断是否为单点触摸
    public static bool singleTouch()
    {
        if (Input.touchCount == 1)

            return true;

        return false;

    }
    //判断是否为多点触摸
    public static bool multipointTouch()

    {
        if (Input.touchCount > 1)
            return true;
        return false;
    }

    //判断单点触摸条件下 是否为移动触摸
    public static bool moveSingleTouch()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Moved)
            return true;
        return false;
    }

    //判断两只手至少一只为移动触摸
    public static bool moveMultiTouch()

    {
        if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            return true;
        return false;
    }

    public static bool isPoniterInScene()
    {
#if UNITY_EDITOR
        return !(Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height || Input.mousePosition.x < 0 || Input.mousePosition.y < 0);
#else
        return true;
#endif
    }

    public static float V3_Distance(Vector3Int a,Vector3Int b) 
    {
        return Vector3.Distance(a, b);
    }

    public static float V3_Distance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

}
