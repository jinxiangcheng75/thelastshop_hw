using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class shopperHead : MonoBehaviour
{
    public GUIIcon icon;

    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            EventController.inst.TriggerEvent(GameEventType.ShopperEvent.SHOPPER_START_CHECKOUT, currShopperuid);
        });
    }
    private int currShopperuid;
    public void SetShopper(ShopperData data)
    {
        currShopperuid = data.data.shopperUid;
        EquipDrawingsConfig cfg = EquipConfigManager.inst.GetEquipDrawingsCfgByEquipId(data.data.targetEquipId);
        icon.SetSprite(cfg.atlas, cfg.icon);
    }
    private Vector2 screenPoint;
    // void Update()
    // {
    //     //  Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, targetTf.position);
    //     // Vector2 localPoint;
    //     // if (RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.hudPlanel, screenPoint, FGUI.inst.uiCamera, out localPoint))
    //     // {
    //     //     transform.localPosition = localPoint;
    //     // }
    //     //this.transform.localPosition = new Vector3(Random.Range(-300, 300), Random.Range(-400, 400), 0);
    // }
}
