using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class CareerDataView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public Button btnClose;
        public Text tvHamdNum, tvRecord, tvTitle;
        public TableView tableView;
        public GameObject itemCareerDataPrefab;

        MdCareerData _md;
        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
        }
        private void Start()
        {
            _md = new MdCareerData();
            _md.game = Transfer.Instance[TransferKey.Game] as string;
            string titleKey = _md.game == GameType.dz ? "7000" : "8000";
            tvTitle.text = LocalizationManager.Instance.GetText(titleKey);
            tableView.Delegate = this;
            tableView.DataSource = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemCareerDataPrefab, "ItemCareerBoardCRI");
            findList(true);
        }

        void findList(bool first)
        {
            _md.FindList((rsp) =>
            {
                if (rsp.data != null)
                {
                    if (first)
                    {
                        tvHamdNum.text = rsp.data.handTotal.ToString();
                        tvRecord.text = rsp.data.roomTotal.ToString();
                        _md.DataItems = rsp.data.list;
                    }
                    else
                    {
                        _md.LoadMore(rsp.data.list);
                    }
                }
                tableView.ReloadData();
            });
        }

        public TableViewCell CellForRowInTableView(TableView tableView, int row)
        {
            ItemCareerBoardCell cell = tableView.ReusableCellForRow("ItemCareerBoardCRI", row) as ItemCareerBoardCell;
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
            return 200;
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
            
        }
    }
}
