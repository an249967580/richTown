using Assets.Scripts.TableView;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public class AnnouncementView : HideMonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public Button btnClose;
        public TableView tableView;
        public GameObject itemEmailPrefab;

        MdAnnouncement _md;

        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
		private static extern void _iOSOpenWebPage(string url,string title);
        #endif

        private void Awake()
        {
            btnClose.onClick.AddListener(HideAndDestory);
            _md = new MdAnnouncement();
        }

        private void Start()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;
            tableView.RegisterPrefabForCellReuseIdentifier(itemEmailPrefab, "ItemAnnouncementCRI");

            findList(true);
        }

        void findList(bool first)
        {
            _md.FindList((rsp) =>
            {
                if (first)
                {
                    _md.DataItems = rsp.data;
                }
                else
                {
                    _md.LoadMore(rsp.data);
                }
                tableView.ReloadData();
            }, true);
        }

        public TableViewCell CellForRowInTableView(TableView tableView, int row)
        {
            ItemAnnouncementCell cell = tableView.ReusableCellForRow("ItemAnnouncementCRI", row) as ItemAnnouncementCell;
            cell.data = _md[row];
            cell.name = "cell_" + row;
            return cell;
        }

        public int NumberOfRowsInTableView(TableView tableView)
        {
            return _md.Count;
        }

        public float SizeForRowInTableView(TableView tableView, int row)
        {
            return 120;
        }

        public void TableViewDidHighlightCellForRow(TableView tableView, int row)
        {

        }

        public void TableViewDidScrollToEnd(TableView tableView)
        {
            if (_md.HasMore)
            {
                findList(false);
            }
        }

        public void TableViewDidScrollToStart(TableView tableView)
        {
        }

        public void TableViewDidSelectCellForRow(TableView tableView, int row)
        {
            ItemAnnouncementData data = _md[row];
            onItemClickEvent(data);
        }

    void onItemClickEvent(ItemAnnouncementData data)
        {
            if(Validate.IsEmpty(data.url))
            {
                return;
            }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            Debug.Log("暂不支持");
#elif UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            jo.Call("StartWebView", data.Title, data.url);
#elif UNITY_IOS
			_iOSOpenWebPage(data.url, data.Title);
#endif
        }
    }
}
