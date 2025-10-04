using System;
using UnityEngine;

namespace RT
{
    /// <summary>
    /// 俱乐部弹窗生成类
    /// </summary>
    public class UIClubSpawn : MonoBehaviour
    {
        

        public static UIClubSpawn Instance;
        public RectTransform spawnParent;

        private LoadMask _loadMask;

        private void Awake()
        {
            Instance = this;
            Game.Instance.SetTips();
        }

        #region  俱乐部页面

        /// <summary>
        /// 俱乐部币兑换页面
        /// </summary>
        /// <returns></returns>
        public ExchangeView CreateExchangeView()
        {
            ExchangeView view = load<ExchangeView>("Prefabs/Club/Info/ExchangeView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ExchangeView Create Failed ...");
        }

        /// <summary>
        /// 俱乐部信息编辑
        /// </summary>
        public ClubEditView CreateEditView()
        {
            ClubEditView view = load<ClubEditView>("Prefabs/Club/Main/ClubEditView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ClubEditView Create Failed ...");
        }

        /// <summary>
        /// 牛牛开局
        /// </summary>
        /// <returns></returns>
        public CreateBullView CreateBullView()
        {
            CreateBullView view = load<CreateBullView>("Prefabs/Club/Main/CreateBullView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("CreateBullView Create Failed ...");
        }

        /// <summary>
        /// 德州开局
        /// </summary>
        /// <returns></returns>
        public CreateTexasView CreateTexasView()
        {
            CreateTexasView view = load<CreateTexasView>("Prefabs/Club/Main/CreateTexasView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("CreateTexasView Create Failed ...");
        }

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

        #region 柜台

        /// <summary>
        /// 柜台页面
        /// </summary>
        /// <returns></returns>
        public CounterView CreateCounterView()
        {
            CounterView view = load<CounterView>("Prefabs/Club/Counter/CounterView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("CounterView Create Failed ...");
        }

        public CounterRecordsView CreateRecordsView()
        {
            CounterRecordsView view = load<CounterRecordsView>("Prefabs/Club/Counter/CounterRecordsView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("CounterView Create Failed ...");
        }

        /// <summary>
        /// 收发弹窗
        /// </summary>
        /// <returns></returns>
        public SendOrRecycleView CreateSendorRecycleView()
        {
            SendOrRecycleView view = load<SendOrRecycleView>("Prefabs/Club/Counter/SendOrRecycleView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("SendOrRecycleView Create Failed ...");
        }

        #endregion

        #region 俱乐部信息页面

        /// <summary>
        /// 俱乐部信息弹窗
        /// </summary>
        /// <returns></returns>
        public ClubProxyView CreateClubProxyView()
        {
            ClubProxyView view = load<ClubProxyView>("Prefabs/Club/Info/ClubProxyView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ClubProxyView Create Failed ...");
        }

        /// <summary>
        /// 俱乐部信息弹窗
        /// </summary>
        /// <returns></returns>
        public ClubNormalView CreateClubNormalView()
        {
            ClubNormalView view = load<ClubNormalView>("Prefabs/Club/Info/ClubNormalView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ClubNormalView Create Failed ...");
        }

        /// <summary>
        /// 等级卡页面
        /// </summary>
        /// <returns></returns>
        public LevelCardsView CreateLevelCardsView()
        {
            LevelCardsView view = load<LevelCardsView>("Prefabs/Club/LevelCards/LevelCardsView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("LevelCardsView Create Failed ...");
        }

        public LevelCardBuyView CreateLevelCardBuyView()
        {
            LevelCardBuyView view = load<LevelCardBuyView>("Prefabs/Club/LevelCards/LevelCardBuyView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("LevelCardBuyView Create Failed ...");
        }

        /// <summary>
        /// 会员列表页
        /// </summary>
        /// <returns></returns>
        public MemberListView CreateMemberListView()
        {
            MemberListView view = load<MemberListView>("Prefabs/Club/Member/MemberListView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("LevelCardsView Create Failed ...");
        }

        /// <summary>
        /// 会员信息页
        /// </summary>
        /// <returns></returns>
        public MemberInfoView CreateMemberInfoView()
        {
            MemberInfoView view = load<MemberInfoView>("Prefabs/Club/Member/MemberInfoView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("MemberInfoView Create Failed ...");
        }

        /// <summary>
        /// 会员备注编辑
        /// </summary>
        /// <returns></returns>
        public MemberNoteView CreateMemberNoteView()
        {
            MemberNoteView view = load<MemberNoteView>("Prefabs/Club/Member/MemberNoteView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("MemberNoteView Create Failed ...");
        }

        /// <summary>
        /// 代理权限设置页
        /// </summary>
        /// <returns></returns>
        public AuthorityView CreateAuthorityView()
        {
            AuthorityView view = load<AuthorityView>("Prefabs/Club/Member/AuthorityView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("AuthorityView Create Failed ...");
        }

        /// <summary>
        /// 俱乐部数据
        /// </summary>
        /// <returns></returns>
        public ClubDataView CreateClubDataView()
        {
            ClubDataView view = load<ClubDataView>("Prefabs/Club/Data/ClubDataView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ClubDataView Create Failed ...");
        }

        /// <summary>
        /// 申请通知页
        /// </summary>
        /// <returns></returns>
        public ApplyNotifyView CreateApplyNotifyView()
        {
            ApplyNotifyView view = load<ApplyNotifyView>("Prefabs/Club/Apply/ApplyNotifyView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ApplyNotifyView Create Failed ...");
        }

        /// <summary>
        /// 每局输赢
        /// </summary>
        /// <returns></returns>
        public BoardView CreateBoardView()
        {
            BoardView view = load<BoardView>("Prefabs/Club/Data/BoardView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("BoardView Create Failed ...");
        }

        /// <summary>
        /// 日期选择控件
        /// </summary>
        /// <returns></returns>
        public DateSelectView CreateDateView()
        {
            DateSelectView view = load<DateSelectView>("Prefabs/Club/Data/DateSelectView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("DateSelectView Create Failed ...");
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

        #region 键盘
        public KeyBoardView CreateKeyBoardView()
        {
            KeyBoardView vi = load<KeyBoardView>("Prefabs/Club/KeyBoardView");
            if (vi)
            {
                setRectTransform(vi.gameObject);
                return vi;
            }
            throw new Exception("KeyBoardView Create Failed ...");
        }
        #endregion

        #region 确认框

        public ConfirmView CreateConfirmView()
        {
            ConfirmView vi = load<ConfirmView>("Prefabs/Club/ConfirmView");
            if (vi)
            {
                setRectTransform(vi.gameObject);
                return vi;
            }
            throw new Exception("ConfirmView Create Failed ...");
        }

        public SureView CreateSureView()
        {
            SureView vi = load<SureView>("Prefabs/Systen/SureView");
            if (vi)
            {
                setRectTransform(vi.gameObject);
                return vi;
            }
            throw new Exception("SureView Create Failed ...");
        }

        #endregion

        #region 游戏选择框

        public GameSelectView CreateGameSelectView()
        {
            GameSelectView vi = load<GameSelectView>("Prefabs/Club/Data/GameSelectView");
            if (vi)
            {
                setRectTransform(vi.gameObject);
                return vi;
            }
            throw new Exception("GameSelectView Create Failed ...");
        }
        #endregion

        void setRectTransform(GameObject go)
        {
            RectTransform t = (RectTransform)go.transform;
            t.SetParent(spawnParent);
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

        public void GetImgPath(string bytes)
        {
            if (Validate.IsNotEmpty(bytes))
            {
                NotificationCenter.Instance.DispatchNotify(NotificationType.EditClubAvatar, new NotifyMsg().value("avatar", bytes));
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

    }

}
