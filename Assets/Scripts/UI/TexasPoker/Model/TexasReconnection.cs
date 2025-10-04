using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RT
{
    [Serializable]
    public class TexasReconnection
    {
        [JsonProperty(PropertyName = "sBlindBet")]
        public int SBlindBet;
        [JsonProperty(PropertyName = "sBlindSite")]
        public int SBlindSite;
        [JsonProperty(PropertyName = "bBlindBet")]
        public int BBlindBet;
        [JsonProperty(PropertyName = "bBlindSite")]
        public int BBlindSite;
        [JsonProperty(PropertyName = "bankerSite")]
        public int BankerSite;
        [JsonProperty(PropertyName = "curPlayerUID")]
        public int CurPlayerUID;
        [JsonProperty(PropertyName = "allBet")]
        public int AllBet;
        [JsonProperty(PropertyName = "allBetList")]
        public Dictionary<string, int> AllBetList;//当前游戏为止的赌注
        [JsonProperty(PropertyName = "underCardArr")]
        public int[] UnderCardArr;
        [JsonProperty(PropertyName = "overBetNum")]
        public int OverBetNum;
        [JsonProperty(PropertyName = "maxBetLimit")]
        public int MaxBetLimit;
        [JsonProperty(PropertyName = "ts")]
        public int Ts;
        [JsonProperty(PropertyName = "selfCards")]
        public int[] SelfCards;
        [JsonProperty(PropertyName = "curBetList")]
        public Dictionary<string,int> CurBetList;//当前游戏牌圈的各个玩家赌注
        [JsonProperty(PropertyName = "allin")]
        public Dictionary<string, int> Allin;
        [JsonProperty(PropertyName = "outPlayers")]
        public Dictionary<string, int> OutPlayers;
        [JsonProperty(PropertyName = "opArrForRound")]
        public Dictionary<string, string> OpArrForRound;
        [JsonProperty(PropertyName = "siteUids")]
        public List<int> SiteUids;
    }
}
