
using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 幸运玩家列表项
    /// </summary>
    public class ItemLuckyCell : TableViewCell
    {
        public CircleImage imgAvatar;
        public Image[] pokers;
        public Text tvName, tvId, tvTime, tvDate, tvGame;

        public ItemLuckyData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemLuckyCRI"; 
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
            LimitText.LimitAndSet(data.nickname, tvName, 120);
            tvGame.text = data.Game;
            tvId.text = "ID：" + data.uid;

            for(int i=0; i<data.CardList.Length;i++)
            {
                Sprite sprite = Resources.Load<Sprite>("Textures/Poker/"+ data.CardList[i]);
                if (i < pokers.Length)
                {
                    if (sprite)
                    {
                        pokers[i].sprite = sprite;
                    }
                }
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
