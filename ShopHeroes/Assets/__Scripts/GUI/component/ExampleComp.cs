using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用MVCS的模板
/// </summary>

public class ExampleComp : MonoBehaviour
{

}

public class ExampleView : ViewBase<ExampleComp>
{
    public override string viewID => ViewPrefabName.FurnitureUpgradeMsgBoxUI;
    public override string sortingLayerName => "window";

    protected override void onInit()
    {
    }

    protected override void onShown()
    {
    }

    protected override void onHide()
    {
    }

}

//数据中转代理仓库
public class ExampleProxy : TSingletonHotfix<ExampleProxy>, IDataModelProx
{
    public void Init()
    {

    }

    public void Clear()
    {

    }

    //Set全局变量的方法

    //GetConfig方法
}

public class ExampleSystem : BaseSystem
{
    private ExampleView panelView;

    protected override void AddListeners()
    {
        // EventController.inst.AddListener(GameEventType.SHOWUI_SHELFUPGRADEFINISHUI, ShowExamplePanel);
    }

    protected override void RemoveListeners()
    {
        // EventController.inst.RemoveListener(GameEventType.SHOWUI_SHELFUPGRADEFINISHUI, ShowExamplePanel);
    }

    private void ShowExamplePanel()
    {
        if (panelView == null)
        {
            panelView = new ExampleView();
        }

        //给Proxy类Set一下View类中要用到的全局变量
        //ExampleProxy.inst.SetTileSelectItem(item);

        //Show方法中可以传参（初始化View中的数据，但不建议这么做，因为Show的回调或者复制操作会在View面板中的OnShow之后做）
        panelView.show();
    }
}
