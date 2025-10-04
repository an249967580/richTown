using System;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class CalendarView : MonoBehaviour
    {
        public GridView gvDay;
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

        private void Start()
        {

            Date = DateTime.Now;
            CreateCalendar();
        }

        public void CreateCalendar()
        {
            gvDay.Clear();
            int last = getDaysOfMonth(_month, _year);

            int firstWeaken = getWeekond(1, _month, _year);  // 第一天是星期几

            int cellCount = firstWeaken + 1 + last;
            while (cellCount % 7 != 0)
            {
                cellCount++;
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
                    ItemDateView vi = gvDay.Add(data) as ItemDateView;
                    vi.EnableClick(true);
                    vi.OnItemClickEvent = onItemClickListener;
                }
                else
                {
                    ItemDateData data = new ItemDateData();
                    data.day = 0;
                    ItemDateView vi = gvDay.Add(data) as ItemDateView;
                    vi.EnableClick(false);
                }

            }
        }

        void onItemClickListener(ItemView vi)
        {
            foreach(ItemView itemView in gvDay.ViewItems)
            {
                ItemDateView tmp = itemView as ItemDateView;
                if (vi.Data.Id() == tmp.Data.Id())
                {
                    tmp.SetSelected(true);
                }
                else
                {
                    tmp.SetSelected(false);
                }
            }
        }

        /// <summary>
        /// 通过年月获取当前月份的天数
        /// </summary>
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
    }
    
}
