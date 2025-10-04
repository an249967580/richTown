using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RT
{

    public class ClubApi
    {

        #region  创建、编辑俱乐部信息

        /// <summary>
        /// 创建俱乐部
        /// </summary>
        public static void CreateClub(string name, string intro,long cid, Action<HttpResult<long>> rspAction)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("title", name);
            param.Add("intro", intro);
            param.Add("cid", cid.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=club&_a=create", param, (rsp, error) =>
            {
                HttpResult<long> ret = new HttpResult<long>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<long>.Value(rsp.Data, "clubId");
                }
                if (rspAction != null)
                {
                    rspAction(ret);
                }
            });
        }

        /// <summary>
        /// 编辑俱乐部信息
        /// </summary>
        public static void EditClub(long clubId, string name, string intro, Action<HttpResult<bool>> rspAction)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("title", name);
            param.Add("intro", intro);
            param.Add("clubId", clubId.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=club&_a=edit", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (rspAction != null)
                {
                    rspAction(ret);
                }
            });
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        public static void EditAvatar(long clubId, string avatar, Action<HttpResult<bool>> rspAction)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("avatar", avatar);
            param.Add("clubId", clubId.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=club&_a=edit", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (rspAction != null)
                {
                    rspAction(ret);
                }
            }, false);
        }

        /// <summary>
        /// 查找俱乐部
        /// </summary>
        public static void SearchClub(long clubId, Action<HttpResult<ClubSearch>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=club&_a=search", param, (rsp, error) =>
            {
                HttpResult<ClubSearch> ret = new HttpResult<ClubSearch>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<ClubSearch>.Deserialize(rsp.Data);
                }
                else
                {
                    ret.errorMsg = "找不到该俱乐部";
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 解散俱乐部
        /// </summary>
        public static void DisbandClub(long clubId, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=club&_a=delete", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 退出俱乐部
        /// </summary>
        public static void QuitClub(long clubId, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubMember&_a=quit", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }


        /// <summary>
        /// 申请加入俱乐部
        /// </summary>
        public static void ApplyJoinClub(long clubId, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubApply&_a=join", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 获取我加入的俱乐部
        /// </summary>
        public static void FindMyClubs(long lastId, int lastRole, int pageSize, Action<HttpResult<List<ItemClubData>>> action, bool showMask)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", lastId.ToString());
            param.Add("pageSize", pageSize.ToString());
            param.Add("manage", lastRole.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=club&_a=list", param, (rsp, error) =>
            {
                HttpResult<List<ItemClubData>> ret = new HttpResult<List<ItemClubData>>();
                ret.code = rsp.Code;
                if (rsp.Data != null)
                {
                    ret.data = JsonUtil<List<ItemClubData>>.Deserialize(rsp.Data);
                }
                else
                {
                    ret.data = new List<ItemClubData>();
                }
                if (action != null)
                {
                    action(ret);
                }
            }, showMask);
        }

        /// <summary>
        /// 获取俱乐部详情（俱乐部主页）
        /// </summary>
        public static void GetClubDetail(long clubId, Action<HttpResult<ClubDetail>> action, bool showMask = true)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=club&_a=detail", param, (rsp, error) =>
            {
                HttpResult<ClubDetail> ret = new HttpResult<ClubDetail>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<ClubDetail>.Deserialize(rsp.Data);
                    if (ret.data != null)
                    {
                        string purview = JsonUtil<string>.Value(rsp.Data, "purview");
                        if (ret.data != null)
                        {
                            ret.data.purview = JsonConvert.DeserializeObject<List<uint>>(purview);
                        }
                    }
                }
                if (action != null)
                {
                    action(ret);
                }
            }, showMask);
        }


        #endregion

        #region 俱乐部币兑换
        /// <summary>
        /// 兑换俱乐部币
        /// </summary>
        public static void ExchangeCoin(long clubId, long diamond, Action<HttpResult<ExchangeResult>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("rmb", diamond.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=club&_a=torb", param, (rsp, error) =>
            {
                HttpResult<ExchangeResult> ret = new HttpResult<ExchangeResult>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<ExchangeResult>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 兑换钻石
        /// </summary>
        public static void ExchangeDiamond(long clubId, long coins, Action<HttpResult<ExchangeResult>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("rb", coins.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=club&_a=tormb", param, (rsp, error) =>
            {
                HttpResult<ExchangeResult> ret = new HttpResult<ExchangeResult>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<ExchangeResult>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        #endregion

        #region 柜台

        /// <summary>
        /// 发放或回收
        /// </summary>
        /// <param name="type">0 发放 1 回收</param>
        public static void SendCoins(long clubId, long targetId, long coinsCount, Action<HttpResult<long>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("uid", targetId.ToString());
            param.Add("coin", coinsCount.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubCounter&_a=send", param, (rsp, error) =>
            {
                HttpResult<long> ret = new HttpResult<long>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<long>.Value(rsp.Data, "rb");
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 回收筹码
        /// </summary>
        public static void RecycleCoins(long clubId, long targetId, long coinsCount, Action<HttpResult<long>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("uid", targetId.ToString());
            param.Add("coin", coinsCount.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubCounter&_a=recycle", param, (rsp, error) =>
            {
                HttpResult<long> ret = new HttpResult<long>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<long>.Value(rsp.Data, "rb");
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 收发记录
        /// </summary>
        public static void FindSendOrRecycleRecord(long clubId, int pageSize, long lastId, Action<HttpResult<List<ItemRecordData>>> action, bool showMask = true)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("pageSize", pageSize.ToString());
            param.Add("id", lastId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubCounter&_a=coinRecord", param, (rsp, error) =>
            {
                HttpResult<List<ItemRecordData>> ret = new HttpResult<List<ItemRecordData>>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<List<ItemRecordData>>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            }, showMask);
        }

        #endregion

        #region 俱乐部会员

        /// <summary>
        /// 获取/搜索俱乐部会员
        /// </summary>
        public static void FindMembers(long clubId, string key, long id, int pageSize, int manage, Action<HttpResult<List<ItemMemberData>>> action, bool showMask = true)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("keyword", key);
            param.Add("id", id.ToString());
            param.Add("pageSize", pageSize.ToString());
            param.Add("manage", manage.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubMember&_a=list", param, (rsp, error) =>
            {
                HttpResult<List<ItemMemberData>> ret = new HttpResult<List<ItemMemberData>>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<List<ItemMemberData>>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            }, showMask);
        }

        /// <summary>
        /// 获取会员信息
        /// </summary>
        public static void GetMemberInfo(long clubId, long targetId, Action<HttpResult<MemberInfo>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("uid", targetId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubMember&_a=info", param, (rsp, error) =>
            {
                HttpResult<MemberInfo> ret = new HttpResult<MemberInfo>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<MemberInfo>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 设置会员备注
        /// </summary>
        public static void SetMemberNote(long clubId, long targetId, string note, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("uid", targetId.ToString());
            param.Add("note", note);

            Game.Instance.HttpReq.POST("code.php?_c=clubMember&_a=note", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 踢出会员
        /// </summary>
        public static void KickoutMember(long clubId, long targetId, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("uid", targetId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubMember&_a=delete", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        public static void SetupProxy(long clubId, long targetId, List<uint> auths, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("uid", targetId.ToString());
            param.Add("purview", JsonConvert.SerializeObject(auths));

            Game.Instance.HttpReq.POST("code.php?_c=clubMember&_a=setSubagent", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 取消代理
        /// </summary>
        public static void UnSetupProxy(long clubId, long targetId, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("uid", targetId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubMember&_a=unsetSubagent", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        #endregion

        #region 申请信息
        /// <summary>
        /// 获取申请列表
        /// </summary>
        public static void FindApplys(long clubId,int pageSize, long lastId, Action<HttpResult<List<ItemApplyData>>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("pageSize", pageSize.ToString());
            param.Add("applyId", lastId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubApply&_a=list", param, (rsp, error) =>
            {
                HttpResult<List<ItemApplyData>> ret = new HttpResult<List<ItemApplyData>>();
                ret.code = rsp.Code;
                if (ret.IsOk && rsp.Data != null)
                {
                    ret.data = JsonUtil<List<ItemApplyData>>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 同意申请
        /// </summary>
        public static void ApplyAgree(long applyId, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("applyId", applyId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubApply&_a=agree", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 拒绝申请
        /// </summary>
        public static void ApplyReject(long applyId, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("applyId", applyId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubApply&_a=reject", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        #endregion

        #region 等级购买

        /// <summary>
        /// 获取等级卡
        /// </summary>
        public static void FindLevelCards(Action<HttpResult<List<ItemLevelCardData>>> action)
        {
            Game.Instance.HttpReq.POST("code.php?_c=clubLevel&_a=list",null, (rsp, error) =>
            {
                HttpResult<List<ItemLevelCardData>> ret = new HttpResult<List<ItemLevelCardData>>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<List<ItemLevelCardData>>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        /// <summary>
        /// 购买等级卡
        /// </summary>
        public static void BuyLevelCard(long clubId, long id, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id.ToString());
            param.Add("clubId", clubId.ToString());

            Game.Instance.HttpReq.POST("code.php?_c=clubLevel&_a=buy", param, (rsp, error) =>
           {
               HttpResult<bool> ret = new HttpResult<bool>();
               ret.code = rsp.Code;
               if (action != null)
               {
                   action(ret);
               }
           });
        }

        #endregion

        #region 创建桌子

        public static void CreateTable(MdCreateTable md, Action<HttpResult<CreateError>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", md.clubId.ToString());
            param.Add("title", md.roomName);
            param.Add("pw", md.pin);
            param.Add("game", md.game);
            param.Add("roomTime", md.time.ToString());
            param.Add("opTimeout", md.thinkTime.ToString());
            param.Add("costScale", md.fee.ToString());
            param.Add("playerNum", md.playerNum.ToString());
            param.Add("blindBet", md.antes.ToString());
            param.Add("minBet", md.minChips.ToString());
            param.Add("maxBet", md.maxChips.ToString());
            param.Add("enableBuy", md.isBuyIn ? "1" : "0");

            Game.Instance.HttpReq.POST("code.php?_c=clubRoom&_a=create", param, (rsp, error) =>
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

        public static void FindTables(long clubId, string game, Action<HttpResult<List<ItemTableData>>> action, bool showMask = false)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("game", game);
            Game.Instance.HttpReq.POST("code.php?_c=clubRoom&_a=list", param, (rsp, error) =>
            {
                HttpResult<List<ItemTableData>> ret = new HttpResult<List<ItemTableData>>();
                ret.code = rsp.Code;
                if (ret.IsOk && rsp.Data != null)
                {
                    ret.data = JsonUtil<List<ItemTableData>>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            }, showMask);
        }

        #endregion

        #region 俱乐部数据

        // 桌子数据
        public static void TableData(long clubId, string game, long startTime, long endTime, int pageSize, long roomId, Action<HttpResult<RoomData>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("pageSize", pageSize.ToString());
            param.Add("roomId", roomId.ToString());
            param.Add("game", game);
            param.Add("startTime", startTime.ToString());
            param.Add("endTime", endTime.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=roomList", param, (rsp, error) =>
            {
                HttpResult<RoomData> ret = new HttpResult<RoomData>();
                ret.code = rsp.Code;
                if (ret.IsOk && rsp.Data != null)
                {
                    ret.data = JsonUtil<RoomData>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            }, true);
        }

        // 每局数据（桌子详细数据）
        public static void BoardData(long roomId, int pageSize, long id, long profitLoss, Action<HttpResult<BoardData>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("roomId", roomId.ToString());
            param.Add("pageSize", pageSize.ToString());
            param.Add("id", id.ToString());
            param.Add("profitLoss", profitLoss.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=roomInfo", param, (rsp, error) =>
            {
                HttpResult<BoardData> ret = new HttpResult<BoardData>();
                ret.code = rsp.Code;
                if (ret.IsOk && rsp.Data != null)
                {
                    ret.data = JsonUtil<BoardData>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            }, true);
        }

        // 幸运玩家
        public static void LuckyPlayer(long clubId, string game, string key, long startTime, long endTime, int pageSize, long timeMin, long id, Action<HttpResult<List<ItemLuckyData>>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("game", game);
            param.Add("keyword", key);
            param.Add("startTime", startTime.ToString());
            param.Add("endTime", endTime.ToString());
            param.Add("pageSize", pageSize.ToString());
            param.Add("timeMin", timeMin.ToString());
            param.Add("id", id.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=luckyUser", param, (rsp, error) =>
            {
                HttpResult<List<ItemLuckyData>> ret = new HttpResult<List<ItemLuckyData>>();
                ret.code = rsp.Code;
                if (ret.IsOk && rsp.Data != null)
                {
                    ret.data = JsonUtil<List<ItemLuckyData>>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            }, true);
        }

        // 玩家输赢
        public static void MemberData(long clubId, string game, string key, long startTime, long endTime, int pageSize, long timeMin, long id, Action<HttpResult<List<ItemWinOrLossData>>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("game", game);
            param.Add("keyword", key);
            param.Add("startTime", startTime.ToString());
            param.Add("endTime", endTime.ToString());
            param.Add("pageSize", pageSize.ToString());
            param.Add("timeMin", timeMin.ToString());
            param.Add("id", id.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=roomUser", param, (rsp, error) =>
            {
                HttpResult<List<ItemWinOrLossData>> ret = new HttpResult<List<ItemWinOrLossData>>();
                ret.code = rsp.Code;
                if (ret.IsOk && rsp.Data != null)
                {
                    ret.data = JsonUtil<List<ItemWinOrLossData>>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            }, true);
        }

        #endregion

        #region 底注，最小/上限筹码关系

        public static void GetTableGonfig(long clubId, Action<HttpResult<BlindsRates>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=clubRoom&_a=multiple", param, (rsp, error) =>
            {
                HttpResult<BlindsRates> ret = new HttpResult<BlindsRates>();
                ret.code = rsp.Code;
                if (ret.IsOk)
                {
                    ret.data = JsonUtil<BlindsRates>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            }, false);
        }

        #endregion

        #region IpCheck
        public static void IpCheck(long clubId,int checkip, Action<HttpResult<bool>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("clubId", clubId.ToString());
            param.Add("checkip", checkip.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=club&_a=checkip", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            }, true);
        }
        #endregion
    }
}
