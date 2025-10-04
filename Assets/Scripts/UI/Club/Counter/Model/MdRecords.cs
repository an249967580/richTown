using System;
using System.Collections.Generic;

namespace RT
{
    public class MdRecords : MdList<ItemRecordData>
    {
        public void FindList(Action<HttpResult<List<ItemRecordData>>> action, bool showMask)
        {
            ClubApi.FindSendOrRecycleRecord(ClubMainView.Instance.ClubId, pageSize, lastId, action, showMask);
        }

        public long lastId
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
    }
}
