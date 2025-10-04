using Newtonsoft.Json;
using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.UI;

public class ShopView : MonoBehaviour, IStoreListener
{
    public Image MyPlayerCardImg;
    public Text MyPlayerCardTxt;
    public Text MyDiamondTxt;
    public Text MyGoldTxt;

    public GridLayoutGroup GoodAGrid;
    public GridLayoutGroup GoodBGrid;

    public Toggle TgAPanel;
    public Toggle TgBPanel;

    List<ShopGoodCard> cardList;
    List<ShopGoodGold> goldList;
    List<ShopGoodDiamond> diamondList;

    CardGoodItem CardTpl;
    GoldGoodItem GoldTpl;
    DiamondGoodItem DiamondTpl;

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    private CrossPlatformValidator validator;


    string orderNo = "";

    private void Awake()
    {
        NotificationCenter.Instance.AddNotifyListener(NotificationType.Paypal, onPayNotify);
        NotificationCenter.Instance.AddNotifyListener(NotificationType.Currency, onCurrencyNotify);
    }

    void Start () {
		CardTpl = Resources.Load<CardGoodItem>("Prefabs/Shop/CardGoodItem");
        GoldTpl = Resources.Load<GoldGoodItem>("Prefabs/Shop/GoldGoodItem");
        DiamondTpl = Resources.Load<DiamondGoodItem>("Prefabs/Shop/DiamondGoodItem");
        cardList = new List<ShopGoodCard>();
        goldList = new List<ShopGoodGold>();
        diamondList = new List<ShopGoodDiamond>();

        GetShopData();
        RefreshUserShopInfo();

        object shopTab = Transfer.Instance[TransferKey.ShopTab];
        if (shopTab != null)
        {
            switch((ShopTab)shopTab)
            {
                case ShopTab.Gold:
                    TgAPanel.isOn = true;
                    break;
                case ShopTab.Diamond:
                    TgBPanel.isOn = true;
                    break;
            }
            Transfer.Instance.Remove(TransferKey.ShopTab);
        }
    }

    private void OnEnable()
    {
        RefreshUserShopInfo();
    }

    void Update () {

    }

    void GetShopData() {
        ShopApi.GetShopConfig((list,error)=> {
            if (string.IsNullOrEmpty(error))
            {
                foreach (ShopGood good in list) {
                    switch (good.Type) {
                        case "card":
                            {
                                ShopGoodCard g = new ShopGoodCard();
                                g.GoodId = good.GoodId;
                                g.Icon = good.Icon;
                                g.Title = good.Title;
                                g.Price = good.Price;
                                Dictionary<string,int> para = JsonConvert.DeserializeObject< Dictionary<string, int>>(good.Param.ToString());
                                g.Vip = para["vip"];
                                g.ClubNum = para["club"];
                                g.EmojiNum = para["emoji"];
                                g.DelayNum = para["delay"];
                                g.Undercard = para["undercard"];
                                cardList.Add(g);
                            }
                            break;
                        case "gold":
                            {
                                ShopGoodGold g = new ShopGoodGold();
                                g.GoodId = good.GoodId;
                                g.Icon = good.Icon;
                                g.Title = good.Title;
                                g.Price = good.Price;
                                g.Gold = good.Num;
                                g.Gift = good.Extra;
                                goldList.Add(g);
                            }
                            break;
                        case "rmb":
                            {
                                ShopGoodDiamond g = new ShopGoodDiamond();
                                g.GoodId = good.GoodId;
                                g.Icon = good.Icon;
                                g.Title = good.Title;
                                g.Price = good.Price;
                                g.Diamond = good.Num;
                                g.Gift = good.Extra;
                                Dictionary<string, string> para = JsonConvert.DeserializeObject<Dictionary<string, string>>(good.Param.ToString());
                                if(para!=null && para.ContainsKey("appleId"))
                                g.AppleIAPId = para["appleId"];
                                diamondList.Add(g);
                            }
                            break;
                    }
                }
            }
            else
            {
                TextAsset ta = Resources.Load<TextAsset>("Json/ShopGoods");
                if (ta)
                {
                    Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(ta.text);
                    cardList = JsonConvert.DeserializeObject<List<ShopGoodCard>>(dic["vipcard"].ToString());
                    goldList = JsonConvert.DeserializeObject<List<ShopGoodGold>>(dic["gold"].ToString());
                    diamondList = JsonConvert.DeserializeObject<List<ShopGoodDiamond>>(dic["diamond"].ToString());
                }
            }
            RenderView();
        });
    }

