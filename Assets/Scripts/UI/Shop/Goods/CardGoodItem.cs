using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGoodItem : MonoBehaviour {

    public Image IcomImg;
    public Text TitleTxt;
    public Text PriceTxt;

    private Button btn;
    private ShopGoodCard card;

    public ShopGoodCard Card
    {
        get
        {
            return card;
        }

        set
        {
            card = value;
            IcomImg.sprite = Resources.Load<Sprite>("Textures/Shop/" + card.Icon);
            IcomImg.SetNativeSize();
            TitleTxt.text = card.Title;
            PriceTxt.text = card.Price.ToString();
        }
    }
    public List<ShopGoodCard> CardList;
    DiamondDialog BuyDialogTpl;

    void Start () {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate {
            ShowGoodDetail();
        });
        BuyDialogTpl = Resources.Load<DiamondDialog>("Prefabs/Shop/ShopDialogPanel");
    }
	
	void Update () {
		
	}

    void ShowGoodDetail() {
        DiamondDialog go = Instantiate(BuyDialogTpl);
        RectTransform t = go.transform as RectTransform;
        t.SetParent(GameObject.Find("_UGUI").transform);
        t.localScale = Vector3.one;
        t.anchoredPosition3D = Vector3.zero;
        go.CardList = CardList;
        go.ShowPersonCardDialog(card);
    }
}
