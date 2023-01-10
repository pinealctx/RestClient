using System;
using System.Collections.Generic;
using PinealCtx.RestClient.Param;

namespace PinealCtx.RestClient
{
    public class RestClientOptions
    {
        private const string Version = "0.0.1";
        public static readonly string DefaultUserAgent = $"RestClient/{Version}";

        public RestClientOptions()
        {
        }

        public RestClientOptions(Uri baseUrl) => BaseUrl = baseUrl;

        public RestClientOptions(string baseUrl) : this(new Uri(baseUrl))
        {
        }

        /// <summary>
        /// 请求BaseUrl
        /// </summary>
        public Uri BaseUrl { get; set; }

        /// <summary>
        /// 请求超时时间
        /// </summary>
        public int Timeout { get; set; } = 10;

        /// <summary>
        /// 请求公共头
        /// </summary>
        public List<HeaderParam> GlobalHeaders { get; set; }

        /// <summary>
        /// 请求执行前的Hooks
        /// </summary>
        public List<Action<RestRequest>> BeforeHooks { get; set; } = new();

        /// <summary>
        /// 请求执行后的Hooks
        /// </summary>
        public List<Action<RestResponse>> AfterHooks { get; set; } = new();

        /// <summary>
        /// 添加执行前Hook
        /// </summary>
        /// <param name="hook"></param>
        public void AddBeforeHook(Action<RestRequest> hook)
        {
            BeforeHooks.Add(hook);
        }

        /// <summary>
        /// 添加执行后Hook
        /// </summary>
        /// <param name="hook"></param>
        public void AddAfterHook(Action<RestResponse> hook)
        {
            AfterHooks.Add(hook);
        }
    }
}