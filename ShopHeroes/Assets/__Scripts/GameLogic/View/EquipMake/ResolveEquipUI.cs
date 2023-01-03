using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveEquipUI : ViewBase<ResolveEquipUIComp>
{
    public override string sortingLayerName => "popup";

    public override string viewID => ViewPrefabName.ResolveEquipUI;

    protected override void onInit()
    {
        base.onInit();
        contentPane.addBtn.onClick.AddListener(() => setResolveNum(true));
        contentPane.delBtn.onClick.AddListener(() => setResolveNum(false));

        contentPane.closeBtn.ButtonClickTween(hide);
        contentPane.cancelBtn.ButtonClickTween(hide);
        contentPane.resolveBtn.ButtonClickTween(resolveEquip);

    }


    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();

        contentPane.uiAnimator.CrossFade("show", 0f);
        contentPane.uiAnimator.Update(0f);
        contentPane.uiAnimator.Play("show");
    }

    protected override void DoHideAnimation()
    {
        contentPane.uiAnimator.Play("hide");

        float animLength = contentPane.uiAnimator.GetClipLength("common_popUpUI_hide");

        GameTimer.inst.AddTimer(animLength, 1, () =>
        {
            contentPane.uiAnimator.CrossFade("null", 0f);
            contentPane.uiAnimator.Update(0f);
            this.HideView();
        });

    }

    protected override void onHide()
    {
        if (GameSettingManager.inst.needShowUIAnim)
            EventController.inst.TriggerEvent(GameEventType.BagEvent.BAG_EQUIP_UPDATE);
    }

    private void setResolveNum(bool isAdd)
    {
        AudioManager.inst.PlaySound(125);
        resolveNum += isAdd ? 1 : -1;
        if (resolveNum == _data.count + 1)
        {
            resolveNum = 1;
        }
        else if (resolveNum == 0)
        {
            resolveNum = (int)_data.count;
        }
        //resolveNum = Mathf.Clamp(resolveNum, 0, (int)_data.count);
        contentPane.resolveNumTx.text = resolveNum.ToString();
        refrshResolveEquipItems();
    }

    private void resolveEquip()
    {
        EventController.inst.TriggerEvent(GameEventType.EquipEvent.EQUIP_REMOVE, _data.itemUid, resolveNum);
    }

    EquipItem _data;
    int resolveNum;
    public void SetData(EquipItem equipItem)
    {
        _data = equipItem;
        resolveNum = 1;
        contentPane.resolveNumTx.text = resolveNum.ToString();
        contentPane.titleTx.text = LanguageManager.inst.GetValueByKey("正在分解{0}", equipItem.equipConfig.name);
        contentPane.tipsTx.text = LanguageManager.inst.GetValueByKey("要分解{0}吗？", equipItem.equipConfig.name);
        contentPane.equipIcon.SetSprite(equipItem.equipConfig.equipDrawingsConfig.atlas, equipItem.equipConfig.equipDrawingsConfig.icon);


        refrshResolveEquipItems();

    }

    void refrshResolveEquipItems()
    {

        for (int i = 0; i < contentPane.items.Count; i++)
        {
            if (i < _data.equipConfig.equipDrawingsConfig.delete_material_id.Length)
            {
                contentPane.items[i].SetData(_data.equipConfig.equipDrawingsConfig.delete_material_id[i], _data.equipConfig.equipDrawingsConfig.delete_material_num[i], resolveNum);
            }
            else
            {
                contentPane.items[i].Clear();
            }
        }
    }


}
