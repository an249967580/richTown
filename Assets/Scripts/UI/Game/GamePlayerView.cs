using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GamePlayerView : MonoBehaviour, IPointerClickHandler
{
    public Button CloseBtn;

    public CircleImage PlayerAvatarImg;
    public Image PlayerAvatarRing;
    public Text PlayerNickTxt;
    public Text PlayerIDTxt;
    public Button LastVoiceBtn;

    public Text VPIPTtitle;
    public Text TotalHandsTitle;
    public Text TotalGamesTtitle;
    public Text VPIPTxt;
    public Text TotalHandsTxt;
    public Text TotalGamesTxt;

    public Text EmojiHintTxt;
    public HListView EmojiListView;

    public Text LimitHintTxt;
    public RectTransform LimitPanel;

    public int TargetUId;
    public int ClubId;
    GamePlayer player;

    List<EmojiItemData> selfEmojis;
    List<EmojiItemData> otherEmojis;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            Close();
        }
    }

    public void Close()
    {
        EmojiListView.Clear();
        gameObject.SetActive(false);
    }
    void GetPlayerInfo()
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("clubId", "-1");
        param.Add("uid", TargetUId.ToString());
        UserApi.GetUserInfo(param, (p, error) =>
        {
            if (error == null)
            {
                player = p;
                gameObject.SetActive(true);
                RenderView();
            }
            else
            {
                Game.Instance.ShowTips(error);
                Close();
            }

        });
    }
    void InitView()
    {
        if (TargetUId == Game.Instance.CurPlayer.Uid)
        {
            EmojiHintTxt.text = string.Format(LocalizationManager.Instance.GetText("7004"), 1);
            for (int i = 0; i < selfEmojis.Count; i++)
            {
                ItemView vi = EmojiListView.Add(selfEmojis[i]);
                vi.OnItemClickEvent += (iv) =>
                {
                    EmojiItemData d = vi.Data as EmojiItemData;
                    SendEmoji(d.Emoji);
                };
            }
        }
        else
        {
            EmojiHintTxt.text = string.Format(LocalizationManager.Instance.GetText("7004"), 2);
            for (int i = 0; i < otherEmojis.Count; i++)
            {
                ItemView vi = EmojiListView.Add(otherEmojis[i]);
                vi.OnItemClickEvent += (iv) =>
                {
                    EmojiItemData d = vi.Data as EmojiItemData;
                    SendEmoji(d.Emoji);
                };
            }
        }
        CloseBtn.onClick.AddListener(delegate ()
        {
            Close();
        });
    }
    void InitData()
    {
        selfEmojis = new List<EmojiItemData>();
        EmojiItemData s1 = new EmojiItemData();
        s1.Emoji = "Fahuo";
        s1.ImgUrl = "Textures/Emoji/Self/fahuo_btn";
        EmojiItemData s2 = new EmojiItemData();
        s2.Emoji = "Saoxiang";
        s2.ImgUrl = "Textures/Emoji/Self/saoxiang_btn";
        EmojiItemData s3 = new EmojiItemData();
        s3.Emoji = "Touxiao";
        s3.ImgUrl = "Textures/Emoji/Self/touxiao_btn";
        EmojiItemData s4 = new EmojiItemData();
        s4.Emoji = "Wabikong";
        s4.ImgUrl = "Textures/Emoji/Self/wabikong_btn";
        EmojiItemData s5 = new EmojiItemData();
        s5.Emoji = "Xiao";
        s5.ImgUrl = "Textures/Emoji/Self/xiao_btn";
        EmojiItemData s6 = new EmojiItemData();
        s6.Emoji = "Xinsui";
        s6.ImgUrl = "Textures/Emoji/Self/xinsui_btn";

        selfEmojis.Add(s1);
        selfEmojis.Add(s2);
        selfEmojis.Add(s3);
        selfEmojis.Add(s4);
        selfEmojis.Add(s5);
        selfEmojis.Add(s6);

        otherEmojis = new List<EmojiItemData>();
        EmojiItemData s11 = new EmojiItemData();
        s11.Emoji = "Kiss";
        s11.ImgUrl = "Textures/Emoji/Player/kiss_btn";
        EmojiItemData s12 = new EmojiItemData();
        s12.Emoji = "Like";
        s12.ImgUrl = "Textures/Emoji/Player/like_btn";
        EmojiItemData s13 = new EmojiItemData();
        s13.Emoji = "Renshi";
        s13.ImgUrl = "Textures/Emoji/Player/renshi_btn";
        EmojiItemData s14 = new EmojiItemData();
        s14.Emoji = "Zhadan";
        s14.ImgUrl = "Textures/Emoji/Player/zhadan_btn";
        EmojiItemData s15 = new EmojiItemData();
        s15.Emoji = "Zhuaji";
        s15.ImgUrl = "Textures/Emoji/Player/zhuaji_btn";
        EmojiItemData s16 = new EmojiItemData();
        s16.Emoji = "Zhuayu";
        s16.ImgUrl = "Textures/Emoji/Player/zhuayu_btn";

        otherEmojis.Add(s11);
        otherEmojis.Add(s12);
        otherEmojis.Add(s13);
        otherEmojis.Add(s14);
        otherEmojis.Add(s15);
        otherEmojis.Add(s16);
    }
    void RenderView()
    {
        string avatar = player.Avatar;
        if (Validate.IsNotEmpty(avatar))
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(LoadImageUtil.LoadImage(avatar, (sprite) =>
                {
                    PlayerAvatarImg.sprite = sprite;
                }));
            }
        }
        else
        {
            PlayerAvatarImg.sprite = Resources.Load<Sprite>("Textures/Common/def_avatar_large");
        }
        PlayerNickTxt.text = player.Nickname;
        PlayerIDTxt.text = "ID:" + TargetUId;
        VPIPTxt.text = player.EnterPool * 100 + "%";
        TotalHandsTxt.text = player.HandTotal + "";
        TotalGamesTxt.text = player.RoomTotal + "";
    }

    public void Show(int tagId, int clubId, bool limit = false)
    {
        gameObject.SetActive(true);
        LimitPanel.gameObject.SetActive(limit);
        TargetUId = tagId;
        ClubId = clubId;
        InitData();
        InitView();
        GetPlayerInfo();
    }

    void SendEmoji(string emoji)
    {
        Close();
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("emoji", emoji);
        param.Add("uid", TargetUId.ToString());
        UserApi.UseEmoji(param ,(error)=> {
            if (error != null) {
                Game.Instance.ShowTips(error);
            }
        });
        // NotifyMsg msg = new NotifyMsg()
        // .value("from", TargetUId)
        // .value("to", TargetUId)
        // .value("emoji", emoji);

        // NotificationCenter.Instance.DispatchNotify(NotificationType.local_user_emoji, msg);
    }
}
