using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class RecorderShowView : HideMonoBehaviour
    {
        public Image[] r;
        public Text tvSeconds, tvTip;

        private int _showIndex;
        private const float _maxLength = 10.0f;  // 10s
        private float _length = 0;

        public override void Show()
        {
            base.Show();
            ShowCancel(false);
            for (int i = 0; i < r.Length; i++)
            {
                r[i].gameObject.SetActive(true);
            }
            _length = 0;
            _showIndex = 2;
            StartCoroutine(countDown());
            InvokeRepeating("showR", 0.5f, 0.3f);
        }

        public override void Hide()
        {
            StopAllCoroutines();
            CancelInvoke("showR");
            base.Hide();
            ShowCancel(false);
            _showIndex = 2;
            _length = 0;
        }

        public void ShowCancel(bool isCancel)
        {
            if(isCancel)
            {
                tvTip.text = LocalizationManager.Instance.GetText("7003");
                tvTip.color = Color.red;
            }
            else
            {
                tvTip.text = LocalizationManager.Instance.GetText("7002");
                tvTip.color = Color.white;
            }
        }

        private void showR()
        {
            if (_showIndex > r.Length)
            {
                _showIndex = 2;
            }
            for (int i = 2; i < r.Length; i++)
            {
                if(i < _showIndex)
                {
                    r[i].gameObject.SetActive(true);
                }
                else
                {
                    r[i].gameObject.SetActive(false);
                }
            }
            _showIndex++;
        }

        IEnumerator countDown()
        {
            tvSeconds.text = _length + "\"";
            while (_length < _maxLength)
            {
                yield return new WaitForSeconds(1);
                _length++;
                tvSeconds.text = _length + "\"";
            }
            CancelInvoke("showR");
            for (int i = 0; i < r.Length; i++)
            {
                r[i].gameObject.SetActive(true);
            }
        } 
    }
}
