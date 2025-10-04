using Facebook.Unity;
using Newtonsoft.Json;
using RT;
using SimpleJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    public InputField PhoneField;
    public InputField passField;
    public InputField CodeField;
    public Button SmsBtn;
    public Button LoginBtn;
    public Button WechatBtn;
    public Button FacebookBtn;
    public Button btnCountry;

    public TipView Tips;
    public SureView sureView;
    public CountryView countryView;
    public Text tvCountry, tvCode;

    float time = 99;
    string loginPage = "";
    string tip = "";

    private List<ItemCountryData> countries;
    private ItemCountryData _curCountry;


    private void Awake()
    {
        initFB();
    }

    void Start()
    {

        Screen.orientation = ScreenOrientation.Portrait;

        SmsBtn.onClick.AddListener(() =>
        {
            SmsBtn.interactable = false;
            SmsBtnClick();
        });

        RT.UIEventListener.Get(LoginBtn.gameObject).onClick = LoginBtnClick;
        Game.Instance.SetTips();
        sureView.Hide();
        if (!string.IsNullOrEmpty(Game.Instance.RtToken))
        {
            AutoLogin();
        }

        object isKickout = Transfer.Instance[TransferKey.Kickout];
        if (isKickout != null)
        {
            if ((bool)isKickout)
            {
                sureView.Show();
            }
        }
        initCountry();
    }

    #region fb初始化
    private void initFB()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
        FacebookBtn.onClick.AddListener(delegate
        {
            FacebookLoginClick();
        });
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("初始化失败");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    #endregion

    #region 发送验证码

    IEnumerator SmsTimer(float time)
    {
        Text txt = SmsBtn.GetComponentInChildren<Text>();
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            txt.text = string.Format(LocalizationManager.Instance.GetText("1110"), time);
            time--;
        }
        txt.text = LocalizationManager.Instance.GetText("1104");
        SmsBtn.interactable = true;
    }

    void SmsBtnClick()
    {

        if (Validate.IsEmpty(PhoneField.text.ToString()))
        {
            SmsBtn.interactable = true;
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1105"));
            return;
        }

        if (_curCountry == null)
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5112"));
            SmsBtn.interactable = true;
            return;
        }

        UserApi.SendSms(PhoneField.text, _curCountry.cid, (error) =>
        {
            if (error == null)
            {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1109"));
                time = 99;
                StartCoroutine(SmsTimer(time));
            }
            else
            {
                Game.Instance.ShowTips(error);
                SmsBtn.interactable = true;
            }
        });
    }

    #endregion

    #region 第三方登录

    public void FacebookLoginClick()
    {
        var perms = new List<string>() { "public_profile"};
        FB.LogInWithReadPermissions(perms, authCallBack);
    }

    private void authCallBack(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);

            FB.API("me?id,name", HttpMethod.GET, (rsp) =>
            {
                Debug.Log(rsp.RawResult);
                string name = "";
                rsp.ResultDictionary.TryGetValue("name", out name);
                if(name != null && name.Length > 20)
                {
                    name = name.Substring(0, 20);
                }
                thirdLogin("facebook", aToken.UserId, name);
            });
        }
        else
        {
            Debug.Log("user cancelled login");
        }
    }

    // 第三方登陸
    void thirdLogin(string platform, string platformId, string platNick)
    {
        UserApi.Login(platform, platformId, platNick, (rsp) =>
        {
            if (rsp.IsOk)
            {
                Game.Instance.CurPlayer = rsp.data;

                PlayerPrefs.SetString("RtToken", Game.Instance.CurPlayer.Token);
                PlayerPrefs.SetInt("PId", Game.Instance.CurPlayer.Pid);
                PlayerPrefs.SetInt("Uid", Game.Instance.CurPlayer.Uid);
                PomeloLogin();
            }
            else
            {
                Game.Instance.ShowTips(rsp.errorMsg);
            }
        });
    }

    #endregion

    #region 账号登陆

    void LoginBtnClick(GameObject go)
    {
        if (PhoneField.text.Length == 0)
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1105"));
            return;
        }

        if (CodeField.text.Length == 0)
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1106"));
            return;
        }

        if (_curCountry == null)
        {
            Game.Instance.ShowTips(LocalizationManager.Instance.GetText("5112"));
            return;
        }

        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("game_code", PhoneField.text);
        param.Add("username", CodeField.text);
        param.Add("password", passField.text);
        // param.Add("pin", "123");
        param.Add("cid", _curCountry.cid.ToString());

        UserApi.Login(param, (user, error) =>
        {
            if (user != null)
            {
                Game.Instance.CurPlayer = user;

                PlayerPrefs.SetString("RtToken", Game.Instance.CurPlayer.Token);
                PlayerPrefs.SetInt("PId", Game.Instance.CurPlayer.Pid);
                PlayerPrefs.SetInt("Uid", Game.Instance.CurPlayer.Uid);
                PomeloLogin();
            }
            else
            {
                Debug.Log("------Php:Login-Error--");
                Game.Instance.ShowTips(error);
            }
        });
    }

    void AutoLogin()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("token", Game.Instance.RtToken);

        UserApi.AutoLogin(param, (user, error) =>
        {
            if (user != null)
            {
                Game.Instance.CurPlayer = user;
                PlayerPrefs.SetString("RtToken", Game.Instance.CurPlayer.Token);
                PomeloLogin();
            }
            else
            {
                // Game.Instance.ShowTips(error);
                Debug.Log("------Php:Login-Error--");
            }
        });
    }

    void PomeloLogin()
    {
        Game.Instance.PomeloNode.Login(false, (result) => {
            if (int.Parse(result["code"].ToString()) == 200)
            {
                if (result.ContainsKey("roomInfo"))
                {
                    //断线重连
                    JsonObject roomInfo = result["roomInfo"] as JsonObject;
                    roomInfo.Add("clubChips", 0);
                    roomInfo.Add("isReConnected", true);
                    Transfer.Instance[TransferKey.RoomInfo] = roomInfo;
                    if (roomInfo["game"].ToString() == "dzPoker")
                    {
                        loginPage = "TexasPokerScene";
                    }
                    else if (roomInfo["game"].ToString() == "cowWater")
                    {
                        loginPage = "ThreeBullScene";
                    }
                    else
                    {
                    }
                }
                else
                {
                    loginPage = "MainScene";
                }
            }
            else
            {
                tip = LocalizationManager.Instance.GetText("1016");
                Debug.Log("------Pomelo:Login-Error--");
            }
        });
    }

    #endregion

    #region 初始化国家
    // 初始化国家
    void initCountry()
    {
        if (PlayerPrefs.HasKey("country"))
        {
            ItemCountryData data = JsonConvert.DeserializeObject<ItemCountryData>(PlayerPrefs.GetString("country"));
            if (data != null)
            {
                tvCountry.text = data.title;
                tvCode.text = data.code;
                _curCountry = data;
            }
        }

        if (PlayerPrefs.HasKey("countries"))
        {
            countries = JsonConvert.DeserializeObject<List<ItemCountryData>>(PlayerPrefs.GetString("countries"));
            if (_curCountry == null)
            {
                _curCountry = countries[0];
                tvCountry.text = _curCountry.title;
                tvCode.text = _curCountry.code;
            }
        }
        else
        {
            // 获取国家
            SystemApi.FindCountry((rsp) =>
            {
                if (rsp.IsOk && Validate.IsNotEmpty(rsp.data))
                {
                    // 保存文件
                    countries = rsp.data;
                    PlayerPrefs.SetString("countries", JsonConvert.SerializeObject(rsp.data));
                    if (_curCountry == null)
                    {
                        _curCountry = countries[0];
                        tvCountry.text = _curCountry.title;
                        tvCode.text = _curCountry.code;
                    }
                }
            });
        }

        btnCountry.onClick.AddListener(() =>
        {
            countryView.Show();
            countryView.InitView(countries);
        });

        countryView.OnCountryClickEvent = onCountryClickEvent;
    }

    void onCountryClickEvent(ItemCountryData data)
    {
        tvCountry.text = data.title;
        tvCode.text = data.code;
        PlayerPrefs.SetString("country", JsonConvert.SerializeObject(data));
        _curCountry = data;
    }

    #endregion

    void Update()
    {
        if (!string.IsNullOrEmpty(loginPage))
        {
            PlayerPrefs.SetString("LastScene", "MainScene");
            if (Game.Instance.CurPlayer.loginNum > 0)
            {
                Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Public;
            }
            else
            {
                Transfer.Instance[TransferKey.MainSwitch] = MainTabSwitch.Person;
            }
            SceneManager.LoadScene(loginPage);
            loginPage = "";
        }
        if (!string.IsNullOrEmpty(tip))
        {
            Game.Instance.ShowTips(tip);
            tip = "";
        }
    }

}
