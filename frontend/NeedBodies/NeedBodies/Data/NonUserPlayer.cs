using System.Text.Json.Serialization;

namespace NeedBodies.Data
{
    public class NonUserPlayer
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("games")] public List<UserTeam> Games { get; set; }
    }
}