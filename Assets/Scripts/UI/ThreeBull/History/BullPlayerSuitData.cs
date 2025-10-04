using Newtonsoft.Json;

namespace RT
{
    public class BullPlayerSuitData : ItemData
    {
        [JsonProperty(PropertyName = "uid")]
        public int UId;
        [JsonProperty(PropertyName = "cards")]
        public string[] Cards;
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname;
        [JsonProperty(PropertyName = "cardType_SG")]
        public string SGCardType;
        [JsonProperty(PropertyName = "cardType_COW")]
        public string NNCardType;
        [JsonProperty(PropertyName = "cardTypePoint_SG")]
        public int SGPoint;
        [JsonProperty(PropertyName = "cardTypePoint_COW")]
        public int NNPoint;
        [JsonProperty(PropertyName = "balance_SG")]
        public int SGBalance;
        [JsonProperty(PropertyName = "balance_COW")]
        public int NNBalance;


        [JsonIgnore]
        public int BaseBet;
        [JsonIgnore]
        public int SGBankUId;
        [JsonIgnore]
        public int NNBankUId;
        [JsonIgnore]
        public int SGOdds;
        [JsonIgnore]
        public int NNOdds;

        public string ConvertSGCardType(string cardType, int point)
        {
            string type = "没牛";
            switch (cardType)
            {
                case "THREE_CARD":
                    type = "三条";
                    break;
                case "JQK_CARD":
                    type = "三公";
                    break;
                case "TEN_CARD":
                    type = "十点";
                    break;
                case "NINE_CARD":
                    type = "九点";
                    break;
                default:
                    type = "散牌";
                    break;
            }

            return type;
        }
        public string ConvertNNCardType(string cardType, int point)
        {
            string type = "没牛";
            switch (cardType)
            {
                case "FIVE_CARD":
                    type = "五公";
                    break;
                case "COW_NDG_CARD":
                    type = "牛冬菇";
                    break;
                case "COW_BABY_TOW_CARD":
                    type = "牛宝宝A";
                    break;
                case "COW_BABY_CARD":
                    type = "牛宝宝";
                    break;
                case "COWCOW_CARD":
                    type = "牛牛";
                    break;
                case "SINGLE_CARD":
                    type = "没牛";
                    break;
                default:
                    type = "牛" + point;
                    break;
            }

            return type;
        }
    }

}