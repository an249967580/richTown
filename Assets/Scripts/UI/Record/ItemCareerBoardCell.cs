using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class ItemCareerBoardCell : TableViewCell
    {
        public Text tvTitle, tvProfit, tvBlinds, tvBoardTime, tvHandNum, tvDateTime, tvClubName;
        public Image imgSubscript;

        public ItemCareerBoardData data;

        public override string ReuseIdentifier
        {
            get
            {
                return  "ItemCareerBoardCRI";
            }
        }

        public override void Display()
        {
            if(data == null)
            {
                return;
            }
            string title = string.Format("{0}   {1}", TimeUtil.SecondsToDate(data.startTime).ToString("MM-dd"), data.roomName);
            LimitText.LimitAndSet(title, tvTitle, 300);
            string res = "";
            if(data.profit > 0)
            {
                tvProfit.text = "+" + data.profit;
                res = "Textures/Career/subscript_win";
            }
            else
            {
                tvProfit.text = data.profit.ToString();
                res = "Textures/Career/subscript_loss";
            }

            Sprite sprite = Resources.Load<Sprite>(res);
            if(sprite)
            {
                imgSubscript.sprite = sprite;
            }

            tvDateTime.text = TimeUtil.SecondsToDate(data.startTime).ToString("HH:mm");
            tvClubName.text = data.clubName;
            tvHandNum.text = data.handNum + LocalizationManager.Instance.GetText("3009");
            tvBoardTime.text = timeStr(data.boardTime);
            tvBlinds.text = (GameType.IsDz(data.game) ? string.Format("{0}/{1}", data.blinds / 2, data.blinds) : data.blinds.ToString());
        }

        public override void SetHighlighted()
        {
            
        }

        public override void SetSelected()
        {
            
        }

        string timeStr(int minutes)
        {
            return (minutes * 1.0f / 3600) + "h";
        }
    }
}
