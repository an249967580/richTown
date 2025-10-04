using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class SGResult
    {
        [JsonProperty(PropertyName = "restBet")]
        public int RestBet;
        [JsonProperty(PropertyName = "winBet_SG")]
        public int WinBet;
        [JsonProperty(PropertyName = "loseBet_SG")]
        public int LoseBet;
        [JsonProperty(PropertyName = "thirdCard")]
        public int ThirdCard;
        [JsonProperty(PropertyName = "cardType_SG")]
        public string CardType;
        [JsonProperty(PropertyName = "isBanker")]
        public int IsBanker;
        [JsonProperty(PropertyName = "cardPoint_SG")]
        public int CardPoint;
    }
}
