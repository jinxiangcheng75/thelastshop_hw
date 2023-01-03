using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

//基础墙 
public class WallBase : Entity
{
    public int index;
    public SpriteRenderer baseRenderer;
    public SpriteRenderer paperRenderer;
    public Sprite wallbase_1;
    public Sprite wallbase_2;
    public Sprite wallbase_3;
    public override void create()
    {
        base.create();

        currFurniture = FurnitureConfigManager.inst.getConfig(id);
        if (currFurniture == null)
        {
            Logger.error($"无法创建家具，找不到对应{id}家具配置！");
            Destroy(this.gameObject, 1);
            return;
        }

        if (string.IsNullOrEmpty(currFurniture.sprites)) return;
        switch (index % 3 + 1)
        {
            case 1:
                baseRenderer.sprite = wallbase_1;
                break;
            case 2:
                baseRenderer.sprite = wallbase_2;
                break;
            case 3:
                baseRenderer.sprite = wallbase_3;
                break;
        }
        if (paperRenderer == null) return;
        string str = "_" + (index % 3 + 1).ToString();
        Load<Sprite>(currFurniture.sprites + str, OnLoaded);
    }
    private void OnLoaded(AsyncOperationHandle<Sprite> obj)
    {
        if (paperRenderer != null)
        {
            paperRenderer.sprite = obj.Result;
        }
    }
    public override void SetCellPosInt(Vector3Int cellposint)
    {
        cellpos = cellposint;
        //室内物品向后推6小格
        var cpos = new Vector3Int(cellpos.x + 6, cellpos.y, 0);
        Vector3 pos = MapUtils.CellPosToWorldPos(cpos);
        pos.z = 5f;
        this.transform.position = pos;
    }
}
