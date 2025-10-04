using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class EmojiUse
    {
        [JsonProperty(PropertyName = "fromUID")]
        public int FromUId;//使用用户id

        [JsonProperty(PropertyName = "toUID")]
        public int ToUId;//对象用户id

        [JsonProperty(PropertyName = "emoji")]
        public string Emoji;//表情名
    }
}
