using System;
using System.Collections.Generic;

namespace RT
{
    public class MdApplyList : MdList<ItemApplyData>
    {
        public void FindList(Action<HttpResult<List<ItemApplyData>>> action)
        {
            ClubApi.FindApplys(ClubMainView.Instance.ClubId, pageSize, lastId, action);
        }

        public void Agree(long applyId, Action<HttpResult<bool>> action)
        {
            ClubApi.ApplyAgree(applyId, action);
        }

        public void Reject(long applyId, Action<HttpResult<bool>> action)
        {
            ClubApi.ApplyReject(applyId, action);
        }

        public void Remove(long applyId)
        {
            if (IsEmpty)
                return;
            for (int i = 0; i < Count; i++)
            {
                if (DataItems[i].applyId == applyId)
                {
                    DataItems.RemoveAt(i);
                    break;
                }
            }
        }

        long lastId
        {
            get
            {
                if(IsEmpty)
                {
                    return 0;
                }
                return DataItems[Count - 1].applyId;
            }
        }
    }
}
