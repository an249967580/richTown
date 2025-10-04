using Newtonsoft.Json;

namespace RT
{ 
    public class GameLiveData : ItemData
    {

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar;
        [JsonProperty(PropertyName = "uid")]
        public long UId;
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname;
        [JsonProperty(PropertyName = "totalBuyBet")]
        public int TotalBuyBet;
        [JsonProperty(PropertyName = "profitLoss")]
        public int ProfitLoss;
        [JsonProperty(PropertyName = "handNum")]
        public int TotalWinHands;
    }
}
