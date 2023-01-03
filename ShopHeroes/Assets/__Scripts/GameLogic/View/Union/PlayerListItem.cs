using Mosframe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour, IDynamicScrollViewItem
{
    public Image bgImg;
    public RectTransform headIconTf;
    public Text playerNameTx;
    public GUIIcon unionIcon;
    public Text unionNameTx;
    public Text playerLvTx;
    private Button selfBtn;

    GraphicDressUpSystem headGraphicSystem;

    UnionMemberData _data;

    private void Start()
    {
        selfBtn = GetComponent<Button>();
        if (selfBtn) selfBtn.onClick.AddListener(onSelfBtnClick);
    }

    public void SetData(UnionMemberData data, string ifVal, int index)
    {
        _data = data;

        bgImg.enabled = index % 2 == 0;
        playerNameTx.text = ifVal + "<color=#a5a2a2>" + _data.unionMember.nickName.Substring(ifVal.Length) + "</color>";
        unionNameTx.text = data.unionName;
        playerLvTx.text = data.unionMember.level.ToString();

        if (headGraphicSystem == null)
        {
            CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.unionMember.gender), SpineUtils.RoleDressToHeadDressIdList(data.unionMember.roleDress), (EGender)data.unionMember.gender, callback: (system) =>
            {
                headGraphicSystem = system;
                system.transform.SetParent(headIconTf);
                system.transform.localScale = Vector3.one * 0.3f;
                system.transform.localPosition = Vector3.down * 186f;
                system.SetDirection(RoleDirectionType.Right);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacter(headGraphicSystem, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.unionMember.gender), SpineUtils.RoleDressToHeadDressIdList(data.unionMember.roleDress), (EGender)data.unionMember.gender);
        }
    }

    void onSelfBtnClick() 
    {

    }

    int _index;
    public void onUpdateItem(int index)
    {
        _index = index;
    }

}
