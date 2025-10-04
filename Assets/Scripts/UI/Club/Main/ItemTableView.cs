using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 德州桌子
    /// </summary>
    public class ItemTableView : ItemView
    {
        public Text tvTitle, tvBlinds, tvTime, tvPlayer;
        public Image imgBg, imgAddBg;
        public GameObject goAdd, goInfo;

        public override void RegisterEvent()
        {
            GetComponent<Button>().onClick.AddListener(()=>
            {
                if(OnItemClickEvent != null)
                {
                    OnItemClickEvent(this);
                }
            });

            goAdd.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (OnItemClickEvent != null)
                {
                    OnItemClickEvent(this);
                }
            });
        }

        public override void Render()
        {
            ItemTableData data = Data as ItemTableData;
            if (data.isCreator)
            {
                goAdd.gameObject.SetActive(true);
                goInfo.gameObject.SetActive(false);
            }
            else
            {
                goAdd.gameObject.SetActive(false);
                goInfo.gameObject.SetActive(true);
                // tvTitle.text = data.title;
                LimitText.LimitAndSet(data.title, tvTitle, 120);
                tvPlayer.text = string.Format("{0}/{1}", data.roomPlayers, data.playerNum);
                tvTime.text = secondsToStr(data.roomTime);
            }
            if (data.isTexas)
            {
                tvBlinds.text = string.Format(LocalizationManager.Instance.GetText("5903"), data.blindBet / 2, data.blindBet);
                getSprite("texas_table_bg", (sprite) =>
                {
                    imgBg.sprite = sprite;
                });
                getSprite("texas_table_bg_add", (sprite) =>
                {
                    imgAddBg.sprite = sprite;
                });
            }
            else
            {
                tvBlinds.text = string.Format(LocalizationManager.Instance.GetText("5904"), data.blindBet);
                getSprite("bull_table_bg", (sprite) =>
                {
                    imgBg.sprite = sprite;
                });
                getSprite("bull_table_bg_add", (sprite) =>
                {
                    imgAddBg.sprite = sprite;
                });
            }
        }

        void getSprite(string fileName, Action<Sprite> callBack)
        {
            string url = "Textures/Club/Main/" + fileName;
            Sprite sprite = Resources.Load<Sprite>(url);
            if(sprite)
            {
                callBack(sprite);
            }
        }

        string secondsToStr(long seconds)
        {
            long hour = seconds / 3600;
            long minute = (seconds - hour * 3600) / 60;
            long secs = seconds - hour * 3600 - minute * 60;
            StringBuilder sb = new StringBuilder();
            if(hour >= 10)
            {
                sb.Append(hour + "");
            }
            else
            {
                sb.Append("0" + hour);
            }
            sb.Append(":");
            if (minute >= 10)
            {
                sb.Append(minute + "");
            }
            else
            {
                sb.Append("0" + minute);
            }
            sb.Append(":");
            if (secs >= 10)
            {
                sb.Append(secs + "");
            }
            else
            {
                sb.Append("0" + secs);
            }

            return sb.ToString();
        }
    }
}
