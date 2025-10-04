using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace RT
{

    public class LocalizationManager : Singleton<LocalizationManager>
    {
        private string _language;
        private Dictionary<string, string> _dictLanguage;
        private readonly string _languageDir = "Localization";
        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                parse();
            }
        }

        public LocalizationManager()
        {
            _dictLanguage = new Dictionary<string, string>();
            ResetLanguage();
        }

        public void ResetLanguage()
        {
            Language = PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetString("Language") : "SC";
        }

        private void parse()
        {
            if(string.IsNullOrEmpty(_language))
            {
                Debug.LogError("语言选择错误：" + _language);
                return;
            }
            var dir = _languageDir + "/" + _language;
            TextAsset asset = Resources.Load<TextAsset>(dir);
            if(!asset)
            {
                Debug.LogError("语言文件不存在：" + dir);
                return;
            }

            _dictLanguage = JsonConvert.DeserializeObject<Dictionary<string, string>>(asset.text);
        }

        public string GetText(string key)
        {
            string text;
            if(string.IsNullOrEmpty(key))
            {
                Debug.LogError("key is empty");
                return string.Empty;
            }
            if(_dictLanguage.TryGetValue(key, out text))
            {
                return text;
            }
            return string.Empty;
        }

        public string GetText(string key, string def)
        {
            string text = GetText(key);
            if(Validate.IsEmpty(text))
            {
                text = GetText(def);
            }
            return text;
        }

    }
}
