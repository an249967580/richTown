using UnityEngine.EventSystems;

namespace RT
{
    public class DropView : HideMonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != gameObject)
            {
                return;
            }
            if (gameObject.activeSelf)
            {
                Hide();
            }
        }
    }
}
