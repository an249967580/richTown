using UnityEngine;
using UnityEngine.UI;

namespace RT
{

    /// <summary>
    /// 德州开桌
    /// </summary>
    public class CreateTexasView : HideMonoBehaviour
    {
        public InputField ipRoom;
        public Image icon2, icon6, icon9, bg2, bg6, bg9;
        public Button btnClose, btnCreate, btnFee, btnTime, btn2, btn6, btn9;
        public Toggle tg15, tg20, tg30, tgBuyIn;
        public Slider sAntes, sMinChips, sMaxChips;
        public Text tvAntes, tvMinChips, tvMaxChips;
        public Text[] tvPins;

        public ListView lstFee, lstTime;
        public GameObject goDrop, goPin;

        public CreateTabbleEvent OnCreateTabbleEvent;

        private MdCreateTable _md;

        private int idxAntes = 3, idxMinChips = 2, idxMaxChips;

        private void Awake()
        {
            _md = new MdCreateTable(GameType.dz);
            btnClose.onClick.AddListener(HideAndDestory);
            btnCreate.onClick.AddListener(onCreate);
            // btnFee.onClick.AddListener(showFee);
            // btnTime.onClick.AddListener(showTime);

            // tvTime.text = _md.TimeList[0].title;
            // tvFee.text = _md.FeeList[0].title;
            // _md.fee = _md.FeeList[0].num;
            _md.thinkTime = 15;

            sAntes.onValueChanged.AddListener(antesChange);
            sMinChips.onValueChanged.AddListener(minChipsChange);
            sMaxChips.onValueChanged.AddListener(maxChipsChange);

            btn2.onClick.AddListener(() =>
            {
                icon2.gameObject.SetActive(false);
                bg2.gameObject.SetActive(true);

                icon6.gameObject.SetActive(true);
                bg6.gameObject.SetActive(false);

                icon9.gameObject.SetActive(true);
                bg9.gameObject.SetActive(false);
                _md.playerNum = 2;
            });

            btn6.onClick.AddListener(() =>
            {
                icon6.gameObject.SetActive(false);
                bg6.gameObject.SetActive(true);

                icon2.gameObject.SetActive(true);
                bg2.gameObject.SetActive(false);
                icon9.gameObject.SetActive(true);
                bg9.gameObject.SetActive(false);
                _md.playerNum = 6;
            });

            btn9.onClick.AddListener(() =>
            {
                icon9.gameObject.SetActive(false);
                bg9.gameObject.SetActive(true);

                icon2.gameObject.SetActive(true);
                bg2.gameObject.SetActive(false);
                icon6.gameObject.SetActive(true);
                bg6.gameObject.SetActive(false);
                _md.playerNum = 9;
            });

            tgBuyIn.onValueChanged.AddListener((isOn) =>
            {
                _md.isBuyIn = isOn;
            });
    
             UIEventListener.Get(goPin).onClick = openPin;
        }

        private void Start()
        {
            idxMaxChips = _md.RateList.Count - 1;
            _md.isPin = true;
            _md.playerNum = 6;
            sAntes.value = idxAntes;
            sMinChips.value = idxMinChips;
            sMaxChips.value = idxMaxChips;
            _md.thinkTime = 15;
            _md.antes = _md.BlindList[idxAntes];
            _md.minChips = _md.BlindList[idxAntes] * _md.RateList[idxMinChips];
            _md.maxChips = _md.BlindList[idxAntes] * _md.RateList[idxMaxChips];
            tgBuyIn.isOn = false;
            _md.isBuyIn = false;
            // _md.fee = _md.FeeList[0].num;
            // tvFee.text = _md.FeeList[0].title;
            // _md.time = _md.TimeList[0].num;
            // tvTime.text = _md.TimeList[0].title;
        }

        void onCreate()
        {
            if (Validate.IsEmpty(ipRoom.text.Trim()))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5901"));
                return;
            }
            _md.roomName = ipRoom.text.Trim();
            if (_md.isPin)
            {
                
            }
            else
            {
                _md.pin = string.Empty;
            }

