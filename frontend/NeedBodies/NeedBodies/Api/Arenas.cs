
namespace NeedBodies.Api
{

    using DataType = Data.Arena;

    public static class Arenas
    {
        public static async Task<List<DataType>?> GetArenaListAsync()
        {
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/arenas");
                response.EnsureSuccessStatusCode();
                List<DataType>? retval = await response.Content.ReadFromJsonAsync<List<DataType>>();
                return retval ?? DefaultArenaList();
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetArenaListAsync:\n" + exc.ToString());
                return DefaultArenaList();
            }
        }

        public static async Task<DataType?> GetArenaByName(string name)
        {
            try
            {
                var response = await BaseApi.client.GetAsync(BaseApi.Endpoint + "/arenas?name=" + name);
                response.EnsureSuccessStatusCode();
                DataType? retval = await response.Content.ReadFromJsonAsync<DataType>();
                return retval ?? new();
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetArenaByName:\n" + exc.ToString());
                return new();
            }
        }

        private static List<DataType> DefaultArenaList()
        {
            return new(){
                new DataType()
                {
                    Name="Tecumseh Arena",
                },
                new DataType()
                {
                    Name="Atlas Tube Center",
                },
                new DataType()
                {
                    Name="Central Park Athletics",
                },
            };
        }
    }
}