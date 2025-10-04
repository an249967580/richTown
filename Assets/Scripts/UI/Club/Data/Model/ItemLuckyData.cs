using Newtonsoft.Json;

namespace RT
{
    public class ItemLuckyData
    {
        public long id;
        public long uid;
        public string avatar;
        public string nickname;
        public long startTime;
        public string game;
        public string cardList;

        [JsonIgnore]
        public int[] CardList
        {
            get
            {
                return JsonConvert.DeserializeObject<int[]>(cardList);
            }
        }

        [JsonIgnore]
        public string Game
        {
            get
            {
                if (Validate.IsNotEmpty(game))
                {
                    if (game == GameType.dz)
                    {
                        return LocalizationManager.Instance.GetText("7000");
                    }
                    return LocalizationManager.Instance.GetText("8000");
                }
                return string.Empty;
            }
        }
    }
}
