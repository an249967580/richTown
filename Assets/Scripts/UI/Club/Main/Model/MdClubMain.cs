using Newtonsoft.Json;
using SimpleJson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RT
{
    /// <summary>
    /// 底注倍数关系
    /// </summary>
    public class BlindsRates
    {
        [JsonProperty("blindBet")]
        public List<long> blinds;
        public List<long> multiple;
        public List<int> costScale;
        public List<long> roomTime;
    }

    public class MdClubMain : MdList<ItemTableData>
    {
        public TableType tableType = TableType.Texas;

        public ClubDetail detail
        {
            get;
            set;
        }
        public BlindsRates rates;

        public string game = GameType.dz;

        #region 权限相关

        public long clubId
        {
            get
            {
                return detail.clubId;
            }
        }

        public bool IsCreator
        {
            get
            {
                return detail.role == 2;
            }
        }

        public bool IsProxy
        {
            get
            {
                return detail.role == 1;
            }
        }

        public bool IsNormal {
            get
            {
                return detail.role == 0;
            }
        }

        /// <summary>
        /// 是否有权限
        /// </summary>
        public bool HasRight(Auth auth)
        {
            if (IsCreator)
            {
                return true;
            }

            if (IsProxy)
            {
                if (detail.purview != null)
                {
                    return detail.purview.Contains((uint)auth);
                }
            }
            return false;
        }

        #endregion

        #region 倍率相关

        public List<long> Blinds
        {
            get
            {
                if(rates != null && Validate.IsNotEmpty(rates.blinds))
                {
                    if(rates.blinds.Count == 12)
                    {
                        return rates.blinds;
                    }
                }
                long[] b = new long[] { 2, 4, 6, 8, 10, 20, 30, 50, 100, 200, 500, 1000 };
                return new List<long>(b.AsEnumerable());
            }
        }

        public List<long> Mutiple
        {
            get
            {
                if (rates != null && Validate.IsNotEmpty(rates.multiple))
                {
                    if (rates.multiple.Count == 12)
                    {
                        return rates.multiple;
                    }
                }
                long[] r = new long[] { 20, 50, 80, 100, 150, 200, 250, 300, 350, 400, 450, 500 };
                return new List<long>(r.AsEnumerable());
            }
        }

        public List<int> CostScale
        {
            get
            {
                if (rates != null && Validate.IsNotEmpty(rates.costScale))
                {
                   return rates.costScale;
                }
                int[] r = new int[] { 2, 3, 5 };
                return new List<int>(r.AsEnumerable());
            }
        }

        public List<long> RoomTime
        {
            get
            {
                if (rates != null && Validate.IsNotEmpty(rates.roomTime))
                {
                    return rates.roomTime;
                }
                long[] r = new long[] { 1800, 3600, 7200, 18000, 36000 };
                return new List<long>(r.AsEnumerable());
            }
        }

        #endregion

        #region http 请求

        public void GetDetail(long clubId, Action<HttpResult<ClubDetail>> action)
        {
            ClubApi.GetClubDetail(clubId, action);
        }

        // 上传头像
        public void UploadAvatar(byte[] bytes, Action<HttpResult<string>> httpPath)
        {
            AwsS3Service.Instance.AsyncUploadObject(bytes, string.Format("club/club_{0}/{1}_{2}.jpg", clubId, clubId, DateTime.Now.ToString("yyyyMMddHHmmss")), httpPath);
        }

        // 更新头像
        public void EditAvatar(string avatar, Action<HttpResult<bool>> action)
        {
            ClubApi.EditAvatar(clubId, avatar, action);
        }

        // 更新信息
        public void EditInfo(string name, string intro, Action<HttpResult<bool>> action)
        {
            ClubApi.EditClub(clubId, name, intro, action);
        }

        public void ExchangeCoin(int diamond, Action<HttpResult<ExchangeResult>> action)
        {
            ClubApi.ExchangeCoin(ClubMainView.Instance.ClubId, diamond, action);
        }

        public void ExchangeDiamond(int coin, Action<HttpResult<ExchangeResult>> action)
        {
            ClubApi.ExchangeDiamond(ClubMainView.Instance.ClubId, coin, action);
        }

        // 获取桌子配置
        public void GetTableGonfig(Action<HttpResult<BlindsRates>> action)
        {
            ClubApi.GetTableGonfig(ClubMainView.Instance.ClubId, action);
        }

        public void FindTables(Action<HttpResult<List<ItemTableData>>> action)
        {
            ClubApi.FindTables(clubId, game, action);
        }

        public void EnterRoom(long roomId, string pw, Action<JsonObject> action)
        {
            JsonObject param = new JsonObject();
            param.Add("roomId", roomId);
            param.Add("uid", Game.Instance.CurPlayer.Uid);
            param.Add("pw", pw);
            if (GameType.IsDz(game))
            {
                TexasApi.EnterRoom(param, action);
            }
            else
            {
                BullApi.EnterRoom(param, action);
            }
        }

        #endregion
    }
}
