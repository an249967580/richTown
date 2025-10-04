using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 幸运玩家列表项
    /// </summary>
    public class ItemWinOrLossCell : TableViewCell
    {
        public CircleImage imgAvatar;
        public Text tvName, tvId, tvTime, tvDate, tvGame, tvProfit;

        public ItemWinOrLossData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemWinOrLossCRI";
            }
        }

        public override void Display()
        {
           if(data == null)
            {
                return;
            }

            tvTime.text = TimeUtil.SecondsToDate(data.startTime).ToString("HH:mm");
            tvDate.text = TimeUtil.SecondsToDate(data.startTime).ToString("MM/dd");
            //tvName.text = data.nick;
            LimitText.LimitAndSet(data.nickname, tvName, 130);
            tvGame.text = data.Game;
            tvId.text = "ID：" + data.uid;
            if (data.profitLoss > 0)
            {
                tvProfit.text = "+" + data.profitLoss.ToString();
                tvProfit.color = Color.green;
            }
            else if (data.profitLoss < 0)
            {
                tvProfit.text = data.profitLoss.ToString();
                tvProfit.color = Color.red;
            }
            else
            {
                tvProfit.color = Color.white;
                tvProfit.text = data.profitLoss.ToString();
            }

        }

        public override void SetHighlighted()
        {
        }

        public override void SetSelected()
        {
        }
    }
}
