using UnityEngine;
using UnityEngine.EventSystems;

namespace RT
{
    public class UIEventListener : EventTrigger
    {
        public delegate void Delegate(GameObject go);
        public Delegate onClick;
        public Delegate onDown;
        public Delegate onEnter;
        public Delegate onExit;
        public Delegate onUp;
        public Delegate onSelect;
        public Delegate onUpdateSelect;

        static public UIEventListener Get(GameObject go)
        {
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if(!listener)
            {
                listener = go.AddComponent<UIEventListener>();
            }
            return listener;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null) onClick(gameObject);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null) onDown(gameObject);
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null) onEnter(gameObject);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null) onExit(gameObject);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null) onUp(gameObject);
        }
        public override void OnSelect(BaseEventData eventData)
        {
            if (onSelect != null) onSelect(gameObject);
        }
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            if (onUpdateSelect != null) onUpdateSelect(gameObject);
        }
    }
}
