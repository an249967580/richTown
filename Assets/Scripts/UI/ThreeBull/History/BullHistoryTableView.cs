using Assets.Scripts.TableView;
using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BullHistoryTableView : MonoBehaviour, ITableViewDataSource, ITableViewDelegate
{
    public TableView tableView;
    public Text TitleTxt;
    public Text EmptyTxt;
    public Button BackBtn;
    public int RoomId;

    public List<BullHistorySuitData> DataList;

    BullHistoryView ParentView;

    private void Awake()
    {
        ParentView = GetComponentInParent<BullHistoryView>();
        DataList = new List<BullHistorySuitData>();
        tableView.Delegate = this;
        tableView.DataSource = this;

        GameObject prefab = Resources.Load<GameObject>("Prefabs/Bull/BullSuitCell");
        tableView.RegisterPrefabForCellReuseIdentifier(prefab, "BullSuitTableViewCRI");
    }

    void Start()
    {
    }
    public void InitView(List<BullHistorySuitData> list)
    {
        DataList = list;
        tableView.ReloadData();
    }
    public void Show()
    {
        if (DataList.Count == 0)
        {
            EmptyTxt.gameObject.SetActive(true);
        }
        else
        {
            EmptyTxt.gameObject.SetActive(false);
        }
    }

    public int NumberOfRowsInTableView(TableView tableView)
    {
        return DataList.Count;
    }

    public float SizeForRowInTableView(TableView tableView, int row)
    {
        return 185.0f;
    }

    public TableViewCell CellForRowInTableView(TableView tableView, int row)
    {
        BullSuitTableCell cell = tableView.ReusableCellForRow("BullSuitTableViewCRI", row) as BullSuitTableCell;
        cell.name = "Cell " + row;
        cell.Data = DataList[row];
        return cell;
    }

    public void TableViewDidHighlightCellForRow(TableView tableView, int row)
    {
        print("TableViewDidHighlightCellForRow : " + row);
    }

    public void TableViewDidSelectCellForRow(TableView tableView, int row)
    {
        ParentView.GoDetailViewClick(row);
    }

    public void TableViewDidScrollToEnd(TableView tableView)
    {
        Dictionary<string, string> parma = new Dictionary<string, string>();
        parma.Add("roomId", RoomId.ToString());
        if (DataList.Count > 0)
        {
            parma.Add("handId", DataList[DataList.Count - 1].HandId.ToString());
        }
        BullApi.GetGameRoomHandsHistory(parma, (resp, error) => {
            if (error == null)
            {
                if (resp.List.Count > 0)
                {
                    DataList.AddRange(resp.List);
                    for (int i = 0; i < DataList.Count; i++) {
                        DataList[i].Index = resp.Total - i;
                    }
                    tableView.ReloadData();
                }
            }
            else
            {
                Game.Instance.ShowTips(error);
            }
        });

    }
    public void TableViewDidScrollToStart(TableView tableView)
    {

    }

}