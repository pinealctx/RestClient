using Newtonsoft.Json;

namespace Pinealctx.RestClient.Param
{
    public class BodyParam : IParam
    {
        public string ContentType { get; set; }
        public byte[] Data { get; set; }

        public BodyParam(string contentType, byte[] data)
        {
            ContentType = contentType;
            Data = data;
        }

        public ParamType ParamType()
        {
            return Param.ParamType.Body;
        }

        public static BodyParam BuildJson(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var bytes = new System.Text.UTF8Encoding().GetBytes(json);
            return new BodyParam(KnownContentTypes.Json, bytes);
        }
    }
}