using Newtonsoft.Json;
using SimpleJson;
using System;
using System.Collections.Generic;

namespace RT
{
    public class BullApi
    {
        #region NODE接口
        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void EnterRoom(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.in", param, callback);
        }
        /// <summary>
        /// 公共房间快速开始
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void FastEnterPublicRoom(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.publicInRoomSiteDown", param, callback);
        }
        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void ExitRoom(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.out", param, callback);
        }
        /// <summary>
        /// 重连
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void ReConnect(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.backRoom", param, callback);
        }
        /// <summary>
        /// 坐下
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void SitDown(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.siteDown", param, callback);
        }
        /// <summary>
        /// 站起
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void StandUp(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.standUp", param, callback);
        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void StartGame(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.startGame", param, callback);
        }
        /// <summary>
        /// 授权买入
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void PreAddBet(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.preAddBet", param, callback);
        }
        /// <summary>
        /// 游戏操作
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void DoGameOption(JsonObject param, Action<JsonObject> callback)
        {
            Game.Instance.PomeloNode.pc.request("game_cowWater.roomCowWaterHandler.gameOp", param, callback);
        }
        #endregion


        /// <summary>
        /// 获取牌局手牌历史
        /// </summary>
        /// <param name="callback"></param>
        public static void GetGameRoomHandsHistory(Dictionary<string, string> param, Action<BullHistoryResp, string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=handListCow", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    BullHistoryResp history = JsonConvert.DeserializeObject<BullHistoryResp>(resp.Data.ToString());

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
    }
}
