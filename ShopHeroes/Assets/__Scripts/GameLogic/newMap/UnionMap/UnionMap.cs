using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnionMap : SingletonMono<UnionMap>
{

    public CameraBounds cameraBounds;
    public UnionRole rolePfb;
    public List<Transform> roleCreatPosTfs;

    List<UnionRole> roles;

    public Transform cameraPoint;
    public Transform cameraPointL;

    int getOrderByRank(int rankIndex)
    {
        int order = 0;
        if (rankIndex == 0)
        {
            order = 6;
        }
        else if (rankIndex >= 1 && rankIndex <= 4)
        {
            order = 5;
        }
        else if (rankIndex >= 5 && rankIndex <= 8)
        {
            order = 4;
        }
        else if (rankIndex >= 9 && rankIndex <= 10)
        {
            order = 3;
        }
        else if (rankIndex >= 11 && rankIndex <= 14)
        {
            order = 2;
        }
        else if (rankIndex >= 15 && rankIndex <= 16)
        {
            order = 1;
        }
        else if (rankIndex >= 17 && rankIndex <= 20)
        {
            order = 0;
        }

        return order;
    }

    private void Start()
    {
        roles = new List<UnionRole>();

        RefreshRoles();
        if (D2DragCamera.inst != null)
            D2DragCamera.inst.setCameraPositionAndSaveLastPos(FGUI.inst.isLandscape ? cameraPointL.position : cameraPoint.position);

    }

    public void RefreshRoles()
    {
        for (int i = 0; i < UserDataProxy.inst.unionDetailInfo.memberList.Count && i < roleCreatPosTfs.Count; i++)
        {
            UnionMemberInfo memberInfo = UserDataProxy.inst.unionDetailInfo.memberList[i];

            if (i < roles.Count)
            {
                UnionRole role = roles[i];
                role.SetActive(true);
                role.SetData(memberInfo, roleCreatPosTfs[i], "Default", getOrderByRank(i));
            }
            else
            {
                GameObject obj = GameObject.Instantiate<GameObject>(rolePfb.gameObject);
                obj.SetActive(true);
                UnionRole role = obj.GetComponent<UnionRole>();
                roles.Add(role);
                role.SetData(memberInfo, roleCreatPosTfs[i], "Default", getOrderByRank(i));
            }

        }

        for (int i = UserDataProxy.inst.unionDetailInfo.memberList.Count; i < roles.Count; i++)
        {
            roles[i]?.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        roles.Clear();
    }

}
