using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{

    public delegate void AgreeEvent();

    /// <summary>
    /// 申请通知
    /// </summary>
    public class ApplyNotifyView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate, IPointerClickHandler
    {
        public Button btnClose;
        public TableView tableView;
        public GameObject goNoNotice;
        public AgreeEvent OnAgreeEvent;

        public GameObject itemApplyPrefab;

        MdApplyList _md;

        private void Awake()
        {
            _md = new MdApplyList();
            btnClose.onClick.AddListener(HideAndDestory);
        }

        private void Start()
        {
            tableView.DataSource = this;
            tableView.Delegate = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemApplyPrefab, "ItemApplyCRI");
            findList(true);
        }

        void findList(bool first)
        {
            if (first)
            {
                _md.Clear();
            }
            _md.FindList((result) =>
            {
                if (result.IsOk)
                {
                    if(first)
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
            ItemApplyCell cell = tableView.ReusableCellForRow("ItemApplyCRI", row) as ItemApplyCell;
            cell.name = "cell_" + row;
            cell.data = _md[row];
            cell.OnOpClickEvent = onOpClickEvent;
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

        public void TableViewDidHighlightCellForRow(TableView tableView, int row)
        {
            
        }

        public void TableViewDidSelectCellForRow(TableView tableView, int row)
        {
            
        }

        public void TableViewDidScrollToStart(TableView tableView)
        {
            
        }

        public void TableViewDidScrollToEnd(TableView tableView)
        {
            if (_md.HasMore)
            {
                findList(false);
            }
        }


        void onOpClickEvent(ApplyOp op, ItemData data)
        {
            ItemApplyData dt = data as ItemApplyData;
            switch (op)
            {
                case ApplyOp.Agree:
                    agree(dt.applyId);
                    break;
                case ApplyOp.Reject:
                    reject(dt.applyId);
                    break;
            }
        }

        // 同意
        void agree(long id)
        {
            _md.Agree(id, (result) =>
            {
                if (result.IsOk)
                {
                    _md.Remove(id);
                    tableView.ReloadData();
                    if (_md.Count <= 0)
                    {
                        goNoNotice.SetActive(true);
                    }
                    if (OnAgreeEvent != null)
                    {
                        OnAgreeEvent();
                    }
                }
                else
                {
                    Game.Instance.ShowTips(result.errorMsg);
                }
            });
        }

        // 拒绝
        void reject(long id)
        {
            _md.Reject(id, (result)=>
            {
                if (result.IsOk)
                {
                    _md.Remove(id);
                    tableView.ReloadData();
                    if (_md.Count <= 0)
                    {
                        goNoNotice.SetActive(true);
                    }
                }
                else
                {
                    Game.Instance.ShowTips(result.errorMsg);
                }
            });
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
