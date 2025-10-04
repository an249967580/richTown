using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class ItemBankChoice : MonoBehaviour
    {
        public Text tvName;
        public Image imgSelect;

        private Banker banker;

        private void Awake()
        {
            imgSelect.gameObject.SetActive(false);
        }

        public void InitView(Banker b)
        {
            banker = b;
            tvName.text = b.nickname;
        }

        public void SetSelected(bool selected)
        {
            imgSelect.gameObject.SetActive(selected);
        }
    }
}
