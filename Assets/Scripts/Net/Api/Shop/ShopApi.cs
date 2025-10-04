using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RT
{
    public class ShopApi
    {
        /// <summary>
        /// 获取商品列表
        /// </summary>
        /// <param name="callback"></param>
        public static void GetShopConfig(Action<List<ShopGood>,string> callback = null)
        {
            Game.Instance.HttpReq.POST("code.php?_c=shop&_a=list", null, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    List<ShopGood> list = new List<ShopGood>();
                    list = JsonConvert.DeserializeObject<List<ShopGood>>(resp.Data.ToString());

                    if (callback != null)
                    {
                        callback(list,null);
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
            });
        }
        /// <summary>
        /// 购买商店商品
        /// </summary>
        /// <param name="param">商品id</param>
        /// <param name="callback"></param>
        public static void BuyShopGood(Dictionary<string, string> param, Action<int,int,string> callback = null) {
            Game.Instance.HttpReq.POST("code.php?_c=shop&_a=buy", param, (resp, error) =>
            {
                if (error == null && resp.Code == 200)
                {
                    Dictionary<string,int> dic= JsonConvert.DeserializeObject<Dictionary<string, int>>(resp.Data.ToString());

                    if (callback != null)
                    {
                        callback(dic["rmb"],dic["gold"],"");
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        string msg = LocalizationManager.Instance.GetText(resp.Code.ToString());
                        msg = string.IsNullOrEmpty(msg) ? LocalizationManager.Instance.GetText("1012") : msg;
                        callback(0, 0, msg);
                    }
                }
            });
        }


        public static void CommitOrder(int id, Action<HttpResult<string>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=shop&_a=commit", param, (httpRsp, error) =>
            {
                HttpResult<string> rsp = new HttpResult<string>();
                rsp.code = httpRsp.Code;
                if (rsp.IsOk)
                {
                    rsp.data = JsonUtil<string>.Value(httpRsp.Data, "orderNo");
                }
                if (action != null)
                {
                    action(rsp);
                }
            });
        }

        public static void VerfyOrder(string orderNo, string payId, Action<HttpResult<OrderResult>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("orderNo", orderNo);
            param.Add("payId", payId);
            Game.Instance.HttpReq.POST("code.php?_c=shop&_a=verify", param, (httpRsp, error) =>
            {
                HttpResult<OrderResult> rsp = new HttpResult<OrderResult>();
                rsp.code = httpRsp.Code;
                if (rsp.IsOk)
                {
                    rsp.data = JsonUtil<OrderResult>.Deserialize(httpRsp.Data);
                }
                if (action != null)
                {
                    action(rsp);
                }
            });
        }

        public static void CommitAppleOrder(int id, Action<HttpResult<string>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=shop&_a=commit_a", param, (httpRsp, error) =>
            {
                HttpResult<string> rsp = new HttpResult<string>();
                rsp.code = httpRsp.Code;
                if (rsp.IsOk)
                {
                    rsp.data = JsonUtil<string>.Value(httpRsp.Data, "orderNo");
                }
                if (action != null)
                {
                    action(rsp);
                }
            });
        }

        public static void VerifyAppleOrder(string orderNo, string payId,string receipt, Action<HttpResult<OrderResult>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("orderNo", orderNo);
            param.Add("payId", payId);
            param.Add("receipt", receipt);

            Game.Instance.HttpReq.POST("code.php?_c=shop&_a=verify_a", param, (httpRsp, error) =>
            {
                HttpResult<OrderResult> rsp = new HttpResult<OrderResult>();
                rsp.code = httpRsp.Code;
                if (rsp.IsOk)
                {
                    rsp.data = JsonUtil<OrderResult>.Deserialize(httpRsp.Data);
                }
                if (action != null)
                {
                    action(rsp);
                }
            });
        }
    }
}
