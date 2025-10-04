using Newtonsoft.Json;
using RT;
using SimpleJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TexasGameOp {
    EnterRoomOp = 0,
    OutRoomOp = 1,
    SitDownOp = 2,
    StandUpOp = 3,
    StartGameOp = 4,
    PreAddBetOp = 5,
    BetOp = 6,
    CallBetOp = 7,
    CheckBetOp = 8,
    AllinOp = 9,
    FoldBetOp = 10,
    Reconnect = 12,
    OpenCard = 13,
    GameNotify = 99,
    BackRoom = 100
}

public class TexasTableMgr : MonoBehaviour, IPointerClickHandler
{
    private PokerItem pokerTpl;
    GameCmdMgr CmdMgr;

    List<PokerItem> playersPokers;
    List<PokerItem> publicPokers;

    public RectTransform PokerPoolPanel;
    public TexasSeatView[] SeatViews;
    public RectTransform[] SeatPositions;
    public List<TexasSeatView> OnSeatViews;

    public RectTransform PublicPanel;
    public RectTransform WaitPanel;
    public RectTransform PublicPokerPanel;
    public RectTransform MainChipPanel;
    public Text MainChipPoolTxt;
    public Text CurPotTxt;
    public Button StartBtn;
	public Text InfoTxt;

    //房间操作界面
    public TexasBuyChipView BuyChipView;
    public TexasHistoryView HistoryPanel;
    public TexasLiveView LiveRecordPanel;
    public TexasFinalStatView FinalStatPanel;
    public TexasMenuView MenuPanel;

    public Button MenuBtn;
    public Button AchieveBtn;
    public Button HistoryBtn;
    public Button VoiceBtn;
    public Button ShopBtn;
    public Button ShowPublicBtn;

    public RectTransform MaskView;

    //游戏操作界面
    public TexasActionView ActionView;
    public GamePlayerView PlayerInfoView;
    
    public Image ProcessImg;
    public Text ProcessTxt;

    //进入房间初始化信息
    public TexasRoom RoomInfo;

    public Dictionary<int, TexasPlayer> PlayerList;
    public Dictionary<int, TexasPlayer> UpPlayerList;

    //当前玩家信息
    TexasPlayer me;
    int ClubChips;//当前玩家在这个俱乐部的所有筹码
    
    //自己玩家位置
    TexasSeatView CurPlayerSeat;
    //当前操作的玩家位置
    TexasSeatView ActionPlayerSeat;

    //当前操作的最高筹码
    int curBet = 0;
    int MaxBlind = 0;
    int wantSit = -1;
    //机器人
    bool isRobot = false;
    //游戏是否进行中
    bool isPlaying = false;

    //底池筹码
    int mainPoolBet = 0;
    int opid = 0;
    int[] finalCards;

    //圈名
    string roundTxt = "";

    //pomelo actions
    Queue<JsonObject> actionQueue;
    string action = "operation";

    //防作弊
    List<Color> txtColors;
    Dictionary<Color, List<TexasPlayer>> colorPlayers;

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
        playersPokers = new List<PokerItem>();
        publicPokers = new List<PokerItem>();
        OnSeatViews = new List<TexasSeatView>();
        txtColors = new List<Color>();
        txtColors.Add(Color.red);
        txtColors.Add(Color.cyan);
        txtColors.Add(Color.blue);
        txtColors.Add(Color.yellow);
        txtColors.Add(Color.white);
        colorPlayers = new Dictionary<Color, List<TexasPlayer>>();
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

    void RefreshNickColor() {
        if (PlayerList != null)
        {
            colorPlayers.Clear();
            List<TexasPlayer> tmp = new List<TexasPlayer>();
            List<TexasPlayer> tmpWhite = new List<TexasPlayer>();
            List<TexasPlayer> tmpLast = new List<TexasPlayer>();

            foreach (int k in PlayerList.Keys)
            {
                TexasPlayer p = PlayerList[k];
                tmp.Add(p);
            }
            for (int i = 0; i < tmp.Count; i++)
            {
                TexasPlayer p1 = tmp[i];
                if (tmpLast.Contains(p1))
                {
                    continue;
                }
                else
                {
                    tmpLast.Clear();
                }
                for (int j = i; j < tmp.Count; j++)
                {
                    TexasPlayer p2 = tmp[j];
                    double lat1 = string.IsNullOrEmpty(p1.Lat) ? 0 : double.Parse(p1.Lat);
                    double lng1 = string.IsNullOrEmpty(p1.Lng) ? 0 : double.Parse(p1.Lng);
                    double lat2 = string.IsNullOrEmpty(p2.Lat) ? 0 : double.Parse(p2.Lat);
                    double lng2 = string.IsNullOrEmpty(p2.Lng) ? 0 : double.Parse(p2.Lng);

                    if (p1.UId != p2.UId && Location.Instance.Distance(lat1, lng1, lat2, lng2) < 50) {
                        if (!tmpLast.Contains(p1)) {
                            tmpLast.Add(p1);
                        }
                        tmpLast.Add(p2);
                    }
                }
                if (tmpLast.Count > 0)
                {
                    foreach (Color c in txtColors)
                    {
                        if (!colorPlayers.ContainsKey(c))
                        {
                            colorPlayers.Add(c, tmpLast);
                            break;
                        }
                    }
                }
                else
                {
                    tmpWhite.Add(p1);
                }
            }
            if (colorPlayers.ContainsKey(Color.white))
            {
                colorPlayers[Color.white] = tmpWhite;
            }
            else
            {
                colorPlayers.Add(Color.white,tmpWhite);
            }
            if (colorPlayers != null) {
                foreach (Color cc in colorPlayers.Keys)
                {
                    List<TexasPlayer> tmp1 = colorPlayers[cc];
                    if (tmp1 != null) {
                        foreach (TexasPlayer p in tmp1)
                        {
                            TexasSeatView s = SeatViews[p.Site];
                            s.NickTxt.color = cc;
                        }
                    }
                }
            }

        }
    }

