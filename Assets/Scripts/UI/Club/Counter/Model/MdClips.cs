using System;
using System.Collections.Generic;

namespace RT
{
    public class MdClips : MdList<ItemMemberData>
    {
        public bool IsSend { get; set; }

        public string key = string.Empty;

        public void FindList(Action<HttpResult<List<ItemMemberData>>> action, bool showMask)
        {
            ClubApi.FindMembers(ClubMainView.Instance.ClubId, key, lastId, pageSize, lastRole, action);
        }

        long lastId
        {
            get
            {
                if (IsEmpty)
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

        public void SendCoins(long uid, long coins, Action<HttpResult<long>> action)
        {
            ClubApi.SendCoins(ClubMainView.Instance.ClubId, uid, coins, action);
        }

        public void RecycleCoins(long uid, long coins, Action<HttpResult<long>> action)
        {
            ClubApi.RecycleCoins(ClubMainView.Instance.ClubId, uid, coins, action);
        }

        public void UpdateCoins(long uid, long coins)
        {
            if (IsEmpty)
                return;
            for (int i = 0; i < Count; i++)
            {
                if (DataItems[i].uid == uid)
                {
                    DataItems[i].coin += coins;
                    break;
                }
            }
        }
    }
}
