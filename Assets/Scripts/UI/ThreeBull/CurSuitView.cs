using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurSuitView : MonoBehaviour {
    private PokerItem pokerTpl;
    AudioSource audioSource;

    public Image NiuResultImg;
    public RectTransform SuitArea1;
    public RectTransform SuitArea2;

    public List<PokerItem> Pokers;
    void Start () {

        pokerTpl = Resources.Load<PokerItem>("Prefabs/Poker/PokerItem");
        Pokers = new List<PokerItem>();
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayAudioEffect(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            if (Game.Instance.VoiceOn == 1)
            {
                audioSource.Play();
            }
        }
    }
    #region 分发牌组

    public void DealCard(int card, float duration)
    {
        StartCoroutine(IEDealCard(card,duration));
    }
    public IEnumerator IEDealCard(int card,float duration = 0.1f)
    {
        var go = Instantiate(pokerTpl);
        go.transform.SetParent(NiuResultImg.transform);
        (go.transform as RectTransform).anchoredPosition3D = Vector3.zero;
        (go.transform as RectTransform).localScale = new Vector3(1.1f, 1.1f, 1);

        PokerItem p = go.GetComponent<PokerItem>();

        if (Pokers.Count > 2 && Pokers.Count < 5)
        {
            if (card != 0)
            {
                p.RenderView("" + card.ToString());
                p.MyRotation.DealCard2Target(SuitArea2, true, 1.1f);
            }
            else
            {
                p.MyRotation.DealCard2Target(SuitArea2, false, 1.1f);
            }
        }
        else {

            if (card != 0)
            {
                p.RenderView("" + card.ToString());
                p.MyRotation.DealCard2Target(SuitArea1, true, 1.1f);
            }
            else
            {
                p.MyRotation.DealCard2Target(SuitArea1, false, 1.1f);
            }
        }

        Pokers.Add(p);
        yield return new WaitForSeconds(duration);
    }
    /// <summary>
    /// 显示第n张牌
    /// </summary>
    /// <param name="card"></param>
    /// <param name="index"></param>
    public void ShowPokerInSuit(int card,int index)
    {
        if (Pokers != null && Pokers.Count > index)
        {
            Pokers[index].RenderView("" + card.ToString());
            Pokers[index].MyRotation.ShowCard();
        }
    }
    public void ShowSuit(int total, int[] cards)
    {
        Clear();
        for (int i = 0; i < total; i++)
        {
            if (i < cards.Length)
            {
                DealCard(cards[i], i > 2 ? 0.3f : 0.1f);
            }
            else
            {
                DealCard(0, i > 2 ? 0.3f : 0.1f);
            }
        }
    }
    #endregion

    #region 显示结果
    public void ShowSGResultImage(string cardType, int point)
    {
        int type = GetSGCardType(cardType, point);

        NiuResultImg.sprite = Resources.Load<Sprite>("Textures/BullAnim/sg" + type);
        NiuResultImg.SetNativeSize();
        NiuResultImg.gameObject.SetActive(true);

        NiuResultImg.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.1f);

        NiuResultImg.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.1f).SetDelay(2).OnComplete(delegate {
            NiuResultImg.sprite = Resources.Load<Sprite>("Textures/BullAnim/bull0");
            NiuResultImg.SetNativeSize();
            NiuResultImg.gameObject.SetActive(false);
        }); ;
    }
    int GetSGCardType(string cardType, int point)
    {
        int type = 0;
        switch (cardType)
        {
            case "THREE_CARD":
                type = 12;
                break;
            case "JQK_CARD":
                type = 11;
                break;
            case "TEN_CARD":
                type = 10;
                break;
            case "NINE_CARD":
                type = 9;
                break;
            default:
                type = point;
                break;
        }

        return type;
    }

    public void ShowNNResultImage(string cardType, int point)
    {
        if (!NiuResultImg.gameObject.activeSelf)
        {
            int type = GetNNCardType(cardType, point);

            PlayAudioEffect(Game.Instance.AudioMgr.GetSoundClip("N" + type + "_0"));

            NiuResultImg.sprite = Resources.Load<Sprite>("Textures/BullAnim/bull" + type);
            NiuResultImg.SetNativeSize();
            NiuResultImg.gameObject.SetActive(true);

            if (type > 9)
            {
                NiuResultImg.transform.DOScale(new Vector3(0.55f, 0.55f, 0.55f), 0.1f);
                NiuResultImg.transform.DOLocalJump(NiuResultImg.transform.localPosition, 15, 3, 0.3f);
            }
            else
            {
                NiuResultImg.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.1f);
            }
        }
    }
    int GetNNCardType(string cardType, int point)
    {
        int type = 0;
        switch (cardType)
        {
            case "FIVE_CARD":
                type = 14;
                break;
            case "COW_NDG_CARD":
                type = 13;
                break;
            case "COW_BABY_TOW_CARD":
                type = 12;
                break;
            case "COW_BABY_CARD":
                type = 11;
                break;
            case "COWCOW_CARD":
                type = 10;
                break;
            case "SINGLE_CARD":
                type = 0;
                break;
            default:
                type = point;
                break;
        }

        return type;
    }
    #endregion
    public void Clear()
    {
        for (int i = 0; i < Pokers.Count; i++)
        {
            PokerItem p = Pokers[i];
            Destroy(p.gameObject);
        }
        Pokers.Clear();
        NiuResultImg.sprite = Resources.Load<Sprite>("Textures/BullAnim/bull0");
        NiuResultImg.SetNativeSize();
        NiuResultImg.gameObject.SetActive(false);
        if (audioSource != null)
        {
            audioSource.clip = null;
        }
    }
}
