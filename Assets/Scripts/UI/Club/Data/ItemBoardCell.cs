using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class ItemBoardCell : TableViewCell
    {
        public CircleImage imgAvatar;
        public Text tvName, tvId, tvHandNum, tvProfit;

        public ItemBoardData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemBoardCRI";
            }
        }

        public override void Display()
        {
            if(data == null)
            {
                return;
            }
            //tvName.text = data.nick;
            LimitText.LimitAndSet(data.nickname, tvName, 180);
            tvId.text = "ID：" + data.uid.ToString();
            tvHandNum.text = data.handNum.ToString();
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
