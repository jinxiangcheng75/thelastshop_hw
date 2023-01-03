using UnityEngine;
using System.Collections;

public interface IResponseDispatchHandlerFactory
{
    IResponseDispatchHandler getHandler(kResponseDispatchType type);
}

public class ResponseDispatchHandlerFactory : AbstractHandlerFactory<IResponseDispatchHandler, kResponseDispatchType>
{
    protected override int getTypeIndex(kResponseDispatchType type)
    {
        return (int)type;
    }

    protected override int getTypeNum()
    {
        return (int)kResponseDispatchType.Seperate + 1;
    }
}

public interface IResponseDispatchHandler
{
    void handleSuccess(INetworkResponse pkg);
    void handleFailed(int cmd);
}

public class UnionResponseDispatchHandler : IResponseDispatchHandler
{

    public void handleSuccess(INetworkResponse resp)
    {
        throw new System.NotImplementedException("not implemented");
    }

    public void handleFailed(int cmd)
    {
        throw new System.NotImplementedException("not implemented");
    }

}

public class SeperateResponseDispatchHandler : IResponseDispatchHandler
{
    public void handleSuccess(INetworkResponse resp)
    {
        if (resp != null)
            NetworkEvent.OnSuccess(resp.GetCMD(), resp);
    }

    public void handleFailed(int cmd)
    {
        NetworkEvent.OnFailed(cmd, null);
    }
}
