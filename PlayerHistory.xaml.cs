using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace Project2A;
public partial class PlayerHistory : ContentPage
{
    private string[][] historyGrid; // Jagged array to track user progress for a UI Element
    List<string> correctGuesses = new List<string>(); // List of correct guesses
    int roundNum; // Number of Round
    int totalGueses = 0; // Total Overall Guesses

    private MainPage mainPage;
    public PlayerHistory()
    {
        InitializeComponent();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        historyGrid = CreateArray();
        await loadGameState();
        Debug.WriteLine($"{historyGrid}");
        mainPage = new MainPage();
        int i = 0;
        UpdateCounters();
        while (i < historyGrid.Length && historyGrid[i][0] != null)
        {
            CreateGrid(i);
            i++;
        }
    }
    private string[][] CreateArray() // Creates a 30x6 array
    {
        string[][] jaggedArray = new string[30][];
        for (int i = 0; i < jaggedArray.Length; i++)
        {
            jaggedArray[i] = new string[6];
        }
        return jaggedArray;
    }

    private async Task loadGameState()
    {
        GameState gameState = await GameStateSerializer.LoadGameStateAsync();

        historyGrid = gameState.HistoryArr;
        correctGuesses = gameState.CorrectGuesses;
        totalGueses = gameState.TotalGuesses;
        roundNum = Int32.Parse(gameState.RoundNumString);
    }

    //HistoryGrid [roundNum][guesses], 3 green, 2 yellow, 1 gray
    private void CreateGrid(int i)
    {
        Debug.WriteLine($"{historyGrid[i].Length}");
        int sizeCounter = 0;
        for (int j = 0; j < 5; j++) // Checks the length of actual data in the array, when the value is 0 or null, no actual data is beyond this point, size counter represents this
        {
            if (historyGrid[i][j] != null)
            {
                if ((Int32.Parse(historyGrid[i][j][0].ToString()) != 0)) // Checking first characcter, 0 means no useful data
                {
                    sizeCounter++;
                }
            }
        }
        Debug.WriteLine($"{sizeCounter}");
        Grid outerGrid = new Grid
        {
            Margin = new Thickness(0),
            HorizontalOptions = LayoutOptions.Start
        };
        Grid innerGrid = new Grid
        {
            Margin = new Thickness(0),
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Start
        };

        for (int col = 0; col < 5; col++)
        {
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for (int row = 0; row < sizeCounter; row++)
        {
            innerGrid.RowDefinitions.Add(new RowDefinition { Height = 20 });
            Color bgColor = Color.FromArgb("#FFFFFF"); //White for default
            for (int col = 0; col < 5; col++) // 5 columns for letters
            {
                char x = historyGrid[i][row][col];
                switch (x)
                {
                    case '3':
                        bgColor = Color.FromArgb("#66eb23"); // Green for 3
                        break;
                    case '2':
                        bgColor = Color.FromArgb("#ebed51"); // Yellow for 2
                        break;
                    case '1':
                        bgColor = Color.FromArgb("#ecf7e6"); // Gray for 1
                        break;
                }

                Frame blankFrame = CreateFrame(bgColor); // Gray for blank
                Grid.SetColumn(blankFrame, col);
                Grid.SetRow(blankFrame, row);
                innerGrid.Children.Add(blankFrame);
            }
            Debug.WriteLine($"{historyGrid[i][row]}");
        }
        Label wordLabel = new Label
        {
            Text = correctGuesses[i],
            Margin = new Thickness(0),
            HorizontalOptions = LayoutOptions.Start
        };
        container1.Children.Add(innerGrid);
        container1.Children.Add(wordLabel);
    }

    private Frame CreateFrame(Color bgColor)
    {
        return new Frame
        {
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            CornerRadius = 5,
            HasShadow = true,
            BackgroundColor = bgColor,
            HeightRequest = 20,
            WidthRequest = 20,
            BorderColor = Color.FromArgb("#000000")
        };
    }

    private async void UpdateCounters()
    {
        roundCounter.Text = "Rounds Won: " + roundNum.ToString();
        guessCounter.Text = "Total Guesses: " + totalGueses.ToString();
    }
}