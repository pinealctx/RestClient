using UnityEngine.Networking;

namespace PinealCtx.RestClient.Param
{
    public class FormParam : IParam
    {
        public FormParam(IMultipartFormSection section) => Section = section;

        public FormParam(string name, string value) : this(new MultipartFormDataSection(name, value))
        {
        }

        public FormParam(string name, byte[] value, string fileName = "", string contentType = "") : this(
            new MultipartFormFileSection(name, value, fileName, contentType))
        {
        }

        public FormParam(string name, FormFile file) : this(name, file.Data, file.FileName, file.ContentType)
        {
        }

        public IMultipartFormSection Section { get; set; }

        public ParamType ParamType() => Param.ParamType.Form;
    }

    public class FormFile
    {
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}