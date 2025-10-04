using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{

    /// <summary>
    /// 俱乐部列表项
    /// </summary>
    public class ItemClubCell : TableViewCell
    {
        public Image imgAvatar, imgOwnerFlag, imgExpir;
        public Text tvName, tvCoin, tvExpir;
        public Slider sLeavel;
        public GameObject goExpir;

        public ItemClubData data;

        public override string ReuseIdentifier
        {
            get
            {
                return "ItemCellCRI";
            }
        }

        public override void Display()
        {
            if(data == null)
            {
                return;
            }

            //tvName.text = data.name;
            LimitText.LimitAndSet(data.name, tvName, 250);
            sLeavel.value = data.level;
            tvCoin.text = data.coin.ToString();
            if (data.level > 0)
            {
                imgExpir.gameObject.SetActive(true);
                if (data.expir > 0)
                {
                    tvExpir.text = data.expir.ToString() + LocalizationManager.Instance.GetText("5013");
                }
                else
                {
                    tvExpir.text = "已过期";
                }
            }
            else
            {
                imgExpir.gameObject.SetActive(false);
            }

            if (data.isCreator)
            {
                imgOwnerFlag.gameObject.SetActive(true);
                Sprite sprite = Resources.Load<Sprite>("Textures/Club/Home/item_creator");
                if(sprite)
                {
                    imgOwnerFlag.sprite = sprite;
                }
            }
            else if (data.isProxy)
            {
                imgOwnerFlag.gameObject.SetActive(true);
                Sprite sprite = Resources.Load<Sprite>("Textures/Club/Home/item_proxy");
                if (sprite)
                {
                    imgOwnerFlag.sprite = sprite;
                }
            }
            else
            {
                imgOwnerFlag.gameObject.SetActive(false);
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
