using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 收发记录
    /// </summary>
    public class CounterRecordsView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate, IPointerClickHandler
    {
        public TableView tableView;
        public Button btnClose;
        public GameObject itemRecordPrefab;

        private MdRecords _md;

        private void Awake()
        {
            _md = new MdRecords();
            if(btnClose)
            {
                btnClose.onClick.AddListener(HideAndDestory);
            }
        }

        private void Start()
        {
            tableView.DataSource = this;
            tableView.Delegate = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemRecordPrefab, "ItemRecordCRI");
        }

        public override void Show()
        {
            base.Show();
            findList(true, false);
        }

        void findList(bool first, bool showMask)
        {
            if (first)
            {
                _md.Clear();
            }
            _md.FindList((result) => {
                if (result.IsOk)
                {
                    if (first)
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
            }, showMask);
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
            ItemRecordCell cell = tableView.ReusableCellForRow("ItemRecordCRI", row) as ItemRecordCell;
            cell.name = "cell_" + row;
            cell.data = _md[row];
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
            if(_md.HasMore)
            {
                findList(false, true);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (btnClose)
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
}
