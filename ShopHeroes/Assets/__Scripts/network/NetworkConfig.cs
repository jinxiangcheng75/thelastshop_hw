using UnityEngine;
using System.Collections;

public class NetworkConfig
{
    //"http://192.168.1.218:8021/town/api/";
    //public static string Host = "http://192.168.1.218:2222/";
    public static string Host = "http://192.168.1.241:2223/";// "http://game.shop.clashofpuzzle.com/";//"http://shop-hero.poptiger.cn:2223/"; //"http://game.shop.clashofpuzzle.com/";
    public static void SetHost(string str)
    {
        Logger.log("lua SetHost " + str);
        Host = str;
    }
    public static int ProtocolVer = 20201019;
}

public static class NetworkStatusCode
{
    public const int Ok = 0;
    public const int NoConnection = 1;
    public const int GetResponseErr = 2;
    public const int NoResponse = 3;
    public const int NullResponse = 4;
    public const int StatusFailed = 5;
    public const int NullStream = 6;
    public const int EmptyData = 7;
    public const int DeserializeFailed = 8;
}

public static class HttpRequestConst
{
    public const string HttpsMark = "https:";
    public const string Method_Get = "GET";
    public const string Method_Post = "POST";
    public const string ContentType_Default = "application/text";
    public const string ContentType_Text = "text/html;charset=utf-8";
    public const string ContentType_Form = "application/x-www-form-urlencoded";
    public const string ContentType_Binary = "application/octet-stream";
    public static int Default_Timeout = 20000;

    public static void SetDefaultTimeout(int timeout = 30000)
    {
        Logger.log("SetDefaultTimeout " + timeout.ToString());
        HttpRequestConst.Default_Timeout = timeout;
    }
}
