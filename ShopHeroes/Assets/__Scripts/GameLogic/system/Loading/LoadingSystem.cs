using UnityEngine;
using System.Collections;

public class LoadingSystem : BaseSystem
{
    LoadingView mLoadingView;
    protected override void AddListeners()
    {
        EventController.inst.AddListener(GameEventType.SHOWUI_LOADINGUI, onShowUI);
        EventController.inst.AddListener(GameEventType.HIDEUI_LOADINGUI, onHideUI);
        //  EventController.inst.AddListener<int>(AssetLoadEvent.LOAD_PROGRESS, onLoadProgress);
    }

    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener(GameEventType.SHOWUI_LOADINGUI, onShowUI);
        EventController.inst.RemoveListener(GameEventType.HIDEUI_LOADINGUI, onHideUI);
    }

    public void onLoadProgress(int progress)
    {

    }
    void onShowUI()
    {
        //mLoadingView = GUIManager.OpenView<LoadingView>();
        //if (mLoadingView == null)
        //{
        //    mLoadingView = new LoadingView();
        //}
        //mLoadingView.show();
    }

    void onHideUI()
    {
        //GUIManager.HideView<LoadingView>();
        //if (mLoadingView != null && mLoadingView.isShowing)
        //{
        //    mLoadingView.hide();
        //}
    }
}
