using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TexasFinalStatView : MonoBehaviour {
    public Text ViewTitle;
    public Button CloseBtn;

    public CircleImage WinnerAvatarImg;
    public Text WinnerNickTxt;
    public CircleImage FishAvatarImg;
    public Text FishNickTxt;
    public CircleImage RichAvatarImg;
    public Text RichNickTxt;
    public Image FishImg;
    public Image WinImg;
    public Image RichImg;

    public CircleImage SelfAvatarImg;
    public Text SelfNickTxt;
    public Text SelfBuyinTitle;
    public Text SelfBuyinTxt;
    public Text SelfProfitTitle;
    public Text SelfProfitTxt;
    
    public Text HeadNickTitle;
    public Text HeadBuyinTitle;
    public Text HeadProfitTitle;
    public ListView FinalListView;

    void Start () {
        CloseBtn.onClick.AddListener(delegate() {
            Close();
        });
    }
	
	void Update () {
		
	}

    void Close()
    {
        string last = PlayerPrefs.GetString("LastScene");
        if (string.IsNullOrEmpty(last))
        {
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            SceneManager.LoadScene(last);
        }
    }

    public void InitView(List<GameLiveData> datas) {
        if (datas != null)
        {
            FinalListView.Clear();
            int tmpfish = 0;
            int tmpfishIdx = 0;
            int tmpwin = 0;
            int tmpwinIdx = 0;
            int tmprich = 0;
            int tmprichIdx = 0;
            int selfIdx = -1;

            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].TotalWinHands > tmpwin) {
                    tmpwin = datas[i].TotalWinHands;
                    tmpwinIdx = i;
                }
                if (datas[i].ProfitLoss > tmprich)
                {
                    tmprich = datas[i].ProfitLoss;
                    tmprichIdx = i;
                }
                if (datas[i].ProfitLoss <= tmpfishIdx)
                {
                    tmpfish = datas[i].TotalWinHands;
                    tmpfishIdx = i;
                }
                if (datas[i].UId == Game.Instance.CurPlayer.Uid) {
                    selfIdx = i;
                }
                FinalListView.Add(datas[i]);
            }

            GameLiveData fish = datas[tmpfishIdx];
            GameLiveData win = datas[tmpwinIdx];
            GameLiveData rich = datas[tmprichIdx];

            if (!string.IsNullOrEmpty(fish.Avatar))
            {
                StartCoroutine(LoadImageUtil.LoadImage(fish.Avatar, (sprite) =>
                {
                    FishAvatarImg.sprite = sprite;
                }));
            }
            FishNickTxt.text = fish.Nickname;
            if (!string.IsNullOrEmpty(win.Avatar))
            {
                StartCoroutine(LoadImageUtil.LoadImage(win.Avatar, (sprite) =>
                {
                    WinnerAvatarImg.sprite = sprite;
                }));
            }
            WinnerNickTxt.text = win.Nickname;
            if (!string.IsNullOrEmpty(rich.Avatar))
            {
                StartCoroutine(LoadImageUtil.LoadImage(rich.Avatar, (sprite) =>
                {
                    RichAvatarImg.sprite = sprite;
                }));
            }
            RichNickTxt.text = rich.Nickname;

            if (selfIdx >= 0)
            {
                GameLiveData self = datas[selfIdx];
                if (!string.IsNullOrEmpty(self.Avatar))
                {
                    StartCoroutine(LoadImageUtil.LoadImage(self.Avatar, (sprite) =>
                    {
                        SelfAvatarImg.sprite = sprite;
                    }));
                }
                SelfNickTxt.text = self.Nickname;
                SelfBuyinTxt.text = self.TotalBuyBet.ToString();
                if (self.ProfitLoss > 0)
                {
                    SelfProfitTxt.text = "+" + self.ProfitLoss.ToString();
                    SelfProfitTxt.color = Color.green;
                }
                else if (self.ProfitLoss < 0)
                {
                    SelfProfitTxt.text = self.ProfitLoss.ToString();
                    SelfProfitTxt.color = Color.red;
                }
                else
                {
                    SelfProfitTxt.text = "" + self.ProfitLoss.ToString();
                    SelfProfitTxt.color = Color.white;
                }
            }
        }
    }
}
