using System;

namespace RT
{
    [Serializable]
    public class Version
    {
        public string downloadUrl;   // apk下载地址 
        public int forceUpdate;      // 是否强制更新（0 否 1 是
        public string updateLog;     // 更新内容 
        public int versionCode;      // 版本号
        public string versionName;      // 版本名称
    }
}
