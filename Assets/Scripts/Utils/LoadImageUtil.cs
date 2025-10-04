using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace RT
{
    public class LoadImageUtil
    {
        private static string path = Application.persistentDataPath + "/ImageCache";
        public static IEnumerator LoadImage(string url, Action<Sprite> action)
        {
            string name = url.Substring(url.LastIndexOf('/'));
            string filePath = "";

            //创建图片缓存文件夹
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            path = path + "/";
            if (!string.IsNullOrEmpty(name) && File.Exists(path + name.GetHashCode()))
            {
                //filePath = "file:///" + path + name.GetHashCode();
                filePath = path + name.GetHashCode();
                var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                fs.Seek(0, SeekOrigin.Begin);
                var binary = new byte[fs.Length];
                fs.Read(binary, 0, binary.Length);
                fs.Close();

                int width = 400;
                int height = 400;
                Texture2D tex2d = new Texture2D(width, height);
                tex2d.LoadImage(binary);
                if (tex2d)
                {
                    Debug.Log("------got image");
                    Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.zero);
                    if (sprite && action != null)
                    {
                        action(sprite);
                    }
                }
                else
                {
                    Debug.Log("------do not got image");
                }
            }
            else
            {
                filePath = url;
                WWW www = new WWW(filePath);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    Texture2D tex2d = www.texture;
                    if (tex2d)
                    {
                        byte[] imgData = tex2d.EncodeToJPG();
                        File.WriteAllBytes(path + name.GetHashCode(), imgData);
                        Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.zero);
                        if (sprite && action != null)
                        {
                            action(sprite);
                        }
                    }
                }
            }
        }

        public static IEnumerator LoadImage(string url, Action<Sprite, bool> action)
        {
            WWW www = new WWW(url);
            yield return www;
            Texture2D tex2d = www.texture;
            if (tex2d)
            {
                Sprite sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), Vector2.zero);
                if (sprite && action != null)
                {
                    action(sprite, true);
                }
                else
                {
                    if (action != null)
                    {
                        action(null, false);
                    }
                }
            }
            else
            {
                if (action != null)
                {
                    action(null, false);
                }
            }
        }

        public static void LoadByte(byte[] bytes,Action<Sprite> action)
        {
            Texture2D texture = new Texture2D(200, 200);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            if (sprite && action != null)
            {
                action(sprite);
            }
        }
    }
}
