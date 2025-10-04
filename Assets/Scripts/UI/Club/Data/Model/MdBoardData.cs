using System;

namespace RT
{
    public class MdBoardData : MdList<ItemBoardData>
    {
        public long roomId;

        public void FindList(Action<HttpResult<BoardData>> action)
        {
            ClubApi.BoardData(roomId, pageSize, lastId, lastProfit, action);
        }

        long lastId
        {
            get
            {
                if(IsEmpty)
                {
                    return 0;
                }
                return DataItems[Count - 1].id;
            }
        }

        long lastProfit
        {
            get
            {
                if (IsEmpty)
                {
                    return 0;
                }
                return DataItems[Count - 1].profitLoss;
            }
        }
    }
}
