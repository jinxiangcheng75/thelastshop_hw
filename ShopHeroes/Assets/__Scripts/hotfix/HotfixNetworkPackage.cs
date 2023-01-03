using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class HotfixNetworkRequest : INetworkRequest
{
    int mCmd;
    string mBody;
    public HotfixNetworkRequest(int cmd, string body)
    {
        mCmd = cmd;
        mBody = body;
    }

    public string Encode()
    {
        return mBody;
    }

    public int GetCMD()
    {
        return mCmd;
    }
}

public class HotfixNetworkResponse : INetworkResponse
{
    int mCmd;
    string mContent;
    public HotfixNetworkResponse(int cmd, string content)
    {
        mCmd = cmd;
        mContent = content;
    }
    public int GetCMD()
    {
        return mCmd;
    }

    public string GetContent()
    {
        return mContent;
    }

    public bool Decode(JsonData jsonData)
    {
        return true;
    }
}

public class HotfixNetworkPackage : INetworkPackage
{
    int mCmd;
    string mUrl;
    int mRetryCount;
    int mStatusCode;
    System.Object mResponseData;
    INetworkRequest mRequest;
    INetworkResponse mResponse;
    bool mHotfix;
    PackagePipelineConfig mPipeLineConfig;

    int mTimestamp;
    public void init(int cmd, INetworkRequest request, Action<INetworkResponse> successCallback = null, Action<int> failedCallback = null)
    {
        mCmd = cmd;
        mRequest = request;
        mPipeLineConfig = PackagePipelineConfig.DefaultPipelineConfig;
        mHotfix = true;
        mUrl = NetworkCommand.GetUrl(cmd);

        mTimestamp = HttpMsgRspdBase.LastServerTime;//TimeUtils.GetNowSeconds();
    }

    public void setUrl(string url)
    {
        mUrl = url;
    }

    public bool canRetry()
    {
        return mRetryCount >= NetworkPackage.MaxRetry;
    }

    public bool deserialize()
    {
        return true;
    }

    public int getCmd()
    {
        return mCmd;
    }

    public PackagePipelineConfig getPipelineConfig()
    {
        return mPipeLineConfig;
    }

    public INetworkResponse getResponse()
    {
        return mResponse;
    }

    public object getResponseData()
    {
        return mResponseData;
    }

    public int getStatusCode()
    {
        return mStatusCode;
    }

    public string getUrl()
    {
        return mUrl;
    }

    public byte[] serialize()
    {
        throw new NotImplementedException();
    }

    public byte[] serialize(IEncryptHandler encryptHandler, IMessageEncodeHandler encodeHandler)
    {
        var req = mRequest;
        string body = encodeHandler.encode(req);
        Logger.log("HotfixNetworkPackage serialize body:" + body);
        string base64Body = encryptHandler.handleEncryption(kEncryptType.Base64, body) as string;
        //string body = mRequest.getParams();
        Logger.log("HotfixNetworkPackage serialize base64:" + base64Body);
        byte[] bts = Encoding.UTF8.GetBytes("&data=" + base64Body + "&ts=" + mTimestamp);
        return bts;
    }

    public void setResponse(INetworkResponse resp)
    {
        mResponse = resp;
    }

    public void setResponseData(object data)
    {
        mResponseData = data;
    }

    public void setStatusCode(int code)
    {
        mStatusCode = code;
    }

    public bool isHotfix()
    {
        return mHotfix;
    }

    public int getTimestamp () {
        return mTimestamp;
    }
}

