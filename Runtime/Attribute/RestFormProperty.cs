namespace PinealCtx.RestClient.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class RestFormProperty : System.Attribute
    {
        public RestFormProperty(string name) => Name = name;

        public string Name { get; }
    }
}