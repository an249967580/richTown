namespace RT
{
    /// <summary>
    /// 申请列表数据
    /// </summary>
    public class ItemApplyData : ItemData
    {
        public long applyId;
        public long uid;
        public string avatar;
        public string nickname;

        public override long Id()
        {
            return applyId;
        }
    }
}