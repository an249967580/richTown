using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public delegate void GameSelectEvent(string game);

    public class GameSelectView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnTexas, btnBull;

        public GameSelectEvent OnGameSelectEvent;

        private void Awake()
        {
            btnTexas.onClick.AddListener(() =>
            {
                if(OnGameSelectEvent != null)
                {
                    OnGameSelectEvent(GameType.dz);
                    HideAndDestory();
                }
            });
            btnBull.onClick.AddListener(() =>
            {
                OnGameSelectEvent(GameType.bull);
                HideAndDestory();
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
