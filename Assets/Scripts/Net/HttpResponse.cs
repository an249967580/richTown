using Newtonsoft.Json;

namespace RT
{
    [System.Serializable]
    public class HttpResponse
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { set; get; }

        [JsonProperty(PropertyName = "data")]
        public object Data { set; get; }
    }
}
