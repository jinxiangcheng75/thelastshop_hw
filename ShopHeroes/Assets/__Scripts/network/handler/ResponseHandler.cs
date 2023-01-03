using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ResponseHandlerFactory : AbstractHandlerFactory<IPackageHandler, kResponseHandlerType>
{
    protected override int getTypeIndex(kResponseHandlerType type)
    {
        return (int)type;
    }

    protected override int getTypeNum()
    {
        return (int)kResponseHandlerType.Binary + 1;
    }
}

public class JsonListResponseHandler : IPackageHandler
{
    public event DelPackageHandleSuccess onSuccess;
    public event DelPackageHandleFailed onFailed;
    private IEncryptHandler mDecryptHandler;
    private IMessageEncodeHandler mEncodeHandler;
    public JsonListResponseHandler(IEncryptHandler encryptHandler, IMessageEncodeHandler encodeHandler)
    {
        mDecryptHandler = encryptHandler;
        mEncodeHandler = encodeHandler;
    }

    bool RespFilter (INetworkPackage pkg) {
        if(pkg.getUrl() != "game/gate") {
            return true;
        }
        return false;
    }

    public void handle(INetworkPackage pkg)
    {
        string data = pkg.getResponseData() as string;
        //data = mDecryptHandler.handleDecryption(pkg.getPipelineConfig().decryptType, data) as string;
        log("[Network] JsonResponseHandler handle  data:" + data);
        if(GameSettingManager.ProtocolEncryption && RespFilter(pkg)) {
            data = FileUtils.ReqDec(data);
        }
        //Dictionary<string, System.Object> dataDict = null;
        IList<System.Object> dataList = null;
        try
        {
            dataList = MiniJSON.Json.Deserialize(data) as IList<System.Object>;// Dictionary<string, System.Object>;
        }
        catch (System.Exception e)
        {

            Logger.logException(e);
        }
        //
        if (dataList == null || dataList.Count <= 0)
        {
            onFailed(pkg, NetworkStatusCode.DeserializeFailed);
            return;
        }
        kEncryptType decryptType = pkg.getPipelineConfig().decryptType;
        for (int i = 0; i < dataList.Count; i++)
        {
            string item = dataList[i] as string;
            string decryptItem = mDecryptHandler.handleDecryption(decryptType, item) as string;
            // string cmdstr = decryptItem.Substring(0, 5);
            // Debug.LogError("大大大大大大" + cmdstr);
            // int cmd = int.Parse(cmdstr);
            // if (NetworkManager.inst.mHotfixResponseDict.TryGetValue(cmd, out bool isHotfix))
            // {
            //     if (string.IsNullOrEmpty(decryptItem))
            //     {
            //         HotfixBridge.inst.OnMessageFailed(pkg, NetworkStatusCode.DeserializeFailed);
            //         break;
            //     }
            //     HotfixBridge.inst.OnMessageSuccess(decryptItem);
            //     if (isHotfix)
            //     {
            //         break;
            //     }
            // }

            if (string.IsNullOrEmpty(decryptItem))
            {
                onFailed(pkg, NetworkStatusCode.DeserializeFailed);
                break;
            }

            INetworkResponse resp = mEncodeHandler.decode(decryptItem);
            if (resp != null)
            {
#if UNITY_EDITOR
                log("[Network] JsonResponseHandler decrypted msg:" + (resp as HttpMsgRspdBase).GetType() + " : " + (resp as HttpMsgRspdBase).GetJsonParams());
#endif
                //保存上一次servertime
                HttpMsgRspdBase.LastServerTime = (resp as HttpMsgRspdBase).serverTime;

#if UNITY_EDITOR
                Logger.log("UpdateServerTime:" + HttpMsgRspdBase.LastServerTime, "#ff0000");
#endif
                var pkgItem = NetworkPackage.POOL.Get();
                pkgItem.init(resp.GetCMD(), null);
                pkgItem.setResponse(resp);
                pkgItem.setStatusCode(NetworkStatusCode.Ok);
                onSuccess(pkgItem);
            }
            else
            {
#if UNITY_EDITOR
                log("[Network] JsonResponseHandler Failed ");
#endif
                onFailed(pkg, NetworkStatusCode.DeserializeFailed);
                break;
            }
        }
    }

    public void HandleHofix(INetworkPackage pkg, System.Action<string> successFunc, System.Action<INetworkPackage, int> failedFunc)
    {
        string data = pkg.getResponseData() as string;
        log("ori ResponseData :" + data);
        if(!string.IsNullOrEmpty(data) && data != "[]" && GameSettingManager.ProtocolEncryption && RespFilter(pkg)) {
            data = FileUtils.ReqDec(data);
            //data = data.Replace("\\", "");
            //data = data.Substring(1, data.Length - 2);
        }
        log("ResponseData :" + data);
        IList<System.Object> dataList = null;
        try
        {
            dataList = MiniJSON.Json.Deserialize(data) as IList<System.Object>;// Dictionary<string, System.Object>;
        }
        catch (System.Exception e)
        {
            Logger.logException(e);
        }
        //
        if (dataList == null || dataList.Count <= 0)
        {
            failedFunc(pkg, NetworkStatusCode.DeserializeFailed);
            return;
        }
        kEncryptType decryptType = pkg.getPipelineConfig().decryptType;
        for (int i = 0; i < dataList.Count; i++)
        {
            string item = dataList[i] as string;
            string decryptItem = mDecryptHandler.handleDecryption(decryptType, item) as string;
            if (string.IsNullOrEmpty(decryptItem))
            {
                failedFunc(pkg, NetworkStatusCode.DeserializeFailed);
                break;
            }
            successFunc(decryptItem);
        }
    }

    void log(string msg)
    {
        Logger.log(msg);
    }
}

public interface IMessageEncodeHandler
{
    string encode(INetworkRequest req);
    HttpMsgRspdBase decode(string msg);
}

public class DefaultMessageEncodeHandler : IMessageEncodeHandler
{
    public HttpMsgRspdBase decode(string msg)
    {
        int cmd = int.Parse(msg.Substring(0, 5));
        string content = msg.Substring(6);
        JsonData contentObj = JsonMapper.ToObject(content);
        HttpMsgRspdBase rspd = MsgObjectFactory.Instance.createRspdMsgObjectByCommand(cmd);
        if (rspd != null)
        {
            rspd.Decode(contentObj);
        }
        else
        {
            Logger.error("DefaultMessageEncodeHandler decode failed : " + msg);
        }
        return rspd;
    }

    public string encode(INetworkRequest req)
    {
        string body = req.GetCMD() + "|" + req.Encode();
        return body;
    }
}
