using Newtonsoft.Json;
using SimpleJson;
using System;
using System.Collections.Generic;

namespace RT
{
    public class TexasApi
    {
        #region NODE接口
        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void EnterRoom(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.in", param, callback);
        }
        /// <summary>
        /// 公共房间快速开始
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void FastEnterPublicRoom(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.publicInRoomSiteDown", param, callback);
        }
        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void ExitRoom(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.out", param, callback);
        }
        /// <summary>
        /// 重连
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void ReConnect(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.backRoom", param, callback);
        }
        /// <summary>
        /// 坐下
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void SitDown(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.siteDown", param, callback);
        }
        /// <summary>
        /// 站起
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void StandUp(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.standUp", param, callback);
        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void StartGame(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.startGame", param, callback);
        }
        /// <summary>
        /// 授权买入
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void PreAddBet(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.preAddBet", param, callback);
        }
        /// <summary>
        /// 显示牌给别人
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void OpenHandCard(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.openHandCard", param, callback);
        }
        /// <summary>
        /// 游戏操作
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void DoGameOption(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_dzPoker.roomDzPokerHandler.gameOp", param, callback);
        }
        #endregion

        #region Php接口
        /// <summary>
        /// 获取桌子剩余时间
        /// </summary>
        /// <param name="callback"></param>
        public static void GetGameRoomTime(Dictionary<string, string> param, Action<int, string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=clubRoom&_a=getRoomTime", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    Dictionary<string, int> time = JsonConvert.DeserializeObject<Dictionary<string, int>>(resp.Data.ToString());

                    if (callback != null)
                    {
                        callback(time["time"], null);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(-1, msg);
                    }
                }
            }, false);
        }
        /// <summary>
        /// 设置桌子剩余时间
        /// </summary>
        /// <param name="callback"></param>
        public static void SetGameRoomTime(Dictionary<string, string> param, Action<string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=clubRoom&_a=setRoomTime", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {

                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(msg);
                    }
                }
            }, false);
        }
        /// <summary>
        /// 牌局结束获取统计信息
        /// </summary>
        /// <param name="callback"></param>
        public static void GetGameRoomStat(Dictionary<string, string> param, Action<List<GameLiveData>, string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=roomEnd", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    List<GameLiveData> list = new List<GameLiveData>();
                    list = JsonConvert.DeserializeObject<List<GameLiveData>>(resp.Data.ToString());

                    if (callback != null)
                    {
                        callback(list, null);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(null, msg);
                    }
                }
            }, false);
        }
        /// <summary>
        /// 获取牌局实时玩家数据
        /// </summary>
        /// <param name="callback"></param>
        public static void GetGameRoomLiveData(Dictionary<string, string> param, bool isPublic, Action<List<GameLiveData>, string> callback = null)
        {
            string url = isPublic ? "code.php?_c=clubData&_a=roomRealPublic" : "code.php?_c=clubData&_a=roomReal";
            Game.Instance.HttpReq.POST(url, param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    List<GameLiveData> list = new List<GameLiveData>();
                    list = JsonConvert.DeserializeObject<List<GameLiveData>>(resp.Data.ToString());

                    if (callback != null)
                    {
                        callback(list, null);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(null, msg);
                    }
                }
            }, false);
        }
        /// <summary>
        /// 获取牌局手牌历史
        /// </summary>
        /// <param name="callback"></param>
        public static void GetGameRoomHandsHistory(Dictionary<string, string> param, Action<TexasHistoryResp, string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=handList", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    TexasHistoryResp history = JsonConvert.DeserializeObject<TexasHistoryResp>(resp.Data.ToString());

                    if (callback != null)
                    {
                        callback(history, null);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(null, msg);
                    }
                }
            }, false);
        }
        
        public static void CreateTable(MdCreateTable md, Action<HttpResult<CreateError>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            // param.Add("clubId", md.clubId.ToString());
            param.Add("title", md.roomName);
            param.Add("pw", md.pin);
            param.Add("game", md.game);
            // param.Add("roomTime", md.time.ToString());
            param.Add("opTimeout", md.thinkTime.ToString());
            // param.Add("costScale", md.fee.ToString());
            param.Add("playerNum", md.playerNum.ToString());
            param.Add("blindBet", md.antes.ToString());
            param.Add("minBet", md.minChips.ToString());
            param.Add("maxBet", md.maxChips.ToString());
            // param.Add("enableBuy", md.isBuyIn ? "1" : "0");

            Game.Instance.HttpReq.POST("code.php?_c=clubRoom&_a=createPublic", param, (rsp, error) =>
            {
                HttpResult<CreateError> ret = new HttpResult<CreateError>();
                ret.code = rsp.Code;
                if (ret.code == 221 || ret.code == 222 || ret.code == 223)
                {
                    ret.data = JsonUtil<CreateError>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }
        #endregion
    }
}
