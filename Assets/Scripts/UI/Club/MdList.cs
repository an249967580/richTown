using System.Collections.Generic;

namespace RT
{
    public class MdList<T>
    {
        private bool _hasMore;
        private List<T> _list;

        public int pageSize = 20;

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> DataItems
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
                if(Validate.IsEmpty(_list))
                {
                    _list = new List<T>();
                }
                _hasMore = (Count == pageSize);
            }
        }

        public bool HasMore
        {
            get
            {
                return _hasMore;
            }
        }

        public T this[int index]
        {
            get
            {
                if(index>=0 && index < Count)
                {
                    return DataItems[index];
                }
                return default(T);
            }
        }

        /// <summary>
        /// 移除一项
        /// </summary>
        public void Remove(T item)
        {
            DataItems.Remove(item);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            DataItems = new List<T>();
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return DataItems == null || DataItems.Count == 0;
            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get
            {
               if(IsEmpty)
                {
                    return 0;
                }
                return DataItems.Count;
            }
        }

        /// <summary>
        /// 添加一项
        /// </summary>
        public void Add(T t)
        {
            if(Validate.IsEmpty(DataItems))
            {
                DataItems = new List<T>();
            }
            DataItems.Add(t);
        }

        /// <summary>
        /// 添加到最前面
        /// </summary>
        public void AddFirst(T t)
        {
            if (Validate.IsEmpty(DataItems))
            {
                DataItems = new List<T>();
            }
            DataItems.Insert(0, t);
        }

        /// <summary>
        /// 加载更多
        /// </summary>
        public void LoadMore(List<T> ts)
        {
            if (Validate.IsEmpty(DataItems))
            {
                _list = new List<T>();
            }
            if(Validate.IsNotEmpty(ts))
            {
                _list.AddRange(ts);
                _hasMore = (ts.Count == pageSize);
            }
            else
            {
                _hasMore = false;
            }
        }
    }
}
