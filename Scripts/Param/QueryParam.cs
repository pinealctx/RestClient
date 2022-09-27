using UnityEngine.Networking;

namespace Pinealctx.RestClient.Param
{
    public class QueryParam : IParam
    {
        public QueryParam(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public ParamType ParamType() => Param.ParamType.Query;

        public override string ToString()
        {
            return $"{UnityWebRequest.EscapeURL(Name)}={UnityWebRequest.EscapeURL(Value)}";
        }
    }
}