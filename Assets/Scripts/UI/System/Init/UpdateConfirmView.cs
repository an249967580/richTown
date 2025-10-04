using UnityEngine.UI;

namespace RT
{
    public enum UpdateEventOp
    {
        Update,
        Cancel
    }

    public delegate void UpdateEvent(UpdateEventOp op);

    public class UpdateConfirmView : HideMonoBehaviour
    {
        public Button btnSure, btnCancel;
        public Text tvMsg;
        public UpdateEvent OnUpdateEvent;

        private Version _version;
        public Version lastVersion
        {
            get
            {
                return _version;
            }

            set
            {
                _version = value;
                string msg = _version.versionName + "\n\n" + _version.updateLog;
                tvMsg.text = msg;
            }
        }

        private void Awake()
        {
            btnCancel.onClick.AddListener(() =>
            {
                onUpdateEvent(UpdateEventOp.Cancel);
            });

            btnSure.onClick.AddListener(() =>
            {
                onUpdateEvent(UpdateEventOp.Update);
            });
        }

        void onUpdateEvent(UpdateEventOp op)
        {
            if(OnUpdateEvent != null)
            {
                OnUpdateEvent(op);
            }
        }
    }
}
