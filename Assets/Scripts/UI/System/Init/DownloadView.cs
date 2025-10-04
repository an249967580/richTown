using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RT
{

    public delegate void DownLoadEvent(string filePath);

    public class DownloadView : HideMonoBehaviour
    {
        public Slider sProgress;
        public Text tvProgress;

        public DownLoadEvent OnDownLoadEvent;

        private void Awake()
        {
        }

        public void DownLoad(string url, string fileName)
        {
            StartCoroutine(IEDownload(url, fileName));
        }



        IEnumerator IEDownload(string url, string fileName)
        {
            UnityWebRequest req = UnityWebRequest.Get(url);
            req.Send();
            while(req.downloadProgress < 1)
            {
                sProgress.value = req.downloadProgress;
                tvProgress.text = string.Format("{0}%", (int)(req.downloadProgress * 100));
                yield return 0;
            }

            sProgress.value = 1.0f;
            tvProgress.text = string.Format("{0}%", 100);

            yield return new WaitForSeconds(1.0f);

            saveApk(fileName, req.downloadHandler.data);

        }

        void saveApk(string fileName, byte[] bytes)
        {
            string filePath = string.Format("{0}/{1}/{2}.{3}", Application.persistentDataPath, "Update", fileName, "apk");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllBytes(filePath, bytes);

            if(OnDownLoadEvent != null)
            {
                OnDownLoadEvent(filePath);
            }
        }
    }
}
