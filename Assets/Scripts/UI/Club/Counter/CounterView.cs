using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 柜台
    /// </summary>
    public class CounterView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose;
        public Toggle tgRecord, tgSendAndRecycle;
        public CounterClipsView clipsView; // 收发
        public CounterRecordsView recordsView; // 记录

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            tgRecord.onValueChanged.AddListener((isOn)=>
            {
                if (isOn)
                {
                    recordsView.Show();
                }
                else
                {
                    recordsView.Hide();
                }
            });
            tgSendAndRecycle.onValueChanged.AddListener((isOn)=>
            {
                if (isOn)
                {
                    clipsView.Show();
                }
                else
                {
                    clipsView.Hide();
                }
            });

            clipsView.InitView(ClubMainView.Instance.ClubCoin);
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
