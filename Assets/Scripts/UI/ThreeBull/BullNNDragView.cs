using RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum DragStatus {
    WaitOpen,
    WaitDrag
}

public class BullNNDragView : MonoBehaviour {

    public Button AutoPutBtn;
    public Button CommitBtn;
    public Button OpenBtn;
    public BullDragItem[] DragItems;
    public RectTransform[] BullDragWrappers;

    public RectTransform OpenPanel;
    public RectTransform DragPanel;

    public RubbingView RubbingView1;
    public RubbingView RubbingView2;

    public CurSuitView PlayerSuitView;
    public event Action onDragDoneEvent;
    public event Action onAutoDoneEvent;

    int Ts = 60;
    int[] myCards;
    DragStatus status;

    bool open3 = false;
    bool open4 = false;
    bool close = false;
    BullSuitTypeUtil util;

    void Start () {
        util = new BullSuitTypeUtil();
        OpenBtn.onClick.AddListener(delegate() {
            OpenClick();
        });
        AutoPutBtn.onClick.AddListener(delegate () {
            DoAutoPut();
        });
        for (int i = 0; i < DragItems.Length; i++)
        {
            BullDragItem item = DragItems[i];
            item.onDragDoneEvent += Item_onDragDoneEvent;
        }
        RubbingView1.OnRubbingEndEvent += (view) => {
            DoOpen3();
        };
        RubbingView2.OnRubbingEndEvent += (view) => {
            DoOpen4();
        };
    }

    private void Item_onDragDoneEvent(BullDragItem obj)
    {
        if (obj.Card != 0 && onDragDoneEvent!=null) {
            onDragDoneEvent();
        }
    }

    public void SetCards(int[] cards)
    {
        if (cards.Length == 5)
        {
            AutoPutBtn.gameObject.SetActive(false);
            CommitBtn.gameObject.SetActive(false);
            OpenBtn.gameObject.SetActive(true);
            Ts = 60;
            close = false;
            status = DragStatus.WaitOpen;
            myCards = cards;
            gameObject.SetActive(true);
            DragPanel.gameObject.SetActive(false);

            OpenPanel.gameObject.SetActive(true);
            RubbingView2.gameObject.SetActive(true);
            RubbingView2.SetCard(myCards[4]);
            RubbingView1.gameObject.SetActive(true);
            RubbingView1.SetCard(myCards[3]);

            for (int i = 0; i < DragItems.Length; i++)
            {
                BullDragItem item = DragItems[i];
                item.SetCardType(cards[i]);
            }
            StartCoroutine(Timer());

        }
    }

    public void SetPutCards(int[] cards,int ts)
    {
        if (cards.Length == 5)
        {
            AutoPutBtn.gameObject.SetActive(true);
            CommitBtn.gameObject.SetActive(true);
            OpenBtn.gameObject.SetActive(false);
            Ts = ts;
            status = DragStatus.WaitDrag;
            myCards = cards;
            open3 = false;
            open4 = false;
            close = false;

            gameObject.SetActive(true);
            DragPanel.gameObject.SetActive(true);

            OpenPanel.gameObject.SetActive(false);
            RubbingView2.gameObject.SetActive(false);
            RubbingView2.SetCard(myCards[4]);
            RubbingView1.gameObject.SetActive(false);
            RubbingView1.SetCard(myCards[3]);

            for (int i = 0; i < DragItems.Length; i++)
            {
                BullDragItem item = DragItems[i];
                item.SetCardType(cards[i]);
            }
            
            StartCoroutine(Timer());
            
        }
    }

    void OpenClick()
    {
        DoOpen3();
        DoOpen4();
    }

    public void Close() {
        Ts = 0;
        close = true;
        gameObject.SetActive(false);
        DragPanel.gameObject.SetActive(false);
        OpenPanel.gameObject.SetActive(false);
        for (int i = 0; i < BullDragWrappers.Length; i++) {
            (DragItems[i].transform as RectTransform).anchoredPosition3D = Vector3.zero;
            DragItems[i].transform.SetParent(BullDragWrappers[i].transform);
        }
    }

    public int[] GetFinalSuit() {
        List<int> list = new List<int>();
        foreach (RectTransform rect in BullDragWrappers) {
            BullDragItem item = rect.GetComponentInChildren<BullDragItem>();
            list.Add(item.Card);
        }
        
        return list.ToArray();
    }

    void DoOpen3()
    {
        Game.Instance.AudioMgr.PlayAudioEffect("fanpai");
        PlayerSuitView.ShowPokerInSuit(myCards[3], 3);
        open3 = true;
        if (open3 && open4) {
            DoDrag();
        }
    }
    void DoOpen4()
    {
        Game.Instance.AudioMgr.PlayAudioEffect("fanpai");
        PlayerSuitView.ShowPokerInSuit(myCards[4], 4);
        open4 = true;
        if (open3 && open4)
        {
            DoDrag();
        }
    }
    void DoDrag() {
        open3 = false;
        open4 = false;
        RubbingView2.Hide();
        RubbingView1.Hide();
        DragPanel.gameObject.SetActive(true);
        OpenPanel.gameObject.SetActive(false);
        AutoPutBtn.gameObject.SetActive(true);
        CommitBtn.gameObject.SetActive(true);
        OpenBtn.gameObject.SetActive(false);
    }
    void DoAutoPut() {
        int[] newcards = util.GetBestSuit(myCards);
        for (int i = 0; i < BullDragWrappers.Length; i++)
        {
            RectTransform rect = BullDragWrappers[i];
            BullDragItem item = rect.GetComponentInChildren<BullDragItem>();
            item.SetCardType(newcards[i]);
        }
        if (onAutoDoneEvent != null) {
            onAutoDoneEvent();
        }
    }

    IEnumerator Timer()
    {
        while (Ts > 0)
        {
            yield return new WaitForSeconds(1);
            Ts--;
            if (Ts <= 0)
            {
                if (!close)
                {
                    Close();
                }
            }
            else if (Ts <= 30 && status == DragStatus.WaitOpen)
            {
                status = DragStatus.WaitDrag;
                DoOpen3();
                DoOpen4();
            }
        }
    }
}
