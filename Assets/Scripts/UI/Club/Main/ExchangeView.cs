using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{

    public delegate void ExchangeEvent(ExchangeResult result);

    /// <summary>
    /// 俱乐部筹码兑换(代理)
    /// </summary>
    public class ExchangeView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnSure;
        public Text tvDiamond, tvDimandRate, tvDimondCoinsNum, tvCoins, tvCoinsRate, tvCoinDiamondNum, tvMinCoin;
        public InputField ipDiamond, ipCoin;

        public Toggle tgCoin, tgDiamond;

        public GameObject goCoin, goDiamand;
        public ExchangeEvent OnExchangeEvent;

        private MdExchange _md;

        private bool _exCoins = true;

        private void Awake()
        {
            _md = new MdExchange();
            _md.clubCoin = ClubMainView.Instance.ClubCoin;
            _md.minClubCoin = ClubMainView.Instance.MinClubCoin;
            _md.diamondRate = ClubMainView.Instance.DiamondRate;
            _md.diamond = Game.Instance.CurPlayer.Diamond;

            btnClose.onClick.AddListener(HideAndDestory);
            btnSure.onClick.AddListener(exchange);

            ipCoin.onValueChanged.AddListener((text)=>
            {
                if(!Validate.IsEmpty(text))
                {
                    long coin = long.Parse(text);
                    tvCoinDiamondNum.text = (coin / _md.diamondRate).ToString();
                }
            });

            ipDiamond.onValueChanged.AddListener((text)=>
            {
                if (!Validate.IsEmpty(text))
                {
                    long diamond = long.Parse(text);
                    tvDimondCoinsNum.text = (diamond * _md.diamondRate).ToString();
                }
            });

            tgCoin.onValueChanged.AddListener((isOn)=>
            {
                goCoin.SetActive(true);
                goDiamand.SetActive(false);
                _exCoins = true;
                ipCoin.text = "";
                tvCoinDiamondNum.text = "";
            });

            tgDiamond.onValueChanged.AddListener((isOn)=>
            {
                goCoin.SetActive(false);
                goDiamand.SetActive(true);
                _exCoins = false;
                ipDiamond.text = "";
                tvDimondCoinsNum.text = "";
            });

            tvDiamond.text = _md.diamond.ToString();
            tvCoins.text = _md.clubCoin.ToString();
            tvDimandRate.text = string.Format(LocalizationManager.Instance.GetText("5404"), 1,  _md.diamondRate);
            tvCoinsRate.text = string.Format(LocalizationManager.Instance.GetText("5406"), _md.diamondRate,  1);
            tvMinCoin.text = string.Format(LocalizationManager.Instance.GetText("5407"), _md.minClubCoin);
        }

        void exchange()
        {
            if(_exCoins)
            {
                exchangeCoin();
            }
            else
            {
                exchangeDiamand();
            }
        }

        void exchangeCoin()
        {
            string diampndStr = ipDiamond.text.Trim();
            if (string.IsNullOrEmpty(diampndStr))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5403"));
                return;
            }
            int diamond = int.Parse(diampndStr);
            if (diamond <= 0)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5403"));
                return;
            }

            if(_md.diamond < diamond)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5409"));
                return;
            }

            _md.exchagneCoin(diamond, (rsp) =>
            {
                if(rsp.IsOk)
                {
                    if(rsp.data != null)
                    {
                        if(OnExchangeEvent != null)
                        {
                            OnExchangeEvent(rsp.data);
                        }
                    }
                }
                else
                {
                    Game.Instance.ShowTips(rsp.errorMsg);
                }
            });
        }

        void exchangeDiamand()
        { 
            string coinStr = ipCoin.text.Trim();
            if (string.IsNullOrEmpty(coinStr))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5405"));
                return;
            }
            int coin = int.Parse(coinStr);
            if (coin <= 0)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5405"));
                return;
            }

            if (_md.clubCoin < coin)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5408"));
                return;
            }

            if (coin < _md.minClubCoin)
            {
                Game.Instance.ShowTips(string.Format(LocalizationManager.Instance.GetText("5407"), _md.minClubCoin));
                return;
            }

            _md.exchagneDiamond(coin, (rsp) =>
            {
                if (rsp.IsOk)
                {
                    if (rsp.data != null)
                    {
                        if (OnExchangeEvent != null)
                        {
                            OnExchangeEvent(rsp.data);
                        }
                    }
                }
                else
                {
                    Game.Instance.ShowTips(rsp.errorMsg);
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
                HideAndDestory();
            }
        }
    }
}
