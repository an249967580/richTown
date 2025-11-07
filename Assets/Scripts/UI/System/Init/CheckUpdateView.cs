using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace RT
{
    public class CheckUpdateView : HideMonoBehaviour
    {
        public UpdateConfirmView confirmView;
        public DownloadView downLoadView;
        public SureView sureView;
        public Image imgLoading;

        private int channel = -1;
        private bool _loadCountries, _getVersion;

        private AndroidJavaObject _jo;

        private void Awake()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            Game.Instance.SetTips();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
			_getVersion = true;
#elif UNITY_ANDROID
            channel = 1;
            androidSplash();
#elif UNITY_IOS
            _getVersion = true;
#endif
            _loadCountries = false;
            _getVersion = false;

            confirmView.OnUpdateEvent = onUpdateEvent;
            sureView.OnSureEvent = () =>
            {
                Application.Quit();
            };
        }

        // android 启动页修改
        void androidSplash()
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            // if (jo != null)
            // {
            //     string activityName = jo.Call<string>("getLocalClassName");
            //     string packageName = jo.Call<string>("getPackageName");
            //     Debug.Log($"Current Activity = {packageName}.{activityName}");
            // }
        }

        private void Start()
        {
            if (_jo != null)
            {
                _jo.Call("HideSplash");
            }
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                sureView.Show();
            }
            else
            {
                findCountries();
                checkVersion();
            }
        }

        void findCountries()
        {
            // 获取国家
            SystemApi.FindCountry((rsp) =>
            {
                if (rsp.IsOk && Validate.IsNotEmpty(rsp.data))
                {
                    _loadCountries = true;
                    // 保存文件
                    PlayerPrefs.SetString("countries", JsonConvert.SerializeObject(rsp.data));
                }
                else
                {
                    if (rsp.code == 0)
                    {
                        sureView.Show();
                    }
                    else
                    {
                        _loadCountries = true;
                        Game.Instance.ShowTips(rsp.errorMsg);
                    }
                }
            });
        }

        void checkVersion()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            // getVersion( Game.Instance.AndroidBuildVersionCode);
            _getVersion = true;
#elif UNITY_ANDROID
            // _getVersion = true;
            getVersion( Game.Instance.AndroidBuildVersionCode);
#elif UNITY_IOS
            _getVersion = true;
#endif
        }

        // IEnumerator RequestVersion(string url, int versionCode)
        // {
        //     using (UnityWebRequest req = UnityWebRequest.Get(url))
        //     {
        //         req.timeout = 10;
        //         yield return req.SendWebRequest();

        //         if (req.result != UnityWebRequest.Result.Success)
        //         {
        //             _getVersion = true;
        //             Game.Instance.ShowTips("获取版本信息失败: " + req.error);
        //             yield break;
        //         }

        //         try
        //         {
        //             string json = req.downloadHandler.text;
        //             var data = JsonUtility.FromJson<VersionData>(json);

        //             if (data != null && data.versionCode > Game.Instance.AndroidBuildVersionCode)
        //             {
        //                 confirmView.lastVersion = data;
        //                 confirmView.Show();
        //             }
        //             else
        //             {
        //                 _getVersion = true;
        //             }
        //         }
        //         catch (System.Exception e)
        //         {
        //             _getVersion = true;
        //             Game.Instance.ShowTips("解析版本信息失败: " + e.Message);
        //         }
        //     }
        // }

        void getVersion(int versionCode)
        {
            string url = "https://lm6789.com/richtown_version.json";

            UnityWebRequest req = UnityWebRequest.Get(url);
            req.timeout = 10;

            req.SendWebRequest().completed += (asyncOp) =>
            {
                if (req.result != UnityWebRequest.Result.Success)
                {
                    _getVersion = true;
                    Game.Instance.ShowTips("获取版本信息失败: " + req.error);
                    return;
                }

                try
                {
                    string json = req.downloadHandler.text;

                    // ✅ 直接用 RT.Version（不要再定义 VersionData）
                    RT.Version data = JsonUtility.FromJson<RT.Version>(json);

                    if (data != null && data.versionCode > Game.Instance.AndroidBuildVersionCode)
                    {
                        confirmView.lastVersion = data;
                        confirmView.Show();
                    }
                    else
                    {
                        _getVersion = true;
                    }
                }
                catch (Exception e)
                {
                    _getVersion = true;
                    Game.Instance.ShowTips("解析版本信息失败: " + e.Message);
                }
            };
            // SystemApi.GetLastVersion(channel, versionCode, (rsp) =>
            //  {
            //      if (rsp.IsOk)
            //      {
            //          if (rsp.data != null && rsp.data.versionCode > Game.Instance.AndroidBuildVersionCode)
            //          {
            //              confirmView.lastVersion = rsp.data;
            //              confirmView.Show();
            //          }
            //          else
            //          {
            //              _getVersion = true;
            //          }
            //      }
            //      else
            //      {
            //          if (rsp.code == 0)
            //          {
            //              sureView.Show();
            //          }
            //          else
            //          {
            //              _getVersion = true;
            //              Game.Instance.ShowTips(rsp.errorMsg);
            //          }
            //      }
            //  });
        }

        void onUpdateEvent(UpdateEventOp op)
        {
            switch (op)
            {
                case UpdateEventOp.Update:
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                    Debug.Log("pc 端不支持");
                    _getVersion = true;
#elif UNITY_ANDROID
                    downLoad(confirmView.lastVersion.downloadUrl, confirmView.lastVersion.versionName);
#elif UNITY_IOS
_getVersion = true;
			// if(!string.IsNullOrEmpty(confirmView.lastVersion.downloadUrl)){
			// 	Application.OpenURL(confirmView.lastVersion.downloadUrl);
			// }
			// else{
			// 	Application.OpenURL("itms-apps://itunes.apple.com/us/app/richtown/id1266119729");
			// }
#endif
                    break;
                case UpdateEventOp.Cancel:
                    if (confirmView.lastVersion.forceUpdate > 0)
                    {
                        Application.Quit();
                    }
                    else
                    {
                        _getVersion = true;
                    }
                    break;
            }
        }

        void downLoad(string url, string fileName)
        {
            confirmView.Hide();
            downLoadView.Show();
            downLoadView.DownLoad(url, string.Format("{0}.pak", fileName));
            downLoadView.OnDownLoadEvent = (filePath) =>
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                jo.Call("Install", filePath);
                // downLoadView.Hide();
                // Application.Quit();
            };
        }

        private void Update()
        {
            imgLoading.transform.Rotate(new Vector3(0, 0, -imgLoading.transform.position.z), 3f);
            if (_loadCountries && _getVersion)
            {
                SceneManager.LoadScene("LoginScene");
            }
        }

        public void OnInstallResult(string resultCode)
        {
            Debug.Log($"[InstallResult] resultCode={resultCode}");

            // Android 安装界面关闭（不论用户取消或完成）都会触发这个
            // resultCode 一般为 -1（RESULT_OK） 或 0（RESULT_CANCELED）

            if (resultCode == "0")
            {
                // 用户取消安装
                downLoadView.Hide();
                confirmView.Show();
                Game.Instance.ShowTips("安装已取消");
                _getVersion = true;
            }
            else
            {
                // 安装成功（或至少用户完成操作）
                downLoadView.Hide();
                Game.Instance.ShowTips("安装完成");
                _getVersion = true;
            }
        }
    }

    // [System.Serializable]
    // public class VersionData
    // {
    //     public int versionCode;
    //     public string updateDesc;
    //     public string downloadUrl;
    // }
}
