using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.IO;

public delegate void DelPackageHandleReady();
public delegate void DelPackageHandleSuccess(INetworkPackage request);
public delegate void DelPackageHandleFailed(INetworkPackage request, int code);

public interface IPackageHandler
{
    void handle(INetworkPackage pkg);

    event DelPackageHandleSuccess onSuccess;
    event DelPackageHandleFailed onFailed;
}

public interface IHandlerFactory<THandler, kType>
{
    THandler getHandler(kType type);
}

public abstract class AbstractHandlerFactory<THandler, kType> : IHandlerFactory<THandler, kType>
{
    THandler[] mHandlers;
    public AbstractHandlerFactory()
    {
        mHandlers = new THandler[getTypeNum()];
    }

    public void add(kType type, THandler handler)
    {
        mHandlers[getTypeIndex(type)] = handler;
    }

    public THandler getHandler(kType type)
    {
        return mHandlers[getTypeIndex(type)];
    }

    protected abstract int getTypeNum();
    protected abstract int getTypeIndex(kType type);
}

public class RequestHandlerFactory : AbstractHandlerFactory<IPackageHandler, kRequestHandlerType>
{
    protected override int getTypeIndex(kRequestHandlerType type)
    {
        return (int)type;
    }

    protected override int getTypeNum()
    {
        return (int)kRequestHandlerType.Http + 1;
    }
}

public struct RequestHandlerParams
{
    public string host;
    public bool useCookie;
    public IHandlerFactory<IStreamHandler, kStreamType> streamHandlerFactory;
    public IEncryptHandler encryptHandler;
    public IMessageEncodeHandler encodeHandler;
    //public Dictionary<string, int> markHeaders;
    public List<string> markHeaders;
    public int headerOverwriteCount;
}

public class HttpRequestHandler : IPackageHandler
{

    public class AsyncData
    {
        public INetworkPackage package;
        public HttpWebRequest request;
    }

    public event DelPackageHandleSuccess onSuccess;
    public event DelPackageHandleFailed onFailed;

    string mHost;
    bool mUseCookie;
    CookieContainer mCookies;
    WebHeaderCollection mHeaders;
    IHandlerFactory<IStreamHandler, kStreamType> mStreamHandlerFactory;
    IEncryptHandler mEncryptHandler;
    IMessageEncodeHandler mEncodeHandler;
    //Dictionary<string, int> mMarkHeaders;
    List<string> mMarkHeaders;
    List<string> mSendHeaders;
    int mHeaderOverwriteCount;
    public HttpRequestHandler(RequestHandlerParams handlerParams)
    {
        setHost(handlerParams.host);
        mStreamHandlerFactory = handlerParams.streamHandlerFactory;
        mEncryptHandler = handlerParams.encryptHandler;
        mEncodeHandler = handlerParams.encodeHandler;
        mMarkHeaders = handlerParams.markHeaders;
        mSendHeaders = new List<string>();
        if (mMarkHeaders == null)
            mMarkHeaders = new List<string>();//new Dictionary<string, int>();
        mHeaderOverwriteCount = handlerParams.headerOverwriteCount;
    }

    public void setHost(string host)
    {
        mHost = host;
        if (mHost.IndexOf(HttpRequestConst.HttpsMark) >= 0)
        {
            httpsSupport();
        }
    }
    public void setHeaderOverwriteCount(int count)
    {
        mHeaderOverwriteCount = count;
    }
    public void handle(INetworkPackage pkg)
    {
        postHttpAsync(pkg);
    }

    bool reqFilter (INetworkPackage pkg) {
        return (pkg.getUrl() != "game/gate");
    }

    void postHttpAsync(INetworkPackage pkg)
    {
        log("HttpRequestHandler postHttpAsync pkg:" + pkg.getUrl());
        HttpWebRequest req = WebRequest.CreateHttp(mHost + pkg.getUrl() + $"?v={GameSettingManager.appVersion}&t={(System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000}");
        setDefaultRequestParams(req);
        setAdditionalHeaders(req, GameSettingManager.appVersion, pkg.getTimestamp(), pkg.getCmd());
        byte[] byteData = pkg.serialize(mEncryptHandler, mEncodeHandler);

        //过滤不需要加密的url
        if(GameSettingManager.ProtocolEncryption && reqFilter(pkg)) {
            var ss = FileUtils.ReqEnc(byteData);
            var tt = FileUtils.ToHexString(ss);

#if UNITY_EDITOR
            var dec = FileUtils.ReqDec(tt);
            Logger.log("encrypted hex:" + tt + " \ndec:" + dec);

#endif

            //var b64 = System.Convert.ToBase64String(byteData);
            string b64s = "&data=" + tt;
            byteData = System.Text.Encoding.UTF8.GetBytes(b64s);

        }
        //encrypt
        req.ContentLength = byteData.Length;
        try
        {
            using (Stream rs = req.GetRequestStream())
            {
                rs.Write(byteData, 0, byteData.Length);
                //rs.Close();
            }
            System.IAsyncResult res = req.BeginGetResponse(new System.AsyncCallback(asyncPostCallback), new AsyncData
            {
                package = pkg,
                request = req,
            });
        }
        catch (System.Exception e)
        {
            //failProcess
            log("HttpRequestHandler postHttpAsync pkg:" + pkg.getUrl() + "Exception :" + e.Message);
            onFailed(pkg, NetworkStatusCode.NoConnection);
        }
    }

