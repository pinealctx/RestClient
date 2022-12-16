namespace PinealCtx.RestClient.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class RestHeaderProperty : System.Attribute
    {
        public RestHeaderProperty(string name) => Name = name;

        public string Name { get; }
    }
}