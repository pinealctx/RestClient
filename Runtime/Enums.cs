namespace PinealCtx.RestClient
{
    public enum Method
    {
        Get,
        Post
    }

    public static class EnumExtensions
    {
        public static string String(this Method enumValue)
        {
            return enumValue switch
            {
                Method.Get => "GET",
                Method.Post => "POST",
                _ => "GET"
            };
        }
    }
}