    void asyncPostCallback(System.IAsyncResult async)
    {
        AsyncData data = async.AsyncState as AsyncData;
        HttpWebRequest req = data.request;
        INetworkPackage pkg = data.package;
        log("HttpRequestHandler asyncPostCallback pkg:" + pkg.getUrl());
        if (req.HaveResponse == false)
        {
            log("HttpRequestHandler asyncPostCallback pkg:" + pkg.getUrl() + " fail !");
            onFailed(pkg, NetworkStatusCode.NoResponse);
            return;
        }
        var response = req.EndGetResponse(async) as HttpWebResponse;
        if (response == null)
        {
            log("HttpRequestHandler asyncPostCallback pkg:" + pkg.getUrl() + " response == null");
            onFailed(pkg, NetworkStatusCode.NullResponse);
            return;
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            log("HttpRequestHandler asyncPostCallback pkg:" + pkg.getUrl() + "response.StatusCode =" + response.StatusCode);
            response.Close();
            onFailed(pkg, NetworkStatusCode.StatusFailed);
            return;
        }

        try
        {
            System.Object content = null;
            //saveResponseHeaders(response.Headers);
            using (Stream s = response.GetResponseStream())
            {
                if (s == null)
                {
                    log("HttpRequestHandler asyncPostCallback pkg:" + pkg.getUrl() + " Stream s == null");
                    onFailed(pkg, NetworkStatusCode.NullStream);
                    return;
                }
                kStreamType type = pkg.getPipelineConfig().streamType;
                IStreamHandler handler = mStreamHandlerFactory.getHandler(type);
                content = handler.handle(s);
            }
            if (content == null)
            {
                log("HttpRequestHandler asyncPostCallback pkg:" + pkg.getUrl() + " content == null");
                onFailed(pkg, NetworkStatusCode.EmptyData);
                return;
            }

#if UNITY_EDITOR
            log("HttpRequestHandler asyncPostCallback content:" + content);
#endif
            if (pkg != null)
            {
                if (pkg.getUrl() == "game/login")
                {
                    saveResponseHeaders(response.Headers);
                }
                pkg.setResponseData(content);
                onSuccess?.Invoke(pkg);
            }
        }
        catch (System.Exception e)
        {
            Logger.logException(e);
        }
        finally
        {
            response.Close();
        }
    }

    void saveResponseHeaders(WebHeaderCollection responseHeaders)
    {
        // if (mHeaderOverwriteCount <= 0)
        //  return;
        // else
        //     mHeaderOverwriteCount--;

        for (int i = 0; i < mMarkHeaders.Count; i++)
        {
            var key = mMarkHeaders[i];
            var val = responseHeaders.Get(key);
            if (val != null)
            {
                bool inList = false;
                for (int j = 0; j < mSendHeaders.Count; j += 2)
                {
                    var sendKey = mSendHeaders[j * 2];
                    if (key.Equals(sendKey))
                    {
                        mSendHeaders[j * 2 + 1] = val;
                        inList = true;
                        log("saveResponseHeaders: (key.Equals(sendKey))  set mSendHeaders  key=" + key + "  val=" + val);
                        break;
                    }
                }
                if (inList == false)
                {
                    mSendHeaders.Add(key);
                    mSendHeaders.Add(val);
                    log("saveResponseHeaders: (inList == false)  set mSendHeaders  key=" + key + "  val=" + val);
                }
                // mHeaderOverwriteCount--;
            }
        }
    }

    void setDefaultRequestParams(HttpWebRequest req)
    {
        req.ContentType = HttpRequestConst.ContentType_Form;
        req.Method = HttpRequestConst.Method_Post;
        req.Timeout = HttpRequestConst.Default_Timeout;
        req.AllowAutoRedirect = true;
        req.ServicePoint.Expect100Continue = false;
        req.ServicePoint.UseNagleAlgorithm = false;
        req.AuthenticationLevel = AuthenticationLevel.None;
        req.AllowWriteStreamBuffering = true;
        req.UserAgent = GameManager.userAgent;

        //req.Proxy = new WebProxy("");
        //req.Credentials = new NetworkCredential("user", "pass");
        if (mUseCookie)
        {
            if (mCookies == null || mCookies.Count == 0)
            {
                req.CookieContainer = new CookieContainer();
            }
            else
            {
                req.CookieContainer = mCookies;
            }
        }
    }

