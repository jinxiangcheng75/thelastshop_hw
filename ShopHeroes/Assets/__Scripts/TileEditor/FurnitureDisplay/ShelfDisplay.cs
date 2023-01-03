
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfChange_Equip_SFX_Data
{
    public SpriteRenderer spriteRenderer;
    public bool onOrOff;
    public string equipUid;
    public int isFromSlotOrBox;
    public int shelfUid;
}

//货架显示
public class ShelfDisplay : MonoBehaviour
{
    [HideInInspector]
    public int shelfUid;
    [SerializeField]
    private Dictionary<int, SpriteRenderer> spriteRendersDic;
    [SerializeField]
    private SpriteRenderer[] equipSprites;
    private Dictionary<string, int> defaultOrderDic;
    private List<SpriteRenderer> spriteRenders;
    private ShelfUpgradeConfig _shelfCfg;

    bool isInit = false;

    void Init()
    {
        spriteRenders = new List<SpriteRenderer>();
        defaultOrderDic = new Dictionary<string, int>();
        spriteRendersDic = new Dictionary<int, SpriteRenderer>();

        foreach (Transform item in gameObject.transform)
        {
            var render = item.GetComponent<SpriteRenderer>();
            if (render != null)
            {
                spriteRenders.Add(render);
                defaultOrderDic[render.name] = render.sortingOrder;

                foreach (Transform it in render.transform)
                {
                    render = it.GetComponent<SpriteRenderer>();
                    if (render != null)
                    {
                        defaultOrderDic[render.name] = render.sortingOrder;
                    }
                }
            }
        }

        for (int i = 0; i < equipSprites.Length; i++)
        {
            equipSprites[i].enabled = false;
            spriteRendersDic[int.Parse(equipSprites[i].name)] = equipSprites[i];
        }

        isInit = true;

    }

    private void Awake()
    {
        if (!isInit) Init();
    }

    public void ResetShelfUpgradeCfg(ShelfUpgradeConfig cfg)
    {
        _shelfCfg = cfg;

    }


