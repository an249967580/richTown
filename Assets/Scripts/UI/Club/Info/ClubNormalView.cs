using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 俱乐部信息
    /// </summary>
    public class ClubNormalView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnMembers, btnQuit, btnCurrency;
        public Text  tvCoins, tvMember;


        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
           
            btnMembers.onClick.AddListener(()=>
            {
                // 会员列表页
                UIClubSpawn.Instance.CreateMemberListView();
            });
            btnQuit.onClick.AddListener(onExit);
            btnCurrency.onClick.AddListener(() =>
            {
                UIClubSpawn.Instance.CreateRecordsView().Show();
            });

            NotificationCenter.Instance.AddNotifyListener(NotificationType.ChangeClubCoin, onCoinChange);
        }

        public void InitView(ClubDetail detail)
        {
            tvCoins.text = detail.coin.ToString();
            tvMember.text = string.Format("{0}/{1}", detail.memberCount, detail.memberLimit);
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
                        Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Club;
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            };
        }

        void onCoinChange(NotifyMsg msg)
        {
            long coin = (long)msg["coin"];
            tvCoins.text = coin.ToString();
        }

        private void OnDestroy()
        {
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.ChangeClubCoin, onCoinChange);
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
