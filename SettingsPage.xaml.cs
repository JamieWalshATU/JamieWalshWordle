namespace Project2A;
using Microsoft.Maui.Controls;
public partial class SettingsPage : ContentPage
{
    private MainPage mainPage;

    private int appThemeInt;
    public SettingsPage()
    {
        InitializeComponent();
        Content = new StackLayout
        {
            Padding = new Thickness(20),
            Children = {
                new Button { Text = "Reset Game Data", Command = new Command(async () => await ResetGameData()) },
                new Button { Text = "Enable/Disable Dark Mode", Command = new Command(async () => await DarkModeToggle()) },
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

    private async Task DarkModeToggle()
    {
        AppTheme appTheme = Application.Current.RequestedTheme;
        if (appTheme == AppTheme.Dark) { Application.Current.UserAppTheme = AppTheme.Light; Preferences.Default.Set("isDarkMode", false); }
        else { Application.Current.UserAppTheme = AppTheme.Dark; Preferences.Default.Set("isDarkMode", true); };
    }
}
