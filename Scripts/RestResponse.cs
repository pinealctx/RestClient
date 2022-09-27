using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Pinealctx.RestClient
{
    public class RestResponse : IDisposable
    {
        public RestResponse(UnityWebRequest request, RestRequest restRequest)
        {
            Request = request;
            RestRequest = restRequest;
            Success = Request.result == UnityWebRequest.Result.Success;
        }

        public UnityWebRequest Request { get; }

        public RestRequest RestRequest { get; }

        public bool Success { get; }

        public T GetFromJson<T>()
        {
            var json = JsonConvert.DeserializeObject<T>(Request.downloadHandler.text);
            Dispose();
            return json;
        }

        public Texture2D GetTexture()
        {
            var texture = DownloadHandlerTexture.GetContent(Request);
            Dispose();
            return texture;
        }

        public Texture2D GetTextureV2()
        {
            var texture = ((DownloadHandlerTexture)Request.downloadHandler).texture;
            Dispose();
            return texture;
        }

        public void Dispose()
        {
            Request.Dispose();
            RestRequest.Dispose();
        }
    }
}