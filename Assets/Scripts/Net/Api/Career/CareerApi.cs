using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RT
{
    public class CareerApi 
    {
        public static void GetCareer(string game, Action<HttpResult<Career>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("game", game);
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=scoreInfo", param, (rsp, error) =>
            {
                HttpResult<Career> result = new HttpResult<Career>();
                result.code = rsp.Code;
                if (result.IsOk)
                {
                    result.data = JsonConvert.DeserializeObject<Career>(rsp.Data.ToString());
                }
                if (action != null)
                {
                    action(result);
                }
            });
        }

        public static void FindCareerData(string game, long id, int pageSize, Action<HttpResult<CareerData>> action)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("game", game);
            param.Add("id", id.ToString());
            param.Add("pageSize", pageSize.ToString());
            Game.Instance.HttpReq.POST("code.php?_c=clubData&_a=scoreList", param, (rsp, error) =>
            {
                HttpResult<CareerData> result = new HttpResult<CareerData>();
                result.code = rsp.Code;
                if (result.IsOk)
                {
                    result.data = JsonConvert.DeserializeObject<CareerData>(rsp.Data.ToString());
                }
                if (action != null)
                {
                    action(result);
                }
            });
        }
    }
}