    void Start()
    {
        Array.Reverse(SeatViews);
        Array.Reverse(SeatPositions);

        pokerTpl = Resources.Load<PokerItem>("Prefabs/Poker/PokerItem");
        CmdMgr = GetComponent<GameCmdMgr>();

        JsonObject roomInfo = Transfer.Instance[TransferKey.RoomInfo] as JsonObject;
        if (roomInfo != null)
        {
            GetRoomInfoDone(roomInfo);
        }
        else {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1012"));// ("加载信息失败，请返回重试");
        }

        StartBtn.onClick.AddListener(delegate {
            OnStartBtnClick();
        });
        ActionView.ActionFoldBtn.onClick.AddListener(delegate {
            OnFoldBtnClick();
        });
        ActionView.ActionCheckBtn.onClick.AddListener(delegate {
            OnCheckBtnClick();
        });
        ActionView.ActionCallBtn.onClick.AddListener(delegate {
            OnCallBtnClick();
        });
        ActionView.ActionAllinBtn.onClick.AddListener(delegate {
            OnAllinBtnClick();
        });
        ActionView.ActionRaiseBtn.onClick.AddListener(delegate {
            OnRaiseBtnClick();
        });
        ActionView.ShowRaiseBtn.onClick.AddListener(delegate {
            OnShowRaiseBtnClick();
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
        MenuPanel.MenuHangOutBtn.onClick.AddListener(delegate () {
            OnMenuHangOutBtnClick();
        });
        MenuPanel.MenuExitRoomBtn.onClick.AddListener(delegate () {
            OnMenuExitRoomBtnClick();
        });
        MenuPanel.MenuCloseRoomBtn.onClick.AddListener(delegate () {
            OnMenuCloseRoomBtnClick();
        });
        ShowPublicBtn.onClick.AddListener(delegate () {
            OnShowPublicBtnClick();
        });

        BuyChipView.BuyBtn.onClick.AddListener(delegate {
            BuyChipView.gameObject.SetActive(false);
            PreAddBet(int.Parse(BuyChipView.BuyChipTxt.text));
        });
    }

    void GetRoomInfoDone(JsonObject roomInfo)
    {
        RoomInfo = JsonConvert.DeserializeObject<TexasRoom>(roomInfo.ToString());
        RoomInfo.BankerSite = 0;
        SetSeatViews();

        CurPlayerSeat = null;
        me = null;

        MaxBlind = int.Parse(roomInfo["blindBet"].ToString());
        ClubChips = int.Parse(roomInfo["clubChips"].ToString());

        InfoTxt.text = roomInfo["title"].ToString() + "  " + string.Format(LocalizationManager.Instance.GetText("5903"), MaxBlind / 2, MaxBlind);//"  盲注" + MaxBlind/2 + "/" + MaxBlind;

        JsonObject players = roomInfo["players"] as JsonObject;
        JsonObject lookers = roomInfo["upPlayers"] as JsonObject;
        if (players != null && !string.IsNullOrEmpty(players.ToString()))
        {
            PlayerList = JsonConvert.DeserializeObject<Dictionary<int, TexasPlayer>>(players.ToString());
        }
        else {
            PlayerList = new Dictionary<int, TexasPlayer>();
        }
		if (lookers != null && !string.IsNullOrEmpty(lookers.ToString()))
        {
            UpPlayerList = JsonConvert.DeserializeObject<Dictionary<int, TexasPlayer>>(lookers.ToString());
        }
        else
        {
            UpPlayerList = new Dictionary<int, TexasPlayer>();
        }
        InitView();
     }
    void SetSeatViews() {

        if (SeatViews.Length > 0 && SeatViews.Length == 9)
        {
            if (RoomInfo.SeatNum == 2)
            {
                Vector3 newPos = (SeatViews[0].gameObject.transform as RectTransform).anchoredPosition3D;
                newPos.x = 0;
                (SeatViews[0].gameObject.transform as RectTransform).anchoredPosition3D = newPos;
                (SeatPositions[0].gameObject.transform as RectTransform).anchoredPosition3D = newPos;

                SeatViews[1].gameObject.SetActive(false);
                SeatPositions[1].gameObject.SetActive(false);

                SeatViews[2].gameObject.SetActive(false);
                SeatPositions[2].gameObject.SetActive(false);

                SeatViews[3].gameObject.SetActive(false);
                SeatPositions[3].gameObject.SetActive(false);

                SeatViews[5].gameObject.SetActive(false);
                SeatPositions[5].gameObject.SetActive(false);

                SeatViews[6].gameObject.SetActive(false);
                SeatPositions[6].gameObject.SetActive(false);

                SeatViews[7].gameObject.SetActive(false);
                SeatPositions[7].gameObject.SetActive(false);

                SeatViews[8].gameObject.SetActive(false);
                SeatPositions[8].gameObject.SetActive(false);

                List<TexasSeatView> tmp2 = SeatViews.ToList();
                tmp2.RemoveAt(8);
                tmp2.RemoveAt(7);
                tmp2.RemoveAt(6);
                tmp2.RemoveAt(5);
                tmp2.RemoveAt(3);
                tmp2.RemoveAt(2);
                tmp2.RemoveAt(1);
                SeatViews = tmp2.ToArray();

                List<RectTransform> tmp3 = SeatPositions.ToList();
                tmp3.RemoveAt(8);
                tmp3.RemoveAt(7);
                tmp3.RemoveAt(6);
                tmp3.RemoveAt(5);
                tmp3.RemoveAt(3);
                tmp3.RemoveAt(2);
                tmp3.RemoveAt(1);
                SeatPositions = tmp3.ToArray();
            }
            if (RoomInfo.SeatNum == 6)
            {
                Vector3 newPos = (SeatViews[0].gameObject.transform as RectTransform).anchoredPosition3D;
                newPos.x = 0;
                (SeatViews[0].gameObject.transform as RectTransform).anchoredPosition3D = newPos;
                (SeatPositions[0].gameObject.transform as RectTransform).anchoredPosition3D = newPos;
                

                SeatViews[2].gameObject.SetActive(false);
                SeatPositions[2].gameObject.SetActive(false);

                SeatViews[6].gameObject.SetActive(false);
                SeatPositions[6].gameObject.SetActive(false);

                SeatViews[8].gameObject.SetActive(false);
                SeatPositions[8].gameObject.SetActive(false);

                List<TexasSeatView> tmp2 = SeatViews.ToList();
                tmp2.RemoveAt(8);
                tmp2.RemoveAt(6);
                tmp2.RemoveAt(2);
                SeatViews = tmp2.ToArray();

                List<RectTransform> tmp3 = SeatPositions.ToList();
                tmp3.RemoveAt(8);
                tmp3.RemoveAt(6);
                tmp3.RemoveAt(2);
                SeatPositions = tmp3.ToArray();
            }
            for (int i = 0; i < SeatViews.Length; i++)
            {
                TexasSeatView s = SeatViews[i];
                s.SeatNo = i;
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
                TexasPlayer p = PlayerList[k];
                TexasSeatView s = SeatViews[p.Site];
                s.Player = p;
                s.SitDown();
                OnSeatViews.Add(s);
                if (p.UId == Game.Instance.CurPlayer.Uid)
                {
                    CurPlayerSeat = s;
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
                        ProcessImg.gameObject.SetActive(true);
                        PublicPanel.gameObject.SetActive(true);
                        WaitPanel.gameObject.SetActive(false);
                        MainChipPanel.gameObject.SetActive(true);
                    }
                }
            }

            if (CurPlayerSeat != null && CurPlayerSeat.isSit)
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
				TexasPlayer p = UpPlayerList[k];
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
                        ProcessImg.gameObject.SetActive(true);
                        PublicPanel.gameObject.SetActive(true);
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
        if (RoomInfo.IsPublic == 1) {
            HistoryBtn.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            return;
        }
        if (ActionView.RaisePanel.gameObject.activeSelf)
        {
            ActionView.RaisePanel.gameObject.SetActive(!ActionView.RaisePanel.gameObject.activeSelf);
            ActionView.ActionPanel.gameObject.SetActive(!ActionView.RaisePanel.gameObject.activeSelf);
        }
    }

    void Update()
    {
        if (!isPlaying && SystemNotify.Instance.WantStopServer)
        {
            SystemNotify.Instance.StopServer();
            return;
        }
        if (RoomInfo != null && RoomInfo.IsPublic == 0)
        {
            if (RoomInfo.RoomTime > 0)
            {
                RoomInfo.RoomTime -= Time.deltaTime;
                if (RoomInfo.RoomTime < 300 && RoomInfo.RoomTime % 60 == 0)
                {
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
            if (jb.Keys.Contains("mod") && jb["mod"].ToString() == "game_dzPoker")
            {
                if (jb.ContainsKey("seqNum"))
                {
                    CmdMgr.AddCmd(jb, DateTime.Now);
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
        GameCommond cmd = CmdMgr.GetCmd(DateTime.Now);
        if (cmd != null)
        {
            actionQueue.Enqueue(cmd.Data);
        }

        if (actionQueue.Count > 0) {
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
                    AddBetOptionDone(resp);
                    break;
                case 7:
                    CallBetOptionDone(resp);
                    break;
                case 8:
                    CheckOptionDone(resp);
                    break;
                case 9:
                    AllinOptionDone(resp);
                    break;
                case 10:
                    FoldOptionDone(resp);
                    break;
                case 12:
                    ReConnectDone(resp);
                    break;
                case 13:
                    SendMyCardDone(resp);
                    break;
                case 100:
                    {
                        JsonObject roomInfo = Transfer.Instance[TransferKey.RoomInfo] as JsonObject;
                        if (roomInfo != null)
                        {
                            GetRoomInfoDone(roomInfo);
                            StartCoroutine(DataClear(0.01f));
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
                    OnTexasNotified(resp);
                    break;
            }
        }
    }
    #region 数据处理
    #region 自己的操作

    #region 重连操作
    void ReConnect() {
        MaskView.gameObject.SetActive(true);
        JsonObject param = new JsonObject();
        TexasApi.ReConnect(param, (result) => {
            result.Add(action, (int)TexasGameOp.Reconnect);
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
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(), "7011"));// "重连房间失败");
        }
        else
        {
            if (!result.ContainsKey("gameProcess"))
            {
                isPlaying = false;
                return;
            }
            else
            {
                ProcessImg.gameObject.SetActive(false);
            }
            TexasReconnection recnt = JsonConvert.DeserializeObject<TexasReconnection>(result["gameProcess"].ToString());
            //重现当前牌局
            //1.初始化牌局信息
            ReSetGambleInfo(recnt.SBlindSite, recnt.SBlindBet, recnt.BBlindSite, recnt.BBlindBet, recnt.BankerSite,recnt.SiteUids);
            //2.初始化公共牌和底池信息
            ReSetPublicInfo(recnt.AllBet,recnt.UnderCardArr);
            //3.初始化玩家信息
            ReSetPlayersChip(recnt.CurBetList,recnt.AllBetList);
            ReSetPlayerStatus(recnt.Allin, recnt.OutPlayers, recnt.OpArrForRound);
            //4.初始化自身信息和当前操作者
            if (recnt.SelfCards != null && recnt.SelfCards.Length == 2 && CurPlayerSeat != null && CurPlayerSeat.isSit)
            {
                isRobot = false;
                CurPlayerSeat.RobotImg.gameObject.SetActive(isRobot);
                SendPlayerCards(recnt.SelfCards);
            }
            else
            {
                SendPlayerCards();
            }
            PlayerTurnNotify(recnt.OverBetNum, recnt.MaxBetLimit, recnt.CurPlayerUID, recnt.Ts);
        }
    }
    void ReSetGambleInfo(int sBlindSeat, int sBlind, int bBlindSeat, int bBlind, int banker,List<int> uids)
    {
        //关闭等待开始界面
        StartBtn.gameObject.SetActive(false);
        PublicPanel.gameObject.SetActive(true);
        WaitPanel.gameObject.SetActive(false);

        //初始化数据
        DataInit(uids);
        //设置庄家
        TexasSeatView bs1 = SeatViews[RoomInfo.BankerSite];
        bs1.SetBanker(false);
        RoomInfo.BankerSite = banker;
        TexasSeatView bs2 = SeatViews[RoomInfo.BankerSite];
        bs2.SetBanker(true);

        //小盲注
        TexasSeatView s1 = SeatViews[sBlindSeat];
        if (s1.isSit)
        {
        }

        //大盲注
        TexasSeatView s2 = SeatViews[bBlindSeat];
        if (s2.isSit)
        {
        }

        curBet = 0;
        MaxBlind = bBlind;

    }
    void ReSetPublicInfo(int allBet,int[] underCardArr) {
        mainPoolBet = allBet;
        MainChipPoolTxt.text = allBet.ToString();
        DealCenterCards(underCardArr);
    }
    void ReSetPlayersChip(Dictionary<string, int> curBetList, Dictionary<string, int> allBetList)
    {
        if (curBetList != null)
        {
            foreach (string uid in curBetList.Keys)
            {
                if (PlayerList.ContainsKey(int.Parse(uid)))
                {
                    TexasPlayer p = PlayerList[int.Parse(uid)];
                    TexasSeatView s = SeatViews[p.Site];
                    int bet = curBetList[uid];
                    s.DeskChipImg.gameObject.SetActive(bet > 0);
                    bet = bet > 0 ? bet : 0;
                    curBet = curBet > bet ? curBet : bet;//获取当前最高的下注
                    p.DeskChip = bet;
                    s.DeskChipTxt.text = bet.ToString();
                    s.ChipTxt.text = p.Bet.ToString();
                }
            }
        }
        if (allBetList != null)
        {
            foreach (string uid in allBetList.Keys)
            {
                if (PlayerList.ContainsKey(int.Parse(uid)))
                {
                    TexasPlayer p = PlayerList[int.Parse(uid)];
                    p.Bet -= allBetList[uid];
                }

            }
        }
    }
    void ReSetPlayerStatus(Dictionary<string, int> allin, Dictionary<string, int> outPlayers, Dictionary<string, string> opArrForRound)
    {
        if (allin != null)
        {
            foreach (string uid in allin.Keys)
            {
                if (PlayerList.ContainsKey(int.Parse(uid)))
                {
                    TexasPlayer p = PlayerList[int.Parse(uid)];
                    TexasSeatView s = SeatViews[p.Site];
                    s.ChangeState(TexasPlayerState.Allin);
                }
            }
        }
        if (outPlayers != null)
        {
            foreach (string uid in outPlayers.Keys)
            {
                if (PlayerList.ContainsKey(int.Parse(uid)))
                {
                    TexasPlayer p = PlayerList[int.Parse(uid)];
                    TexasSeatView s = SeatViews[p.Site];
                    s.ChangeState(TexasPlayerState.Fold);
                }
            }
        }
        if (opArrForRound != null)
        {
            foreach (string uid in opArrForRound.Keys)
            {
                if (PlayerList.ContainsKey(int.Parse(uid)))
                {
                    TexasPlayer p = PlayerList[int.Parse(uid)];
                    TexasSeatView s = SeatViews[p.Site];
                    string ops = opArrForRound[uid];
                    if (ops == "firstBet")
                    {
                        s.ChangeState(TexasPlayerState.Bet);
                    }
                    if (ops == "addBet")
                    {
                        s.ChangeState(TexasPlayerState.Raise);
                    }
                    if (ops == "followBet")
                    {
                        s.ChangeState(TexasPlayerState.Call);
                    }
                    if (ops == "pass")
                    {
                        s.ChangeState(TexasPlayerState.Check);
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
        TexasApi.SetGameRoomTime(param, (err) =>
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
        TexasApi.ExitRoom(param,(result)=> {
            result.Add(action, (int)TexasGameOp.OutRoomOp);
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
        else {
            MenuPanel.gameObject.SetActive(false);
        }
    }
    void StartGame()
    {
        if (PlayerList.Count > 1)
        {
            MaskView.gameObject.SetActive(true);
            JsonObject param = new JsonObject();
            param.Add("roomId", RoomInfo.RoomId);
            TexasApi.StartGame(param, (result) =>
            {
                result.Add(action, (int)TexasGameOp.StartGameOp);
                actionQueue.Enqueue(result);
            });
        }
        else {
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
        if (me.Bet < MaxBlind || me.Bet == 0)
        {
            BuyChip();
            wantSit = site;
        }
        else
        {
            MaskView.gameObject.SetActive(true);
            JsonObject param = new JsonObject();
            param.Add("site", site);
            TexasApi.SitDown(param, (result) =>
            {
                result.Add(action, (int)TexasGameOp.SitDownOp);
                actionQueue.Enqueue(result);
            });
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
        MaskView.gameObject.SetActive(true);
        JsonObject param = new JsonObject();
        TexasApi.StandUp(param, (result) => {
            result.Add(action, (int)TexasGameOp.StandUpOp);
            actionQueue.Enqueue(result);
        });
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

    void PreAddBet(int bet)
    {
        MaskView.gameObject.SetActive(true);
        JsonObject param = new JsonObject();
        param.Add("bet", bet);
        TexasApi.PreAddBet(param, (result) => {
            result.Add(action, (int)TexasGameOp.PreAddBetOp);
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
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(),"7017"));// "添加筹码失败");
        }
    }

    #region 下注、加注、跟注、弃牌、全压
    /// <summary>
    /// 下注
    /// </summary>
    /// <param name="bet"></param>
    void AddBetOption(int bet)
    {
        ActionView.CloseActionPanel();

        opid++;
        JsonObject param = new JsonObject();
        param.Add("uid", CurPlayerSeat.Player.UId);
        param.Add("op", "addBet");
        param.Add("bet", bet);
        param.Add("opid", opid);

        TexasApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)TexasGameOp.BetOp);
            result.Add("bet", bet);
            actionQueue.Enqueue(result);
        });
        MaskView.gameObject.SetActive(true);
    }
    void AddBetOptionDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            ActionView.CloseActionPanel();
        }
        else
        {
            ActionView.ActionPanel.gameObject.SetActive(true);
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(),"7018")); // "下注失败");
        }
    }
    /// <summary>
    /// 跟注
    /// </summary>
    void CallBetOption()
    {
        ActionView.CloseActionPanel();

        opid++;
        JsonObject param = new JsonObject();
        param.Add("uid", CurPlayerSeat.Player.UId);
        param.Add("op", "followBet");
        param.Add("opid", opid);

        TexasApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)TexasGameOp.CallBetOp);
            actionQueue.Enqueue(result);
        });

        MaskView.gameObject.SetActive(true);
    }
    void CallBetOptionDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            ActionView.CloseActionPanel();
        }
        else
        {
            ActionView.ActionPanel.gameObject.SetActive(true);
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(),"7019"));// "跟注失败");
        }
    }
    /// <summary>
    /// 过牌
    /// </summary>
    void CheckOption()
    {
        ActionView.CloseActionPanel();

        opid++;
        JsonObject param = new JsonObject();
        param.Add("uid", CurPlayerSeat.Player.UId);
        param.Add("op", "pass");
        param.Add("opid", opid);

        TexasApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)TexasGameOp.CheckBetOp);
            actionQueue.Enqueue(result);
        });
        MaskView.gameObject.SetActive(true);
    }
    void CheckOptionDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            ActionView.CloseActionPanel();
        }
        else
        {
            ActionView.ActionPanel.gameObject.SetActive(true);
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(), "7020"));// "过牌失败");
        }
    }
    /// <summary>
    /// 全压
    /// </summary>
    void AllinOption()
    {
        ActionView.CloseActionPanel();

        opid++;
        JsonObject param = new JsonObject();
        param.Add("uid", CurPlayerSeat.Player.UId);
        param.Add("op", "allin");
        param.Add("opid", opid);

        TexasApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)TexasGameOp.AllinOp);
            actionQueue.Enqueue(result);
        });
        MaskView.gameObject.SetActive(true);
    }
    void AllinOptionDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            ActionView.CloseActionPanel();
        }
        else
        {
            ActionView.ActionPanel.gameObject.SetActive(true);
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(), "7021"));// "allin失败");
        }
    }
    /// <summary>
    /// 弃牌
    /// </summary>
    void FoldOption()
    {
        ActionView.CloseActionPanel();

        opid++;
        JsonObject param = new JsonObject();
        param.Add("uid", CurPlayerSeat.Player.UId);
        param.Add("op", "out");
        param.Add("opid", opid);

        TexasApi.DoGameOption(param, (result) =>
        {
            result.Add(action, (int)TexasGameOp.FoldBetOp);
            actionQueue.Enqueue(result);
        });
        MaskView.gameObject.SetActive(true);
    }
    void FoldOptionDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            ActionView.CloseActionPanel();
        }
        else
        {
            ActionView.ActionPanel.gameObject.SetActive(true);
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText(result["code"].ToString(), "7022"));// "弃牌失败");
        }
    }
    #endregion

    #region 发送自己的底牌
    void SendMyCard(int card,int index)
    {
        MaskView.gameObject.SetActive(true);
        JsonObject param = new JsonObject();
        param.Add("card", card);
        param.Add("cardIndex", index);
        TexasApi.OpenHandCard(param, (result) => {
            result.Add(action, (int)TexasGameOp.OpenCard);
            result.Add("cardIndex", index);
            actionQueue.Enqueue(result);
        });
    }
    void SendMyCardDone(JsonObject result)
    {
        MaskView.gameObject.SetActive(false);
        if (int.Parse(result["code"].ToString()) == 200)
        {
            int cardIndex = int.Parse(result["cardIndex"].ToString());
            if (CurPlayerSeat != null) {
                PokerItem p = CurPlayerSeat.OwnPokerCards[cardIndex];
                p.EyeBtn.onClick.RemoveAllListeners();
                p.EyeImg.gameObject.SetActive(true);
            }
        }
    }

    #endregion

    #endregion

    #region 监听到的操作和系统事件

    void OnTexasNotified(JsonObject data) {
        if (data != null && data.ContainsKey("op")) {
            string op = data["op"].ToString();
            switch (op) {
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
                        TexasPlayer p = JsonConvert.DeserializeObject<TexasPlayer>(data["player"].ToString());
                        PlayerSitDownNotify(int.Parse(data["uid"].ToString()), p);
                        break;
                    }
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
                case "blind":
                    //一局牌局开始
                    GambleStartNotify(data);
                    break;
                case "turnPlayer":
                    //切换玩家操作
                    PlayerTurnNotify(int.Parse(data["overBetNum"].ToString()), int.Parse(data["maxBetLimit"].ToString()), int.Parse(data["uid"].ToString()), int.Parse(data["ts"].ToString()));
                    break;
                case "self_showCards":
                    {
                        //发放玩家手牌
                        int[] cards = JsonConvert.DeserializeObject<int[]>(data["cards"].ToString());
                        ShowSelfPokerNotify(cards);
                        break;
                    }
                case "nextRound":
                    //下一圈开始
                    NextRoundNotify(data["round"].ToString());
                    break;
                case "showUnderCards":
                    {
                        //公共底牌
                        int[] cards = JsonConvert.DeserializeObject<int[]>(data["cards"].ToString());
                        ShowCenterCardsNotify(cards);
                        break;
                    }
                case "out":
                    //弃牌
                    FoldNotify(int.Parse(data["uid"].ToString()));
                    break;
                case "allin":
                    //全押
                    AllinNotify(int.Parse(data["uid"].ToString()));
                    break;
                case "addBet":
                    // 押注、加注
                    BetNotify(int.Parse(data["uid"].ToString()), int.Parse(data["bet"].ToString()));
                    break;
                case "followBet":
                    //跟注
                    CallNotify(int.Parse(data["uid"].ToString()));
                    break;
                case "pass":
                    //让牌 
                    CheckNotify(int.Parse(data["uid"].ToString()));
                    break;
                case "checkout":
                    //回合结束
                    {
                        //公共底牌
                        int[] cards = JsonConvert.DeserializeObject<int[]>(data["underCards"].ToString());
                        int num = int.Parse(data["underCardsIndex"].ToString());
                        int compNum = int.Parse(data["compCardUserNum"].ToString());
                        GambleResultNotify(data["result"] as JsonObject, cards, num, compNum);
                        break;
                    }
                case "openHandCard":
                    {
                        OpendHandCardNotify(int.Parse(data["openUid"].ToString()), int.Parse(data["card"].ToString()), int.Parse(data["cardIndex"].ToString()));
                        break;
                    }
                case "user_emoji":
                    {
                        string emoji = data["data"].ToString();
                        EmojiUse eu = JsonConvert.DeserializeObject<EmojiUse>(emoji);
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
                case "opError":
                    {
                        //错误提示
                        Game.Instance.ShowTips(LocalizationManager.Instance.GetText(data["msg"].ToString()));
                        if (data.ContainsKey("opid"))
                        {
                            int lastop = int.Parse(data["opid"].ToString());
                            if (lastop == opid && ActionPlayerSeat != null && CurPlayerSeat != null && CurPlayerSeat.Player.UId == ActionPlayerSeat.Player.UId)
                            {
                                ActionView.ActionPanel.gameObject.SetActive(true);
                            }
                        }
                    }
                    break;
                case "roomOver":
                    {
                        RoomClosedNotify();
                        break;
                    };
                case "clubRoom_setRoomTime":
                    {
                        data = data["data"] as JsonObject;
                        int roomid = int.Parse(data["roomId"].ToString());
                        if (roomid == RoomInfo.RoomId) {
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
        TexasPlayer p = JsonConvert.DeserializeObject<TexasPlayer>(data.ToString());
        if (UpPlayerList.ContainsKey(p.UId))
        {
            UpPlayerList[p.UId] = p;
        }
        else
        {
            UpPlayerList.Add(p.UId, p);
        }
        Game.Instance.ShowTips(string.Format(LocalizationManager.Instance.GetText("7023"), p.UId));// ("玩家ID:"+p.UId+"进入房间");
    }
    /// <summary>
    /// 玩家退出房间
    /// </summary>
    /// <param name="uid"></param>
    void PlayerExitNotify(int uid)
    {
        Game.Instance.ShowTips(string.Format(LocalizationManager.Instance.GetText("7024"), uid));// ("玩家ID:" + uid + "退出房间");
        if (PlayerList.ContainsKey(uid)) {
            TexasPlayer p = PlayerList[uid];
            if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
            {
                string scene = PlayerPrefs.GetString("LastScene");
                if(scene == "ClubScene")
                {
                    Transfer.Instance[TransferKey.RoomSwitch] = ClubRoomSwitch.Dz;
                }
                SceneManager.LoadScene(scene);
            }
            else {
                TexasSeatView s = SeatViews[p.Site];
                if (CurPlayerSeat == null)
                {
                    s.StandUp(0);
                }
                else {
                    s.StandUp(1);
                }
                if (ActionPlayerSeat != null && ActionPlayerSeat.Player.UId == uid)
                {
                    s.EndActionTimer();
                    ActionPlayerSeat = null;
                }
                s.Player = null;
                OnSeatViews.Remove(s);
                PlayerList.Remove(uid);
            }
        }
        if (UpPlayerList.ContainsKey(uid))
        {
            TexasPlayer p = UpPlayerList[uid];
            if (me.UId == uid)
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
            TexasPlayer p = PlayerList[uid];
            TexasSeatView s = SeatViews[p.Site];
            if (string.IsNullOrEmpty(type))
            {
                s.ChangeState(TexasPlayerState.Normal);
                if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
                {
                    ActionView.CloseActionPanel();
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
                if (ActionPlayerSeat != null && ActionPlayerSeat.Player.UId == uid)
                {
                    s.EndActionTimer();
                    ActionPlayerSeat = null;
                }
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
                s.ChangeState(TexasPlayerState.Look);
            }
        }
    }
    /// <summary>
    /// 玩家坐下
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="seat"></param>
    void PlayerSitDownNotify(int uid, TexasPlayer player) {
        if (!UpPlayerList.ContainsKey(uid))
        {
            UpPlayerList.Add(uid, player);
        }
        else {
            UpPlayerList[uid] = player;
        }
        if (Game.Instance.CurPlayer.Uid == uid)
        {
            MaskView.gameObject.SetActive(true);
            AnimationBeforeSitdown(player.Site, () =>
            {
                TexasPlayer p = UpPlayerList[uid];
                p.Bet = player.Bet;
                p.Site = player.Site;
                p.State = TexasPlayerState.Wait;
                me = p;

                TexasSeatView s = SeatViews[p.Site];
                s.Player = p;
                s.SitDown();

                OnSeatViews.Add(s);
                PlayerList.Add(p.UId, p);
                UpPlayerList.Remove(uid);

                CurPlayerSeat = s;
                isRobot = false;
                CurPlayerSeat.RobotImg.gameObject.SetActive(isRobot);
                RefreshSeatViews();
                MaskView.gameObject.SetActive(false);
            });
        }
        else
        {
            TexasPlayer p = UpPlayerList[uid];
            p.Bet = player.Bet;
            p.Site = player.Site;
            p.State = TexasPlayerState.Wait;

            TexasSeatView s = SeatViews[p.Site];
            s.Player = p;
            s.SitDown();

            OnSeatViews.Add(s);
            PlayerList.Add(p.UId, p);
            UpPlayerList.Remove(uid);
            RefreshSeatViews();
        }
    }
    void AnimationBeforeSitdown(int site, Action callback = null) {

        if (SeatViews.Length > 0)
        {
            TexasSeatView targSeat = SeatViews[site];

            int all = SeatViews.Length;
            int ctNum = all / 2;
            int ptNum = Math.Abs(ctNum - targSeat.SeatPosIndex);
            if (targSeat.SeatPosIndex == ctNum)
            {
                targSeat.SeatTurnAnim(null,0, callback);

                for (int i = 0; i < SeatViews.Length; i++)
                {
                    TexasSeatView s = SeatViews[i];

                    int fiNum = i;
                    if (s.Content.gameObject.activeSelf && i != site)
                    {
                        s.isSit = true;
                    }
                    int chipPos = 5;
                    if (RoomInfo.SeatNum == 6)
                    {
                        chipPos = (fiNum == 0 || fiNum == 1 || fiNum == 5) ? 5 : 0;
                    }
                    else
                    {
                        chipPos = fiNum == 6 ? 3 : fiNum == 5 ? 1 : fiNum == 4 ? 0 : fiNum == 3 ? 2 : fiNum == 2 ? 4 : 5;
                    }
                    s.SeatPosIndex = fiNum;
                    if (i != site)
                    {
                        s.SeatTurnAnim(null, chipPos);
                    }
                }
            }
            else if (targSeat.SeatPosIndex < ctNum)
            {
                for (int i = 0; i < SeatViews.Length; i++)
                {
                    TexasSeatView s = SeatViews[i];

                    Vector3[] wpts = new Vector3[ptNum];

                    int fiNum = (s.SeatPosIndex + ptNum) % all;

                    for (int j = 1; j <= ptNum; j++)
                    {
                        int idx = (s.SeatPosIndex + j) % all;
                        wpts[j - 1] = SeatPositions[idx].localPosition;
                    }

                    if (s.Content.gameObject.activeSelf && i != site)
                    {
                        s.isSit = true;
                    }
                    int chipPos = 5;
                    if (RoomInfo.SeatNum == 6)
                    {
                        chipPos = (fiNum == 0 || fiNum == 1 || fiNum == 5) ? 5 : 0;
                    }
                    else
                    {
                        chipPos = fiNum == 6 ? 3 : fiNum == 5 ? 1 : fiNum == 4 ? 0 : fiNum == 3 ? 2 : fiNum == 2 ? 4 : 5;
                    }
                    s.SeatPosIndex = fiNum;
                    if (i == SeatViews.Length - 1)
                    {
                        s.SeatTurnAnim(wpts, chipPos, callback);
                    }
                    else
                    {
                        s.SeatTurnAnim(wpts, chipPos);
                    }
                }
            }
            else
            {
                for (int i = 0; i < SeatViews.Length; i++)
                {
                    TexasSeatView s = SeatViews[i];

                    Vector3[] wpts = new Vector3[ptNum];

                    int fiNum = (s.SeatPosIndex - ptNum + all) % all;

                    for (int j = 1; j <= ptNum; j++)
                    {
                        int idx = (s.SeatPosIndex - j + all) % all;
                        wpts[j - 1] = SeatPositions[idx].localPosition;
                    }

                    if (s.Content.gameObject.activeSelf && i != site)
                    {
                        s.isSit = true;
                    }
                    int chipPos = 5;
                    if (RoomInfo.SeatNum == 6)
                    {
                        chipPos = (fiNum == 0 || fiNum == 1 || fiNum == 5) ? 5 : 0;
                        Debug.Log("----chippos----:" + fiNum + "-->" + chipPos);
                    }
                    else
                    {
                        chipPos = fiNum == 6 ? 3 : fiNum == 5 ? 1 : fiNum == 4 ? 0 : fiNum == 3 ? 2 : fiNum == 2 ? 4 : 5;
                    }
                    s.SeatPosIndex = fiNum;
                    if (i == SeatViews.Length - 1)
                    {
                        s.SeatTurnAnim(wpts, chipPos, callback);
                    }
                    else
                    {
                        s.SeatTurnAnim(wpts, chipPos);
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
    void PlayerAddChipNotify(int uid, int bet) {
        TexasPlayer p = null;
        if (PlayerList.ContainsKey(uid)) {
            p = PlayerList[uid];
            p.Bet += bet;
            TexasSeatView s = SeatViews[p.Site];
            s.ChipTxt.text = p.Bet.ToString();
        }
        if (UpPlayerList.ContainsKey(uid))
        {
            p = UpPlayerList[uid];
            p.Bet += bet;
        }
        if (me.UId == uid) {
            ClubChips -= bet;
            if (RoomInfo.IsPublic == 1)
            {
                Game.Instance.CurPlayer.Gold = ClubChips;
            }
            if (wantSit >= 0) {
                int seat = wantSit;
                SitDown(seat);
                wantSit = -1;
            }
        }
    }

    void PlayerUseEmoji(EmojiUse eu)
    {
        if (PlayerList.ContainsKey(eu.FromUId) && PlayerList.ContainsKey(eu.ToUId))
        {
            TexasPlayer fromPlayer = PlayerList[eu.FromUId];
            TexasPlayer toPlayer = PlayerList[eu.ToUId];

            TexasSeatView fromSeat = SeatViews[fromPlayer.Site];
            TexasSeatView toSeat = SeatViews[toPlayer.Site];

            EmojiItem ei = Instantiate(Resources.Load<EmojiItem>("Prefabs/Emoji/" + eu.Emoji));
            ei.transform.SetParent(fromSeat.transform);
            (ei.transform as RectTransform).anchoredPosition3D = Vector3.zero;

            ei.TargetRect = toSeat.transform as RectTransform;
            ei.Show();
        }
    }
    void PlayerSound(int uid, string audio)
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
    /// <param name="sBlindUid"></param>
    /// <param name="sBlind"></param>
    /// <param name="bBlindUid"></param>
    /// <param name="bBlind"></param>
    /// <param name="banker"></param>
    void GambleStartNotify(JsonObject data) {
        int sBlindSeat = int.Parse(data["sBlindSite"].ToString());
        int sBlind = int.Parse(data["sBlindBet"].ToString());
        int bBlindSeat = int.Parse(data["bBlindSite"].ToString());
        int bBlind = int.Parse(data["bBlindBet"].ToString());
        int banker = int.Parse(data["bankerSite"].ToString());
        List<int> realPlayers = JsonConvert.DeserializeObject<List<int>>(data["siteUids"].ToString());

        //关闭等待开始界面
        StartBtn.gameObject.SetActive(false);
        PublicPanel.gameObject.SetActive(true);
        WaitPanel.gameObject.SetActive(false);
        MainChipPanel.gameObject.SetActive(true);

        //初始化数据
        Game.Instance.AudioMgr.PlayAudioEffect("start1");

        ProcessImg.gameObject.SetActive(false);
        DataInit(realPlayers);
        //设置庄家
        TexasSeatView bs1 = SeatViews[RoomInfo.BankerSite];
        bs1.SetBanker(false);
        RoomInfo.BankerSite = banker;
        TexasSeatView bs2 = SeatViews[RoomInfo.BankerSite];
        bs2.SetBanker(true);

        //小盲注
        TexasSeatView s1 = SeatViews[sBlindSeat];
        if (s1.isSit)
        {
            s1.DoBet(sBlind);
        }
        //大盲注
        TexasSeatView s2 = SeatViews[bBlindSeat];
        if (s2.isSit)
        {
            s2.DoBet(bBlind);
        }

        curBet = bBlind;
        MaxBlind = bBlind;

        if (CurPlayerSeat == null || CurPlayerSeat.isSit == false) {
            SendPlayerCards();
        }
    }

    /// <summary>
    /// 切换玩家操作
    /// </summary>
    /// <param name="overBteNum"></param>
    /// <param name="maxBetLimit"></param>
    /// <param name="uid"></param>
    /// <param name="ts"></param>
    void PlayerTurnNotify(int overBteNum,int maxBetLimit,int uid,int ts) {
        //上玩家结束
        if (ActionPlayerSeat != null)
        {
            ActionPlayerSeat.EndActionTimer();
            if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == ActionPlayerSeat.Player.UId)
            {
                ActionView.CloseActionPanel();
                ActionView.SetToggleOff();
            }
        }

        if (OnSeatViews != null) {
            int pot = curBet;
            foreach(TexasSeatView item in OnSeatViews) {
                pot += item.Player.DeskChip;
            }
            CurPotTxt.text = LocalizationManager.Instance.GetText("7032") + ":" + pot;
        }

        if (!PlayerList.ContainsKey(uid)) {
            //不存在就不处理
            return;
        }


        //下玩家开始
        TexasPlayer p = PlayerList[uid];
        TexasSeatView s = SeatViews[p.Site];
        s.PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("turnAction"));
        ActionPlayerSeat = s;
        ActionPlayerSeat.ChangeState(TexasPlayerState.Normal);
        //1.进入倒计时
        s.coldTime = ts;
        s.StartActionTimer();
        //2.如果是当前玩家，显示操作面板
        if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == uid)
        {
            if (ActionView.AutoNextToggleGroup.ActiveToggles().Count() > 0)
            {
                Toggle tgl = ActionView.AutoNextToggleGroup.ActiveToggles().First();
                if (tgl.name == "FoldToggle")
                {
                    if (CurPlayerSeat.Player.DeskChip >= curBet)
                    {
                        OnCheckBtnClick();
                    }
                    else
                    {
                        OnFoldBtnClick();
                    }
                }
                if (tgl.name == "CheckToggle")
                {
                    if (CurPlayerSeat.Player.DeskChip >= curBet)
                    {
                        OnCheckBtnClick();
                    }
                    else
                    {
                        ActiveActionPanel(overBteNum, maxBetLimit);
                    }
                }
                if (tgl.name == "CallToggle" || tgl.name == "AnyCallToggle")
                {
                    if (CurPlayerSeat.Player.Bet >= curBet - CurPlayerSeat.Player.DeskChip)
                    {
                        OnCallBtnClick();
                    }
                    else
                    {
                        ActiveActionPanel(overBteNum, maxBetLimit);
                    }
                }
                if (tgl.name == "AllinToggle")
                {
                    OnAllinBtnClick();
                }
            }
            else if (isRobot)
            {
                if (CurPlayerSeat.Player.DeskChip >= curBet)
                {
                    OnCheckBtnClick();
                }
                else
                {
                    OnFoldBtnClick();
                }
            }
            else
            {
                ActiveActionPanel(overBteNum, maxBetLimit);
            }
        }
        else
        {
            if (CurPlayerSeat != null && CurPlayerSeat.Player.State != TexasPlayerState.Fold && CurPlayerSeat.Player.State != TexasPlayerState.Wait && CurPlayerSeat.Player.State != TexasPlayerState.Allin)
            {
                ActiveAutoNextPanel();
            }
            else
            {
                ActionView.CloseAutoNextPanel();
            }
        }
    }

    /// <summary>
    /// 显示玩家手牌
    /// </summary>
    /// <param name="cards"></param>
    void ShowSelfPokerNotify(int[] cards)
    {
        //发牌动画
        Game.Instance.AudioMgr.PlayAudioEffect("dealingCard");
        SendPlayerCards(cards);
    }

    /// <summary>
    /// 进入牌局下一圈叫牌
    /// </summary>
    /// <param name="round"></param>
    void NextRoundNotify(string round) {
        //把玩家筹码加入底池
        Game.Instance.AudioMgr.PlayAudioEffect("allin");
        MainChipPanel.gameObject.SetActive(true);
        roundTxt = round;

        if (OnSeatViews != null)
        {
            foreach (TexasSeatView s in OnSeatViews)
            {
                if (s.Player.DeskChip > 0)
                {
                    s.CollectChips(MainChipPanel);
                }
                mainPoolBet += s.Player.DeskChip;
                s.Player.DeskChip = 0;

                //修改玩家状态
                //如果Allin、弃牌、等待状态就保持原有状态，其他恢复普通状态
                if (s.Player.State == TexasPlayerState.Wait|| s.Player.State == TexasPlayerState.Allin|| s.Player.State == TexasPlayerState.Fold)
                {

                }
                else
                {
                    s.ChangeState(TexasPlayerState.Normal);
                }
            }
        }
        curBet = 0;
        MainChipPoolTxt.text = mainPoolBet.ToString();
    }
    /// <summary>
    /// 发系统牌通知
    /// </summary>
    /// <param name="cards"></param>
    void ShowCenterCardsNotify(int[] cards) {
        Game.Instance.AudioMgr.PlayAudioEffect("fapai");
        if (publicPokers.Count == 0)
        {
            DealCenterCards(cards);
        }
        else if (publicPokers.Count == 3)
        {
            if (cards.Length == 5)
            {
                DealFourthCenterCard(cards[3]);
                DealFifthCenterCard(cards[4]);
            }
            else if (cards.Length == 4)
            {
                DealFourthCenterCard(cards[3]);
            }
        }
        else if (publicPokers.Count == 4 && cards.Length == 5)
        {
            DealFifthCenterCard(cards[4]);
        }
    }
    /// <summary>
    /// 弃牌通知
    /// </summary>
    /// <param name="uid"></param>
    void FoldNotify(int uid)
    {
        TexasPlayer p = PlayerList[uid];
        TexasSeatView s = SeatViews[p.Site];
        s.PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("fold"));

        s.ChangeState(TexasPlayerState.Fold);
    }
    /// <summary>
    /// 全压通知
    /// </summary>
    /// <param name="uid"></param>
    void AllinNotify(int uid)
    {
        TexasPlayer p = PlayerList[uid];
        TexasSeatView s = SeatViews[p.Site];
        s.PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("allin"));
        s.DoBet(s.Player.Bet);
        curBet = s.Player.DeskChip;
        s.ChangeState(TexasPlayerState.Allin);
    }
    /// <summary>
    /// 下注通知，注意区分下注加注
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="bet"></param>
    void BetNotify(int uid,int bet)
    {
        TexasPlayer p = PlayerList[uid];
        TexasSeatView s = SeatViews[p.Site];
        s.PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("bet"));
        s.DoBet(bet);
        if (curBet>0)
        {
            s.ChangeState(TexasPlayerState.Raise);
        }
        else
        {
            s.ChangeState(TexasPlayerState.Bet);
        }
        curBet = s.Player.DeskChip;
    }
    /// <summary>
    /// 跟注通知
    /// </summary>
    /// <param name="uid"></param>
    void CallNotify(int uid)
    {
        TexasPlayer p = PlayerList[uid];
        TexasSeatView s = SeatViews[p.Site];
        s.PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("bet"));

        s.DoBet(curBet-s.Player.DeskChip);
        s.ChangeState(TexasPlayerState.Call);
    }
    /// <summary>
    /// 过牌
    /// </summary>
    /// <param name="uid"></param>
    void CheckNotify(int uid)
    {
        TexasPlayer p = PlayerList[uid];
        TexasSeatView s = SeatViews[p.Site];
        s.PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("check"));
        s.ChangeState(TexasPlayerState.Check);
    }
    /// <summary>
    /// 显示某人底牌
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="card"></param>
    /// <param name="cardIndex"></param>
    void OpendHandCardNotify(int uid ,int card ,int cardIndex) {
        if (PlayerList.ContainsKey(uid) && isPlaying) {
            TexasPlayer p = PlayerList[uid];
            TexasSeatView s = SeatViews[p.Site];

            if (s.OwnPokerCards.Count ==2)
            {
                s.OwnPokerCards[cardIndex].RenderView(card.ToString());
                s.ShowBottomCards(cardIndex);
            }
        }
    }

    /// <summary>
    /// 牌局结束
    /// </summary>
    /// <param name="data"></param>
    void GambleResultNotify(JsonObject data, int[] cards,int cardNum,int compNum)
    {
        ActionView.CloseAutoNextPanel();
        StartCoroutine(IEGambleResult(1.2f,data,cards,cardNum, compNum));
        StartCoroutine(DataClear(8.6f));
    }
    IEnumerator IEGambleResult(float time, JsonObject data, int[] cards, int cardNum, int compNum)
    {
        if (ActionPlayerSeat != null)
        {
            ActionPlayerSeat.EndActionTimer();
            ActionPlayerSeat = null;
            if (CurPlayerSeat != null && ActionView.ActionPanel.gameObject.activeSelf)
            {
                ActionView.CloseActionPanel();
            }
        }
        //公共底牌
        finalCards = cards;
        int[] tmp = cards.ToList().GetRange(0, cardNum).ToArray();
        if (publicPokers.Count < 5 && cardNum < 5)
        {
            ShowPublicBtn.gameObject.SetActive(true);
        }

        ShowCenterCardsNotify(tmp);
        yield return new WaitForSeconds(time);

        if (CurPlayerSeat != null && CurPlayerSeat.OwnPokerCards != null)
        {
            for (int i = 0; i < CurPlayerSeat.OwnPokerCards.Count; i++)
            {
                PokerItem p = CurPlayerSeat.OwnPokerCards[i];
                int index = i;
                p.EyeBtn.onClick.RemoveAllListeners();
                p.EyeBtn.onClick.AddListener(delegate {
                    SendMyCard(int.Parse(p.Type), index);
                });
            }
        }
        foreach (string k in data.Keys)
        {
            int idx = int.Parse(k);
            JsonObject obj = data[k] as JsonObject;

            if (!PlayerList.ContainsKey(idx))
            {
                if (UpPlayerList.ContainsKey(idx))
                {
                    TexasPlayer up = UpPlayerList[idx];
                    up.Bet = int.Parse(obj["restBet"].ToString());
                }
                continue;
            }

            TexasPlayer p = PlayerList[idx];
            TexasSeatView s = SeatViews[p.Site];
            p.Bet = int.Parse(obj["restBet"].ToString());
            s.ChipTxt.text = p.Bet.ToString();
            int balance = int.Parse(obj["balance"].ToString());
            if (balance > 0)
            {
                if (compNum > 1)
                {
                    int[] wincards = JsonConvert.DeserializeObject<int[]>(obj["cards"].ToString());
                    if (s.OwnPokerCards.Count == wincards.Length)
                    {
                        for (int i = 0; i < s.OwnPokerCards.Count; i++)
                        {
                            s.OwnPokerCards[i].RenderView(wincards[i].ToString());
                        }
                        s.ShowBottomCards();

                    }
                }
                s.DealChips(s.transform as RectTransform, MainChipPanel.position);
            }
            MainChipPoolTxt.text = "0";
            s.ShowResult(balance);
            if (CurPlayerSeat != null && CurPlayerSeat.Player.UId == s.Player.UId && balance > 0)
            {
                s.PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("incomePot"));
                if (compNum > 1) {
                    s.CloseEyeBtns();
                }
            }
        }
    }
    void DataInit(List<int> uids) {
        MaskView.gameObject.SetActive(false);
        isPlaying = true;
        roundTxt = "";
        ActionPlayerSeat = null;
        curBet = 0;
        mainPoolBet = 0;
        finalCards = null;
        ShowPublicBtn.gameObject.SetActive(false);
        CurPotTxt.text = LocalizationManager.Instance.GetText("7032") + ":0";
        TexasSeatView bs = SeatViews[RoomInfo.BankerSite];
        bs.SetBanker(false);
        if (OnSeatViews != null)
        {
            for (int i = 0; i < OnSeatViews.Count; i++)
            {
                TexasSeatView sx = OnSeatViews[i];
                sx.OwnPokerCards.Clear();
                if (uids.Contains(sx.Player.UId))
                {
                    sx.ChangeState(TexasPlayerState.Normal);
                }
                else
                {
                    sx.ChangeState(TexasPlayerState.Wait);
                }
            }
        }
        RecyclePokers();
    }
    IEnumerator DataClear(float time) {
        MainChipPanel.gameObject.SetActive(true);
        if (OnSeatViews != null)
        {
            foreach (TexasSeatView s in OnSeatViews)
            {
                if (s.Player.DeskChip > 0)
                {
                    s.CollectChips(MainChipPanel);
                }
                mainPoolBet += s.Player.DeskChip;
                s.Player.DeskChip = 0;
            }
        }
        MainChipPoolTxt.text = mainPoolBet.ToString();
        
        yield return new WaitForSeconds(time);
        //关闭界面
        if (RoomInfo.Status == 1)
        {
            StartBtn.gameObject.SetActive(false);
            PublicPanel.gameObject.SetActive(true);
            WaitPanel.gameObject.SetActive(false);
        }

        MaskView.gameObject.SetActive(false);
        ActionView.CloseActionPanel();
        ActionView.SetToggleOff();

        isPlaying = false;
        ActionPlayerSeat = null;
        curBet = 0;
        mainPoolBet = 0;
        finalCards = null;
        ShowPublicBtn.gameObject.SetActive(false);
        MainChipPoolTxt.text = "0";
        roundTxt = "";
        CurPotTxt.text = LocalizationManager.Instance.GetText("7032") + ":0";
        ProcessImg.gameObject.SetActive(RoomInfo.Status == 1);
        ProcessTxt.text = LocalizationManager.Instance.GetText("7025");// "等待牌局开始";
        TexasSeatView bs = SeatViews[RoomInfo.BankerSite];
        bs.SetBanker(false);
        if (SeatViews != null)
        {
            for (int i = 0; i < SeatViews.Length; i++)
            {
                TexasSeatView sx = SeatViews[i];
                sx.OwnPokerCards.Clear();
                if (sx.Player != null)
                {
                    if (sx.Player.State == TexasPlayerState.Look || !PlayerList.ContainsKey(sx.Player.UId))
                    {
                        PlayerStandUpNotify(sx.Player.UId, "");
                    }
                    else
                    {
                        sx.ChangeState(TexasPlayerState.Normal);
                    }
                }
            }
        }
        RecyclePokers();
        GetRestTime();
    }
    void GetRestTime()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("roomId", RoomInfo.RoomId.ToString());
        TexasApi.GetGameRoomTime(param, (result, err) =>
        {
            if (!string.IsNullOrEmpty(err))
            {
                RoomInfo.RoomTime = result;
            }
        });
    }
    void RoomClosedNotify()
    {
        if (FinalStatPanel.gameObject.activeSelf)
        {
            return;
        }
        StartCoroutine(RoomClosed());
    }
    IEnumerator RoomClosed()
    {
        yield return new WaitForSeconds(5.0f);
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
    void RefreshSeatViews() {
        if (SeatViews != null)
        {
            for (int i = 0; i < SeatViews.Length; i++)
            {
                TexasSeatView sx = SeatViews[i];
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

            RefreshNickColor();
        }
    }
	public void OnStartBtnClick(){
		StartGame ();
	}
    public void OnMenuBtnClick() {
        MenuPanel.gameObject.SetActive(true);
    }
    public void OnMenuStandUpBtnClick() {
		if (CurPlayerSeat != null) {
			StandUp();
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
        else {
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
    }
    public void OnMenuHangOutBtnClick()
    {
        MenuPanel.gameObject.SetActive(false);

        if (CurPlayerSeat != null && CurPlayerSeat.isSit)
        {
            isRobot = !isRobot;
            CurPlayerSeat.RobotImg.gameObject.SetActive(isRobot);
        }
    }
    public void OnMenuExitRoomBtnClick()
    {
        MenuPanel.gameObject.SetActive(false);
        ExitRoom();
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
        TexasApi.GetGameRoomLiveData(param, RoomInfo.IsPublic == 1, (resp,error) =>
        {
            if (error == null)
            { 
                LiveRecordPanel.gameObject.SetActive(true);
                LiveRecordPanel.InitTexasView(resp,UpPlayerList);
            }
            else {
                Game.Instance.ShowTips(error);
            }
        });
    }
    public void OnHistoryBtnClick() {
        HistoryPanel.roomId = RoomInfo.RoomId;
        HistoryPanel.Show();
    }
    public void OnShowPublicBtnClick()
    {
        if (Game.Instance.CurPlayer.CanLookCard == 1 && finalCards != null && isPlaying)
        {
            ShowCenterCardsNotify(finalCards);
            ShowPublicBtn.gameObject.SetActive(false);
        }
        else
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("6304"));
        }
    }

    public void OnShopBtnClick() { }

    public void ActiveActionPanel(int overBetNum,int maxBetLimit) {
        ActionView.ActiveActionPanel(overBetNum, maxBetLimit, CurPlayerSeat.Player.DeskChip, CurPlayerSeat.Player.Bet, MaxBlind, curBet,mainPoolBet);
    }
    public void ActiveAutoNextPanel()
    {
        ActionView.ActiveAutoNextPanel(CurPlayerSeat.Player.DeskChip, CurPlayerSeat.Player.Bet, curBet);
    }

    public void OnShowRaiseBtnClick()
    {
        ActionView.RaisePanel.gameObject.SetActive(!ActionView.RaisePanel.gameObject.activeSelf);
        ActionView.ActionPanel.gameObject.SetActive(!ActionView.RaisePanel.gameObject.activeSelf);
    }

    public void OnRaiseBtnClick()
    {
        ActionView.RaisePanel.gameObject.SetActive(false);
        if (ActionView.RaiseSlider.value > ActionView.RaiseSlider.maxValue)
        {
            ActionView.RaiseSlider.value = ActionView.RaiseSlider.maxValue;
            ActionView.RaiseSlider.minValue = ActionView.RaiseSlider.maxValue;
        }
        if (ActionView.RaiseSlider.value == ActionView.RaiseSlider.maxValue)
        {
            AllinOption();
        }
        else
        {
            int tmp = (int)ActionView.RaiseSlider.value;
            AddBetOption(tmp);
        }
    }
    public void OnAllinBtnClick()
    {
        AllinOption();
    }
    public void OnFoldBtnClick() {
        FoldOption();
    }
    public void OnCheckBtnClick() {
        CheckOption();
    }
    public void OnCallBtnClick() {
        CallBetOption();
    }
    #region 发牌动画
    public void SendPlayerCards()
    {
        StartCoroutine(ISendPlayerCards());
    }

    public IEnumerator ISendPlayerCards()
    {
        yield return new WaitForSeconds(0.5f);
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < OnSeatViews.Count; i++)
            {
                var go = Instantiate(pokerTpl);
                PokerItem p = go.GetComponent<PokerItem>();
                go.transform.SetParent(PokerPoolPanel);
                (go.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                (go.transform as RectTransform).localScale = new Vector3(0.3f, 0.3f, 1);

                TexasSeatView seat = OnSeatViews[i];
                if (seat.Player != null && seat.Player.State == TexasPlayerState.Wait)
                {
                    continue;
                }
                seat.Player.State = TexasPlayerState.Normal;
                p.MyRotation.DealCard(j, seat.SmallArea);
                seat.OwnPokerCards.Add(p);
                playersPokers.Add(p);
            }
        }
    }

    public void SendPlayerCards(int[] cards)
    {
        StartCoroutine(ISendPlayerCards(cards));
    }
    public IEnumerator ISendPlayerCards(int[] cards)
    {
        for (int j = 0; j < 2; j++) {
            for (int i = 0; i < OnSeatViews.Count; i++)
            {
                var go = Instantiate(pokerTpl);
                PokerItem p = go.GetComponent<PokerItem>();
                go.transform.SetParent(PokerPoolPanel);
                (go.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                (go.transform as RectTransform).localScale = new Vector3(0.3f, 0.3f, 1);
                
                TexasSeatView seat = OnSeatViews[i];
                if (seat.Player != null && seat.Player.State == TexasPlayerState.Wait)
                {
                    continue;
                }
                seat.Player.State = TexasPlayerState.Normal;
                if (CurPlayerSeat != null && seat.Player.UId == CurPlayerSeat.Player.UId) {
                    p.RenderView(cards[j].ToString());
                }
                p.MyRotation.DealCard(j, seat.SmallArea);
                seat.OwnPokerCards.Add(p);
                playersPokers.Add(p);
            }
        }
        yield return new WaitForSeconds(0.7f);
        if (CurPlayerSeat != null && CurPlayerSeat.isSit)
        {
            CurPlayerSeat.ShowBottomCards();
        }
    }

    public void DealCenterCards(int[] cards)
    {
        string[] strs = new string[cards.Length];
        for (int i = 0; i < cards.Length; i++) {
            strs[i] = cards[i].ToString();
        }
        StartCoroutine(DealCenterCards(strs));
    }
    public IEnumerator DealCenterCards(string[] strs)
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < strs.Length; i++)
        {
            if (i < 3)
            {
                var go = Instantiate(pokerTpl);
                PokerItem p = go.GetComponent<PokerItem>();
                p.RenderView(strs[i]);
                go.transform.SetParent(PokerPoolPanel);
                (go.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                (go.transform as RectTransform).localScale = new Vector3(0.3f, 0.3f, 1);

                p.MyRotation.DealCenterCardAndSpreadOut(PublicPokerPanel, i);
                publicPokers.Add(p);

            }
            else if (i == 3)
            {
                yield return new WaitForSeconds(1.1f);
                if (isPlaying)
                    StartCoroutine(DealFourthCenterCard(strs[i]));
            }
            else if (i == 4)
            {
                yield return new WaitForSeconds(0.6f);
                if (isPlaying)
                    StartCoroutine(DealFifthCenterCard(strs[i]));
            }
        }
    }

    public void DealFourthCenterCard(int card)
    {
        StartCoroutine(DealFourthCenterCard(card.ToString()));
    }
    public IEnumerator DealFourthCenterCard(string card)
    {
        var go = Instantiate(pokerTpl);
        PokerItem p = go.GetComponent<PokerItem>();
        p.RenderView(card);
        go.transform.SetParent(PokerPoolPanel);
        (go.transform as RectTransform).anchoredPosition3D = Vector3.zero;
        (go.transform as RectTransform).localScale = new Vector3(0.3f, 0.3f, 1);

        p.MyRotation.DealFourthCenterCard(PublicPokerPanel);
        publicPokers.Add(p);
        yield return new WaitForSeconds(0.1f);
    }

    public void DealFifthCenterCard(int card)
    {
        StartCoroutine(DealFifthCenterCard(card.ToString()));
    }
    public IEnumerator DealFifthCenterCard(string card)
    {
        var go = Instantiate(pokerTpl);
        PokerItem p = go.GetComponent<PokerItem>();
        p.RenderView(card);
        go.transform.SetParent(PokerPoolPanel);
        (go.transform as RectTransform).anchoredPosition3D = Vector3.zero;
        (go.transform as RectTransform).localScale = new Vector3(0.3f, 0.3f, 1);

        p.MyRotation.DealFifthCenterCard(PublicPokerPanel);
        publicPokers.Add(p);
        yield return new WaitForSeconds(0.1f);
    }
    #endregion
    public void RecyclePokers()
    {
        if (playersPokers.Count > 0)
        {
            foreach (PokerItem p in playersPokers)
            {
                p.gameObject.transform.SetParent(null);
                Destroy(p.gameObject);
            }
            playersPokers.Clear();
        }
        if (publicPokers.Count > 0)
        {
            foreach (PokerItem p in publicPokers)
            {
                p.gameObject.transform.SetParent(null);
                Destroy(p.gameObject);
            }
            publicPokers.Clear();
        }
    }

    #endregion
}
