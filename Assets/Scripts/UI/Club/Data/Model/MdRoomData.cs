using System;
using System.Collections.Generic;

namespace RT
{
    public class MdRoomData : MdList<ItemRoomData>  
    {
        public string game = GameType.dz;
        public long startTime;
        public long endTime;
        public DateTime now;

        public MdRoomData()
        {
            now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

            startTime = TimeUtil.DateToSeconds(now);
            endTime = startTime + 24 * 60 * 60 - 1;
        }

        public void SetDate(DateTime date)
        {
            now = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            startTime = TimeUtil.DateToSeconds(now);
            endTime = startTime + 24 * 60 * 60 - 1;
        }

        public void findList(Action<HttpResult<RoomData>> action)
        {
            ClubApi.TableData(ClubMainView.Instance.ClubId, game, startTime, endTime, pageSize, lastId, action);
        }

        public void GetBorrdData(long roomId, Action<HttpResult<BoardData>> action)
        {
            ClubApi.BoardData(roomId, pageSize, 0, 0, action);
        }

        long lastId
        {
            get
            {
                if(IsEmpty)
                {
                    return 0;
                }
                return DataItems[Count - 1].roomId;
            }
        }
    }

    public class RoomData
    {
        public long startTotal;
        public long serviceChargeTotal;
        public long handTotal;

        public List<ItemRoomData> list;
    }
}
