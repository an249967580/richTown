using RT;
using UnityEngine;
using UnityEngine.UI;

public class TexasHistoryDetailView : MonoBehaviour
{
    public Text ViewTitle;
    public Text EmptyText;
    public RectTransform Wrapper;

    public Text HandsIdTxt;
    public Text SelfNickTxt;
    public Text SelfTypeTxt;
    public Text SelfProfitTxt;
    public Image[] SelfCardImgs;
    public Image[] CenterCardImgs;

    public ListView OtherListView;
    public Button GoListBtn;

    public void InitView(TexasHistoryHandsData data)
    {
        HandsIdTxt.text = string.Format(LocalizationManager.Instance.GetText("7009"), data.Index, data.HandId); //"第" + data.Index + "手" + "(" + data.HandId + ")";
        SelfNickTxt.text = data.Nickname;
        if (data.Balance > 0)
        {
            SelfProfitTxt.text = "+" + data.Balance;
            SelfProfitTxt.color = Color.green;
        }
        else if (data.Balance < 0)
        {
            SelfProfitTxt.text = "" + data.Balance;
            SelfProfitTxt.color = Color.red;
        }
        else
        {
            SelfProfitTxt.text = "" + data.Balance;
            SelfProfitTxt.color = Color.white;
        }
        for (int i = 0; i < SelfCardImgs.Length; i++)
        {
            if (data.Cards != null && data.Cards.Length <= SelfCardImgs.Length && data.Cards.Length>i)
            {
                SelfCardImgs[i].gameObject.SetActive(true);
                SelfCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + data.Cards[i]);
            }
            else
            {
                SelfCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/card_back_bg");
            }
        }
        for (int i = 0; i < CenterCardImgs.Length; i++)
        {
            if (data.UnderCards != null && data.UnderCards.Length <= CenterCardImgs.Length && data.UnderCards.Length > i)
            {
                CenterCardImgs[i].gameObject.SetActive(true);
                CenterCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + data.UnderCards[i]);
            }
            else
            {
                CenterCardImgs[i].gameObject.SetActive(false);
            }
        }
        if (data != null && data.OtherList != null)
        {
            OtherListView.Clear();
            for (int i = 0; i < data.OtherList.Length; i++)
            {
                OtherListView.Add(data.OtherList[i]);
            }
        }
    }
}
