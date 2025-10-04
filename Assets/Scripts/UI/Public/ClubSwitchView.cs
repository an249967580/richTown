using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine;
using SimpleJson;

namespace RT
{

    public delegate void ClubEnterEvent(long clubId, string clubName, long coins);

    public class ClubSwitchView : HideMonoBehaviour, IPointerClickHandler
    {
        public ListView lstView;
        public ClubEnterEvent OnClubEnterEvent;

        public void InitView(List<ItemClubData> clubs)
        {
            lstView.Clear();
            if(Validate.IsEmpty(clubs))
            {
                return;
            }

            for (int i = 0; i < clubs.Count; i++)
            {
                ItemClubView vi = lstView.Add(clubs[i]) as ItemClubView;
                vi.OnItemClickEvent = (view)=> {
                    if(OnClubEnterEvent != null)
                    {
                        ItemClubData data = view.Data as ItemClubData;
                        OnClubEnterEvent(data.clubId, data.name, data.coin);
                    }
                };
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


        
    }
}
