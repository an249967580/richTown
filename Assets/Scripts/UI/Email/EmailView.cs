using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public delegate void UpdateDiamondEvent();

    public class EmailView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public Button btnClose;
        public TableView tableView;
        public GameObject itemEmailPrefab, goNoneEmail;

        public UpdateDiamondEvent OnUpdateDiamondEvent;

        MdMail _md;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            _md = new MdMail();
        }

        private void Start()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemEmailPrefab, "ItemEmailCRI");
            goNoneEmail.SetActive(false);
            findList(true);
        }

        void findList(bool first)
        {
            _md.FindList((rsp) =>
            {
                if(first)
                {
                    _md.DataItems = rsp.data;
                    if(_md.IsEmpty)
                    {
                        goNoneEmail.SetActive(true);
                    }
                }
                else
                {
                    _md.LoadMore(rsp.data);
                }
                tableView.ReloadData();
            }, true);
        }

        public TableViewCell CellForRowInTableView(TableView tableView, int row)
        {
            ItemEmailCell cell = tableView.ReusableCellForRow("ItemEmailCRI", row) as ItemEmailCell;
            cell.data = _md[row];
            cell.name = "cell_" + row;
            return cell;
        }

        public int NumberOfRowsInTableView(TableView tableView)
        {
            return _md.Count;
        }

        public float SizeForRowInTableView(TableView tableView, int row)
        {
            return 120;
        }

        public void TableViewDidHighlightCellForRow(TableView tableView, int row)
        {
            
        }

        public void TableViewDidScrollToEnd(TableView tableView)
        {
            if(_md.HasMore)
            {
                findList(false);
            }
        }

        public void TableViewDidScrollToStart(TableView tableView)
        {
        }

        public void TableViewDidSelectCellForRow(TableView tableView, int row)
        {
            ItemEmailData data = _md[row];
            _md.GetDatail(data.id, (rsp) =>
            {
                if(rsp.IsOk)
                {
                    if(rsp.data != null)
                    {
                        _md.Read(data.id);
                        tableView.ReloadData();
                        EmailDetailView view = MainView.Instance.CreateEmailDetailView();
                        view.InitView(rsp.data);
                        view.OnReceiveEvent = (id) =>
                        {
                            _md.Receive(id, (ret) =>
                            {
                                if(ret.IsOk)
                                {
                                    Game.Instance.CurPlayer.Diamond = ret.data;
                                    if(OnUpdateDiamondEvent != null)
                                    {
                                        OnUpdateDiamondEvent();
                                    }
                                    // 更新頁面
                                    view.SetReceive(1);
                                    _md.Receive(id);
                                    tableView.ReloadData();
                                }
                                else
                                {
                                    Game.Instance.ShowTips(ret.errorMsg);
                                }
                            });
                        };
                    }
                }
                else
                {
                    Game.Instance.ShowTips(rsp.errorMsg);
                }
            });
        }
    }
}
