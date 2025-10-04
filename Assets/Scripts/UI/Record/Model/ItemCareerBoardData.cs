using Newtonsoft.Json;

namespace RT
{
    public class ItemCareerBoardData
    {
        public long id;
        [JsonProperty("clubTitle")]
        public string clubName;
        public string avatar;
        [JsonProperty("roomTitle")]
        public string roomName;
        public long startTime;
        [JsonProperty("roomTime")]
        public int boardTime;
        [JsonProperty("profitLoss")]
        public long profit;
        public int handNum;
        [JsonProperty("blindBet")]
        public int blinds;
        [JsonIgnore]
        public string game;
    }
}
