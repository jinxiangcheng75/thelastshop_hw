using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;
using DG.Tweening;
public class ShopperDealUIComp : MonoBehaviour
{
    [Header("面板")]
    public RectTransform shopperHeadTip;

    public GameObject topNpcObj;
    public GUIIcon npcHeadIcon;
    public Text npcNameTx;
    public Text npcTalkTx;
    public GameObject highPriceIconObj;
    public Text highPriceTimesTx;
    public GameObject discountsPriceObj;
    public Text discountsPriceTx;

    public GUIIcon tipBgImage;
    public GUIIcon tipBgArrowIcon;
    public Text tipShopperDisTx;
    public GUIIcon tipHeroBgIcon;
    public GUIIcon tipHeroProfessionIcon;
    public GUIIcon tipWorkerIcon;
    public RectTransform suggestInventory;  //推荐栏
    public Image[] goldState;

    public GUIIcon equipQualityIcon;
    public GUIIcon equipIcon;
    public GUIIcon equipSubTypeIcon;
    public GameObject obj_superEquipSign;
    public GameObject equipLvObj;
    public Text equipLvTx;
    public Text equipNameTx;
    public Text batch_itemNameTx;
    public Text equipPriceTx;
    public Button headTipBtn;
    public Button lastShopperBtn;
    public Button nextShopperBtn;
    public Button discountBtn;
    public Button doubleBtn;
    public Button chatBtn;
    public Button refuseBtn;
    public Button checkoutBtn;
    public GUIIcon checkoutIcon;
    public Button stockBtn;
    public Button cancelBtn;

    public Text discountTLText;
    public Text doubleTLText;
    public Text chatTLText;
    public Text refuseTLText;
    public Text checkoutTLText;

    //能量 + 小黑圆圈
    public Image discountTLImage;
    public Image doubleTLImage;
    public Image chatTLImage;
    public Image refuseTLImage;
    public Image SuggestTLImage;//推荐
    public Image checkoutTLImage;

    public Text discountBtnText;
    public Text doubleBtnText;
    public Text checkoutBtnText;
    public Text chatBtnText;

    public RectTransform SuggestBtnTF;
    public Button SuggestBtn;
    public GUIIcon SuggestIcon;
    public Text SuggestTLText;
    public Button OtherSuggestBtn;
    public Image suggestBgImg;

    [Header("推荐")]
    public Button closeSuggestInventoryBtn;
    public DynamicScrollView suggestItemList;
    public GameObject suggest_superEquipSignObj;

    [Header("详情")]
    public Button tip_infoTip;
    public GUIIcon tip_typeicon;
    public Text tip_nameTx;
    public Text tip_levelTx;
    public Text tip_disTx;
    public Text tip_countTx;
    public GUIIcon highPriceIcon;
    public Image highPriceArrowUpImg;
    public GUIIcon batch_awardIcon;
    public Button batch_awardBtn;
    public Text batch_awardCountTx;
    public Text batch_awardNameTx;
    public Text batch_awardPriceTx;
    public Button tip_awardTip;
    public Text awardTipTx;

    public Text targetCountTx;

    [Header("动画")]
    public DOTweenAnimation UI_Animation;
    public Animator topAnimator;
    public Animator uiAnimator;
    public Animator TipBgAnimator;
    public Transform middleTf;
    public AnimationCurve buttonCurve;

    public Button bgBtn;

    public GameObject discountArrowUp;
    public GameObject discountArrowDown;
    public GameObject doubleArrowUp;
    public GameObject doubleArrowDown;

    public GameObject countdownObj;
    public Text countdownTx;
}
