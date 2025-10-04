using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class TexasRoom : GameRoom
    {
        [JsonProperty(PropertyName = "blindBet")]
        public int BlindBet;
        [JsonProperty(PropertyName = "playerNum")]
        public int SeatNum;
    }
}
