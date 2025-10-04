using Newtonsoft.Json;
using System;

namespace RT
{
    public enum BullPlayerState
    {
        Normal = 0,
        Fold = 1,//弃
        Bank = 2,//庄
        Wait = 3,
        Look = 4,
        PreStandUp = 5
    };

    [Serializable]
    public class BullPlayer
    {
        [JsonProperty(PropertyName = "uid")]
        public int UId;
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname;
        [JsonProperty(PropertyName = "bet")]
        public int Bet;//当前携带筹码
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
        public int[] Cards;//扑克
        [JsonIgnore]
        public BullPlayerState State;
    }
}
