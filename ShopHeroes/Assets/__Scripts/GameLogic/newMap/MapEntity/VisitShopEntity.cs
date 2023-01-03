using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitShopEntity : Entity
{
    private GameObject furnitureGO;
    private List<SpriteRenderer> renderObjs = new List<SpriteRenderer>();
    public bool isIndoor = true;
    //位置控制器
    ActorPosCrl actorPosCrl;
    public List<ShelfEquip> equipList = new List<ShelfEquip>();
    public int fLV = 0;
    public void setType(int _type)
    {
        type = (kTileGroupType)_type;
    }
    void Awake()
    {
        mapsize = new RectInt();
        if (actorPosCrl == null)
            actorPosCrl = GetComponent<ActorPosCrl>();
    }
    //创建对象
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

        if (string.IsNullOrEmpty(currFurniture.prefabnew))
        {
            return;
        }
        ManagerBinder.inst.Asset.loadPrefabAsync(currFurniture.prefabnew, transform, OnLoaded);
    }


    private void OnLoaded(GameObject obj)
    {
        furnitureGO = obj;
        renderObjs.Clear();
        furnitureGO.transform.localPosition = Vector3.zero;
        setDir(dir);

        foreach (Transform item in furnitureGO.transform)
        {
            var render = item.GetComponent<SpriteRenderer>();
            if (render != null) renderObjs.Add(render);
        }

        if (currFurniture.type_1 == 9)
            ResItemState();
        else if (currFurniture.type_1 == 7)
            ResetShelfData();
        UpdateSortingLayer();


        CreatPet();
    }

    //按满资源创建
    public void ResItemState()
    {
        if (currFurniture.type_2 == 4 || currFurniture.type_2 == 5) return; //储油箱，珠宝箱没有阶段转换
        string iconNameBase = currFurniture.prefabnew.Replace(".prefab", "");
        int resBoxStage = 3;
        if (renderObjs.Count == 1)
        {
            SpriteRenderer spriteRenderer = renderObjs[0];
            string iconName = iconNameBase + "_" + string.Format("{0:00}", resBoxStage);

            ManagerBinder.inst.Asset.loadMiscAsset<Sprite>(iconName, (sprite) =>
            {
                if (spriteRenderer) spriteRenderer.sprite = sprite;
            });
        }
        else
        {
            for (int i = 0; i < renderObjs.Count; i++)
            {
                SpriteRenderer spriteRenderer = renderObjs[i];
                string iconName = iconNameBase + "_" + string.Format("{0:00}", resBoxStage) + spriteRenderer.gameObject.name;
                ManagerBinder.inst.Asset.loadMiscAsset<Sprite>(iconName, (sprite) =>
                {
                    if (spriteRenderer) spriteRenderer.sprite = sprite;
                });
            }
        }
    }

    private void setDir(int _dir)
    {
        dir = _dir;
        if (dir == 0)
        {
            currSize.x = currFurniture.width;
            currSize.y = currFurniture.height;
        }
        else
        {
            currSize.x = currFurniture.height;
            currSize.y = currFurniture.width;
        }

        if (furnitureGO != null)
            furnitureGO.transform.localScale = new Vector3(dir == 0 ? 1 : -1, 1, 1);
        SetCellPosInt(cellpos);
    }

    public void setpos(int x, int y)
    {
        SetCellPosInt(new Vector3Int(x, y, 0));
    }
    public RectInt mapsize;
    //改变位置
    public override void SetCellPosInt(Vector3Int cellposint)
    {
        var Furniturecfg = FurnitureConfigManager.inst.getConfig(id);
        if (Furniturecfg.type_1 != (int)kTileGroupType.OutdoorFurniture)
        {
            cellpos = cellposint;
            actorPosCrl.SetPosition(MapUtils.IndoorCellposToMapCellPos(cellposint));
        }
        else
        {
            cellpos = cellposint;
            actorPosCrl.SetPosition(cellposint);
        }

    }
    public ShelfDisplay shelfDisplay;
    public void ResetShelfData()
    {
        if (type == kTileGroupType.Shelf)
        {
            shelfDisplay = furnitureGO.GetComponent<ShelfDisplay>();
            if (shelfDisplay != null)
            {
                var ucfg = ShelfUpgradeConfigManager.inst.getConfigByType(currFurniture.type_2, fLV);
                if (ucfg != null)
                {
                    shelfDisplay.ResetShelfUpgradeCfg(ucfg);
                    shelfDisplay.RefreshDisPlay(equipList);
                }
            }
        }
    }

    private void UpdateSortingLayer()
    {
        if (shelfDisplay != null)// 货架逻辑多一层处理
        {
            shelfDisplay.SetSortingOrder(currSize.x, currSize.y);
        }
        else
        {
            for (int i = 0; i < renderObjs.Count; i++)
            {
                var obj = renderObjs[i].transform;
                renderObjs[i].sortingOrder = MapUtils.GetTileMapOrder(obj.position.y, obj.position.x, currSize.x, currSize.y);
                if (type == kTileGroupType.OutdoorFurniture) //outdoorActor
                {
                    renderObjs[i].sortingLayerName = "outdoorActor";
                }
                else
                {
                    if (type == kTileGroupType.Carpet || type == kTileGroupType.WallFurniture)
                    {
                        renderObjs[i].sortingLayerName = "map_floor";
                        return;
                    }
                    renderObjs[i].sortingLayerName = "map_Actor";
                }
            }
        }
    }

    public OnePetInfo petInfo;
    public void CreatPet()
    {
        if (petInfo != null)
        {
            var petcfg = PetConfigManager.inst.GetConfig(petInfo.petId);
            CharacterManager.inst.GetCharacterByModel<DressUpSystem>(petcfg.model, callback: (system) =>
            {
                system.gameObject.name = "petHouse_pet";
                system.Play("rest", completeDele: (a) =>
                {
                    system.Play("idle", true);
                });

                Vector3 pos = MapUtils.CellPosToCenterPos(MapUtils.IndoorCellposToMapCellPos(this.cellpos));
                system.transform.position = pos;
                system.transform.parent = this.transform;
                system.SetSortingAndOrderLayer(type == kTileGroupType.Carpet ? "map_floor" : "map_Actor", MapUtils.GetTileMapOrder(system.transform.position.y - 0.2f, system.transform.position.x, 1, 1));
                system.SetDirection(RoleDirectionType.Left);
            });
        }
    }
}
