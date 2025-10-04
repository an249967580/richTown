using RT;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingView : MonoBehaviour
{
    public Toggle VoiceToggle;
    public Button LanguageBtn;
    public Button FeedbackBtn;
    public Button ServiceBtn;
    public Button PrivacyBtn;


    LanguageView SetLanguageView;
    DialogPanel DialogTpl;
    private void Awake()
    {
        DialogTpl = Resources.Load<DialogPanel>("Prefabs/Widgets/DialogPanel");
        SetLanguageView = Resources.Load<LanguageView>("Prefabs/Setting/LanguageDialogPanel");
    }
    void Start () {
        if (Game.Instance.VoiceOn == 1)
        {
            VoiceToggle.isOn = true;
        }
        else {
            VoiceToggle.isOn = false;
        }
        VoiceToggle.onValueChanged.AddListener(delegate{
            VoiceToggleClick();
        });
        LanguageBtn.onClick.AddListener(delegate {
            ShowLanguageDialog();
        });
        FeedbackBtn.onClick.AddListener(delegate {
            Application.OpenURL("http://richtown.io/contact-us");
        });
        ServiceBtn.onClick.AddListener(delegate {
            Application.OpenURL("http://richtown.io/terms");
        });
        PrivacyBtn.onClick.AddListener(delegate {
            Application.OpenURL("http://richtown.io/privacy-policy");
        });
    }
	
	void Update () {
		
	}

    void VoiceToggleClick()
    {
        if (Game.Instance.VoiceOn == 1)
        {
            VoiceToggle.isOn = false;
            Game.Instance.VoiceOn = 0;
        }
        else
        {
            VoiceToggle.isOn = true;
            Game.Instance.VoiceOn = 1;
        }
        PlayerPrefs.SetInt("VoiceOn", Game.Instance.VoiceOn);
    }
    void ShowLanguageDialog() {
        DialogPanel dialog = Instantiate(DialogTpl);
        LanguageView content = Instantiate(SetLanguageView);
        if (dialog)
        {
            RectTransform t = (RectTransform)dialog.gameObject.transform;
            GameObject ugui = GameObject.Find("_UGUI");
            RectTransform uguit = (RectTransform)ugui.gameObject.transform;
            t.SetParent(ugui.transform);
            t.sizeDelta = uguit.sizeDelta;
            t.localScale = Vector3.one;
            t.anchoredPosition3D = Vector3.zero;

            dialog.SetTitle(LocalizationManager.Instance.GetText("2102")).SetButtonText(LocalizationManager.Instance.GetText("1000")).SetContent(content.gameObject).SetClickEvent(()=> {
                if (content.Language != Game.Instance.Language)
                {
                    Game.Instance.Language = content.Language;
                    PlayerPrefs.SetString("Language", content.Language);
                    LocalizationManager.Instance.ResetLanguage();
                    SceneManager.LoadScene("MainScene");
                }
            });
            dialog.Show();
        }
    }
    void ShowFeedbackDialog() {

    }
    void ShowWebView(string url) {

    }
}
