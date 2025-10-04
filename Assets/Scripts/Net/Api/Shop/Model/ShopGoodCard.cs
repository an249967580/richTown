using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class ShopGoodCard
    {
        [JsonProperty(PropertyName = "id")]
        public int GoodId;//商品id

        [JsonProperty(PropertyName = "title")]
        public string Title;//商品名

        [JsonProperty(PropertyName = "icon")]
        public string Icon;//图标

        [JsonProperty(PropertyName = "price")]
        public int Price;//商品价格

        [JsonProperty(PropertyName = "clubnum")]
        public int ClubNum;//可创建俱乐部数

        [JsonProperty(PropertyName = "emojinum")]
        public int EmojiNum;//免费表情数目

        [JsonProperty(PropertyName = "delaynum")]
        public int DelayNum;//免费延时次数

        [JsonProperty(PropertyName = "undercard")]
        public int Undercard;//是否看公共牌

        [JsonProperty(PropertyName = "vip")]
        public int Vip;//vip等级
    }
}
