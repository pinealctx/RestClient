using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PinealCtx.RestClient.Param;
using UnityEngine;
using UnityEngine.Networking;

namespace PinealCtx.RestClient
{
    public class RestClient
    {
        public static RestClient DefaultRestClient = new();

        public RestClientOptions Options { get; }

        public RestClient(RestClientOptions options)
        {
            Options = options;
        }

        public RestClient() : this(new RestClientOptions())
        {
        }

        public RestClient(Uri baseUrl) : this(new RestClientOptions { BaseUrl = baseUrl })
        {
        }

        public RestClient(string baseUrl) : this(new Uri(baseUrl))
        {
        }

        public IEnumerator Do(RestRequest request, Action<RestResponse> completedAction)
        {
            Options.BeforeHooks?.ForEach(hook => hook(request));
            using var webRequest = BuildUnityWebRequest(request);
            yield return webRequest.SendWebRequest();
            var response = new RestResponse(webRequest, request);
            Options.AfterHooks?.ForEach(hook => hook(response));
            completedAction?.Invoke(response);
        }

        public async Task<RestResponse> Do(RestRequest request)
        {
            Options.BeforeHooks?.ForEach(hook => hook(request));
            var webRequest = BuildUnityWebRequest(request);
            try
            {
                webRequest = await webRequest.SendWebRequest();
            }
            catch (UnityWebRequestException e)
            {
                Debug.LogError($"do request exception: {e.Message}");
            }
            var response = new RestResponse(webRequest, request);
            Options.AfterHooks?.ForEach(hook => hook(response));
            return response;
        }

        private UnityWebRequest BuildUnityWebRequest(RestRequest request)
        {
            var url = request.BuildUrl(Options.BaseUrl);
            var webRequest = new UnityWebRequest(url, request.Method.String());
            webRequest.downloadHandler = request.DownloadHandler;
            webRequest.uploadHandler = request.BuildBody();
            Options.GlobalHeaders?.ForEach(header => request.Headers.Add(header));
            request.Headers.ForEach(header => webRequest.SetRequestHeader(header.Name, header.Value));

            var userAgent = webRequest.GetRequestHeader(KnownHeaders.UserAgent);
            if (string.IsNullOrEmpty(userAgent))
            {
                webRequest.SetRequestHeader(KnownHeaders.UserAgent, RestClientOptions.DefaultUserAgent);
            }

            var timeout = Options.Timeout;
            if (request.Timeout == 0)
            {
                timeout = Options.Timeout;
            }

            webRequest.timeout = timeout;
            return webRequest;
        }

        public IEnumerator Execute(Method method, string path, Action<RestResponse> completedAction,
            params IParam[] @params)
        {
            var request = new RestRequest(method, path);
            request.WithParams(@params);
            yield return Do(request, completedAction);
        }

        public IEnumerator Get(string path, Action<RestResponse> completedAction, params IParam[] @params)
        {
            yield return Execute(Method.Get, path, completedAction, @params);
        }

        public IEnumerator Post(string path, Action<RestResponse> completedAction, params IParam[] @params)
        {
            yield return Execute(Method.Post, path, completedAction, @params);
        }
    }
}