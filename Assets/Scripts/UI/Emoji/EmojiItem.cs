using DG.Tweening;
using UnityEngine;

public class EmojiItem : MonoBehaviour {
    public float Time = 0.2f;

    public RectTransform TargetRect;

    UGUISpriteAnimation childAnim;
    bool isPlaying = false;
    void Awake () {
        childAnim = GetComponentInChildren<UGUISpriteAnimation>();
    }
	
	void Update () {
        if (!childAnim.IsPlaying && isPlaying) {
            Close();
        }
		
	}
    public void Show() {
        if (TargetRect != null)
        {
            RectTransform rect = gameObject.transform as RectTransform;
            Tweener tw = rect.DOMove(TargetRect.position, Time);
            tw.OnComplete(delegate () {
                isPlaying = true;
                childAnim.Play();
                transform.SetParent(TargetRect);
            });
        }
    }
    void Close() {
        isPlaying = false;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
