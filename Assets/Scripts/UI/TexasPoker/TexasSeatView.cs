using DG.Tweening;
using RT;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TexasSeatView : MonoBehaviour
{
    public Button SitBtn;
    public Image EmptyImg;

    public RectTransform Content;

    public CircleImage AvatarImg;
    public Button AvatarBtn;
    public Text NickTxt;
    public Text ChipTxt;

    public Image RobotImg;
    public Image TagImg;
    public Image StatusImg;
    public Image WonImg;
    public Text ChipResultTxt;

    public Image DeskChipImg;
    public Image ChipImg;
    public Text DeskChipTxt;

    public RectTransform SmallArea;
    public RectTransform BigArea;

    public Image TimerImg;
    public Image TimerLighterImg;
    public float coldTime = 20;
    private float timer = 0;
    private bool isStartTimer = false;

    public int SeatNo = 0;
    public int SeatPosIndex = 0;
    public bool isSit = false;
    public TexasPlayer Player;
    public List<PokerItem> OwnPokerCards;

    public RectTransform[] ChipPoss;

    private AudioSource audioSource;
    bool isClockRing = false;

    void Start()
    {
        OwnPokerCards = new List<PokerItem>();
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

            if (coldTime - timer < 5 && isClockRing == false && Player.UId == Game.Instance.Uid) {
                isClockRing = true;
                PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("ntimeout"));
            }

            rot.z = -timer / coldTime * 360;

            if (timer >= coldTime)
            {
                TimerImg.fillAmount = 0;
                timer = 0;
                isStartTimer = false;
                isClockRing = false;
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
        if (clip!=null)
        {
            audioSource.clip = clip;
            if (Game.Instance.VoiceOn == 1)
            {
                audioSource.Play();
            }
        }
    }
    /// <summary>
    /// 关闭眼睛按钮
    /// </summary>
    public void CloseEyeBtns() {
        if (OwnPokerCards != null)
        {
            for (int i = 0; i < OwnPokerCards.Count; i++)
            {
                PokerItem p = OwnPokerCards[i];
                p.EyeImg.gameObject.SetActive(false);
                p.EyeBtn.onClick.RemoveAllListeners();
            }
        }
    }
    //坐下按钮点击
    public void SitDownClick() {
        SitDown();
    }
    public void SitDown() {
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
        if (Player.UId == Game.Instance.CurPlayer.Uid) {
            BigArea.anchoredPosition3D = ChipPoss[3].anchoredPosition3D;
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
        isClockRing = false;
        rot.z = 0;
        TimerLighterImg.gameObject.SetActive(false);
        TimerLighterImg.transform.localEulerAngles = rot;
        TimerLighterImg.transform.parent.localEulerAngles = rot;
    }

    /// <summary>
    /// 庄家修改
    /// </summary>
    /// <param name="state"></param>
    public void SetBanker(bool state) {
        TagImg.gameObject.SetActive(state);
    }
    /// <summary>
    /// 玩家下注
    /// </summary>
    /// <param name="bet"></param>
    public void DoBet(int bet)
    {
        Player.Bet -= bet;
        Player.Bet = Player.Bet < 0 ? 0 : Player.Bet;
        Player.DeskChip += bet;
        DeskChipImg.gameObject.SetActive(true);
        DeskChipTxt.text = Player.DeskChip.ToString(); // bet.ToString();
        ChipTxt.text = Player.Bet.ToString();
    }
    /// <summary>
    /// 更改玩家状态
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(TexasPlayerState state) {
        Player.State = state;
        StatusImg.gameObject.SetActive(true);
        switch (state) {
            case TexasPlayerState.Allin:
                StatusImg.sprite = Resources.Load<Sprite>("Textures/TexasPoker/actionTag_AllIn");
                break;
            case TexasPlayerState.Fold:
                StatusImg.sprite = Resources.Load<Sprite>("Textures/TexasPoker/actionTag_Fold_" + Game.Instance.Language);
                break;
            case TexasPlayerState.Check:
                StatusImg.sprite = Resources.Load<Sprite>("Textures/TexasPoker/actionTag_Check_" + Game.Instance.Language);
                break;
            case TexasPlayerState.Bet:
                StatusImg.sprite = Resources.Load<Sprite>("Textures/TexasPoker/actionTag_Bet_" + Game.Instance.Language);
                break;
            case TexasPlayerState.Call:
                StatusImg.sprite = Resources.Load<Sprite>("Textures/TexasPoker/actionTag_Call_" + Game.Instance.Language);
                break;
            case TexasPlayerState.Raise:
                StatusImg.sprite = Resources.Load<Sprite>("Textures/TexasPoker/actionTag_Raise_" + Game.Instance.Language);
                break;
            case TexasPlayerState.Wait:
                StatusImg.gameObject.SetActive(false);
                break;
            default:
                StatusImg.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 筹码收集动画
    /// </summary>
    /// <param name="target"></param>
    /// <param name="callback"></param>
    public void CollectChips(RectTransform target, Action callback = null)
    {
        DeskChipImg.gameObject.SetActive(false);
        ChipImg.gameObject.SetActive(true);
        SetChipImagePosition();
        Vector3 pos = target.position;
        Tweener tw1 = (ChipImg.transform as RectTransform).DOMove(pos, 0.6f);
        tw1.OnComplete(delegate
        {
            ChipImg.gameObject.SetActive(false);
            if (callback != null)
            {
                callback();
            }
        });
    }
    /// <summary>
    /// 筹码赢取动画
    /// </summary>
    /// <param name="target"></param>
    /// <param name="origin"></param>
    /// <param name="callback"></param>
    public void DealChips(RectTransform target, Vector3 origin, Action callback = null)
    {
        ChipImg.gameObject.SetActive(true);
        ChipImg.gameObject.transform.position = origin;

        Vector3 pos = target.position;
        Tweener tw1 = (ChipImg.transform as RectTransform).DOMove(pos, 0.3f).SetDelay(0.3f);
        tw1.OnComplete(delegate
        {
            ChipImg.gameObject.SetActive(false);
            if (callback != null)
            {
                callback();
            }
        });
    }
    /// <summary>
    /// 显示底牌动画
    /// </summary>
    public void ShowBottomCards()
    {
        for (int i = 0; i < OwnPokerCards.Count; i++)
        {
            if(!string.IsNullOrEmpty(OwnPokerCards[i].Type))
            OwnPokerCards[i].MyRotation.ShowDealCard(i, BigArea);
        }
    }

    /// <summary>
    /// 显示底牌动画
    /// </summary>
    public void ShowBottomCards(int index)
    {
        if (index < OwnPokerCards.Count)
        {
            OwnPokerCards[index].MyRotation.ShowDealCard(index, BigArea);
        }
    }
    /// <summary>
    /// 显示结果动画
    /// </summary>
    /// <param name="chips"></param>
    public void ShowResult(int chips)
    {
        Tweener tw = (ChipResultTxt.transform as RectTransform).DOLocalRotate(Vector3.one, 0.2f).SetDelay(0.9f);
        tw.OnComplete(delegate {
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


        });

    }

    void SetChipImagePosition()
    {
        Image img = DeskChipImg.GetComponentInChildren<Image>();
        ChipImg.transform.DOMove(img.transform.position, 0.01f);
    }

    public void SeatTurnAnim(Vector3[] wpts,int chipPos, Action callback = null) {
        //1.先隐藏玩家内容，聚集到中心
        if (wpts == null) {
            DeskChipImg.transform.DOLocalMove(ChipPoss[chipPos].localPosition, 0.01f);
            if (callback != null)
            {
                callback();
            }
            return;
        }
        EmptyImg.gameObject.SetActive(false);
        Content.gameObject.SetActive(false);
        DeskChipImg.transform.DOLocalMove(ChipPoss[chipPos].localPosition, 0.01f);

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
    #endregion
}
