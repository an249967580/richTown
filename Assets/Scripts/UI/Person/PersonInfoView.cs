using RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void EditAvatarEvent(string avatar);

public class PersonInfoView : MonoBehaviour {

    public CircleImage AvatarImg;
    public Button AvatarBtn;
    public InputField NickField;
    public Text UidText;

    public EditAvatarEvent OnEditAvatarEvent;

    UserInfo usr;

    private void Awake()
    {
        NotificationCenter.Instance.AddNotifyListener(NotificationType.EditAvatar, onSelectPhoto);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance.RemoveNotifyListener(NotificationType.EditAvatar, onSelectPhoto);
    }

    void onSelectPhoto(NotifyMsg msg)
    {
        string bytes = msg["avatar"] as string;
        if(Validate.IsNotEmpty(bytes))
        {
            updateAvatar(Convert.FromBase64String(bytes));
        }
    }

    void Start () {
        usr = Game.Instance.CurPlayer;
        if (usr != null) {
            UidText.text = usr.Uid.ToString();
            NickField.text = usr.NickName;

            if(Validate.IsNotEmpty(usr.Avatar))
            {
                StartCoroutine(LoadImageUtil.LoadImage(usr.Avatar, (sprite) =>
                {
                    AvatarImg.sprite = sprite;
                }));
            }
            else
            {
                AvatarImg.sprite = Resources.Load<Sprite>("Textures/Common/def_avatar_large");
            }

        }
        NickField.onEndEdit.AddListener(delegate {
            NickField.text = NickField.text.Trim();
            if (string.IsNullOrEmpty(NickField.text)) {
                Game.Instance.ShowTips(LocalizationManager.Instance.GetText("1111"));
            }
        });

        AvatarBtn.onClick.AddListener(delegate () {
            ToEditAvatar();
        });
    }

    void ToEditAvatar() {
        MainView.Instance.CreatePhotoSelectView();
    }

    void updateAvatar(byte[] bytes)
    {
        // 上传头像
        MainView.Instance.CreateLoadMask();
        Coroutine coroutine = StartCoroutine(hideMask());
        AwsS3Service.Instance.AsyncUploadObject(bytes, string.Format("avatar/user_{0}/{1}_{2}.jpg", usr.Uid, usr.Uid, DateTime.Now.ToString("yyyyMMddHHmmss")), (httpResult) =>
        {
            if (httpResult.IsOk)
             {
                 Dictionary<string, string> param = new Dictionary<string, string>();
                 string httpPath = httpResult.data;
                 param.Add("avatar", httpPath);
                 UserApi.EditUserInfo(param, (result) =>
                 {
                     MainView.Instance.HideMask();
                     // 失败
                     if (Validate.IsNotEmpty(result))
                     {
                         StopCoroutine(coroutine);
                         Game.Instance.ShowTips(result);
                         return;
                     }
                     Game.Instance.CurPlayer.Avatar = httpPath;
                     LoadImageUtil.LoadByte(bytes, (sprite) =>
                     {
                         AvatarImg.sprite = sprite;
                     });
                     if (OnEditAvatarEvent != null)
                     {
                         OnEditAvatarEvent(httpPath);
                     }
                 });
             }
             else
             {
                 Game.Instance.ShowTips(httpResult.errorMsg);
                 StopCoroutine(coroutine);
                 MainView.Instance.HideMask();
             }
         });
    }

    IEnumerator hideMask()
    {
        yield return new WaitForSeconds(15);
        MainView.Instance.HideMask();
    }
}
