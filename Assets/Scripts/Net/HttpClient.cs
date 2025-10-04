using RT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HttpClient : MonoBehaviour
{
    LoadMask mask;

    private string ipAdr = NetConfig.serverAdr;

    private void Awake()
    {
        mask = Instantiate(Resources.Load<LoadMask>("Prefabs/Widgets/LoadMask"));
        RectTransform t = (RectTransform)mask.gameObject.transform;
        t.SetParent(gameObject.transform);
        t.localScale = Vector3.one;
    }
    void Start()
    {

        //RectTransform ugui = GameObject.Find("_UGUI").GetComponent<RectTransform>();
        //
        //t.SetParent(ugui);
        //t.anchoredPosition3D = Vector3.zero;
        //t.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// HTTP GET方法
    /// </summary>
    /// <param name="url">请求路径，需要自己拼接参数</param>
    /// <param name="callback">回调方法（返回错误码和请求的Response数据）</param>
    public void GET(string url, Action<HttpResponse, string> callback = null, bool showMask = true)
    {
        StartCoroutine(IEGET(this.ipAdr + url, callback, showMask));
    }

    /// <summary>
    /// HTTP GET方法
    /// </summary>
    /// <param name="url">请求路径，需要自己拼接参数</param>
    /// <param name="callback">回调方法（返回错误码和请求的Response数据）</param>
    public void GET(string url, Dictionary<string, string> args, Action<HttpResponse, string> callback = null, bool showMask = true)
    {
        if (args != null && args.Count > 0)
        {
            var param = new StringBuilder(1024);
            foreach (KeyValuePair<string, string> pair in args)
            {
                if (pair.Value != null && pair.Value.Length > 0)
                {
                    param.AppendFormat("&{0}={1}", pair.Key, pair.Value.ToString());
                }
            }
            url += param;
        }

        StartCoroutine(IEGET(this.ipAdr + url, callback, showMask));
    }

    IEnumerator IEGET(string url, Action<HttpResponse, string> callback = null, bool showMask = true)
    {
        if (showMask)
        {
            mask.Show();
        }
        UnityWebRequest req = UnityWebRequest.Get(url);
        if (!string.IsNullOrEmpty(Game.Instance.CurPlayer.SessionId))
        {
            req.SetRequestHeader("Cookie", "PHPSESSID=" + Game.Instance.CurPlayer.SessionId);
        }

        yield return req.Send();
        if (showMask)
        {
            mask.Hide();
        }
        if (req.error != null)
        {
            Debug.Log("http error:" + req.error);
        }

        if (callback != null)
        {
            HttpResponse rsp;
            if (req.responseCode == 0)
            {
                rsp = new HttpResponse();
                rsp.Code = (int)req.responseCode;
            }
            else
            {
                rsp = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpResponse>(req.downloadHandler.text);
            }
            if (!SystemNotify.Instance.HandleSpecialCode(rsp.Code))
            {
                if (rsp.Code == 201 && !url.Contains("public.php"))
                {
                    Transfer.Instance[TransferKey.Kickout] = true;
                    Game.Instance.RemoveToken();
                    SceneManager.LoadScene("LoginScene");
                }
                else
                {
                    callback(rsp, req.error);
                }
            }
        }
        //req.Dispose();
    }

    /// <summary>
    /// HTTP POST方法--普通
    /// </summary>
    /// <param name="url">请求路径</param>
    /// <param name="args">请求参数</param>
    /// <param name="callback">回调方法（返回错误码和请求的Response数据）</param>
    public void POST(string url, Dictionary<string, string> args, Action<HttpResponse, string> callback = null, bool showMask = true)
    {
        StartCoroutine(IEPOST(this.ipAdr + url, args, "application/x-www-form-urlencoded", callback, showMask));
    }


    public void POSTFullUrl(string url, Dictionary<string, string> args, Action<HttpResponse, string> callback = null, bool showMask = true)
    {
        StartCoroutine(IEPOST(url, args, "application/x-www-form-urlencoded", callback, showMask));
    }

    /// <summary>
    /// HTTP POST方法--JSON
    /// </summary>
    /// <param name="url">请求路径</param>
    /// <param name="args">请求参数</param>
    /// <param name="callback">回调方法（返回错误码和请求的Response数据）</param>
    public void JPOST(string url, Dictionary<string, string> args, Action<HttpResponse, string> callback = null, bool showMask = true)
    {
        StartCoroutine(IEPOST(this.ipAdr + url, args, "application/json", callback, showMask));
    }

    IEnumerator IEPOST(string url, Dictionary<string, string> args, string contentType, Action<HttpResponse, string> callback = null, bool showMask = true)
    {
        WWWForm form = new WWWForm();
        if (args != null)
        {
            foreach (KeyValuePair<string, string> arg in args)
            {
                form.AddField(arg.Key, arg.Value);
            }
        }
        if (showMask)
        {
            mask.Show();
        }
        UnityWebRequest req = UnityWebRequest.Post(url, form);
        req.SetRequestHeader("Content-Type", contentType);
        if (Game.Instance.CurPlayer != null && !string.IsNullOrEmpty(Game.Instance.CurPlayer.SessionId))
        {
            req.SetRequestHeader("Cookie", "PHPSESSID=" + Game.Instance.CurPlayer.SessionId);
        }
        yield return req.Send();
        if (showMask)
        {
            mask.Hide();
        }
        if (req.error != null)
        {
            Debug.Log("http error:" + req.responseCode);
            Debug.Log("http error:" + req.error);
        }
        if (callback != null)
        {
            HttpResponse rsp;
            if (req.responseCode == 0)
            {
                rsp = new HttpResponse();
                rsp.Code = (int)req.responseCode;
            }
            else
            {
                rsp = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpResponse>(req.downloadHandler.text);
            }
            if (!SystemNotify.Instance.HandleSpecialCode(rsp.Code))
            {
                if (rsp.Code == 201 && !url.Contains("public.php"))
                {
                    Transfer.Instance[TransferKey.Kickout] = true;
                    Game.Instance.RemoveToken();
                    SceneManager.LoadScene("LoginScene");
                }
                else
                {
                    callback(rsp, req.error);
                }
            }
        }
        //req.Dispose();
    }

    public void Download(string url, Action<HttpResult<byte[]>> callback, bool showMask = true)
    {
        StartCoroutine(IEDownload(url, callback, showMask));
    }

    IEnumerator IEDownload(string url, Action<HttpResult<byte[]>> callback, bool showMask = true)
    {
        if (showMask)
        {
            mask.Show();
        }
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.Send();
        if (showMask)
        {
            mask.Hide();
        }
        if (req.error != null)
        {
            Debug.Log("http error:" + req.error);
        }

        if (callback != null)
        {
            HttpResult<byte[]> rsp = new HttpResult<byte[]>();
            rsp.code = (int)req.responseCode;
            rsp.data = req.downloadHandler.data;
            callback(rsp);
        }
        //req.Dispose();
    }
}
