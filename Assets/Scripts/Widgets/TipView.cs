using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TipView : MonoBehaviour
{
    public Text MsgTxt;
    RectTransform rect;
    Tweener tw;

    void Start () {
        rect = gameObject.transform as RectTransform;
        tw = rect.DOLocalMove(new Vector3(0, rect.localPosition.y - 60, 0), 0.8f);
        tw.SetAutoKill(false);
        tw.Pause();
    }
	void Update () {
		
	}

	public void ShowMsg(string msg) {
		rect.anchoredPosition3D = new Vector3(0, 30);
		tw.ChangeValues (new Vector3 (0, rect.localPosition.y, 0), new Vector3 (0, rect.localPosition.y - 60, 0), 0.8f);
        MsgTxt.text = msg;
		tw.PlayForward();
        tw.OnComplete(delegate ()
        {
            //Debug.Log("Complete");
            StartCoroutine(Timer(1));
        });
    }
    public IEnumerator Timer(float time)
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
        }
        tw.PlayBackwards();
    }
}
