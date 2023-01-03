using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class GamePay : SingletonMono<GamePay>
{
    private int currPayId = -1;
    private string CurrOrderId = "";
    private string thirdOrderId = "";

    int currCheckTimerId = 0;
    private bool paying = false;
    private System.Action<bool> PayCallBack;

    //订单
    public static GamePaySaveData orderData;
    void clear()
    {
        if (currCheckTimerId > 0)
        {
            GameTimer.inst.RemoveTimer(currCheckTimerId);
            currCheckTimerId = 0;
            paying = false;
        }

        currPayId = -1;
        CurrOrderId = "";
        PayCallBack = null;
        thirdOrderId = "";
        SaveManager.inst.DeleteKey("gameOrderId");
        SaveManager.inst.DeleteKey("thirdOrderId");
        SaveManager.inst.DeleteKey("OriginalJson");
        SaveManager.inst.DeleteKey("productSign");
    }

    void OnDestroy()
    {
        SaveOrderData();
    }

    public override void init()
    {
        Helper.AddNetworkRespListener(MsgType.Response_Pay_Order_Cmd, OnResponsePayOrder);
        Helper.AddNetworkRespListener(MsgType.Response_Pay_Balance_Cmd, OnResponsePayBalance);
    }


    public void LoadOrderData()
    {
        SaveManager.LoadData(ResPathUtility.getpersistentDataPath(false) + $"gpay{AccountDataProxy.inst.userId}", ref orderData);


        if (orderData == null)
        {
            orderData = new GamePaySaveData();
            orderData.Init();
        }
        //检查sdk订单
        //SDKManager.inst.ReQueryPurchaess();
    }

    public void SaveOrderData()
    {
        if (orderData != null)
        {
            SaveManager.SaveData<GamePaySaveData>(orderData, ResPathUtility.getpersistentDataPath(false) + $"gpay{AccountDataProxy.inst.userId}");
        }
    }

    //对外调用
    #region 充值接口
    //拉取充值服务器说有商品价格
    private Dictionary<string, string> AllProductPriceList = new Dictionary<string, string>();
    //pricesid 充值商品id列表 使用'&'分割
    public void FindAllProductPriceList(string pricesid)
    {
        //Debug.Log("pricesid:" + pricesid);
        //SDKManager.inst.FindProductPriceList(pricesid);
    }

    public void addProductPriceList(string product, string price)
    {
        if (AllProductPriceList.ContainsKey(product))
        {
            AllProductPriceList[product] = price;
        }
        else
        {
            AllProductPriceList.Add(product, price);
        }
    }

    public string GetProductPrice(string Productid)
    {
        // if (AllProductPriceList.Count > 0)
        // {
        //     if (AllProductPriceList.ContainsKey(Productid))
        //     {
        //         return AllProductPriceList[Productid];
        //     }
        // }
        // return "";
        return GamePayPricecConfigManager.inst.GetMoneystr(Productid);
    }
    string currProductId = "";
    public bool Pay(int priceId, string productId, int payActivityType, int payActivityId, System.Action<bool> callback)
    {
        //停掉订单检查
        this.StopAllCoroutines();

        //FGUI.inst.showGlobalMask(0.5f);
        currPayId = priceId;
        PayCallBack = callback;
        currProductId = productId;
        //到服务器下订单
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Pay_Order()
            {
                priceId = priceId,
                payActivityType = payActivityType,
                payActivityId = payActivityId,
                platform = (int)PlatformManager.platform,
            }
        });
        return true;
    }

    #endregion
    void OnResponsePayOrder(HttpMsgRspdBase msg)
    {
        var data = (Response_Pay_Order)msg;

        if (data.errorCode == (int)EErrorCode.EEC_PayNotOpen)
        {
            EventController.inst.TriggerEvent<string, System.Action>(GameEventType.SHOWUI_OK_MSGBOX, LanguageManager.inst.GetValueByKey("充值尚未开启，敬请期待！"), null);
            return;
        }

        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            //获取当前订单成功
            CurrOrderId = data.gameOrderId;
            if (currProductId != data.productId)
            {
                Debug.LogError($"服务器下发购买商品{data.productId}与客户端商品{currProductId}不符");
            }

            //拉起sdk充值界面

            string productinfo = string.Format("{0}&{1}&{2}&{3}", data.productId, data.gameOrderId, data.productName, data.amount.ToString());
            PlatformManager.inst.PayProduct(productinfo, AccountDataProxy.inst.userId, CurrOrderId, true, SdkPayCallback);
            //FGUI.inst.showGlobalMask(5f);

            //Debug.LogError($"服务器下发购买商品{data.productId}");
            //轮询
            startCheckGamePayOrde();
        }
    }
    void OnResponsePayBalance(HttpMsgRspdBase msg)
    {
        var data = (Response_Pay_Balance)msg;

        if (currCheckTimerId == 0)
        {
            FGUI.inst.HideGolbalAwaitMask();
        }

        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {
            if (string.IsNullOrEmpty(data.productId))
            {
                //失败
                //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("充值订单好像丢了，请联系客服！"), GUIHelper.GetColorByColorHex("FF2828"));
                return;
            }

            if (orderData != null)
            {
                orderData.SetOrderState(data.purchase, 1);
                SaveOrderData();
            }
            //成功
            PayCallBack?.Invoke(true);
            clear();

            HotfixBridge.inst.TriggerLuaEvent("HideUI_GameHintUI");
            EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutBuyGoods { type = ReceiveInfoUIType.BuyGoodsComplete, atlas = data.pic1, icon = data.pic2, tag = data.tag });
            if (data.itemList.Count > 0)
            {
                if (data.itemList.Count > 1)
                {
                    List<CommonRewardData> rewardList = new List<CommonRewardData>();

                    for (int i = 0; i < data.itemList.Count; i++)
                    {
                        CommonRewardData tempData = new CommonRewardData(data.itemList[i].itemId, data.itemList[i].count, data.itemList[i].quality, data.itemList[i].itemType);
                        rewardList.Add(tempData);
                    }

                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new Award_AboutCommon { type = ReceiveInfoUIType.CommonReward, allRewardList = rewardList });
                }
                else if (data.itemList.Count == 1)
                {
                    EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.GetItem, "", 0, data.itemList[0].itemId, data.itemList[0].count));
                }
            }

            FGUI.inst.HideGolbalAwaitMask();
        }
        else
        {
            //if (orderData != null && !string.IsNullOrEmpty(data.purchase))
            //{
            //    orderData.SetOrderState(data.purchase, 1);
            //    SaveOrderData();
            //}
            //失败
            //PayCallBack?.Invoke(false);
            //clear();
            //EventController.inst.TriggerEvent(GameEventType.SHOWUI_TEXTMSGTIP, LanguageManager.inst.GetValueByKey("充值订单好像丢了，请联系客服！"), GUIHelper.GetColorByColorHex("FF2828"));
        }
    }

    private void SdkPayCallback(bool finish, string productId, string OriginalJson, string sign, string purchasetoken)
    {
        //充值回调
        if (finish)
        {
            //  int count = SaveManager.inst.GetInt("orderCount");
            //  count++;
            //保存订单
            SaveManager.inst.SaveString("gameOrderId", CurrOrderId);
            SaveManager.inst.SaveString("productId", productId);
            SaveManager.inst.SaveString("OriginalJson", OriginalJson);
            SaveManager.inst.SaveString("productSign", sign);

            //if (orderData != null)
            //{
            //    orderData.AddDate(purchasetoken, OriginalJson, sign);
            //}
            //SaveOrderData();
            //轮询
            startCheckGamePayOrde();
        }
        else
        {
            currPayId = -1;
            CurrOrderId = "";
            currCheckTimerId = 0;
            //失败
            PayCallBack?.Invoke(false);
        }
    }

    public void startCheckOrderList()
    {
        //检查sdk订单
        //SDKManager.inst.ReQueryPurchaess();

        //this.StopAllCoroutines();
        // this.StartCoroutine("checkorder");
        checkPayBalance();
    }

    IEnumerator checkorder()
    {
        if (orderData != null)
        {
            var _orderlist = orderData.GetNeedCheckOrders();
            if (_orderlist.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);
                for (int i = 0; i < _orderlist.Count; i++)
                {
                    var _order = _orderlist[i];
                    if (_order.purchaseState != 1)
                    {
                        NetworkEvent.SendRequest(new NetworkRequestWrapper()
                        {
                            req = new Request_Pay_Balance()
                            {
                                platform = (int)PlatformManager.platform,
                                //#if UNITY_ANDROID
                                purchase = _order.purchaseJson
                            }
                        });
                        //#endif
                    }
                    //2秒后检查下一个
                    yield return new WaitForSeconds(1);
                }
                //30秒后检查下一次
                yield return new WaitForSeconds(30);
                startCheckOrderList();
            }
        }
    }

    public void AddPurchaseOrderToCheckList(string _token, string _json, string _sign)
    {
        if (orderData != null)
        {
            orderData.AddDate(_token, _json, _sign);
        }
        SaveOrderData();
    }

    public void startCheckGamePayOrde()
    {
        CurrOrderId = SaveManager.inst.GetString("gameOrderId");
        //订单状态轮询
        checkGameOrde();
        GameTimer.inst.RemoveTimer(currCheckTimerId);
        currCheckTimerId = 0;
        currCheckTimerId = GameTimer.inst.AddTimer(3, 10, checkGameOrde);
    }
    public void checkGameOrde()
    {
        paying = false;
        var OriginalJson = SaveManager.inst.GetString("OriginalJson");
        //var json = new JsonData(OriginalJson);

        var productid = SaveManager.inst.GetString("productId");
        var productsign = SaveManager.inst.GetString("productSign");
        JsonData data = new JsonData();
        data["OriginalJson"] = OriginalJson;
        data["productSign"] = productsign;
        //到服务器查询订单
        // if (!string.IsNullOrEmpty(CurrOrderId))
        {
            //Debug.Log("发送消息: Request_Pay_Balance");
            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Pay_Balance()
                {
                    platform = (int)PlatformManager.platform,
#if UNITY_EDITOR
                    gameOrderId = CurrOrderId,
                    productId = productid,
#elif UNITY_ANDROID
                    gameOrderId = CurrOrderId,
                    productId = productid,
                    purchase = data.ToJson(),
#elif UNITY_IOS
                    gameOrderId = CurrOrderId,
                    productId = productid,
                    purchase = OriginalJson
#endif

                }
            });
        }
    }

    public void checkPayBalance()
    {
        NetworkEvent.SendRequest(new NetworkRequestWrapper()
        {
            req = new Request_Pay_Balance()
            {
                platform = (int)PlatformManager.platform
            }
        });
    }
}

