using SimpleJson;
using System;
using System.Collections.Generic;

namespace RT
{
    public class MdPublicTable : MdList<ItemTableData>
    {
        public string game = GameType.dz;
        public long clubId = -1;

        public void FindList(Action<HttpResult<List<ItemTableData>>> action)
        {
            ClubApi.FindTables(clubId, game, action, true);
        }


        // public void EnterRoom(long roomId, Action<JsonObject> action)
        // {
        //     JsonObject param = new JsonObject();
        //     param.Add("roomId", roomId);
        //     param.Add("uid", Game.Instance.CurPlayer.Uid);
        //     param.Add("pw", string.Empty);
        //     if (GameType.IsDz(game))
        //     {
        //         TexasApi.EnterRoom(param, action);
        //     }
        //     else
        //     {
        //         BullApi.EnterRoom(param, action);
        //     }
        // }

        public void EnterRoom(long roomId, string pw, Action<JsonObject> action)
        {
            JsonObject param = new JsonObject();
            param.Add("roomId", roomId);
            param.Add("uid", Game.Instance.CurPlayer.Uid);
            param.Add("pw", pw);
            if (GameType.IsDz(game))
            {
                TexasApi.EnterRoom(param, action);
            }
            else
            {
                BullApi.EnterRoom(param, action);
            }
        }
    }
}
