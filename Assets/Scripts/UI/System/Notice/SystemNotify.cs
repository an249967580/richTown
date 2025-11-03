using SimpleJson;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RT
{
    // 系统通知
    public class SystemNotify : MonoBehaviour
    {
        public static SystemNotify Instance;
        private void Awake()
        {
            Instance = this;
        }

        private DisconnectTip _dsTip;
        private SureView _sureView;
        private NoticeView _noticeView;
        private NoticeMsg _noticeMsg;

        public bool WantStopServer = false;

    

        RectTransform UGUI()
        {
            GameObject _ugui = GameObject.Find("_UGUI");
            if(_ugui)
            {
                return _ugui.GetComponent<RectTransform>();
            }
            return null;
        }

        /// <summary>
        /// 断线处理
        /// </summary>
        /// <param name="dsTipCallback"></param>
        public void ShowDisconnectTip(Action<DisconnectTip> dsTipCallback)
        {
            if (_dsTip)
            {
                return;
            }
            DisconnectTip tipPrefab = Resources.Load<DisconnectTip>("Prefabs/System/DisconnectTip");
            _dsTip = Instantiate(tipPrefab);
            RectTransform rt = _dsTip.GetComponent<RectTransform>();
            RectTransform _ugui = UGUI();
            if(_ugui)
            {
                rt.SetParent(_ugui);
                rt.localScale = Vector3.one;
                rt.anchoredPosition3D = Vector3.zero;
                if (Screen.orientation == ScreenOrientation.Portrait)
                {
                    rt.sizeDelta = new Vector2(750, 1334);
                }
                else
                {
                    rt.sizeDelta = new Vector2(1334, 750);
                }
                _dsTip.OnReconnectEvent = (tip) =>
                {
                    if(dsTipCallback != null)
                    {
                        dsTipCallback(_dsTip);
                    }
                };

                _dsTip.OnReconnectTimeout = () =>
                {
                    netError();
                };
            }
        }

        /// <summary>
        /// node 通知处理
        /// </summary>
        /// <param name="jObject"></param>
        public void OnMessageNotify(JsonObject jObject)
        {
            // 重复登录
            if (jObject["op"].ToString() == "login_repeat")
            {
                loginRepeat();
            }
            else if (jObject["op"].ToString() == "user_info")       // 货币赠送
            {
                currencyNotify(jObject["data"] as JsonObject);
            }
            else if(jObject["op"].ToString() == "public_room_dissolve")
            {
                NotificationCenter.Instance.DispatchNotify(NotificationType.public_room_dissolve, new NotifyMsg());
            }
            else if (jObject["op"].ToString() == "stop_server")  // 停服
            {
                string sceen = SceneManager.GetActiveScene().name;
                if (sceen != "TexasPokerScene" && sceen != "ThreeBullScene")
                {
                    StopServer();
                }
                else
                {
                    NotificationCenter.Instance.DispatchNotify(NotificationType.StopServer, new NotifyMsg());
                    WantStopServer = true;
                }
            }
            else if (jObject["op"].ToString() == "inform")       // 公告（跑马灯）
            {
                startNotice(jObject);
            }
            else if (jObject["op"].ToString() == "kick")
            {
                lockUser();
            }
            else
            {
                // 其他消息，转发到对应Scene中处理
                NotificationCenter.Instance.DispatchNotify(NotificationType.OnMsg, new NotifyMsg().value("msg", jObject));
            }
        }

        // 重复登录
        void loginRepeat()
        {
            if (SceneManager.GetActiveScene().name != "LoginScene")
            {
                if (Game.Instance.PomeloNode.pc != null)
                {
                    Game.Instance.PomeloNode.pc.disconnect();
                }
                Screen.orientation = ScreenOrientation.Portrait;
                Game.Instance.RemoveToken(true);
                Transfer.Instance[TransferKey.Kickout] = true;
                SceneManager.LoadScene("LoginScene");
            }
        }

        public bool HandleSpecialCode(int code)
        {
            return handleClubNotExists(code) || handleStopServer(code);
        }

        /// <summary>
        /// 停服处理
        /// </summary>
        bool handleStopServer(int code)
        {
            // 停服处理
            if (code == 100000)
            {
                string sceen = SceneManager.GetActiveScene().name;
                if (sceen != "TexasPokerScene" && sceen != "ThreeBullScene")
                {
                    StopServer();
                    return true;
                }
            }
            return false;
        }

        // 显示停服信息
        public void StopServer()
        {
            if (_sureView)
            {
                return;
            }
            GameObject go = Resources.Load("Prefabs/System/SureView") as GameObject;
            if (go)
            {
                RectTransform _ugui = UGUI();
                if (_ugui)
                {
                    _sureView = Instantiate(go).GetComponent<SureView>();
                    RectTransform rt = _sureView.GetComponent<RectTransform>();
                    rt.SetParent(_ugui);
                    rt.localScale = Vector3.one;
                    rt.anchoredPosition3D = Vector3.zero;
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        rt.sizeDelta = new Vector2(750, 1334);
                    }
                    else
                    {
                        rt.sizeDelta = new Vector2(1334, 750);
                    }
                    _sureView.ShowTip(LocalizationManager.Instance.GetText("100000"));
                    _sureView.AutoHide(10, LocalizationManager.Instance.GetText("5010"));
                    _sureView.OnSureEvent = () =>
                    {
                        Application.Quit();
                    };
                }
            }
        }

        // 俱乐部页面，俱乐部不存在或者俱乐部被解散/锁定
        bool handleClubNotExists(int code)
        {
            if (code == 209 || code == 231)
            {
                if (SceneManager.GetActiveScene().name == "ClubScene")
                {
                    GameObject go = Resources.Load("Prefabs/System/SureView") as GameObject;
                    if (go)
                    {
                        RectTransform _ugui = UGUI();
                        if (_ugui)
                        {
                            SureView vi = Instantiate(go).GetComponent<SureView>();
                            RectTransform rt = vi.GetComponent<RectTransform>();
                            rt.SetParent(_ugui);
                            rt.localScale = Vector3.one;
                            rt.anchoredPosition3D = Vector3.zero;
                            if (Screen.orientation == ScreenOrientation.Portrait)
                            {
                                rt.sizeDelta = new Vector2(750, 1334);
                            }
                            else
                            {
                                rt.sizeDelta = new Vector2(1334, 750);
                            }
                            vi.ShowTip(LocalizationManager.Instance.GetText("5022"));
                            vi.AutoHide(10, LocalizationManager.Instance.GetText("5010"));
                            vi.OnSureEvent = () =>
                            {
                                Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Club;
                                Transfer.Instance.Remove(TransferKey.ClubId);
                                Transfer.Instance.Remove(TransferKey.ClubInfo);
                                SceneManager.LoadScene("MainScene");
                            };
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        // 货币通知
        void currencyNotify(JsonObject jObject)
        {

            if (jObject.ContainsKey("rmb"))
            {
                long rmb = (long)jObject["rmb"];
                Game.Instance.CurPlayer.Diamond = rmb;
            }
            if (jObject.ContainsKey("gold"))
            {
                long gold = (long)jObject["gold"];
                Game.Instance.CurPlayer.Gold = gold;
            }
            // 发送消息，更新页面
            NotificationCenter.Instance.DispatchNotify(NotificationType.Currency, new NotifyMsg());
        }

        // 跑马灯效果
        void startNotice(JsonObject jObject)
        {
            NoticeMsg msg = JsonUtil<NoticeMsg>.Deserialize(jObject["data"]);
            if (msg == null)
                return;
            _noticeMsg = msg;
            if (_noticeView)
            {
                _noticeView.HideAndDestory();
            }
            showNotice();
        }
        
        // 显示公告
        void showNotice()
        {
            if (_noticeMsg == null || _noticeMsg.loop == 0)
            {
                _noticeMsg = null;
                return;
            }
            if (SceneManager.GetActiveScene().name == "LoginScene" || SceneManager.GetActiveScene().name == "LoadScene")
            {
                return;
            }
            if (_noticeView)
            {
                return;
            }
            GameObject go = Resources.Load("Prefabs/System/NoticeView") as GameObject;
            if (go)
            {
                Debug.Log("start");
                RectTransform _ugui = UGUI();
                if (_ugui)
                {
                    _noticeView = Instantiate(go).GetComponent<NoticeView>();
                    RectTransform rt = _noticeView.GetComponent<RectTransform>();
                    rt.SetParent(_ugui);
                    rt.localScale = Vector3.one;
                    rt.anchoredPosition3D = new Vector3(0, -30, 0);
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        rt.sizeDelta = new Vector2(550, 60);
                    }
                    else
                    {
                        rt.sizeDelta = new Vector2(1034, 60);
                    }
                    _noticeView.Msg = _noticeMsg;
                }
            }
        }

        // 鎖定用戶
        void lockUser()
        {
            GameObject go = Resources.Load("Prefabs/System/SureView") as GameObject;
            if (go)
            {
                RectTransform _ugui = UGUI();
                if (_ugui)
                {
                    SureView vi = Instantiate(go).GetComponent<SureView>();
                    RectTransform rt = vi.GetComponent<RectTransform>();
                    rt.SetParent(_ugui);
                    rt.localScale = Vector3.one;
                    rt.anchoredPosition3D = Vector3.zero;
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        rt.sizeDelta = new Vector2(750, 1334);
                    }
                    else
                    {
                        rt.sizeDelta = new Vector2(1334, 750);
                    }
                    vi.ShowTip(LocalizationManager.Instance.GetText("1021"));
                    vi.AutoHide(10, LocalizationManager.Instance.GetText("5010"));
                    vi.OnSureEvent = () =>
                    {   
                        Game.Instance.RemoveToken(true);
                        Application.Quit();
                    };
                }
            }
        }

        // 网络连接异常
        void netError()
        {
            GameObject go = Resources.Load("Prefabs/System/SureView") as GameObject;
            if (go)
            {
                RectTransform _ugui = UGUI();
                if (_ugui)
                {
                    SureView vi = Instantiate(go).GetComponent<SureView>();
                    RectTransform rt = vi.GetComponent<RectTransform>();
                    rt.SetParent(_ugui);
                    rt.localScale = Vector3.one;
                    rt.anchoredPosition3D = Vector3.zero;
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        rt.sizeDelta = new Vector2(750, 1334);
                    }
                    else
                    {
                        rt.sizeDelta = new Vector2(1334, 750);
                    }
                    vi.ShowTip(LocalizationManager.Instance.GetText("0"));
                    vi.OnSureEvent = () =>
                    {
                        Application.Quit();
                    };
                }
            }
        }

        private void Update()
        {
            if (_noticeMsg != null)
            {
                showNotice();
            }
        }
    }
}
