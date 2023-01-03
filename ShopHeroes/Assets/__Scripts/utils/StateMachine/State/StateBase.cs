public class StateInfo //后续若有数据传送 继承此类 状态转换OnEnter接收
{

}

public abstract class StateBase : IState
{
    public virtual int getState()
    {
        return -1;
    }

    public virtual void onEnter(StateInfo info)
    {
    }

    public virtual void onExit()
    {
    }

    public virtual void onUpdate()
    {
    }
}
