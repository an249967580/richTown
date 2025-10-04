using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public delegate bool CalendarSelectEvent(DateTime date);

    public class ItemCalendarView : ItemView, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private GridView _gvDay;
        private ScrollRect _rect;

        public Text tvTitle;

        private int _year;
        private int _month;
        private int _day;
        private DateTime _date;

        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                _year = _date.Year;
                _month = _date.Month;
                _day = _date.Day;
            }
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }
        public int Month
        {
            get { return _month; }
            set { _month = value; }
        }
        public int Day
        {
            get { return _day; }
            set { _day = value; }
        }

        public CalendarSelectEvent OnCalendarSelectEvent;

        private DateTime? _selectDate;
        private RectTransform _rt;


        public override void Awake()
        {
            _gvDay = GetComponent<GridView>();
            base.Awake();
        }

      
        #region 日历排版

        // 通过年月获取当前月份的天数
        int getDaysOfMonth(int mm, int yy)
        {
            if (mm <= 0 || mm > 12)
            {
                return 0;
            }
            switch (mm)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2:
                    if ((yy % 400 == 0) || ((yy % 100 != 0) && (yy % 4 == 0)))
                        return 29;
                    return 28;
                default:
                    return 30;
            }
        }
        // 某一天为周几
        int getWeekond(int day, int mm, int yy)
        {
            DateTime d = new DateTime(yy, mm, day);
            return (int)d.DayOfWeek;
        }

        public void CreateCalendar()
        {
            _gvDay.Clear();
            int last = getDaysOfMonth(_month, _year);
            int firstWeaken = getWeekond(1, _month, _year);  // 第一天是星期几

            int cellCount = firstWeaken + 1 + last;
            while (cellCount % 7 != 0)
            {
                cellCount++;
            }

            int n = cellCount / 7;          // 日历行数
            _rt = GetComponent<RectTransform>(); // 更改日历高度
            switch (n)
            {
                case 4:
                    _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, _rt.sizeDelta.y - 200);
                    break;
                case 5:
                    _rt.sizeDelta = new Vector2(_rt.sizeDelta.x, _rt.sizeDelta.y - 100);
                    break;
            }

            int day = 0;
            for (int i = 0; i < cellCount; i++)
            {
                if (i >= firstWeaken && i < last + firstWeaken)
                {
                    day++;
                    ItemDateData data = new ItemDateData();
                    data.day = day;
                    data.date = new DateTime(Year, Month, day, 0, 0, 0);
                    ItemDateView vi = _gvDay.Add(data) as ItemDateView;
                    vi.EnableClick(true);
                    vi.OnItemClickEvent = onItemClickListener;
                }
                else
                {
                    ItemDateData data = new ItemDateData();
                    data.day = 0;
                    ItemDateView vi = _gvDay.Add(data) as ItemDateView;
                    vi.EnableClick(false);
                }

            }
        }

        // 点击事件
        void onItemClickListener(ItemView vi)
        {
            DateTime sel = new DateTime(_year, _month, (int)vi.Data.Id());
            if (OnCalendarSelectEvent != null)
            {
                if (OnCalendarSelectEvent(sel))
                {
                    _selectDate = sel;
                }
            }
        }

        public override void Render()
        {
            ItemCalendarData data = Data as ItemCalendarData;
            Date = data.date;
            tvTitle.text = string.Format("{0}/{1}", _month, _year);
            CreateCalendar();
        }

        #endregion

        #region 拖曳处理

        public void OnDrag(PointerEventData eventData)
        {
            if(!_rect)
            {
                _rect = gameObject.transform.parent.parent.GetComponent<ScrollRect>();
            }
            _rect.OnDrag(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_rect)
            {
                _rect = gameObject.transform.parent.parent.GetComponent<ScrollRect>();
            }
            _rect.OnBeginDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_rect)
            {
                _rect = gameObject.transform.parent.parent.GetComponent<ScrollRect>();
            }
            _rect.OnEndDrag(eventData);
        }

        #endregion

        public void SetChildSelect(int day)
        {
            foreach(ItemDateView tmp in _gvDay.ViewItems)
            {
                if(tmp.Data.Id() == day)
                {
                    tmp.SetSelected(true);
                }
                else
                {
                    tmp.SetSelected(false);
                }
            }
        }

        public void SetChildSelect(int dayMin, int dayMax)
        {
            foreach (ItemDateView tmp in _gvDay.ViewItems)
            {
                if (tmp.Data.Id() >= dayMin && tmp.Data.Id() <= dayMax)
                {
                    tmp.SetSelected(true);
                }
                else
                {
                    tmp.SetSelected(false);
                }
            }
        }

        public void SetChildSelect(bool selected)
        {
            foreach (ItemDateView tmp in _gvDay.ViewItems)
            {
                tmp.SetSelected(selected);
            }
        }

        public void SetChildSelectLarge(int day)
        {
            foreach (ItemDateView tmp in _gvDay.ViewItems)
            {
                if (tmp.Data.Id() >= day)
                {
                    tmp.SetSelected(true);
                }
                else
                {
                    tmp.SetSelected(false);
                }
            }
        }

        public void SetChildSelectLess(int day)
        {
            foreach (ItemDateView tmp in _gvDay.ViewItems)
            {
                if (tmp.Data.Id() <= day)
                {
                    tmp.SetSelected(true);
                }
                else
                {
                    tmp.SetSelected(false);
                }
            }
        }
    }

    public class ItemCalendarData : ItemData
    {
        public int id;
        public DateTime date;

        public override long Id()
        {
            return id;
        }
    }
}
