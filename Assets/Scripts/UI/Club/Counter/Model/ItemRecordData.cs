
using Newtonsoft.Json;

namespace RT
{
    /// <summary>
    /// 收发记录列表项数据
    /// </summary>
    public class ItemRecordData
    {
        [JsonProperty("operator")]
        public string sender;
        [JsonProperty("nickname")]
        public string recipient;
        public long time;        // 操作时间
        public long coin;        // 筹码数量
        public int type;         // 类型 1 发放 2 回收
        public long id;
    }
}