    public void RefreshDisPlay(List<ShelfEquip> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            TakeEquipOnOrOff(list[i], 0, true);
        }
    }

    public void TakeEquipOnOrOff(ShelfEquip shelfEquip, int isAuto, bool onOrOff, int isFromSlotOrBox = 0)
    {
        if (!isInit) Init();

        EquipConfig equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(shelfEquip.equipId);
        int field = shelfEquip.fieldId;
        var spriteRenderer = spriteRendersDic[field];

        if (spriteRenderer == null)
        {
#if UNITY_EDITOR
            Logger.error($" shelfType = {_shelfCfg.type}   shelfLv = {getcurShelfLevel()}  未找到对应sprieteRender   该field: " + field);
#endif
            return;
        }

        if (onOrOff)
        {
            if (equipCfg == null)
            {
                Logger.error("货架上架时 未找到该装备 equipId:" + shelfEquip.equipId);
                return;
            }


            equipDisplayConfig cfg = ShelfDisplayConfigManager.inst.GetConfig(equipCfg.equipDrawingId, _shelfCfg.type, getcurShelfLevel());

            if (cfg != null)
            {
                float[] values = cfg.getSlotByIndex(field);

                float scale = 0;
                float posX = 0;
                float posY = 0;
                float rotZ = 0;

                if (/*true*/values == null || values.Length != 4) //暂时改为默认摆放情况
                {
                    Logger.log($"______________该装备暂未配置货架显示    equipId = {shelfEquip.equipId}   shelfType = {_shelfCfg.type}   shelfLv = {getcurShelfLevel()}    field = {field}     已设定为当前槽位的装备spriteReder的大小位置");
                    scale = spriteRenderer.transform.localScale.x;
                    posX = spriteRenderer.transform.localPosition.x;
                    posY = spriteRenderer.transform.localPosition.y;
                    rotZ = spriteRenderer.transform.localEulerAngles.z;
                    //return;
                }
                else
                {
                    scale = values[0];
                    posX = values[1];
                    posY = values[2];
                    rotZ = values[3];
                }

                spriteRenderer.transform.localScale = Vector3.one * scale;
                spriteRenderer.transform.localPosition = new Vector3(posX, posY, 0);
                spriteRenderer.transform.localRotation = Quaternion.Euler(spriteRenderer.transform.eulerAngles.x, spriteRenderer.transform.eulerAngles.y, rotZ);
            }
            else
            {
                Logger.error($"~~~~~~~~~~~~~~~~~~未找到对应装备显示配置表   equipId = {shelfEquip.equipId}   shelfType = {_shelfCfg.type}   shelfLv = {getcurShelfLevel()}    field = {field}");
            }
        }


        if (!onOrOff)
        {
            spriteRenderer.enabled = isAuto == 1;
        }
        else
        {
            string qcolor = equipCfg.equipQualityConfig.quality > 1 ? StaticConstants.qualityColor[equipCfg.equipQualityConfig.quality - 1] : "";
            //Material mat = new Material(GUIHelper.GetSceneOutlineMat());
            //mat.SetColor("_OutlineColor", string.IsNullOrEmpty(qcolor) ? GUIHelper.GetColorByColorHex("000000") : GUIHelper.GetColorByColorHex(qcolor));
            //mat.SetFloat("_Width", 0.002f);
            //spriteRenderer.material = mat;
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_OutlineColor", string.IsNullOrEmpty(qcolor) ? GUIHelper.GetColorByColorHex("000000") : GUIHelper.GetColorByColorHex(qcolor));
            materialPropertyBlock.SetFloat("_Width", 0.002f);
            spriteRenderer.SetPropertyBlock(materialPropertyBlock);


            AtlasAssetHandler.inst.GetAtlasSprite(equipCfg.equipDrawingsConfig.atlas, equipCfg.equipDrawingsConfig.icon, (gsprite) =>
            {
                spriteRenderer.enabled = isAuto == 0;
                //spriteRenderer.sprite = sprite;
                SpriteEX sex = spriteRenderer.gameObject.GetComponent<SpriteEX>() ?? spriteRenderer.gameObject.AddComponent<SpriteEX>();
                sex.mGSprite = gsprite;

            });

        }
        if (isAuto == 1)
        {
            EventController.inst.TriggerEvent(GameEventType.FurnitureDisplayEvent.ShelfChange_Equip_SFX, new ShelfChange_Equip_SFX_Data()
            { spriteRenderer = spriteRenderer, onOrOff = onOrOff, equipUid = shelfEquip.equipUid, isFromSlotOrBox = isFromSlotOrBox, shelfUid = shelfUid });//  onOrOff ：True 0 制作栏 1 储物箱  False :  shopperUid
        }

    }

    public void SetSortingLayer(string sortingLayer)
    {
        if (!isInit) Init();

        for (int i = 0; i < spriteRenders.Count; i++)
        {
            var obj = spriteRenders[i].gameObject;
            spriteRenders[i].sortingLayerName = sortingLayer;

            foreach (Transform it in obj.transform)
            {
                var render = it.GetComponent<SpriteRenderer>();
                if (render != null)
                {
                    render.sortingLayerName = sortingLayer;
                }
            }
        }
    }

    public void SetSortingOrder(int sizex, int sizey, int sortingOrder = -1)
    {
        if (!isInit) Init();

        for (int i = 0; i < spriteRenders.Count; i++)
        {
            var obj = spriteRenders[i].gameObject;
            int _sortingOrder = spriteRenders[i].sortingOrder = sortingOrder == -1 ? MapUtils.GetTileMapOrder(obj.transform.position.y, obj.transform.position.x, sizex, sizey) : sortingOrder;

            foreach (Transform it in obj.transform)
            {
                var render = it.GetComponent<SpriteRenderer>();
                if (render != null)
                {
                    render.sortingOrder = defaultOrderDic[render.name] + _sortingOrder;
                }
            }

            spriteRenders[i].sortingOrder += defaultOrderDic[obj.name];

        }
    }

    private int getcurShelfLevel()
    {
        if (_shelfCfg.level >= 11)
        {
            return 3;
        }
        else if (_shelfCfg.level >= 6)
        {
            return 2;
        }

        return 1;
    }

}
