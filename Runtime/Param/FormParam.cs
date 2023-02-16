using System.Text;
using UnityEngine.Networking;

namespace PinealCtx.RestClient.Param
{
    public class FormParam : IParam
    {
        public FormParam(string fieldName, string value, Encoding encoding = null)
        {
            FieldName = fieldName;
            Value = value;
            Encoding = encoding ?? Encoding.UTF8;
        }

        public FormParam(string fieldName, byte[] data, string fileName = null, string mimeType = null)
        {
            FieldName = fieldName;
            Data = data;
            FileName = fileName;
            MimeType = mimeType;
        }

        public string FieldName { get; }
        public string Value { get; }
        public Encoding Encoding { get; }
        public byte[] Data { get; }
        public string FileName { get; }
        public string MimeType { get; }

        public ParamType ParamType() => Param.ParamType.Form;
    }

    public class FormFile
    {
        public byte[] Data { get; set; }
        public string FileName { get; set; }
    }
}