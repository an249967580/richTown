using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace RT
{
    public delegate void CountryClickEvent(ItemCountryData data);

    public class CountryView : HideMonoBehaviour, IPointerClickHandler
    {
        public ListView lstView;
        public CountryClickEvent OnCountryClickEvent;

        public void InitView(List<ItemCountryData> list)
        {
            lstView.Clear();
            if (Validate.IsEmpty(list))
            {
                return;
            }
            for(int i=0;i<list.Count;i++)
            {
                lstView.Add(list[i]).OnItemClickEvent = onItemClickEvent;
            }
        }

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

        void onItemClickEvent(ItemView vi)
        {
            ItemCountryData data = vi.Data as ItemCountryData;
            if(OnCountryClickEvent != null)
            {
                OnCountryClickEvent(data);
                Hide();
            }
        }
    }
}
