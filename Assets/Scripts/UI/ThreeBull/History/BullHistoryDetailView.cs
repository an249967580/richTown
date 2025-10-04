using RT;
using UnityEngine;
using UnityEngine.UI;

public class BullHistoryDetailView : MonoBehaviour
{
    public Text ViewTitle;
    public Text BetTitle;
    public Text EmptyText;
    public RectTransform Wrapper;

    public Text SGTitle;
    public Text NNTitle;
    
    public ListView PlayerListView;
    public Button GoListBtn;
    public BullRoom RoomInfo;

    public void InitView(BullHistorySuitData data)
    {
        ViewTitle.text = string.Format(LocalizationManager.Instance.GetText("8002"), data.Index);// "牌局回顾第" + data.Index + "手";
        BetTitle.text = string.Format(LocalizationManager.Instance.GetText("5904"), RoomInfo.BaseBet); //"底注：" + RoomInfo.BaseBet;

        if (data != null && data.DataList != null)
        {
            PlayerListView.Clear();
            foreach (int k in data.DataList.Keys)
            {
                data.DataList[k].BaseBet = RoomInfo.BaseBet;
                data.DataList[k].SGBankUId = data.SGBankUId;
                data.DataList[k].NNBankUId = data.NNBankUId;
                data.DataList[k].SGOdds = data.SGOdds;
                data.DataList[k].NNOdds = data.NNOdds;
                PlayerListView.Add(data.DataList[k]);
            }
        }
    }
}
