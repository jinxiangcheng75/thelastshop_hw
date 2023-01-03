using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//地板
public class Floor : Entity
{
    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void create()
    {
        base.create();//必须先调用
        currFurniture = FurnitureConfigManager.inst.getConfig(id);
        if (currFurniture == null)
        {
            Logger.error($"无法创建家具，找不到对应{id}家具配置！");
            Destroy(this.gameObject, 1);
            return;
        }
        if (string.IsNullOrEmpty(currFurniture.sprites)) return;
        Load<Sprite>(currFurniture.sprites, OnLoaded);
    }

    private void OnLoaded(AsyncOperationHandle<Sprite> obj)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = (Sprite)obj.Result;
            SetUpState(floorstate);
        }
    }

    public void setpos(int x, int y)
    {
        SetCellPosInt(new Vector3Int(x, y, 0));
    }

    public override void SetCellPosInt(Vector3Int cellposint)
    {
        cellpos = cellposint;
        //室内物品向后推6小格
        var cpos = new Vector3Int(cellpos.x + StaticConstants.IndoorOffsetX, cellpos.y, 0);
        Vector3 pos = MapUtils.CellPosToWorldPos(cpos); // IndoorMap.inst.gameMapGrid.CellToLocal(cpos);
        pos.z = 5f;
        this.transform.localPosition = pos;
    }

    private int floorstate = 0;
    public void SetUpState(int _state)
    {
        floorstate = _state;
        if (spriteRenderer == null) return;
        if (floorstate == 0)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            if (floorstate == 1)
            {
                spriteRenderer.color = Color.red;
            }
            else if (floorstate == 2)
            {
                spriteRenderer.color = Color.green;
            }
        }
        // updateUpgradePop();
    }

}
