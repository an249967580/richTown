using DG.Tweening;
using RT;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BullSeatView : MonoBehaviour {
    public Button SitBtn;
    public Image EmptyImg;
    public RectTransform Content;

    public CircleImage AvatarImg;
    public Button AvatarBtn;
    public Text NickTxt;
    public Text ChipTxt;

    public Image TagImg;
    public Text PreBankText;
    public Text BankText;
    public Image WonImg;
    public Text ChipResultTxt;
    
    public Image ChipImg;
    public Image RobotImg;

    public int SeatPosIndex;
    public int SuitPosIndex;
    public BullSuitView SuitView;

    public Image TimerImg;
    public Image TimerLighterImg;

    public float coldTime = 20;
    private float timer = 0;
    private bool isStartTimer = false;

    public int SeatNo = 0;
    public bool isSit = false;
    public BullPlayer Player;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ChipResultTxt.fontSize = 36;
    }

    void Update()
    {
        if (isStartTimer && TimerImg != null)
        {
            TimerLighterImg.gameObject.SetActive(true);
            Vector3 rot = TimerLighterImg.transform.localEulerAngles;

            timer += Time.deltaTime;
            TimerImg.fillAmount = timer / coldTime;

            rot.z = -timer / coldTime * 360;

            if (timer >= coldTime)
            {
                TimerImg.fillAmount = 0;
                timer = 0;
                isStartTimer = false;
                rot.z = 0;
                TimerLighterImg.gameObject.SetActive(false);
            }
            TimerLighterImg.transform.localEulerAngles = rot;
            TimerLighterImg.transform.parent.localEulerAngles = rot;
        }

    }
    #region UI动画

    public void PlayAudioEffect(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            if (Game.Instance.VoiceOn == 1)
            {
                audioSource.Play();
            }
        }
    }

    //坐下按钮点击
    public void SitDownClick()
    {
        SitDown();
    }
    public void SitDown()
    {
        isSit = true;
        Content.gameObject.SetActive(true);

        string avatar = Player.Avatar;
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
        NickTxt.text = Player.Nickname;
        ChipTxt.text = Player.Bet.ToString();
    }
    /// <summary>
    /// 玩家站立
    /// </summary>
    /// <param name="type"></param>
    public void StandUp(int type)
    {
        AvatarImg.sprite = Resources.Load<Sprite>("Textures/Common/def_avatar_large");
        //1为坐着的人看到有人站起，0为旁观者看到有人站起
        isSit = false;
        if (type == 1)
        {
            Content.gameObject.SetActive(false);
            EmptyImg.gameObject.SetActive(true);
        }
        else
        {
            Content.gameObject.SetActive(false);
            EmptyImg.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 显示空白座位
    /// </summary>
    public void ShowEmpty()
    {
        Content.gameObject.SetActive(false);
    }
    /// <summary>
    /// 玩家开始倒计时
    /// </summary>
    public void StartActionTimer()
    {
        isStartTimer = true;
    }
    /// <summary>
    /// 结束玩家操作倒计时
    /// </summary>
    public void EndActionTimer()
    {
        Vector3 rot = TimerLighterImg.transform.localEulerAngles;
        TimerImg.fillAmount = 0;
        timer = 0;
        isStartTimer = false;
        rot.z = 0;
        TimerLighterImg.gameObject.SetActive(false);
        TimerLighterImg.transform.localEulerAngles = rot;
        TimerLighterImg.transform.parent.localEulerAngles = rot;
    }

    /// <summary>
    /// 庄家修改
    /// </summary>
    /// <param name="state"></param>
    public void SetBanker(bool state)
    {
        TagImg.gameObject.SetActive(state);

    }

    /// <summary>
    /// 玩家倍数
    /// </summary>
    /// <param name="bet"></param>
    public void DoPreBank(int bet)
    {
        TagImg.gameObject.SetActive(false);
        BankText.gameObject.SetActive(false);
        PreBankText.gameObject.SetActive(true);
        PreBankText.text = "×" + bet;
    }

    public void DoSetBank(int bet)
    {
        if (bet == 0)
        {
            PreBankText.gameObject.SetActive(false);
            BankText.gameObject.SetActive(false);
            TagImg.gameObject.SetActive(false);
        }
        else
        {
            PreBankText.gameObject.SetActive(false);
            TagImg.gameObject.SetActive(true);
            BankText.gameObject.SetActive(true);
            BankText.text = "×" + bet;
        }
    }

    /// <summary>
    /// 更改玩家状态
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(BullPlayerState state)
    {
        Player.State = state;
        switch (state)
        {
            case BullPlayerState.Fold:
                break;
            case BullPlayerState.Bank:
                TagImg.gameObject.SetActive(true);
                break;
            default:
                TagImg.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 筹码转移动画
    /// </summary>
    /// <param name="target"></param>
    /// <param name="callback"></param>
    public void CollectChips(RectTransform target, Action callback = null)
    {
        ChipImg.gameObject.SetActive(true);
        Vector3 pos = target.position;
        Tweener tw1 = (ChipImg.transform as RectTransform).DOMove(pos, 0.6f);
        tw1.OnComplete(delegate
        {
            ChipImg.gameObject.SetActive(false);
            ChipImg.gameObject.transform.position = Vector3.zero;
            if (callback != null)
            {
                callback();
            }
        });
    }

    /// <summary>
    /// 显示结果动画
    /// </summary>
    /// <param name="chips"></param>
    public void ShowResult(int chips)
    {
        ChipResultTxt.gameObject.SetActive(true);
        var pos = (ChipResultTxt.transform as RectTransform).anchoredPosition3D;
        pos.y = 0;
        (ChipResultTxt.transform as RectTransform).anchoredPosition3D = pos;
        if (chips > 0)
        {
            WonImg.gameObject.SetActive(true);
            Font winFont = Resources.Load<Font>("Fonts/fnt-green/font");
            ChipResultTxt.font = winFont;
            ChipResultTxt.fontSize = 40;
            ChipResultTxt.text = "+" + chips;

            (ChipResultTxt.transform as RectTransform).DOLocalMoveY(90, 1.2f);
            Tweener tw2 = (WonImg.transform as RectTransform).DOLocalRotate(new Vector3(0, 0, 330), 5.2f);
            tw2.OnComplete(delegate
            {
                ChipResultTxt.gameObject.SetActive(false);
                WonImg.gameObject.SetActive(false);
            });
        }
        else if (chips < 0)
        {
            Font winFont = Resources.Load<Font>("Fonts/fnt-red/font");
            ChipResultTxt.font = winFont;
            ChipResultTxt.fontSize = 40;
            ChipResultTxt.text = "" + chips;

            (ChipResultTxt.transform as RectTransform).DOLocalMoveY(90, 1.2f);
            Tweener tw2 = (WonImg.transform as RectTransform).DOLocalRotate(new Vector3(0, 0, 330), 5.2f);
            tw2.OnComplete(delegate
            {
                ChipResultTxt.gameObject.SetActive(false);
            });
        }
        else
        {
            ChipResultTxt.gameObject.SetActive(false);
        }
    }

    public void SeatTurnAnim(Vector3[] wpts,Action callback = null)
    {
        //1.先隐藏玩家内容，聚集到中心
        if (wpts == null)
        {
            if (callback != null)
            {
                callback();
            }
            return;
        }
        EmptyImg.gameObject.SetActive(false);
        Content.gameObject.SetActive(false);

        (transform as RectTransform).DOLocalMove(Vector3.zero, 0.6f);

        Tweener tw2 = transform.DOLocalPath(wpts, 0.6f, PathType.CatmullRom).SetDelay(0.2f);
        tw2.OnComplete(() => {
            EmptyImg.gameObject.SetActive(!isSit);
            Content.gameObject.SetActive(isSit);
            if (callback != null)
            {
                callback();
            }
        });
    }

    public void SeatLocalMove(Vector3 pos) {
        transform.DOLocalMove(pos, 0.1f);
    }
    #endregion
}
