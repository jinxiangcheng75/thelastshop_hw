using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIHUDCol : MonoBehaviour
{
    public GameObject shopperHeadTip;    //顾客头顶气泡
    private Dictionary<int, RectTransform> tipList = new Dictionary<int, RectTransform>();
    // Start is called before the first frame update
    //ShopperMovementNotifierParam shopperMovementNotifierParam;
    void Awake()
    {
        /*shopperMovementNotifierParam = new ShopperMovementNotifierParam();
        shopperMovementNotifierParam.popupCallback += ShopperTipPopup;
        shopperMovementNotifierParam.moveCallback += ShopperMove;*/
    }
    void Start()
    {
        // this.Invoke("updateShopper", 2);
        // EventController.inst.AddListener(GameEventType.ShopperEvent.SHOPPERDATA_GETEND, updateShopper);
        //EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_COMING_NEW, creatnewShopper);
        // EventController.inst.AddListener<int>(GameEventType.ShopperEvent.SHOPPER_LEAVING, shopperLeaving);
        //EventController.inst.AddListener<IShopperMovementNotifier>(GameEventType.ShopperEvent.SHOPPER_MOVEMENT_NOTIFIER, shopperMoveNotifier);
    }

    /*private void shopperMoveNotifier(IShopperMovementNotifier notifier)
    {
        notifier.setNotifer(shopperMovementNotifierParam);
    }*/

    //泡泡处理
    // private void ShopperTipPopup(int shopperuid, kShopperPopupType popuptype, Vector3 pos)
    // {
    //     switch (popuptype)
    //     {
    //         case kShopperPopupType.Enter:
    //             creatnewShopper(shopperuid); //创建气泡节点
    //             //进场
    //             break;
    //         case kShopperPopupType.FoundTarget:
    //             //找到物品
    //             break;
    //         case kShopperPopupType.NotFound:
    //             //没有找到
    //             break;
    //         case kShopperPopupType.Checkout:
    //             //等待结账 （购买气泡）
    //             Logger.log(shopperuid + "等待结账");
    //             ShopperCheckout(shopperuid);
    //             break;
    //         case kShopperPopupType.CheckoutFailed:
    //             //
    //             break;
    //         case kShopperPopupType.Leave:
    //             shopperLeaving(shopperuid);
    //             break;
    //     }
    //     updatePopupTipPoint(shopperuid, pos);
    // }
    //移动处理
    private void ShopperMove(int shopperuid, Vector3 movepos)
    {
        updatePopupTipPoint(shopperuid, movepos);
    }

    //更新泡泡位置
    private void updatePopupTipPoint(int uid, Vector3 pos)
    {
        if (tipList.ContainsKey(uid))
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.hudPlanel, screenPoint, FGUI.inst.uiCamera, out localPoint))
            {
                tipList[uid].anchoredPosition = localPoint;
            }
        }

    }


    //顾客离开
    private void shopperLeaving(int shopperuid)
    {
        if (tipList.ContainsKey(shopperuid))
        {
            GameObject.Destroy(tipList[shopperuid].gameObject);
            tipList.Remove(shopperuid);
        }
    }
    //顾客出场
    private void creatnewShopper(int shopperuid)
    {
        ShopperData data = ShopperDataProxy.inst.GetShopperData(shopperuid);
        tipList.Add(data.data.shopperUid, creatTip(data));
    }

    //顾客结账
    private void ShopperCheckout(int shopperuid)
    {
        if (!tipList.ContainsKey(shopperuid))
        {
            creatnewShopper(shopperuid);
        }
    }
    private void updateShopper()
    {
        if (ShopperDataProxy.inst.ShopperCount() > 0)
        {
            List<ShopperData> sdata = ShopperDataProxy.inst.GetShopperList();
            for (int i = 0; i < sdata.Count; i++)
            {
                //  tipList.Add(sdata[i].shopperUId, creatTip(sdata[i]));
            }
        }
    }

    private RectTransform creatTip(ShopperData d)
    {
        var newGo = GameObject.Instantiate(shopperHeadTip, FGUI.inst.hudPlanel);
        newGo.SetActive(true);
        // newGo.transform.localPosition = new Vector3(Random.Range(-300, 300), Random.Range(-400, 400), 0);
        shopperHead head = newGo.GetComponent<shopperHead>();
        head.SetShopper(d);
        return newGo.GetComponent<RectTransform>();
    }

}
