using System.Text.Json.Serialization;

namespace NeedBodies.Data
{
    public class UserTeam
    {
        [JsonPropertyName("game id")] public int GameID { get; set; }
        [JsonPropertyName("team")] public int Team { get; set; }
    }
}