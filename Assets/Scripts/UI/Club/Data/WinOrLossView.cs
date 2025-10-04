using Assets.Scripts.TableView;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 幸运玩家列表
    /// </summary>
    public class WinOrLossView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public Button btnPrev, btnNext, btnDateSelect, btnGame, btnClose;
        public Text tvDate, tvSelectGame;
        public InputField ipKey;

        public TableView tableView;
        public GameObject itemWinOrLossPrefab;

        MdWinOrLoss _md;
        private DateTime _now;

        private void Awake()
        {
            _md = new MdWinOrLoss();
            _now = DateTime.Now;
            tvDate.text = string.Format("{0} - {1}", _now.ToString("yyyy.MM.dd"), _now.ToString("yyyy.MM.dd"));
            btnNext.interactable = false;
            btnDateSelect.onClick.AddListener(() =>
            {
                DateSelectView dateView = UIClubSpawn.Instance.CreateDateView();
                dateView.OnDateSelectEvent = (min, max) =>
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
            btnPrev.onClick.AddListener(() => {
                DateTime date = _md.now.AddDays(-1);
                tvDate.text = string.Format("{0} - {1}", date.ToString("yyyy.MM.dd"), date.ToString("yyyy.MM.dd"));
                _md.SetDate(date);
                btnPrev.interactable = true;
                btnNext.interactable = true;
                findList(true);
            });
            btnNext.onClick.AddListener(() => {
                DateTime date = _md.now.AddDays(1);
                if (date.Year == _now.Year && date.Month == _now.Month && date.Day == _now.Day)
                {
                    btnNext.interactable = false;
                }
                tvDate.text = string.Format("{0} - {1}", date.ToString("yyyy.MM.dd"), date.ToString("yyyy.MM.dd"));
                _md.SetDate(date);
                findList(true);
            });
            ipKey.onValueChanged.AddListener((text) =>
            {
                if (Validate.IsNotEmpty(text))
                {
                    btnClose.gameObject.SetActive(true);
                }
                else
                {
                    btnClose.gameObject.SetActive(false);
                }
            });
            ipKey.onEndEdit.AddListener((text) =>
            {
                findList(true);
            });
            btnClose.onClick.AddListener(() =>
            {
                ipKey.text = string.Empty;
                _md.key = string.Empty;
                findList(true);
            });
            btnGame.onClick.AddListener(() =>
            {
                UIClubSpawn.Instance.CreateGameSelectView().OnGameSelectEvent = (game) =>
                {
                    _md.game = game;
                    string key = (game == GameType.dz ? "7000" : "8000");
                    tvSelectGame.text = LocalizationManager.Instance.GetText(key);
                    findList(true);
                };
            });
        }

        private void Start()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemWinOrLossPrefab, "ItemWinOrLossCRI");
            findList(true);
        }

        void findList(bool first)
        {
            _md.key = ipKey.text.Trim();
            if (first)
            {
                _md.Clear();
            }
            _md.findList((rsp) =>
            {
                if (rsp.IsOk)
                {
                    if (first)
                    {
                        _md.DataItems = rsp.data;
                    }
                    else
                    {
                        _md.LoadMore(rsp.data);
                    }
                    tableView.ReloadData();

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
            return 120;
        }

        public TableViewCell CellForRowInTableView(TableView tableView, int row)
        {
            ItemWinOrLossCell cell = tableView.ReusableCellForRow("ItemWinOrLossCRI", row) as ItemWinOrLossCell;
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
                findList(false);
            }
        }
    }
}
