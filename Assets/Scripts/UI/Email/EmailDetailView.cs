using UnityEngine;
using UnityEngine.UI;

namespace RT
{

    public delegate void ReceiveEvent(long emailId);

    public class EmailDetailView : HideMonoBehaviour
    {

        public Text tvTitle, tvContent, tvStatus, tvDiamond;
        public Button btnClose, btnReceive;
        public GameObject goDiamond;
        public ReceiveEvent OnReceiveEvent;
        EmailDetail _detail;

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            btnReceive.onClick.AddListener(() =>
            {
                if (OnReceiveEvent != null)
                {
                    OnReceiveEvent(_detail.id);
                }
            });
        }

        public void InitView(EmailDetail detail)
        {
            _detail = detail;
            tvTitle.text = detail.title;
            tvContent.text = detail.content;
            tvDiamond.text = detail.rmb.ToString();
            if (detail.rmb > 0)
            {
                goDiamond.gameObject.SetActive(true);
            }
            else
            {
                goDiamond.gameObject.SetActive(false);
            }

            SetReceive(detail.rmbFlag);
        }

        public void SetReceive(int flag)
        {
            if (flag > 0)
            {
                tvStatus.gameObject.SetActive(true);
                btnReceive.gameObject.SetActive(false);
            }
            else
            {
                tvStatus.gameObject.SetActive(false);
                btnReceive.gameObject.SetActive(true);
            }
        }
    }
}
