using Newtonsoft.Json;

namespace RT
{

    public class ItemClubData : ItemData
    {
        public long id;
        public long clubId;
        public string avatar;
        [JsonProperty("title")]
        public string name;
        public int level;
        public int expir;
        [JsonProperty("manage")]
        public int role;           // 0 普通会员 1 代理 2 创建者
        public long coin;          // 俱乐部币

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

        public override long Id()
        {
            return clubId;
        }
    }
}