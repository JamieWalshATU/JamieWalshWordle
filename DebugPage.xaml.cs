namespace Project2A;
using Microsoft.Maui.Controls;
public partial class DebugMenuPage : ContentPage
{
    private MainPage mainPage;
    public DebugMenuPage()
    {
        InitializeComponent();
        Content = new StackLayout
        {
            Padding = new Thickness(20),
            Children = {
                new Label { Text = "Debug Menu", FontSize = 24, HorizontalOptions = LayoutOptions.Center },
                new Button { Text = "Reset Game Data", Command = new Command(async () => await ResetGameData()) },
                new Button { Text = "Clear History Grid", Command = new Command(async () => await ClearHistoryGrid()) },
                new Button { Text = "Reset Round Number", Command = new Command(async () => await ResetRoundNumber()) }
            }
        };
    }
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
    }

    private async Task ResetGameData()
    {
        // Clear game data
        await GameStateSerializer.SaveEntriesAsync(new List<string>());
        await GameStateSerializer.SaveSelectedWordAsync(string.Empty);
        await GameStateSerializer.SaveRoundNumAsync("0");
        await GameStateSerializer.SaveHistoryArrAsync(new int[30][]);
        await DisplayAlert("Reset Game Data", "Game data has been reset.", "OK");
    }

    private async Task ClearHistoryGrid()
    {
        // Clear history grid
        await GameStateSerializer.SaveHistoryArrAsync(new int[30][]);
        await DisplayAlert("Clear History Grid", "History grid has been cleared.", "OK");
    }

    private async Task ResetRoundNumber()
    {
        // Reset round number
        await GameStateSerializer.SaveRoundNumAsync("0");
        await DisplayAlert("Reset Round Number", "Round number has been reset.", "OK");
    }
}
