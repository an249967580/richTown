using System;
using System.Collections.Generic;

namespace RT
{
    public class MdMemberList : MdList<ItemMemberData>
    {
        public string key = string.Empty;

        public void FindList(Action<HttpResult<List<ItemMemberData>>> action)
        {
            ClubApi.FindMembers(ClubMainView.Instance.ClubId, key, lastId, pageSize, lastRole, action);
        }

        public void GetMemberInfo(long uid, Action<HttpResult<MemberInfo>> action)
        {
            ClubApi.GetMemberInfo(ClubMainView.Instance.ClubId, uid, action);
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

        public void Remove(long uid)
        {
            if (IsEmpty)
                return;
            for (int i = 0; i < Count; i++)
            {
                if (DataItems[i].uid == uid)
                {
                    DataItems.RemoveAt(i);
                    break;
                }
            }
        }

        public void UpdateRole(long uid, bool isProxy)
        {
            if (IsEmpty)
                return;
            for (int i = 0; i < Count; i++)
            {
                if (DataItems[i].uid == uid)
                {
                    DataItems[i].role = isProxy ? 1 : 0;
                    break;
                }
            }
        }
    }
}
