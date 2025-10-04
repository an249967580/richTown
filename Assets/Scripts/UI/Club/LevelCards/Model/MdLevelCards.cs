using System;
using System.Collections.Generic;

namespace RT
{
    public class MdLevelCards : MdList<ItemLevelCardData>
    {
        public long diamond;
        public int level;

        public bool IsDiamondEnough(int diamond)
        {
            return diamond <= this.diamond;
        }

        public void MinusDiamond(int diamond)
        {
            this.diamond -= diamond;
            Game.Instance.CurPlayer.Diamond = this.diamond;
        }

        public void FindList(Action<HttpResult<List<ItemLevelCardData>>> action)
        {
            ClubApi.FindLevelCards(action);
        }

        public void Buy(int id, Action<HttpResult<bool>> action)
        {
            ClubApi.BuyLevelCard(ClubMainView.Instance.ClubId, id, action);
        }
    }
}