    void setAdditionalHeaders(HttpWebRequest req, string appver, int timestamp, int cmd)
    {
        var headers = req.Headers;
        for (int i = 0; i < mSendHeaders.Count; i += 2)
        {
            string key = mSendHeaders[i];
            string val = mSendHeaders[i + 1];
            headers.Add(key, val);
        }

        headers.Add("ts", timestamp.ToString());
        headers.Add("vv", appver);
        headers.Add("cmd", cmd.ToString());
        if(cmd == MsgType.Request_User_Login_Cmd) {
            headers.Add("lei", FileUtils.GetDkkIndex().ToString());
        }

#if UNITY_EDITOR
        string hs = "headers:";
        foreach(var k in headers.Keys) {
            hs += "\nk:" + k + " v:" + headers.Get(k.ToString());
        }

        //Logger.log("extract cmd:" + cmd + " ts:" + timestamp);
        Logger.log(hs);
#endif

    }

    void setCookies(HttpWebResponse response)
    {
        foreach (Cookie ck in response.Cookies)
        {
            //if(ck.Domain != null && ck.Domain.IndexOf("") >= 0)
            //    ck.Domain = "";
            mCookies.Add(ck);
        }
    }

    void httpsSupport()
    {
        if (ServicePointManager.ServerCertificateValidationCallback == null)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(checkValidationResult);
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;
        }
    }

    bool checkValidationResult(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors err)
    {
        return true;
    }

    void log(string msg)
    {
        Logger.log(msg);
    }
}

public class WebSocketRequestHandler : IPackageHandler
{
    public event DelPackageHandleSuccess onSuccess;
    public event DelPackageHandleFailed onFailed;
    string mHost;
    bool mConnected;
    IHandlerFactory<IStreamHandler, kStreamType> mStreamHandlerFactory;
    IEncryptHandler mEncryptHandler;
    IMessageEncodeHandler mEncodeHandler;
    ClientWebSocket mClient;
    byte[] mSendBuffer;
    byte[] mReceiveBuffer;
    public WebSocketRequestHandler(RequestHandlerParams p)
    {
        mHost = p.host;
        mStreamHandlerFactory = p.streamHandlerFactory;
        mEncryptHandler = p.encryptHandler;
        mEncodeHandler = p.encodeHandler;

        connect();
    }

    async void connect()
    {
        mClient = new ClientWebSocket();
        //mClient.Options.SetRequestHeader("x-token", "abcdefg");
        var uri = new System.Uri("ws://");
        await mClient.ConnectAsync(uri, CancellationToken.None);//.Wait();
        mConnected = true;
        log("[Websocket] connected :" + uri.AbsolutePath);
    }

    public void handle(INetworkPackage pkg)
    {
        var bts = new byte[1024];
    }

    async void send(byte[] data)
    {
        await mClient.SendAsync(new System.ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    async void startReceive()
    {
        var buffer = new byte[1024];
        while (true)
        {
            await mClient.ReceiveAsync(new System.ArraySegment<byte>(buffer), CancellationToken.None);
            //var str = buffer.Length;
            //Logger.log(str);
        }
    }
    void httpsSupport()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                                                                          | SecurityProtocolType.Tls
                                                                          | SecurityProtocolType.Tls11
                                                                          | SecurityProtocolType.Tls12;
        if (ServicePointManager.ServerCertificateValidationCallback == null)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(checkValidationResult);
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;
        }
    }
    bool checkValidationResult(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors err)
    {
        return true;
    }

    void log(string msg)
    {
        Logger.log(msg);
    }
}

public struct PackageData
{
    public System.ArraySegment<byte> data;
}

public class ConcurrentByteBuffer
{

}

public class WebHeaderHandler
{
    List<string> mMarkHeaders;
    List<string> mSendHeaders;
    int mHeaderOverwriteCount;
    public WebHeaderHandler(List<string> markHeaders, int overwriteCount)
    {
        mHeaderOverwriteCount = overwriteCount;
        mSendHeaders = new List<string>();
        mMarkHeaders = markHeaders;
        if (mMarkHeaders == null)
            mMarkHeaders = new List<string>();
    }

    public void saveResponseHeaders(WebHeaderCollection responseHeaders)
    {
        if (mHeaderOverwriteCount <= 0)
            return;
        else
            mHeaderOverwriteCount--;

        for (int i = 0; i < mMarkHeaders.Count; i++)
        {
            var key = mMarkHeaders[i];
            var val = responseHeaders.Get(key);
            if (val != null)
            {
                bool inList = false;
                for (int j = 0; j < mSendHeaders.Count; j += 2)
                {
                    var sendKey = mSendHeaders[j * 2];
                    if (key.Equals(sendKey))
                    {
                        mSendHeaders[j * 2 + 1] = val;
                        inList = true;
                        break;
                    }
                }
                if (inList == false)
                {
                    mSendHeaders.Add(key);
                    mSendHeaders.Add(val);
                }
            }
        }
    }

    public void setAdditionalHeaders(HttpWebRequest req)
    {
        var headers = req.Headers;
        for (int i = 0; i < mSendHeaders.Count; i += 2)
        {
            string key = mSendHeaders[i];
            string val = mSendHeaders[i + 1];
            headers.Add(key, val);
        }
    }
}