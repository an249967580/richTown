using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public delegate void BuyLevelCardEvent(ItemLevelCardData data);

    /// <summary>
    /// 购买等级卡页面
    /// </summary>
    public class LevelCardBuyView : HideMonoBehaviour, IPointerClickHandler
    {
        public Text tvDiamond;
        public Button btnBuy, btnClose;

        private ItemLevelCardData _cardData;
        public BuyLevelCardEvent OnBuyLevelCardEvent;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnBuy.onClick.AddListener(()=>
            {
                if (_cardData != null)
                {
                    if(OnBuyLevelCardEvent != null)
                    {
                        OnBuyLevelCardEvent(_cardData);
                    }
                }
            });
        }

        public void InitView(ItemLevelCardData cardData)
        {
            tvDiamond.text = cardData.diamond.ToString();
            _cardData = cardData;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != gameObject)
            {
                return;
            }
            if (gameObject.activeSelf)
            {
                HideAndDestory();
            }
        }
    }
}
