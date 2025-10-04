using UnityEngine;
using UnityEngine.UI;

public class LoadMask : MonoBehaviour
{
    public Image LoadImg;
    void Start () {
		
	}
	
	void Update () {
        if (gameObject.activeSelf) {
            LoadImg.transform.Rotate(new Vector3(0, 0, LoadImg.transform.position.z-10), 3f);
        }
	}

    public void Show()
    {
        RectTransform ugui = GameObject.Find("_UGUI").GetComponent<RectTransform>();
        RectTransform t = (RectTransform)gameObject.transform;
        t.SetParent(ugui);
        t.anchoredPosition3D = Vector3.zero;
        t.localScale = Vector3.one;

        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.transform.SetParent(Game.Instance.transform);
        gameObject.SetActive(false);
    }
}
