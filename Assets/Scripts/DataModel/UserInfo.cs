using Newtonsoft.Json;
using System;

namespace RT
{
    [Serializable]
    public class UserInfo
    {
        [JsonProperty(PropertyName = "pid")]
        public int Pid;//平台id

        [JsonProperty(PropertyName = "uid")]
        public int Uid;//用户id

        [JsonProperty(PropertyName = "username")]
        public string Username;//账号

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar;//头像地址

        [JsonProperty(PropertyName = "nickname")]
        public string NickName;//昵称

        [JsonProperty(PropertyName = "rmb")]
        public long Diamond;//钻石

        [JsonProperty(PropertyName = "gold")]
        public long Gold;//金币

        [JsonProperty(PropertyName = "token")]
        public string Token;//token

        [JsonProperty(PropertyName = "vip_card")]
        public int Vip;//会员类型

        [JsonProperty(PropertyName = "expir")]
        public long VipExpirTime;//会员过期时间

        [JsonProperty(PropertyName = "card_club_num")]
        public int VipClubNum;//会员可建俱乐部数目

        [JsonProperty(PropertyName = "card_emoji_num")]
        public int VipEmojiNum;//会员免费表情剩余数目

        [JsonProperty(PropertyName = "card_delay_num")]
        public int VipDelayNum;//会员免费超时剩余数目

        [JsonProperty(PropertyName = "is_look_undercard")]
        public int CanLookCard;//会员是否可以发来看看

        [JsonProperty(PropertyName = "sid")]
        public string SessionId;//session

        [JsonProperty(PropertyName = "login_num")]
        public long loginNum;

        [JsonProperty(PropertyName = "game_switch_dz")]
        public int DzGameEnable;//德州是否可用
        [JsonProperty(PropertyName = "game_switch_cow")]
        public int CowGameEnable;//牛牛是否可用
    }
}
