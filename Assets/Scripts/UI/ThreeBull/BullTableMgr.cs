using Newtonsoft.Json;
using RT;
using SimpleJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BullGameOp
{
    EnterRoomOp = 0,
    OutRoomOp = 1,
    SitDownOp = 2,
    StandUpOp = 3,
    StartGameOp = 4,
    PreAddBetOp = 5,
    BankSGOp = 6,
    BankNNOp = 7,
    ShowSGOp = 8,
    ShowNNOp = 9,
    GameNotify = 99,
    Reconnect = 11,
    BackRoom = 101
}

public class BullTableMgr : MonoBehaviour
{
    private PokerItem pokerTpl;
    GameCmdMgr CmdMgr;

    public BullSuitView[] SuitViews;
    public BullSeatView[] SeatViews;
    public RectTransform[] SeatPositions;
    public List<BullSeatView> OnSeatViews;

    public RectTransform PublicPanel;
    public RectTransform WaitPanel;
    public Button StartBtn;
    public Text RoomNameTxt;
    public Text RoomBetTxt;

    public BullSGView BullOpenView;
    public BullNNDragView BullDragView;

    //房间操作界面
    public TexasBuyChipView BuyChipView;
    public BullHistoryView HistoryPanel;
    public TexasLiveView LiveRecordPanel;
    public TexasFinalStatView FinalStatPanel;
    public BullMenuView MenuPanel;
    public BullIntroView IntroPanel;

    public Button MenuBtn;
    public Button AchieveBtn;
    public Button HistoryBtn;
    public Button VoiceBtn;
    public Button ShopBtn;

    public Image ProcessImg;
    public Text ProcessTxt;

    //游戏操作界面
    public BullActionView ActionPanel;
    public GamePlayerView PlayerInfoView;
    public RectTransform MaskView;

    //进入房间初始化信息
    public BullRoom RoomInfo;

    public Dictionary<int, BullPlayer> PlayerList;
    public Dictionary<int, BullPlayer> UpPlayerList;

    //当前玩家信息
    BullPlayer me;
    int ClubChips;//当前玩家在这个俱乐部的所有筹码

    //自己玩家位置
    BullSeatView CurPlayerSeat=null;
    public CurSuitView CurPlayerSuit;
    public BullClock MyClock;

    //想要坐下的位置，未买筹码
    int wantSit = -1;
    //预约退出房间
    bool wantExit = false;
    //机器人
    bool isRobot = false;
    //游戏是否进行中
    bool isPlaying = false;
    
    //表庄倍数
    int opid = 0;
    int seatnum = 7;
    string curProcess = "wait";

    //pomelo actions
    Queue<JsonObject> actionQueue;
    string action = "operation";
    private void Awake()
    {
		actionQueue = new Queue<JsonObject>();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
        Game.Instance.SetTips();
        OnSeatViews = new List<BullSeatView>();
        NotificationCenter.Instance.AddNotifyListener(NotificationType.StopServer, onStopServer);
    }

    #region 停服通知

