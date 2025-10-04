using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace RT
{
    public delegate void ReconnectEvent(DisconnectTip tip);

    public delegate void ReconnectTimeout();

    public class DisconnectTip : HideMonoBehaviour
    {
        public Button btnCancel;
        public Image imgLoad;
        public ReconnectEvent OnReconnectEvent;
        public ReconnectTimeout OnReconnectTimeout;
        private float _timeout = 180;     // 重连超时时间
        private float _tick = 0;          // 累计超时

        private void Awake()
        {
            btnCancel.onClick.AddListener(()=>
            {
                Hide();
                Screen.orientation = ScreenOrientation.Portrait;
                Game.Instance.RemoveToken();
                PlayerPrefs.SetString("LastScene", "");
                SceneManager.LoadScene("LoginScene");
            });

            StartCoroutine(countDown());
        }


        IEnumerator countDown()
        {
            while (_tick < _timeout)
            {
                if(_tick % 10 == 0)     // 10秒连一次
                {
                    if(OnReconnectEvent != null)
                    {
                        OnReconnectEvent(this);
                    }
                }
                _tick++;
                yield return new WaitForSeconds(1.0f);
            }
            yield return 0;
            if(OnReconnectTimeout != null)
            {
                // 断线重连次数太多，视为网络连接错误，执行timeout
                Debug.Log("timeout");
                StopAllCoroutines();
                OnReconnectTimeout();
                HideAndDestory();
            }
        }

        private void Update()
        {
            imgLoad.transform.Rotate(new Vector3(0, 0, -imgLoad.transform.position.z), 3f);
        }
    }
}
