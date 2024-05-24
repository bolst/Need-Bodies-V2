namespace NeedBodies.Data
{
    public class Game
    {
        public int Id { get; set; } = 0;
        public string DisplayName { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
        public string ArenaName { get; set; } = "";
        public string Visibility { get; set; } = "Public";

    }
}