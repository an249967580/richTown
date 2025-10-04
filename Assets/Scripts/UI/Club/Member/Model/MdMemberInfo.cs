using System;
using System.Collections.Generic;

namespace RT
{
    public class MdMemberInfo
    {
        public MemberInfo info
        {
            get;
            set;
        }

        public int uid
        {
            get
            {
                return info.uid;
            }
        }

        public void SetNote(string note, Action<HttpResult<bool>> action)
        {
            ClubApi.SetMemberNote(ClubMainView.Instance.ClubId, info.uid, note, action);
        }

        public void Kickout(Action<HttpResult<bool>> action)
        {
            ClubApi.KickoutMember(ClubMainView.Instance.ClubId, info.uid, action);
        }

        public void Proxy(List<uint> auths, Action<HttpResult<bool>> action)
        {
            ClubApi.SetupProxy(ClubMainView.Instance.ClubId, info.uid, auths, action);
        }

        public void UnProxy(Action<HttpResult<bool>> action)
        {
            ClubApi.UnSetupProxy(ClubMainView.Instance.ClubId, info.uid, action);
        }

    }
}
