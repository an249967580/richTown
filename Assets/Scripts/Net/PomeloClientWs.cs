// using System;
// using System.Collections.Generic;
// using WebSocketSharp;
// using SimpleJson;

// public enum NetWorkState
// {
//     ERROR,
//     TIMEOUT,
//     CLOSED,
//     DISCONNECTED,
//     CONNECTED
// }

// public class PomeloClientWS
// {
//     private WebSocket ws;
//     private string url;
//     private int reqId = 1;

//     // 事件分发表
//     private Dictionary<string, Action<JsonObject>> eventHandlers = new();
//     private Dictionary<int, Action<JsonObject>> pendingRequests = new();

//     public Action<NetWorkState> NetWorkStateChangedEvent;

//     // 初始化
//     public void initClient(string host, int port, Action cb)
//     {
//         url = host.StartsWith("ws://") || host.StartsWith("wss://")
//             ? host
//             : $"ws://{host}:{port}";
//         cb?.Invoke();
//     }

//     // 建立连接
//     public void connect(JsonObject user, Action<JsonObject> cb)
//     {
//         ws = new WebSocket(url);

//         ws.OnOpen += (s, e) =>
//         {
//             NetWorkStateChangedEvent?.Invoke(NetWorkState.CONNECTED);
//             cb?.Invoke(new JsonObject { { "code", 200 } });
//         };

//         ws.OnMessage += (s, e) =>
//         {
//             if (!e.IsText) return;

//             var msg = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(e.Data);

//             // 如果有 requestId，走回调
//             if (msg.ContainsKey("reqId"))
//             {
//                 int rid = Convert.ToInt32(msg["reqId"]);
//                 if (pendingRequests.TryGetValue(rid, out var cbReq))
//                 {
//                     cbReq(msg);
//                     pendingRequests.Remove(rid);
//                     return;
//                 }
//             }

//             // 如果有 route，走事件
//             if (msg.ContainsKey("route"))
//             {
//                 string route = msg["route"].ToString();
//                 if (eventHandlers.TryGetValue(route, out var cbEvt))
//                 {
//                     cbEvt(msg);
//                 }
//             }
//         };

//         ws.OnError += (s, e) =>
//         {
//             NetWorkStateChangedEvent?.Invoke(NetWorkState.ERROR);
//         };

//         ws.OnClose += (s, e) =>
//         {
//             NetWorkStateChangedEvent?.Invoke(NetWorkState.CLOSED);
//         };

//         ws.ConnectAsync();
//     }

//     // 断开
//     public void disconnect()
//     {
//         if (ws != null && ws.IsAlive)
//         {
//             ws.Close();
//         }
//     }

//     // 请求
//     public void request(string route, JsonObject msg, Action<JsonObject> cb)
//     {
//         int rid = reqId++;
//         msg["route"] = route;
//         msg["reqId"] = rid;
//         pendingRequests[rid] = cb;

//         ws.Send(msg.ToString());
//     }

//     // 事件监听
//     public void on(string eventName, Action<JsonObject> cb)
//     {
//         eventHandlers[eventName] = cb;
//     }
// }
