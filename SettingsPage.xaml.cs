namespace Project2A;
using Microsoft.Maui.Controls;
public partial class SettingsPage : ContentPage
{
    private MainPage mainPage;
    public SettingsPage()
    {
        InitializeComponent();
        Content = new StackLayout
        {
            Padding = new Thickness(20),
            Children = {
                new Label { Text = "Debug Menu", FontSize = 24, HorizontalOptions = LayoutOptions.Center },
                new Button { Text = "Reset Game Data", Command = new Command(async () => await ResetGameData()) },
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
        GameState gameState = new GameState();
        await GameStateSerializer.SaveGameStateAsync(gameState);
        await DisplayAlert("Reset Game Data", "Game data has been reset.", "OK");
    }
}
