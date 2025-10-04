using System;
using System.Collections.Generic;

namespace RT
{
    public class MdHome : MdList<ItemClubData>
    {
        public void FindList(Action<HttpResult<List<ItemClubData>>> action, bool showMask)
        {
            ClubApi.FindMyClubs(lastId, lastRole, pageSize, action, showMask);
        }

        long lastId
        {
            get
            {
                if(IsEmpty)
                {
                    return 0;
                }
                return this[Count - 1].id;
            }
        }

        int lastRole
        {
            get
            {
                if (IsEmpty)
                {
                    return 0;
                }
                return this[Count - 1].role;
            }
        }
    }
}
