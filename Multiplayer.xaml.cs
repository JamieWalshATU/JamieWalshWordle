using System.Diagnostics;
using System.Timers;
namespace Project2A;

public partial class Multiplayer : ContentPage
{
    // Alot of this code was added last minute and is a mess as a result, most of it has the same functionality as mainPage
    private SortedWords sortedWords;
    private WordViewModel wordViewModel;
    private AudioPlayer player = new AudioPlayer(); //Audioplayer for sound
    private KeyHandlingMulti keyHandling;
    private int player1score = 0;
    private int player2score = 0;
    private int turnCounter = 0;
    private int roundCounter = 0;
    public int guesses;
    private string selectedWord;
    List<string> multiplayerWordList = new List<string>();
    private static System.Timers.Timer aTimer;
    private int timePassed;
    bool buttonEnabled = true;

    public Multiplayer()
    {
        InitializeComponent();
        keyHandling = new KeyHandlingMulti
        {
            EntryLabelMulti = EntryLabelMulti // EntryLabel is the label that displays the user's input
        };
        keyHandling.KeyClickedEvent += GuessSubmission;
        HttpClient client = new HttpClient();
        wordViewModel = new WordViewModel(client);
        sortedWords = SortedWords.GetInstance(wordViewModel);
        LoadMultiplayerWords();
        wordGridMulti = this.wordGridMulti; // Grid that displays guesses

        aTimer = new System.Timers.Timer(1000);
        aTimer.AutoReset = true;
        aTimer.Elapsed += OnTimedEvent;
        aTimer.Enabled = true; 
    }
    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        timePassed++;
    }

    protected override async void OnAppearing()
    {
        await DisplayAlert("Welcome to Multiplayer", "Both Players will get the same 4 Words to guess, whoever has the lowest time when all words are guessed wins. Failing to guess a word after 6 attempts will add 60 seconds to your score. Be sure to look away when it's not your turn", "Play");
    }
    private async void GuessSubmission(string enteredWord)
    {
        if (!string.IsNullOrWhiteSpace(enteredWord))
        {
            string word = enteredWord.ToUpper();
            CreateWord(multiplayerWordList[roundCounter], word, false);
        }
    }

    private async void LoadMultiplayerWords()
    {
        multiplayerWordList = await sortedWords.MultiplayerWords();
    }
    public async void CreateWord(String selectedWord, String guessedWord, Boolean loadingEntry) //Creates the word in the grid, loadingEntry is used to determine if the game is loading or if the user is inputting a word
    {
        selectedWord = selectedWord.ToUpper();
        string word = guessedWord.ToUpper();

        if (IsValidWord(word))
        {
            Color BGColor = Color.FromArgb("#ecf7e6"); // Gray BG (Default)
            int row = guesses;
            //Displays each letter 
            for (int i = 0; i < word.Length; i++)
            {
                BGColor = await GetLetterColorAsync(word[i], selectedWord, i);//Gets Colour based on letter
                
                Frame existingFrame = GetFrameAtPosition(row, i);

                if (existingFrame != null)
                {
                    // Update the frame's content and background color
                    ((Label)existingFrame.Content).Text = word[i].ToString();
                    existingFrame.BackgroundColor = BGColor;

                    // Add bounce animation
                    await existingFrame.ScaleTo(1.2, 100);
                    await existingFrame.ScaleTo(1.0, 100);
                }

                if (existingFrame != null)

                    if (!loadingEntry)// if this is an actual user input, the game will play audio and delay to inputs to form an animation, if the game is just laoding up these functions are ignored
                    {
                        await player.PlayAudio();
                        await Task.Delay(500);
                    }
            }
            guesses++;
            await HandleGuessResult(word, selectedWord);
        }
        else
        {
            await DisplayAlert("Invalid Entry", "Please enter a valid 5 letter word", "Try Again");
        }
    }
    private bool IsValidWord(string word)
    {
        return word.Length == 5 && word.All(c => Char.IsLetter(c));
    }
    private async Task HandleGuessResult(string word, string selectedWord)
    {

        if (CheckForWin(word, selectedWord)) // If word is correct
        {
            await player.CreateAudioPlayer("LevelPassed.mp3");
            await player.PlayAudio();
            if (turnCounter == 0)
            {
                player1score = +1;
            }
            if (turnCounter == 1)
            {
                player2score = +1;
            }
            roundCounter++;
            guesses = 0;
            await RestartGame();
        }
        else if (guesses == 6) // if all guesses are used up
        {
            if (turnCounter == 0)
            {
                player1score = +60;
            }
            if (turnCounter == 1)
            {
                player2score = +60;
            }
            roundCounter++;
            guesses = 0;
            await RestartGame();
        }
    }
    private async Task<Color> GetLetterColorAsync(char letter, string selectedWord, int index)
    {
        Color bgColor;
        Debug.WriteLine($"{index}");
        String roundIndexString = "";
        try
        {
            if (index >= 0 && index < selectedWord.Length)
            {
                if (letter == selectedWord[index])
                {
                    bgColor = Color.FromArgb("#66eb23"); // Green for correct letter
                    await player.CreateAudioPlayer("GreenLetter.mp3");
                }
                else if (selectedWord.Contains(letter))
                {
                    bgColor = Color.FromArgb("#ebed51");  // Yellow for letter in word but wrong position
                    await player.CreateAudioPlayer("YellowLetter.mp3");

                }
                else
                {
                    bgColor = Color.FromArgb("#ecf7e6");// Gray for incorrect letter
                    await player.CreateAudioPlayer("GrayLetter.mp3");
                }
            }
            else
            {
                Debug.WriteLine("roundNum is out of range.");
                bgColor = Color.FromArgb("#ecf7e6"); // Default color
            } 
            return bgColor;
        }
        catch (Exception ex)
        {
            return Color.FromArgb("#ecf7e6"); // Default color in case of error
        }
    }
    public async Task RestartGame()
    {
        guesses = 0; // Reset guesses
        if (roundCounter == 4)
        {
            aTimer.Stop();
            if (turnCounter == 0)
            {
                player1score = timePassed;
                await DisplayAlert("Game Over", $"Player 1's score: {player1score}, Press the Button to Start Again", "OK");
                buttonEnabled = true;
            }
            else if (turnCounter == 1)
            {
                player2score = timePassed;
                await DisplayAlert("Game Over", $"Player 2's score: {player2score}, Press the Button to Start Again", "OK");
                buttonEnabled = true;
            }
            wordGridMulti.Children.Clear();
            wordGridMulti.RowDefinitions.Clear();
            return;
        }
        selectedWord = multiplayerWordList[roundCounter];
        await InitializeGame();
    }
    private async Task InitializeGame()
    {
        InitializeBlankGrid();
        LoadMultiplayerWords();
        Debug.WriteLine(multiplayerWordList[roundCounter]);
    }
    private void InitializeBlankGrid()
    {
        wordGridMulti.Children.Clear();
        wordGridMulti.RowDefinitions.Clear();

        for (int row = 0; row < 6; row++)
        {
            wordGridMulti.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int col = 0; col < 5; col++) // 5 columns for letters
            {
                Frame blankFrame = CreateLetterFrame(' ', Color.FromArgb("#ecf7e6")); // Gray for blank
                wordGridMulti.Children.Add(blankFrame);
                Grid.SetColumn(blankFrame, col);
                Grid.SetRow(blankFrame, row);
            }
        }

    }
    // Create a frame for displaying a letter, with specified background color
    private Frame CreateLetterFrame(char letter, Color bgColor)
    {
        return new Frame
        {
            Content = new Label
            {
                Text = letter.ToString(),
                FontSize = 30,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            },
            BorderColor = Colors.Black,
            Padding = new Thickness(10),
            CornerRadius = 5,
            HasShadow = true,
            BackgroundColor = bgColor,
            HeightRequest = 80,
        };
    }
    Frame GetFrameAtPosition(int row, int column)
    {
        foreach (var child in wordGridMulti.Children) // Loop through all children in grid
        {
            if (wordGridMulti.GetRow(child) == row && wordGridMulti.GetColumn(child) == column && child is Frame frame) // Check if child is a frame and if it is at the specified position in the grid
            {
                return frame;
            }
        }
        return null;
    }

    private bool CheckForWin(string word, string selectedWord)
    {
        return word == selectedWord;
    }

    private void OnKeyClicked(object sender, EventArgs e)
    {
        keyHandling.OnKeyClicked(sender, e);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        if (buttonEnabled == true)
        {
            guesses = 0;
            if (turnCounter == 0)
            {
                LoadMultiplayerWords();
            }
            InitializeGame();
            aTimer.Start();
            buttonEnabled = false;
        }
    }

    private void StartTimer()
    {
        timePassed = 0;
        aTimer.Start();
    }
    private void StopTimer()
    {
        aTimer.Stop();
    }
    public async Task ShowWinnerAndReset()
    {
        string winnerMessage;
        if (player1score < player2score)
        {
            winnerMessage = "Player 1 wins!";
        }
        else if (player2score < player1score)
        {
            winnerMessage = "Player 2 wins!";
        }
        else
        {
            winnerMessage = "It's a tie!";
        }

        await DisplayAlert("Game Over", winnerMessage, "OK");

        player1score = 0;
        player2score = 0;
        turnCounter = 0;
        roundCounter = 0;
        guesses = 0;
        timePassed = 0;
        multiplayerWordList.Clear();
        wordGridMulti.Children.Clear();
        wordGridMulti.RowDefinitions.Clear();
    }
}
