using System;
using UnityEngine.UI;
using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RT
{
    public class BoardView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate, IPointerClickHandler
    {
        public Button btnClose;
        public Text tvBlinds, tvRate, tvTime, tvBuyIn, tvServiceFee, tvPlayerNum;
        public TableView tableView;
        public GameObject itemBoardPrefab;
        private bool _isInit = false;

        MdBoardData _md;

        private void Awake()
        {
            _md = new MdBoardData();
            btnClose.onClick.AddListener(HideAndDestory);
        }

        private void Start()
        {
           if(!_isInit)
            {
                initTable();
            }
        }

        public void InitView(string game, long roomId, BoardData data)
        {
            if (!_isInit)
            {
                initTable();
            }
            tvRate.text = data.Rate + "%";
            tvBuyIn.text = data.buyBetTotal.ToString();
            tvServiceFee.text = data.serviceChargeTotal.ToString();
            tvPlayerNum.text = data.userNumTotal.ToString();
            if (GameType.IsBull(game))
            {
                tvBlinds.text = string.Format(LocalizationManager.Instance.GetText("5904"), data.blindBet);
            }
            else
            {
                tvBlinds.text = string.Format(LocalizationManager.Instance.GetText("5903"), data.blindBet / 2, data.blindBet);
            }
            tvTime.text = timeStr(data.roomTime);
            _md.roomId = roomId;
            _md.DataItems = data.list;
            tableView.ReloadData();
        }

        void initTable ()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemBoardPrefab, "ItemBoardCRI");
            _isInit = true;
        }

        string timeStr(int seconds)
        {
            return (seconds * 1.0f / 3600) + "h";
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
            ItemBoardCell cell = tableView.ReusableCellForRow("ItemBoardCRI", row) as ItemBoardCell;
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

        void findList(bool first)
        {
            if (first)
            {
                _md.Clear();
            }
            _md.FindList((rsp) =>
            {
                if(rsp.IsOk)
                {
                    if(rsp.data != null)
                    {
                        if(first)
                        {
                            _md.DataItems = rsp.data.list;
                        }
                        else
                        {
                            _md.LoadMore(rsp.data.list);
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
