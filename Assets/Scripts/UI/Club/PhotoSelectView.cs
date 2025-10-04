using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 照片选择
    /// </summary>
    public class PhotoSelectView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnAlbum, btnCancel;

		#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _iOSOpenPhotoAlbum();
        #endif

        private void Awake()
        {
            btnCancel.onClick.AddListener(HideAndDestory);
            btnClose.onClick.AddListener(HideAndDestory);
            btnAlbum.onClick.AddListener(openAlbum);
            NotificationCenter.Instance.AddNotifyListener(NotificationType.EditAvatar, onSelectPhoto);
            NotificationCenter.Instance.AddNotifyListener(NotificationType.EditClubAvatar, onSelectPhoto);
        }

        void onSelectPhoto(NotifyMsg msg)
        {
            HideAndDestory();
        }

        private void OnDestroy()
        {
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.EditAvatar, onSelectPhoto);
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.EditClubAvatar, onSelectPhoto);
        }
        void openAlbum()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            Game.Instance.ShowTips("PC 端不支持");
#elif UNITY_ANDROID
           androidOpen();
#elif UNITY_IOS
			_iOSOpenPhotoAlbum();
#endif
        }

        void androidOpen()
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("OpenAlbum");
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
