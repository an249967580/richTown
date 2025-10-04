using UnityEngine.UI;

namespace RT
{
    public class AboutView : HideMonoBehaviour
    {
        public Button btnClose;
        public Text tvVersion, tvEmail, tvWww;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
        }
    }
}
