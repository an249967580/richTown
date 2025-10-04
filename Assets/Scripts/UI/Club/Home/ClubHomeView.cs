using System;
using UnityEngine;

namespace RT
{
    public class ClubHomeView : MonoBehaviour
    {
        public ClubListView goList;
        public ClubOpView goOp;

        public static ClubHomeView Instance;

        private MdHome _md;

        private void Awake()
        {
            Instance = this;
            _md = new MdHome();
            goList._md = _md;

            NotificationCenter.Instance.AddNotifyListener(NotificationType.ApplyAgree, onApplyAgreeNotify);
            NotificationCenter.Instance.AddNotifyListener(NotificationType.Kickout, onApplyAgreeNotify);
        }

        private void OnEnable()
        {
            _md.Clear();
            findList(true);
        }

        public void OnCreateClub(ItemClubData data)
        {
            if (!goList.gameObject.activeSelf)
            {
                goList.gameObject.SetActive(true);
            }
            _md.AddFirst(data);
            goList.ReloadData(); 
        }

        public void ShowList(bool isShow)
        {
            if(isShow)
            {
                goList.gameObject.SetActive(true);
                goOp.gameObject.SetActive(false);
            }
            else
            {
                goList.gameObject.SetActive(false);
                goOp.gameObject.SetActive(true);
            }
        }

        void findList(bool showMask)
        {
            // 获取我的俱乐部列表
            _md.FindList((result) =>
            {
                if (result.IsOk)
                {
                    if (result.data != null && result.data.Count > 0)
                    {
                        ShowList(true);
                        _md.DataItems = result.data;
                        goList.ReloadData();
                    }
                    else
                    {
                        ShowList(false);
                    }
                }
                else
                {
                    ShowList(false);
                    Game.Instance.ShowTips(result.errorMsg);
                }
            }, showMask);
        }

        #region UI 生成

        public ClubOpView CreateClubOpView()
        {
            ClubOpView view = load<ClubOpView>("Prefabs/Club/Home/ClubOpView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ClubOpView Create Failed ...");
        }

        public ClubCreateView CreateClubCreateView()
        {
            ClubCreateView view = load<ClubCreateView>("Prefabs/Club/Home/ClubCreateView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ClubCreateView Create Failed ...");
        }

        public ClubSearchView CreateClubSearchView()
        {
            ClubSearchView view = load<ClubSearchView>("Prefabs/Club/Home/ClubSearchView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ClubSearchView Create Failed ...");
        }

        public ClubApplyTipView CreateClubApplyTipView()
        {
            ClubApplyTipView view = load<ClubApplyTipView>("Prefabs/Club/Home/ClubApplyTipView");
            if (view)
            {
                setRectTransform(view.gameObject);
                return view;
            }
            throw new Exception("ClubApplyTipView Create Failed ...");
        }

        void setRectTransform(GameObject go)
        {
            RectTransform t = (RectTransform)go.transform;
            t.SetParent(gameObject.transform.parent);
            t.localScale = Vector3.one;
            t.anchoredPosition3D = Vector3.zero;
        }

        T load<T>(string url)
        {
            GameObject go = Resources.Load(url) as GameObject;
            if (go)
            {
                GameObject target = Instantiate(go);
                if (target)
                {
                    return target.GetComponent<T>();
                }
            }
            return default(T);
        }

        #endregion

        void onApplyAgreeNotify(NotifyMsg msg)
        {
            if(gameObject.activeSelf)
            {
                findList(false);
            }
        }

        private void OnDestroy()
        {
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.ApplyAgree, onApplyAgreeNotify);
            NotificationCenter.Instance.RemoveNotifyListener(NotificationType.Kickout, onApplyAgreeNotify);
        }
    }

}
