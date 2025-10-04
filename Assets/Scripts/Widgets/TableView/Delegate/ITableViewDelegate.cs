namespace Assets.Scripts.TableView
{
    public interface ITableViewDelegate
    {
        void TableViewDidHighlightCellForRow(TableView tableView, int row);
        void TableViewDidSelectCellForRow(TableView tableView, int row);
        void TableViewDidScrollToStart(TableView tableView);
        void TableViewDidScrollToEnd(TableView tableView);
    }
}