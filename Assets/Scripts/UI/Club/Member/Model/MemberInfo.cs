using Newtonsoft.Json;

namespace RT
{
    public class MemberInfo
    {
        public int uid;             // 用户id
        public string nickname;     // 昵称
        public string avatar;       //"头像",
        public int serviceFee;      // 服务费
        public int handNum;         // 手牌数
        public int profit;          // 盈亏
        public int buyIn;           // 买入
        public int returnClips;     // 退码数
        public string note;         // 备注
        [JsonProperty("manage")]
        public int role;     // 0 普通 1 代理 2 创建者

        [JsonIgnore]
        public bool isCreator       // 是否创建者
        {
            get
            {
                return role == 2;
            }
        }
        [JsonIgnore]
        public bool isProxy   // 是否副代理
        {
            get
            {
                return role == 1;
            }
        }
    }
}
