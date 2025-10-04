using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RT
{
    public class VoiceButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public Image imgCancel;
        public RecorderShowView showView;

        private Vector2 _beginPos, _curPos;
        private bool _isCancel;

        private Queue<Action> _queue;

        private void Awake()
        {
            _beginPos = Vector2.zero;
            _curPos = Vector2.zero;
            _queue = new Queue<Action>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            showView.Show();
            VoiceRecorder.Instance.StartRecord();
            _beginPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _curPos = eventData.position;
            if(_curPos.y > _beginPos.y && (_curPos - _beginPos).sqrMagnitude > 4000)
            {
                _isCancel = true;
                showView.ShowCancel(true);
                imgCancel.gameObject.SetActive(true);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            showView.Hide();
            if (!_isCancel)
            {
                VoiceRecorder.Instance.StopRecord();
                // 上传到服务器
                byte[] audio = VoiceRecorder.Instance.Save();
                string path = string.Format("audio/{0}/{1}_{2}.wav", Game.Instance.Uid, Game.Instance.Uid, TimeUtil.DateToMills(DateTime.Now));
                AwsS3Service.Instance.AsyncUploadObject(audio, path, (rsp) =>
                 {
                     if (rsp.IsOk)
                     {
                         // 上传成功，播放录音
                         Dictionary<string, string> param = new Dictionary<string, string>();
                         param.Add("audio", rsp.data);
                         UserApi.SaveUserSound(param, (error) =>
                         {
                             if (error != null)
                             {
                                 Game.Instance.ShowTips(error);
                             }
                         });
                     }
                 });
            }
            else
            {
                imgCancel.gameObject.SetActive(false);
                VoiceRecorder.Instance.StopRecord();
            }
            _isCancel = false;
        }
    }
}
