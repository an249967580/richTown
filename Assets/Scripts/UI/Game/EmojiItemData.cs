using Newtonsoft.Json;

namespace RT
{
    public class EmojiItemData : ItemData
    {

        [JsonProperty(PropertyName = "url")]
        public string ImgUrl;
        [JsonProperty(PropertyName = "emoji")]
        public string Emoji;
    }
}
