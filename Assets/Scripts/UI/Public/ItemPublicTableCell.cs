
using Assets.Scripts.TableView;
using UnityEngine.UI;

namespace RT
{
    public class ItemPublicTableCell : TableViewCell
    {
        public Text tvRoomName, tvPlayers, tvBlinds, tvBuyIn;
        public ItemTableData data;

        public Image suoIcon;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemPublicTableCRI";
            }
        }

        public override void Display()
        {
            if(data == null)
            {
                return;
            }
            tvRoomName.text = data.title;
            tvPlayers.text = string.Format("{0}/{1}", data.roomPlayers, data.playerNum);
            if (GameType.IsDz(data.game))
            {
                tvBlinds.text = string.Format("{0}/{1}", data.blindBet / 2, data.blindBet);
            }else
            {
                tvBlinds.text = data.blindBet.ToString();
            }
            tvBuyIn.text = data.minBet.ToString();
            if (data.isPin)
            {
                suoIcon.gameObject.SetActive(true);
            }
            else
            {
                suoIcon.gameObject.SetActive(false);
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
