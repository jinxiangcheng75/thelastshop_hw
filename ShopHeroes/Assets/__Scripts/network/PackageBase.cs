using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public enum kStreamType
{
    String,
    Binary,
}

public enum kEncryptType
{
    None,
    Des,
    Base64,
}

public enum kRequestHandlerType
{
    Http,
}

public enum kResponseHandlerType
{
    Json,
    Binary,
}

public enum kPipelineType
{
    Default,
}

public enum kResponseDispatchType
{
    Union,
    Seperate,
}

public sealed class PackagePipelineConfig
{
    public kEncryptType encryptType;
    public kRequestHandlerType requestHandlerType;
    public kStreamType streamType;
    public kResponseHandlerType responseHandlerType;
    public kEncryptType decryptType;
    public static readonly PackagePipelineConfig DefaultPipelineConfig = new PackagePipelineConfig()
    {
        encryptType = kEncryptType.None,
        requestHandlerType = kRequestHandlerType.Http,
        streamType = kStreamType.String,
        responseHandlerType = kResponseHandlerType.Json,
        decryptType = kEncryptType.Base64,
    };

    public static PackagePipelineConfig GetPipeline(kPipelineType type)
    {
        return DefaultPipelineConfig;
    }
}

public struct NetworkRequestWrapper
{
    public INetworkRequest req;
}

public interface INetworkPackage
{
    void init(int cmd, INetworkRequest request, System.Action<INetworkResponse> successCallback, System.Action<int> failedCallback);
    string getUrl();
    int getCmd();
    bool canRetry();
    void setStatusCode(int code);
    int getStatusCode();
    byte[] serialize();
    byte[] serialize(IEncryptHandler encryptHandler, IMessageEncodeHandler encodeHandler);
    bool deserialize();
    void setResponseData(System.Object data);
    void setResponse(INetworkResponse resp);
    INetworkResponse getResponse();
    System.Object getResponseData();
    //void onFailed ();
    //void onSuccess ();
    PackagePipelineConfig getPipelineConfig();
    bool isHotfix();

    int getTimestamp ();
}

public class NetworkPackage : INetworkPackage
{
    public static readonly ScalableObjectPool<NetworkPackage> POOL = new ScalableObjectPool<NetworkPackage>(null, null);

    public const int MaxRetry = 3;
    string mUrl;
    int mCommand;
    int mRetryCount;
    int mStatusCode;
    System.Object mResponseData;
    INetworkRequest mRequest;
    INetworkResponse mResponse;
    //System.Action<INetworkResponse> mSuccessCallback;
    //System.Action<int> mFailedCallback;
    PackagePipelineConfig mPipeLineConfig;
    bool mHotfix;
    int mTimestamp;
    public NetworkPackage() { }

    public void init(int cmd, INetworkRequest request, System.Action<INetworkResponse> successCallback = null, System.Action<int> failedCallback = null)
    {
        mRequest = request;
        mCommand = cmd;
        mUrl = NetworkCommand.GetUrl(cmd);
        mPipeLineConfig = PackagePipelineConfig.DefaultPipelineConfig;
        mHotfix = false;

        mTimestamp = HttpMsgRspdBase.LastServerTime;//TimeUtils.GetNowSeconds();
    }
    public void setUrl(string url)
    {
        mUrl = url;
    }

    public int getCmd()
    {
        return mCommand;
    }
    public byte[] serialize()
    {
        var req = mRequest as HttpMsgRqstBase;
        string body = req.GetCMD() + "|" + req.Encode();
        byte[] bts = Encoding.UTF8.GetBytes(body);
        return bts;
    }

    public byte[] serialize(IEncryptHandler encryptHandler, IMessageEncodeHandler encodeHandler)
    {
        var req = mRequest as HttpMsgRqstBase;
        string body = encodeHandler.encode(req);
        Logger.log("++++++++++" + req.GetType() + " : " + req.GetJsonParams());
        string base64Body = encryptHandler.handleEncryption(kEncryptType.Base64, body) as string;
        //string body = mRequest.getParams();
        string reqStr = "&data=" + base64Body + "&ts=" + mTimestamp;
        byte[] bts = Encoding.UTF8.GetBytes(reqStr);
        return bts;
    }

    public int serialize(byte[] buffer, int offset)
    {
        string body = "";// mRequest.getParams();
        int len = Encoding.UTF8.GetBytes(body, 0, body.Length, buffer, offset);
        return len;
    }

    public bool deserialize()
    {
        return true;
    }

    /*public void onFailed () {
        if(mFailedCallback != null)
            mFailedCallback(mStatusCode);
    }

    public void onSuccess () {
        if(mSuccessCallback != null)
            mSuccessCallback(mResponse);
    }*/

    public string getUrl()
    {
        return mUrl;
    }

    public bool canRetry()
    {
        return mRetryCount < MaxRetry;
    }

    public void setStatusCode(int code)
    {
        mRetryCount++;
        mStatusCode = code;
    }

    public int getStatusCode()
    {
        return mStatusCode;
    }

    public void setResponseData(object data)
    {
        mResponseData = data;
    }

    public object getResponseData()
    {
        return mResponseData;
    }
    public void setResponse(INetworkResponse resp)
    {
        mResponse = resp;
    }
    public INetworkResponse getResponse()
    {
        return mResponse;
    }

    public void setPipelineConfig(PackagePipelineConfig pipelineConfig)
    {
        mPipeLineConfig = pipelineConfig;
    }

    public PackagePipelineConfig getPipelineConfig()
    {
        return mPipeLineConfig;
    }

    public void clear()
    {
        mStatusCode = 0;
        mResponseData = null;
        mRetryCount = 0;
    }

    public void setHotfix(bool val)
    {
        mHotfix = val;
    }
    public bool isHotfix()
    {
        return mHotfix;
    }

    public int getTimestamp () {
        return mTimestamp;
    }
}

public interface INetworkRequest
{
    int GetCMD();
    string Encode();
}
public interface INetworkResponse
{
    int GetCMD();
}

public class UniversalNetworkRequest : INetworkRequest
{
    IList<string> mKVList;
    int mLength;
    public UniversalNetworkRequest(IList<string> kvList)
    {
        mKVList = kvList;
        mLength = getLength();
    }

    int getLength()
    {
        int len = 0;
        for (int i = 0; i < mKVList.Count; i++)
        {
            len += mKVList[i].Length;
        }
        return len;
    }

    public string getParams()
    {
        var sb = new StringBuilder(mLength);
        for (int i = 0; i < mKVList.Count; i++)
        {
            sb.Append(mKVList[i]);
        }
        return sb.ToString();
    }

    public int GetCMD()
    {
        throw new System.NotImplementedException();
    }

    public string Encode()
    {
        throw new System.NotImplementedException();
    }
}


public class TestNetworkRequest : INetworkRequest
{
    public string c_id;
    public string pos;
    public string code;
    public string userId;

    public string Encode()
    {
        throw new System.NotImplementedException();
    }

    public int GetCMD()
    {
        throw new System.NotImplementedException();
    }

    public string getParams()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("c_id=").Append(c_id)
            .Append("pos=").Append(pos)
            .Append("code=").Append(code)
            .Append("userId=").Append(userId);
        return sb.ToString();
    }
}

public class TestNetworkResponse
{
    public int a;
    public bool b;
    public string c;
}