using System.Collections.Generic;

namespace RT
{
    public class BoardData
    {
        public int blindBet;
        public float costScale;
        public int roomTime;
        public int buyBetTotal;
        public int serviceChargeTotal;
        public int userNumTotal;

        public int Rate
        {
            get
            {
                float f = costScale * 100;
                return (int)f;
            }
        }

        public List<ItemBoardData> list;
    }
}
