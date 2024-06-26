namespace NeedBodies.Api
{

    public static class BaseApi
    {
        public const string Endpoint = "https://bolst.pythonanywhere.com";
        public static HttpClient client { get; } = new HttpClient();
    }
}
