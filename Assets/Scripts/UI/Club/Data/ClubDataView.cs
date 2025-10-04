using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

namespace RT
{
    /// <summary>
    /// 俱乐部数据
    /// </summary>
    public class ClubDataView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose;
        public RoomDataView roomView;
        public LuckyPlayerView lpView;
        public WinOrLossView wlView;
        public Button btnMore;

        public Toggle tgRoom, tgWinAndLoss, tgLuckyPlayer;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            tgRoom.onValueChanged.AddListener((isOn)=>
            {
                if (isOn)
                {
                    roomView.Show();
                }
                else
                {
                    roomView.Hide();
                }
            });
            tgLuckyPlayer.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    lpView.Show();
                }
                else
                {
                    lpView.Hide();
                }
            });
            tgWinAndLoss.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    wlView.Show();
                }
                else
                {
                    wlView.Hide();
                }
            });

            if (ClubMainView.Instance.ShowLucky)
            {
                tgLuckyPlayer.gameObject.SetActive(true);
            }
            else
            {
                tgLuckyPlayer.gameObject.SetActive(false);
            }

            if(ClubMainView.Instance.Level > 0)
            {
                btnMore.gameObject.SetActive(true);
            }
            else
            {
                btnMore.gameObject.SetActive(false);
            }

            btnMore.onClick.AddListener(() =>
            {
                Application.OpenURL("http://agent.richtown.io");
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
