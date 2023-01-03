using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using static IndoorData;

public class ShelfContentUIView : ViewBase<ShelfContentUIComp>
{
    public override string viewID => ViewPrefabName.ShelfContentUI;
    public override string sortingLayerName => "window";

    ShopDesignItem mitem;
    Furniture mShelf;
    int curShlefUid;

    protected override void onInit()
    {
        base.onInit();

        isShowResPanel = true;
        topResPanelType = TopPlayerShowType.noSetting;

        var c = contentPane;
        c.btn_close.ButtonClickTween(() => { hide(); });
        c.btn_left.ButtonClickTween(() => pageChange(true));
        c.btn_right.ButtonClickTween(() => pageChange(false));
    }

    protected override void onShown()
    {
        base.onShown();
    }

    protected override void onHide()
    {
        base.onHide();
        if (mShelf != null)
        {
            mShelf.ReSetPos();
            mShelf.OnSelected();
        }
    }

    void pageChange(bool isLeft)
    {
        AudioManager.inst.PlaySound(21);
        setItem(UserDataProxy.inst.getNearFurniture(mitem.uid, mitem.type, isLeft));
    }

    public void setItem(ShopDesignItem item)
    {
        if (mitem != null)
        {
            if (IndoorMap.inst.GetFurnituresByUid(mitem.uid, out mShelf))
            {
                mShelf.ReSetPos();
            }
        }

        var c = contentPane;
        mitem = item;
        IndoorMap.inst.OnFurnituresSelect(mitem.uid);

        if (IndoorMap.inst.GetFurnituresByUid(mitem.uid, out mShelf))
        {
            curShlefUid = mitem.uid;
            mShelf.SetUIPosition(contentPane.shelfTf, _uiCanvas.sortingLayerName, _uiCanvas.sortingOrder + 1, mitem.config.height);
        }

        var ucfg = ShelfUpgradeConfigManager.inst.getConfigByType(mitem.config.type_2, mitem.level);

        for (int i = 0; i < 3; i++)
        {
            var sucfg = ShelfUpgradeConfigManager.inst.getConfigByType(ucfg.type, i * 5 + 1);
            var ftcfg = FurnitureConfigManager.inst.getConfig(sucfg.furniture_id);
            GUIIcon img = c.ctrl.img_levelShelfList[i];
            img.SetSprite(ftcfg.atlas, ftcfg.icon);

            if (i * 5 + 1 > ucfg.level)
            {
                GUIHelper.SetUIGray(img.transform, true);
            }
            else
            {
                GUIHelper.SetUIGray(img.transform, false);
            }
        }
        var ssucfg = ShelfUpgradeConfigManager.inst.getConfigByType(ucfg.type, ucfg.level);
        var fftcfg = FurnitureConfigManager.inst.getConfig(ssucfg.furniture_id);
        c.img_title.SetSprite(fftcfg.atlas, fftcfg.icon);

        switch (ucfg.type)
        {
            case (int)kShelfType.ColdeWeapon:
                c.ctrl.SetNodes(false, item);
                break;
            //热武器 防具
            case (int)kShelfType.ThermalWeapon:
            case (int)kShelfType.Armor:
            case (int)kShelfType.Misc:
                c.ctrl.SetNodes(true, item);
                break;
            default:
                break;
        }

        //contentPane.leftRightBtnObj.gameObject.SetActive(ShopDesignDataProxy.inst.getFurnitureNum(mitem.uid) != 1);
    }


    public void RefreshShelfGridItem(ShopDesignItem shelf)
    {
        if (curShlefUid != shelf.uid) return;
        contentPane.ctrl.RefreshShelfGridItem(shelf);
    }
}