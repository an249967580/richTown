using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RT
{
    public class UserApi
    {
        #region 登陆模块
        /// <summary>
        /// 发送登陆短信
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void SendSms(string phone, long cid, Action<string> callback = null)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", phone);
            param.Add("cid", cid.ToString());
            Game.Instance.HttpReq.POST("public.php?_c=verify&_a=send", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(msg);
                    }
                }
            });
        }

        /// <summary>
        /// 用户手机登陆
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void Login(Dictionary<string, string> param, Action<UserInfo,string> callback = null) {
            Game.Instance.HttpReq.POST("public.php?_c=login", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    UserInfo player = JsonConvert.DeserializeObject<UserInfo>(resp.Data.ToString());
                    player.Pid = 1;
                    if (callback != null)
                    {
                        callback(player, "");
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(null, msg);
                    }
                }
            });
        }

        // 第三方登錄
        public static void Login(string platform, string platformId, string platNick, Action<HttpResult<UserInfo>> callback)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("platform", platform);
            param.Add("platformId", platformId);
            param.Add("platnick", platNick);
            Game.Instance.HttpReq.POST("public.php?_c=login", param, (rsp, error) =>
            {
                HttpResult<UserInfo> ret = new HttpResult<UserInfo>();
                ret.code = rsp.Code;
                if(ret.IsOk)
                {
                    ret.data = JsonConvert.DeserializeObject<UserInfo>(rsp.Data.ToString());
                    if(ret.data == null)
                    {
                        ret.code = -1;
                        ret.errorMsg = LocalizationManager.Instance.GetText("1012");
                    }
                }
                if(callback != null)
                {
                    callback(ret);
                }
            });
        }

        /// <summary>
        /// token自动登陆
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void AutoLogin(Dictionary<string, string> param, Action<UserInfo, string> callback = null)
        {
           
            Game.Instance.HttpReq.POST("public.php?_c=login", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    UserInfo player = JsonConvert.DeserializeObject<UserInfo>(resp.Data.ToString());
                    player.Pid = 1;
                    if (callback != null)
                    {
                        callback(player, "");
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(null, msg);
                    }
                }
            });
        }
        /// <summary>
        /// 用户退出
        /// </summary>
        /// <param name="callback"></param>
        public static void Logout( Action<string> callback = null)
        {
            Game.Instance.HttpReq.POST("public.php?_c=logout", null, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(msg);
                    }
                }
            });
        }
#endregion


        #region 用户资料模块
        /// <summary>
        /// 修改用户资料
        /// </summary>
        /// <param name="param"></param>
        /// <param name="callback"></param>
        public static void EditUserInfo(Dictionary<string, string> param, Action<string> callback = null) {
            Game.Instance.HttpReq.POST("code.php?_c=user&_a=edit", param, (resp, error) =>
            {
                if (resp == null)
                {
                    error = LocalizationManager.Instance.GetText("1012");
                }
                else
                {
                    if (error == null && (resp.Code == 200 || resp.Code == 211))
                    {
                        if (callback != null)
                        {
                            callback(null);
                        }
                    }
                    else
                    {
                        if (callback != null)
                        {
                            string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                            msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                            callback(msg);
                        }
                    }
                }
            });
        }
        #endregion

        #region 用户信息和表情使用
        public static void GetUserInfo(Dictionary<string, string> param, Action<GamePlayer, string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=user&_a=info", param, (resp, error) =>
            {
                if (resp == null)
                {
                    error = LocalizationManager.Instance.GetText("1012");
                }
                else
                {
                    if (error == null && (resp.Code == 200 || resp.Code == 211))
                    {
                        GamePlayer p = JsonConvert.DeserializeObject<GamePlayer>(resp.Data.ToString());
                        if (callback != null)
                        {
                            callback(p,null);
                        }
                    }
                    else
                    {
                        if (callback != null)
                        {
                            string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                            msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                            callback(null,msg);
                        }
                    }
                }
            });
        }
        public static void UseEmoji(Dictionary<string, string> param, Action<string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=user&_a=emoji", param, (resp, error) =>
            {
                if (resp == null)
                {
                    error = LocalizationManager.Instance.GetText("1012");
                }
                else
                {
                    if (error == null && (resp.Code == 200 || resp.Code == 211))
                    {
                        if (callback != null)
                        {
                            callback(null);
                        }
                    }
                    else
                    {
                        if (callback != null)
                        {
                            string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                            msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                            callback(msg);
                        }
                    }
                }
            });
        }
        #endregion

        #region 保存经纬度
        public static void SaveLocation(double latitue, double longitue, Action<HttpResult<bool>> action) 
        {
            Dictionary<string, string> param = new Dictionary<string, string>() ;
            param.Add("x", latitue.ToString());
            param.Add("y", longitue.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=user&_a=pos", param, (rsp, error) =>
            {
                HttpResult<bool> ret = new HttpResult<bool>();
                ret.code = rsp.Code;
                if (action != null)
                {
                    action(ret);
                }
            }, false);
        }
        #endregion
        #region 语音

        public static void SaveUserSound(Dictionary<string, string> param, Action<string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=user&_a=audio", param, (resp, error) =>
            {
                if (resp == null)
                {
                    error = LocalizationManager.Instance.GetText("1012");
                }
                else
                {
                    if (error == null && resp.Code == 200)
                    {
                        if (callback != null)
                        {
                            callback(null);
                        }
                    }
                    else
                    {
                        if (callback != null)
                        {
                            string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                            msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                            callback(msg);
                        }
                    }
                }
            }, false);
        }
        #endregion
    }
}
