using System;
using System.Collections.Generic;

namespace RT
{
    public class EmailApi
    {
        public static void FindEmails(long id, int pageSize, Action<HttpResult<List<ItemEmailData>>> action, bool showMask)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("pageSize", pageSize.ToString());
            param.Add("id", id.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=mail&_a=list", param, (rsp, error) =>
            {
                HttpResult<List<ItemEmailData>> ret = new HttpResult<List<ItemEmailData>>();
                ret.code = rsp.Code;
                if (rsp.Data != null)
                {
                    ret.data = JsonUtil<List<ItemEmailData>>.Deserialize(rsp.Data);
                }
                else
                {
                    ret.data = new List<ItemEmailData>();
                }
                if (action != null)
                {
                    action(ret);
                }
            }, showMask);
        }

        public static void GetDetail(long id, Action<HttpResult<EmailDetail>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=mail&_a=read", param, (rsp, error) =>
            {
                HttpResult<EmailDetail> ret = new HttpResult<EmailDetail>();
                ret.code = rsp.Code;
                if (rsp.Data != null)
                {
                    ret.data = JsonUtil<EmailDetail>.Deserialize(rsp.Data);
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        public static void ReceiveDiamond(long id, Action<HttpResult<int>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=mail&_a=getRmb", param, (rsp, error) =>
            {
                HttpResult<int> ret = new HttpResult<int>();
                ret.code = rsp.Code;
                if (rsp.Data != null)
                {
                    ret.data = JsonUtil<int>.Value(rsp.Data, "rmb");
                }
                if (action != null)
                {
                    action(ret);
                }
            });
        }

        public static void FindAnnouncement(long id, int pageSize, Action<HttpResult<List<ItemAnnouncementData>>> action, bool showMask)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("pageSize", pageSize.ToString());
            param.Add("id", id.ToString());
            Game.Instance.HttpReq.POST("public.php?_c=notice&_a=list", param, (rsp, error) =>
            {
                HttpResult<List<ItemAnnouncementData>> ret = new HttpResult<List<ItemAnnouncementData>>();
                ret.code = rsp.Code;
                if (rsp.Data != null)
                {
                    ret.data = JsonUtil<List<ItemAnnouncementData>>.Deserialize(rsp.Data);
                }
                else
                {
                    ret.data = new List<ItemAnnouncementData>();
                }
                if (action != null)
                {
                    action(ret);
                }
            }, showMask);
        }

        public static void UnReadEmail(Action<HttpResult<int>> action)
        {
            Game.Instance.HttpReq.POST("code.php?_c=mail&_a=unread", null, (rsp, error) =>
            {
                HttpResult<int> ret = new HttpResult<int>();
                ret.code = rsp.Code;
                if (rsp.Data != null)
                {
                    ret.data = JsonUtil<int>.Value(rsp.Data, "num");
                }
                if (action != null)
                {
                    action(ret);
                }
            }, false);
        }

    }
}
