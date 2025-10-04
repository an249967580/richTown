using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public enum Auth : uint
    {
        Join = 1,       // 批准,踢出俱乐部
        Counter = 2,    // 充值，提现
        Table = 3,      // 管理桌子
        Exchange = 4,   // 购买钻石、兑换
        Edit = 5        // 编辑信息
    }

    public delegate void SetupProxyEvent(List<uint> auths);
    public delegate void CancelProxy();

    /// <summary>
    /// 权限设置
    /// </summary>
    public class AuthorityView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnSure, btnCancel;
        public Toggle tgJoinAuth, tgCounterAuth, tgTableAuth, tgExchangeAuth, tgEditAuth;

        private HashSet<uint> _authSet;
        private int _uid;

        public SetupProxyEvent OnSetupProxyEvent;
        public CancelProxy OnCancelProxy;

        private void Awake()
        {
            _authSet = new HashSet<uint>();
            
            btnClose.onClick.AddListener(HideAndDestory);
            btnCancel.onClick.AddListener(HideAndDestory);
            btnSure.onClick.AddListener(onSaveAuth);
            tgJoinAuth.onValueChanged.AddListener((isOn)=>
            {
                selectAuth(Auth.Join, isOn);
            });
            tgExchangeAuth.onValueChanged.AddListener((isOn) =>
            {
                selectAuth(Auth.Exchange, isOn);
            });
            tgTableAuth.onValueChanged.AddListener((isOn) =>
            {
                selectAuth(Auth.Table, isOn);
            });
            tgCounterAuth.onValueChanged.AddListener((isOn) =>
            {
                selectAuth(Auth.Counter, isOn);
            });
            tgEditAuth.onValueChanged.AddListener((isOn) =>
            {
                selectAuth(Auth.Edit, isOn);
            });
        }

        // 初始化数据
        public void InitView(int uid)
        {
            _uid = uid;
            _authSet.Add((uint)Auth.Join);
            _authSet.Add((uint)Auth.Edit);
            _authSet.Add((uint)Auth.Exchange);
            _authSet.Add((uint)Auth.Table);
            _authSet.Add((uint)Auth.Counter);
        }

        // 保存权限
        void onSaveAuth()
        {
            if (_authSet.Count <= 0)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5514"));
                return;
            }
            else
            {
                List<uint> auths = new List<uint>(_authSet);
                if(OnSetupProxyEvent != null)
                {
                    OnSetupProxyEvent(auths);
                }
            }
        }

        // 权限勾选
        void selectAuth(Auth auth, bool isOn)
        {
            if(isOn)
            {
                if(!_authSet.Contains((uint)auth))
                {
                    _authSet.Add((uint)auth);
                }
            }
            else
            {
                _authSet.Remove((uint)auth);
            }
        }

        public override void HideAndDestory()
        {
            base.HideAndDestory();
            if(OnCancelProxy != null)
            {
                OnCancelProxy();
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
