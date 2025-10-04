using Newtonsoft.Json;
using System;
using UnityEngine;

namespace RT
{
    [Serializable]
    public class NoticeMsg
    {
        public string msg;
        public int loop;
        [JsonProperty("loopTs")]
        public int duration;
        public string color;  


        public Color Color
        {
            get
            {
                switch(color)
                {
                    case "red":
                        return Color.red;
                    case "yellow":
                        return Color.yellow;
                    default:
                        return Color.white;
                }

            }
        }
    }
}
