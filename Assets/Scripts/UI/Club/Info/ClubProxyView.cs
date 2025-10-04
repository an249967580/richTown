using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 俱乐部信息
    /// </summary>
    public class ClubProxyView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnCurrency, btnMembers, btnLevel, btnClubData, 
            btnApplyNotify, btnExit, btnDisband, btnCheckOn, btnCheckOff;
        public Text tvStartNum, tvServiceFee, tvHandNum, tvCoins, tvMember, tvLevel;
        public Image imgNotice;

        private ClubDetail _detail;


        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnCurrency.onClick.AddListener(onExchange);
            btnMembers.onClick.AddListener(()=>
            {
                // 会员列表页
                MemberListView vi = UIClubSpawn.Instance.CreateMemberListView();
                vi.OnMemberRemoveEvent = () =>
                {
                    _detail.memberCount -= 1;
                    tvMember.text = string.Format("{0}/{1}", _detail.memberCount, _detail.memberLimit);
                };
            });
            btnLevel.onClick.AddListener(onLevelCards);
            btnClubData.onClick.AddListener(()=>
            {
                // 俱乐部数据
                UIClubSpawn.Instance.CreateClubDataView();
            });
            btnApplyNotify.onClick.AddListener(onApplyNotify);
            btnCheckOff.onClick.AddListener(() =>
            {
                onIpCheck(1);
            });
            btnCheckOn.onClick.AddListener(() =>
            {
                onIpCheck(0);
            });
            btnExit.onClick.AddListener(onExit);
            btnDisband.onClick.AddListener(onDisband);
            btnExit.gameObject.SetActive(ClubMainView.Instance.IsProxy);
            btnDisband.gameObject.SetActive(ClubMainView.Instance.IsCreator);
            imgNotice.gameObject.SetActive(false);
            NotificationCenter.Instance.AddNotifyListener(NotificationType.ApplyJoin, onApplyNotify);


        }

        public void InitView(ClubDetail detail, bool showNotice)
        {
            _detail = detail;
            tvCoins.text = detail.clubCoins.ToString();
            tvMember.text = string.Format("{0}/{1}", detail.memberCount, detail.memberLimit);
            tvLevel.text = "LV." + detail.level;
            tvStartNum.text = detail.startTotal.ToString();
            tvServiceFee.text = detail.serviceChargeTotal.ToString();
            tvHandNum.text = detail.handTotal.ToString();
            imgNotice.gameObject.SetActive(showNotice);
            if (detail.checkip <= 0)
            {
                btnCheckOff.gameObject.SetActive(true);
                btnCheckOn.gameObject.SetActive(false);
            }
            else
            {
                btnCheckOff.gameObject.SetActive(false);
                btnCheckOn.gameObject.SetActive(true);
            }
        }

        // 兑换
        void onExchange()
        {
            if (!ClubMainView.Instance.HasRight(Auth.Exchange))
            {
                UIClubSpawn.Instance.CreateRecordsView();
                return;
            }

            ExchangeView vi = UIClubSpawn.Instance.CreateExchangeView();
            vi.OnExchangeEvent = (result) =>
            {
                ClubMainView.Instance.UpdateCoins(result.clubCoins);
                Game.Instance.CurPlayer.Diamond = result.diamond;
                tvCoins.text = result.clubCoins.ToString();
                vi.HideAndDestory();
            };
        }

        void onExit()
        {
            ConfirmView vi = UIClubSpawn.Instance.CreateConfirmView();
            vi.ShowTip(LocalizationManager.Instance.GetText("5017"));
            vi.OnConfirmEvent = (confirm) =>
            {
                ClubApi.QuitClub(ClubMainView.Instance.ClubId, (result) =>
               {
                   if (result.IsOk)
                   {
                       SceneManager.LoadScene("MainScene");
                   }
                   else
                   {
                       Game.Instance.ShowTips(result.errorMsg);
                   }
               });
            };
        }

        void onDisband()
        {
            ConfirmView vi = UIClubSpawn.Instance.CreateConfirmView();
            vi.ShowTip(LocalizationManager.Instance.GetText("5018"));
            vi.OnConfirmEvent = (confirm) =>
            {
                ClubApi.DisbandClub(ClubMainView.Instance.ClubId, (result) =>
                {
                    if(result.IsOk)
                    {
                        SceneManager.LoadScene("MainScene");
                        Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Club;
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            };
        }

        void onLevelCards()
        {
            if(!ClubMainView.Instance.HasRight(Auth.Exchange))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5020"));
                return;
            }
            // 等级卡列表
            LevelCardsView vi = UIClubSpawn.Instance.CreateLevelCardsView();
            vi.InitView(Game.Instance.CurPlayer.Diamond, ClubMainView.Instance.Level);
            vi.OnLevelChangeEvent = (data) =>
            {
                if(_detail.level == data.level)
                {
                    _detail.expir += data.expirDays;
                }
                else
                {
                    _detail.expir = data.expirDays;
                }
                _detail.level = data.level;
                _detail.memberLimit = data.memberNum;
                tvLevel.text = "Lv." + data.level;
                tvMember.text = tvMember.text = string.Format("{0}/{1}", _detail.memberCount, _detail.memberLimit);
                ClubMainView.Instance.UpdateExpir();
                ClubMainView.Instance.UpdateLevel(data);
            };
        }

        void onApplyNotify()
        {
            if (!ClubMainView.Instance.HasRight(Auth.Join))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5020"));
                return;
            }

            // 申请通知页
            ApplyNotifyView vi = UIClubSpawn.Instance.CreateApplyNotifyView();
            ClubMainView.Instance.imgNotify.gameObject.SetActive(false);
            imgNotice.gameObject.SetActive(false);
            vi.OnAgreeEvent = () =>
            {
                _detail.memberCount += 1;
                tvMember.text = string.Format("{0}/{1}", _detail.memberCount, _detail.memberLimit);
            };
        }

        void onApplyNotify(NotifyMsg msg)
        {
            imgNotice.gameObject.SetActive(true);
        }

        void onIpCheck(int check)
        {
            ClubApi.IpCheck(ClubMainView.Instance.ClubId, check, (rsp) =>
            {
                if(rsp.IsOk)
                {
                    ClubMainView.Instance.UpdateCheckIp(check);
                    if(check <= 0)
                    {
                        btnCheckOff.gameObject.SetActive(true);
                        btnCheckOn.gameObject.SetActive(false);
                    }
                    else
                    {
                        btnCheckOff.gameObject.SetActive(false);
                        btnCheckOn.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Game.Instance.ShowTips(rsp.errorMsg);
                }
            });
        }

        private void OnDestroy()
        {
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.ApplyJoin, onApplyNotify);
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
