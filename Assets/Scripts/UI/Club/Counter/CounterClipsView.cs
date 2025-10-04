using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 收发筹码
    /// </summary>
    public class CounterClipsView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public TableView tableView;
        public GameObject itemSendPrefab;

        public Text tvCoins;
        public InputField ipSearch;
        public Button btnSend, btnRecycle;
        public Button btnClear, btnExchange;

        private MdClips _md;

        private void Awake()
        {
            btnClear.gameObject.SetActive(false);
            btnSend.onClick.AddListener(() =>
            {
                _md.IsSend = true;
                showDlg();
            });
            btnRecycle.onClick.AddListener(()=>
            {
                _md.IsSend = false;
                showDlg();
            });
            _md = new MdClips();
            ipSearch.onEndEdit.AddListener((text) =>
            {
                _md.key = text;
                _md.Clear();
                findList(true, true);
            });

            ipSearch.onValueChanged.AddListener((text) =>
            {
                if(Validate.IsEmpty(text))
                {
                    btnClear.gameObject.SetActive(false);
                }
                else
                {
                    btnClear.gameObject.SetActive(true);
                }
            });

            btnClear.onClick.AddListener(()=>
            {
                ipSearch.text = string.Empty;
                _md.key = string.Empty;
                _md.Clear();
                findList(true, true);
            });

            btnExchange.onClick.AddListener(()=>
            {
                ExchangeView vi = UIClubSpawn.Instance.CreateExchangeView();
                vi.OnExchangeEvent = (result) =>
                {
                    ClubMainView.Instance.UpdateCoins(result.clubCoins);
                    Game.Instance.CurPlayer.Diamond = result.diamond;
                    tvCoins.text = result.clubCoins.ToString();
                    vi.HideAndDestory();
                };
            });
            btnExchange.gameObject.SetActive(ClubMainView.Instance.HasRight(Auth.Exchange));
        }

        private void Start()
        {
            tableView.DataSource = this;
            tableView.Delegate = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemSendPrefab, "ItemSendCRI");
            findList(true, true);
        }

        void findList(bool first, bool showMask)
        {
            _md.FindList((result)=>
            {
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
            ItemSendCell cell = tableView.ReusableCellForRow("ItemSendCRI", row) as ItemSendCell;
            cell.name = "cell_" + row;
            cell.data = _md[row];
            cell.OnItemToggleEvent = onItemToggleEvent;
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
                findList(false, true);
            }
        }

        public void InitView(long clubCoin)
        {
            tvCoins.text = clubCoin.ToString();
        }

        void onItemToggleEvent(bool isOn, ItemMemberData data)
        {
            for (int i = 0; i < _md.Count; i++)
            {
                if (_md[i].uid == data.uid)
                {
                    _md[i].isChecked = !_md[i].isChecked;
                }
                else
                {
                    _md[i].isChecked = false;
                }
            }
            tableView.ReloadData();
        }

        ItemMemberData checkedData()
        {
            foreach(ItemMemberData data in _md.DataItems)
            {
                if(data.isChecked)
                {
                    return data;
                }
            }
            return null;
        }
       
        // 显示弹窗
        void showDlg()
        {
            ItemMemberData sel = checkedData();
            if (sel == null)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5306"));
                return;
            }
            SendOrRecycleView vi = UIClubSpawn.Instance.CreateSendorRecycleView();
            vi.InitView(_md.IsSend, sel);
            vi.OnSendOrRecycleEvent = onSendOrRecycleEvent;
        }

        // 网络请求
        void onSendOrRecycleEvent(long uid, long coins, SendOrRecycleView view)
        {
            if(_md.IsSend)
            {
                if(coins > ClubMainView.Instance.ClubCoin)
                {
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5308"));
                    return;
                }

                _md.SendCoins(uid, coins, (result)=>
                {
                    if(result.IsOk)
                    {
                        tvCoins.text = result.data.ToString();
                        UpdateCoins(uid, coins);
                        ClubMainView.Instance.UpdateCoins(result.data);
                        view.HideAndDestory();
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            }
            else
            {
                _md.RecycleCoins(uid, coins, (result)=>
                {
                    if (result.IsOk)
                    {
                        tvCoins.text = result.data.ToString();
                        UpdateCoins(uid, -coins);
                        ClubMainView.Instance.UpdateCoins(result.data);
                        view.HideAndDestory();
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            }
        }

        // 更新r币
        void UpdateCoins(long uid, long coins)
        {
            _md.UpdateCoins(uid, coins);
            tableView.ReloadData();
            if (uid == Game.Instance.CurPlayer.Uid)
            {
                ClubMainView.Instance.UpdateMyCoins(coins);
            }
        }

    }
}

   