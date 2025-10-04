using Newtonsoft.Json;

namespace RT
{
    public class WatcherData : ItemData
    {
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname;
        [JsonProperty(PropertyName = "avatar")]
        public string Avatar;
    }
}