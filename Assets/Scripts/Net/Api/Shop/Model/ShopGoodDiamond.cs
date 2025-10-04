using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class ShopGoodDiamond
    {
        [JsonProperty(PropertyName = "id")]
        public int GoodId;//商品id

        [JsonProperty(PropertyName = "title")]
        public string Title;//商品名

        [JsonProperty(PropertyName = "icon")]
        public string Icon;//图标

        [JsonProperty(PropertyName = "price")]
        public int Price;//商品价格

        [JsonProperty(PropertyName = "diamond")]
        public int Diamond;//兑换钻石数

        [JsonProperty(PropertyName = "gift")]
        public int Gift;//赠送钻石数

        [JsonProperty(PropertyName = "appleItemId")]
        public string AppleIAPId;//苹果内购商品Id
    }
}
