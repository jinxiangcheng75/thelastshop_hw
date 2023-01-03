using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIUnlockManager : BaseSystem
{

    List<string> ignoreViewNames;

    protected override void OnInit()
    {
        ignoreViewNames = new List<string>() { "WelfareUI", "MallUIView" };
    }

    protected override void AddListeners()
    {
        EventController.inst.AddListener<string>(GameEventType.UIUnlock.VIEW_ONSHOW, onViewShow);
        EventController.inst.AddListener(GameEventType.UIUnlock.SHOP_ONLVUP, onShopLevelUp);
        EventController.inst.AddListener(GameEventType.UIUnlock.GUIDE_END, onGuideEnd);
    }
    protected override void RemoveListeners()
    {
        EventController.inst.RemoveListener<string>(GameEventType.UIUnlock.VIEW_ONSHOW, onViewShow);
        EventController.inst.RemoveListener(GameEventType.UIUnlock.SHOP_ONLVUP, onShopLevelUp);
        EventController.inst.RemoveListener(GameEventType.UIUnlock.GUIDE_END, onGuideEnd);
    }

    //店主升级事件
    void onShopLevelUp()
    {
        foreach (var view in GUIManager.allShowingView)
        {
            updateUILockState(view.viewID);
        }
        UIUnLockConfigMrg.inst.updateAll();
        // updateUILockState(GUIManager.CurrWindow.viewID);
    }
    //界面打开事件
    void onViewShow(string viewname)
    {

        if (ignoreViewNames.Contains(viewname))
        {
            //忽略 界面内部自行筛查
            return;
        }

        updateUILockState(viewname);
        Logger.log("打开界面------------" + GUIManager.GetCurrWindowViewID());
    }
    //引导完成回调
    void onGuideEnd()
    {
        foreach (var view in GUIManager.allShowingView)
        {
            UIUnLockConfigMrg.inst.updateAll();
            updateUILockState(view.viewID);
        }
    }

    bool btnIgnoreCheck(string viewName,string uiModuleName,Transform uiModuleTf) 
    {

        if (viewName == "ShopDesignUI" && !uiModuleTf.gameObject.activeSelf) //设计界面部分按钮
            return true;

        if (ViewPrefabName.ShopperUI == viewName) return true;
        if (viewName == ViewPrefabName.MainUI && uiModuleName == "btn_welfare")
            return true;

        return false;
    }


    void updateUILockState(string viewname)
    {
        List<UIUnLockConfig> cfgs = UIUnLockConfigMrg.inst.getConfigsByUIName(viewname);
        if (cfgs.Count <= 0) return;

        //顶部
        List<UIUnLockConfig> topcfgs = UIUnLockConfigMrg.inst.getConfigsByUIName("TopPlayerInfoPanel");
        if (topcfgs != null && topcfgs.Count > 0)
        {
            for (int i = 0; i < topcfgs.Count; i++)
            {
                bool term = UIUnLockConfigMrg.inst.checkTerm(topcfgs[i]);

                var topInfoTransform = GUIHelper.FindChild(FGUI.inst.uiRootTF, "TopPlayerInfoPanel");
                if (topInfoTransform != null)
                {
                    var _btnTf = GUIHelper.FindChild(topInfoTransform, topcfgs[i].ui_btn);
                    if (_btnTf != null)
                    {
                        if (!term)
                            _btnTf.gameObject.SetActive(term);
                        // UIUnLockConfigMrg.inst.setInteractableValue(topcfgs[i].ui_btn, term);
                    }
                }
            }
            UIUnLockConfigMrg.inst.setInteractableValue("Top", true);
        }


        var cView = GUIManager.GetWindowByViewId(viewname);
        if (cView == null) return;
        Transform viewtf = cView.GetRootTf();
        if (viewtf == null) return;
        for (int i = 0; i < cfgs.Count; i++)
        {
            if (ViewPrefabName.TopPlayerInfoPanel == viewname)
            {
                continue;
            }
            var btnTf = GUIHelper.FindChild(viewtf, cfgs[i].ui_btn);
            if (btnTf != null)
            {
                bool term = UIUnLockConfigMrg.inst.checkTerm(cfgs[i]);
                UIUnLockConfigMrg.inst.setInteractableValue(btnTf.name, term);

                if (btnIgnoreCheck(viewname, cfgs[i].ui_btn, btnTf)) continue;

                Toggle toggle = btnTf.GetComponentInChildren<Toggle>();
                if (toggle != null)
                {
                    toggle.interactable = term;
                }
                Button btn = btnTf.GetComponentInChildren<Button>(true);
                if (btn != null)
                {
                    btn.interactable = term;
                }
                if (cfgs[i].showtype == 1)
                {
                    btnTf.gameObject.SetActive(term);
                }
                else if (cfgs[i].showtype == 2 && btnTf.gameObject.activeSelf)
                {
                    GUIHelper.SetUIGray(btnTf, !term);
                }
            }
        }

    }


}
