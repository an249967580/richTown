using UnityEngine;

namespace RT
{
    public class HideMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 显示
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void HideAndDestory()
        {
            Destroy(gameObject);
        }
    }
}
