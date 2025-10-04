using RT;
using UnityEngine;
using UnityEngine.UI;

public class WatcherItemView : ItemView
{
    public Text NickTxt;
    public CircleImage AvatarImg;

    public override void RegisterEvent()
    {
    }

    public override void Render()
    {
        WatcherData data = Data as WatcherData;
        LimitText.LimitAndSet(data.Nickname, NickTxt, 100);
        if (Validate.IsNotEmpty(data.Avatar))
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(LoadImageUtil.LoadImage(data.Avatar, (sprite) =>
                {
                    AvatarImg.sprite = sprite;
                }));
            }
        }
        else
        {
            AvatarImg.sprite = Resources.Load<Sprite>("Textures/Common/def_avatar_large");
        }
    }
}
