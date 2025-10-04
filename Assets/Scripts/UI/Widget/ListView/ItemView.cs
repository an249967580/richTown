using UnityEngine;

namespace RT
{
    public delegate void ItemClickEvent(ItemView vi);

    public abstract class ItemView : MonoBehaviour
    {

        private ItemData _data;
        public ItemClickEvent OnItemClickEvent;

        public ItemData Data
        {
            get
            {
                return _data;
            }
        }

        public virtual void Awake()
        {
            RegisterEvent();
        }

        public void Init(ItemData data)
        {
            _data = data;
            Render();
        }

        // 注册事件
        public virtual void RegisterEvent()
        {

        }

        // 渲染页面
        public abstract void Render();

    }
}
