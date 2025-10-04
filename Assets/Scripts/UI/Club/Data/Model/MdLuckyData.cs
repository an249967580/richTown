using System;
using System.Collections.Generic;

namespace RT
{
    public class MdLuckyData : MdList<ItemLuckyData>
    {
        public string game = GameType.dz;
        public long startTime;
        public long endTime;
        public DateTime now;
        public string key;

        public MdLuckyData()
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

        public void findList(Action<HttpResult<List<ItemLuckyData>>> action)
        {
            ClubApi.LuckyPlayer(ClubMainView.Instance.ClubId, game, key, startTime, endTime, pageSize, lastTime, lastId, action);
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

        long lastTime
        {
            get
            {
                if (IsEmpty)
                {
                    return 0;
                }
                return DataItems[Count - 1].startTime;
            }
        }
    }
}
