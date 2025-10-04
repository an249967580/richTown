
using Assets.Scripts.TableView;
using UnityEngine.UI;

namespace RT
{
    public class ItemAnnouncementCell : TableViewCell
    {
        public Text tvTitle;
        public ItemAnnouncementData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemAnnouncementCRI";
            }
        }

        public override void Display()
        {
            if (data == null)
                return;
            tvTitle.text = data.Title;
        }

        public override void SetHighlighted()
        {
            
        }

        public override void SetSelected()
        {
            
        }
    }
}
