


public interface IState 
{
    void onEnter(StateInfo info);
    void onUpdate();
    void onExit();

    int getState();
}
