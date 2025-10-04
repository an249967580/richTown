using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{

    public delegate void EditInfoEvent(string name, string intro);
    public delegate void EditAvatarEvent(byte[] bytes);

    /// <summary>
    /// 创建俱乐部
    /// </summary>
    public class ClubEditView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnCreate;
        public InputField ipName, ipIntro;
        public Image imgAvatar, imgBg;

        public EditInfoEvent OnEditInfoEvent;
        public EditAvatarEvent OnEditAvatarEvent;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnCreate.onClick.AddListener(editClub);
            imgAvatar.gameObject.SetActive(false);
            UIEventListener.Get(imgBg.gameObject).onClick = photoSelect;
            NotificationCenter.Instance.AddNotifyListener(NotificationType.EditClubAvatar, onSelectPhoto);
        }

        // 图片选择成功，上传并更新头像
        void onSelectPhoto(NotifyMsg msg)
        {
            string bytes = msg["avatar"] as string;
            if(OnEditAvatarEvent != null)
            {
                OnEditAvatarEvent(System.Convert.FromBase64String(bytes));
            }
        }

        public void InitView(string name, string intro, string avatar)
        {
            ipName.text = name;
            ipIntro.text = intro;
            LoadAvatar(avatar);
        }

        private void OnDestroy()
        {
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.EditClubAvatar, onSelectPhoto);
        }

        public void LoadAvatar(string avatar)
        {
            if (Validate.IsNotEmpty(avatar))
            {
                StartCoroutine(LoadImageUtil.LoadImage(avatar, (sprite)=>
                {
                    imgAvatar.gameObject.SetActive(true);
                    imgAvatar.sprite = sprite;
                }));
            }
            else
            {
                imgAvatar.gameObject.SetActive(false);
            }
        }

        void editClub()
        {
            string name = ipName.text.Trim();
            string intro = ipIntro.text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5109"));
                return;
            }

            if (string.IsNullOrEmpty(intro))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5110"));
                return;
            }

            if(OnEditInfoEvent != null)
            {
                OnEditInfoEvent(name, intro);
            }
        }

        void photoSelect(GameObject go)
        {
            if(go.gameObject == imgBg.gameObject)
            {
                UIClubSpawn.Instance.CreatePhotoSelectView();
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
