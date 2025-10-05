using Assets.Scripts.TableView;
using Newtonsoft.Json.Linq;
using SimpleJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RT
{
    public class PublicTableView : MonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public Text tvDiamond, tvCoins, tvTitleBlinds, tvTitle, tvClub, tvRoomName, tvMyCoins;
        public Button btnBack, btnDiamondBuy, btnCoinBuy, btnStart, btnSwitch;
        public TableView tableView;
        public ClubSwitchView switchView;
        public LoadMask maskView;
        private Coroutine _enterRoom;

        private string clubName;

        private List<ItemClubData> clubs;

        public GameObject itemPublicTablePrefab;

        MdPublicTable _md;
        Queue<Action> _queueAction;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Game.Instance.SetTips();
            Screen.orientation = ScreenOrientation.Portrait;
            _queueAction = new Queue<Action>();
            maskView.gameObject.SetActive(false);
            btnDiamondBuy.onClick.AddListener(() =>
            {
                goToShop(ShopTab.Diamond);
            });
            btnCoinBuy.onClick.AddListener(() =>
            {
                goToShop(ShopTab.Gold);
            });
            btnBack.onClick.AddListener(() =>
            {
                Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Public;
                PlayerPrefs.SetString("LastScene", "");
                SceneManager.LoadScene("MainScene");
            });
            btnStart.onClick.AddListener(() =>
            {
                FastEnter();
            });
            _md = new MdPublicTable();
            _md.game = Transfer.Instance[TransferKey.Game] as string;

            tvDiamond.text = Game.Instance.CurPlayer.Diamond.ToString();
            tvCoins.text = Game.Instance.CurPlayer.Gold.ToString();
            if (GameType.IsDz(_md.game))
            {
                tvTitleBlinds.text = LocalizationManager.Instance.GetText("7001");
                tvTitle.text = LocalizationManager.Instance.GetText("7000");
            }
            else
            {
                tvTitleBlinds.text = LocalizationManager.Instance.GetText("8001");
                tvTitle.text = LocalizationManager.Instance.GetText("8000");
            }

            if (PlayerPrefs.HasKey("lastClubInfo"))
            {
                string clubInfo = PlayerPrefs.GetString("lastClubInfo");
                JObject jObj = JObject.Parse(clubInfo);
                clubName = jObj.Value<string>("clubName");
                tvClub.text = string.Format(LocalizationManager.Instance.GetText("4006"), clubName);
                _md.clubId = jObj.Value<long>("clubId");
                tvRoomName.text = LocalizationManager.Instance.GetText("4008");
                if (_md.clubId == -1)
                {
                    btnStart.gameObject.SetActive(true);
                }
                else
                {
                    btnStart.gameObject.SetActive(false);
                }
            }
            else
            {
                tvClub.text = string.Format(LocalizationManager.Instance.GetText("4006"), LocalizationManager.Instance.GetText("4007"));
                tvRoomName.text = LocalizationManager.Instance.GetText("4001");
                btnStart.gameObject.SetActive(true);
            }

            findClub();

            switchView.Hide();
            btnSwitch.onClick.AddListener(() =>
            {
                switchView.Show();
                switchView.InitView(clubs);
            });

            tvClub.gameObject.SetActive(false);
            switchView.OnClubEnterEvent = enterClub;
        }

        private void Start()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemPublicTablePrefab, "ItemPublicTableCRI");
            findList(true);
        }

        void findList(bool first)
        {
            _md.FindList((rsp) =>
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

        void goToShop(ShopTab tab)
        {
            Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Shop;
            Transfer.Instance[TransferKey.ShopTab] = tab;
            Transfer.Instance.Remove(TransferKey.Game);
            SceneManager.LoadScene("MainScene");
        }

        #region tableView

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
            ItemPublicTableCell cell = tableView.ReusableCellForRow("ItemPublicTableCRI", row) as ItemPublicTableCell;
            cell.data = _md[row];
            cell.name = "cell_" + row;
            return cell;
        }

        public void TableViewDidHighlightCellForRow(TableView tableView, int row)
        {

        }

        public void TableViewDidSelectCellForRow(TableView tableView, int row)
        {
            ItemTableData data = _md[row];
            showMask(true);
            _enterRoom = StartCoroutine(hideMask());
            if (data.isPin)
            {
                onPin(data.roomId);
            }
            // _md.EnterRoom(data.roomId, (result) =>
            // {
            //     _queueAction.Enqueue(() =>
            //     {
            //         canEnter(result);
            //     });
            // });
        }

        void onPin(long roomId)
        {
            KeyBoardView vi = UIClubSpawn.Instance.CreateKeyBoardView();
            vi.OnKeyBoardEvent = (pin) =>
            {
                vi.HideAndDestory();
                onEnterTable(roomId, pin);
            };
        }

        void onEnterTable(long roomId, string ping)
        {
            UIClubSpawn.Instance.CreateLoadMask();
            _enterRoom = StartCoroutine(hideMask());
            _md.EnterRoom(roomId, ping,(result) =>
            {
                _queueAction.Enqueue(() =>
                {
                    canEnter(result);
                });
            });
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

        #endregion

        #region 進入桌子

        // 快速加入
        void FastEnter()
        {
            CreateTexasView view = UIClubSpawn.Instance.CreateTexasView();
            view.OnCreateTabbleEvent = () =>
            {
                view.HideAndDestory();
                findList(false);
            };
            // JsonObject param = new JsonObject();
            // showMask(true);
            // _enterRoom = StartCoroutine(hideMask());
            // if (GameType.IsDz(_md.game))
            // {
            //     TexasApi.FastEnterPublicRoom(param,(result) => {
            //         _queueAction.Enqueue(() =>
            //         {
            //             canEnter(result);
            //         });
            //     });
            // }
            // else
            // {
            //     BullApi.FastEnterPublicRoom(param, (result) => {
            //         _queueAction.Enqueue(() =>
            //         {
            //             canEnter(result);
            //         });
            //     });
            // }
        }

        // 是否可以加入
        void canEnter(JsonObject json)
        {
            showMask(false);
            if (_enterRoom != null)
            {
                StopCoroutine(_enterRoom);
            }
            int code = int.Parse(json["code"].ToString());
            if (code == 200)
            {
                JsonObject roomInfo = json["roomInfo"] as JsonObject;
                if (roomInfo != null)
                {
                    roomInfo.Add("clubChips", Game.Instance.CurPlayer.Gold);
                    Transfer.Instance[TransferKey.RoomInfo] = roomInfo;
                    PlayerPrefs.SetString("LastScene", "PublicTableScene");
                    if (_md.clubId > 0)
                    {
                        Transfer.Instance[TransferKey.ClubId] = _md.clubId;
                    }
                    Transfer.Instance.Remove(TransferKey.ClubInfo);
                    if (GameType.IsDz(roomInfo["game"].ToString()))
                    {
                        SceneManager.LoadScene("TexasPokerScene");
                    }
                    else if (GameType.IsBull(roomInfo["game"].ToString()))
                    {
                        SceneManager.LoadScene("ThreeBullScene");
                    }
                }
            }
            else
            {
                string errorMsg = LocalizationManager.Instance.GetText(code.ToString());
                Game.Instance.ShowTips(errorMsg);
                if (code == 216) // 房间不存在或已过期，刷新列表
                {
                    findList(true);
                }
            }
        }

        void showMask(bool show)
        {
            maskView.gameObject.SetActive(show);
        }

        IEnumerator hideMask()
        {
            yield return new WaitForSeconds(10);
            showMask(false);
        }

        #endregion

        #region 查找俱樂部

        void findClub()
        {
            ClubApi.FindMyClubs(0, 0, 20, (rsp) =>
            {
                if (rsp.IsOk)
                {
                    clubs = rsp.data;
                    if (clubs == null)
                    {
                        clubs = new List<ItemClubData>();
                    }
                    ItemClubData data = new ItemClubData();
                    data.clubId = -1;
                    data.coin = Game.Instance.CurPlayer.Gold;
                    data.name = LocalizationManager.Instance.GetText("4007");
                    clubs.Insert(0, data);
                    bool hasClub = false;
                    for (int i = 0; i < clubs.Count; i++)
                    {
                        if (_md.clubId == clubs[i].clubId)
                        {
                            tvMyCoins.text = clubs[i].coin.ToString();
                            hasClub = true;
                            break;
                        }
                    }
                    // 俱乐部不存在
                    if (!hasClub)
                    {
                        _md.clubId = -1;
                        tvClub.text = string.Format(LocalizationManager.Instance.GetText("4006"), LocalizationManager.Instance.GetText("4007"));
                        tvRoomName.text = LocalizationManager.Instance.GetText("4001");
                        PlayerPrefs.DeleteKey("lastClubInfo");
                        btnStart.gameObject.SetActive(true);
                        tvMyCoins.text = Game.Instance.CurPlayer.Gold.ToString();
                    }
                    tvClub.gameObject.SetActive(true);
                }
            }, true);
        }

        void enterClub(long clubId, string clubName, long coins)
        {
            switchView.Hide();
            if (clubId == _md.clubId)
            {
                return;
            }
            JsonObject jObj = new JsonObject();
            jObj.Add("clubId", clubId);
            jObj.Add("clubName", clubName);
            PlayerPrefs.SetString("lastClubInfo", jObj.ToString());
            if (clubId == -1)
            {
                tvRoomName.text = LocalizationManager.Instance.GetText("4008");
                btnStart.gameObject.SetActive(true);
            }
            else
            {
                tvRoomName.text = LocalizationManager.Instance.GetText("4001");
                btnStart.gameObject.SetActive(false);
            }
            tvMyCoins.text = coins.ToString();
            _md.clubId = clubId;
            tvClub.text = string.Format(LocalizationManager.Instance.GetText("4006"), clubName);
            _md.Clear();
            tableView.ReloadData();
            findList(true);
        }

        #endregion

        private void Update()
        {
            if (_queueAction.Count > 0)
            {
                _queueAction.Dequeue()();
            }
        }
    }
}
