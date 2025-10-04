using RT;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PublicView : MonoBehaviour {
    public Button TexasBtn;
    public Button BullBtn;
	void Start () {
        TexasBtn.onClick.AddListener(delegate {
            Transfer.Instance[TransferKey.Game] = GameType.dz;
            SceneManager.LoadScene("PublicTableScene");
        });

        BullBtn.onClick.AddListener(delegate {
            Transfer.Instance[TransferKey.Game] = GameType.bull;
            SceneManager.LoadScene("PublicTableScene");
        });

        TexasBtn.gameObject.SetActive(Game.Instance.CurPlayer.DzGameEnable == 1);
        BullBtn.gameObject.SetActive(Game.Instance.CurPlayer.CowGameEnable == 1);
    }
	void Update () {
		
	}
}
