using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace RT
{
    public delegate void DateSelectEvent(DateTime min, DateTime max);

    public class DateSelectView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnSure;
        public ListView lstView;
        public DateSelectEvent OnDateSelectEvent;
        public DateTime? minDate, maxDate;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnSure.onClick.AddListener(()=>
            {
                if (OnDateSelectEvent != null)
                {
                    if(minDate == null)
                    {
                        minDate = DateTime.Now;
                    }
                    if(maxDate == null)
                    {
                        maxDate = minDate;
                    }
                    OnDateSelectEvent(minDate.Value, maxDate.Value);
                }
                HideAndDestory();
            });
        }

        private void Start()
        {
            
            DateTime now = DateTime.Now;

            for (int i = 3; i >= 0; i--)
            {
                ItemCalendarData data = new ItemCalendarData();
                data.date = now.AddMonths(-i);
                ItemCalendarView vi = lstView.Add(data) as ItemCalendarView;
                vi.OnCalendarSelectEvent = onCalendarSelectEvent;
            }
            StartCoroutine(scrollToBottom());
        }

        // 滑到底部
        IEnumerator scrollToBottom()
        {
            yield return new WaitForEndOfFrame();
            ScrollRect rect = lstView.VerticleLG.gameObject.transform.parent.GetComponent<ScrollRect>();
            if (rect)
            {
                RectTransform obj = lstView.ViewItems[lstView.ViewItems.Count - 1].GetComponent<RectTransform>();
                float normalizePosition = rect.GetComponent<RectTransform>().anchorMin.y - obj.anchoredPosition.y;
                normalizePosition += (float)obj.transform.GetSiblingIndex() / (float)rect.content.transform.childCount;
                normalizePosition /= 1000f;
                normalizePosition = Mathf.Clamp01(1 - normalizePosition);
                rect.verticalNormalizedPosition = normalizePosition;
            }
        }

        bool onCalendarSelectEvent(DateTime date)
        {
            if (minDate == null && maxDate == null)
            {
                minDate = date;
            }
            else if (minDate != null && maxDate != null)
            {
                minDate = date;
                maxDate = null;
            }
            else if (minDate != null && date.CompareTo(minDate.Value) < 0)
            {
                minDate = date;
                maxDate = null;
            }
            else if (minDate != null && date.CompareTo(minDate.Value) > 0)
            {
                DateTime tmp = date.AddDays(-6);
                if(compareDay(tmp, minDate.Value))
                {
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5709"));
                    return true;
                }
                else
                {
                    maxDate = date;
                }
            }
            updateUI();
            return true;
        }

        void updateUI()
        {
            foreach (ItemView vi in lstView.ViewItems)
            {
                ItemCalendarView view = vi as ItemCalendarView;
                ItemCalendarData data = vi.Data as ItemCalendarData;

                if(minDate != null)
                {
                    if (isDateEquals(data.date, minDate.Value))
                    {
                        view.SetChildSelect(minDate.Value.Day);
                    }
                    #region 大日期不为空
                    if (maxDate != null)
                    {
                        if (isDateEquals(minDate.Value, maxDate.Value) && isDateEquals(minDate.Value, data.date)) //  同一个月
                        {
                            view.SetChildSelect(minDate.Value.Day, maxDate.Value.Day);
                        }
                        else if (isDateLarge(maxDate.Value, minDate.Value)) // 不同月份（跨月）
                        {
                            if (isDateEquals(data.date, minDate.Value))
                            {
                                view.SetChildSelectLarge(minDate.Value.Day);
                            }
                            if (isDateEquals(data.date, maxDate.Value))
                            {
                                view.SetChildSelectLess(maxDate.Value.Day);
                            }
                        }
                        else
                        {
                            view.SetChildSelect(false);
                        }
                    }
                    else
                    {
                        if (isDateEquals(data.date, minDate.Value))
                        {
                            view.SetChildSelect(minDate.Value.Day);
                        }
                        else
                        {
                            view.SetChildSelect(false);
                        }
                    }
                    #endregion
                }
                else
                {
                    view.SetChildSelect(false);
                } 
            }
        }

        bool isDateEquals(DateTime d1, DateTime d2)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month;
        }

        bool isDateLarge(DateTime d1, DateTime d2)
        {
            if(d1.Year > d2.Year)
            {
                return true;
            }

            if(d2.Year == d2.Year)
            {
                return d1.Month > d2.Month;
            }

            return false;
        }

        bool compareDay(DateTime d1, DateTime d2)
        {
            if (d1.Year > d2.Year)
            {
                return true;
            }

            if (d2.Year == d2.Year)
            {
                if(d1.Month > d2.Month)
                {
                    return true;
                }

                if (d1.Month == d2.Month)
                {
                    return d1.Day > d2.Day;
                }
            }

            return false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != gameObject)
            {
                return;
            }
            if (gameObject.activeSelf)
            {
                HideAndDestory();
            }
        }

    }
}
