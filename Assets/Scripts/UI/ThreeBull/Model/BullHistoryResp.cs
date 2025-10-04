using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RT
{
    [Serializable]
    public class BullHistoryResp
    {
        [JsonProperty(PropertyName = "total")]
        public int Total;
        [JsonProperty(PropertyName = "list")]
        public List<BullHistorySuitData> List;
    }
}