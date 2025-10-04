using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class BullRoom : GameRoom
    {
        [JsonProperty(PropertyName = "baseBet")]
        public int BaseBet;
        [JsonProperty(PropertyName = "siteNum")]
        public int SeatNum;
    }
}