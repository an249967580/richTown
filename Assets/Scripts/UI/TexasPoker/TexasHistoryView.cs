using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TexasHistoryView : MonoBehaviour, IPointerClickHandler
{
    public int roomId;
    public TexasHistoryTableView ListTableView;
    public TexasHistoryDetailView DetailView;

    List<TexasHistoryHandsData> list;
    
    void Start ()
    {
        list = new List<TexasHistoryHandsData>();
        ListTableView.gameObject.SetActive(false);
        DetailView.gameObject.SetActive(true);

        DetailView.GoListBtn.onClick.AddListener(delegate (){
            GoListViewClick();
        });
        ListTableView.BackBtn.onClick.AddListener(delegate() {
            GoDetailViewClick(0);
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ListTableView.RoomId = roomId;
        ListTableView.gameObject.SetActive(false);
        DetailView.gameObject.SetActive(true);
        Dictionary<string, string> parma = new Dictionary<string, string>();
        parma.Add("roomId", roomId.ToString());
        TexasApi.GetGameRoomHandsHistory(parma,(resp,error)=> {
            if (error == null)
            {
                if (resp.List.Count > 0)
                {
                    list = resp.List;
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Index = resp.Total - i;
                    }
                    DetailView.InitView(resp.List[0]);
                    DetailView.Wrapper.gameObject.SetActive(true);
                    DetailView.EmptyText.gameObject.SetActive(false);
                }
                else
                {
                    DetailView.Wrapper.gameObject.SetActive(false);
                    DetailView.EmptyText.gameObject.SetActive(true);
                    ListTableView.tableView.gameObject.SetActive(false);
                    ListTableView.EmptyTxt.gameObject.SetActive(true);
                }
            }
            else
            {
                Game.Instance.ShowTips(error);
            }
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
            Close();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void GoDetailViewClick(int index)
    {
        DetailView.gameObject.SetActive(true);
        ListTableView.gameObject.SetActive(false);

        if (index < list.Count)
        {
            DetailView.InitView(list[index]);
            DetailView.Wrapper.gameObject.SetActive(true);
            DetailView.EmptyText.gameObject.SetActive(false);
        }
        else
        {
            DetailView.Wrapper.gameObject.SetActive(false);
            DetailView.EmptyText.gameObject.SetActive(true);
        }
    }
    void GoListViewClick()
    {
        DetailView.gameObject.SetActive(false);
        ListTableView.gameObject.SetActive(true);

        ListTableView.InitView(list);
        ListTableView.Show();
    }
}
