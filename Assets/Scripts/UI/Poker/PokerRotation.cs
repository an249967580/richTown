using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PokerRotation : MonoBehaviour {
    public Image BackImg;
    public Image FrontImg;

    Tweener btw;
    Tweener ftw;
    private void Awake()
    {
        FrontImg.gameObject.SetActive(false);
        FrontImg.gameObject.transform.localEulerAngles = new Vector3(0, 90, 0);
    }
    void Start ()
    {
    }
	
	void Update () {
		
	}
    public void Recycle()
    {
        FrontImg.gameObject.SetActive(false);
        FrontImg.gameObject.transform.localEulerAngles = new Vector3(0, 90, 0);
        BackImg.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);

        transform.localEulerAngles = Vector3.zero;
        (transform as RectTransform).anchoredPosition3D = Vector3.zero;
    }

    public void ShowCard() {
        if (!FrontImg.gameObject.activeSelf)
        {
            FrontImg.gameObject.SetActive(true);
            btw = (BackImg.transform as RectTransform).DOLocalRotate(new Vector3(0, 90, 0), 0.1f);
            ftw = (FrontImg.transform as RectTransform).DOLocalRotate(Vector3.zero, 0.2f).SetDelay(0.1f);
        }
    }

    #region 德州
    /// <summary>
    /// 发玩家底牌
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    public void DealCard(int type, RectTransform target)
    {
        Vector3 rot = transform.localEulerAngles;
        Vector3 pos = target.position;

        rot.z = type == 1 ? -15 : 0;
        pos.x += type == 1 ? 6 : -6;
        transform.localEulerAngles = rot;
        Tweener tw = (transform as RectTransform).DOMove(pos, 0.4f);
        tw.OnComplete(delegate {
            transform.SetParent(target);
        });
    }
    /// <summary>
    /// 翻玩家底牌
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    public void ShowDealCard(int type, RectTransform target)
    {
        RectTransform rect = transform as RectTransform;

        Vector3 rot = rect.localEulerAngles;
        Vector3 pos = target.anchoredPosition3D;
        
        rot.z = 0;
        transform.SetParent(target);
        pos.x = type == 1 ? rect.rect.size.x * 0.70f / 2 : -rect.rect.size.x * 0.70f / 2;
        transform.localEulerAngles = rot;
        Tweener tw = (transform as RectTransform).DOLocalMove(pos, 0.2f);
        tw.OnComplete(delegate {
            ShowCard();
        });
        (transform as RectTransform).DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.2f);
    }
    /// <summary>
    /// 中间牌组摊开
    /// </summary>
    /// <param name="x"></param>
    public void SpreadOut(float x) {

        RectTransform rect = transform as RectTransform;

        Tweener tw = (transform as RectTransform).DOLocalMoveX(rect.anchoredPosition3D.x + x * rect.rect.size.x * 0.92f, 0.5f).SetDelay(0.2f);
        tw.OnComplete(delegate {
            ShowCard();
        });
    }
    /// <summary>
    /// 发中间3张底牌并摊开牌组
    /// </summary>
    /// <param name="target"></param>
    /// <param name="x"></param>
    public void DealCenterCardAndSpreadOut(RectTransform target,float x)
    {
        RectTransform rect = transform as RectTransform;
        Vector3 pos = target.anchoredPosition3D;
        transform.SetParent(target);
        pos.x = pos.x - 2 * rect.rect.size.x * 0.92f;
        pos.y = 0;
        Tweener tw1 = (transform as RectTransform).DOLocalMove(pos, 0.1f);
        tw1.OnComplete(delegate {
            SpreadOut(x);
        });
        (transform as RectTransform).DOScale(new Vector3(0.92f,0.92f,0.92f), 0.1f);
    }
    /// <summary>
    /// 发第4张底牌
    /// </summary>
    /// <param name="target"></param>
    /// <param name="x"></param>
    public void DealFourthCenterCard(RectTransform target)
    {
        RectTransform rect = transform as RectTransform;
        Vector3 pos = target.anchoredPosition3D;
        transform.SetParent(target);
        pos.x = pos.x + rect.rect.size.x * 0.92f;
        pos.y = 0;
        Tweener tw1 = (transform as RectTransform).DOLocalMove(pos, 0.1f);
        tw1.OnComplete(delegate {
            ShowCard();
        });
        (transform as RectTransform).DOScale(new Vector3(0.92f, 0.92f, 0.92f), 0.1f);
    }
    /// <summary>
    /// 发第5张底牌
    /// </summary>
    /// <param name="target"></param>
    /// <param name="x"></param>
    public void DealFifthCenterCard(RectTransform target)
    {
        RectTransform rect = transform as RectTransform;
        Vector3 pos = target.anchoredPosition3D;
        transform.SetParent(target);
        pos.x = pos.x + 2 * rect.rect.size.x * 0.92f;
        pos.y = 0;
        Tweener tw1 = (transform as RectTransform).DOLocalMove(pos, 0.1f);
        tw1.OnComplete(delegate {
            ShowCard();
        });
        (transform as RectTransform).DOScale(new Vector3(0.92f, 0.92f, 0.92f), 0.1f);
    }
    #endregion

    #region 牛牛

    public void DealCard2Target(RectTransform target, bool isShow = false, float scale = 0.6f, float duration = 0.2f)
    {
        BackImg.sprite = Resources.Load<Sprite>("Textures/Poker/Bull/0");
        RectTransform rect = transform as RectTransform;
        Vector3 pos = target.anchoredPosition3D;
        Tweener tw1 = (transform as RectTransform).DOLocalMove(pos, duration);
        tw1.OnComplete(delegate {
            transform.SetParent(target);
            if (isShow)
            {
                ShowCard();
            }
        });
        (transform as RectTransform).DOScale(new Vector3(scale, scale, scale), duration);
    }

    #endregion
}
