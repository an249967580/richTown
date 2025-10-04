using UnityEngine;
using UnityEngine.UI;

namespace RT
{

    public class LocalizationText : MonoBehaviour {

        public string textKey;

        void Start()
        {
            GetComponent<Text>().text = Text;
        }

        public string Text
        {
            get
            {
                return LocalizationManager.Instance.GetText(textKey);
            }
        }

        public void SetText(string textKey)
        {
            this.textKey = textKey;
            GetComponent<Text>().text = Text;
        }
    }
}
