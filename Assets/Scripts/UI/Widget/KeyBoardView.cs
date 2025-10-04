using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public delegate void KeyBoardEvent(string pin);

    public class KeyBoardView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnDel, btnClose;
        public Button[] btnDigitals;
        public Text[] tv;

        public KeyBoardEvent OnKeyBoardEvent;

        private string _pin;

        private void Awake()
        {
            _pin = string.Empty;
            btnDel.onClick.AddListener(onDel);
            btnClose.onClick.AddListener(HideAndDestory);
            for (int i=0;i<btnDigitals.Length;i++)
            {
                Button btn = btnDigitals[i];
                btn.onClick.AddListener(() =>
                {
                    onClick(btn.name);
                });
            }
        }

        void onDel()
        {
            int length = _pin.Length;
            if (length > 0)
            {
                _pin = _pin.Substring(0, length - 1);
                setTv();
            }
        }

        void onClick(string pin)
        {
            if (_pin.Length < 4)
            {
                _pin += pin;
                setTv();
            }

            if(_pin.Length == 4)
            {
                if (OnKeyBoardEvent != null)
                {
                    OnKeyBoardEvent(_pin);
                    HideAndDestory();
                }
            }
        }

        void setTv()
        {
            int length = _pin.Length;
            for (int i = 0; i < length; i++)
            {
                tv[i].text = _pin.Substring(i, 1);
            }

            for(int i=length;i<tv.Length; i++)
            {
                tv[i].text = string.Empty;
            }
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
