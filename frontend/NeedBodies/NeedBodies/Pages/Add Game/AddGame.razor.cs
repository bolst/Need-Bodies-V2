
using Microsoft.AspNetCore.Components;

namespace NeedBodies.Pages.AddGame
{
    public partial class AddGame
    {
        [Inject] protected NavigationManager navManager { get; set; } = default!;

        private List<Data.Arena>? ArenaList { get; set; } = null;

        private string enteredDisplayName { get; set; } = "";
        private string selectedArenaName { get; set; } = "";
        private DateTime selectedDate { get; set; } = DateTime.Now;
        private string selectedVisibility { get; set; } = "Public";

        private bool alertVisible { get; set; } = false;
        private string alertMessage { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            ArenaList = await Api.Arenas.GetArenaListAsync();
            selectedArenaName = ArenaList![0].Name;
        }
        async Task AddGameAsync()
        {
            Console.WriteLine("Game being added...");
            Console.WriteLine(enteredDisplayName);
            Console.WriteLine(selectedArenaName);
            Console.WriteLine(selectedDate);
            Console.WriteLine(selectedVisibility);

            Data.Game newGame = new Data.Game
            {
                DisplayName = enteredDisplayName,
                ArenaName = selectedArenaName,
                Date = selectedDate,
                Visibility = selectedVisibility
            };

            bool status = await Api.Games.AddGameAsync(newGame);
            if (status)
            {
                Console.WriteLine("\nAdded Game!");
                navManager.NavigateTo("/");
            }
            else
            {
                // TODO: handle if game could not be added
                alertMessage = "Something went wrong :(";
                alertVisible = true;
            }
        }
    }
}