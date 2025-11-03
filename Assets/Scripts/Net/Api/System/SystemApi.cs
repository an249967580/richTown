using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RT
{
    public class SystemApi
    {

        // 获取国家
        public static void FindCountry(Action<HttpResult<List<ItemCountryData>>> action)
        {
            // Game.Instance.HttpReq.POST("public.php?_c=verify&_a=country", null, (rsp, error)=> {
            //     HttpResult<List<ItemCountryData>> ret = new HttpResult<List<ItemCountryData>>();
            //     ret.code = rsp.Code;
            //     if(ret.IsOk)
            //     {
            //         ret.data = JsonUtil<List<ItemCountryData>>.Deserialize(rsp.Data);
            //     }
            //     if(action != null)
            //     {
            //         action(ret);
            //     }
            // }, false);
            Game.Instance.HttpReq.POST("public.php?_c=verify&_a=country", null, (rsp, error) =>
            {
                HttpResult<List<ItemCountryData>> ret = new HttpResult<List<ItemCountryData>>();

                // 打印返回信息（无论成功失败）
                UnityEngine.Debug.Log($"HTTP 回调: code={rsp?.Code}, data={rsp?.Data}, error={error}");

                if (error != null)
                {
                    UnityEngine.Debug.LogError("HTTP 请求失败: " + error);
                }
                else
                {
                    ret.code = rsp.Code;
                    if (ret.IsOk)
                    {
                        ret.data = JsonUtil<List<ItemCountryData>>.Deserialize(rsp.Data);
                    }
                }

                if (action != null)
                {
                    action(ret);
                }
            }, false);
        }

        // 获取最新版本
        public static void GetLastVersion(int channel, int version, Action<HttpResult<Version>> action)
        {
            if (NetConfig.updateEnable)
            {
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("channel", channel.ToString());
                param.Add("versionCode", version.ToString());
                Game.Instance.HttpReq.POSTFullUrl(NetConfig.updateAdr, param, (rsp, error) =>
                {
                    HttpResult<Version> ret = new HttpResult<Version>();
                    ret.code = rsp.Code;
                    if (ret.IsOk)
                    {
                        ret.data = JsonUtil<Version>.Deserialize(rsp.Data);
                    }
                    if (action != null)
                    {
                        action(ret);
                    }
                }, false);
            }
            else
            {
                HttpResult<Version> ret = new HttpResult<Version>();
                ret.code = 200;
                ret.data = null;
                if (action != null)
                {
                    action(ret);
                }
            }
        }
    }
}
