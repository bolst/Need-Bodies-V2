
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NeedBodies.Auth;

namespace NeedBodies.Pages
{
    public partial class AddGame
    {
        [Inject] protected NavigationManager navManager { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider authStateProvider { get; set; } = default;
        [Inject] protected UserService userService { get; set; } = default;
        [Inject] protected ProtectedSessionStorage protectedSessionStorage { get; set; } = default;

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

            var userSession = await protectedSessionStorage.GetAsync<UserSession>("UserSession");
            if (!userSession.Success)
            {
                return;
            }
            int currentUserID = userSession.Value.ID;
            bool status = await Api.Games.AddNewGameAsync(newGame, currentUserID);
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