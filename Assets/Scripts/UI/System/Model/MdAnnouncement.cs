using System;
using System.Collections.Generic;

namespace RT
{
    public class MdAnnouncement : MdList<ItemAnnouncementData>
    {
        public void FindList(Action<HttpResult<List<ItemAnnouncementData>>> action, bool showMask)
        {
            EmailApi.FindAnnouncement(lastId, pageSize, action, showMask);
        }

        long lastId
        {
            get
            {
                if (IsEmpty)
                {
                    return 0;
                }
                return DataItems[Count - 1].id;
            }
        }
    }
}
