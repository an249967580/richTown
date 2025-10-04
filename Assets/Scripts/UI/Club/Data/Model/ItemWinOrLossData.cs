using Newtonsoft.Json;

namespace RT
{
    public class ItemWinOrLossData
    {
        public long id;
        public long uid;
        public string nickname;
        public string avatar;
        public string game;
        public long startTime;
        public long profitLoss;

        [JsonIgnore]
        public string Game
        {
            get
            {
                if(Validate.IsNotEmpty(game))
                {
                    if(game == GameType.dz)
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
