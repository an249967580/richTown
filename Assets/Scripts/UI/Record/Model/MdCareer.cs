using System;

namespace RT
{
    public class MdCareer
    {

        public Career career;

        public void GetCareer(string game, Action<HttpResult<Career>> action)
        {
            CareerApi.GetCareer(game, action);
        }
    }
}
