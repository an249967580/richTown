using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RT;

public class BullTest : MonoBehaviour {

    public InputField Field;
    public Text ResultTxt;
    public Button Btn;
    
    void Start () {
        Btn.onClick.AddListener(delegate {
            Test();
        });
    }

    void Test() {
        string[] strs = Field.text.Split('-');
        if (strs.Length > 0) {
            int[] arr = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                arr[i] = int.Parse(strs[i]);
            }
            PokerUtilCardType rs = BullSuitTypeUtil.Instance.GetBestSuitType(arr);
            ResultTxt.text = "最佳牌型：" + rs.type + "--" + rs.typePoint;
        }

    }
}
