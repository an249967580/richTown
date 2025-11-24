using System;
using System.Collections;
using System.Collections.Generic;

namespace RT
{
    public delegate void OnNotify(NotifyMsg msg);

    public class NotificationCenter
    {
        private static NotificationCenter _instance;

        public static NotificationCenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotificationCenter();
                }
                return _instance;
            }
        }

        NotificationCenter()
        {
            _dictEvent = new Dictionary<NotificationType, List<OnNotify>>();
        }

        private Dictionary<NotificationType, List<OnNotify>> _dictEvent;

        public void AddNotifyListener(NotificationType type, OnNotify onNotify)
        {
            if (!_dictEvent.ContainsKey(type))
            {
                List<OnNotify> list = new List<OnNotify>();
                list.Add(onNotify);
                _dictEvent.Add(type, list);
            }
            else
            {
                _dictEvent[type].Add(onNotify);
            }
        }

        public void RemoveNotifyListener(NotificationType type)
        {
            if (!_dictEvent.ContainsKey(type))
            {
                return;
            }
            _dictEvent[type].Clear();
            _dictEvent[type] = null;
            _dictEvent.Remove(type);
        }

        public void RemoveNotifyListener(NotificationType type, OnNotify onNotify)
        {
            if (!_dictEvent.ContainsKey(type))
            {
                return;
            }
            _dictEvent[type].Remove(onNotify);
        }

        public void DispatchNotify(NotificationType type, NotifyMsg msg)
        {
            if (!_dictEvent.ContainsKey(type))
            {
                return;
            }
            List<OnNotify> list = _dictEvent[type];
            if (list != null && list.Count > 0)
            {
                foreach (OnNotify onNotify in list)
                {
                    onNotify(msg);
                }
            }
        }

        public bool Exists(NotificationType type)
        {
            return _dictEvent.ContainsKey(type);
        }
    }

    public class NotifyMsg
    {
        private Hashtable _hash;

        public NotifyMsg()
        {
            _hash = new Hashtable();
        }

        public NotifyMsg value(string index, object param)
        {
            this[index] = param;
            return this;
        }

        public object this[string index]
        {
            get
            {
                if (_hash.Contains(index))
                {
                    return _hash[index];
                }
                else
                {
                    throw new Exception("");
                }
            }
            set { _hash[index] = value; }
        }
    }

    public enum NotificationType : uint
    {
        EditAvatar,        // 上传人物部头像
        EditClubAvatar,    // 上传俱乐部头像
        OnMsg,             // 消息通知
        ChangeClubCoin,    // 更新俱乐部币
        ApplyJoin,         // 申请消息
        ApplyAgree,        // 同意加入俱乐部
        Kickout,           // 踢出俱乐部
        Lock,              // 账号锁定
        Currency,          // 赠送钻石
        Paypal,            // paypal支付
        StopServer,         // 停服
        public_room_dissolve, //刷新桌子
        local_user_emoji //发送表情
    }
}
