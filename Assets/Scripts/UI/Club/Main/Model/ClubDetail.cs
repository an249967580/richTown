using Newtonsoft.Json;
using System.Collections.Generic;

namespace RT
{
    public class ClubDetail   // 俱乐部详情
    {
        
        public long clubId;
        public string avatar;
        [JsonProperty("title")]
        public string name;
        public string creator;
        public int level;
        public int memberCount;
        public int memberLimit;
        public int expir;    // 过期时间
        public string intro;
        [JsonProperty("rb")]
        public long clubCoins;       // 俱乐部币
        public long coin;            // 个人俱乐部币
        [JsonProperty("manage")]
        public int role;
        [JsonProperty("rmbToclubRb")]
        public int diamondRate;            // 钻石:R币转换率=1:diamondRate 
        [JsonProperty("clubRbLeast")]
        public long minCoin;               // R币->钻石 最小兑换数量
        public long startTotal;
        public long serviceChargeTotal;
        public long handTotal;
        public int applyNum;
        public int checkip;
        public int luckyFlag;


        [JsonIgnore]
        public bool showLucky
        {
            get
            {
                return luckyFlag > 0;
            }
        }

        [JsonIgnore]
        public bool isCreator
        {
            get
            {
                return role == 2;
            }
        }
        [JsonIgnore]
        public bool isProxy
        {
            get
            {
                return role == 1;
            }
        }

        [JsonIgnore]
        public List<uint> purview;

        [JsonIgnore]
        public bool isExpir
        {
            get
            {
                return level > 0 && expir <= 0;
            }
        }
    }
}
