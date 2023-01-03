using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mosframe;

public class ShopClothingItemComp : MonoBehaviour, IPointerClickHandler, IDynamicScrollViewItem
{
    //public Action<int, ShopClothingItemComp> isCurHandler;
    Action<ShopClothingItemComp, bool> clickHandler;
    public Image icon;
    public GUIIcon saleBgIcon;
    public GUIIcon saleIcon;
    public GameObject selectImg;
    public GameObject buyObj;
    public GameObject buySelectObj;
    public GameObject glodIcon;
    public GameObject gemIcon;
    public Image buyImg;
    public Text buyGemText;
    public Image select;
    public Image grayImg;
    public Text lockText;
    public bool clickTime;
    public int index = 0;
    private RoleSubTypeData data;
    public OverrideAnimatorButton selfBtn;
    public Animator btnAnim;

    int timerId = 0;
    private void Awake()
    {
        selfBtn.isItem = true;
        selfBtn.onClick.AddListener(() =>
        {
            AudioManager.inst.PlaySound(11);
            //Logger.error("我点击了DressId为 " + data.config.id + " 的小Item");
            clickHandler?.Invoke(this, true);
        });
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
    }

    public RoleSubTypeData Data { get { return data; } set { } }

    public void InitData(RoleSubTypeData data, EGender curSex, Action<ShopClothingItemComp, bool> clickAction, int index)
    {
        clickTime = false;
        gameObject.SetActive(true);
        this.data = data;

        clickHandler = clickAction;
        string iconName = "";
        icon.sprite = null;
        selectImg.SetActive(data.isSelect);
        select.gameObject.SetActive(data.isSelect);

        iconName = data.config.icon;

        if (!string.IsNullOrEmpty(data.config.val))
        {
            Color tempColor;
            ColorUtility.TryParseHtmlString(data.config.val, out tempColor);
            icon.GetComponent<GUIIcon>().SetSprite(CreatRoleProxy.inst.atlasName, "dianzhu_chunbaiyuan");
            icon.color = tempColor;
        }
        else if (data.config.name.StartsWith("无"))
        {
            icon.color = Color.white;
            icon.GetComponent<GUIIcon>().SetSprite(ShopkeeperDataProxy.inst.atlasName, "huanzhuang_buxuanze");
        }
        else
        {
            icon.color = Color.white;
            icon.GetComponent<GUIIcon>().SetSprite("ClotheIcon_atlas", iconName);
        }

        if (data.isSelect)
        {
            //buyObj.GetComponent<Image>().sprite = ManagerBinder.inst.Asset.getSprite("Assets/GUI2D/SpriteAtlas/dressup_atlas.spriteatlas", "huanzhuang_xiaofeidi2");
            buySelectObj.SetActiveTrue();
            clickHandler?.Invoke(this, false);
        }
        else
        {
            //buyObj.GetComponent<Image>().sprite = ManagerBinder.inst.Asset.getSprite("Assets/GUI2D/SpriteAtlas/dressup_atlas.spriteatlas", "huanzhuang_xiaofeidi1");
            buySelectObj.SetActiveFalse();
        }

        buyObj.SetActive(data.config.guide == 0);

        saleBgIcon.iconImage.enabled = false;
        saleIcon.iconImage.enabled = false;
        grayImg.enabled = false;
        lockText.enabled = false;
        if (data.config.get_type == 1)
        {
            buyGemText.text = data.config.price_money.ToString();
            glodIcon.SetActive(true);
            gemIcon.SetActive(false);
        }
        else if (data.config.get_type == 2)
        {
            buyGemText.text = data.config.price_diamond.ToString();
            gemIcon.SetActive(true);
            glodIcon.SetActive(false);
        }
        else if (data.config.get_type == 3)
        {
            buyObj.SetActive(false);
            if(data.config.guide != 1)
            {
                if (HotfixBridge.inst.GetDirectPurchaseDataById(data.config.sale_id, out DirectPurchaseData directPurchaseData))
                {
                    saleBgIcon.iconImage.enabled = true;
                    saleIcon.iconImage.enabled = true;
                    saleBgIcon.SetSprite(directPurchaseData.bgIconAtlas, directPurchaseData.bgIcon);
                    saleIcon.SetSprite(directPurchaseData.iconAtlas, directPurchaseData.icon);
                }
                else
                {
                    grayImg.enabled = true;
                    lockText.enabled = true;
                    lockText.text = LanguageManager.inst.GetValueByKey("未解锁");
                    //Logger.error("没有该礼包" + data.config.sale_id + "    装饰的id是" + data.config.id);
                }
            }
        }
        else if (data.config.get_type == 4)
        {
            if(data.config.guide != 1)
            {
                buyObj.SetActive(false);
                grayImg.enabled = true;
                lockText.enabled = true;
                lockText.text = LanguageManager.inst.GetValueByKey("大亨之路解锁");
            }
        }

        if (index < 8)
            showAnim(index);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //isCurHandler?.Invoke(data.config.type_1, this);
    }

    public void Clear()
    {
        data = null;
        icon.sprite = null;
        select.gameObject.SetActive(false);
        selectImg.SetActive(false);
        gameObject.SetActive(false);
    }

    void showAnim(int index)
    {
        //btnAnim.enabled = true;
        //gameObject.SetActive(false);
        //timerId = GameTimer.inst.AddTimer(0.05f + 0.05f * index, 1, () =>
        //{
        //    gameObject.SetActive(true);
        //    btnAnim.Play("show");
        //});
    }

    public void clearAnim()
    {
        gameObject.SetActive(false);
        //btnAnim.enabled = false;
        GameTimer.inst.RemoveTimer(timerId);
    }
}
