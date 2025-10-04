using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BullMenuView : MonoBehaviour, IPointerClickHandler
{
    public Button MenuStandUpBtn;
    public Button MenuBuyChipBtn;
    public Button MenuIntroBtn;
    public Button MenuSettingBtn;
    public Button MenuExitRoomBtn;
    public Button MenuCloseRoomBtn;

    public RectTransform Wrapper;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            Close();
        }
    }
    public void ShowCloseBtn()
    {
        float h = Wrapper.sizeDelta.y + 70;
        Wrapper.sizeDelta = new Vector2(Wrapper.sizeDelta.x, h);
        MenuCloseRoomBtn.gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
