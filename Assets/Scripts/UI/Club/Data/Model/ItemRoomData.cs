using Newtonsoft.Json;

namespace RT
{
    // 房间数据
    public class ItemRoomData
    {
        public long roomId;        
        public long startTime;  // 时间
        public float costScale;     // 抽水
        public int blindBet;      // 盲注或底注
        public long serviceCharge; // 服务费

        [JsonIgnore]
        public int Rate
        {
            get
            {
                float f = costScale * 100;
                return (int)f;
            }
        }

        [JsonIgnore]
        public string game;
    }
}