    void RenderView() {
        foreach (ShopGoodCard card in cardList) {
            CardGoodItem go = Instantiate(CardTpl);
            go.Card = card;
            go.CardList = cardList;
            RectTransform t = go.transform as RectTransform;
            t.SetParent(GoodAGrid.transform);
            t.localScale = Vector3.one;
            t.anchoredPosition3D = Vector3.zero;
        }

        foreach (ShopGoodGold gold in goldList)
        {
            GoldGoodItem go = Instantiate(GoldTpl);
            go.Gold = gold;
            RectTransform t = go.transform as RectTransform;
            t.SetParent(GoodAGrid.transform);
            t.localScale = Vector3.one;
            t.anchoredPosition3D = Vector3.zero;
        }

        foreach (ShopGoodDiamond diamond in diamondList)
        {
            DiamondGoodItem go = Instantiate(DiamondTpl);
            go.Diamond = diamond;
            RectTransform t = go.transform as RectTransform;
            t.SetParent(GoodBGrid.transform);
            t.localScale = Vector3.one;
            t.anchoredPosition3D = Vector3.zero;
            go.OnDiamondBuyEvent = onDiamondBuyEvent;
        }

#if UNITY_IOS
        if (m_StoreController == null)
		{
			Debug.Log("purchase ------ 1");
            InitializePurchasing();
        }
#endif
    }

    public void RefreshUserShopInfo()
    {
        switch (Game.Instance.CurPlayer.Vip)
        {
            case 1:
                MyPlayerCardImg.sprite = Resources.Load<Sprite>("Textures/Common/Vip/vip_card_1");
                MyPlayerCardTxt.text = LocalizationManager.Instance.GetText("6006");
                break;
            case 2:
                MyPlayerCardImg.sprite = Resources.Load<Sprite>("Textures/Common/Vip/vip_card_2");
                MyPlayerCardTxt.text = LocalizationManager.Instance.GetText("6007");
                break;
            case 3:
                MyPlayerCardImg.sprite = Resources.Load<Sprite>("Textures/Common/Vip/vip_card_3");
                MyPlayerCardTxt.text = LocalizationManager.Instance.GetText("6008");
                break;
            default:
                MyPlayerCardImg.sprite = Resources.Load<Sprite>("Textures/Common/Vip/vip_card");
                MyPlayerCardTxt.text = LocalizationManager.Instance.GetText("6001");
                break;
        }
        MyDiamondTxt.text = Game.Instance.CurPlayer.Diamond.ToString();
        MyGoldTxt.text = Game.Instance.CurPlayer.Gold.ToString();


        Game.Instance.MainTabView.PersonViewPanel.InitPlayerInfo();
    }

