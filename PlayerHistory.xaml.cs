namespace Project2A;
public partial class PlayerHistory : ContentPage
{
    private string[][] historyGrid; // Jagged array to track user progress for a UI Element
    List<string> correctGuesses = new List<string>(); // List of correct guesses
    int roundNum; // Number of Round

    private MainPage mainPage;
    public PlayerHistory()
    {
        InitializeComponent();
        CreateArray();
        loadGameState();
        
        for (int i = 0; i < roundNum; i++)
        {
            CreateGrid(i);
        }
    }

    private int[][] CreateArray() // Creates a 30x6 array
    {
        int[][] jaggedArray = new int[30][];
        for (int i = 0; i < jaggedArray.Length; i++)
        {
            jaggedArray[i] = new int[6];
        }
        return jaggedArray;
    }
    private async Task loadGameState()
    {
        GameState gameState = await GameStateSerializer.LoadGameStateAsync();

        historyGrid = gameState.HistoryArr;
        correctGuesses = gameState.CorrectGuesses;
        roundNum = Int32.Parse(gameState.RoundNumString);
    }
    //HistoryGrid [roundNum][index] =  3 green, 2 yellow, 1 gray
    private void CreateGrid(int i)
    {
        Grid grid = new Grid();

    }
}