    void onStopServer(NotifyMsg msg)
    {
        if (isPlaying)
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1022"));
        }
    }

    private void OnDestroy()
	{
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.Portrait;
        NotificationCenter.Instance.RemoveNotifyListener(NotificationType.StopServer, onStopServer);
    }

    #endregion
    void Start()
    {
        pokerTpl = Resources.Load<PokerItem>("Prefabs/Poker/PokerItem");

        CmdMgr = GetComponent<GameCmdMgr>();
        JsonObject roomInfo = Transfer.Instance[TransferKey.RoomInfo] as JsonObject;

        if (roomInfo != null)
        {
            GetRoomInfoDone(roomInfo);
        }
        else
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1113"));// ("加载信息失败，请返回重试");
        }

        StartBtn.onClick.AddListener(delegate {
            OnStartBtnClick();
        });

        MenuBtn.onClick.AddListener(delegate () {
            OnMenuBtnClick();
        });
        MenuPanel.MenuStandUpBtn.onClick.AddListener(delegate () {
            OnMenuStandUpBtnClick();
        });
        MenuPanel.MenuBuyChipBtn.onClick.AddListener(delegate () {
            OnMenuBuyChipBtnClick();
        });
        MenuPanel.MenuIntroBtn.onClick.AddListener(delegate () {
            OnMenuIntroBtnClick();
        });
        MenuPanel.MenuSettingBtn.onClick.AddListener(delegate ()
        {
            OnMenuSettingBtnClick();
        });
        MenuPanel.MenuExitRoomBtn.onClick.AddListener(delegate () {
            OnMenuExitRoomBtnClick();
        });
        MenuPanel.MenuCloseRoomBtn.onClick.AddListener(delegate () {
            OnMenuCloseRoomBtnClick();
        });
        ActionPanel.NoBankBtn.onClick.AddListener(delegate () {
            PlayerBankClick(0);
        });

        for (int i = 0; i < ActionPanel.BankBtnList.Count; i++)
        {
            Button btn = ActionPanel.BankBtnList[i];
            btn.onClick.AddListener(delegate () {
                int idx = ActionPanel.BankBtnList.IndexOf(btn);
                PlayerBankClick(ActionPanel.BankTimes[idx]);
            });
        }

        BullOpenView.OpenBtn.onClick.AddListener(delegate () {
            SGOpenClick();
        });
        BullOpenView.SGRubbingView.OnRubbingEndEvent += (view) => {
            SGOpenClick();
        };
        BullDragView.CommitBtn.onClick.AddListener(delegate () {
            NNDragCommitClick();
        });
        BullDragView.onDragDoneEvent += () => {
            if (CurPlayerSeat != null && CurPlayerSeat.isSit)
            {
                CurPlayerSeat.Player.Cards = BullDragView.GetFinalSuit();
                ShowNNCards(CurPlayerSeat.Player.Cards,0);
            }
        };
        BullDragView.onAutoDoneEvent += () => {
            NNDragCommitClick();
        };

        AchieveBtn.onClick.AddListener(delegate () {
            OnAchieveBtnClick();
        });
        HistoryBtn.onClick.AddListener(delegate () {
            OnHistoryBtnClick();
        });

        BuyChipView.BuyBtn.onClick.AddListener(delegate {
            BuyChipView.gameObject.SetActive(false);
            PreAddBet(int.Parse(BuyChipView.BuyChipTxt.text));
        });
    }

    void GetRoomInfoDone(JsonObject roomInfo)
    {
        RoomInfo = JsonConvert.DeserializeObject<BullRoom>(roomInfo.ToString());
        if (RoomInfo.MaxBet == 0) {
            RoomInfo.MaxBet = RoomInfo.BaseBet * 1000;
        }
        RoomInfo.BankerSite = 0;
        seatnum = RoomInfo.SeatNum;
        SetSeatView();

        CurPlayerSeat = null;
        me = null;
        ClubChips = int.Parse(roomInfo["clubChips"].ToString());

        RoomNameTxt.text = RoomInfo.RoomTitle;
        RoomBetTxt.text = string.Format(LocalizationManager.Instance.GetText("5904"), RoomInfo.BaseBet);// "底注：" + RoomInfo.BaseBet;

        JsonObject players = roomInfo["players"] as JsonObject;
        JsonObject lookers = roomInfo["upPlayers"] as JsonObject;
        if (players != null && !string.IsNullOrEmpty(players.ToString()))
        {
            PlayerList = JsonConvert.DeserializeObject<Dictionary<int, BullPlayer>>(players.ToString());
        }
        else
        {
            PlayerList = new Dictionary<int, BullPlayer>();
        }
        if (lookers != null && !string.IsNullOrEmpty(lookers.ToString()))
        {
            UpPlayerList = JsonConvert.DeserializeObject<Dictionary<int, BullPlayer>>(lookers.ToString());
        }
        else
        {
            UpPlayerList = new Dictionary<int, BullPlayer>();
        }
        InitView();

    }
    void SetSeatView() {
        if (SeatViews.Length > 0)
        {
            if (seatnum == 5 && SeatViews.Length == 7)
            {
                SuitViews[1].gameObject.SetActive(false);
                SeatViews[1].gameObject.SetActive(false);
                SeatPositions[1].gameObject.SetActive(false);

                SuitViews[5].gameObject.SetActive(false);
                SeatViews[5].gameObject.SetActive(false);
                SeatPositions[5].gameObject.SetActive(false);

                List<BullSuitView> tmp1 = SuitViews.ToList();
                tmp1.RemoveAt(5);
                tmp1.RemoveAt(1);
                SuitViews = tmp1.ToArray();

                List<BullSeatView> tmp2 = SeatViews.ToList();
                tmp2.RemoveAt(5);
                tmp2.RemoveAt(1);
                SeatViews = tmp2.ToArray();

                List<RectTransform> tmp3 = SeatPositions.ToList();
                tmp3.RemoveAt(5);
                tmp3.RemoveAt(1);
                SeatPositions = tmp3.ToArray();
            }
            for (int i = 0; i < SeatViews.Length; i++)
            {
                BullSeatView s = SeatViews[i];
                s.SeatNo = i;
                s.SuitPosIndex = i;
                s.SuitView = SuitViews[i];
                s.SeatPosIndex = i;
                s.SitBtn.onClick.RemoveAllListeners();
                s.SitBtn.onClick.AddListener(delegate () {
                    SitDown(s.SeatNo);
                });
                s.AvatarBtn.onClick.RemoveAllListeners();
                s.AvatarBtn.onClick.AddListener(delegate () {
                    if (s.Player != null && s.isSit)
                    {
                        PlayerInfoView.Show(s.Player.UId, RoomInfo.ClubId, CurPlayerSeat == null || !CurPlayerSeat.isSit);
                    }
                });
            }
        }
    }
    void InitView()
    {
        OnSeatViews.Clear();
        if (PlayerList != null)
        {
            foreach (int k in PlayerList.Keys)
            {
                BullPlayer p = PlayerList[k];
                BullSeatView s = SeatViews[p.Site];
                s.Player = p;
                s.SitDown();
                OnSeatViews.Add(s);
                if (p.UId == Game.Instance.CurPlayer.Uid)
                {
                    CurPlayerSeat = s;
                    me = p;
                    CurPlayerSuit.gameObject.SetActive(true);
                    if (RoomInfo.Status == 0)
                    {
                        StartBtn.gameObject.SetActive(p.StartEnable == 1);
                        PublicPanel.gameObject.SetActive(p.StartEnable == 0);
                        WaitPanel.gameObject.SetActive(p.StartEnable == 0);
                    }
                    else
                    {
                        StartBtn.gameObject.SetActive(false);
                        ProcessImg.gameObject.SetActive(true);
                        PublicPanel.gameObject.SetActive(true);
                        WaitPanel.gameObject.SetActive(false);
                    }
                }
            }

            if (CurPlayerSeat != null)
            {
                MaskView.gameObject.SetActive(true);
                AnimationBeforeSitdown(me.Site, delegate ()
                {
                    ReConnect();
                    MaskView.gameObject.SetActive(false);
                });
            }
            else
            {
                ReConnect();
            }
        }
        if (UpPlayerList != null)
        {
            foreach (int k in UpPlayerList.Keys)
            {
                BullPlayer p = UpPlayerList[k];
                if (p.UId == Game.Instance.CurPlayer.Uid)
                {
                    me = p;
                    if (RoomInfo.Status == 0)
                    {
                        StartBtn.gameObject.SetActive(p.StartEnable == 1);
                        PublicPanel.gameObject.SetActive(p.StartEnable == 0);
                        WaitPanel.gameObject.SetActive(p.StartEnable == 0);
                    }
                    else
                    {
                        StartBtn.gameObject.SetActive(false);
                        PublicPanel.gameObject.SetActive(true);
                        ProcessImg.gameObject.SetActive(true);
                        WaitPanel.gameObject.SetActive(false);
                    }
                }
            }
        }

        RefreshSeatViews();

        if (me.StartEnable == 1)
        {
            //MenuPanel.ShowCloseBtn();
        }
        if (RoomInfo.IsPublic == 1)
        {
            HistoryBtn.gameObject.SetActive(false);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            //牌局恢复
            ClearGameOp();
            MyClock.Close();
            StartCoroutine(DataClear(0.01f));
            ReConnect();
        }
    }

    void Update()
    {
        if (!isPlaying && SystemNotify.Instance.WantStopServer) {
            SystemNotify.Instance.StopServer();
            return;
        }

        if (RoomInfo != null && RoomInfo.IsPublic == 0)
        {
            if (RoomInfo.RoomTime > 0)
            {
                RoomInfo.RoomTime -= Time.deltaTime;
                if (RoomInfo.RoomTime<300 && RoomInfo.RoomTime % 60 == 0) {
                    GetRestTime();
                }
            }
            else
            {
                if (!isPlaying)
                {
                    RoomInfo.RoomTime = 0;
                    RoomClosedNotify();
                }
            }
        }
        if (Game.Instance.PomeloNode.GameDataQueue != null && Game.Instance.PomeloNode.GameDataQueue.Count > 0)
        {
            //1.如果是表情的op，直接进入队列
            //2.如果没有序列号的op，直接进入队列
            //3.有序列号的op，进入Cmd管理排序
            JsonObject jb = Game.Instance.PomeloNode.GameDataQueue.Dequeue();
            if (jb.Keys.Contains("mod") && jb["mod"].ToString() == "game_cowWater")
            {
                if (jb.ContainsKey("seqNum"))
                {
                    CmdMgr.AddBullCmd(jb, DateTime.Now);
                }
                else
                {
                    actionQueue.Enqueue(jb);
                }
            }
            else if (jb.Keys.Contains("op") && jb["op"].ToString() == "user_emoji")
            {
                actionQueue.Enqueue(jb);
            }
            else if (jb.Keys.Contains("op") && jb["op"].ToString() == "user_audio")
            {
                actionQueue.Enqueue(jb);
            }
            else if (jb.Keys.Contains("op") && jb["op"].ToString() == "clubRoom_setRoomTime")
            {
                actionQueue.Enqueue(jb);
            }
        }
        GameCommond cmd = CmdMgr.GetBullCmd(DateTime.Now);
        if (cmd != null)
        {
            actionQueue.Enqueue(cmd.Data);
        }

        if (actionQueue.Count > 0)
        {
            JsonObject resp = actionQueue.Dequeue();
            int op = int.Parse(resp[action].ToString());
            Debug.Log("operation---" + op + "---data--->>" + resp.ToString());

            switch (op)
            {
                case 0:
                    GetRoomInfoDone(resp);
                    break;
                case 1:
                    ExitRoomDone(resp);
                    break;
                case 2:
                    SitDownDone(resp);
                    break;
                case 3:
                    StandUpDone(resp);
                    break;
                case 4:
                    StartGameDone(resp);
                    break;
                case 5:
                    PreAddBetDone(resp);
                    break;
                case 6:
                    CallSGBankerDone(resp);
                    break;
                case 7:
                    CallNNBankerDone(resp);
                    break;
                case 8:
                    ShowSGCardsDone(resp);
                    break;
                case 9:
                    ShowNNCardsDone(resp);
                    break;
                case 11:
                    ReConnectDone(resp);
                    break;
                case 101:
                    {
                        JsonObject roomInfo = Transfer.Instance[TransferKey.RoomInfo] as JsonObject;
                        if (roomInfo != null)
                        {
                            GetRoomInfoDone(roomInfo);
                        }
                        else
                        {
                            string last = PlayerPrefs.GetString("LastScene");
                            if (string.IsNullOrEmpty(last))
                            {
                                SceneManager.LoadScene("MainScene");
                            }
                            else
                            {
                                SceneManager.LoadScene(last);
                            }
                        }
                    }
                    break;
                default:
                    OnBullNotified(resp);
                    break;
            }
        }
    }
    #region 数据处理

    #region 重连操作
    void ReConnect()
    {
        MaskView.gameObject.SetActive(true);
        JsonObject param = new JsonObject();
        BullApi.ReConnect(param, (result) => {
            result.Add(action, (int)BullGameOp.Reconnect);
            actionQueue.Enqueue(result);
        });

    }
    void ReConnectDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) != 200)
        {
            Debug.Log(result["code"].ToString());
            SceneManager.LoadScene(PlayerPrefs.GetString("LastScene"));
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(),"7011"));// "重连房间失败");
        }
        else
        {
            if (!result.ContainsKey("gameProcess"))
            {
                //牌局恢复
                ClearGameOp();
                MyClock.Close();
                StartCoroutine(DataClear(0.01f));
                return;
            }
            ClearGameOp();
            BullReconnection recnt = JsonConvert.DeserializeObject<BullReconnection>(result["gameProcess"].ToString());
            //重现当前牌局
            ReSetGambleInfo(recnt);
            ResetOwnInfo(recnt);
            ResetOtherCards(recnt.OtherPlayerCards);

            MyClock.Show(recnt.Ts);
        }
    }
    void ClearGameOp()
    {
        Game.Instance.PomeloNode.GameDataQueue.Clear();
        CmdMgr.ClearCmd();
    }
    void ReSetGambleInfo(BullReconnection recnt)
    {
        //关闭等待开始界面
        StartBtn.gameObject.SetActive(false);
        PublicPanel.gameObject.SetActive(true);
        WaitPanel.gameObject.SetActive(false);
        ActionPanel.Close();
        BullOpenView.gameObject.SetActive(false);
        BullDragView.gameObject.SetActive(false);

        //初始化数据
        DataInit();

        curProcess = recnt.CurRound;
        ProcessImg.gameObject.SetActive(true);

        //设置庄家
        BullSeatView bs1 = SeatViews[RoomInfo.BankerSite];
        bs1.SetBanker(false);

        Debug.Log("Ronconnect ------ processtep---->"+curProcess);

        switch (curProcess)
        {
            case "step_callBanker_SG":
                {
                    ProcessTxt.text = LocalizationManager.Instance.GetText("8003");// "三公叫庄";
                    CmdMgr.bullNextProcess = BullGameProcess.BullGPSGBank;
                    break;
                }
            case "step_showCard_SG":
                {
                    ProcessTxt.text = LocalizationManager.Instance.GetText("8004");// "等待玩家搓牌";
                    CmdMgr.bullNextProcess = BullGameProcess.BullGPSGShow;
                    if(recnt.BankerSite > -1 && recnt.BankerSite < 7 && recnt.SGBankOdds > 0)
                    {
                        RoomInfo.BankerSite = recnt.BankerSite;
                        BullSeatView bs2 = SeatViews[RoomInfo.BankerSite];
                        bs2.SetBanker(true);
                        bs2.DoSetBank(recnt.SGBankOdds);
                    }
                    break;
                }
            case "step_callBanker_COW":
                {
                    ProcessTxt.text = LocalizationManager.Instance.GetText("8005");// "牛牛叫庄";
                    CmdMgr.bullNextProcess = BullGameProcess.BullGPNNBank;
                    break;
                }
            case "step_putCard_COW":
                {
                    ProcessTxt.text = LocalizationManager.Instance.GetText("8006");// "等待玩家排牌";
                    CmdMgr.bullNextProcess = BullGameProcess.BullGPNNShow;
                    if (recnt.BankerSite > -1 && recnt.BankerSite < 7 && recnt.NNBankOdds > 0)
                    {
                        RoomInfo.BankerSite = recnt.BankerSite;
                        BullSeatView bs2 = SeatViews[RoomInfo.BankerSite];
                        bs2.SetBanker(true);
                        bs2.DoSetBank(recnt.NNBankOdds);
                    }
                    break;
                }
                case "wait":
                {
                    CmdMgr.bullNextProcess = BullGameProcess.BullGPStart;
                    break;
                }
        }
    }
    void ResetOwnInfo(BullReconnection recnt)
    {
        if (CurPlayerSeat != null)
        {
            isRobot = false;
            CurPlayerSeat.RobotImg.gameObject.SetActive(isRobot);
            switch (curProcess)
            {
                case "step_callBanker_SG":
                    {
                        if (recnt.MySGBankOdds >= 0)
                        {
                            ActionPanel.gameObject.SetActive(false);
                        }
                        else
                        {
                            ActiveActionPanel();
                        }
                        break;
                    }
                case "step_showCard_SG":
                    {
                        //设置自己的卡片
                        if (recnt.SelfCards != null)
                        {
                            CurPlayerSeat.Player.Cards = recnt.SelfCards;
                            CurPlayerSuit.gameObject.SetActive(true);
                            CurPlayerSuit.ShowSuit(3, recnt.SelfCards);
                        }
                        break;
                    }
                case "step_callBanker_COW":
                    {
                        if (recnt.MyNNBankOdds >= 0)
                        {
                            ActionPanel.gameObject.SetActive(false);
                        }
                        else
                        {
                            ActiveActionPanel();
                        }
                        //设置自己的卡片
                        if (recnt.SelfCards != null)
                        {
                            CurPlayerSeat.Player.Cards = recnt.SelfCards;
                            CurPlayerSuit.gameObject.SetActive(true);
                            CurPlayerSuit.ShowSuit(3, recnt.SelfCards);
                        }
                        break;
                    }
                case "step_putCard_COW":
                    {
                        //设置自己的卡片
                        if (recnt.SelfCards != null)
                        {
                            CurPlayerSeat.Player.Cards = recnt.SelfCards;
                            CurPlayerSuit.gameObject.SetActive(true);
                            CurPlayerSuit.ShowSuit(5, recnt.SelfCards);
                            BullDragView.PlayerSuitView = CurPlayerSuit;
                            BullDragView.SetPutCards(recnt.SelfCards,recnt.Ts);
                        }
                        break;
                    }
                default:
                    {
                        if (recnt.SelfCards != null)
                        {
                            CurPlayerSeat.Player.Cards = recnt.SelfCards;
                            CurPlayerSuit.gameObject.SetActive(true);
                            if (recnt.SelfCards.Length <= 3)
                            {
                                CurPlayerSuit.ShowSuit(3, recnt.SelfCards);
                            }
                            else
                            {
                                CurPlayerSuit.ShowSuit(5, recnt.SelfCards);
                                BullDragView.PlayerSuitView = CurPlayerSuit;
                                BullDragView.SetPutCards(recnt.SelfCards, recnt.Ts);
                            }
                        }
                        break;
                    }
            }
        }

        if (recnt.PlayerBetList != null)
        {
            foreach (string uid in recnt.PlayerBetList.Keys)
            {
                if (PlayerList.ContainsKey(int.Parse(uid)))
                {
                    BullPlayer p = PlayerList[int.Parse(uid)];
                    p.Bet -= recnt.PlayerBetList[uid];
                }

            }
        }
    }
    void ResetOtherCards(Dictionary<string,int[]>otherCards) {
        if (otherCards != null) {
            foreach (string k in otherCards.Keys) {
                int uid = int.Parse(k);
                if (PlayerList.ContainsKey(uid)) {
                    BullPlayer p = PlayerList[uid];
                    p.Cards = otherCards[k];
                    BullSeatView s = SeatViews[p.Site];

                    switch (curProcess)
                    {
                        case "step_callBanker_SG":
                            {
                                break;
                            }
                        case "step_showCard_SG":
                        case "step_callBanker_COW":
                            {
                                s.SuitView.ShowSuit(3, otherCards[k]);
                                break;
                            }
                        case "step_putCard_COW":
                            {
                                s.SuitView.ShowSuit(5, otherCards[k]);
                                break;
                            }
                        case "wait":
                            {

                                if (otherCards[k].Length <= 3)
                                {
                                    s.SuitView.ShowSuit(3, otherCards[k]);
                                }
                                else
                                {
                                    s.SuitView.ShowSuit(5, otherCards[k]);
                                }
                                break;
                            }
                    }
                }
            }
        }
    }
    #endregion

    #region 进出房间和坐下
    void CloseRoom()
    {
        MenuPanel.gameObject.SetActive(false);
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("roomId", RoomInfo.RoomId.ToString());
        TexasApi.SetGameRoomTime(param,  (err) =>
        {
            if (!string.IsNullOrEmpty(err))
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("7039"));
            }
        });
    }

    void ExitRoom()
    {
        MaskView.gameObject.SetActive(true);
        JsonObject param = new JsonObject();
        BullApi.ExitRoom(param, (result) => {
            result.Add(action, (int)BullGameOp.OutRoomOp);
            actionQueue.Enqueue(result);
        });
    }
    void ExitRoomDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) != 200)
        {
            Debug.Log(result["code"].ToString());
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(),"7012"));// "退出房间失败");
        }
        else
        {
            MenuPanel.gameObject.SetActive(false);
        }
    }
    void StartGame()
    {
        if (PlayerList.Count > 1)
        {
            JsonObject param = new JsonObject();
            param.Add("roomId", RoomInfo.RoomId);
            BullApi.StartGame(param, (result) =>
            {
                result.Add(action, (int)BullGameOp.StartGameOp);
                actionQueue.Enqueue(result);
            });
            MaskView.gameObject.SetActive(true);
        }
        else
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("7013"));// "至少两人才能开始");
        }
    }
    void StartGameDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) != 200)
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(),"7014"));// "开始游戏失败");
        }
    }
    void SitDown(int site)
    {
        if (me.Bet < RoomInfo.BaseBet || me.Bet == 0)
        {
            BuyChip();
            wantSit = site;
        }
        else
        {
            JsonObject param = new JsonObject();
            param.Add("site", site);
            BullApi.SitDown(param, (result) =>
            {
                result.Add(action, (int)BullGameOp.SitDownOp);
                actionQueue.Enqueue(result);
            });
            MaskView.gameObject.SetActive(true);
        }
    }
    void SitDownDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) != 200)
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(),"7015"));// "坐下失败");
        }
    }
    void StandUp()
    {
        JsonObject param = new JsonObject();
        BullApi.StandUp(param, (result) => {
            result.Add(action, (int)BullGameOp.StandUpOp);
            actionQueue.Enqueue(result);
        });
        MaskView.gameObject.SetActive(true);
    }
    void StandUpDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) != 200)
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(),"7016"));// "站起失败");
        }
    }
    #endregion

    #region 预购筹码
    void PreAddBet(int bet)
    {
        MaskView.gameObject.SetActive(true);
        JsonObject param = new JsonObject();
        param.Add("bet", bet);
        BullApi.PreAddBet(param, (result) => {
            result.Add(action, (int)BullGameOp.PreAddBetOp);
            actionQueue.Enqueue(result);
        });
    }
    void PreAddBetDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            BuyChipView.gameObject.SetActive(false);
        }
        else
        {
            BuyChipView.gameObject.SetActive(true);
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("7017"));// "添加筹码失败");
        }
    }
    #endregion

    #region 标庄和搓牌
    void CallSGBanker(int odds)
    {
        ActionPanel.gameObject.SetActive(false);
        opid++;
        JsonObject param = new JsonObject();
        param.Add("uid", CurPlayerSeat.Player.UId);
        param.Add("op", "callBanker_SG");
        param.Add("odds", odds);
        param.Add("opid", opid);

        BullApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)BullGameOp.BankSGOp);
            result.Add("odds", odds);
            actionQueue.Enqueue(result);
        });
        MaskView.gameObject.SetActive(true);
    }
    void CallSGBankerDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            int odds = int.Parse(result["odds"].ToString());
            //CurPlayerSeat.DoPreBank(odds);
        }
        else
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(), "8007"));// "SG标庄失败");
        }
    }

    void ShowSGCards()
    {
        opid++;
        JsonObject param = new JsonObject();
        param.Add("op", "showCard_SG");
        param.Add("opid", opid);

        BullApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)BullGameOp.ShowSGOp);
            actionQueue.Enqueue(result);
        });
        MaskView.gameObject.SetActive(true);
    }
    void ShowSGCardsDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (CurPlayerSeat.Player.Cards != null && CurPlayerSeat.Player.Cards.Length == 3)
        {
            CurPlayerSuit.gameObject.SetActive(true);
            CurPlayerSuit.ShowPokerInSuit(CurPlayerSeat.Player.Cards[2],2);
        }
        if (int.Parse(result["code"].ToString()) == 200)
        {
            Debug.Log("SG搓牌上传成功");
        }
        else
        {
            Debug.Log("SG搓牌上传失败");
        }
    }

    void CallNNBanker(int odds)
    {
        MaskView.gameObject.SetActive(true);
        ActionPanel.gameObject.SetActive(false);
        opid++;
        JsonObject param = new JsonObject();
        param.Add("uid", CurPlayerSeat.Player.UId);
        param.Add("op", "callBanker_COW");
        param.Add("odds", odds);
        param.Add("opid", opid);

        BullApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)BullGameOp.BankNNOp);
            result.Add("odds", odds);
            actionQueue.Enqueue(result);
        });
    }
    void CallNNBankerDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            int odds = int.Parse(result["odds"].ToString());
            //CurPlayerSeat.DoPreBank(odds);
        }
        else
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(), "8008")); // "NN标庄失败");
        }
    }

    void ShowNNCards(int [] cards,int ok)
    {
        MaskView.gameObject.SetActive(true);
        string tmp = "------final--cards---";
        foreach (int c in cards) { 
            tmp = tmp+c;
        }
        Debug.Log(tmp);
        opid++;
        JsonObject param = new JsonObject();
        param.Add("op", "putCard_COW");
        param.Add("uid", CurPlayerSeat.Player.UId);
        param.Add("opid", opid);
        param.Add("cards", cards);
        param.Add("ok", ok);

        BullApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)BullGameOp.ShowNNOp);
            actionQueue.Enqueue(result);
        });
    }
    void ShowNNCardsDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            Debug.Log("NN搓牌上传成功");
            CurPlayerSuit.Clear();
            CurPlayerSuit.gameObject.SetActive(true);
            CurPlayerSuit.ShowSuit(5, CurPlayerSeat.Player.Cards);
        }
        else
        {
            BullDragView.gameObject.SetActive(true);
            Debug.Log("NN搓牌上传失败");
        }
    }
    #endregion

    #region 监听到的操作和系统事件

    void OnBullNotified(JsonObject data)
    {
        //data.Keys.Contains("mod") && data["mod"].ToString() == "game_cowWater"
        if (data != null && data.ContainsKey("op"))
        {
            string op = data["op"].ToString();
            switch (op)
            {
                case "inRoom":
                    //玩家进入通知
                    PlayerEnterNotify(data["player"] as JsonObject);
                    break;
                case "outRoom":
                    //玩家出去通知
                    PlayerExitNotify(int.Parse(data["uid"].ToString()));
                    break;
                case "siteDown":
                    {
                        //玩家坐下通知
                        if (data.ContainsKey("player")) {
                            BullPlayer p = JsonConvert.DeserializeObject<BullPlayer>(data["player"].ToString());
                            PlayerSitDownNotify(int.Parse(data["uid"].ToString()), p);
                        }
                    }
                    break;
                case "standUp":
                    //玩家站起通知
                    if (data.ContainsKey("type"))
                    {
                        PlayerStandUpNotify(int.Parse(data["uid"].ToString()), data["type"].ToString());
                    }
                    else
                    {
                        PlayerStandUpNotify(int.Parse(data["uid"].ToString()), "");
                    }
                    break;
                case "preAddBet":
                    //玩家购买筹码通知
                    PlayerAddChipNotify(int.Parse(data["uid"].ToString()), int.Parse(data["bet"].ToString()));
                    break;
                case "sg_start":
                    //一局牌局开始
                    GambleStartNotify();
                    break;
                case "sg_showCards":
                    //三公标庄后发其他人2张牌
                    {
                        Dictionary<string, int[]> cards = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(data["showCards"].ToString());
                        SendPlayerSGPokerNotify(cards);
                    }
                    break;
                case "sg_self_showCards":
                    {
                        //三公发自己三张牌
                        int[] cards = JsonConvert.DeserializeObject<int[]>(data["cards"].ToString());
                        ShowSelfSGPokerNotify(cards);
                        break;
                    }
                case "sg_callBanker":
                    {
                        //三公标庄结果
                        int[] players = JsonConvert.DeserializeObject<int[]>(data["maxCallBankerUids_SG"].ToString());
                        int bankId = int.Parse(data["uid"].ToString());
                        int odds = int.Parse(data["odds"].ToString());
                        ShowSGBankerNotify(players, bankId, odds);
                        break;
                    }
                case "cow_callBanker":
                    {
                        //牛牛标庄结果
                        int[] players = JsonConvert.DeserializeObject<int[]>(data["maxCallBankerUids_COW"].ToString());
                        int bankId = int.Parse(data["uid"].ToString());
                        int odds = int.Parse(data["odds"].ToString());
                        ShowNNBankerNotify(players, bankId, odds);
                        break;
                    }
                case "sg_checkout":
                    {
                        //三公结算
                        BullOpenView.SGRubbingView.Hide();
                        ShowSGResultNotify(data["result"].ToString());
                        break;
                    }
                case "cow_self_showCards":
                    //牛牛发牌
                    {
                        int[] cards = JsonConvert.DeserializeObject<int[]>(data["cards"].ToString());
                        SendSelfNNPokerNotify(cards);
                        break;
                    }
                case "cow_selfCardType":
                    //牛牛牌型
                    {
                        ShowSelfNNPokerNotify(data["type"].ToString(), int.Parse(data["typePoint"].ToString()));
                        break;
                    }
                case "cow_checkout":
                    //牛牛结算
                    {
                        ShowNNResultNotify(data["result"].ToString());
                        break;
                    }
                case "nextRound": {
                        NextRoundNotify(data["round"].ToString());
                        break;
                    }
                case "opError":
                    {
                        //错误提示
                        Game.Instance.ShowTips(LocalizationManager.Instance.GetText(data["msg"].ToString()));
                        if (data.ContainsKey("opid"))
                        {
                            int lastop = int.Parse(data["opid"].ToString());
                            if (lastop == opid && CurPlayerSeat != null)
                            {
                                ActionPanel.gameObject.SetActive(true);
                            }
                        }
                    }
                    break;
                case "user_emoji":
                    {
                        EmojiUse eu = JsonConvert.DeserializeObject<EmojiUse>(data["data"].ToString());
                        PlayerUseEmoji(eu);
                        break;
                    }
                case "user_audio":
                    {
                        data = data["data"] as JsonObject;
                        int uid = int.Parse(data["uid"].ToString());
                        string audio = data["audio"].ToString();
                        PlayerSound(uid,audio);
                        break;
                    }
                case "roomOver":
                    {
                        RoomClosedNotify();
                        break;
                    }
                case "clubRoom_setRoomTime":
                    {
                        data = data["data"] as JsonObject;
                        int roomid = int.Parse(data["roomId"].ToString());
                        if (roomid == RoomInfo.RoomId)
                        {
                            if (RoomInfo.Status == 0)
                            {
                                string scene = PlayerPrefs.GetString("LastScene");
                                if (scene == "ClubScene")
                                {
                                    Transfer.Instance[TransferKey.RoomSwitch] = ClubRoomSwitch.Dz;
                                }
                                SceneManager.LoadScene(scene);
                            }
                            else
                            {
                                RoomInfo.RoomTime = 0;
                            }
                        }
                        break;
                    }
            }
        }
    }
    /// <summary>
    /// 玩家进入
    /// </summary>
    /// <param name="data"></param>
    void PlayerEnterNotify(JsonObject data)
    {
        BullPlayer p = new BullPlayer();
        p.UId = int.Parse(data["uid"].ToString());
        p.Nickname = data["nickname"].ToString();
        p.Bet = int.Parse(data["bet"].ToString());
        p.Avatar = data["avatar"].ToString();

        if (UpPlayerList.ContainsKey(p.UId))
        {
            UpPlayerList[p.UId] = p;
        }
        else
        {
            UpPlayerList.Add(p.UId, p);
        }
        Game.Instance.ShowTips(string.Format(LocalizationManager.Instance.GetText("7023"), p.UId));// "玩家ID:" + p.UId + "进入房间");
    }
    /// <summary>
    /// 玩家退出房间
    /// </summary>
    /// <param name="uid"></param>
    void PlayerExitNotify(int uid)
    {
        Game.Instance.ShowTips(string.Format(LocalizationManager.Instance.GetText("7024"), uid));// "玩家ID:" + uid + "退出房间");
        if (PlayerList.ContainsKey(uid))
        {
            BullPlayer p = PlayerList[uid];
            if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
            {
                string scene = PlayerPrefs.GetString("LastScene");
                if (scene == "ClubScene")
                {
                    Transfer.Instance[TransferKey.RoomSwitch] = ClubRoomSwitch.Bull;
                }
                SceneManager.LoadScene(scene);
            }
            else
            {
                BullSeatView s = SeatViews[p.Site];
                if (CurPlayerSeat == null)
                {
                    s.StandUp(0);
                }
                else
                {
                    s.StandUp(1);
                }
                s.SuitView.Clear();
                s.EndActionTimer();
                s.Player = null;
                OnSeatViews.Remove(s);
                PlayerList.Remove(uid);
            }
        }
        if (UpPlayerList.ContainsKey(uid))
        {
            BullPlayer p = UpPlayerList[uid];
            if (me.UId == uid)
            {
                string scene = PlayerPrefs.GetString("LastScene");
                if (scene == "ClubScene")
                {
                    Transfer.Instance[TransferKey.RoomSwitch] = ClubRoomSwitch.Bull;
                }
                SceneManager.LoadScene(scene);
            }
            else
            {
                UpPlayerList.Remove(uid);
            }
        }
    }
    /// <summary>
    /// 玩家站起
    /// </summary>
    /// <param name="uid"></param>
    void PlayerStandUpNotify(int uid,string type)
    {
        if (PlayerList.ContainsKey(uid))
        {
            BullPlayer p = PlayerList[uid];
            BullSeatView s = SeatViews[p.Site];

            if (string.IsNullOrEmpty(type))
            {
                if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
                {
                    ActionPanel.gameObject.SetActive(false);
                    Vector3 pos = CurPlayerSeat.transform.localPosition;
                    pos.x = 0;
                    CurPlayerSeat.SeatLocalMove(pos);
                    CurPlayerSuit.Clear();
                    CurPlayerSuit.gameObject.SetActive(false);
                    CurPlayerSeat = null;
                    RefreshSeatViews();
                }

                if (CurPlayerSeat == null)
                {
                    s.StandUp(0);
                }
                else
                {
                    s.StandUp(1);
                }
                s.SuitView.Clear();
                s.EndActionTimer();
                s.Player = null;
                OnSeatViews.Remove(s);
                PlayerList.Remove(uid);

                if (UpPlayerList.ContainsKey(uid))
                {
                    UpPlayerList[uid] = p;
                }
                else
                {
                    UpPlayerList.Add(uid, p);
                }
            }
            else
            {
                if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
                {
                    if (type == "betLess")
                    {
                        Game.Instance.ShowTips(LocalizationManager.Instance.GetText("214"));// ("筹码不足");
                    }
                }
                s.ChangeState(BullPlayerState.Look);
            }
        }
    }
    /// <summary>
    /// 玩家坐下
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="seat"></param>
    void PlayerSitDownNotify(int uid,BullPlayer player)
    {
        if (!UpPlayerList.ContainsKey(uid)) {
            UpPlayerList.Add(uid,player);
        }
        else
        {
            UpPlayerList[uid] = player;
        }
        if (Game.Instance.CurPlayer.Uid == uid)
        {
            ClearSuitViews();
            MaskView.gameObject.SetActive(true);
            AnimationBeforeSitdown(player.Site, () =>
            {
                BullPlayer p = UpPlayerList[uid];
                p.Bet = player.Bet;
                p.Site = player.Site;
                p.State = BullPlayerState.Wait;
                me = p;

                BullSeatView s = SeatViews[p.Site];
                s.Player = p;
                s.SitDown();
                s.SuitView = SuitViews[s.SuitPosIndex];

                OnSeatViews.Add(s);
                if (PlayerList.ContainsKey(p.UId))
                {
                    PlayerList[p.UId]= p;
                }
                else
                {
                    PlayerList.Add(p.UId, p);
                }
                UpPlayerList.Remove(uid);

                CurPlayerSeat = s;
                CurPlayerSuit.gameObject.SetActive(true);
                isRobot = false;
                CurPlayerSeat.RobotImg.gameObject.SetActive(isRobot);
                ResetSuitViews();
                RefreshSeatViews();
                MaskView.gameObject.SetActive(false);
            });
        }
        else
        {
            BullPlayer p = UpPlayerList[uid];
            p.Bet = player.Bet;
            p.Site = player.Site;
            p.State = BullPlayerState.Wait;

            BullSeatView s = SeatViews[p.Site];
            s.Player = p;
            s.SitDown();
            s.SuitView = SuitViews[s.SuitPosIndex];

            if (s.Player.Cards != null && s.Player.Cards.Length > 0)
            {
                if (curProcess == "step_putCard_COW")
                {
                    s.SuitView.ShowSuit(5, s.Player.Cards);
                }
                else
                {
                    s.SuitView.ShowSuit(s.Player.Cards.Length, s.Player.Cards);
                }
            }

            OnSeatViews.Add(s);
            PlayerList.Add(p.UId, p);
            UpPlayerList.Remove(uid);
            RefreshSeatViews();
        }
    }
    void AnimationBeforeSitdown(int site, Action callback = null)
    {
        if (SeatViews.Length > site)
        {
            BullSeatView targSeat = SeatViews[site];

            int all = SeatViews.Length;
            int ctNum = all / 2;
            int ptNum = Math.Abs(ctNum - targSeat.SeatPosIndex);
            if (targSeat.SeatPosIndex == ctNum)
            {
                targSeat.SuitPosIndex = ctNum;
                targSeat.SuitView = SuitViews[ctNum];
                targSeat.transform.localPosition = SeatPositions[ctNum].localPosition;
                targSeat.SeatTurnAnim(null, callback);
                for (int i = 0; i < SeatViews.Length; i++)
                {
                    BullSeatView s = SeatViews[i];
                    int fiNum = i;
                    if (s.Content.gameObject.activeSelf && i != site)
                    {
                        s.isSit = true;
                    }
                    s.SuitPosIndex = fiNum;
                    s.SuitView = SuitViews[fiNum];
                    s.SeatPosIndex = fiNum;
                    if (i != site)
                    {
                        s.SeatTurnAnim(null);
                    }
                }
            }
            else if (targSeat.SeatPosIndex < ctNum)
            {
                for (int i = 0; i < SeatViews.Length; i++)
                {
                    BullSeatView s = SeatViews[i];

                    Vector3[] wpts = new Vector3[ptNum];

                    int fiNum = (s.SeatPosIndex + ptNum) % all; 

                    for (int j = 1; j <= ptNum; j++)
                    {
                        int idx = (s.SeatPosIndex + j) % all;
                        wpts[j-1] = SeatPositions[idx].localPosition;
                    }

                    if (s.Content.gameObject.activeSelf && i != site)
                    {
                        s.isSit = true;
                    }
                    s.SuitPosIndex = fiNum;
                    s.SuitView = SuitViews[fiNum];
                    s.SeatPosIndex = fiNum;
                    if (i == SeatViews.Length - 1)
                    {
                        s.SeatTurnAnim(wpts, callback);
                    }
                    else
                    {
                        s.SeatTurnAnim(wpts);
                    }
                }
            }
            else
            {
                for (int i = 0; i < SeatViews.Length; i++)
                {
                    BullSeatView s = SeatViews[i];

                    Vector3[] wpts = new Vector3[ptNum];

                    int fiNum = (s.SeatPosIndex - ptNum + all) % all;

                    for (int j = 1; j <= ptNum; j++)
                    {
                        int idx = (s.SeatPosIndex - j +all) % all;
                        wpts[j - 1] = SeatPositions[idx].localPosition;
                    }

                    if (s.Content.gameObject.activeSelf && i != site)
                    {
                        s.isSit = true;
                    }
                    s.SuitPosIndex = fiNum;
                    s.SuitView = SuitViews[fiNum];
                    s.SeatPosIndex = fiNum;
                    if (i == SeatViews.Length - 1)
                    {
                        s.SeatTurnAnim(wpts, callback);
                    }
                    else
                    {
                        s.SeatTurnAnim(wpts);
                    }
                }
            }
        }
    }
    void ClearSuitViews() {
        if (SuitViews != null) {
            foreach (BullSuitView sv in SuitViews) {
                sv.Clear();
            }
        }
    }
    void ResetSuitViews() {
        if (OnSeatViews != null)
        {
            foreach (BullSeatView s in OnSeatViews)
            {
                s.SuitView = SuitViews[s.SuitPosIndex];
                if (CurPlayerSeat != null && s.Player.UId == CurPlayerSeat.Player.UId) {
                    continue;//跳过自己
                }
                if (s.Player.Cards != null && s.Player.Cards.Length > 0)
                {
                    if (curProcess == "step_putCard_COW")
                    {
                        s.SuitView.ShowSuit(5, s.Player.Cards);
                    }
                    else
                    {
                        s.SuitView.ShowSuit(s.Player.Cards.Length, s.Player.Cards);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 玩家购买筹码
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="bet"></param>
    void PlayerAddChipNotify(int uid, int bet)
    {
        BullPlayer p = null;
        if (PlayerList.ContainsKey(uid))
        {
            p = PlayerList[uid];
            p.Bet += bet;
            BullSeatView s = SeatViews[p.Site];
            s.ChipTxt.text = p.Bet.ToString();
        }
        if (UpPlayerList.ContainsKey(uid))
        {
            p = UpPlayerList[uid];
            p.Bet += bet;
        }
        if (me.UId == uid)
        {
            ClubChips -= bet;
            if (RoomInfo.IsPublic == 1)
            {
                Game.Instance.CurPlayer.Gold = ClubChips;
            }
            if (wantSit >= 0)
            {
                int seat = wantSit;
                SitDown(seat);
                wantSit = -1;
            }
        }
    }

    void PlayerUseEmoji(EmojiUse eu) {
        if (PlayerList.ContainsKey(eu.FromUId) && PlayerList.ContainsKey(eu.ToUId)) {
            BullPlayer fromPlayer = PlayerList[eu.FromUId];
            BullPlayer toPlayer = PlayerList[eu.ToUId];

            BullSeatView fromSeat = SeatViews[fromPlayer.Site];
            BullSeatView toSeat = SeatViews[toPlayer.Site];

            EmojiItem ei = Instantiate(Resources.Load<EmojiItem>("Prefabs/Emoji/"+eu.Emoji));
            ei.transform.SetParent(fromSeat.transform);
            (ei.transform as RectTransform).anchoredPosition3D = Vector3.zero;

            ei.TargetRect = toSeat.transform as RectTransform;
            ei.Show();
        }
    }

    void PlayerSound(int uid,string audio)
    {
        if (uid == Game.Instance.CurPlayer.Uid)
            return;

        AwsS3Service.Instance.AsyncDownloadObject(audio, (rsp) =>
        {
                if (rsp.IsOk)
                {
                    AudioClip clip = VoiceRecorder.Instance.Read(rsp.data);
                    if (clip)
                    {
                        VoiceRecorder.Instance.PlayRecord(clip);
                    }
                }
        });
    }
    /// <summary>
    /// 牌局开始
    /// </summary>
    void GambleStartNotify()
    {
        //关闭等待开始界面
        StartBtn.gameObject.SetActive(false);
        PublicPanel.gameObject.SetActive(true);
        WaitPanel.gameObject.SetActive(false);

        Game.Instance.AudioMgr.PlayAudioEffect("start");

        //初始化数据
        DataInit();
    }

    void NextRoundNotify(string round) {
        curProcess = round;
        Debug.Log("-------next-round:--"+round);

        switch (curProcess)
        {
            case "step_callBanker_SG":
                {
                    //玩家进行标庄
                    ProcessImg.gameObject.SetActive(true);
                    ProcessTxt.text = LocalizationManager.Instance.GetText("8003");// "三公叫庄";

                    if (CurPlayerSeat != null && CurPlayerSeat.isSit)
                    {
                        if (isRobot)
                        {
                            CallSGBanker(0);
                        }
                        else
                        {
                            ActiveActionPanel();
                        }
                        MyClock.Show(10);
                    }
                    break;
                }
            case "step_showCard_SG":
                {
                    if (CurPlayerSeat != null && CurPlayerSeat.isSit)
                    {
                        MyClock.Show(20);
                    }
                    ProcessTxt.text = LocalizationManager.Instance.GetText("8004");// "等待玩家搓牌";
                    break;
                }
            case "step_callBanker_COW":
                {
                    ProcessTxt.text = LocalizationManager.Instance.GetText("8005");// "牛牛叫庄";
                    if (CurPlayerSeat != null && CurPlayerSeat.isSit)
                    {
                        MyClock.Show(10);
                        if (isRobot)
                        {
                            CallNNBanker(0);
                        }
                        else
                        {
                            ActiveActionPanel();
                        }
                    }
                    break;
                }
            case "step_putCard_COW":
                {
                    if (CurPlayerSeat != null && CurPlayerSeat.isSit)
                    {
                        MyClock.Show(60);
                    }
                    ProcessTxt.text = LocalizationManager.Instance.GetText("8006");// "等待玩家排牌";
                    break;
                }
        }
    }

    /// <summary>
    /// 三公叫庄结果
    /// </summary>
    /// <param name="preBanker"></param>
    /// <param name="bankId"></param>
    /// <param name="odds"></param>
    void ShowSGBankerNotify(int[] preBanker,int bankId,int odds)
    {
        MyClock.Close();
        ActionPanel.gameObject.SetActive(false);
        if (preBanker != null) {
            StartCoroutine(IEPreBanker(preBanker,bankId,odds));
        }
    }
    /// <summary>
    /// 分发玩家三公2张牌
    /// </summary>
    /// <param name="showCards"></param>
    void SendPlayerSGPokerNotify(Dictionary<string,int[]> showCards) {
        if (showCards != null)
        {
            StartCoroutine(IESendPlayerSGPokerNotify(showCards));
        }
    }
    IEnumerator IESendPlayerSGPokerNotify(Dictionary<string, int[]> showCards)
    {
        yield return new WaitForSeconds(0.01f);
        Game.Instance.AudioMgr.PlayAudioEffect("fapai");
        foreach (string uid in showCards.Keys)
        {
            if (PlayerList.ContainsKey(int.Parse(uid)))
            {
                int[] cards = showCards[uid];
                if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == int.Parse(uid))
                {
                    CurPlayerSuit.gameObject.SetActive(true);
                    CurPlayerSuit.ShowSuit(3, cards);
                }
                else
                {
                    BullPlayer p = PlayerList[int.Parse(uid)];
                    BullSeatView s = SeatViews[p.Site];
                    s.SuitView.ShowSuit(3, cards);
                }
            }
        }
    }
    /// <summary>
    /// 发玩家三公3张牌
    /// </summary>
    /// <param name="cards"></param>
    void ShowSelfSGPokerNotify(int[] cards)
    {
        curProcess = "step_callBanker_SG";
        ProcessTxt.text = LocalizationManager.Instance.GetText("8004");// "等待玩家搓牌";
        if (CurPlayerSeat != null)
        {
            CurPlayerSeat.Player.Cards = cards;
            BullOpenView.gameObject.SetActive(true);
            BullOpenView.SetRubView(cards.Last());
            if (isRobot)
            {
                SGOpenClick();
            }
        }
    }
    /// <summary>
    /// 显示三公结果
    /// </summary>
    void ShowSGResultNotify(string result) {
        ProcessTxt.text = LocalizationManager.Instance.GetText("8009");// "三公结算";
        MyClock.Close();
        BullOpenView.gameObject.SetActive(false);
        Dictionary<int,SGResult> dic = JsonConvert.DeserializeObject<Dictionary<int, SGResult>>(result);
        if (dic != null) {
            foreach (int uid in dic.Keys) {
                if (PlayerList.ContainsKey(uid)) {
                    SGResult sgr = dic[uid];
                    BullPlayer p = PlayerList[uid];
                    BullSeatView s = SeatViews[p.Site];
                    if (sgr.IsBanker == 1)
                    {
                        RoomInfo.BankerSite = p.Site;
                        StartCoroutine(IEShowSGResult(dic));
                    }
                    else {
                        if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
                        {
                            CurPlayerSuit.ShowPokerInSuit(sgr.ThirdCard, 2);
                            CurPlayerSuit.ShowSGResultImage(sgr.CardType, sgr.CardPoint);
                        }
                        else
                        {
                            s.SuitView.ShowPokerInSuit(sgr.ThirdCard, 2);
                            s.SuitView.ShowSGResultImage(sgr.CardType, sgr.CardPoint);
                        }
                    }
                }
            }
        }
    }
    IEnumerator IEShowSGResult(Dictionary<int, SGResult> dic)
    {
        yield return new WaitForSeconds(2.0f);
        foreach (int uid in dic.Keys)
        {
            if (PlayerList.ContainsKey(uid))
            {
                SGResult sgr = dic[uid];
                BullPlayer p = PlayerList[uid];
                BullSeatView s = SeatViews[p.Site];
                if (sgr.IsBanker == 1)
                {
                    s.DoSetBank(0);
                    if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
                    {
                        CurPlayerSuit.ShowPokerInSuit(sgr.ThirdCard, 2);
                        CurPlayerSuit.ShowSGResultImage(sgr.CardType, sgr.CardPoint);
                        if (sgr.WinBet - sgr.LoseBet > 0)
                        {
                            Game.Instance.AudioMgr.PlayAudioEffect("win");
                        }
                        if (sgr.WinBet - sgr.LoseBet < 0)
                        {
                            Game.Instance.AudioMgr.PlayAudioEffect("lose");
                        }
                    }
                    else
                    {
                        s.SuitView.ShowPokerInSuit(sgr.ThirdCard, 2);
                        s.SuitView.ShowSGResultImage(sgr.CardType, sgr.CardPoint);
                    }
                }
                s.Player.Bet = sgr.RestBet;
                s.ChipTxt.text = sgr.RestBet.ToString();
                s.ShowResult(sgr.WinBet - sgr.LoseBet);
            }
            if (UpPlayerList.ContainsKey(uid))
            {
                SGResult sgr = dic[uid];
                BullPlayer p = UpPlayerList[uid];
                p.Bet = sgr.RestBet;
            }
        }
    }
    /// <summary>
    /// 牛牛标庄结果
    /// </summary>
    /// <param name="preBanker"></param>
    /// <param name="bankId"></param>
    /// <param name="odds"></param>
    void ShowNNBankerNotify(int[] preBanker, int bankId, int odds)
    {
        MyClock.Close();
        ActionPanel.gameObject.SetActive(false);
        if (preBanker != null)
        {
            StartCoroutine(IEPreBanker(preBanker, bankId, odds));
        }
    }
    /// <summary>
    /// 牛牛发牌
    /// </summary>
    /// <param name="cards"></param>
    void SendSelfNNPokerNotify(int[] cards)
    {
        StartCoroutine(IESendSelfNNPokerNotify(cards));
    }
    IEnumerator IESendSelfNNPokerNotify(int[] cards)
    {
        yield return new WaitForSeconds(0.01f);
        ActionPanel.gameObject.SetActive(false);
        ProcessTxt.text = LocalizationManager.Instance.GetText("8006");// "等待玩家排牌";
        foreach (int uid in PlayerList.Keys)
        {
            if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
            {
                List<int> tmp = CurPlayerSeat.Player.Cards.ToList();
                for (int i = 0; i < cards.Length; i++)
                {
                    CurPlayerSuit.DealCard(0,0.1f);
                }
                tmp.AddRange(cards);
                CurPlayerSeat.Player.Cards = tmp.ToArray();
                BullDragView.PlayerSuitView = CurPlayerSuit;
                BullDragView.SetCards(CurPlayerSeat.Player.Cards);
                if (isRobot)
                {
                    NNDragCommitClick();
                }
            }
            else
            {
                BullPlayer p = PlayerList[uid];
                BullSeatView s = SeatViews[p.Site];
                s.SuitView.AddSuitCards(2, null);
            }
        }
    }

    void ShowSelfNNPokerNotify(string cardType,int cardPoint) {
        if (curProcess == "step_putCard_COW")
        {
            CurPlayerSuit.ShowNNResultImage(cardType, cardPoint);
        }
    }

    /// <summary>
    /// 牛牛结算
    /// </summary>
    void ShowNNResultNotify(string result)
    {
        MyClock.Close();
        ProcessTxt.text = LocalizationManager.Instance.GetText("8010");// "牛牛结算";
        BullDragView.Close();
        Dictionary<int, NNResult> dic = JsonConvert.DeserializeObject<Dictionary<int, NNResult>>(result.ToString());
        if (dic != null)
        {
            foreach (int uid in dic.Keys)
            {
                if (PlayerList.ContainsKey(uid))
                {
                    NNResult nnr = dic[uid];
                    BullPlayer p = PlayerList[uid];
                    BullSeatView s = SeatViews[p.Site];
                    if (nnr.IsBanker == 1)
                    {
                        RoomInfo.BankerSite = p.Site;
                        StartCoroutine(IEShowNNResult(dic));
                    }
                    else
                    {
                        if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
                        {
                            CurPlayerSuit.Clear();
                            CurPlayerSuit.gameObject.SetActive(true);
                            CurPlayerSuit.ShowSuit(5, nnr.Cards);
                        }
                        else
                        {
                            s.SuitView.Clear();
                            s.SuitView.ShowSuit(5, nnr.Cards);
                        }
                    }
                }
            }
            StartCoroutine(DataClear(6.6f));
        }
    }
    IEnumerator IEShowNNResult(Dictionary<int, NNResult> dic)
    {
        yield return new WaitForSeconds(2.0f);

        foreach (int uid in dic.Keys)
        {
            if (PlayerList.ContainsKey(uid))
            {
                NNResult nnr = dic[uid];
                BullPlayer p = PlayerList[uid];
                BullSeatView s = SeatViews[p.Site];
                if (nnr.IsBanker == 1)
                {
                    RoomInfo.BankerSite = p.Site;
                    if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
                    {
                        CurPlayerSuit.Clear();
                        CurPlayerSuit.gameObject.SetActive(true);
                        CurPlayerSuit.ShowSuit(5, nnr.Cards);
                    }
                    else
                    {
                        s.SuitView.Clear();
                        s.SuitView.ShowSuit(5, nnr.Cards);
                    }
                }
                if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
                {
                    CurPlayerSuit.ShowNNResultImage(nnr.CardType, nnr.CardPoint);
                    if (nnr.WinBet - nnr.LoseBet > 0)
                    {
                        Game.Instance.AudioMgr.PlayAudioEffect("win");
                    }
                    if (nnr.WinBet - nnr.LoseBet < 0)
                    {
                        Game.Instance.AudioMgr.PlayAudioEffect("lose");
                    }
                }
                else
                {
                    s.SuitView.ShowNNResultImage(nnr.CardType, nnr.CardPoint);
                }
                s.Player.Bet = nnr.RestBet;
                s.ChipTxt.text = nnr.RestBet.ToString();
                s.ShowResult(nnr.WinBet - nnr.LoseBet);
            }
            if (UpPlayerList.ContainsKey(uid))
            {
                NNResult nnr = dic[uid];
                BullPlayer p = UpPlayerList[uid];
                p.Bet = nnr.RestBet;
            }
        }
    }

    void DataInit()
    {
        isPlaying = true;

        MaskView.gameObject.SetActive(false);
        BullSeatView bs = SeatViews[RoomInfo.BankerSite];
        bs.SetBanker(false);
        if (SeatViews != null)
        {
            for (int i = 0; i < SeatViews.Length; i++)
            {
                BullSeatView sx = SeatViews[i];
                if (sx.SuitView != null)
                {
                    sx.SuitView.Clear();
                }
                if (sx.Player != null)
                {
                    sx.ChangeState(BullPlayerState.Normal);
                }
            }
        }
    }
    IEnumerator DataClear(float time)
    {
        yield return new WaitForSeconds(time);
        //预约退出
        if (wantExit)
        {
            ExitRoom();
        }
        if (SeatViews != null)
        {
            foreach (BullSeatView s in SeatViews)
            {
                if (s.SuitView != null)
                {
                    s.SuitView.Clear();
                }
            }
        }

        //关闭界面
        if (RoomInfo.Status == 1)
        {
            StartBtn.gameObject.SetActive(false);
            PublicPanel.gameObject.SetActive(true);
            WaitPanel.gameObject.SetActive(false);
        }
        MaskView.gameObject.SetActive(false);
        ActionPanel.Close();
        BullOpenView.gameObject.SetActive(false);
        BullDragView.gameObject.SetActive(false);

        curProcess = "wait";
        ProcessImg.gameObject.SetActive(RoomInfo.Status == 1);
        ProcessTxt.text = LocalizationManager.Instance.GetText("7025");// "等待牌局开始";
        CurPlayerSuit.Clear();
        isPlaying = false;
        BullSeatView bs = SeatViews[RoomInfo.BankerSite];
        bs.SetBanker(false);
        if (SeatViews != null)
        {
            for (int i = 0; i < SeatViews.Length; i++)
            {
                BullSeatView sx = SeatViews[i];
                if (sx.Player != null)
                {
                    if (sx.Player.State == BullPlayerState.Look || !PlayerList.ContainsKey(sx.Player.UId))
                    {
                        PlayerStandUpNotify(sx.Player.UId, "");
                    }
                    else if (sx.Player.State == BullPlayerState.PreStandUp) {
                        StandUp();
                    }
                    else
                    {
                        sx.ChangeState(BullPlayerState.Normal);
                    }
                }
            }
        }
        GetRestTime();
    }

    void GetRestTime()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("roomId", RoomInfo.RoomId.ToString());
        TexasApi.GetGameRoomTime(param, (result,err) =>
        {
            if (string.IsNullOrEmpty(err)) {
                RoomInfo.RoomTime = result;
            }
        });
    }

    void RoomClosedNotify()
    {
        if (FinalStatPanel.gameObject.activeSelf) {
            return;
        }
        StartCoroutine(RoomClosed());
    }
    IEnumerator RoomClosed() {
        yield return new WaitForSeconds(3.0f);
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("roomId", RoomInfo.RoomId.ToString());
        TexasApi.GetGameRoomStat(param, (resp, error) =>
        {
            if (error == null)
            {
                FinalStatPanel.gameObject.SetActive(true);
                FinalStatPanel.InitView(resp);
            }
            else
            {
                Game.Instance.ShowTips(error);
            }
        });
    }
    #endregion
    #endregion

    #region UI操作
    void RefreshSeatViews()
    {
        if (SeatViews != null)
        {
            for (int i = 0; i < SeatViews.Length; i++)
            {
                BullSeatView sx = SeatViews[i];
                if (!sx.isSit)
                {
                    if (CurPlayerSeat == null)
                    {
                        sx.StandUp(0);
                    }
                    else
                    {
                        sx.StandUp(1);
                    }
                }
            }
        }
    }
    public void OnStartBtnClick()
    {
        StartGame();
    }
    public void OnMenuBtnClick()
    {
        MenuPanel.gameObject.SetActive(true);
    }
    public void OnMenuStandUpBtnClick()
    {
        if (CurPlayerSeat != null)
        {
            if (isPlaying)
            {
                CurPlayerSeat.ChangeState(BullPlayerState.PreStandUp);
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("8060"));
            }
            else
            {
                StandUp();
            }
        }
        MenuPanel.gameObject.SetActive(false);
    }
    void BuyChip()
    {
        MenuPanel.gameObject.SetActive(false);
        if (RoomInfo.MinBet <= ClubChips)
        {
            BuyChipView.CurChipTxt.text = me.Bet.ToString();
            BuyChipView.BuyChipTxt.text = RoomInfo.MinBet.ToString();
            BuyChipView.ChipSlider.minValue = RoomInfo.MinBet;
            BuyChipView.ChipSlider.maxValue = RoomInfo.MaxBet > ClubChips ? ClubChips : RoomInfo.MaxBet;
            BuyChipView.ChipSlider.value = RoomInfo.MinBet;
            BuyChipView.gameObject.SetActive(true);
        }
        else
        {
            if (RoomInfo.IsPublic == 1)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("7026"));// 公共房间直接提示不够钱"钱不够买入");
                return;
            }
            int clubId = int.Parse(Transfer.Instance[TransferKey.ClubId].ToString());
            ClubApi.GetClubDetail(clubId, (result) =>
            {
                if (result.IsOk)
                {
                    ClubChips = (int)result.data.coin;
                    if (RoomInfo.MinBet <= ClubChips)
                    {
                        BuyChipView.CurChipTxt.text = me.Bet.ToString();
                        BuyChipView.BuyChipTxt.text = RoomInfo.MinBet.ToString();
                        BuyChipView.ChipSlider.minValue = RoomInfo.MinBet;
                        BuyChipView.ChipSlider.maxValue = RoomInfo.MaxBet > ClubChips ? ClubChips : RoomInfo.MaxBet;
                        BuyChipView.ChipSlider.value = RoomInfo.MinBet;
                        BuyChipView.gameObject.SetActive(true);
                    }
                    else
                    {
                        Game.Instance.ShowTips(LocalizationManager.Instance.GetText("7026"));// "钱不够买入");
                    }
                }
                else
                {
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("7026"));// "钱不够买入");
                }
            }, false);


        }
    }
    public void OnMenuBuyChipBtnClick()
    {
        BuyChipView.CurChipTxt.text = me.Bet.ToString();
        BuyChipView.BuyChipTxt.text = RoomInfo.MinBet.ToString();
        BuyChipView.ChipSlider.minValue = RoomInfo.MinBet;
        BuyChipView.ChipSlider.maxValue = RoomInfo.MaxBet > ClubChips ? ClubChips : RoomInfo.MaxBet;
        BuyChipView.ChipSlider.value = RoomInfo.MinBet;
        BuyChipView.gameObject.SetActive(true);
    }
    public void OnMenuSettingBtnClick()
    {
        MenuPanel.gameObject.SetActive(false);
        if (CurPlayerSeat != null && CurPlayerSeat.isSit)
        {
            isRobot = !isRobot;
            CurPlayerSeat.RobotImg.gameObject.SetActive(isRobot);
        }
    }
    public void OnMenuIntroBtnClick()
    {
        MenuPanel.gameObject.SetActive(false);
        IntroPanel.Show();
    }
    public void OnMenuExitRoomBtnClick()
    {
        MenuPanel.gameObject.SetActive(false);
        if (!isPlaying)
        {
            MenuPanel.gameObject.SetActive(false);
            ExitRoom();
        }
        else {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("8061"));
            wantExit = true;
        }
    }
    public void OnMenuCloseRoomBtnClick()
    {
        MenuPanel.gameObject.SetActive(false);
        CloseRoom();
    }
    public void OnAchieveBtnClick()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("roomId", RoomInfo.RoomId.ToString());
        TexasApi.GetGameRoomLiveData(param,RoomInfo.IsPublic==1, (resp, error) =>
        {
            if (error == null)
            {
                LiveRecordPanel.gameObject.SetActive(true);
                LiveRecordPanel.InitBullView(resp, UpPlayerList);
            }
            else
            {
                Game.Instance.ShowTips(error);
            }
        });
    }
    public void OnHistoryBtnClick()
    {
        HistoryPanel.RoomInfo = RoomInfo;
        HistoryPanel.Show();
    }

    public void OnShopBtnClick() { }

    public void ActiveActionPanel()
    {
        if (RoomInfo.SeatNum == 5)
        {
            if (CurPlayerSeat.Player.Bet < RoomInfo.BaseBet * 20)
            {
                ActionPanel.gameObject.SetActive(false);
                Game.Instance.ShowTips(""+LocalizationManager.Instance.GetText("8062"));
                PlayerBankClick(0);
            }
            else
            {
                ActionPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            if (CurPlayerSeat.Player.Bet < RoomInfo.BaseBet * 30)
            {
                ActionPanel.gameObject.SetActive(false);
                Game.Instance.ShowTips("" + LocalizationManager.Instance.GetText("8062"));
                PlayerBankClick(0);
            }
            else
            {
                ActionPanel.gameObject.SetActive(true);
            }
        }
    }
    public void PlayerBankClick(int mul)
    {
        if (curProcess == "step_callBanker_SG")
        {
            CallSGBanker(mul);
        }
        else if (curProcess == "step_callBanker_COW")
        {
            CallNNBanker(mul);
        }
    }

    public void SGOpenClick() {
        BullOpenView.gameObject.SetActive(false);
        Game.Instance.AudioMgr.PlayAudioEffect("fanpai");
        ShowSGCards();
    }
    public void NNDragCommitClick() {
        CurPlayerSeat.Player.Cards = BullDragView.GetFinalSuit();
        BullDragView.Close();
        ShowNNCards(CurPlayerSeat.Player.Cards,1);
    }

    #region UI动画
    IEnumerator IEPreBanker(int[] preBanker,int bankId,int odds)
    {
        for (int m = 0; m < 2; m++)
        {
            for (int i = 0; i < preBanker.Length; i++)
            {
                int uid = preBanker[i];
                if (PlayerList.ContainsKey(uid))
                {
                    BullPlayer p = PlayerList[uid];
                    BullSeatView s = SeatViews[p.Site];
                    s.TimerImg.fillAmount = 1;
                    yield return new WaitForSeconds(0.1f);
                    s.TimerImg.fillAmount = 0;
                }
            }
        }

        if (PlayerList.ContainsKey(bankId))
        {
            BullPlayer p = PlayerList[bankId];
            BullSeatView s = SeatViews[p.Site];
            s.DoSetBank(odds);
            RoomInfo.BankerSite = p.Site;
        }
    }
    #endregion
    #endregion
}
