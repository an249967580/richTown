using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class NNResult
    {
        [JsonProperty(PropertyName = "restBet")]
        public int RestBet;
        [JsonProperty(PropertyName = "winBet_COW")]
        public int WinBet;
        [JsonProperty(PropertyName = "loseBet_COW")]
        public int LoseBet;
        [JsonProperty(PropertyName = "cards")]
        public int[] Cards;
        [JsonProperty(PropertyName = "cardType_COW")]
        public string CardType;
        [JsonProperty(PropertyName = "cardTypePoint_COW")]
        public int CardPoint;
        [JsonProperty(PropertyName = "isBanker")]
        public int IsBanker;
    }
}
