namespace Pinealctx.RestClient.Param
{
    public class HeaderParam : IParam
    {
        public HeaderParam(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public ParamType ParamType() => Param.ParamType.Header;
    }
}