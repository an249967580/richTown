using System;

namespace RT
{
    public class MdExchange
    {
        public long diamond;
        public long clubCoin;
        public int diamondRate;
        public long minClubCoin;

        public void exchagneCoin(int diamond, Action<HttpResult<ExchangeResult>> action)
        {
            ClubApi.ExchangeCoin(ClubMainView.Instance.ClubId, diamond, action);
        }

        public void exchagneDiamond(int coin, Action<HttpResult<ExchangeResult>> action)
        {
            ClubApi.ExchangeDiamond(ClubMainView.Instance.ClubId, coin, action);
        }
    }
}
