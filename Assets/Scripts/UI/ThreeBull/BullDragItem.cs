using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BullDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int Card;
    public Image CardImage;
    public event Action<BullDragItem> onDragDoneEvent;

    private Vector3 Original;
    private Transform OriginalParent;


    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetCardType(int card) {
        Card = card;
        CardImage.sprite = Resources.Load<Sprite>("Textures/Poker/Bull/" + card);
    }

    /// <summary>
     /// 开始拖动
     /// </summary>
     /// <param name="EventData"></param>
    public void OnBeginDrag(PointerEventData EventData)
    {
        OriginalParent = this.transform.parent;//设置父位置点
        Original = this.transform.position;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;//设置射线穿透
    }
    /// <summary>
    /// 正在拖拽
    /// </summary>
    /// <param name="EventData"></param>
    public void OnDrag(PointerEventData EventData)
    {
        this.transform.position = Input.mousePosition;//当前位置为鼠标所在位置
        this.transform.SetParent(EventData.pointerEnter.transform.parent);
    }
    public void OnEndDrag(PointerEventData EventData)
    {
        this.transform.localPosition = new Vector3(0, 0, 0);
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (EventData.pointerEnter.GetComponent<BullDragItem>() == null)
        {
            //如果目标不是其他item，则放回原位置
            this.transform.position = Original;
            this.transform.SetParent(OriginalParent);
        }
        else
        {
            EventData.pointerEnter.transform.position = Original;
            EventData.pointerEnter.transform.SetParent(OriginalParent);
            OriginalParent = this.transform.parent;
            Original = this.transform.position;
            if (onDragDoneEvent != null) {
                onDragDoneEvent(this);
            }
        }
    }
}
