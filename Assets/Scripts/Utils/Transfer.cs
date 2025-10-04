using System.Collections;

namespace RT
{
    /// <summary>
    /// 用于场景之间的值传递
    /// </summary>
    public class Transfer : Singleton<Transfer>
    {
        private Hashtable _hash;

        public Transfer()
        {
            _hash = new Hashtable();
        }

        public object this[string index]
        {
            get
            {
                if (_hash.ContainsKey(index))
                {
                    return _hash[index];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _hash[index] = value;
            }
        }

        public void Remove(string index)
        {
            _hash.Remove(index);
        }
    }

    public class TransferKey
    {
        public const string ClubId = "clubId";     // 俱乐部id
        public const string ClubInfo = "clubInfo"; // 俱乐部信息
        public const string MainSwitch = "mainSwitch";  // 主页项
        public const string RoomInfo = "roomInfo";      // 房间信息
        public const string Game = "game";              // 游戏 德州/牛牛
        public const string ShopTab = "shopTab";        // 商店切换
        public const string Kickout = "kickout";        // 设备在其他地方登陆
        public const string RoomSwitch = "roomSwitch";
    }

    // 首页标签切换
    public enum MainTabSwitch
    {
        Shop,
        Club,
        Public,
        Record,
        Person
    }

    public enum ShopTab
    {
        Gold,
        Diamond,
    }

    public class Platform
    {
        public const string facebook = "facebook";
        public const string wechat = "wechat";
    }

    public enum ClubRoomSwitch
    {
        Dz,  // 德州
        Bull // 牛牛
    }

    // 游戏类型
    public class GameType
    {
        public const string dz = "dzPoker";
        public const string bull = "cowWater";

        public static bool IsDz(string game)
        {
            return game != null && game == dz;
        }

        public static bool IsBull(string game)
        {
            return game != null && game == bull;
        }
    }
}
