using Newtonsoft.Json;

namespace RT
{

    public class ItemLevelCardData : ItemData
    {
        public int id;
        public string title;
        public string avatar;
        [JsonProperty("time_num")]
        public int expirDays;
        [JsonProperty("subagent_num")]
        public int managerNum;
        [JsonProperty("member_num")]
        public int memberNum;
        [JsonProperty("price")]
        public int diamond;
        public int level;

        public override long Id()
        {
            return id;
        }
    }
}