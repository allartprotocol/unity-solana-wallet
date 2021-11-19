using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AllArt.Solana.Utility
{
    public static class FileLoader
    {
        public static async Task<T> LoadFile<T>(string path, string optionalName = "")
        {

            if (typeof(T) == typeof(Texture2D))
            {
                return await LoadTexture<T>(path);
            }
            else
            {
                return await LoadJson<T>(path);
            }
        }

        private static async Task<T> LoadTexture<T>(string filePath, CancellationToken token = default)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
            {
                uwr.SendWebRequest();

                while (!uwr.isDone && !token.IsCancellationRequested)
                {
                    await Task.Yield();
                }

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                    return default;
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    return (T)Convert.ChangeType(texture, typeof(T));
                }
            }
        }

        private static async Task<T> LoadJsonWebRequest<T>(string path)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(path))
            {
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SendWebRequest();

                while (!uwr.isDone)
                {
                    await Task.Yield();
                }

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                    return default(T);
                }
                else
                {
                    string json = uwr.downloadHandler.text;
                    Debug.Log(json);
                    try
                    {
                        T data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                        return data;
                    }
                    catch
                    {
                        return default;
                    }

                }
            }
        }

        private static async Task<T> LoadJson<T>(string path)
        {            
            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();

            try
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                T data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseBody);
                client.Dispose();
                return data;
            }
            catch
            {
                client.Dispose();
                return default;
                throw;
            }
        }

        public static T LoadFileFromLocalPath<T>(string path)
        {
            T data;

            if (!File.Exists(path))
                return default;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            if (typeof(T) == typeof(Texture2D))
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                return (T)Convert.ChangeType(texture, typeof(T));
            }
            else
            {
                string contents = File.ReadAllText(path);
                try
                {
                    data = JsonUtility.FromJson<T>(contents);
                    return data;
                }
                catch
                {
                    return default;
                }
            }
        }

        public static void SaveToPersistenDataPath<T>(string path, T data)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                byte[] dataToByte = ((Texture2D)Convert.ChangeType(data, typeof(Texture2D))).EncodeToPNG();
                File.WriteAllBytes(path, dataToByte);
            }
            else
            {
                string dataString = JsonUtility.ToJson(data);
                File.WriteAllText(path, dataString);
            }
        }

    }

}
