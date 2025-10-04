using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiamondDialog : MonoBehaviour, IPointerClickHandler
{

    public Button BuyBtn;
    public Button CloseBtn;

    public Text AmountTxt;
    public Text TitleTxt;

    public Text ClubNumTxt1;
    public Text ClubNumTxt2;
    public Text ClubNumTxt3;
    public Text EmojNumTxt1;
    public Text EmojNumTxt2;
    public Text EmojNumTxt3;
    public Text DelayTimeTxt1;
    public Text DelayTimeTxt2;
    public Text DelayTimeTxt3;
    public Image ShowPublicImg1;
    public Image ShowPublicImg2;
    public Image ShowPublicImg3;

    public Image SMask;
    public Image GMask;
    public Image BMask;
    
    public Image CoinImg;
    public Text CoinTxt;

    public RectTransform Wrapper;
    public GameObject PersonCardContent;
    public GameObject GoldCoinContent;
    public List<ShopGoodCard> CardList;

    // Use this for initialization
    void Start () {
        gameObject.SetActive(true);
        CloseBtn.onClick.AddListener(delegate {
            Close();
        });
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowPersonCardDialog(ShopGoodCard card) {
        TitleTxt.text = LocalizationManager.Instance.GetText("6100");
        Wrapper.sizeDelta = new Vector2(Wrapper.sizeDelta.x, 676);

        if (CardList != null)
        {
            foreach (ShopGoodCard item in CardList)
            {
                if (item.Vip == 1) {
                    ClubNumTxt1.text = "" + item.ClubNum;
                    EmojNumTxt1.text = "" + item.EmojiNum;
                    DelayTimeTxt1.text = "" + item.DelayNum;
                    ShowPublicImg1.gameObject.SetActive(item.Undercard==1);
                }
                if (item.Vip == 2)
                {
                    ClubNumTxt2.text = "" + item.ClubNum;
                    EmojNumTxt2.text = "" + item.EmojiNum;
                    DelayTimeTxt2.text = "" + item.DelayNum;
                    ShowPublicImg2.gameObject.SetActive(item.Undercard == 1);
                }
                if (item.Vip == 3)
                {
                    ClubNumTxt3.text = "" + item.ClubNum;
                    EmojNumTxt3.text = "" + item.EmojiNum;
                    DelayTimeTxt3.text = "" + item.DelayNum;
                    ShowPublicImg3.gameObject.SetActive(item.Undercard == 1);
                }
            }
        }


        AmountTxt.text = card.Price.ToString();
        PersonCardContent.SetActive(true);
        switch (card.GoodId) {
            case 1:
                SMask.sprite = Resources.Load<Sprite>("Textures/Shop/list_img_sel");
                break;
            case 2:
                GMask.sprite = Resources.Load<Sprite>("Textures/Shop/list_img_sel");
                break;
            case 3:
                BMask.sprite = Resources.Load<Sprite>("Textures/Shop/list_img_sel");
                break;
        }
        BuyBtn.onClick.AddListener(delegate {
            DoBuyPersonCard(card);
        });
    }

    public void ShowGoinDialog(ShopGoodGold gold)
    {
        TitleTxt.text = "金币";
        Wrapper.sizeDelta = new Vector2(Wrapper.sizeDelta.x, 430);
        GoldCoinContent.SetActive(true);
        CoinImg.sprite = Resources.Load<Sprite>("Textures/Shop/" + gold.Icon);
        CoinImg.SetNativeSize();
        CoinTxt.text = gold.Gift == 0 ? gold.Gold.ToString() : gold.Gold.ToString() + " + " + gold.Gift.ToString();
        AmountTxt.text = gold.Price.ToString();

        BuyBtn.onClick.AddListener(delegate {
            DoBuyCoin(gold);
        });
    }

    void DoBuyPersonCard(ShopGoodCard card) {
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("id", card.GoodId.ToString());

        ShopApi.BuyShopGood(param, (rmb, coin, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Game.Instance.CurPlayer.Diamond = rmb;
                Game.Instance.CurPlayer.Gold = coin;
                if (Game.Instance.CurPlayer.Vip == card.Vip)
                {
                    Game.Instance.CurPlayer.VipExpirTime += 30;
                }
                else
                {
                    Game.Instance.CurPlayer.VipExpirTime = 30;
                }
                if (card.Undercard == 1) {
                    Game.Instance.CurPlayer.CanLookCard = 1;
                }
                Game.Instance.CurPlayer.Vip = card.Vip;
                Game.Instance.CurPlayer.VipDelayNum += card.DelayNum;
                Game.Instance.CurPlayer.VipEmojiNum += card.EmojiNum;

                Game.Instance.MainTabView.ShopViewPanel.RefreshUserShopInfo();
                Close();
            }
            else {
                Game.Instance.ShowTips(error);
            }
        });
    }

    void DoBuyCoin(ShopGoodGold gold)
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("id", gold.GoodId.ToString());

        ShopApi.BuyShopGood(param, (rmb, coin, error) =>
        {
            if (string.IsNullOrEmpty(error))
            {
                Game.Instance.CurPlayer.Diamond = rmb;
                Game.Instance.CurPlayer.Gold = coin;
                Game.Instance.MainTabView.ShopViewPanel.RefreshUserShopInfo();
                Close();
            }
            else
            {
                Game.Instance.ShowTips(error);
            }
        });

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            Close();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
