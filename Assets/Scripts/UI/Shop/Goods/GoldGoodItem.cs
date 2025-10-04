using RT;
using UnityEngine;
using UnityEngine.UI;

public class GoldGoodItem : MonoBehaviour {
    public Image IcomImg;
    public Text TitleTxt;
    public Text PriceTxt;

    private Button btn;
    private ShopGoodGold gold;

    public ShopGoodGold Gold
    {
        get
        {
            return gold;
        }

        set
        {
            gold = value;
            IcomImg.sprite = Resources.Load<Sprite>("Textures/Shop/" + gold.Icon);
            IcomImg.SetNativeSize();
            TitleTxt.text = gold.Gift == 0 ? gold.Gold.ToString() : gold.Gold.ToString() + " + " + gold.Gift.ToString();
            PriceTxt.text = gold.Price.ToString();
        }
    }


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
    void ShowGoodDetail()
    {
        DiamondDialog go = Instantiate(BuyDialogTpl);
        RectTransform t = go.transform as RectTransform;
        t.SetParent(GameObject.Find("_UGUI").transform);
        t.localScale = Vector3.one;
        t.anchoredPosition3D = Vector3.zero;
        go.ShowGoinDialog(gold);
    }
}
