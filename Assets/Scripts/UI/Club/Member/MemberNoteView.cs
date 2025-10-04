using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{

    public delegate void UpdateNoteEvent(string note);

    /// <summary>
    /// 会员备注编辑
    /// </summary>
    public class MemberNoteView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnSure, btnCancel;
        public InputField ipNote;

        public UpdateNoteEvent OnUpdateNoteEvent;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnCancel.onClick.AddListener(HideAndDestory);
            btnSure.onClick.AddListener(editNote);
        }

        public void InitView(string note)
        {
            ipNote.text = note;
        }

        // 编辑备注
        void editNote()
        {
            string note = ipNote.text.Trim();
            if(string.IsNullOrEmpty(note))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5016"));
                return;
            }

            OnUpdateNoteEvent(note);
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
