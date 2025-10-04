using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class GameRoom
    {
        [JsonProperty(PropertyName = "title")]
        public string RoomTitle;
        [JsonProperty(PropertyName = "game")]
        public string GameCode;
        [JsonProperty(PropertyName = "roomId")]
        public int RoomId;
        [JsonProperty(PropertyName = "clubId")]
        public int ClubId;
        [JsonProperty(PropertyName = "costScale")]
        public float CostScale;
        [JsonProperty(PropertyName = "minBet")]
        public int MinBet;
        [JsonProperty(PropertyName = "maxBet")]
        public int MaxBet;

        [JsonProperty(PropertyName = "roomTime")]
        public float RoomTime;
        [JsonProperty(PropertyName = "enableBuy")]
        public int EnableBuy;
        [JsonProperty(PropertyName = "opTimeout")]
        public int OpTimeout;
        [JsonProperty(PropertyName = "public")]
        public int IsPublic;
        [JsonProperty(PropertyName = "status")]
        public int Status;
        [JsonProperty(PropertyName = "isReConnected")]
        public bool IsReConnected = false;

        [JsonProperty(PropertyName = "bankerSite")]
        public int BankerSite;
    }
}