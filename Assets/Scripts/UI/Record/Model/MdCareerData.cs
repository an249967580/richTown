using System;
namespace RT
{
    public class MdCareerData : MdList<ItemCareerBoardData>
    {
        public string game;

        public void FindList(Action<HttpResult<CareerData>> action)
        {
            CareerApi.FindCareerData(game, lasId, pageSize, action);
        }

        long lasId
        {
            get
            {
                if(IsEmpty)
                {
                    return 0;
                }
                return DataItems[Count - 1].id;
            }
        }

    }
}
