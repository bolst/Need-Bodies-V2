namespace NeedBodies.Api
{
    using System.Text;
    using System.Text.Json;
    using Microsoft.AspNetCore.Components;
    using NeedBodies.Auth;
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

        public static async Task<List<Data.Game>> GetHostGames(int host_id)
        {
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/games?hid=" + host_id.ToString());
                response.EnsureSuccessStatusCode();
                List<Data.Game> retval = await response.Content.ReadFromJsonAsync<List<Data.Game>>() ?? new();
                return retval;
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetHostGames:\n" + exc.ToString());
                return new();
            }
        }

        public static async Task<List<Data.Game>> GetUserGames(int host_id)
        {
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/games?uid=" + host_id.ToString());
                response.EnsureSuccessStatusCode();
                List<Data.Game> retval = await response.Content.ReadFromJsonAsync<List<Data.Game>>() ?? new();
                return retval;
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetUserGames:\n" + exc.ToString());
                return new();
            }
        }

        public static async Task<bool> AddUserToGameAsync(int uid, DataType game, string child_name = "")
        {
            try
            {
                var gameID = game.Id;
                var data = new Dictionary<string, object>
                {
                    { "game id", gameID },
                    {"user id", uid},
                    {"child name", child_name},
                };
                var response = await BaseApi.client.PostAsJsonAsync(BaseApi.Endpoint + "/joingame", data);
                response.EnsureSuccessStatusCode();
                string retval = await response.Content.ReadAsStringAsync();
                return retval == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("AddUserToGameAsync:\n" + exc.ToString());
                return false;
            }
        }

        public static async Task<List<User>> GetPlayersInGame(DataType game)
        {
            try
            {
                var gameID = game.Id;
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/users?gid=" + gameID.ToString());
                response.EnsureSuccessStatusCode();
                List<User> retval = await response.Content.ReadFromJsonAsync<List<User>>() ?? new();
                return retval;
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetPlayersInGame:\n" + exc.ToString());
                return new();
            }
        }

        public static async Task<List<Data.NonUserPlayer>> GetNonUserPlayersInGame(DataType game)
        {
            try
            {
                var gameID = game.Id;
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/nonusers/" + gameID.ToString());
                response.EnsureSuccessStatusCode();
                List<Data.NonUserPlayer> retval = await response.Content.ReadFromJsonAsync<List<Data.NonUserPlayer>>() ?? new();
                return retval;
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetNonUserPlayersInGame:\n" + exc.ToString());
                return new();
            }
        }

        public static async Task<bool> RemovePlayerFromGame(DataType game, int uid, string child_name = "")
        {
            try
            {
                var gameID = game.Id;
                var data = new Dictionary<string, object>
                {
                    { "game id", gameID },
                    {"user id", uid},
                    {"child name", child_name},
                };
                var response = await BaseApi.client.PostAsJsonAsync(BaseApi.Endpoint + "/removeuser", data);
                response.EnsureSuccessStatusCode();
                string retval = await response.Content.ReadAsStringAsync();
                return retval == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("RemovePlayerFromGame:\n" + exc.ToString());
                return false;
            }
        }

        public static async Task<bool> DeleteGame(DataType game, int uid)
        {
            try
            {
                var gameID = game.Id;
                var data = new Dictionary<string, object>
                {
                    { "game id", gameID },
                    {"user id", uid}
                };
                var response = await BaseApi.client.PostAsJsonAsync(BaseApi.Endpoint + "/deletegame", data);
                response.EnsureSuccessStatusCode();
                string retval = await response.Content.ReadAsStringAsync();
                return retval == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("DeleteGame:\n" + exc.ToString());
                return false;
            }
        }

        public static async Task<bool> CycleUserTeam(DataType game, int uid, string childName = "")
        {
            try
            {
                var gameID = game.Id;
                var data = new Dictionary<string, object>
                {
                    { "game id", gameID },
                    {"user id", uid },
                    {"child name", childName}
                };
                var response = await BaseApi.client.PostAsJsonAsync(BaseApi.Endpoint + "/cycleut", data);
                response.EnsureSuccessStatusCode();
                string retval = await response.Content.ReadAsStringAsync();
                return retval == "success";
            }
            catch (Exception exc)
            {
                Console.WriteLine("CycleUserTeam:\n" + exc.ToString());
                return false;
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