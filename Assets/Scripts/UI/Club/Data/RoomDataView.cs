using Assets.Scripts.TableView;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    // 牌局列表
    public class RoomDataView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public Button btnPrev, btnNext, btnDateSelect, btnYesterday, btnGame;
        public Text tvDate, tvSelectGame, tvStartNum, tvServiceFee, tvHandNum;

        public TableView tableView;
        public GameObject itemRoomPrefab;

        MdRoomData _md;

        private DateTime _now;

        private void Awake()
        {
            _md = new MdRoomData();
            _now = DateTime.Now;
            tvDate.text = string.Format("{0} - {1}", _now.ToString("yyyy.MM.dd"), _now.ToString("yyyy.MM.dd"));
            btnNext.interactable = false;
            btnDateSelect.onClick.AddListener(()=>
            {
                DateSelectView dateView = UIClubSpawn.Instance.CreateDateView();
                dateView.OnDateSelectEvent = (min, max)=>
                {
                    tvDate.text = string.Format("{0} - {1}", min.ToString("yyyy.MM.dd"), max.ToString("yyyy.MM.dd"));
                    _md.startTime = TimeUtil.DateToSeconds(min);
                    _md.endTime = TimeUtil.DateToSeconds(max) + 24 * 60 * 60 - 1;
                    btnPrev.interactable = (min == max);
                    btnNext.interactable = (min == max);
                    if (min == max)
                    {
                        if (min.Year == _now.Year && min.Month == _now.Month && min.Day == _now.Day)
                        {
                            btnNext.interactable = false;
                        }
                    }
                    if (min == max)
                    {
                        _md.SetDate(min);
                    }
                    findList(true);
                };
            });
            btnYesterday.onClick.AddListener(()=> {
                DateTime yesterday = _now.AddDays(-1);
                yesterday = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0);
                tvDate.text = string.Format("{0} - {1}", yesterday.ToString("yyyy.MM.dd"), yesterday.ToString("yyyy.MM.dd"));
                _md.SetDate(yesterday);
                btnPrev.interactable = true;
                btnNext.interactable = true;
                findList(true);
            });
            btnPrev.onClick.AddListener(()=> {
                DateTime date = _md.now.AddDays(-1);
                tvDate.text = string.Format("{0} - {1}", date.ToString("yyyy.MM.dd"), date.ToString("yyyy.MM.dd"));
                _md.SetDate(date);
                btnPrev.interactable = true;
                btnNext.interactable = true;
                findList(true);
            });
            btnNext.onClick.AddListener(()=> {
                DateTime date = _md.now.AddDays(1);
                if (date.Year == _now.Year && date.Month == _now.Month && date.Day == _now.Day)
                {
                    btnNext.interactable = false;
                }
                tvDate.text = string.Format("{0} - {1}", date.ToString("yyyy.MM.dd"), date.ToString("yyyy.MM.dd"));
                _md.SetDate(date);
                findList(true);
            });
            btnGame.onClick.AddListener(() =>
            {
                UIClubSpawn.Instance.CreateGameSelectView().OnGameSelectEvent = (game) =>
                {
                    _md.game = game;
                    string key = GameType.IsDz(game) ? "7000" : "8000";
                    tvSelectGame.text = LocalizationManager.Instance.GetText(key);
                    findList(true);
                };
            });
        }

        private void Start()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemRoomPrefab, "ItemRoomCRI");

            findList(true);
        }

        void findList(bool first)
        {
            if(first)
            {
                _md.Clear();
            }
            _md.findList((rsp) =>
            {
                if(rsp.IsOk)
                {
                    RoomData data = rsp.data;
                    if (data != null)
                    {
                        if (first)
                        {
                            tvStartNum.text = data.startTotal.ToString();
                            tvServiceFee.text = data.serviceChargeTotal.ToString();
                            tvHandNum.text = data.handTotal.ToString();
                            _md.DataItems = data.list;
                            for (int i = 0; i < _md.Count; i++)
                            {
                                _md[i].game = _md.game;
                            }
                        }
                        else
                        {
                            if (Validate.IsNotEmpty(data.list))
                            {
                                foreach (ItemRoomData d in data.list)
                                {
                                    d.game = _md.game;
                                }
                            }
                            _md.LoadMore(data.list);
                        }
                        tableView.ReloadData();
                    }
                }
                else
                {
                    Game.Instance.ShowTips(rsp.errorMsg);
                }
            });
        }

        public int NumberOfRowsInTableView(TableView tableView)
        {
            return _md.Count;
        }

        public float SizeForRowInTableView(TableView tableView, int row)
        {
            return 100;
        }

        public TableViewCell CellForRowInTableView(TableView tableView, int row)
        {
            ItemRoomCell cell = tableView.ReusableCellForRow("ItemRoomCRI", row) as ItemRoomCell;
            cell.name = "cell_" + row;
            cell.data = _md[row];
            return cell;
        }

        public void TableViewDidHighlightCellForRow(TableView tableView, int row)
        {
            
        }

        public void TableViewDidSelectCellForRow(TableView tableView, int row)
        {
            onItemClickListener(_md[row]);
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

        void onItemClickListener(ItemRoomData data)
        {
            _md.GetBorrdData(data.roomId, (rsp) =>
            {
                if (rsp.IsOk)
                {
                    if (rsp.data != null)
                    {
                        BoardView view = UIClubSpawn.Instance.CreateBoardView();
                        view.InitView(_md.game,data.roomId, rsp.data);
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
