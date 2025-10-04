using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 申请成功弹出窗
    /// </summary>
    public class ClubApplyTipView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnSure;

        public void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnSure.onClick.AddListener(HideAndDestory);
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
