using Pomelo.DotNetClient;
using RT;
using SimpleJson;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PomeloMgr : MonoBehaviour
{

    private string host = NetConfig.pomeloHost;
    private int port = NetConfig.pomeloPort;

    public  PomeloClient pc = null;

    private string uid = "1";
    private string token = "pw";

    Queue<Dictionary<string,string>> PomeloQueue;

    public Queue<JsonObject> GameDataQueue;
    public Queue<JsonObject> SystemDataQueue;
    private Queue<Action> _queueAction;

    private void Start()
    {
        _queueAction = new Queue<Action>();
        PomeloQueue = new Queue<Dictionary<string, string>>();
        SystemDataQueue = new Queue<JsonObject>();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause && pc != null)
        {
            //pc.disconnect();
        }
    }
    //When quit, release resource
    void OnApplicationQuit()
    {
        if (pc != null)
        {
            pc.disconnect();
        }
    }

	void Update ()
    {
        if (PomeloQueue != null && PomeloQueue.Count > 0) {
            Dictionary<string, string> dic = PomeloQueue.Dequeue();
            string type = dic["type"];
            switch (type) {
                case "NetError":
                    {
                        if (SceneManager.GetActiveScene().name != "LoginScene")
                        {
                            SystemNotify.Instance.ShowDisconnectTip(reconnectCallback);
                        }
                        break;
                    }
                case "NetTimeOut":
                    {
                        Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1015"));
                        break;
                    }
                case "NetClosed":
                    {
                        if (SceneManager.GetActiveScene().name != "LoginScene")
                        {
                            SystemNotify.Instance.ShowDisconnectTip(reconnectCallback);
                        }
                        break;
                    }
                case "NetDisconnected":
                    {
                        if (SceneManager.GetActiveScene().name != "LoginScene")
                        {
                            SystemNotify.Instance.ShowDisconnectTip(reconnectCallback);
                        }
                        break;
                    }
                case "NetConnected": {
                        Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1014"));
                        break;
                    }
            }
        }

        if(SystemDataQueue.Count > 0)
        {
            SystemNotify.Instance.OnMessageNotify(SystemDataQueue.Dequeue());
        }

        if(_queueAction.Count > 0)
        {
            _queueAction.Dequeue()();
        }
    }

    public void Login(bool reconnect, Action<JsonObject> callback = null)
    {
        GameDataQueue = new Queue<JsonObject>();
        SystemDataQueue = new Queue<JsonObject>();
        if (pc != null)
        {
            pc.disconnect();
            pc = null;
        }
        pc = new PomeloClient();
        pc.NetWorkStateChangedEvent += (state) =>
        {
            //Debug.Log("pomelo network state---" + state);
            switch (state)
            {
                case NetWorkState.ERROR:
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("type", "NetError");
                        PomeloQueue.Enqueue(dic);
                        break;
                    }
                case NetWorkState.TIMEOUT:
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("type", "NetTimeOut");
                        PomeloQueue.Enqueue(dic);
                        break;
                    }
                case NetWorkState.CLOSED:
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("type", "NetClosed");
                        PomeloQueue.Enqueue(dic);
                        break;
                    }
                case NetWorkState.DISCONNECTED:
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("type", "NetDisconnected");
                        PomeloQueue.Enqueue(dic);
                        break;
                    }
                case NetWorkState.CONNECTED:
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("type", "NetConnected");
                        PomeloQueue.Enqueue(dic);
                        break;
                    }
            }
        };
        uid = Game.Instance.CurPlayer.Uid.ToString();
        token = Game.Instance.CurPlayer.Token;
        pc.initClient(host, port, () => {
            pc.connect(null, (data) => {
                JsonObject msg = new JsonObject();
                msg["uid"] = uid;
                msg["token"] = token;
                pc.request("connector.entryHandler.loginServer", msg, (jobject) =>
                {
                    if(callback != null)
                    {
                        callback(jobject);
                    }
                    if (reconnect)
                    {
                        backRoom(jobject);
                    }
                    
                });
                On("onData", (result) => {
                    result.Add("operation", 99);
                    GameDataQueue.Enqueue(result);
                });

                On("onMsg", (result) => {
                    if (result["op"].ToString() == "user_emoji")
                    {
                        result.Add("operation", 99);
                        GameDataQueue.Enqueue(result);
                    }
                    else if (result["op"].ToString() == "user_audio")
                    {
                        result.Add("operation", 99);
                        Debug.Log("msg:" + result);
                        GameDataQueue.Enqueue(result);
                    }
                    else if (result["op"].ToString() == "clubRoom_setRoomTime")
                    {
                        result.Add("operation", 99);
                        GameDataQueue.Enqueue(result);
                    }
                    else
                    {
                        SystemDataQueue.Enqueue(result);
                    }
                });
            });
        });
    }

    public void Logout(Action<JsonObject> callback = null)
    {
        GameDataQueue = new Queue<JsonObject>();
        SystemDataQueue = new Queue<JsonObject>();
        Game.Instance.RemoveToken(true);
        pc.disconnect();
    }

    public void On(string eventName, Action<SimpleJson.JsonObject> action)
    {
        pc.on(eventName, action);
    }

    // 重连
    void reconnectCallback(DisconnectTip dsTip)
    {
        Login(true, (rsp) =>
        {
            if (int.Parse(rsp["code"].ToString()) == 200)
            {
                _queueAction.Enqueue(() =>
                {
                    dsTip.HideAndDestory();
                });
            }
        });
    }

    void backRoom(JsonObject jobject)
    {
        // 还在房间内，进行backroom
        if (jobject.ContainsKey("roomInfo"))
        {
            //断线重连
            JsonObject roomInfo = jobject["roomInfo"] as JsonObject;
            roomInfo.Add("clubChips", 0);
            roomInfo.Add("isReConnected", true);
            Transfer.Instance[TransferKey.RoomInfo] = roomInfo;
            string game = roomInfo["game"] as string;
            if (Validate.IsNotEmpty(game))
            {
                if (GameType.IsDz(game))
                {
                    JsonObject jo = new JsonObject();
                    jo.Add("operation", 100);
                    jo.Add("mod", "game_dzPoker");
                    GameDataQueue.Enqueue(jo);
                }
                else if (GameType.IsBull(game))
                {
                    JsonObject jo = new JsonObject();
                    jo.Add("operation", 101);
                    jo.Add("mod", "game_cowWater");
                    GameDataQueue.Enqueue(jo);
                }
            }
        }
        else
        {
            _queueAction.Enqueue(() =>
            {
                // 不再房间内，跳到之前的scene 或者 跳到主页
                if (SceneManager.GetActiveScene().name == "LoginScene"|| SceneManager.GetActiveScene().name == "TexasPokerScene" || SceneManager.GetActiveScene().name == "ThreeBullScene")
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
            });
        }
    }
   
}
