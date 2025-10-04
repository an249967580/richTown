using UnityEngine.UI;
namespace RT
{
    public class HelpView : HideMonoBehaviour
    {
        public Button btnClose;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
        }
    }
}
