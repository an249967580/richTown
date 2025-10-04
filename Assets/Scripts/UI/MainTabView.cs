using RT;
using UnityEngine;
using UnityEngine.UI;

public class MainTabView : MonoBehaviour {

    public ShopView ShopViewPanel;
    public PersonView PersonViewPanel;
    public ClubHomeView ClubHomePanel;
    public GameObject GoPublic, GoRecord;

    public Toggle TgShop, TgClub, TgPublic, TgRecord, TgPersion;



    void Start () {
        Game.Instance.SetTips();
        Game.Instance.MainTabView = this;
        Screen.orientation = ScreenOrientation.Portrait;

        object obj = Transfer.Instance[TransferKey.MainSwitch];
        if(obj != null)
        {
            MainTabSwitch mts = (MainTabSwitch)obj;
            switch(mts)
            {
                case MainTabSwitch.Shop:
                    TgShop.isOn = true;
                    break;
                case MainTabSwitch.Club:
                    TgClub.isOn = true;
                    break;
                case MainTabSwitch.Public:
                    TgPublic.isOn = true;
                    break;
                case MainTabSwitch.Record:
                    TgRecord.isOn = true;
                    break;
                case MainTabSwitch.Person:
                    TgPersion.isOn = true;
                    break;
                default:
                    TgPublic.isOn = true;
                    break;
            }
            Transfer.Instance.Remove(TransferKey.MainSwitch);
        }
        else
        {
            TgPublic.isOn = true;
        }
    }
	
	void Update () {
		
	}
}
