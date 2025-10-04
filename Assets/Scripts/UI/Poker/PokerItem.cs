using UnityEngine;
using UnityEngine.UI;

public class PokerItem : MonoBehaviour {
    public Image CardNumImg;
    public Button EyeBtn;
    public Image EyeImg;
    public string Type;

    public PokerRotation MyRotation;

    void Start () {
        MyRotation = GetComponent<PokerRotation>();
        if (!string.IsNullOrEmpty(Type))
        {
            RenderView(Type);
        }
	}
	
	void Update () {
		
	}
    /// <summary>
    /// 扑克牌图片初始化(不显示)
    /// </summary>
    /// <param name="type">扑克牌的类型，如2h</param>
    public void RenderView(string type) {
        Type = type;
        CardNumImg.sprite = Resources.Load<Sprite>("Textures/Poker/" + type);
    }

    public void Recycle() {
        Type = "";
        CardNumImg.sprite = null;
        MyRotation.Recycle();
    }
}