[Serializable]
public class GamePaySaveData
{
    public List<ProductOrderData> dataList;
    public void Init()
    {
        dataList = new List<ProductOrderData>();
        Save();
    }

    public void AddDate(string tokenkey, string json, string sign)
    {
        if (string.IsNullOrEmpty(tokenkey) || string.IsNullOrEmpty(json)) return;
        var data = dataList.Find(item => item.purchaseTokenKey == tokenkey);
        if (data == null)
        {
            dataList.Add(new ProductOrderData(tokenkey, json, sign, 0));
        }
    }

    public void SetOrderState(string key, int state)
    {
        var data = dataList.Find(item => item.purchaseTokenKey == key);
        if (data != null)
        {
            data.purchaseState = state;
        }
    }

    public List<ProductOrderData> GetNeedCheckOrders()
    {
        var list = dataList.FindAll(item => item.purchaseState == 0);
        return list;
    }

    public void Save()
    {
        SaveManager.SaveData<GamePaySaveData>(this, ResPathUtility.getpersistentDataPath(false) + $"gpay{AccountDataProxy.inst.userId}");
    }
}

[Serializable]
public class ProductOrderData
{
    public string purchaseTokenKey;
    public string purchaseJson;
    public string purchaseSign;
    public int purchaseState;        //0、未核销 ，1、已核销
    public ProductOrderData(string key, string purchase, string sing, int state)
    {
        purchaseTokenKey = key;
        purchaseJson = purchase;
        purchaseState = state;
        purchaseSign = sing;
    }

    public void SetOrderState(int type)
    {
        purchaseState = type;
    }
}
