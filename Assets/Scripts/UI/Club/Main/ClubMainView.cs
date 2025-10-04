using SimpleJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RT
{

    public enum TableType : uint
    {
        Texas,
        Bull
    }

    /// <summary>
    /// 俱乐部主页
    /// </summary>
    public class ClubMainView : MonoBehaviour
    {
        public GridView gdView;
        public Button btnBack, btnMember, btnCounter, btnClubEdit, btnExchange;
        public Image imgAvatar, imgExpir, imgNotify, imgLoad;
        public Text tvName, tvId, tvClubCoins, tvMyCoins, tvIntro, tvExpir;
        public Slider sLevel;
        public Toggle tgTexas, tgBull;
        public GameObject goClubCoin;

        public static ClubMainView Instance;

        private MdClubMain _md;

        private Queue<Action> _queueAction;

        #region 俱乐部数据相关

        #region 权限相关

        public long ClubId
        {
            get
            {
                return _md.clubId;
            }
        }

        public bool IsCreator
        {
            get
            {
                return _md.IsCreator;
            }
        }

        public bool IsProxy
        {
            get
            {
                return _md.IsProxy;
            }
        }

        public bool IsNormal
        {
            get
            {
                return _md.IsNormal;
            }
        }

        public long ClubCoin
        {
            get
            {
                return _md.detail.clubCoins;
            }
        }

        public long MinClubCoin
        {
            get
            {
                return _md.detail.minCoin;
            }
        }

        public int DiamondRate
        {
            get
            {
                return _md.detail.diamondRate;
            }
        }

        public int MemberCount
        {
            get
            {
                return _md.detail.memberCount;
            }
        }

        public bool ShowLucky
        {
            get
            {
                return _md.detail.showLucky;
            }
        }

        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool HasRight(Auth auth)
        {
            return _md.HasRight(auth);
        }

        public int Level
        {
            get
            {
                return _md.detail.level;
            }
        }

        #endregion

        #region 底注倍率

        public List<long> Blinds
        {
            get
            {
                return _md.Blinds;
            }
        }
        public List<long> Mutiple
        {
            get
            {
                return _md.Mutiple;
            }
        }

        public List<int> CostScale
        {
            get
            {
                return _md.CostScale;
            }
        }

        public List<long> RoomTime
        {
            get
            {
                return _md.RoomTime;
            }
        }

        #endregion

        public void UpdateCheckIp(int checkIp)
        {
            _md.detail.checkip = checkIp;
        }

        #endregion

        #region 初始化

        private void Awake()
        {
            Instance = this;
            Screen.orientation = ScreenOrientation.Portrait;
            _md = new MdClubMain();
            _queueAction = new Queue<Action>();
            btnBack.onClick.AddListener(()=>
            {
                string lastScene = PlayerPrefs.GetString("LastScene");
                if(Validate.IsNotEmpty(lastScene) && lastScene == "PublicTableScene")
                {
                    SceneManager.LoadScene("PublicTableScene");
                    return;
                }
                Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Club;
                Transfer.Instance.Remove(TransferKey.ClubInfo);
                Transfer.Instance.Remove(TransferKey.RoomSwitch);
                SceneManager.LoadScene("MainScene");
            });
            btnMember.onClick.AddListener(onMember);
            btnCounter.onClick.AddListener(()=>
            {
                UIClubSpawn.Instance.CreateCounterView();
            });
            btnClubEdit.onClick.AddListener(onEdit);
            btnExchange.onClick.AddListener(onExchange);

            tgTexas.onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    _md.tableType = TableType.Texas;
                    onSwitch();
                }
            });
            tgBull.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    _md.tableType = TableType.Bull;
                    onSwitch();
                }
            });
            imgNotify.gameObject.SetActive(false);
            imgLoad.gameObject.SetActive(false);

            // 注册监听
            NotificationCenter.Instance.AddNotifyListener(NotificationType.OnMsg, onNotify);
        }

        public void Start()
		{
			Screen.orientation = ScreenOrientation.Portrait;

            ClubDetail detail = Transfer.Instance[TransferKey.ClubInfo] as ClubDetail;
            if(detail != null)
            {
                _md.detail = detail;
                _md.GetTableGonfig((ret) =>
                {
                    if (ret.IsOk)
                    {
                        _md.rates = ret.data;
                    }
                });
                initView();
            }
            else
            {
                long clubId = (long)Transfer.Instance[TransferKey.ClubId];
                _md.GetDetail(clubId, (result) =>
                {
                    if(result.IsOk)
                    {
                        _md.detail = result.data;
                        if (HasRight(Auth.Table))
                        {
                            _md.GetTableGonfig((ret) =>
                            {
                                if (ret.IsOk)
                                {
                                    _md.rates = ret.data;
                                }
                            });
                        }
                        initView();
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });
            }
        }

        void initView()
        {
            imgAvatar.gameObject.SetActive(false);
            if (Validate.IsNotEmpty(_md.detail.avatar))
            {
                StartCoroutine(LoadImageUtil.LoadImage(_md.detail.avatar, (sprite) =>
                {
                    imgAvatar.gameObject.SetActive(true);
                    imgAvatar.sprite = sprite;
                }));
            }
            tvName.text = _md.detail.name;
            string text = string.Format(LocalizationManager.Instance.GetText("5200"), _md.detail.clubId, _md.detail.creator);
            LimitText.LimitAndSet(text, tvId, 280);
            tvClubCoins.text = _md.detail.clubCoins.ToString();
            tvMyCoins.text = _md.detail.coin.ToString();
            tvIntro.text = _md.detail.intro;
            sLevel.value = _md.detail.level;

            btnClubEdit.gameObject.SetActive(HasRight(Auth.Edit));
            btnExchange.gameObject.SetActive(HasRight(Auth.Exchange));
            btnCounter.gameObject.SetActive(HasRight(Auth.Counter));

            if(HasRight(Auth.Join))
            {
                if(_md.detail.applyNum > 0)
                {
                    imgNotify.gameObject.SetActive(true);
                }
            }

            if (IsNormal)
            {
                goClubCoin.SetActive(false);
            }
            else
            {
                goClubCoin.SetActive(true);
            }

            if (_md.detail.level > 0)
            {
                imgExpir.gameObject.SetActive(true);
                if (_md.detail.expir <= 0)
                {
                    tvExpir.text = "已过期";
                }
                else
                {
                    tvExpir.text = _md.detail.expir.ToString() + LocalizationManager.Instance.GetText("5013");
                }
            }
            else
            {
                imgExpir.gameObject.SetActive(false);
            }

            if (!Game.Instance.DzEnabled || !Game.Instance.BullEnabled)
            {
                if (Game.Instance.DzEnabled)
                {
                    tgTexas.isOn = true;
                    _md.tableType = TableType.Texas;
                }

                if (Game.Instance.BullEnabled)
                {
                    tgBull.isOn = true;
                    _md.tableType = TableType.Bull;
                }
            }

            tgTexas.gameObject.SetActive(Game.Instance.DzEnabled);
            tgBull.gameObject.SetActive(Game.Instance.BullEnabled);

            object roomSwitch = Transfer.Instance[TransferKey.RoomSwitch];
            if (roomSwitch != null)
            {
                switch ((ClubRoomSwitch)roomSwitch)
                {
                    case ClubRoomSwitch.Bull:
                        _md.tableType = TableType.Bull;
                        tgBull.isOn = true;
                        _md.Clear();
                        break;
                    case ClubRoomSwitch.Dz:
                        _md.tableType = TableType.Texas;
                        tgTexas.isOn = true;
                        _md.Clear();
                        break;
                }
            }
            if (Game.Instance.DzEnabled || Game.Instance.BullEnabled)
            {
                onSwitch();
            }
        }

        #endregion

        #region 数据及ui更新

        public void UpdateLevel(ItemLevelCardData data)
        {
            _md.detail.level = data.level;
            sLevel.value = data.level;
            _md.detail.level = data.level;
            _md.detail.memberLimit = data.memberNum;
        }

        public void UpdateCoins(long clubCoins)
        {
            _md.detail.clubCoins = clubCoins;
            tvClubCoins.text = clubCoins.ToString();
        }

        public void UpdateMyCoins(long coins)
        {
            _md.detail.coin += coins;
            tvMyCoins.text = _md.detail.coin.ToString();
        }

        public void UpdateExpir()
        {
            if (_md.detail.level > 0)
            {
                imgExpir.gameObject.SetActive(true);
                if (_md.detail.expir <= 0)
                {
                    tvExpir.text = "已过期";
                }
                else
                {
                    tvExpir.text = _md.detail.expir.ToString() + LocalizationManager.Instance.GetText("5013");
                }
            }
            else
            {
                imgExpir.gameObject.SetActive(false);
            }
        }

        #endregion

        #region  编辑信息

        void onEdit()
        {
            ClubEditView vi = UIClubSpawn.Instance.CreateEditView();
            vi.InitView(_md.detail.name, _md.detail.intro, _md.detail.avatar);
            vi.OnEditInfoEvent = (name, intro)=>
            {
                _md.EditInfo(name, intro, (result) =>
                {
                    if (result.IsOk)
                    {
                        _md.detail.name = name;
                        _md.detail.intro = intro;
                        tvName.text = name;
                        tvIntro.text = intro;
                        vi.HideAndDestory();
                    }
                    else
                    {
                        Game.Instance.ShowTips(result.errorMsg);
                    }
                });

            };

            vi.OnEditAvatarEvent = (bytes) =>
            {
                UIClubSpawn.Instance.CreateLoadMask();
                Coroutine coroutine = StartCoroutine(hideMask());
                _md.UploadAvatar(bytes, (httpResult) =>
                {
                    if (httpResult.IsOk)
                    {
                        string httpPath = httpResult.data;

                        #region   更新头像
                        _md.EditAvatar(httpPath, (result) =>
                        {
                            UIClubSpawn.Instance.HideMask();
                            StopCoroutine(coroutine);
                            if (!result.IsOk)
                            {
                                Game.Instance.ShowTips(result.errorMsg);
                                return;
                            }
                            vi.LoadAvatar(httpPath);
                            _md.detail.avatar = httpPath;
                            imgAvatar.gameObject.SetActive(true);
                            LoadImageUtil.LoadByte(bytes, (sprite) =>
                            {
                                imgAvatar.sprite = sprite;
                            });
                        });
                        #endregion
                    }
                    else
                    {
                        Game.Instance.ShowTips(httpResult.errorMsg);
                        StopCoroutine(coroutine);
                        UIClubSpawn.Instance.HideMask();
                    }
                });
            };
        }

        IEnumerator hideMask()
        {
            yield return new WaitForSeconds(15);
            UIClubSpawn.Instance.HideMask();
        }

        #endregion

        #region 点击成员
        void onMember()
        {
            if (IsNormal)
            {
                UIClubSpawn.Instance.CreateClubNormalView().InitView(_md.detail);
            }
            else
            {
                UIClubSpawn.Instance.CreateClubProxyView().InitView(_md.detail, imgNotify.gameObject.activeSelf);
            }
        }
        #endregion

        #region 兑换

        void onExchange()
        {
            ExchangeView vi = UIClubSpawn.Instance.CreateExchangeView();
            vi.OnExchangeEvent = (result) =>
            {
                UpdateCoins(result.clubCoins);
                Game.Instance.CurPlayer.Diamond = result.diamond;
                vi.HideAndDestory();
            };
        }

        #endregion

        #region 德州、牛牛切换
        void onSwitch()
        {
            switch (_md.tableType)
            {
                case TableType.Texas:
                    _md.game = GameType.dz;
                    findList();
                    break;
                case TableType.Bull:
                    _md.game = GameType.bull;
                    findList();
                    break;
            }
        }
        #endregion

        #region 桌子列表

        void findList()
        {
            imgLoad.gameObject.SetActive(true);
            gdView.gameObject.SetActive(false);
            _md.FindTables((result) =>
            {
                imgLoad.gameObject.SetActive(false);
                gdView.gameObject.SetActive(true);
                if (result.IsOk)
                {
                    _md.DataItems = result.data;
                }
                else
                {
                    _md.DataItems = new List<ItemTableData>();
                }
                initList();
            });
        }

        void initList()
        {
            gdView.Clear();
            if (!_md.detail.isExpir)
            {
                if (HasRight(Auth.Table))
                {
                    ItemTableData data = new ItemTableData();
                    data.game = _md.game;
                    data.isCreator = true;
                    _md.AddFirst(data);
                }
            }

            for (int i = 0; i < _md.Count; i++)
            {
                ItemTableView view = gdView.Add(_md.DataItems[i]) as ItemTableView;
                view.OnItemClickEvent = (vi) =>
                {
                    if(_md.detail.isExpir)
                    {
                        Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5014"));
                        return;
                    }

                    ItemTableData data = vi.Data as ItemTableData;
                    if (data.isCreator)
                    {
                        if (data.isTexas)
                        {
                            onCreateTexas();
                        }
                        else
                        {
                            onCreateBull();
                        }
                    }
                    else
                    {
                        if(data.isPin)
                        {
                            onPin(data.roomId);
                        }
                        else
                        {
                            onEnterTable(data.roomId, string.Empty);
                        }
                    }
                };
            }
        }

        // 创建桌子
        void onCreateTexas()
        {
            CreateTexasView view = UIClubSpawn.Instance.CreateTexasView();
            view.OnCreateTabbleEvent = () =>
            {
                view.HideAndDestory();
                findList();
            };
        }

        // 创建桌子
        void onCreateBull()
        {
            CreateBullView view = UIClubSpawn.Instance.CreateBullView();
            view.OnCreateTabbleEvent = () =>
            {
                view.HideAndDestory();
                findList();
            };
        }

        // 输入房间密码
        void onPin(long roomId)
        {
            KeyBoardView vi = UIClubSpawn.Instance.CreateKeyBoardView();
            vi.OnKeyBoardEvent = (pin) =>
            {
                vi.HideAndDestory();
                onEnterTable(roomId, pin);
            };
        }

        private Coroutine _enterRoom;
        // 进入房间
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

        void canEnter(JsonObject json)
        {
            UIClubSpawn.Instance.HideMask();
            if(_enterRoom != null)
            {
                StopCoroutine(_enterRoom);
            }
            int code = int.Parse(json["code"].ToString());
            if ( code == 200)
            {
                JsonObject roomInfo = json["roomInfo"] as JsonObject;
                if(roomInfo != null)
                {
                    roomInfo.Add("clubChips", _md.detail.coin);
                    Transfer.Instance[TransferKey.RoomInfo] = roomInfo;
					PlayerPrefs.SetString("LastScene","ClubScene");
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
                if (code == 216) // 房间不存在或已过期
                {
                    findList();
                }
            }
        }

        #endregion

        private void Update()
        {
            if(_queueAction.Count > 0)
            {
                _queueAction.Dequeue()();
            }

            if(imgLoad.gameObject.activeSelf)
            {
                imgLoad.transform.Rotate(new Vector3(0, 0, -imgLoad.transform.position.z), 3f);
            }
        }

        #region 消息监听

        // 申请通知
        void onNotify(NotifyMsg msg)
        {
            JsonObject jObj = msg["msg"] as JsonObject;
            if (jObj == null)
                return;
            string op = jObj["op"] as string;
            JsonObject data = jObj["data"] as JsonObject;
            long clubId = (long)data["clubId"];
            if (op == null)
                return;
            switch(op)
            {
                case "clubApply_join":
                    onApplyNotify(clubId);
                    break;
                case "clubMember_delete":
                    onKickoutNotify(clubId);
                    break;
                case "clubMember_setSubagent":
                    onSubagentNotify(true, clubId);
                    break;
                case "clubMember_unsetSubagent":
                    onSubagentNotify(false, clubId);
                    break;
                case "clubCounter_send":
                    onChipsNotify(true, clubId, (long)data["coin"]);
                    break;
                case "clubCounter_recycle":
                    onChipsNotify(false, clubId, (long)data["coin"]);
                    break;
            }
        }

        void onApplyNotify(long clubId)
        {
            if (clubId != ClubId || !HasRight(Auth.Join))
                return;
            imgNotify.gameObject.SetActive(true);
            NotificationCenter.Instance.DispatchNotify(NotificationType.ApplyJoin, new NotifyMsg());
        }

        void onKickoutNotify(long clubId)
        {
            if (clubId != ClubId)
                return;
            SureView view = UIClubSpawn.Instance.CreateSureView();
            view.ShowTip(LocalizationManager.Instance.GetText("5004"))
                .AutoHide(10, LocalizationManager.Instance.GetText("5010"));
            view.OnSureEvent = () =>
            {
                Transfer.Instance.Remove(TransferKey.ClubId);
                SceneManager.LoadScene("MainScene");
            };
        }

        void onSubagentNotify(bool isSet, long clubId)
        {
            if(clubId != ClubId)
                return;
            if(isSet)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5007"));
            }
            else
            {
                SureView view = UIClubSpawn.Instance.CreateSureView();
                view.ShowTip(LocalizationManager.Instance.GetText("5006"))
                    .AutoHide(10, LocalizationManager.Instance.GetText("5010"));
                view.OnSureEvent = () =>
                {
                    Transfer.Instance.Remove(TransferKey.ClubId);
                    SceneManager.LoadScene("MainScene");
                };
            }
        }

        void onChipsNotify(bool isSend, long clubId, long coin)
        {
            if (clubId != ClubId)
                return;
            if (isSend)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5008"));
            }
            else
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5009"));
            }
            _md.detail.coin = coin;
            tvMyCoins.text = coin.ToString();
            if (IsNormal)
            {
                NotificationCenter.Instance.DispatchNotify(NotificationType.ChangeClubCoin, new NotifyMsg().value("coin", coin));
            }
        }

        private void OnDestroy()
        {
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.OnMsg, onNotify);
        }
        #endregion

    }
}
