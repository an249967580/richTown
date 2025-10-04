using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{

    public delegate void SendOrRecycleEvent(long uid, long coins, SendOrRecycleView vi);

    /// <summary>
    /// 收/发筹码操作
    /// </summary>
    public class SendOrRecycleView : HideMonoBehaviour, IPointerClickHandler
    {
        public Text tvTitle, tvCoins, tvName;
        public Button btnClose, btnSure;
        public InputField ipCoins;

        public SendOrRecycleEvent OnSendOrRecycleEvent;

        private bool _isSend;

        private ItemMemberData _data;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnSure.onClick.AddListener(sure);
        }

        public void InitView(bool isSend, ItemMemberData data)
        {
            _isSend = isSend;
            string key = isSend ? "5304" : "5305";
            tvTitle.text = LocalizationManager.Instance.GetText(key);

            string name = isSend ? LocalizationManager.Instance.GetText("5000") : data.nickname;
            LimitText.LimitAndSet(name, tvName, "：", 160);
            tvCoins.text = isSend ? ClubMainView.Instance.ClubCoin.ToString() : data.coin.ToString();
            _data = data;
        }

        void sure()
        {
            string coinStr = ipCoins.text.Trim();
            if (string.IsNullOrEmpty(coinStr))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5307"));
                return;
            }
            int coins = int.Parse(coinStr);
            if (coins <= 0)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5307"));
                return;
            }

            if(!_isSend)
            {
                if (coins > _data.coin)
                {
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5308"));
                    return;
                }
            }
            
            if(OnSendOrRecycleEvent != null)
            {
                OnSendOrRecycleEvent(_data.uid, coins, this);
            }
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
