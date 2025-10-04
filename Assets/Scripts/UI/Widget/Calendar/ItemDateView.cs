using System;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class ItemDateView : ItemView
    {

        public Text tvDay;
        public Image imgSelect;
        private Button _btn;
        private ItemDateData data;

        public override void RegisterEvent()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(delegate
            {
                if (OnItemClickEvent != null)
                {
                    OnItemClickEvent(this);
                }
            });
        }

        public override void Render()
        {
            data = Data as ItemDateData;

            tvDay.text = data.day.ToString();
            imgSelect.gameObject.SetActive(data.isSelect);
            if (equals(data.date))
            {
                tvDay.color = Color.yellow;
                _btn.interactable = true;
            }
            if (large(data.date))
            {
                tvDay.color = Color.gray;
                _btn.interactable = false;
            }
        }

        public void EnableClick(bool enabled)
        {
            // _btn.interactable = enabled;
            if (!enabled)
            {
                tvDay.text = string.Empty;
            }

        }

        public void SetSelected(bool select)
        {
            data.isSelect = select;
            imgSelect.gameObject.SetActive(data.isSelect);
        }

        bool equals(DateTime date)
        {
            DateTime now = DateTime.Now;
            return date.Year == now.Year && date.Month == now.Month && date.Day == now.Day;
        }

        bool large(DateTime date)
        {
            DateTime now = DateTime.Now;

            if (date.Year >= now.Year)
            {
                if (date.Year > now.Year)
                {
                    return true;
                }
                else
                {
                    if (date.Month >= now.Month)
                    {
                        if (date.Month > now.Month)
                        {
                            return true;
                        }
                        else
                        {
                            return date.Day > now.Day;
                        }
                    }
                }
            }
            return false;
        }
    }

    public class ItemDateData : ItemData
    {
        public int index;
        public int day;
        public bool isSelect;

        public DateTime date;

        public override long Id()
        {
            return day;
        }
    }
}
