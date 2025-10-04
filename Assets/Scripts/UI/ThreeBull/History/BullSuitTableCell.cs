using Assets.Scripts.TableView;
using RT;
using UnityEngine;
using UnityEngine.UI;


public class BullSuitTableCell : TableViewCell
{
    public Text TitleTxt;
    public Text SGTitleTxt;
    public Text NNTitleTxt;

    public Text NickTxt;
    public Text SGProfitTxt;
    public Text NNProfitTxt;

    public Image SGTagImg;
    public Image NNTagImg;

    public Image[] SGCardImgs;
    public Image[] NNCardImgs;

    public BullHistorySuitData Data;

    public override string ReuseIdentifier
    {
        get { return "BullSuitTableViewCRI"; }
    }

    public override void SetHighlighted()
    {

    }

    public override void SetSelected()
    {

    }

    public override void Display()
    {
        if (Data != null && Data.DataList.ContainsKey(Game.Instance.CurPlayer.Uid))
        {
            BullPlayerSuitData cps = Data.DataList[Game.Instance.CurPlayer.Uid];
            TitleTxt.text = string.Format(LocalizationManager.Instance.GetText("7009"), Data.Index, Data.HandId);// "第" + Data.Index + "手" + "(" + Data.HandId + ")";
            NickTxt.text = cps.Nickname;

            //三公
            SGTagImg.gameObject.SetActive(Data.SGBankUId == cps.UId);
            if (cps.SGBalance > 0)
            {
                SGProfitTxt.text = "+" + cps.SGBalance;
                SGProfitTxt.color = Color.green;
            }
            else if (cps.SGBalance < 0)
            {
                SGProfitTxt.text = "" + cps.SGBalance;
                SGProfitTxt.color = Color.red;
            }
            else
            {
                SGProfitTxt.text = "" + cps.SGBalance;
                SGProfitTxt.color = Color.white;
            }
            for (int i = 0; i < SGCardImgs.Length; i++)
            {
                if (i < cps.Cards.Length)
                {
                    SGCardImgs[i].gameObject.SetActive(true);
                    SGCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + cps.Cards[i]);
                }
                else
                {
                    SGCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/Bull/0");
                }
            }
            //牛牛
            
            NNTagImg.gameObject.SetActive(Data.NNBankUId == cps.UId);
            if (cps.NNBalance > 0)
            {
                NNProfitTxt.text = "+" + cps.NNBalance;
                NNProfitTxt.color = Color.green;
            }
            else if (cps.NNBalance < 0)
            {
                NNProfitTxt.text = "" + cps.NNBalance;
                NNProfitTxt.color = Color.red;
            }
            else
            {
                NNProfitTxt.text = "" + cps.NNBalance;
                NNProfitTxt.color = Color.white;
            }
            for (int i = 0; i < NNCardImgs.Length; i++)
            {
                if (i + 3 < cps.Cards.Length)
                {
                    NNCardImgs[i].gameObject.SetActive(true);
                    NNCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/" + cps.Cards[i + 3]);
                }
                else
                {
                    NNCardImgs[i].gameObject.SetActive(false);
                    NNCardImgs[i].sprite = Resources.Load<Sprite>("Textures/Poker/Bull/0");
                }
            }
        }
    }
}
