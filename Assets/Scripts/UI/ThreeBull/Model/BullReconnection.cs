using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RT
{
    [Serializable]
    public class BullReconnection
    {
        [JsonProperty(PropertyName = "bankerSite")]
        public int BankerSite;
        [JsonProperty(PropertyName = "callBankerOdds_SG")]
        public int MySGBankOdds;
        [JsonProperty(PropertyName = "callBankerOdds_COW")]
        public int MyNNBankOdds;
        [JsonProperty(PropertyName = "maxCallBankerOdds_SG")]
        public int SGBankOdds;
        [JsonProperty(PropertyName = "maxCallBankerOdds_Cow")]
        public int NNBankOdds;
        [JsonProperty(PropertyName = "curRound")]
        public string CurRound;
        [JsonProperty(PropertyName = "ts")]
        public int Ts;
        [JsonProperty(PropertyName = "selfCards")]
        public int[] SelfCards;
        [JsonProperty(PropertyName = "otherPlayerCards")]
        public Dictionary<string,int[]> OtherPlayerCards;
        [JsonProperty(PropertyName = "playerBetList")]
        public Dictionary<string, int> PlayerBetList;//当前游戏为止的赌注
    }
}
