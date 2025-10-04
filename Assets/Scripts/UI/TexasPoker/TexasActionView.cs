using RT;
using UnityEngine;
using UnityEngine.UI;

public class TexasActionView : MonoBehaviour
{
    public RectTransform ActionPanel;
    public Button ActionFoldBtn;
    public Button ActionCheckBtn;
    public Button ActionAllinBtn;
    public Button ActionCallBtn;
    public Button ShowRaiseBtn;

    public RectTransform RaisePanel;
    public Button ActionRaiseBtn;
    public InputField RaiseField;
    public Slider RaiseSlider;
    public Button AddRaiseBtn;
    public Button RedRaiseBtn;
    bool isAjust = false;

    public RectTransform RaiseBtnPanel;
    public Button R2xBtn;
    public Button R3xBtn;
    public Button R4xBtn;

    public RectTransform BetBtnPanel;
    public Button Pot12Btn;
    public Button Pot23Btn;
    public Button Pot100Btn;


    public RectTransform AutoNextPanel;
    public ToggleGroup AutoNextToggleGroup;
    public Toggle ActionFoldToggle;
    public Toggle ActionCheckToggle;
    public Toggle ActionCallToggle;
    public Toggle ActionAllinToggle;
    public Toggle ActionAnyCallToggle;

    int Pot = 0;
    int CurMaxBet = 0;
    int MyDeskBet = 0;

