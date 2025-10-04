using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RT
{
    public delegate void SureEvent();
    public class SureView : HideMonoBehaviour
    {


        public Text tvMsg;
        public Button btnSure;

        public SureEvent OnSureEvent;
        private string msg, countDownMsg = "秒后自动退出";
        private bool _isAuto;

        private void Awake()
        {
            if(_isAuto)
            {
                StopAllCoroutines();
            }

            btnSure.onClick.AddListener(() =>
            {
                HideAndDestory();
                if (OnSureEvent != null)
                {
                    OnSureEvent();
                }
            });
        }

        public SureView ShowTip(string msg)
        {
            this.msg = msg;
            tvMsg.text = msg;
            return this;
        }

        public void AutoHide(float time, string countDownMsg)
        {
            _isAuto = true;
            this.countDownMsg = countDownMsg;
            StartCoroutine(countDown(time));
        }

        IEnumerator countDown(float time)
        {
            while(time > 0)
            {
                tvMsg.text = string.Format("{0}，<color=#ff0000>{1}</color>{2}", msg,time,countDownMsg);
                time--;
                yield return new WaitForSeconds(1);
            }
            HideAndDestory();
            if (OnSureEvent != null)
            {
                OnSureEvent();
            }
        }
    }

}