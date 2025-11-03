using UnityEngine;
using UnityEngine.UI;

namespace RT
{

    public delegate void CreateTabbleEvent();

    /// <summary>
    /// 牛牛开桌
    /// </summary>
    public class CreateBullView : HideMonoBehaviour
    {
        public InputField ipRoom;
        public Image icon5, icon7, bg5,bg7;
        public Button btnClose, btnCreate, btnFee, btnTime, btn5, btn7;
        public Toggle tg15, tg20, tg30, tgBuyIn;
        public Slider sAntes, sChips;
        public Text tvAntes, tvMinChips, tvFee, tvTime;
        public Text[] tvPins;

        public ListView lstFee, lstTime;
        public GameObject goDrop, goPin;

        public CreateTabbleEvent OnCreateTabbleEvent;

        private MdCreateTable _md;

        private int idxAntes = 3, idxChips = 2;

        private void Awake()
        {
            _md = new MdCreateTable(GameType.bull);
            btnClose.onClick.AddListener(HideAndDestory);
            btnCreate.onClick.AddListener(onCreate);
            // btnFee.onClick.AddListener(showFee);
            // btnTime.onClick.AddListener(showTime);
            sAntes.onValueChanged.AddListener(antesChange);
            sChips.onValueChanged.AddListener(chipsChange);

            btn5.onClick.AddListener(() =>
            {
                icon5.gameObject.SetActive(false);
                bg5.gameObject.SetActive(true);

                icon7.gameObject.SetActive(true);
                bg7.gameObject.SetActive(false);
                _md.playerNum = 5;
            });

            btn7.onClick.AddListener(() =>
            {
                icon7.gameObject.SetActive(false);
                bg7.gameObject.SetActive(true);

                icon5.gameObject.SetActive(true);
                bg5.gameObject.SetActive(false);
                _md.playerNum = 7;
            });

            tgBuyIn.onValueChanged.AddListener((isOn) =>
            {
                _md.isBuyIn = isOn;
            });

            UIEventListener.Get(goPin).onClick = openPin;
        }

        private void Start()
        {
            _md.isPin = true;
            _md.playerNum = 7;
            sAntes.value = 3;
            sChips.value = 2;
            _md.thinkTime = 15;
            _md.antes = _md.BlindList[idxAntes];
            _md.minChips = _md.BlindList[idxAntes] * _md.RateList[idxChips];
            tgBuyIn.isOn = false;
            _md.isBuyIn = false;
            // _md.fee = _md.FeeList[0].num;
            // tvFee.text = _md.FeeList[0].title;
            // _md.time = _md.TimeList[0].num;
            // tvTime.text = _md.TimeList[0].title;
        }

        void onCreate()
        {
            if(Validate.IsEmpty(ipRoom.text.Trim()))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5901"));
                return;
            }
            _md.roomName = ipRoom.text.Trim();
            _md.CreateTable((result) =>
            {
                if(result.IsOk)
                {
                    if(OnCreateTabbleEvent != null)
                    {
                        OnCreateTabbleEvent();
                    }
                }
                else
                {
                    if (result.data != null)
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
            tvAntes.text = string.Format(LocalizationManager.Instance.GetText("5904"), _md.BlindList[idxAntes]);
            tvMinChips.text = string.Format(LocalizationManager.Instance.GetText("5905"), _md.BlindList[idxAntes] * _md.RateList[idxChips]);
            _md.antes = _md.BlindList[idxAntes];
            _md.minChips = _md.BlindList[idxAntes] * _md.RateList[idxChips];
        }

        void chipsChange(float value)
        {
            idxChips = (int)value;
            tvMinChips.text = string.Format( LocalizationManager.Instance.GetText("5905"), _md.BlindList[idxAntes] * _md.RateList[idxChips]);
            _md.minChips = _md.BlindList[idxAntes] * _md.RateList[idxChips];
        }

        void openPin(GameObject go)
        {
            if(go == goPin)
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
        //                 tvTime.text = data.title;
        //                 _md.time = data.num;
        //                 goDrop.SetActive(false);
        //             };
        //         }
        //     }
        // }

        #endregion

    }
}
