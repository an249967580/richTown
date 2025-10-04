using UnityEngine;
using UnityEngine.UI;

public class LanguageView : MonoBehaviour
{
    public ToggleGroup LanguageGroup;

    Toggle[] toggles;

    private string _language;

    public string Language
    {
        get
        {
            return _language;
        }
    }

    private void Awake()
    {
        _language = Game.Instance.Language;
    }

    void Start () {

        toggles = LanguageGroup.GetComponentsInChildren<Toggle>();

        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].onValueChanged.AddListener(delegate {
                ChangeLanguage();
            });
        }
        switch (Game.Instance.Language)
        {
            case "EN":
                toggles[0].isOn = true;
                break;
            case "SC":
                toggles[1].isOn = true;
                break;
            case "TC":
                toggles[2].isOn = true;
                break;
            case "MY":
                toggles[3].isOn = true;
                break;
        }
    }
	
    void ChangeLanguage()
    { 
        string str = "EN";
        for(int i=0;i<toggles.Length; i++){
            if (toggles[i].isOn) {
                str = i == 1 ? "SC" : i == 2 ? "TC" : i == 3 ? "MY" : "EN";
                break;
            }
        }
        _language = str;
    }
}