    void Start() {
        AddRaiseBtn.onClick.AddListener(delegate{
            RaiseAddClick();
        });
        RedRaiseBtn.onClick.AddListener(delegate{
            RaiseRedClick();
        });
        Pot12Btn.onClick.AddListener(delegate {
            PotBtnClick(0.5f);
        });
        Pot23Btn.onClick.AddListener(delegate {
            PotBtnClick(2.0f/3.0f);
        });
        Pot100Btn.onClick.AddListener(delegate {
            PotBtnClick(1);
        });
        R2xBtn.onClick.AddListener(delegate {
            RaiseBtnClick(2);
        });
        R3xBtn.onClick.AddListener(delegate {
            RaiseBtnClick(3);
        });
        R4xBtn.onClick.AddListener(delegate {
            RaiseBtnClick(4);
        });


        RaiseSlider.onValueChanged.AddListener(delegate{
            RaiseSliderChanged();
        });
        RaiseField.keyboardType = TouchScreenKeyboardType.NumberPad;
        RaiseField.onEndEdit.AddListener(delegate {
            RaiseFieldEndEdit();
        });
        RaiseField.text = "" + (int)RaiseSlider.value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="overBetNum">加注次数</param>
    /// <param name="maxBetLimit">最大加注筹码限制</param>
    /// <param name="myDeskChip">我已下注的筹码</param>
    /// <param name="myBet">我的剩余筹码</param>
    /// <param name="maxBlind">大盲注</param>
    /// <param name="curMaxChip">当前最高下注筹码</param>
    /// <param name="pot">底池</param>
    public void ActiveActionPanel(int overBetNum, int maxBetLimit, int myDeskChip, int myBet, int maxBlind, int curMaxChip,int pot)
    {
        ActionPanel.gameObject.SetActive(true);
        AutoNextPanel.gameObject.SetActive(false);
        RaisePanel.gameObject.SetActive(false);

        Pot = pot;
        CurMaxBet = curMaxChip;
        MyDeskBet = myDeskChip;

        //2.如果桌面筹码>=当前筹码，可以过牌
        if (myDeskChip >= curMaxChip)
        {
            ActionCallBtn.gameObject.SetActive(false);
            ActionCheckBtn.gameObject.SetActive(true);
            //2.1.如果用户仍剩余筹码，就可以下注
            if (myBet > 0)
            {
                //2.1.1.如果玩家剩余筹码>大盲注显示加注，否则allin
                if (myBet > maxBlind)
                {
                    ShowRaiseBtn.gameObject.SetActive(true);
                    ActionAllinBtn.gameObject.SetActive(false);
                }
                else
                {
                    ShowRaiseBtn.gameObject.SetActive(false);
                    ActionAllinBtn.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            //3.如果桌面筹码 < 当前筹码
            //3.1.如果剩余筹码+桌面筹码低于当前筹码，只能Allin和弃牌
            if ((myDeskChip + myBet) <= curMaxChip)
            {
                ActionAllinBtn.gameObject.SetActive(true);
                ActionCheckBtn.gameObject.SetActive(false);
                ActionCallBtn.gameObject.SetActive(false);
                ShowRaiseBtn.gameObject.SetActive(false);
            }
            else
            {
                //3.1.1.如果剩余筹码<=2*(当前筹码-桌面筹码<+大忙注，只能Allin，跟牌和弃牌
                if (myBet <= 2*(curMaxChip - myDeskChip))
                {
                    ActionAllinBtn.gameObject.SetActive(true);
                    ActionCheckBtn.gameObject.SetActive(false);
                    ActionCallBtn.gameObject.SetActive(false);
                    ShowRaiseBtn.gameObject.SetActive(false);
                }
                else
                {
                    //3.1.1.如果剩余筹码>2*(当前筹码-桌面筹码<+大忙注，跟牌和加注
                    ActionAllinBtn.gameObject.SetActive(false);
                    ActionCheckBtn.gameObject.SetActive(false);
                    ActionCallBtn.gameObject.SetActive(true);
                    ShowRaiseBtn.gameObject.SetActive(true);
                }
            }
        }

        //1.如果已经加注3次就不能加注
        if (ShowRaiseBtn.gameObject.activeSelf)
        {
            ShowRaiseBtn.gameObject.SetActive(overBetNum == 0 ? false : true);
        }
        if (curMaxChip > 0)
        {
            Text txt1 = ActionRaiseBtn.GetComponentInChildren<Text>();
            txt1.text = LocalizationManager.Instance.GetText("7005");  //  加注
            Text txt2 = ShowRaiseBtn.GetComponentInChildren<Text>();   //  加注
            txt2.text = LocalizationManager.Instance.GetText("7005");
            RaiseBtnPanel.gameObject.SetActive(ShowRaiseBtn.gameObject.activeSelf);
            BetBtnPanel.gameObject.SetActive(false);
            if (ShowRaiseBtn.gameObject.activeSelf)
            {
                //最低为大盲注，最高为用户的剩余筹码或者系统要求的最高押注（两取最小）
                int betMax = maxBetLimit > myBet ? myBet : maxBetLimit;
                RaiseSlider.maxValue = betMax;
                if (myDeskChip == maxBlind / 2)
                {
                    RaiseSlider.minValue = curMaxChip >= maxBlind ? 2 * curMaxChip - myDeskChip : maxBlind;
                }
                else
                {
                    RaiseSlider.minValue = curMaxChip >= maxBlind ? 2 * (curMaxChip - myDeskChip) : maxBlind;
                }
                RaiseSlider.value = RaiseSlider.minValue;

                R2xBtn.gameObject.SetActive(myBet >= (2 * curMaxChip - myDeskChip));
                R3xBtn.gameObject.SetActive(myBet >= (3 * curMaxChip - myDeskChip));
                R4xBtn.gameObject.SetActive(myBet >= (4 * curMaxChip - myDeskChip));
            }
        }
        else
        {
            Text txt1 = ActionRaiseBtn.GetComponentInChildren<Text>();
            txt1.text = LocalizationManager.Instance.GetText("7006");  //  下注
            Text txt2 = ShowRaiseBtn.GetComponentInChildren<Text>();
            txt2.text = LocalizationManager.Instance.GetText("7006");  //  下注
            BetBtnPanel.gameObject.SetActive(ShowRaiseBtn.gameObject.activeSelf);
            RaiseBtnPanel.gameObject.SetActive(false);
            if (ShowRaiseBtn.gameObject.activeSelf)
            {
                //最低为大盲注，最高为用户的剩余筹码或者系统要求的最高押注（两取最小）
                int betMax = maxBetLimit > myBet ? myBet : maxBetLimit;
                RaiseSlider.maxValue = betMax;
                RaiseSlider.minValue = curMaxChip > maxBlind ? curMaxChip : maxBlind;
                RaiseSlider.value = RaiseSlider.minValue;
                Pot12Btn.gameObject.SetActive(myBet >= (1 / 2 * pot));
                Pot23Btn.gameObject.SetActive(myBet >= (2 / 3 * pot));
                Pot100Btn.gameObject.SetActive(myBet >= pot);
            }
        }
    }

    void PotBtnClick(float mul)
    {
        isAjust = true;
        RaiseSlider.value = mul * Pot;
    }
    void RaiseBtnClick(int mul)
    {
        isAjust = true;
        RaiseSlider.value = mul * CurMaxBet - MyDeskBet;
    }

    void RaiseAddClick() {
        int val = (int)RaiseSlider.value +1;
        if (val > (int)RaiseSlider.maxValue) {
            val = (int)RaiseSlider.maxValue;
        }
        isAjust = true;
        RaiseSlider.value = val;
    }
    void RaiseRedClick()
    {
        int val = (int)RaiseSlider.value - 1;
        if (val < (int)RaiseSlider.minValue)
        {
            val = (int)RaiseSlider.minValue;
        }

        isAjust = true;
        RaiseSlider.value = val;
    }

    void RaiseSliderChanged()
    {
        if (isAjust)
        {
            isAjust = false;
        }
        else
        {
            if (RaiseSlider.value != RaiseSlider.maxValue && RaiseSlider.value != RaiseSlider.minValue)
            {
                int val = (int)RaiseSlider.value;
                float x = (val / RaiseSlider.maxValue) * 100;
                int y = (int)x / 5;
                int z = (int)x % 5;
                y = z > 0 ? y + 1 : y;
                int newVal = (int)(y / 20.0f * RaiseSlider.maxValue);
                if (newVal > RaiseSlider.maxValue)
                {
                    RaiseSlider.value = RaiseSlider.maxValue;
                }
                else if (newVal < RaiseSlider.minValue)
                {
                    RaiseSlider.value = RaiseSlider.minValue;
                }
                else
                {
                    RaiseSlider.value = newVal;
                }
            }
        }
        if (RaiseSlider.value == RaiseSlider.maxValue)
        {
            RaiseField.text = "Allin";
        }
        else
        {
            RaiseField.text = "" + (int)RaiseSlider.value;
        }
    }


    void RaiseFieldEndEdit()
    {
        isAjust = true;
        if (string.IsNullOrEmpty(RaiseField.text))
        {
            RaiseSlider.value = RaiseSlider.minValue;
            return;
        }
        int val = int.Parse(RaiseField.text);
        if (val > RaiseSlider.maxValue)
        {
            RaiseSlider.value = RaiseSlider.maxValue;
        }
        else if (val < RaiseSlider.minValue)
        {
            RaiseSlider.value = RaiseSlider.minValue;
        }
        else
        {
            RaiseSlider.value = val;
        }
    }


    public void CloseActionPanel() {
        ActionPanel.gameObject.SetActive(false);
        RaisePanel.gameObject.SetActive(false);
    }
    public void ActiveAutoNextPanel(int curDeskChip, int curPlayerBet, int curMaxChip) {
        ActionPanel.gameObject.SetActive(false);
        AutoNextPanel.gameObject.SetActive(true);
        RaisePanel.gameObject.SetActive(false);


        if (curDeskChip >= curMaxChip)
        {
            ActionAllinToggle.gameObject.SetActive(false);
            ActionCallToggle.gameObject.SetActive(false);
        }
        else{
            if (curPlayerBet > curMaxChip - curDeskChip)
            {
                ActionAllinToggle.gameObject.SetActive(false);
                ActionCallToggle.gameObject.SetActive(true);
                Text txt = ActionCallToggle.GetComponentInChildren<Text>();
                txt.text = LocalizationManager.Instance.GetText("7007") + (curMaxChip - curDeskChip); // 跟注
            }
            else
            {
                ActionAllinToggle.gameObject.SetActive(true);
                ActionCallToggle.gameObject.SetActive(false);
            }
        }
    }

    public void CloseAutoNextPanel()
    {
        AutoNextPanel.gameObject.SetActive(false);
    }

    public void SetToggleOff()
    {
        ActionFoldToggle.isOn = false;
        ActionCallToggle.isOn = false;
        ActionAllinToggle.isOn = false;
        ActionCheckToggle.isOn = false;
        ActionAnyCallToggle.isOn = false;
    }
}
