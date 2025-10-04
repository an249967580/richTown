using RT;
using UnityEngine;
using UnityEngine.UI;

public class EmojiItemView : ItemView
{
    public Image EmojiImg;

    public override void RegisterEvent()
    {
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            if (OnItemClickEvent != null)
            {
                OnItemClickEvent.Invoke(this);
            }
        });
    }

    public override void Render()
    {
        EmojiItemData data = Data as EmojiItemData;
        EmojiImg.sprite = Resources.Load<Sprite>(data.ImgUrl);
    }
}