    void onDiamondBuyEvent(ShopGoodDiamond diamond)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		testBuy(diamond);
#elif UNITY_ANDROID
        androidPay(diamond);
#elif UNITY_IOS
        applePay(diamond);
#endif

    }

    void testBuy(ShopGoodDiamond diamond)
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("id", diamond.GoodId.ToString());
        ShopApi.BuyShopGood(param, (rmb, coin, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Game.Instance.CurPlayer.Diamond = rmb;
                Game.Instance.CurPlayer.Gold = coin;
                Game.Instance.MainTabView.ShopViewPanel.RefreshUserShopInfo();
            }
            else
            {
                Game.Instance.ShowTips(error);
            }
        });
    }

    // 提交订单， 调起paypal支付
    void androidPay(ShopGoodDiamond diamond)
    {
        ShopApi.CommitOrder(diamond.GoodId, (rsp) =>
        {
            if (rsp.IsOk)
            {
                if (Validate.IsNotEmpty(rsp.data))
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                    jo.Call("Pay", rsp.data, diamond.Price /100.0f, diamond.Title);
                }
                else
                {
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6302"));
                }
            }
            else
            {
                Game.Instance.ShowTips(rsp.errorMsg);
            }
        });
    }

   

    #region 应用内消息监听
    // 收到后台赠送的金币或钻石
    void onCurrencyNotify(NotifyMsg msg)
    {
        if (Game.Instance.CurPlayer != null)
        {
            MyDiamondTxt.text = Game.Instance.CurPlayer.Diamond.ToString();
            MyGoldTxt.text = Game.Instance.CurPlayer.Gold.ToString();
        }
    }

    // paypal 验证
    void onPayNotify(NotifyMsg msg)
    {
        PayInfo info = msg["payInfo"] as PayInfo;
        if (info == null)
        {
            return;
        }

        ShopApi.VerfyOrder(info.orderNo, info.payId, (rsp) =>
        {
            if (rsp.IsOk)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6303"));
                Game.Instance.CurPlayer.Diamond = rsp.data.rmb;
                //Game.Instance.CurPlayer.Gold = rsp.data.gold;
                Game.Instance.MainTabView.ShopViewPanel.RefreshUserShopInfo();
            }
            else
            {
                Game.Instance.ShowTips(rsp.errorMsg);
            }
        });
    }

    // 删除监听
    private void OnDestroy()
    {
        NotificationCenter.Instance.RemoveNotifyListener(NotificationType.Paypal, onPayNotify);
        NotificationCenter.Instance.RemoveNotifyListener(NotificationType.Currency, onCurrencyNotify);
    }
    #endregion

    void applePay(ShopGoodDiamond diamond)
    {
        ShopApi.CommitAppleOrder(diamond.GoodId, (rsp) =>
        {
            if (rsp.IsOk)
            {
                if (Validate.IsNotEmpty(rsp.data))
                {
					orderNo = rsp.data;
                    BuyProductID(diamond.AppleIAPId);
                }
                else
                {
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6302"));
                }
            }
            else
            {
                Game.Instance.ShowTips(rsp.errorMsg);
            }
        });
    }
    void appleVerify(string orderNo,string payId,string receipt)
    {

        ShopApi.VerifyAppleOrder(orderNo, payId,receipt, (rsp) =>
        {
            if (rsp.IsOk)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6303"));
                Game.Instance.CurPlayer.Diamond = rsp.data.rmb;
                //Game.Instance.CurPlayer.Gold = rsp.data.gold;
                Game.Instance.MainTabView.ShopViewPanel.RefreshUserShopInfo();
            }
            else
            {
                Game.Instance.ShowTips(rsp.errorMsg);
            }
        });
    }

    #region IAP
    public void InitializePurchasing()
    {
        if (IsInitialized())
		{
		Debug.Log("purchase ------ 2");
            return;
        }
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        //添加商品  
        foreach (ShopGoodDiamond diamond in diamondList)
        {
            IDs product = new IDs();
            product.Add(diamond.AppleIAPId, new string[] { AppleAppStore.Name });
            builder.AddProduct(diamond.AppleIAPId, ProductType.Consumable,product);
        }

		Debug.Log("purchase ------ 3");
        UnityPurchasing.Initialize(this, builder);
    }
    
    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                //返回一个bool值，如果成功，则会多次调用支付回调，然后根据支付回调中的参数得到商品id，最后做处理(ProcessPurchase)  
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("初始化失败: " + error);
    }


    void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
    {
        try
        {

            //如果有服务器，服务器用这个receipt去苹果验证。  
            if (!string.IsNullOrEmpty(orderNo))
            {
                Dictionary<string, string> receiptJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(e.purchasedProduct.receipt);
                if (receiptJson.ContainsKey("Payload"))
                {
                    var receipt = receiptJson["Payload"];
                    appleVerify(orderNo, e.purchasedProduct.transactionID, receipt);
                }
                else
                {
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6301"));
                }
            }
            //var result = validator.Validate(e.purchasedProduct.receipt);
            //Debug.Log("Receipt is valid. Contents:");
            //foreach (IPurchaseReceipt productReceipt in result)
            //{
            //    Debug.Log(productReceipt.productID);
            //    Debug.Log(productReceipt.purchaseDate);
            //    Debug.Log(productReceipt.transactionID);

            //    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
            //    if (null != apple)
            //    {
            //        Debug.Log(apple.originalTransactionIdentifier);
            //        Debug.Log(apple.subscriptionExpirationDate);
            //        Debug.Log(apple.cancellationDate);
            //        Debug.Log(apple.quantity);



            //        //var receipt = receiptJson.GetString("Payload");
            //    }

            //}
            return PurchaseProcessingResult.Complete;
        }
        catch (IAPSecurityException)
        {
            Debug.Log("Invalid receipt, not unlocking content");
            return PurchaseProcessingResult.Complete;
        }
    }

    void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        //支付失败  
        Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6301"));
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", i.definition.storeSpecificId, p));
    }

    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
		RestorePurchases();
    }


    #endregion
}
