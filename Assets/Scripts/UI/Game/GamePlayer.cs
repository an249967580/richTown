using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class GamePlayer
    {
        [JsonProperty(PropertyName = "uid")]
        public int UId;//用户id
        
        [JsonProperty(PropertyName = "avatar")]
        public string Avatar;//头像地址
        [JsonProperty(PropertyName = "nickname")]
        public string Nickname;//昵称
        [JsonProperty(PropertyName = "audio")]
        public string LastAudio;//最近语音
        

        [JsonProperty(PropertyName = "enterPool")]
        public int EnterPool;//入池率
        [JsonProperty(PropertyName = "handTotal")]
        public int HandTotal;//手牌数
        [JsonProperty(PropertyName = "roomTotal")]
        public int RoomTotal;//牌局数
    }
}

