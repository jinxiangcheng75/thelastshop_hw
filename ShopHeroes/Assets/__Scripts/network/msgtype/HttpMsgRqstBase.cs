using System;
using UnityEngine.Networking;

public enum kHttpMsgState {
    WAITSEND,//等待发送
    SENDING,//发送中
    STOP,//已停止
    ERROR,//发生错误
    WAITRECONNECT,//等待手动重连
}
/// <summary>
/// http请求消息基类
/// </summary>
public abstract class HttpMsgRqstBase : INetworkRequest
{
    public uint ID;

    public kHttpMsgState state;
    public UnityWebRequest www;
    public DateTime time;//发送时间
    public int sendTimes;//该消息已发送次数
    public bool responseError = false;

    public abstract int GetCMD();
    public abstract string GetJsonParams();
    public abstract string Encode();

    public int lastTs = 0; //上次请求时间-辅助Server记录在线在数

    /// <param name="sendFrequencyLimit">发送频率限制[默认0不限制]</param>
    public void Send(int sendFrequencyLimit = 0)
    {
        //NetManager.Ins.SendMsg(this, sendFrequencyLimit);
    }

    public string getParams () {
        throw new NotImplementedException();
    }
}

