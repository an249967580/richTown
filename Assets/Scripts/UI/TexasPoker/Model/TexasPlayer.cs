using Newtonsoft.Json;
using System;

namespace RT
{
    public enum TexasPlayerState
    {
        Normal = 0,
        Fold = 1,//弃
        Allin = 2,//全
        Check = 3,//过
        Bet = 4,//下
        Raise = 5,//加
        Call = 6,//跟
        Wait = 7,
        Look = 8
    };

    [Serializable]
    public class TexasPlayer
    {
        [JsonProperty(PropertyName = "uid")]
        public int UId;
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname;
        [JsonProperty(PropertyName = "bet")]
        public int Bet;//当前携带筹码
        [JsonIgnore]
        public int DeskChip;//下注筹码
        [JsonProperty(PropertyName = "avatar")]
        public string Avatar;
        [JsonProperty(PropertyName = "site")]
        public int Site;
        [JsonProperty(PropertyName = "startEnable")]
        public int StartEnable;
        [JsonProperty(PropertyName = "state")]
        public int SState;//后台状态

        //用户坐标
        [JsonProperty(PropertyName = "pos_x")]
        public string Lat;
        [JsonProperty(PropertyName = "pos_y")]
        public string Lng;

        [JsonIgnore]
        public TexasPlayerState State;
    }
}
