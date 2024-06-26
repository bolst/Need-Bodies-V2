using System.Security.AccessControl;
using System.Text.Json.Serialization;
using NeedBodies.Data;

namespace NeedBodies.Auth
{
    public class User
    {

        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("games")]
        public List<int> Games { get; set; }

        [JsonPropertyName("hosted games")]
        public List<int> HostedGames { get; set; }

        [JsonPropertyName("non users")]
        public List<NonUserPlayer> NonUsers { get; set; }

        [JsonPropertyName("teams")]
        public List<UserTeam> Teams { get; set; }

        [JsonPropertyName("position")]
        public string Position { get; set; }



        public async Task<bool> CheckPassword(string attempt)
        {
            return await Api.Auth.Validate(Username, attempt);
            //return await ServiceLayer.GetInstance().CheckUserCredentials(this.Username, attempt);
        }

    }
}
