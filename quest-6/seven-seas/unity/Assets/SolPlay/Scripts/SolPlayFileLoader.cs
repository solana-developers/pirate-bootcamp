using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ThreeDISevenZeroR.UnityGifDecoder;
using ThreeDISevenZeroR.UnityGifDecoder.Model;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SolPlay.Scripts
{
    /// <summary>
    /// Originally from AllArt, but with some changes to also be able to load gifs and reduce garbage and name textures
    /// to be easily found in profiler. I will put this back into the UnitySDK core soon.
    /// </summary>
    public static class SolPlayFileLoader
    {
        public static async UniTask<T> LoadFile<T>(string path, string optionalName = "")
        {
            if (typeof(T) == typeof(Texture2D))
            {
                if (path.Contains("gif"))
                {
                    return await LoadGif<T>(path);
                }

                return await LoadTexture<T>(path);
            }

#if UNITY_WEBGL
            return await LoadJsonWebRequest<T>(path);
#else
            return await LoadJson<T>(path);
#endif
        }

        private static async UniTask<T> LoadTexture<T>(string filePath, CancellationToken token = default)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath))
            {
                await uwr.SendWebRequest();

                while (!uwr.isDone && !token.IsCancellationRequested)
                {
                    await Task.Yield();
                }

                if (uwr.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(uwr.error);
                    return default;
                }

                var texture = DownloadHandlerTexture.GetContent(uwr);
                // Important here to destroy the texture in the web request because it will leak otherwise.
                Object.Destroy(((DownloadHandlerTexture) uwr.downloadHandler).texture);

                return (T) Convert.ChangeType(texture, typeof(T));
            }
        }

        public static Sprite LoadFromResources(string fileName)
        {
            return Resources.Load<Sprite>(fileName);
        }
        
        private static async UniTask<T> LoadGif<T>(string filePath, CancellationToken token = default)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(filePath))
            {
                uwr.SendWebRequest();

                while (!uwr.isDone && !token.IsCancellationRequested)
                {
                    await Task.Yield();
                }

                if (uwr.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(uwr.error);
                    return default;
                }

                Texture mainTexture = GetTextureFromGifByteStream(uwr.downloadHandler.data);
               
                var changeType = (T) Convert.ChangeType(mainTexture, typeof(T));
                return changeType;
            }
        }

        private static Texture2D GetTextureFromGifByteStream(byte[] bytes)
        {
            // Can use frame delayed for animated gifs, but i didnt to that for memory reasons 
            var frameDelays = new List<float>();

            using (var gifStream = new GifStream(bytes))
            {
                while (gifStream.HasMoreData)
                {
                    switch (gifStream.CurrentToken)
                    {
                        case GifStream.Token.Image:
                            GifImage image = gifStream.ReadImage();
                            var frame = new Texture2D(
                                gifStream.Header.width,
                                gifStream.Header.height,
                                TextureFormat.ARGB32, false);

                            frame.SetPixels32(image.colors);
                            frame.Apply();

                            frameDelays.Add(image.SafeDelaySeconds); // More about SafeDelay below

                            //var imageSize = ServiceFactory.Resolve<NftService>().NftImageSize;
                            //Texture2D resizedTexture = Resize(frame, imageSize, imageSize);

                            return frame;

                        case GifStream.Token.Comment:
                            var commentText = gifStream.ReadComment();
                            Debug.Log(commentText);
                            break;

                        default:
                            gifStream.SkipToken(); // Other tokens
                            break;
                    }
                }
            }

            return null;
        }

        static Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            rt.name = "render texture";
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            result.name = "resize result2d";
            return result;
        }

        private static async UniTask<T> LoadJsonWebRequest<T>(string path)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(path))
            {
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SendWebRequest();

                while (!uwr.isDone)
                {
                    await Task.Yield();
                }

                if (uwr.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(uwr.error);
                    return default(T);
                }

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

        private static async UniTask<T> LoadJson<T>(string path)
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
                return (T) Convert.ChangeType(texture, typeof(T));
            }

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

        public static void SaveToPersistenDataPath<T>(string path, T data)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                byte[] dataToByte = ((Texture2D) Convert.ChangeType(data, typeof(Texture2D))).EncodeToPNG();
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