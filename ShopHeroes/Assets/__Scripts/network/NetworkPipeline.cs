using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface INetworkSendHandler
{
    void handleSend(INetworkPackage pkg);
}

/*public interface INetworkReceivedHandler {
    void handleReceived ();
}*/
public struct NetworkPipelineParams
{
    public IHandlerFactory<IPackageHandler, kRequestHandlerType> requestHandlerFactory;
    public IHandlerFactory<IPackageHandler, kResponseHandlerType> responseHandlerFactory;
    public INetworkErrorHandler errorHandler;
    public IHandlerFactory<IResponseDispatchHandler, kResponseDispatchType> dispatchHandlerFactory;
    public Dictionary<int, bool> hotfixAccessDict;
}

public class NetworkPipeline : INetworkSendHandler, IMonoUpdater
{
    volatile bool mHasReceived;

    PackageQueueHandler<INetworkPackage> mQueueHandler;

    IHandlerFactory<IPackageHandler, kRequestHandlerType> mRequestHandlerFactory;
    IHandlerFactory<IPackageHandler, kResponseHandlerType> mResponseHandlerFactory;
    INetworkErrorHandler mErrorHandler;
    IHandlerFactory<IResponseDispatchHandler, kResponseDispatchType> mDispatchHandlerFactory;
    Dictionary<int, bool> mRequestEventDict;
    Dictionary<int, bool> mResponseEventDict;
    Dictionary<int, bool> mHotfixAccessDict;
    public NetworkPipeline(NetworkPipelineParams p)
    {

        mQueueHandler = new PackageQueueHandler<INetworkPackage>(processPackage);
        mRequestHandlerFactory = p.requestHandlerFactory;
        mResponseHandlerFactory = p.responseHandlerFactory;
        mErrorHandler = p.errorHandler;
        mDispatchHandlerFactory = p.dispatchHandlerFactory;

        mRequestEventDict = new Dictionary<int, bool>();
        mResponseEventDict = new Dictionary<int, bool>();
        mHotfixAccessDict = p.hotfixAccessDict;
        if (mHotfixAccessDict == null)
            mHotfixAccessDict = new Dictionary<int, bool>();

        mQueueHandler.start();
        log("NetworkPipeline start successfully !!!");
    }

    void processPackage(INetworkPackage pkg)
    {
        log("NetworkPipeline processPackage : " + pkg.getUrl());
        kRequestHandlerType type = pkg.getPipelineConfig().requestHandlerType;
        IPackageHandler handler = mRequestHandlerFactory.getHandler(type);
        if (mRequestEventDict.ContainsKey((int)type) == false)
        {
            handler.onSuccess += requestHandler_onSuccess;
            handler.onFailed += requestHandler_onFailed;
            mRequestEventDict[(int)type] = true;
        }
        handler.handle(pkg);
    }

    void requestHandler_onSuccess(INetworkPackage pkg)
    {
        log("NetworkPipeline requestHandler_onSuccess pkg:" + pkg.getUrl());
        // kResponseHandlerType type = pkg.getPipelineConfig().responseHandlerType;
        // IPackageHandler handler = mResponseHandlerFactory.getHandler(type);
        // if (mResponseEventDict.ContainsKey((int)type) == false)
        // {
        //     handler.onSuccess += responseHandler_onSuccess;
        //     handler.onFailed += responseHandler_onFailed;
        //     mResponseEventDict[(int)type] = true;
        // }
        // if (pkg.isHotfix())
        // {
        //     responseHandler_onSuccess(pkg);
        // }
        // else
        // {
        //     handler.handle(pkg);
        // }
        responseHandler_onSuccess(pkg);
    }

    void requestHandler_onFailed(INetworkPackage pkg, int code)//服务器无响应
    {
        log("NetworkPipeline requestHandler_onFailed pkg:" + pkg.getUrl());
        pkg.setStatusCode(code);
        if (pkg.canRetry())
        {
            mQueueHandler.pushSendQueue(pkg);
        }
        else
        {
            mQueueHandler.pushReceivedQueue(pkg);
            mHasReceived = true;
        }
    }

    void responseHandler_onSuccess(INetworkPackage pkg)
    {
        log("NetworkPipeline responseHandler_onSuccess pkg:" + pkg.getUrl());
        mQueueHandler.pushReceivedQueue(pkg);
        mHasReceived = true;
    }

    void responseHandler_onFailed(INetworkPackage pkg, int code)  //服务器响应 错误码
    {
        log("NetworkPipeline responseHandler_onFailed pkg:" + pkg.getUrl());
        pkg.setStatusCode(code);
        mQueueHandler.pushReceivedQueue(pkg);
    }

    public void handleSend(INetworkPackage pkg)
    {
        log("NetworkPipeline handleSend Package : " + pkg.getUrl());
        mQueueHandler.pushSendQueue(pkg);
    }

    public void onUpdate()
    {
        handleReceived();
    }

    void handleReceived()
    {
        if (!mHasReceived)
            return;
        INetworkPackage pkg = mQueueHandler.popReceivedQueue();
        if (pkg == null)
        {
            mHasReceived = false;
            return;
        }

        handleHotfix(pkg);

        // if (pkg.isHotfix())
        // {
        //     return;
        // }

        //handlePackageFunc //
        //var handler = mDispatchHandlerFactory.getHandler(kResponseDispatchType.Seperate);
        //if (pkg.getStatusCode() != NetworkStatusCode.Ok)
        //{
        //    //handle error
        //    log("NetworkPipeline handleReceived failed pkg: " + pkg.getUrl());
        //    mErrorHandler.handle(pkg.getStatusCode());
        //    handler.handleFailed(pkg.getCmd());
        //}
        // else
        // {
        //     //handle success
        //     log("NetworkPipeline handleReceived success pkg: " + pkg.getUrl());
        //     handler.handleSuccess(pkg.getResponse());
        // }

    }

