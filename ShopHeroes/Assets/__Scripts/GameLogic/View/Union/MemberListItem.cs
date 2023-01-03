using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class MemberListItem : MonoBehaviour, IDynamicScrollViewItem
{
    public RectTransform headIconTf;
    public Text lvTx;
    public Text nameTx;
    public GUIIcon unionJobIcon;
    public Text unionJobTx;
    public Text investTx;
    private Button selfBtn;

    GraphicDressUpSystem headGraphicSystem;

    UnionMemberInfo _data;
    string _unionId;

    private void Start()
    {
        selfBtn = GetComponent<Button>();
        if (selfBtn) selfBtn.ButtonClickTween(onSelfBtnClick);
    }

    public void SetData(UnionMemberInfo data, string unionId)
    {
        _data = data;
        _unionId = unionId;

        lvTx.text = _data.level.ToString();
        nameTx.text = data.nickName;
        nameTx.color = UserDataProxy.inst.playerData.userUid == _data.userId ? GUIHelper.GetColorByColorHex("#4af46a") : Color.white;
        unionJobTx.text = LanguageManager.inst.GetValueByKey(getString((EUnionJob)data.memberJob));
        investTx.text = LanguageManager.inst.GetValueByKey("投资：") + data.invest.ToString("N0");

        if (headGraphicSystem == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), SpineUtils.RoleDressToHeadDressIdList(data.roleDress), (EGender)data.gender, callback: (system) =>
            {
                headGraphicSystem = system;
                system.transform.SetParent(headIconTf);
                system.transform.localScale = Vector3.one * 0.35f;
                system.transform.localPosition = Vector3.down * 220f;
                system.SetDirection(RoleDirectionType.Right);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(headGraphicSystem, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), SpineUtils.RoleDressToHeadDressIdList(data.roleDress), (EGender)data.gender);
        }

    }

    private void onSelfBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.SocialEvent.REQUEST_OTHERUSERDATA, _data.userId);
    }

    string getString(EUnionJob eUnionJob)
    {
        string result = "";
        switch (eUnionJob)
        {
            case EUnionJob.Common:
                result = "成员";
                unionJobIcon.SetSprite("union_atlas", "gonghui_huiyuan");
                break;
            case EUnionJob.Manager:
                result = "管理员";
                unionJobIcon.SetSprite("union_atlas", "gonghui_huiyuan");
                break;
            case EUnionJob.President:
                result = "会长";
                unionJobIcon.SetSprite("union_atlas", "gonghui_huizhang");
                break;
        }

        return result;
    }

    int _index;
    public void onUpdateItem(int index)
    {
        _index = index;
    }
}
