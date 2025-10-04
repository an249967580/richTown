using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{

    public delegate void KickoutEvent(long uid);
    public delegate void SetProxyEvent(long uid, bool isProxy);

    /// <summary>
    /// 会员信息
    /// </summary>
    public class MemberInfoView : HideMonoBehaviour, IPointerClickHandler
    {
        public Button btnClose, btnDel, btnEdit, btnOn, btnOff;
        public CircleImage imgAvatar;
        public Text tvName, tvId, tvNote, tvHandNum, tvServiceFee, tvDownCode, tvBuyIn, tvProfit;
        public GameObject goAdmin;

        public KickoutEvent OnKickoutEvent;
        public SetProxyEvent OnSetProxyEvent;

        private MdMemberInfo _md;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnDel.onClick.AddListener(onDelMember);

            _md = new MdMemberInfo();

            btnEdit.onClick.AddListener(onEditNote);
            btnOn.onClick.AddListener(unProxy);
            btnOff.onClick.AddListener(onProxy);
        }

        #region 初始化

        // 初始化UI
        public void InitView(MemberInfo info)
        {
            _md.info = info;
            tvName.text = info.nickname;
            tvId.text = "ID：" + info.uid;
            tvNote.text = info.note;
            tvHandNum.text = info.handNum.ToString();
            tvServiceFee.text = info.serviceFee.ToString();
            tvDownCode.text = info.returnClips.ToString();
            tvBuyIn.text = info.buyIn.ToString();
            if (info.profit > 0)
            {
                tvProfit.text = "+" + info.profit.ToString();
                tvProfit.color = Color.green;
            }
            else if (info.profit < 0)
            {
                tvProfit.text = info.profit.ToString();
                tvProfit.color = Color.red;
            }
            else
            {
                tvProfit.color = Color.white;
                tvProfit.text = info.profit.ToString();
            }

            if (Validate.IsNotEmpty(info.avatar))
            {
                StartCoroutine(LoadImageUtil.LoadImage(info.avatar, (sprite)=>
                {
                    imgAvatar.sprite = sprite;
                }));
            }

            btnEdit.gameObject.SetActive(ClubMainView.Instance.HasRight(Auth.Edit));
            btnDel.interactable = ClubMainView.Instance.HasRight(Auth.Join);
            goAdmin.gameObject.SetActive(false);
            if (ClubMainView.Instance.IsCreator)  // 我是创建者
            {
                goAdmin.gameObject.SetActive(true);
                if (info.isProxy)    // 当是代理
                {
                    btnOn.gameObject.SetActive(true);
                    btnOff.gameObject.SetActive(false);
                }
                else
                {
                    btnOn.gameObject.SetActive(false);
                    btnOff.gameObject.SetActive(true);
                }

                if (info.isCreator)  // 是创建者
                {
                    btnDel.interactable = false;
                    goAdmin.gameObject.SetActive(false);
                }
            }
            else if (ClubMainView.Instance.IsProxy) // 我是创建者
            {
                if(info.uid == Game.Instance.CurPlayer.Uid)
                {
                    btnDel.interactable = false;
                }
                if(info.isCreator)
                {
                    btnDel.interactable = false;
                }
            }
        }

        #endregion

        // 剔除
        void onDelMember()
        {
            ConfirmView vi = UIClubSpawn.Instance.CreateConfirmView();
            vi.ShowTip(LocalizationManager.Instance.GetText("5019"), true);
            vi.OnConfirmEvent = (confirm) =>
            {
                _md.Kickout((result) =>
                {
                    if (result.IsOk)
                    {
                        if (OnKickoutEvent != null)
                        {
                            // 更新列表
                            OnKickoutEvent(_md.uid);
                        }
                        HideAndDestory();
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            };
        }

        // 编辑备注
        void onEditNote()
        {
            if (_md == null)
            {
                return;
            }
            MemberNoteView noteView = UIClubSpawn.Instance.CreateMemberNoteView();
            noteView.InitView(_md.info.note);
            noteView.OnUpdateNoteEvent = (note)=>
            {
                _md.SetNote(note, (result)=>
                 {
                     if (result.IsOk)
                     {
                         tvNote.text = note;
                         _md.info.note = note;
                         noteView.HideAndDestory();
                     }
                     else
                     {
                         Game.Instance.ShowTips(result.errorMsg);
                     }
                 });
            };
        }

        // 设置
        void onProxy()
        {
            // 设置，弹出权限设置页面
            AuthorityView authorityView = UIClubSpawn.Instance.CreateAuthorityView();
            authorityView.InitView(_md.uid);
            authorityView.OnSetupProxyEvent = (auths) =>
            {
                _md.Proxy(auths, (result) =>
                {
                    if (result.IsOk)
                    {
                        btnOff.gameObject.SetActive(false);
                        btnOn.gameObject.SetActive(true);
                        OnSetProxyEvent(_md.uid, true);
                        authorityView.HideAndDestory();
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            };
        }

        //取消副代理
        void unProxy()
        {
            _md.UnProxy((result)=>
            {
                if (result.IsOk)
                {
                    btnOn.gameObject.SetActive(false);
                    btnOff.gameObject.SetActive(true);
                    OnSetProxyEvent(_md.uid, false);
                }
                else
                {
                    Game.Instance.ShowTips(result.errorMsg);
                }
            });
        }

        void onProxyEvent(int uid, bool isProxy)
        {
            if (OnSetProxyEvent != null)
            {
                OnSetProxyEvent(uid, isProxy);
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
