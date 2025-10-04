using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void ButtonClickEvent();
public class DialogPanel : MonoBehaviour, IPointerClickHandler
{

    public Text TitleTxt;
    public Button CloseBtn;

    public Button CommitBtn;
    public Text CommitBtnTxt;

    public RectTransform Wrapper;
    public RectTransform ContentPanel;

    private ButtonClickEvent OnCommitClick;

    void Start () {
        RT.UIEventListener.Get(CloseBtn.gameObject).onClick = Close;
        RT.UIEventListener.Get(CommitBtn.gameObject).onClick = CommitClick;
    }
	
	void Update () {

    }
    void CommitClick(GameObject go)
    {
        if (OnCommitClick != null)
        {
            OnCommitClick();
        }
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            Close(gameObject);
        }
    }

    public void Close(GameObject go)
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    // 设置标题
    public DialogPanel SetTitle(string title)
    {
        TitleTxt.text = title;
        return this;
    }

    // 设置内容
    public DialogPanel SetContent(GameObject go)
    {
        RectTransform t = (RectTransform)go.gameObject.transform;
        float h = t.sizeDelta.y + 250;
        Wrapper.sizeDelta = new Vector2(Wrapper.sizeDelta.x, h);
        t.SetParent(ContentPanel);
        t.anchoredPosition3D = Vector3.zero;
        t.localScale = Vector3.one;
        t.offsetMax = Vector2.zero;
        t.offsetMin = Vector2.zero;

        return this;
    }

    // 设置确认按钮文本
    public DialogPanel SetButtonText(string txt)
    {
        CommitBtnTxt.text = txt;
        return this;
    }

    //设置确认按钮事件
    public DialogPanel SetClickEvent(ButtonClickEvent ent)
    {
        OnCommitClick = ent;
        return this;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