    void extractServerTime (string msg) {
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("\"d\":[0-9]*");
        var match = regex.Match(msg);
        if(match.Success) {
            var ss = match.Value;
            var colonIndex = ss.IndexOf(":");
            if(colonIndex >= 0) {
                var timeStr = ss.Substring(colonIndex + 1);
                int.TryParse(timeStr, out int ts);
                if(ts == 0) {
                    ts = HttpMsgRspdBase.LastServerTime;
#if UNITY_EDITOR
                    Logger.log("ExtractServerTime int parse failed :" + timeStr, "#ff0000");
#endif
                } else {
                    HttpMsgRspdBase.LastServerTime = ts;
                }
#if UNITY_EDITOR
                Logger.log("UpdateServerTime:" + HttpMsgRspdBase.LastServerTime, "#ff0000");
#endif
            } else {

#if UNITY_EDITOR
                Logger.log("ExtractServerTime not found :" + ss, "#ff0000");
#endif

            }
        } else {

#if UNITY_EDITOR
            Logger.log("ExtractServerTime Failed:" + msg, "#ff0000");
#endif

        }
    }

    void handleHotfix(INetworkPackage pkg)
    {
        if (pkg.getStatusCode() == NetworkStatusCode.Ok)
        {
            kResponseHandlerType type = pkg.getPipelineConfig().responseHandlerType;
            IPackageHandler responseHandler = mResponseHandlerFactory.getHandler(type);
            (responseHandler as JsonListResponseHandler).HandleHofix(pkg, (decodedMsg) =>
            {
                string cmdstr = decodedMsg.Substring(0, 5);
                if (NetworkManager.inst.mHotfixResponseDict.TryGetValue(int.Parse(cmdstr), out bool isHotfix))
                {
                
                    //单独处理 servertime 并保存
                    extractServerTime(decodedMsg);

                    if (string.IsNullOrEmpty(decodedMsg))
                    {
                        HotfixBridge.inst.OnMessageFailed(pkg, NetworkStatusCode.DeserializeFailed);
                        return;
                    }
                    HotfixBridge.inst.OnMessageSuccess(decodedMsg);
                    if (isHotfix)
                    {
                        return;
                    }
                }
                INetworkResponse resp = ManagerBinder.inst.mNetworkMgr.mEncodeHandler.decode(decodedMsg);
                if (resp != null)
                {
#if UNITY_EDITOR
                    log("[Network] JsonResponseHandler decrypted msg:" + (resp as HttpMsgRspdBase).GetType() + " : " + (resp as HttpMsgRspdBase).GetJsonParams());
#endif
                    //保存上一次的servertime
                    HttpMsgRspdBase.LastServerTime = (resp as HttpMsgRspdBase).serverTime;

#if UNITY_EDITOR
                    Logger.log("UpdateServerTime:" + HttpMsgRspdBase.LastServerTime, "#ff0000");
#endif

                    var handler = mDispatchHandlerFactory.getHandler(kResponseDispatchType.Seperate);
                    handler.handleSuccess(resp);
                }
                else
                {
#if UNITY_EDITOR
                    log("[Network] JsonResponseHandler Failed ");
#endif
                    pkg.setStatusCode(NetworkStatusCode.DeserializeFailed);
                    var handler = mDispatchHandlerFactory.getHandler(kResponseDispatchType.Seperate);
                    mErrorHandler.handle(pkg.getStatusCode(), "EncodeHandler.decode error");
                    HotfixBridge.inst.OnMessageFailed(pkg, pkg.getStatusCode());
                    handler.handleFailed(pkg.getCmd());
                }
            },
            (fpkg, code) =>
            {
                fpkg.setStatusCode(NetworkStatusCode.DeserializeFailed);
                var handler = mDispatchHandlerFactory.getHandler(kResponseDispatchType.Seperate);
                mErrorHandler.handle(pkg.getStatusCode(), "error cmd = " + fpkg.getCmd());
                HotfixBridge.inst.OnMessageFailed(fpkg, code);

                if (NetworkManager.inst.mHotfixResponseDict.TryGetValue(fpkg.getCmd(), out bool isHotfix))
                {
                    if (isHotfix)
                    {
                        
                        return;
                    }
                }
                handler.handleFailed(fpkg.getCmd());
            });
        }
        else
        {
            var handler = mDispatchHandlerFactory.getHandler(kResponseDispatchType.Seperate);
            mErrorHandler.handle(pkg.getStatusCode(), "NetworkPipeline handleReceived failed" );
            HotfixBridge.inst.OnMessageFailed(pkg, pkg.getStatusCode());

            if (handler != null)
                handler.handleFailed(pkg.getCmd());
        }
    }

    public void clear()
    {
        mQueueHandler.abort();

        foreach (var key in mRequestEventDict.Keys)
        {
            var handler = mRequestHandlerFactory.getHandler((kRequestHandlerType)key);
            if (handler != null)
            {
                handler.onSuccess -= requestHandler_onSuccess;
                handler.onFailed -= requestHandler_onFailed;
            }
        }
        foreach (var key in mResponseEventDict.Keys)
        {
            var handler = mResponseHandlerFactory.getHandler((kResponseHandlerType)key);
            if (handler != null)
            {
                handler.onSuccess -= responseHandler_onSuccess;
                handler.onFailed -= responseHandler_onFailed;
            }
        }
        log("NetworkPipeline cleared !!!");
    }

    void log(string msg)
    {
        Logger.log(msg);
    }
}