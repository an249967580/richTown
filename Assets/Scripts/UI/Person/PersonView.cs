using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersonView : MonoBehaviour {
    public Button MailBtn;
    public Button SettingBtn;
    public Button AvatarBtn;
    public Button InfoEditBtn;
    public Button AddDiamondBtn;
    public Button AddCoinBtn;
    public Image imgMailNotice;

    public CircleImage AvatarImg;
    public Image AvatarRingImg;
    public Text NickTxt;
    public Text UidTxt;

    public Image ExpirImg;
    public Text ExpirTxt;
    public Text DiamondTxt;
    public Text GoldTxt;

    public Button NoticeBtn;
    public Button AboutBtn;
    public Button HelpBtn;


    PersonInfoView PersonInfoDialog;
    SettingView SettingDialog;
    DialogPanel DialogTpl;


    private void Awake()
    {
        PersonInfoDialog = Resources.Load<PersonInfoView>("Prefabs/Person/PersonInfoView");
        SettingDialog = Resources.Load<SettingView>("Prefabs/Setting/SettingDialogPanel");
        DialogTpl = Resources.Load<DialogPanel>("Prefabs/Widgets/DialogPanel");
        RT.UIEventListener.Get(AvatarBtn.gameObject).onClick = ShowInfoDialog;
        RT.UIEventListener.Get(InfoEditBtn.gameObject).onClick = ShowInfoDialog;
        RT.UIEventListener.Get(SettingBtn.gameObject).onClick = ShowSettingDialog;
        RT.UIEventListener.Get(AddDiamondBtn.gameObject).onClick = GoShopCashView;
        RT.UIEventListener.Get(AddCoinBtn.gameObject).onClick = GoShopDiamondView;
        imgMailNotice.gameObject.SetActive(false);
        NoticeBtn.onClick.AddListener(() =>
        {
            MainView.Instance.CreateAnnouncementView();
        });
        AboutBtn.onClick.AddListener(() =>
        {
            MainView.Instance.CreateAboutView();
        });
        HelpBtn.onClick.AddListener(() =>
        {
            MainView.Instance.CreateHelpView();
        });
        MailBtn.onClick.AddListener(() =>
        {
            imgMailNotice.gameObject.SetActive(false);
            MainView.Instance.CreateEmailView().OnUpdateDiamondEvent = ()=>
            {
                if (Game.Instance.CurPlayer != null)
                {
                    DiamondTxt.text = Game.Instance.CurPlayer.Diamond.ToString();
                    GoldTxt.text = Game.Instance.CurPlayer.Gold.ToString();
                }
            };
        });
        NotificationCenter.Instance.AddNotifyListener(NotificationType.Currency, onCurrencyNotify);
    }

    void Start ()
    {
        InitPlayerInfo();
    }

    private void OnEnable()
    {
        if (Game.Instance.CurPlayer != null)
        {
            DiamondTxt.text = Game.Instance.CurPlayer.Diamond.ToString();
            GoldTxt.text = Game.Instance.CurPlayer.Gold.ToString();
        }

        EmailApi.UnReadEmail((rsp) =>
        {
            if(rsp.IsOk)
            {
                if(rsp.data > 0)
                {
                    imgMailNotice.gameObject.SetActive(true);
                }
            }
        });
    }

    void Update () {
		
	}
    
    public void InitPlayerInfo() {

        if (Game.Instance.CurPlayer != null)
        {
            UidTxt.text = "ID:" + Game.Instance.CurPlayer.Uid;
            NickTxt.text = Game.Instance.CurPlayer.NickName == "" ? "--" : Game.Instance.CurPlayer.NickName;
            DiamondTxt.text = Game.Instance.CurPlayer.Diamond.ToString();
            GoldTxt.text = Game.Instance.CurPlayer.Gold.ToString();
            if (Game.Instance.CurPlayer.Vip > 0)
            {
                ExpirImg.gameObject.SetActive(true);
                ExpirTxt.text = Game.Instance.CurPlayer.VipExpirTime.ToString();
                AvatarRingImg.sprite = Resources.Load<Sprite>("Textures/Common/Vip/vip_headring_" + Game.Instance.CurPlayer.Vip + "_l");
            }
            else
            {
                ExpirImg.gameObject.SetActive(false);
                AvatarRingImg.sprite = Resources.Load<Sprite>("Textures/Common/avatar_border_large");
            }
            string avatar = Game.Instance.CurPlayer.Avatar;
            if (Validate.IsNotEmpty(avatar))
            {
                if (gameObject.activeSelf)
                {
                    StartCoroutine(LoadImageUtil.LoadImage(avatar, (sprite) =>
                    {
                        AvatarImg.sprite = sprite;
                    }));
                }
            }
            else
            {
                AvatarImg.sprite = Resources.Load<Sprite>("Textures/Common/def_avatar_large");
            }
        }
    }

    void GoShopDiamondView(GameObject go) {
        Game.Instance.MainTabView.TgShop.isOn = true;
        Game.Instance.MainTabView.ShopViewPanel.TgAPanel.isOn = true;
    }
    void GoShopCashView(GameObject go)
    {
        Game.Instance.MainTabView.TgShop.isOn = true;
        Game.Instance.MainTabView.ShopViewPanel.TgBPanel.isOn = true;
    }

    void ShowInfoDialog(GameObject go) {
        DialogPanel dialog = Instantiate(DialogTpl);
        PersonInfoView content = Instantiate(PersonInfoDialog);
        content.OnEditAvatarEvent = (avatar) =>
        {
            StartCoroutine(LoadImageUtil.LoadImage(avatar, (sprite) =>
            {
                AvatarImg.sprite = sprite;
            }));
        };
        if (dialog) {
            RectTransform t = (RectTransform)dialog.gameObject.transform;
            GameObject ugui = GameObject.Find("Bg");
            RectTransform uguit = (RectTransform)ugui.transform;
            t.SetParent(ugui.transform);
            t.sizeDelta = new Vector2(750,1334);
            t.localScale = Vector3.one;
            t.anchoredPosition3D = Vector3.zero;

            dialog.SetTitle(LocalizationManager.Instance.GetText("2300")).SetButtonText(LocalizationManager.Instance.GetText("1000")).SetContent(content.gameObject).SetClickEvent(delegate (){
                string nick = content.NickField.text.Trim();
                if (!string.IsNullOrEmpty(nick)) {
                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("nickname", nick);

                    UserApi.EditUserInfo(param, (error) => {
                        if (error == null)
                        {
                            Game.Instance.CurPlayer.NickName = nick;
                            NickTxt.text = nick;
                            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1112"));
                        }
                        else
                        {
                            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1113"));
                        }
                    });
                    
                }
            });
            dialog.Show();
        }
    }

    void ShowSettingDialog(GameObject go)
    {
        DialogPanel dialog = Instantiate(DialogTpl);
        SettingView content = Instantiate(SettingDialog);
        if (dialog)
        {
            RectTransform t = (RectTransform)dialog.gameObject.transform;
            GameObject ugui = GameObject.Find("_UGUI");
            RectTransform uguit = (RectTransform)ugui.gameObject.transform;
            t.SetParent(ugui.transform);
            t.sizeDelta = uguit.sizeDelta;
            t.localScale = Vector3.one;
            t.anchoredPosition3D = Vector3.zero;

            dialog.SetTitle(LocalizationManager.Instance.GetText("2100")).SetButtonText(LocalizationManager.Instance.GetText("2106")).SetContent(content.gameObject).SetClickEvent(delegate () {
                UserApi.Logout((error)=> {
                    Game.Instance.PomeloNode.Logout();
                    Game.Instance.RemoveToken();
                    SceneManager.LoadScene("LoginScene");
                });
            });
            dialog.Show();
        }
    }

    #region 应用内消息监听
    // 收到后台赠送的金币或钻石
    void onCurrencyNotify(NotifyMsg msg)
    {
        if (Game.Instance.CurPlayer != null)
        {
            DiamondTxt.text = Game.Instance.CurPlayer.Diamond.ToString();
            GoldTxt.text = Game.Instance.CurPlayer.Gold.ToString();
        }
    }

    // 删除监听
    private void OnDestroy()
    {
        NotificationCenter.Instance.RemoveNotifyListener(NotificationType.Currency, onCurrencyNotify);
    }
    #endregion
}
