using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class ItemMemberCell : TableViewCell
    {
        public Image imgCreator;
        public CircleImage imgAvatar;
        public Text tvName, tvId, tvServiceFee, tvProfitLoss;

        public ItemMemberData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemMemberCRI";
            }
        }

        public override void Display()
        {
            if (data == null)
                return;

            tvServiceFee.text = data.serviceFee.ToString();
            if (data.profit > 0)
            {
                tvProfitLoss.text = "+" + data.profit.ToString();
                tvProfitLoss.color = Color.green;
            }
            else if (data.profit < 0)
            {
                tvProfitLoss.text = data.profit.ToString();
                tvProfitLoss.color = Color.red;
            }
            else
            {
                tvProfitLoss.color = Color.white;
                tvProfitLoss.text = data.profit.ToString();
            }
            // tvName.text = data.nickname;
            LimitText.LimitAndSet(data.nickname, tvName, 200);
            tvId.text = "ID：" + data.uid;

            if (ClubMainView.Instance.IsNormal)
            {
                tvServiceFee.gameObject.SetActive(false);
                tvProfitLoss.gameObject.SetActive(false);
            }
            else
            {
                tvServiceFee.gameObject.SetActive(true);
                tvProfitLoss.gameObject.SetActive(true);
            }

            if (data.isCreator)
            {
                imgCreator.gameObject.SetActive(true);
                Sprite sprite = Resources.Load<Sprite>("Textures/Club/Member/m_creator");
                if (sprite)
                {
                    imgCreator.sprite = sprite;
                }
            }
            else if (data.isProxy)
            {
                imgCreator.gameObject.SetActive(true);
                Sprite sprite = Resources.Load<Sprite>("Textures/Club/Member/m_admin");
                if (sprite)
                {
                    imgCreator.sprite = sprite;
                }
            }
            else
            {
                imgCreator.gameObject.SetActive(false);
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
