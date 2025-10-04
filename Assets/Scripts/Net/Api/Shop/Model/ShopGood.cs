using Newtonsoft.Json;
using System;


namespace RT
{
    [Serializable]
    public class ShopGood
    {
        [JsonProperty(PropertyName = "id")]
        public int GoodId;//商品id

        [JsonProperty(PropertyName = "title")]
        public string Title;//商品名

        [JsonProperty(PropertyName = "stype")]
        public string Type;//商品类型

        [JsonProperty(PropertyName = "avatar")]
        public string Icon;//图标
        
        [JsonProperty(PropertyName = "num")]
        public int Num;//数量

        [JsonProperty(PropertyName = "extra")]
        public int Extra;//额外赠送

        [JsonProperty(PropertyName = "price")]
        public int Price;//商品价格

        [JsonProperty(PropertyName = "intro")]
        public string Intro;//介绍

        [JsonProperty(PropertyName = "param")]
        public object Param;//配置
    }
}
