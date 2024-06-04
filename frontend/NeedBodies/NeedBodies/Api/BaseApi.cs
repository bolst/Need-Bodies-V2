namespace NeedBodies.Api
{

    public static class BaseApi
    {
        public const string Endpoint = "https://bolst.pythonanywhere.com";//"http://127.0.0.1:8000";
        public static HttpClient client { get; } = new HttpClient();
    }
}
