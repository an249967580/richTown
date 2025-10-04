using UnityEngine.UI;

namespace RT
{

    public delegate void ConfirmEvent(ConfirmView vi);

    public class ConfirmView : HideMonoBehaviour
    {
        public Text tvMsg;
        public Button btnSure, btnCancel;

        public ConfirmEvent OnConfirmEvent;
        private bool defaultSureEvent;

        private void Awake()
        {
            btnCancel.onClick.AddListener(HideAndDestory);
            btnSure.onClick.AddListener(() =>
            {
                if(OnConfirmEvent != null)
                {
                    OnConfirmEvent(this);
                    if (defaultSureEvent)
                    {
                        HideAndDestory();
                    }
                }
            });
        }

        public void ShowTip(string msg, bool defaultSureEvent = true)
        {
            tvMsg.text = msg;
            this.defaultSureEvent = defaultSureEvent;
        }
    }
}
