namespace NeedBodies.Api
{

    public static class BaseApi
    {
        public const string Endpoint = "http://127.0.0.1:5000";
        public static HttpClient client { get; } = new HttpClient();
    }
}