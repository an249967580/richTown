using RT;
using UnityEngine;
using UnityEngine.UI;

public delegate void DiamondBuyEvent(ShopGoodDiamond diamond);

public class DiamondGoodItem : MonoBehaviour {

    public Image IcomImg;
    public Text TitleTxt;
    public Text PriceTxt;

    public DiamondBuyEvent OnDiamondBuyEvent;

    private Button btn;
    private ShopGoodDiamond diamond;

    public ShopGoodDiamond Diamond
    {
        get
        {
            return diamond;
        }

        set
        {
            diamond = value;
            IcomImg.sprite = Resources.Load<Sprite>("Textures/Shop/" + diamond.Icon);
            IcomImg.SetNativeSize();
            TitleTxt.text = diamond.Gift == 0 ? diamond.Diamond.ToString() : diamond.Diamond.ToString() + " + " + diamond.Gift.ToString();

            double price = diamond.Price / 100.0;
            PriceTxt.text = "$" + price.ToString("#0.00");
        }
    }
    
    void Start ()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(delegate {
            if(OnDiamondBuyEvent != null)
            {
                OnDiamondBuyEvent(diamond);
            }
        });

    }

	void Update () {

    }


   


}
