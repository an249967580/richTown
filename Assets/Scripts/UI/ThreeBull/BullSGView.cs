using RT;
using UnityEngine;
using UnityEngine.UI;

public class BullSGView : MonoBehaviour {

    public RubbingView SGRubbingView;
    public Button OpenBtn;
	void Start () {
		
	}
    public void SetRubView(int card) {
        SGRubbingView.SetCard(card);
        SGRubbingView.Show();
    }
}
