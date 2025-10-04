using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RT
{
    public class ThirdLogin : MonoBehaviour
    {
        public void ThirdSuccess(string platformInfo)
        {
            JObject jObj = JObject.Parse(platformInfo);
            string platform = jObj.Value<string>("platform");
            string platformId = jObj.Value<string>("platformId");
        }

        public void ThirdError(string msg)
        {
            switch(msg)
            {
                case "Failed":
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1114"));
                    break;
                case "Cancel":
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1115"));
                    break;
                case "NoApp":
                    Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1116"));
                    break;
                case "Start":
                    // Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1117"));
                    break;
            }
        }
    }
}
