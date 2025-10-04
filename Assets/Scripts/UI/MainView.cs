using SimpleJson;
using System;
using UnityEngine;

namespace RT
{
    public class MainView : MonoBehaviour
    {
        public RectTransform parent;

        private LoadMask _loadMask;

        public static MainView Instance;

        private void Awake()
        {
            Instance = this;
            Screen.orientation = ScreenOrientation.Portrait;
            NotificationCenter.Instance.AddNotifyListener(NotificationType.OnMsg, onNotify);
        }

        #region 照片选择弹窗
        public PhotoSelectView CreatePhotoSelectView()
        {
            PhotoSelectView view = load<PhotoSelectView>("Prefabs/Club/PhotoSelectView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("PhotoSelectView Create Failed ...");
        }
        #endregion

        #region 加载框
        public LoadMask CreateLoadMask()
        {
            if (!_loadMask)
            {
                _loadMask = load<LoadMask>("Prefabs/Widgets/LoadMask");
                if (_loadMask)
                {
                    setRectTransform(_loadMask.gameObject);
                    return _loadMask;
                }
                throw new Exception("LoadMask Create Failed ...");
            }
            return _loadMask;
        }

        public void HideMask()
        {
            if (_loadMask)
            {
                Destroy(_loadMask.gameObject);
            }
        }
        #endregion

        #region 个人中心

        public AnnouncementView CreateAnnouncementView()
        {
            AnnouncementView view = load<AnnouncementView>("Prefabs/System/AnnouncementView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("AnnouncementView Create Failed ...");
        }

        public AboutView CreateAboutView()
        {
            AboutView view = load<AboutView>("Prefabs/System/AboutView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("AboutView Create Failed ...");
        }

        public HelpView CreateHelpView()
        {
            HelpView view = load<HelpView>("Prefabs/System/HelpView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("HelpView Create Failed ...");
        }

        #endregion

        #region 生涯数据

        public CareerDataView CreateCareerDataView()
        {
            CareerDataView view = load<CareerDataView>("Prefabs/Career/CareerDataView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("CareerDataView Create Failed ...");
        }

        #endregion

        #region 确认框
        public SureView CreateSureView()
        {
            SureView view = load<SureView>("Prefabs/System/SureView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("SureView Create Failed ...");
        }
        #endregion

        #region email

        public EmailView CreateEmailView()
        {
            EmailView view = load<EmailView>("Prefabs/Email/EmailView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("EmailView Create Failed ...");
        }

        public EmailDetailView CreateEmailDetailView()
        {
            EmailDetailView view = load<EmailDetailView>("Prefabs/Email/EmailDetailView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("EmailDetailView Create Failed ...");
        }

        #endregion

        void setRectTransform(GameObject go)
        {
            RectTransform t = (RectTransform)go.transform;
            t.SetParent(parent);
            t.localScale = Vector3.one;
            t.anchoredPosition3D = Vector3.zero;
        }

        T load<T>(string url)
        {
            GameObject go = Resources.Load(url) as GameObject;
            if (go)
            {
                GameObject target = Instantiate(go);
                if (target)
                {
                    return target.GetComponent<T>();
                }
            }
            return default(T);
        }

        #region android, ios 交互

        public void GetImgPath(string bytes)
        {
            if (Validate.IsNotEmpty(bytes))
            {
                NotificationCenter.Instance.DispatchNotify(NotificationType.EditAvatar, new NotifyMsg().value("avatar", bytes));
            }
            else
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1011"));
            }
        }

        public void showErrorMessage(string errorMsg)
        {
            string msg = LocalizationManager.Instance.GetText(errorMsg);
            Game.Instance.ShowTips(Validate.IsEmpty(msg) ? errorMsg : msg);
        }

        public void PaySuccess(string payInfo)
        {
            PayInfo pay = JsonUtil<PayInfo>.Deserialize(payInfo);
            NotificationCenter.Instance.DispatchNotify(NotificationType.Paypal, new NotifyMsg().value("payInfo", pay));
        }

        public void PayError(string error)
        {
            switch (error)
            {
                case "Cancel":
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6300"));
                    break;
                default:
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6301"));
                    break;
            }
        }

        #endregion

        // 医用内通知处理
        void onNotify(NotifyMsg msg)
        {
            JsonObject jObj = msg["msg"] as JsonObject;
            if (jObj == null)
                return;
            string op = jObj["op"] as string;
            JsonObject data = jObj["data"] as JsonObject;
            switch (op)
            {
                case "clubApply_agree":
                    {
                        string clubName = data["title"] as string;
                        CreateSureView().ShowTip(string.Format(LocalizationManager.Instance.GetText("5011"), clubName));
                        NotificationCenter.Instance.DispatchNotify(NotificationType.ApplyAgree, new NotifyMsg());
                    }
                    break;
                case "clubApply_reject":
                    {
                        SureView vi = CreateSureView();
                        string clubName = data["title"] as string;
                        vi.ShowTip(string.Format(LocalizationManager.Instance.GetText("5012"), clubName));
                    }
                    break;
                case "clubMember_delete":
                    {
                        string clubName = data["title"] as string;
                        CreateSureView().ShowTip(string.Format(LocalizationManager.Instance.GetText("5005"), clubName));
                        NotificationCenter.Instance.DispatchNotify(NotificationType.Kickout, new NotifyMsg());
                    }
                    break;
            }
        }

        private void OnDestroy()
        {
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.OnMsg, onNotify);
        }
    }
}
