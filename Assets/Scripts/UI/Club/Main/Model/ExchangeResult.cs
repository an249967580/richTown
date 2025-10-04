using Newtonsoft.Json;

namespace RT
{
    public class ExchangeResult
    {
        [JsonProperty("rb")]
        public int clubCoins;    // 俱乐部币
        [JsonProperty("rmb")]
        public int diamond;      // 俱乐部钻石
    }
}
