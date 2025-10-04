using System.Collections;
using UnityEngine;

public class WaitingChip : MonoBehaviour {
    float deg = 0;
    void Start ()
    {
        StartCoroutine(Timer());
    }
	
	void Update () {
    }
    public IEnumerator Timer()
    {
        while (true)
        {
            deg = (deg - 60) % 360;
            Vector3 rot = transform.localEulerAngles;
            rot.z = deg;
            transform.localEulerAngles = rot;
            
            yield return new WaitForSeconds(0.2f);
        }
    }
}
