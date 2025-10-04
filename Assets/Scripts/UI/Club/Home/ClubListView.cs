using Assets.Scripts.TableView;
using SimpleJson;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RT
{
    /// <summary>
    /// 俱乐部列表
    /// </summary>
    public class ClubListView : MonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public TableView tableView;
        public GameObject itemClubPrefab;
        public Button btnAdd;
        bool _isInit;

        public MdHome _md;

        private void Awake()
        {
            btnAdd.onClick.AddListener(()=>
            {
                ClubHomeView.Instance.CreateClubOpView();
            });
        }

        private void Start()
        {
            if (!_isInit)
            {
                initTable();
            }
        }

        public int NumberOfRowsInTableView(TableView tableView)
        {
            if(_md == null)
            {
                return 0;
            }
            return _md.Count;
        }

        public float SizeForRowInTableView(TableView tableView, int row)
        {
            return 160;
        }

        public TableViewCell CellForRowInTableView(TableView tableView, int row)
        {
            ItemClubCell cell = tableView.ReusableCellForRow("ItemClubCRI", row) as ItemClubCell;
            cell.name = "cell_" + row;
            cell.data = _md[row];
            cell.imgAvatar.gameObject.SetActive(false);
            if (Validate.IsNotEmpty(_md[row].avatar))
            {
                if (gameObject.activeSelf)
                {
                    StartCoroutine(LoadImageUtil.LoadImage(_md[row].avatar, (sprite) =>
                    {
                        cell.imgAvatar.gameObject.SetActive(true);
                        cell.imgAvatar.sprite = sprite;
                    }));
                }
            }
            else
            {
                cell.imgAvatar.gameObject.SetActive(false);
            }
            return cell;
        }

        public void TableViewDidHighlightCellForRow(TableView tableView, int row)
        {
            
        }

        public void TableViewDidSelectCellForRow(TableView tableView, int row)
        {
            ItemClubData data = _md[row];
            // 获取俱乐部信息
            MainView.Instance.CreateLoadMask();
            ClubApi.GetClubDetail(data.clubId, (result) =>
            {
                if (result.IsOk)
                {
                    JsonObject jObj = new JsonObject();
                    jObj.Add("clubId", result.data.clubId);
                    jObj.Add("clubName", result.data.name);
                    PlayerPrefs.SetString("lastClubInfo", jObj.ToString());
                    Transfer.Instance[TransferKey.ClubId] = data.clubId;
                    Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Club;
                    Transfer.Instance[TransferKey.ClubInfo] = result.data;
                    StartCoroutine(loadScene());
                }
                else
                {
                    MainView.Instance.HideMask();
                    Game.Instance.ShowTips(result.errorMsg);
                }
            }, false);
        }

        public void TableViewDidScrollToStart(TableView tableView)
        {
            
        }

        public void TableViewDidScrollToEnd(TableView tableView)
        {
            if(_md.HasMore)
            {
                _md.FindList((rsp)=>
                {
                    if(rsp.IsOk)
                    {
                        _md.LoadMore(rsp.data);
                        tableView.ReloadData();
                    }
                }, true);
            }
        }

        public void ReloadData()
        {
            if(!_isInit)
            {
                initTable();
            }
            tableView.ReloadData();
        }

        void initTable()
        {
            if (tableView.Delegate == null)
            {
                tableView.Delegate = this;
            }
            if (tableView.DataSource == null)
            {
                tableView.DataSource = this;
            }
            tableView.RegisterPrefabForCellReuseIdentifier(itemClubPrefab, "ItemClubCRI");
            _isInit = true;
        }

        IEnumerator loadScene()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync("ClubScene");
            op.allowSceneActivation = false;
            while (op.progress < 0.9f)
            {
                yield return 0;
            }
            op.allowSceneActivation = true;
            MainView.Instance.HideMask();
        }
    }
}
