using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class NoticeView : HideMonoBehaviour
    {
        public Text tvMsg;
        RectTransform _rt;

        private NoticeMsg _msg;
        private float speed = 50; // 飘字速度

        public NoticeMsg Msg
        {
            set
            {
                _msg = value;
                StartCoroutine(Scrolling());
            }
            get
            {
                return _msg;
            }
        }

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        public IEnumerator Scrolling()
        {
            float width = _rt.sizeDelta.x;
            float start = width / 2;

            tvMsg.text = _msg.msg;
            tvMsg.color = _msg.Color;
            float textWidth = tvMsg.preferredWidth;
            float distance = textWidth + start;
            float duration = distance / speed;       // 飘字时间
            while (_msg.loop-- > 0)
            {
                tvMsg.rectTransform.localPosition = new Vector2(start, 0);
                tvMsg.rectTransform.DOLocalMoveX(-distance, duration).SetEase(Ease.Linear);
                yield return new WaitForSeconds(_msg.duration + duration);
            }
            _msg = null;
            HideAndDestory();
        }
    }
}
