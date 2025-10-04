using System;
using System.Collections.Generic;
using System.Linq;

namespace RT
{
    public class MdCreateTable
    {
        public List<ItemDropData> FeeList;
        public List<ItemDropData> TimeList;

        public List<long> BlindList;
        public List<long> RateList;

        public string game;     // 游戏名
        public long fee;        // 抽水
        public long time;       // 牌局时间
        public string roomName; // 房间名
        public long thinkTime;  // 操作时间
        public long antes;      // 底注/盲注
        public long minChips;   // 最小筹码
        public long maxChips;   // 最大筹码
        public long playerNum;  // 游戏玩家数量
        public bool isBuyIn;    // 是否授权买入
        public bool isPin;      // 是否要密码
        public string pin=string.Empty;      // 密码

        public BlindsRates rates;

        public long clubId
        {
            get
            {
                return ClubMainView.Instance.ClubId;
            }
        }

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

        public MdCreateTable(string game)
        {
            BlindList = Blinds;
            RateList = Mutiple;
            FeeList = new List<ItemDropData>();
            TimeList = new List<ItemDropData>();
            this.game = game;
            init();
        }

        public void CreateTable(Action<HttpResult<CreateError>> action)
        {
            TexasApi.CreateTable(this, action);
        }


        #region 初始化数据
        void init()
        {
            
            // for(int i=0;i<ClubMainView.Instance.CostScale.Count;i++)
            // {
            //     ItemDropData fee = new ItemDropData();
            //     fee.title = ClubMainView.Instance.CostScale[i] + "%";
            //     fee.num = ClubMainView.Instance.CostScale[i];
            //     FeeList.Add(fee);
            // }

            // for (int i = 0; i < ClubMainView.Instance.RoomTime.Count; i++)
            // {
            //     ItemDropData time = new ItemDropData();
            //     time.title = ClubMainView.Instance.RoomTime[i] / 3600.0f + LocalizationManager.Instance.GetText("5021");
            //     time.num = ClubMainView.Instance.RoomTime[i];
            //     TimeList.Add(time);
            // }
        }
        #endregion

    }
}
