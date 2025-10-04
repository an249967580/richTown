using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class ItemEmailCell : TableViewCell
    {
        public ItemEmailData data;
        public Image imgStatus;
        public Text tvTitle, tvDate;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemEmailCRI";
            }
        }

        public override void Display()
        {
            if (data == null)
                return;
            string res = "";
            if(data.readFlag == 0)
            {
                res = "Textures/Email/email_new";
            }
            else
            {
                res = "Textures/Email/email_old";
            }
            Sprite sprite = Resources.Load<Sprite>(res);
            if(sprite)
            {
                imgStatus.sprite = sprite;
            }
            tvTitle.text = data.title;
            tvDate.text = TimeUtil.SecondsToDate(data.createTime).ToString("yyyy/MM/dd");
        }

        public override void SetHighlighted()
        {
            
        }

        public override void SetSelected()
        {
            
        }
    }
}
