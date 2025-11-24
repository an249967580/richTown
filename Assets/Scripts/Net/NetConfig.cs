using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RT
{
    public class NetConfig
    {
        // php 服务地址
        public static string serverAdr = "https://dzapi.7919.cn/";
        // public static string serverAdr = "http://129.204.21.168:20001/poker/";

        // pomelo 服务器
        public static string pomeloHost = "47.86.167.154";
        // public static string pomeloHost = "129.204.21.168";

        public static int pomeloPort = 10001;

        // 版本更新地址
        public static string updateAdr = "http://192.168.100.100:20005";
        public static bool updateEnable = true; // 是否开启检测更新
    }
}
