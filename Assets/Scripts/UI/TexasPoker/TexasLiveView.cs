using RT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TexasLiveView : MonoBehaviour, IPointerClickHandler
{
    public Text ViewTitleTxt;
    public Text NickTitleTxt;
    public Text BuyinTitleTxt;
    public Text ProfitTitleTxt;
    public ListView LiveListView;
    
    public Text WatchTitleTxt;
    public HListView WatcherListView;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != gameObject)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            Close();
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    void Start () {
		
	}
	
	void Update () {

    }

    public void InitTexasView(List<GameLiveData> datas,Dictionary<int,TexasPlayer> upList)
    {
        if (datas != null)
        {
            LiveListView.Clear();
            for (int i = 0; i < datas.Count; i++)
            {
                LiveListView.Add(datas[i]);
            }
        }
        WatcherListView.Clear();
        if (upList != null)
        {
            WatchTitleTxt.text = string.Format(LocalizationManager.Instance.GetText("7010"), upList.Count);// "看客 （" + upList.Count + "）";
            foreach (int k in upList.Keys)
            {
                WatcherData wd = new WatcherData();
                wd.Nickname = upList[k].Nickname;
                wd.Avatar = upList[k].Avatar;
                WatcherListView.Add(wd);
            }
        }
        else
        {
            WatchTitleTxt.text = string.Format(LocalizationManager.Instance.GetText("7010"), 0);// "看客 （0）";
        }
    }

    public void InitBullView(List<GameLiveData> datas, Dictionary<int, BullPlayer> upList)
    {
        if (datas != null)
        {
            LiveListView.Clear();
            for (int i = 0; i < datas.Count; i++)
            {
                LiveListView.Add(datas[i]);
            }
        }
        WatcherListView.Clear();
        if (upList != null)
        {
            WatchTitleTxt.text = string.Format(LocalizationManager.Instance.GetText("7010"), upList.Count);// "看客 （" + upList.Count + "）";
            foreach (int k in upList.Keys)
            {
                WatcherData wd = new WatcherData();
                wd.Nickname = upList[k].Nickname;
                wd.Avatar = upList[k].Avatar;
                WatcherListView.Add(wd);
            }
        }
        else
        {
            WatchTitleTxt.text = string.Format(LocalizationManager.Instance.GetText("7010"), 0);// "看客 （0）";
        }
    }
}
