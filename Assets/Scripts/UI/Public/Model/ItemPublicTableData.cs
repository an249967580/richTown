namespace RT
{
    public class ItemPublicTableData : ItemData
    {
        public long roomId;
        public string roomName;
        public int playerCount;
        public int inGamePlayer;
        public int blinds;
        public int minBuyIn;
        public string game;

        public override long Id()
        {
            return roomId;
        }
    }
}
