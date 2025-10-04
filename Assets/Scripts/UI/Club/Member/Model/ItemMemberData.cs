using Newtonsoft.Json;

namespace RT
{
    /// <summary>
    /// 会员数据
    /// </summary>
    public class ItemMemberData
    {
        public long id;
        public long uid;
        public string avatar;
        public string nickname;
        public long serviceFee;
        public long profit;
        public long loss;
        public long coin;
        [JsonProperty("manage")]
        public int role;

        [JsonIgnore]
        public bool isChecked;

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
    }
}
