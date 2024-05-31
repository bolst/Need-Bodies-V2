namespace NeedBodies.Api
{
    using System.Text;
    using System.Text.Json;
    using DataType = Data.Game;
    public static class Games
    {
        public static async Task<List<DataType>?> GetPublicGameListAsync()
        {
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/games");
                response.EnsureSuccessStatusCode();
                List<DataType>? retval = await response.Content.ReadFromJsonAsync<List<DataType>>() ?? DefaultPublicGameList();
                return retval;
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetArenaListAsync:\n" + exc.ToString());
                return DefaultPublicGameList();
            }
        }

        public static async Task<bool> AddNewGameAsync(DataType game, int uid)
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    { "game", game },
                    {"user id", uid}
                };
                var response = await BaseApi.client.PostAsJsonAsync(BaseApi.Endpoint + "/addgame", data);
                response.EnsureSuccessStatusCode();
                string retval = await response.Content.ReadAsStringAsync();
                return retval == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetArenaListAsync:\n" + exc.ToString());
                return false;
            }
        }

        public static async Task<DataType> GetGameById(int id)
        {
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/games?id=" + id.ToString());
                response.EnsureSuccessStatusCode();
                DataType retval = await response.Content.ReadFromJsonAsync<DataType>() ?? new();
                return retval;
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetArenaListAsync:\n" + exc.ToString());
                return new();
            }
        }

        public static async Task MapGameToHost(int gameID, int userID)
        {
            var userInfo = new Dictionary<string, int>
                {
                    {"gameID", gameID},
                    {"userID", userID},
                };
            var json = JsonSerializer.Serialize(userInfo);
            var strJson = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await BaseApi.client.PostAsync(BaseApi.Endpoint + "/mgth", strJson);
            }
            catch (Exception exc)
            {
                Console.WriteLine("MapGameToHost:\n" + exc.ToString());
            }
        }

        private static List<DataType> DefaultPublicGameList()
        {
            return new()
            {
                new DataType()
                {
                    Id=1,
                    DisplayName="Bolton Cup",
                },
                new DataType()
                {
                    Id=2,
                    DisplayName="HAP Hockey",
                    Date=new DateTime(2024,12,25,14,30,0),
                },
            };
        }


    }
}