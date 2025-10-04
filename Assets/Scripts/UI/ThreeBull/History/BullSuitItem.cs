using RT;
using UnityEngine;
using UnityEngine.UI;

public class BullSuitItem : ItemView
{
    public Text NickTxt;

    public Image SGTagImg;
    public Image NNTagImg;

    public Image[] SGCardImgs;
    public Image[] NNCardImgs;

    public Text SGResultTxt;
    public Text SGProfitTxt;
    public Text NNResultTxt;
    public Text NNProfitTxt;

    public override void RegisterEvent()
    {
    }

    public override void Render()
    {
        SGTagImg.gameObject.SetActive(false);
        NNTagImg.gameObject.SetActive(false);
        BullPlayerSuitData data = Data as BullPlayerSuitData;

        LimitText.LimitAndSet(data.Nickname, NickTxt, 230);

        //三公
        if (data.UId == data.SGBankUId)
        {
            SGTagImg.gameObject.SetActive(true);
        }
        SGResultTxt.text = "("+data.ConvertSGCardType(data.SGCardType,data.SGPoint)+ ")×"+data.SGOdds;
        if (data.SGBalance == 0)
        {
            SGProfitTxt.text = data.SGBalance.ToString();
            SGProfitTxt.color = Color.white;
        }
        else if (data.SGBalance > 0)
        {
            SGProfitTxt.text = "+" + data.SGBalance;
            SGProfitTxt.color = Color.green;
        }
        else
        {
            SGProfitTxt.text = "" + data.SGBalance;
            SGProfitTxt.color = Color.red;
        }
        for (int i = 0; i < SGCardImgs.Length; i++)
        {
            if (i < data.Cards.Length)
            {
                SGCardImgs[i].gameObject.SetActive(true);
                SGCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + data.Cards[i]);
            }
            else
            {
                SGCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Bull/bull_poker_back");
            }
        }

        //牛牛
        if (data.UId == data.NNBankUId)
        {
            NNTagImg.gameObject.SetActive(true);
        }
        NNResultTxt.text = "(" + data.ConvertNNCardType(data.NNCardType, data.NNPoint) + ")×" + data.NNOdds;
        if (data.NNBalance == 0)
        {
            NNProfitTxt.text = data.NNBalance.ToString();
            NNProfitTxt.color = Color.white;
        }
        else if (data.NNBalance > 0)
        {
            NNProfitTxt.text = "+" + data.NNBalance;
            NNProfitTxt.color = Color.green;
        }
        else
        {
            NNProfitTxt.text = "" + data.NNBalance;
            NNProfitTxt.color = Color.red;
        }
        for (int i = 0; i < NNCardImgs.Length; i++)
        {
            if (i+3 < data.Cards.Length)
            {
                NNCardImgs[i].gameObject.SetActive(true);
                NNCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + data.Cards[i+3]);
            }
            else
            {
                NNCardImgs[i].gameObject.SetActive(false);
                NNCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/Bull/0");
            }
        }
    }
}
