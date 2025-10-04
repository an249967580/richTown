using Newtonsoft.Json;
using System.Collections.Generic;

namespace RT
{
    public class BullHistorySuitData : ItemData
    {

        [JsonProperty(PropertyName = "handId")]
        public int HandId;
        [JsonProperty(PropertyName = "baseBet")]
        public int BaseBet;
        [JsonProperty(PropertyName = "bankerUid_SG")]
        public int SGBankUId;
        [JsonProperty(PropertyName = "bankerUid_COW")]
        public int NNBankUId;
        [JsonProperty(PropertyName = "maxCallBankerOdds_SG")]
        public int SGOdds;
        [JsonProperty(PropertyName = "maxCallBankerOdds_COW")]
        public int NNOdds;


        [JsonProperty(PropertyName = "players")]
        public Dictionary<int,BullPlayerSuitData> DataList;
        [JsonIgnore]
        public int Index;
    }
}
