using RT;
using UnityEngine;
using UnityEngine.UI;

public class TexasHandsItemView : ItemView
{
    public Text NickTxt;
    public Text TypeTxt;
    public Text ProfitTxt;

    public Image[] SelfCardImgs;

    public override void RegisterEvent()
    {
    }

    public override void Render()
    {
        TypeTxt.gameObject.SetActive(false);
        TexasPlayerHandsData data = Data as TexasPlayerHandsData;

        LimitText.LimitAndSet(data.Nickname, NickTxt, 230);
        if (data.Balance == 0)
        {
            ProfitTxt.text = data.Balance.ToString();
            ProfitTxt.color = Color.white;
        }
        else if (data.Balance > 0)
        {
            ProfitTxt.text = "+" + data.Balance;
            ProfitTxt.color = Color.green;
        }
        else
        {
            ProfitTxt.text = "" + data.Balance;
            ProfitTxt.color = Color.red;
        }
        for (int i = 0; i < SelfCardImgs.Length; i++)
        {
            if (data.Cards.Length <= SelfCardImgs.Length)
            {
                SelfCardImgs[i].gameObject.SetActive(true);
                SelfCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + data.Cards[i]);
            }
            else
            {
                SelfCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/card_back_bg");
            }
        }
    }
}

