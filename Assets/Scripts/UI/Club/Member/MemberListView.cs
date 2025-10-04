using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{

    public delegate void MemberRemoveEvent();

    /// <summary>
    /// 会员列表
    /// </summary>
    public class MemberListView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate, IPointerClickHandler
    {
        public Button btnClose;
        public Text tvMemberNum, tvTitleServiceFee, tvTitleProfit;
        public InputField ipSearch;
        public Button btnClear;

        public TableView tableView;
        public GameObject itemMemberCell;

        public MemberRemoveEvent OnMemberRemoveEvent;

        MdMemberList _md;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnClear.gameObject.SetActive(false);
            if (ClubMainView.Instance.IsNormal)
            {
                tvTitleServiceFee.gameObject.SetActive(false);
                tvTitleProfit.gameObject.SetActive(false);
            }
            else
            {
                tvTitleServiceFee.gameObject.SetActive(true);
                tvTitleProfit.gameObject.SetActive(true);
            }
            _md = new MdMemberList();
            btnClear.onClick.AddListener(() =>
            {
                ipSearch.text = string.Empty;
                _md.key = string.Empty;
                _md.Clear();
                findList(true);
            });
            ipSearch.onEndEdit.AddListener((text) =>
            {
                _md.key = text;
                _md.Clear();
                findList(true);
            });
            ipSearch.onValueChanged.AddListener((text) =>
            {
                if (!Validate.IsEmpty(text))
                {
                    btnClear.gameObject.SetActive(true);
                }
                else
                {
                    btnClear.gameObject.SetActive(false);
                }
            });
            tvMemberNum.text = string.Format(LocalizationManager.Instance.GetText("5501"), ClubMainView.Instance.MemberCount);
        }

        private void Start()
        {
            tableView.RegisterPrefabForCellReuseIdentifier(itemMemberCell, "ItemMemberCRI");
            tableView.DataSource = this;
            tableView.Delegate = this;
            findList(true);
        }

        void findList(bool isFirst)
        {
            _md.FindList((result) =>
            {
                if (result.IsOk)
                {
                    if (isFirst)
                    {
                        _md.DataItems = result.data;
                    }
                    else
                    {
                        _md.LoadMore(result.data);
                    }
                    tableView.ReloadData();
                }
                else
                {
                    Game.Instance.ShowTips(result.errorMsg);
                }
            });
        }

        public int NumberOfRowsInTableView(TableView tableView)
        {
            return _md.Count;
        }

        public float SizeForRowInTableView(TableView tableView, int row)
        {
            return 120;
        }

        public TableViewCell CellForRowInTableView(TableView tableView, int row)
        {
            ItemMemberCell cell = tableView.ReusableCellForRow("ItemMemberCRI", row) as ItemMemberCell;
            cell.name = "cell_" + row;
            cell.data = _md[row];
            if (Validate.IsNotEmpty(_md[row].avatar))
            {
                if (gameObject.activeSelf)
                {
                    StartCoroutine(LoadImageUtil.LoadImage(_md[row].avatar, (sprite) =>
                    {
                        cell.imgAvatar.sprite = sprite;
                    }));
                }
            }
            else
            {
                cell.imgAvatar.sprite = Resources.Load<Sprite>("Textures/Common/def_avatar_large");
            }
            return cell;
        }

        void ITableViewDelegate.TableViewDidHighlightCellForRow(TableView tableView, int row)
        {
            
        }

        void ITableViewDelegate.TableViewDidSelectCellForRow(TableView tableView, int row)
        {
            memberInfo(_md[row]);
        }

        void ITableViewDelegate.TableViewDidScrollToStart(TableView tableView)
        {
            
        }

        void ITableViewDelegate.TableViewDidScrollToEnd(TableView tableView)
        {
            if (_md.HasMore)
            {
                findList(false);
            }
        }


        void memberInfo(ItemMemberData data)
        {
            if (ClubMainView.Instance.IsNormal)
            {
                return;
            }
            _md.GetMemberInfo(data.uid, (ret) =>
            {
                if (ret.IsOk && ret.data != null)
                {
                    MemberInfoView infoView = UIClubSpawn.Instance.CreateMemberInfoView();
                    infoView.InitView(ret.data);
                    infoView.OnKickoutEvent = onKickoutMember;
                    infoView.OnSetProxyEvent = onSetProxyMember;
                }
                else
                {
                    Game.Instance.ShowTips(ret.errorMsg);
                }
            });
        }

        // 移除成员
        void onKickoutMember(long uid)
        {
            _md.Remove(uid);
            tableView.ReloadData();
            if (OnMemberRemoveEvent != null)
            {
                OnMemberRemoveEvent();
            }
        }

        void onSetProxyMember(long uid, bool isProxy)
        {
            _md.UpdateRole(uid, isProxy);
            tableView.ReloadData();
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
