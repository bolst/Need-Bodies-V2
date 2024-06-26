namespace NeedBodies.Api
{
    using System.Text;
    using System.Text.Json;
    using Microsoft.AspNetCore.Components;
    using NeedBodies.Auth;

    public static class Users
    {

        public static async Task<bool> SetUserPosition(int uid, string position)
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    { "position", position },
                    {"user id", uid}
                };
                var response = await BaseApi.client.PostAsJsonAsync(BaseApi.Endpoint + "/setposition", data);
                response.EnsureSuccessStatusCode();
                string retval = await response.Content.ReadAsStringAsync();
                return retval == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("SetUserPosition:\n" + exc.ToString());
                return false;
            }
        }

        public static async Task<bool> SetHeadshot(int uid, byte[] bytes)
        {
            try
            {
                var data = new ByteArrayContent(bytes);
                var response = await BaseApi.client.PostAsync(BaseApi.Endpoint + "/setheadshot?uid=" + uid.ToString(), data);
                response.EnsureSuccessStatusCode();
                string retval = await response.Content.ReadAsStringAsync();
                return retval == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("SetHeadshot:\n" + exc.ToString());
                return false;
            }
        }

        public static async Task<string> GetHeadshot(int uid)
        {
            return "";
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/headshot?id=" + uid.ToString());
                response.EnsureSuccessStatusCode();
                var retval = await response.Content.ReadAsStringAsync();

                byte[] bytes = Convert.FromBase64String(retval);
                using (var image = new FileStream("wwwroot/" + uid.ToString() + ".png", FileMode.Create))
                {
                    image.Write(bytes, 0, bytes.Length);
                    image.Flush();
                }

                return string.Format("data:image;base64,{0}", retval);
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetHeadshot:\n" + exc.ToString());
                return "";
            }
        }

        public static async Task<bool> DeleteNonUser(int uid, string nonUserName)
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    {"user id", uid},
                    {"child name", nonUserName}
                };
                var response = await BaseApi.client.PostAsJsonAsync(BaseApi.Endpoint + "/deletenonuser", data);
                response.EnsureSuccessStatusCode();
                string retval = await response.Content.ReadAsStringAsync();
                return retval == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("DeleteNonUser:\n" + exc.ToString());
                return false;
            }

        }

    }
}