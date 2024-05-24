namespace NeedBodies.Api
{
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

        public static async Task<bool> AddGameAsync(DataType game)
        {
            try
            {
                var response = await BaseApi.client.PostAsJsonAsync<DataType>(BaseApi.Endpoint + "/addgame", game);
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

        public static async Task<string> MapHtml()
        {
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/");
                response.EnsureSuccessStatusCode();
                var retval = await response.Content.ReadAsStringAsync();
                return retval;
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetArenaListAsync:\n" + exc.ToString());
                return "";
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