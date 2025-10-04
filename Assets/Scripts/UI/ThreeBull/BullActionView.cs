using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BullActionView : MonoBehaviour {

    public Button NoBankBtn;
    public List<Button> BankBtnList;

    public int[] BankTimes = { 1,2,3};
    void Start () {
        NoBankBtn.onClick.AddListener(delegate() {
            Close();
        });
        for(int i=0;i< BankBtnList.Count;i++) {
            Button btn = BankBtnList[i];
            Text txt = btn.GetComponentInChildren<Text>();
            txt.text = "×" + BankTimes[i];
        }
    }
    public void Close() {
        gameObject.SetActive(false);
    }
}
