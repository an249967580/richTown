using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace RT
{
    public class JsonUtil<T>
    {
        public static T Deserialize(object jsonObject)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonObject.ToString());
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                return default(T);
            }
        }

        public static T Value(object jsonObject, string key)
        {
            try
            {
                return JObject.Parse(jsonObject.ToString()).Value<T>(key);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                return default(T);
            }
        }
    }
}
