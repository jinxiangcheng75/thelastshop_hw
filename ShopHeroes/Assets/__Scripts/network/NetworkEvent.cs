using System.Collections;
using System.Collections.Generic;
using com.poptiger.events;

namespace com.poptiger.events
{
    public delegate void EventDelegate();
    public delegate void EventDelegate<T0>(T0 p0);
    public delegate void EventDelegate<T0, T1>(T0 p0, T1 p1);
    public delegate void EventDelegate<T0, T1, T2>(T0 p0, T1 p1, T2 p2);
}

public class NetworkEvent : TSingletonHotfix<NetworkEvent>
{
    public event EventDelegate<NetworkRequestWrapper> onSendRequest;
    public event EventDelegate<INetworkPackage> onSendPackage;
    void sendRequest(NetworkRequestWrapper wrapper)
    {
        if (onSendRequest != null)
            onSendRequest(wrapper);
    }
    void sendPackage(INetworkPackage pkg)
    {
        if (onSendPackage != null)
            onSendPackage(pkg);
    }


    public static void SendRequest(NetworkRequestWrapper wrapper)
    {
        inst.sendRequest(wrapper);
    }

    public static void SendPackage(INetworkPackage pkg)
    {
        inst.sendPackage(pkg);
    }

    public static void SetCallback(int cmd, System.Action<INetworkResponse> successCallback, System.Action<INetworkResponse> failedCallback = null)
    {
        inst.Success.setCallback(cmd, successCallback);
        if (failedCallback != null)
            inst.Fail.setCallback(cmd, failedCallback);
    }

    public static void OnSuccess(int cmd, INetworkResponse resp)
    {
        inst.Success.call(cmd, resp);
    }

    public static void OnFailed(int cmd, INetworkResponse resp)
    {
        inst.Fail.call(cmd, resp);
    }

    public static void clear()
    {
        inst.Success.clear();
        inst.Fail.clear();
    }

    readonly CallbackHelper<int, INetworkResponse> Success = new CallbackHelper<int, INetworkResponse>();
    readonly CallbackHelper<int, INetworkResponse> Fail = new CallbackHelper<int, INetworkResponse>();
    public class CallbackHelper<T, TResult>
    {

        Dictionary<T, System.Delegate> mDelTable = new Dictionary<T, System.Delegate>();

        public void setCallback(T cmd, System.Action<TResult> callback)
        {
            mDelTable[cmd] = callback;
        }

        public void call(T cmd, TResult response)
        {
            System.Delegate del;
            if (mDelTable.TryGetValue(cmd, out del))
            {
                var func = del as System.Action<TResult>;
                if (func != null)
                {
                    func(response);

                }
                else
                {
                    throw new System.Exception(cmd.ToString());
                }
            }
        }

        public void clear()
        {
            mDelTable.Clear();
        }
    }
}
