using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RT
{
    [Serializable]
    public class TexasHistoryResp
    {
        [JsonProperty(PropertyName = "total")]
        public int Total;
        [JsonProperty(PropertyName = "list")]
        public List<TexasHistoryHandsData> List;
    }
}