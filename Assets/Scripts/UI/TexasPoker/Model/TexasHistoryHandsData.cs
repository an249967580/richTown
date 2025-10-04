using Newtonsoft.Json;

namespace RT
{
    public class TexasHistoryHandsData : ItemData
    {

        [JsonProperty(PropertyName = "handId")]
        public int HandId;
        [JsonProperty(PropertyName = "cards")]
        public string[] Cards;
        [JsonProperty(PropertyName = "underCards")]
        public string[] UnderCards;
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname;
        [JsonProperty(PropertyName = "balance")]
        public int Balance;
        [JsonProperty(PropertyName = "otherList")]
        public TexasPlayerHandsData[] OtherList;
        [JsonIgnore]
        public int Index;
    }
}
