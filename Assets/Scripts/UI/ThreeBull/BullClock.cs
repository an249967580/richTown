using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BullClock : MonoBehaviour {

    public Text ClockTxt;
    int Ts;
    bool isClockRing = false;
    AudioSource audioSource;

    void Start () {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = Game.Instance.AudioMgr.GetSoundClip("ntimeout");
    }
	
	void Update () {
    }

    public void Show(int ts) {
        gameObject.SetActive(true);
        Ts = ts;
        ClockTxt.text = Ts.ToString();
        StartCoroutine(Timer());
    }

    public void Close() {
        Ts = 0;
        ClockTxt.text = "0";
        isClockRing = false;
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        gameObject.SetActive(false);
    }

    IEnumerator Timer()
    {
        while (Ts > 0)
        {
            yield return new WaitForSeconds(1);
            Ts--;
            ClockTxt.text = Ts.ToString();
            if (Ts <= 5 && isClockRing == false)
            {
                isClockRing = true;
                if (Game.Instance.VoiceOn == 1)
                {
                    audioSource.Play();
                }
            }
        }
        if (Ts <= 0) {
            Close();
        }
    }
}
