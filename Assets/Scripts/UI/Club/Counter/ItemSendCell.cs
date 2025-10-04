using Assets.Scripts.TableView;
using UnityEngine.UI;

namespace RT
{

    public delegate void ItemToggleEvent(bool isOn, ItemMemberData data);

    /// <summary>
    /// 收发筹码列表项
    /// </summary>
    public class ItemSendCell : TableViewCell
    {
        public CircleImage imgAvatar;
        public Text tvName, tvId, tvCoins;
        public Button btnOn, btnOff;

        public ItemMemberData data;

        public ItemToggleEvent OnItemToggleEvent;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemSendCRI";
            }
        }

        protected override void Awake()
        {
            base.Awake();
            btnOn.onClick.AddListener(()=>
            {
                if(OnItemToggleEvent != null && data != null)
                {
                    OnItemToggleEvent(true, data);
                }
            });
            btnOff.onClick.AddListener(() =>
            {
                if (OnItemToggleEvent != null && data != null)
                {
                    OnItemToggleEvent(false, data);
                }
            });
        }

        public override void Display()
        {
            if(data == null)
            {
                return;
            }
            tvId.text = "ID：" + data.uid;
            //tvName.text = data.nickname;
            LimitText.LimitAndSet(data.nickname, tvName, 150);
            tvCoins.text = data.coin.ToString();
            btnOff.gameObject.SetActive(!data.isChecked);
            btnOn.gameObject.SetActive(data.isChecked);
        }


        public override void SetHighlighted()
        {
            
        }

        public override void SetSelected()
        {
            
        }
    }
}
