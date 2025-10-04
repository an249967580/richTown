using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class ShopGoodGold
    {
        [JsonProperty(PropertyName = "id")]
        public int GoodId;//商品id

        [JsonProperty(PropertyName = "title")]
        public string Title;//商品名

        [JsonProperty(PropertyName = "icon")]
        public string Icon;//图标

        [JsonProperty(PropertyName = "price")]
        public int Price;//商品价格

        [JsonProperty(PropertyName = "gold")]
        public int Gold;//兑换金币数

        [JsonProperty(PropertyName = "gift")]
        public int Gift;//赠送金币数
    }
}
