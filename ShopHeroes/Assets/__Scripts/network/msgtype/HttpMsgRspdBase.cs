using System;

/// <summary>
/// http返回基类
/// </summary>

public abstract class HttpMsgRspdBase : INetworkResponse
{
    public static int LastServerTime;
    public int result;
    public int serverTime;
    public abstract int GetCMD();
    public abstract bool Decode(JsonData json);

#if UNITY_EDITOR
    public abstract string GetJsonParams();
#endif
}

