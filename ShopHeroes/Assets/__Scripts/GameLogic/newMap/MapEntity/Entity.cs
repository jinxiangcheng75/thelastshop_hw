using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
//场景中 实体
public class Entity : MonoBehaviour
{
    public int id;
    [HideInInspector]
    public int uid; //对应userid
    [HideInInspector]
    public int dir = 0;
    [HideInInspector]
    public Vector2Int currSize = Vector2Int.zero;
    [HideInInspector]
    public kTileGroupType type; //分类
    [HideInInspector]
    public bool isSelected = false; //是否选中
    [HideInInspector]
    public Vector3Int cellpos;  //场景中的网格位置
    [HideInInspector]
    public bool OccupyDrag = false;

    protected FurnitureConfig currFurniture;
    public Entity()
    {
        // XLuaManager.inst.HotfixRaw(GetType().Name, this);
    }
    public virtual void create()
    {
    }
    //设置位置
    public virtual void SetCellPosInt(Vector3Int cellposint)
    {
    }
    //显示占格网格
    public virtual void ShowGrid()
    {
    }

    public virtual void HideGrid()
    {
    }

    protected void Load<T>(string resPath, System.Action<AsyncOperationHandle<T>> callback)
    {
        Addressables.LoadAssetAsync<T>(resPath).Completed += callback;
    }
    //改变方向
    //旋转 两个角度
    public virtual int Rotate()
    {
        return dir == 0 ? 1 : 0;
    }


    //选中
    public bool isPickUp = false;
    public virtual void OnSelected()
    {
        if (isSelected) return;
        AudioManager.inst.PlaySound(21);
        isSelected = true;
        ShowGrid();

    } //被选中
    public virtual void UnSelected()
    {
        OccupyDrag = false;
        isSelected = false;
        HideGrid();
    }  //取消选中

    public void SetColliderVisible(bool visible)
    {
        var col2ds = transform.GetComponentsInChildren<Collider2D>();
        foreach (var item in col2ds)
        {
            item.enabled = visible;
        }
    }

    #region 选择与拖拽操作
    protected Vector3Int offsetMousePos;
    public virtual void MouseClick(Vector3 mousepos) { }
    public virtual void MouseDrag(Vector3 lastpos, Vector3 newpos) { }
    public virtual void MousePointBlank() { }
    public virtual void MouseUp(Vector3 mousepos) { }
    public virtual void MouseDown(Vector3 mousepos) { }
    public virtual void Drag(Vector3 lastpos, Vector3 newpos) { }
    public virtual void Reset() { }
    #endregion
}
