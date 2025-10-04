using Newtonsoft.Json;

namespace RT
{
    public class TexasPlayerHandsData : ItemData
    {
        [JsonProperty(PropertyName = "cards")]
        public string[] Cards;
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname;
        [JsonProperty(PropertyName = "balance")]
        public int Balance;
    }
}
