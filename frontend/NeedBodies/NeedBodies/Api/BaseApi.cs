namespace NeedBodies.Api
{

    public static class BaseApi
    {
        public const string Endpoint = "http://127.0.0.1:8000";//"https://bolst.pythonanywhere.com";
        public static HttpClient client { get; } = new HttpClient();
    }
}
