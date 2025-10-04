using Assets.Scripts.TableView;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 收发记录列表项
    /// </summary>
    public class ItemRecordCell: TableViewCell
    {
        public Text tvName, tvLb, tvCoins, tvTime;
        public ItemRecordData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemRecordCRI";
            }
        }

        public override void Display()
        {
             if(data == null)
            {
                return;
            }

            //tvName.text = data.sender;
            LimitText.LimitAndSet(data.sender, tvName, 200);
            if (data.type == 1)
            {
                tvLb.text = string.Format(LocalizationManager.Instance.GetText("5309"), LimitText.Limit(data.recipient, tvLb, 170));
            }
            else
            {
                tvLb.text = string.Format(LocalizationManager.Instance.GetText("5310"), LimitText.Limit(data.recipient, tvLb, 170));
            }
            tvCoins.text = data.coin.ToString();
            tvTime.text = TimeUtil.SecondsToDate(data.time).ToString("d");
        }

        public override void SetHighlighted()
        {
            
        }

        public override void SetSelected()
        {
            
        }
    }
}
