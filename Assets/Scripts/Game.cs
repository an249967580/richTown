using RT;
using UnityEngine;

public class Game : MonoBehaviour {

    static TipView Tips;

    private static Game instance;

    public static Game Instance
    {
        get
        {
            GameObject main = GameObject.Find("Game");
            if (!main)
            {
                main = new GameObject("Game");
                DontDestroyOnLoad(main);
            }

            if (instance == null)
            {
                main.AddComponent<HttpClient>();
                main.AddComponent<PomeloMgr>();
                main.AddComponent<AudioSource>();
                main.AddComponent<AudioSource>();
                main.AddComponent<AudioManger>();
                main.AddComponent<VoiceRecorder>();
                main.AddComponent<Location>();
                main.AddComponent<AwsS3Service>();
                main.AddComponent<SystemNotify>();
                instance = main.AddComponent<Game>();

                Tips = Instantiate(Resources.Load<TipView>("Prefabs/Widgets/Tips"));
            }
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    private string rtToken;
    public string RtToken
    {
        get
        {
            rtToken = PlayerPrefs.HasKey("RtToken") ? PlayerPrefs.GetString("RtToken") : "";
            return rtToken;
        }
    }

    // 移除token
    public void RemoveToken(bool clearUser = true)
    {
        PlayerPrefs.DeleteKey("RtToken");
        if(clearUser)
        {
            CurPlayer = new UserInfo();
        }
    }

    public bool DzEnabled
    {
        get
        {
            return CurPlayer.DzGameEnable > 0;
        }
    }

    public bool BullEnabled
    {
        get
        {
            return CurPlayer.CowGameEnable > 0;
        }
    }


    private int pId;
    public int PId
    {
        get
        {
            pId = PlayerPrefs.HasKey("PId") ? PlayerPrefs.GetInt("PId") : 1;
            return pId;
        }
    }

    public int Uid
    {
        get
        {
            if(CurPlayer != null)
            {
                return CurPlayer.Uid;
            }
            return 0;
        }
    }

    public HttpClient HttpReq;
    public PomeloMgr PomeloNode;
    public AudioManger AudioMgr;

    public UserInfo CurPlayer;

    public int VoiceOn;
    public string Language;

    public MainTabView MainTabView;

    // 这个地方，升级版本时要修改
    public int AndroidBuildVersionCode = 5;
    public int IOsBuildVersionCode = 5;

    private void Awake()
    {
        instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (HttpReq == null) {
            HttpReq = gameObject.GetComponent<HttpClient>();
        }
        if (PomeloNode == null) {
            PomeloNode = gameObject.GetComponent<PomeloMgr>();
        }
        if (AudioMgr == null) {
            AudioMgr = gameObject.GetComponent<AudioManger>();
        }
        if (CurPlayer == null) {
            CurPlayer = new UserInfo();
        }
        VoiceOn = PlayerPrefs.HasKey("VoiceOn") ? PlayerPrefs.GetInt("VoiceOn") : 1;
        Language = PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetString("Language") : "SC";
        DontDestroyOnLoad(this);
    }
    // Use this for initialization
    void Start () {
        InvokeRepeating("pin", 60, 60 * 3);
	}

    // Update is called once per frame
    void Update()
    {
		
    }

    void OnApplicationFocus(bool isFocus)
    {
    }

    void OnApplicationPause(bool isPause)
    {
    }

    void OnApplicationQuit()
    {
       
    }
    /// <summary>
    /// 加载Scene的UGUI的脚本一定要调用一次
    /// </summary>
    public void SetTips()
    {
        if (Tips == null) {
            Tips = Instantiate(Resources.Load<TipView>("Prefabs/Widgets/Tips"));
        }
        RectTransform ugui = GameObject.Find("_UGUI").GetComponent<RectTransform>();
        Tips.transform.SetParent(ugui);
        RectTransform t = Tips.transform as RectTransform;
        t.anchoredPosition3D = new Vector3(0, 30);
        t.localScale = Vector3.one;
    }


    public void ShowTips(string msg) {

        Tips.ShowMsg(msg);
    }


    void pin()
    {
        if (!string.IsNullOrEmpty(CurPlayer.SessionId))       // 已经登录
        {
            HttpReq.POST("code.php?_c=ping", null, (rsp, error) => {}, false);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke("pin");
    }

}
