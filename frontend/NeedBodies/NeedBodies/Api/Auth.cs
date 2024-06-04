using System.Text;
using System.Text.Json;
using NeedBodies.Auth;

namespace NeedBodies.Api
{
    public static class Auth
    {

        public static async Task<bool> Validate(string username, string password)
        {
            var content = new Dictionary<string, string>
            {
                {"username", username},
                {"password", password}
            };
            var json = JsonSerializer.Serialize(content);
            var strJson = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await BaseApi.client.PostAsync(BaseApi.Endpoint + "/validate", strJson);
                return await response.Content.ReadAsStringAsync() == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("Validate: ", exc.ToString());
                return false;
            }
        }

        public static async Task<(string, int)> AddUser(string username, string email, string password, string parent_id = "")
        {
            var userInfo = new Dictionary<string, string>
                {
                    {"username", username},
                    {"password", password},
                    {"email", email},
                    {"parent id", parent_id},
                };
            var json = JsonSerializer.Serialize(userInfo);
            var strJson = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await BaseApi.client.PostAsync(BaseApi.Endpoint + "/adduser", strJson);
                var content = await response.Content.ReadAsStringAsync();
                JsonDocument? retval = JsonDocument.Parse(content);
                string message = retval.RootElement.GetProperty("message").ToString();
                int newID = int.Parse(retval.RootElement.GetProperty("id").ToString());
                return (message, newID);
            }
            catch (Exception exc)
            {
                Console.WriteLine("AddUser:\n" + exc.ToString());
                return ("Something went wrong", 0);
            }
        }

        public static async Task<bool> AddNonUserPlayer(int currentUID, string name)
        {
            return (await AddUser(name, "", "", currentUID.ToString())).Item1 == "success";
        }

        public static async Task<List<User>?> GetUsers()
        {
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/users");
                response.EnsureSuccessStatusCode();
                var s = await response.Content.ReadAsStringAsync();
                List<User>? retval = await response.Content.ReadFromJsonAsync<List<User>?>();
                return retval;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Auth.cs:\n", exc.ToString());
                return null;
            }
        }


    }
}