            _md.CreateTable((result) =>
            {
                if (result.IsOk)
                {
                    if (OnCreateTabbleEvent != null)
                    {
                        OnCreateTabbleEvent();
                    }
                }
                else
                {
                    if(result.data != null)
                    {
                        SureView vi = UIClubSpawn.Instance.CreateSureView();
                        vi.ShowTip(string.Format(result.errorMsg, result.data.num, result.data.limit));
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                    
                }
            });

        }

        void antesChange(float value)
        {
            idxAntes = (int)value;
            tvAntes.text = string.Format(LocalizationManager.Instance.GetText("5903"), _md.BlindList[idxAntes] / 2, _md.BlindList[idxAntes]);
            tvMinChips.text = string.Format(LocalizationManager.Instance.GetText("5905"), _md.BlindList[idxAntes] * _md.RateList[idxMinChips]);
            tvMaxChips.text = string.Format(LocalizationManager.Instance.GetText("5906"), _md.BlindList[idxAntes] * _md.RateList[idxMaxChips]);
            _md.antes = _md.BlindList[idxAntes];
            _md.minChips = _md.BlindList[idxAntes] * _md.RateList[idxMinChips];
            _md.maxChips = _md.BlindList[idxAntes] * _md.RateList[idxMaxChips];
        }

        void minChipsChange(float value)
        {
            idxMinChips = (int)value;
            tvMinChips.text = string.Format(LocalizationManager.Instance.GetText("5905"), _md.BlindList[idxAntes] * _md.RateList[idxMinChips]);
            _md.minChips = _md.BlindList[idxAntes] * _md.RateList[idxMinChips];
        }

        void maxChipsChange(float value)
        {
            idxMaxChips = _md.RateList.Count - (int)value - 1;
            tvMaxChips.text = string.Format(LocalizationManager.Instance.GetText("5906"), _md.BlindList[idxAntes] * _md.RateList[idxMaxChips]);
            _md.maxChips = _md.BlindList[idxAntes] * _md.RateList[idxMaxChips];
        }

        void openPin(GameObject go)
        {
            KeyBoardView vi = UIClubSpawn.Instance.CreateKeyBoardView();
            vi.OnKeyBoardEvent = (pin) =>
            {
                _md.pin = pin;
                for (int i = 0; i < pin.Length; i++)
                {
                    tvPins[i].text = pin.Substring(i, 1);
                }
            };
        }


        #region

        // void showFee()
        // {
        //     goDrop.SetActive(true);
        //     lstFee.gameObject.SetActive(true);
        //     lstTime.gameObject.SetActive(false);
        //     if (lstFee.ViewItems.Count <= 0)
        //     {
        //         for (int i = 0; i < _md.FeeList.Count; i++)
        //         {
        //             ItemDropView vi = lstFee.Add(_md.FeeList[i]) as ItemDropView;
        //             vi.OnItemClickEvent = (view) =>
        //             {
        //                 ItemDropData data = view.Data as ItemDropData;
        //                 tvFee.text = data.title;
        //                 _md.fee = data.num;
        //                 goDrop.SetActive(false);
        //             };
        //         }
        //     }

        // }

        // void showTime()
        // {
        //     goDrop.SetActive(true);
        //     lstFee.gameObject.SetActive(false);
        //     lstTime.gameObject.SetActive(true);
        //     if (lstTime.ViewItems.Count <= 0)
        //     {
        //         for (int i = 0; i < _md.TimeList.Count; i++)
        //         {
        //             ItemDropView vi = lstTime.Add(_md.TimeList[i]) as ItemDropView;
        //             vi.OnItemClickEvent = (view) =>
        //             {
        //                 ItemDropData data = view.Data as ItemDropData;
        //                 // tvTime.text = data.title;
        //                 // _md.time = data.num;
        //                 goDrop.SetActive(false);
        //             };
        //         }
        //     }
        // }

        #endregion

    }
}
