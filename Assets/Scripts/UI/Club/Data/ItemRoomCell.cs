using Assets.Scripts.TableView;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 房间数据(每张桌子数据)
    /// </summary>
    public class ItemRoomCell : TableViewCell
    {
        public CircleImage imgAvatar;
        public Text tvTime, tvDate, tvBlinds, tvRate, tvServiceFee;
        public ItemRoomData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemRoomCRI";
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

            if (GameType.IsDz(data.game))
            {
                tvBlinds.text = string.Format(LocalizationManager.Instance.GetText("5903"), data.blindBet / 2, data.blindBet);
            }
            else
            {
                tvBlinds.text = string.Format(LocalizationManager.Instance.GetText("5904"), data.blindBet / 2, data.blindBet);
            }
            
            tvServiceFee.text = data.serviceCharge.ToString();
            tvRate.text = string.Format("{0}%", data.Rate);
        }

        public override void SetHighlighted()
        {
            
        }

        public override void SetSelected()
        {
            
        }
    }
}
