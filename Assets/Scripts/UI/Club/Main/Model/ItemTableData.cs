using Newtonsoft.Json;

namespace RT
{
    public class ItemTableData : ItemData
    {
        public long roomId;
        public string title;
        public string game;
        public long roomTime;
        public int playerNum;
        public int roomPlayers;
        public int pw;
        public int blindBet;
        public int minBet;

        [JsonIgnore]
        public bool isCreator;

        [JsonIgnore]
        public bool isPin
        {
            get
            {
                return pw == 1;
            }
        }

        [JsonIgnore]
        public bool isTexas
        {
            get
            {
                return game.Equals(GameType.dz);
            }
        }
    }
}
