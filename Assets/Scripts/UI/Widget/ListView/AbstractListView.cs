using System.Collections.Generic;
using UnityEngine;

namespace RT
{
    public abstract class AbstractListView : MonoBehaviour
    {
        public ItemView itemPrefab;
        public abstract GameObject LayoutView();

        private List<ItemView> _viewItems;
        private List<ItemData> _itemDatas;

        public List<ItemView> ViewItems
        {
            get
            {

                return _viewItems;
            }
        }

        public List<ItemData> ItemDatas
        {
            get
            {
                return _itemDatas;
            }
        }

        public virtual void Awake()
        {
            _viewItems = new List<ItemView>();
            _itemDatas = new List<ItemData>();
        }


        /// <summary>
        /// 添加一项
        /// </summary>
        /// <param name="data"></param>
        public ItemView Add(ItemData data)
        {
            ItemView vi = Instantiate(itemPrefab) as ItemView;
            vi.gameObject.transform.SetParent(LayoutView().transform);
            vi.gameObject.transform.localScale = Vector3.one;
            (vi.gameObject.transform as RectTransform).anchoredPosition3D = Vector3.zero;
            vi.Init(data);
            _viewItems.Add(vi);
            _itemDatas.Add(data);
            return vi;
        }

        /// <summary>
        /// 添加多项
        /// </summary>
        /// <param name="datas"></param>
        public void Add(ICollection<ItemData> datas)
        {
            if (datas != null && datas.Count > 0)
            {
                foreach (ItemData data in datas)
                {
                    Add(data);
                }
            }
        }
     
        /// <summary>
        /// id 移除
        /// </summary>
        /// <param name="id"></param>
        public void Remove(long id)
        {
            ItemView itemView = _viewItems.Find(v => v.Data.Id() == id);
            if (itemView)
            {
                Destroy(itemView.gameObject);
            }
            _viewItems.Remove(itemView);
            _itemDatas.Remove(itemView.Data);
        }

        /// <summary>
        /// id 移除
        /// </summary>
        /// <param name="id"></param>
        public void Remove(string sid)
        {
            ItemView itemView = _viewItems.Find(v => v.Data.Sid() == sid);
            if (itemView)
            {
                Destroy(itemView.gameObject);
            }
            _viewItems.Remove(itemView);
            _itemDatas.Remove(itemView.Data);
        }

        /// <summary>
        /// 移除一项
        /// </summary>
        /// <param name="data"></param>
        public void Remove(ItemData data)
        {
            ItemView itemView = _viewItems.Find(v => v.Data == data);
            if(itemView)
            {
                Destroy(itemView.gameObject);
            }
            _viewItems.Remove(itemView);
            _itemDatas.Remove(data);
        }

        /// <summary>
        /// 移除一项
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            Destroy(_viewItems[index].gameObject);
            _viewItems.RemoveAt(index);
            _itemDatas.RemoveAt(index);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            foreach(ItemView vi in _viewItems)
            {
                Destroy(vi.gameObject);
            }
            _viewItems.Clear();
            _itemDatas.Clear();
        }

        /// <summary>
        /// 通过id获取view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ItemView GetViewById(long id)
        {
            for (int i = 0; i < _viewItems.Count; i++)
            {
                if (_viewItems[i].Data.Id() == id)
                {
                    return _viewItems[i];
                }
            }
            return null;
            //return _viewItems.Find(vi => vi.Data.Id() == id);
        }

        /// <summary>
        /// 通过id获取view
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ItemView GetViewById(string sid)
        {
            for (int i = 0; i < _viewItems.Count; i++)
            {
                if (_viewItems[i].Data.Sid() == sid)
                {
                    return _viewItems[i];
                }
            }
            return null;
            //return _viewItems.Find(vi => vi.Data.Sid() == sid);
        }

        /// <summary>
        /// 通过下标，获取view
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public ItemView GetView(int idx)
        {
            return _viewItems[idx];
        }

        /// <summary>
        /// 通过数据获取view
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ItemView GetView(ItemData data)
        {
            for(int i=0;i<_viewItems.Count;i++)
            {
                if(_viewItems[i].Data == data)
                {
                    return _viewItems[i];
                }
            }
            return null;
            //return _viewItems.Find(v => v.Data == data);
        }

    }
}
