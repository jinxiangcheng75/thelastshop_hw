using UnityEngine;
using System.Collections;

public interface INetworkErrorHandler
{
    void handle(int code, string errormsg);
}
public class NetworkErrorHandler : INetworkErrorHandler
{
    public void handle(int code, string errormsg)
    {
        Logger.error("NetworkErrorHandler code:" + code + ". errormsg = " + errormsg);
    }
}
