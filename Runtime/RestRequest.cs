using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PinealCtx.RestClient.Attribute;
using PinealCtx.RestClient.Param;
using UnityEngine;
using UnityEngine.Networking;
using RestHeaderProperty = PinealCtx.RestClient.Attribute.RestHeaderProperty;

namespace PinealCtx.RestClient
{
    public class RestRequest : IDisposable
    {
        public RestRequest(Method method, string path)
        {
            Method = method;
            Path = path;
        }

        public RestRequest(string path) : this(Method.Get, path)
        {
        }

        public Method Method { get; set; }

        public string Path { get; set; }

        public List<QueryParam> Queries { get; set; } = new();

        public List<HeaderParam> Headers { get; set; } = new();

        public WWWForm Forms { get; set; }

        public BodyParam Body { get; set; }

        public int Timeout { get; set; }

        public DownloadHandler DownloadHandler { get; set; } = new DownloadHandlerBuffer();

        public RestRequest WithParam(IParam param)
        {
            switch (param)
            {
                case QueryParam queryParam:
                    Queries.Add(queryParam);
                    break;
                case HeaderParam headerParam:
                    Headers.Add(headerParam);
                    break;
                case BodyParam bodyParam:
                    Body = bodyParam;
                    break;
                case FormParam formParam:
                    if (formParam.Data == null)
                    {
                        Forms.AddField(formParam.FieldName, formParam.Value, formParam.Encoding);
                    }
                    else
                    {
                        Forms.AddBinaryData(formParam.FieldName, formParam.Data, formParam.MimeType);
                    }

                    break;
            }

            return this;
        }

        public RestRequest WithParams(params IParam[] @params)
        {
            foreach (var param in @params)
            {
                WithParam(param);
            }

            return this;
        }

        public RestRequest WithQuery(string name, string value)
        {
            Queries.Add(new QueryParam(name, value));
            return this;
        }

        public RestRequest WithHeader(string name, string value)
        {
            Headers.Add(new HeaderParam(name, value));
            return this;
        }

        public RestRequest WithBody(string contentType, byte[] body)
        {
            Body = new BodyParam(contentType, body);
            return this;
        }

        public RestRequest WithJsonBody(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var bytes = new UTF8Encoding().GetBytes(json);
            Body = new BodyParam(KnownContentTypes.Json, bytes);
            return this;
        }

        public RestRequest WithParamObject(object obj)
        {
            var type = obj.GetType();
            var props = type.GetProperties();
            var hasForm = false;
            foreach (var prop in props)
            {
                var attrs = prop.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    switch (attr)
                    {
                        case RestHeaderProperty headerProperty:
                        {
                            WithHeader(headerProperty.Name, Object2String(prop.GetValue(obj)));
                            break;
                        }
                        case RestQueryProperty queryProperty:
                        {
                            WithQuery(queryProperty.Name, Object2String(prop.GetValue(obj)));
                            break;
                        }
                        case RestFormProperty formProperty:
                        {
                            hasForm = true;
                            Forms ??= new WWWForm();
                            var value = prop.GetValue(obj);
                            if (value == null) continue;
                            if (value.GetType() != typeof(FormFile))
                            {
                                Forms.AddField(formProperty.Name, Object2String(value));
                                continue;
                            }

                            if (value is FormFile formFile) Forms.AddBinaryData(formProperty.Name, formFile.Data);

                            break;
                        }
                    }
                }
            }

            if (hasForm)
            {
                Body = null;
            }

            return this;
        }

        private static string Object2String(object value) => Type.GetTypeCode(value.GetType()) == TypeCode.Int32
            ? Convert.ToInt32(value).ToString()
            : value.ToString();

        public RestRequest WithDownloadHandler(DownloadHandler handler)
        {
            DownloadHandler = handler;
            return this;
        }

        public Uri BuildUrl(Uri baseUrl)
        {
            UriBuilder builder;

            if (Path.StartsWith("http://") || Path.StartsWith("https://") || Path.StartsWith("file://"))
            {
                builder = new UriBuilder(Path);
            }
            else
            {
                if (baseUrl == null)
                {
                    throw new Exception("invalid.url");
                }

                builder = new UriBuilder(baseUrl);
                builder.Path = string.Join("/", builder.Path.TrimEnd('/'), Path.TrimStart('/'));
            }

            if (Path.StartsWith("file://")) return builder.Uri;

            builder.Query.TrimStart('?').Split("&").ToList().ForEach(v =>
            {
                if (v == "") return;
                var vs = v.Split("=");
                var name = vs[0];
                var value = "";
                if (vs.Length == 2)
                {
                    value = vs[1];
                }

                Queries.Add(new QueryParam(name, value));
            });

            if (Queries.Count != 0)
            {
                builder.Query = $"?{string.Join("&", Queries.Select(q => q.ToString()))}";
            }

            return builder.Uri;
        }

        public UploadHandlerRaw BuildBody()
        {
            if (Method == Method.Get) return null;
            if (Body == null)
            {
                if (Forms == null) return null;
                var data = Forms.data;
                if (data.Length == 0) return null;
                Forms.headers.TryGetValue("Content-Type", out var contentType);
                Body = new BodyParam(contentType, data);
            }

            var uploadHandler = new UploadHandlerRaw(Body.Data);
            uploadHandler.contentType = Body.ContentType;
            return uploadHandler;
        }

        public void Dispose()
        {
            Queries?.Clear();
            Queries = null;
            Headers?.Clear();
            Forms = null;
            Headers = null;
            Body = null;
        }
    }
}