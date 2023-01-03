using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//角色控制父类
public abstract class RoleBase : MonoBehaviour
{

    protected virtual void Init()
    {

    }

    private void Awake()
    {
        Init();
    }
}
