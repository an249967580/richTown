using RT;
using UnityEngine;
using UnityEngine.UI;

public class FinalStatItemView : ItemView
{
    public CircleImage AvatarImg;
    public Text NickTxt;
    public Text UidTxt;
    public Text BuyinTxt;
    public Text ProfitTxt;

    public override void RegisterEvent()
    {
    }

    public override void Render()
    {
        GameLiveData data = Data as GameLiveData;

        LimitText.LimitAndSet(data.Nickname, NickTxt, 225);
        UidTxt.text = "ID: " + data.UId;
        BuyinTxt.text = data.TotalBuyBet.ToString();
        if (data.ProfitLoss == 0)
        {
            ProfitTxt.text = data.ProfitLoss.ToString();
            ProfitTxt.color = Color.white;
        }
        else if (data.ProfitLoss > 0)
        {
            ProfitTxt.text = "+" + data.ProfitLoss;
            ProfitTxt.color = Color.green;
        }
        else
        {
            ProfitTxt.text = "" + data.ProfitLoss;
            ProfitTxt.color = Color.red;
        }
    }
}

