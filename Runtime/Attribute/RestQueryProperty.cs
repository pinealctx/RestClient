namespace PinealCtx.RestClient.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class RestQueryProperty : System.Attribute
    {
        public RestQueryProperty(string name) => Name = name;

        public string Name { get; }
    }
}