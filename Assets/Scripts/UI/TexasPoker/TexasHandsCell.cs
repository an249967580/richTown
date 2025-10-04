using Assets.Scripts.TableView;
using RT;
using UnityEngine;
using UnityEngine.UI;

public class TexasHandsCell : TableViewCell
{
    public Text TitleTxt;
    public Text NickTxt;
    public Text TypeTxt;
    public Text ProfitTxt;

    public Image[] SelfCardImgs;
    public Image[] CenterCardImgs;

    public TexasHistoryHandsData Data;

    public override string ReuseIdentifier
    {
        get { return "HandsTableViewCRI"; }
    }

    public override void SetHighlighted()
    {
        
    }

    public override void SetSelected()
    {
        
    }

    public override void Display()
    {
        TypeTxt.gameObject.SetActive(false);

        if (Data != null)
        {
            TitleTxt.text = string.Format(LocalizationManager.Instance.GetText("7009"), Data.Index, Data.HandId);// "第" + Data.Index + "手" + "(" + Data.HandId + ")";
            NickTxt.text = Data.Nickname;
            if (Data.Balance > 0)
            {
                ProfitTxt.text = "+" + Data.Balance;
                ProfitTxt.color = Color.green;
            }
            else if (Data.Balance < 0)
            {
                ProfitTxt.text = "" + Data.Balance;
                ProfitTxt.color = Color.red;
            }
            else
            {
                ProfitTxt.text = "" + Data.Balance;
                ProfitTxt.color = Color.white;
            }
            for (int i = 0; i < SelfCardImgs.Length; i++) {
                if (Data.Cards != null && Data.Cards.Length <= SelfCardImgs.Length && Data.Cards.Length > i)
                {
                    SelfCardImgs[i].gameObject.SetActive(true);
                    SelfCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + Data.Cards[i]);
                }
                else {
                    SelfCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/card_back_bg");
                }
            }
            for (int i = 0; i < CenterCardImgs.Length; i++)
            {
                if (Data.UnderCards != null && Data.UnderCards.Length <= CenterCardImgs.Length && Data.UnderCards.Length > i)
                {
                    CenterCardImgs[i].gameObject.SetActive(true);
                    CenterCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + Data.UnderCards[i]);
                }
                else
                {
                    CenterCardImgs[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
