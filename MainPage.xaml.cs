using System.Diagnostics;

namespace Project2A
{
    public partial class MainPage : ContentPage
    {
        private KeyHandling keyHandling;
        private WordViewModel wordViewModel;
        private SortedWords sortedWords;
        private string selectedWord;   // Selected word is a random chosen word from the sorted list, this is the word the user must guess.
        public int guesses;
        public int itemLCount;
        public int itemRCount;
        List<string> guessEntries = new List<string>(); // List of all user-entries
        List<string> correctGuesses = new List<string>(); // List of correct guesses
        private AudioPlayer player = new AudioPlayer(); //Audioplayer for sound
        int roundNum;
        private string roundNumString;
        private int[][] historyGrid;

        private int[][] CreateArray() // Creates a 30x6 array
        {
            int[][] jaggedArray = new int[30][];
            for (int i = 0; i < jaggedArray.Length; i++)
            {
                jaggedArray[i] = new int[6]; 
            }
            return jaggedArray;
        }

        Grid grid;
        public MainPage()
        {
            InitializeComponent();

            keyHandling = new KeyHandling
            {
                EntryLabel = EntryLabel // EntryLabel is the label that displays the user's input
            };

            keyHandling.KeyClickedEvent += GuessSubmission; // Event handler for user input

            HttpClient client = new HttpClient();
            wordViewModel = new WordViewModel(client);
            sortedWords = SortedWords.GetInstance(wordViewModel); // Use singleton instance of Sorted Words
            wordGrid = this.wordGrid; // Grid that displays guesses

            historyGrid = CreateArray();
        }

        private async Task InitializeGame()
        {
            try
            {
                guesses = 0;

                InitializeBlankGrid();
                ChangeAllButtonColors(Color.FromArgb("#A9A9A9")); // Dark gray color

                Debug.WriteLine("Initializing game..."); 

                // Gets words from viewModel, sorts then binds them
                await wordViewModel.GetWords();
                Debug.WriteLine($"WordList has {wordViewModel.WordList.Count} words.");

                sortedWords.SortWords();
                Debug.WriteLine($"Sorted words list has {sortedWords.WordListSorted.Count} words.");
                BindingContext = sortedWords;

                // Load game state
                GameState gameState = await GameStateSerializer.LoadGameStateAsync();

                // Set game state
                selectedWord = gameState.SelectedWord;
                roundNumString = gameState.RoundNumString;
                roundNum = int.Parse(roundNumString);
                guessEntries = gameState.GuessEntries;
                correctGuesses = gameState.CorrectGuesses;
                historyGrid = gameState.HistoryArr;
                itemLCount = gameState.ItemLCount;
                itemRCount = gameState.ItemRCount;


                if (string.IsNullOrEmpty(selectedWord))
                {
                    selectedWord = GetRandomWord();
                    gameState.SelectedWord = selectedWord;
                    await SaveGameStateAsync();
                }
                Debug.WriteLine($"Selected word: {selectedWord}");

                // Initialize previous guesses
                await InitializeGuesses();
                UpdateItemCounts();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing game: {ex.Message}");
                await DisplayAlert("Error", "An error occurred while initializing the game. Please try again.", "OK");
                RestartGame();
            }
        }
        private async void UpdateItemCounts()
        {
            LabelL.Text = $"{itemLCount}";
            Debug.WriteLine($"Item L Count: {itemLCount}");
            LabelR.Text = $"{itemRCount}";
            Debug.WriteLine($"Item R Count: {itemRCount}");
        }

        private async Task SaveGameStateAsync()
        {
            GameState gameState = new GameState
            {
                GuessEntries = guessEntries,
                SelectedWord = selectedWord,
                RoundNumString = roundNum.ToString(),
                HistoryArr = historyGrid,
                CorrectGuesses = correctGuesses,
                ItemLCount = itemLCount,
                ItemRCount = itemRCount
            };

            await GameStateSerializer.SaveGameStateAsync(gameState);
        }

        //Creates a 6x5 Grid
        private void InitializeBlankGrid()
        {
            wordGrid.Children.Clear();
            wordGrid.RowDefinitions.Clear();

            for (int row = 0; row < 6; row++)
            {
                wordGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (int col = 0; col < 5; col++) // 5 columns for letters
                {
                    Frame blankFrame = CreateLetterFrame(' ', Color.FromArgb("#ecf7e6")); // Gray for blank
                    wordGrid.Children.Add(blankFrame);
                    Grid.SetColumn(blankFrame, col);
                    Grid.SetRow(blankFrame, row);
                }
            }

        }
        private async Task<string> GetSelectedWord()
        {
            GameState gameState = await GameStateSerializer.LoadGameStateAsync();
            selectedWord = gameState.SelectedWord;
            if (string.IsNullOrEmpty(selectedWord) || !sortedWords.WordListSorted.Contains(selectedWord))
            {
                selectedWord = GetRandomWord();
                await SaveGameStateAsync();
            }
            return selectedWord;
        }
        //method for fetching a random word
        private string GetRandomWord()
        {
            Random random = new Random();
            if (sortedWords.WordListSorted.Count > 0)
            {
                int randomIndex = random.Next(sortedWords.WordListSorted.Count);
                return sortedWords.WordListSorted[randomIndex];
            }
            else
            {
                Debug.WriteLine("No words available in sorted list.");
                return "ERROR";
            }
        }
        //Initializes previous guesses, tnis is how the game saves/loads
        private async Task InitializeGuesses()
        {
            GameState gameState = await GameStateSerializer.LoadGameStateAsync();
            guessEntries = gameState.GuessEntries;
            if (guessEntries.Count > 0)
            {
                foreach (string entry in guessEntries)
                {
                    CreateWord(selectedWord, entry, true); //Rather than trying to load the previous state of the game, the game reads your previous inputs and tries them again essentially returning to the same position and when the app was closed.
                }
            }
        }
        //Checks if the entry is 5 alpabetic letters for error handling
        private bool IsValidWord(string word)
        {
            return word.Length == 5 && word.All(c => Char.IsLetter(c));
        }

        //Initializes or Re-Initializes the game on page appearing
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await InitializeGame();
        }

        //User Entry to Grid-UI
        public async void CreateWord(String selectedWord, String guessedWord, Boolean loadingEntry) //Creates the word in the grid, loadingEntry is used to determine if the game is loading or if the user is inputting a word
        {
            Debug.WriteLine("createWord called with selectedWord: " + selectedWord);

            if (string.IsNullOrEmpty(selectedWord))
            {
                selectedWord = await GetSelectedWord();
                if (string.IsNullOrEmpty(selectedWord))
                {
                    Debug.WriteLine("Selected word is still null");
                    selectedWord = "ERROR";
                    return;
                }
            }

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
                await SaveGameStateAsync(); // Save game state after updating variables
            }
            else
            {
                await DisplayAlert("Invalid Entry", "Please enter a valid 5 letter word", "Try Again");
            }
        }
        Frame GetFrameAtPosition(int row, int column)
        {
            foreach (var child in wordGrid.Children) // Loop through all children in grid
            {
                if (wordGrid.GetRow(child) == row && wordGrid.GetColumn(child) == column && child is Frame frame) // Check if child is a frame and if it is at the specified position in the grid
                {
                    return frame;
                }
            }
            return null;
        }

        private async Task HandleGuessResult(string word, string selectedWord)
        {
            if (word == "DEBUG")  // If user enters "DEBUG" in the game a seperate page is opened
            {
                await DisplayAlert("Debug Mode", "Entering debug menu...", "Continue");
                await NavigateToDebugMenu();
                return;
            }
            if (CheckForWin(word, selectedWord)) // If word is correct
            {
                correctGuesses.Add(word); // Add correct guess to the list
                if ((roundNum + 1) % 3 == 0)
                {
                    await player.CreateAudioPlayer("LevelUpWithHint.mp3");
                }
                else if ((roundNum + 1) % 5 == 0)
                {
                    await player.CreateAudioPlayer("LevelUpBonus.mp3");
                }
                else
                {
                    await player.CreateAudioPlayer("LevelPassed.mp3");
                }
                await player.PlayAudio();
                await DisplayAlert("Correct Word Guessed!", "You have guessed the correct word, press continue to move on to the next word", "Continue");
                RemoveWordFromList();
                roundNum += 1;
                if (roundNum % 3 == 0) // Increment itemRCount every 3 rounds
                {
                    itemRCount += 1;
                }
                if (roundNum % 5 == 0) // Increment itemLCount every 3 rounds
                {
                    itemLCount += 1;
                }
                UpdateItemCounts();
                await SaveGameStateAsync();
                await RestartGame();
            }
            else if (guesses == 6) // if all guesses are used up
            {
                await DisplayAlert("No Guesses Remaining", $"You have not guessed the correct word: {selectedWord}. Press continue to move on to the next word.", "Continue");
                roundNum = 0;
                ResetHistoryGrid(); 
                await SaveGameStateAsync();
                await RestartGame();
            }
        }
        private async Task NavigateToDebugMenu() //Navigates to the debug menu
        {
            // Navigate to the debug menu page
            await Navigation.PushAsync(new DebugMenuPage());
        }

        private void ResetHistoryGrid() //Resets the history grid to 0 for every value
        {
            for (int i = 0; i < historyGrid.Length; i++)
            {
                for (int j = 0; j < historyGrid[i].Length; j++)
                {
                    historyGrid[i][j] = 0;
                }
            }
        }
        //Clears entries and selects a new word
        public async Task RestartGame()
        {
            guessEntries.Clear();
            await SaveGameStateAsync();
            selectedWord = GetRandomWord();
            await SaveGameStateAsync();
            await InitializeGame();
        }
        //Checks if guessed word is the same as the selected word
        private bool CheckForWin(string word, string selectedWord)
        {
            return word == selectedWord;
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
        //gets background & audio colour for letter
        private async Task<Color> GetLetterColorAsync(char letter, string selectedWord, int index)
        {
            Color bgColor;
            Debug.WriteLine($"{index}");
            try
            {
                if (index >= 0 && index < selectedWord.Length)
                {
                    if (roundNum >= 0 && roundNum < historyGrid.Length) // Check if roundNum is within range
                    {
                        if (letter == selectedWord[index])
                        {
                            bgColor = Color.FromArgb("#66eb23"); // Green for correct letter
                            await player.CreateAudioPlayer("GreenLetter.mp3");
                            historyGrid[roundNum][index] = 3; // 3 = Green for correct letter
                            ChangeButtonColor(letter, Color.FromArgb("#66eb23"));
                        }
                        else if (selectedWord.Contains(letter))
                        {
                            bgColor = Color.FromArgb("#ebed51");  // Yellow for letter in word but wrong position
                            await player.CreateAudioPlayer("YellowLetter.mp3");
                            historyGrid[roundNum][index] = 2; // 2 = Yellow for letter in word but wrong position
                            ChangeButtonColor(letter, Color.FromArgb("#ebed51"));
                            
                        }
                        else
                        {
                            bgColor = Color.FromArgb("#ecf7e6");// Gray for incorrect letter
                            await player.CreateAudioPlayer("GrayLetter.mp3");
                            historyGrid[roundNum][index] = 1; // 1 = Gray for incorrect letter
                            ChangeButtonColor(letter, Color.FromArgb("#ecf7e6"));
                        }
                    }
                    else
                    {
                        Debug.WriteLine("roundNum is out of range.");
                        bgColor = Color.FromArgb("#ecf7e6"); // Default color
                    }
                    await SaveGameStateAsync(); // Save game state after updating variables
                    return bgColor;
                }
                else
                {
                    return Color.FromArgb("#ecf7e6"); // Default color
                    RestartGame();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetLetterColorAsync: {ex.Message}");
                return Color.FromArgb("#ecf7e6"); // Default color in case of error
            }
        }
        //Handler for submitting a guess
        public async void GuessSubmission(string enteredWord)
        {
            if (!string.IsNullOrWhiteSpace(enteredWord))
            {
                string word = enteredWord.ToUpper();
                if (IsValidWord(word))
                {
                    guessEntries.Add(word);
                    await SaveGameStateAsync();
                }
                CreateWord(selectedWord, word, false);
            }
        }
        private void OnKeyClicked(object sender, EventArgs e)
        {
            keyHandling.OnKeyClicked(sender, e);
        }
        private void ItemR(object sender, EventArgs e) // ItemR will change two letters on the keyboard to green
        {
            Debug.WriteLine($"ItemR called. selectedWord: {selectedWord}");
            if (!string.IsNullOrEmpty(selectedWord) && selectedWord.Length == 5)
            {
                Random random = new Random();
                int randomIndex = random.Next(selectedWord.Length);
                int randomIndex2 = random.Next(selectedWord.Length);
                while (randomIndex == randomIndex2)
                    randomIndex2 = random.Next(selectedWord.Length);
                char randomLetter = char.ToUpper(selectedWord[randomIndex]);
                ChangeButtonColor(randomLetter, Color.FromArgb("#66eb23")); // Green color
                randomLetter = char.ToUpper(selectedWord[randomIndex2]);
                ChangeButtonColor(randomLetter, Color.FromArgb("#66eb23")); // Green color
            }
        }
        
        private void ItemL(Object sender, EventArgs e)// Item L will skip a round entirely guessing the correct word,
        {
            Debug.WriteLine($"ItemL called.");
            GuessSubmission(selectedWord);
        }

        private async void RemoveWordFromList()
        {
            if (sortedWords.Remove(selectedWord)) // Remove word from shared list
            {
                Debug.WriteLine($"Word '{selectedWord}' removed. Remaining words: {sortedWords.WordListSorted.Count}"); // If word is found in list
            }
            else
            {
                Debug.WriteLine($"Word '{selectedWord}' not found in list."); // If word is not found in list
            }
            await SaveGameStateAsync();
        }


        private void ChangeButtonColor(char letter, Color backgroundColor)
        {
            Button button = null; 
            switch (letter) // Switch case for each letter
            {
                case 'A':
                    button = ButtonA;
                    break;
                case 'B':
                    button = ButtonB;
                    break;
                case 'C':
                    button = ButtonC;
                    break;
                case 'D':
                    button = ButtonD;
                    break;
                case 'E':
                    button = ButtonE;
                    break;
                case 'F':
                    button = ButtonF;
                    break;
                case 'G':
                    button = ButtonG;
                    break;
                case 'H':
                    button = ButtonH;
                    break;
                case 'I':
                    button = ButtonI;
                    break;
                case 'J':
                    button = ButtonJ;
                    break;
                case 'K':
                    button = ButtonK;
                    break;
                case 'L':
                    button = ButtonL;
                    break;
                case 'M':
                    button = ButtonM;
                    break;
                case 'N':
                    button = ButtonN;
                    break;
                case 'O':
                    button = ButtonO;
                    break;
                case 'P':
                    button = ButtonP;
                    break;
                case 'Q':
                    button = ButtonQ;
                    break;
                case 'R':
                    button = ButtonR;
                    break;
                case 'S':
                    button = ButtonS;
                    break;
                case 'T':
                    button = ButtonT;
                    break;
                case 'U':
                    button = ButtonU;
                    break;
                case 'V':
                    button = ButtonV;
                    break;
                case 'W':
                    button = ButtonW;
                    break;
                case 'X':
                    button = ButtonX;
                    break;
                case 'Y':
                    button = ButtonY;
                    break;
                case 'Z':
                    button = ButtonZ;
                    break;
                default:
                    break;
            }

            if (button != null && button.BackgroundColor != Color.FromArgb("#66eb23")) // If button is not already green change to yellow
            {
                button.BackgroundColor = backgroundColor;
            }
        }

        private void ChangeAllButtonColors(Color backgroundColor)
        {
            ButtonQ.BackgroundColor = backgroundColor;
            ButtonW.BackgroundColor = backgroundColor;
            ButtonE.BackgroundColor = backgroundColor;
            ButtonR.BackgroundColor = backgroundColor;
            ButtonT.BackgroundColor = backgroundColor;
            ButtonY.BackgroundColor = backgroundColor;
            ButtonU.BackgroundColor = backgroundColor;
            ButtonI.BackgroundColor = backgroundColor;
            ButtonO.BackgroundColor = backgroundColor;
            ButtonP.BackgroundColor = backgroundColor;
            ButtonA.BackgroundColor = backgroundColor;
            ButtonS.BackgroundColor = backgroundColor;
            ButtonD.BackgroundColor = backgroundColor;
            ButtonF.BackgroundColor = backgroundColor;
            ButtonG.BackgroundColor = backgroundColor;
            ButtonH.BackgroundColor = backgroundColor;
            ButtonJ.BackgroundColor = backgroundColor;
            ButtonK.BackgroundColor = backgroundColor;
            ButtonL.BackgroundColor = backgroundColor;
            ButtonZ.BackgroundColor = backgroundColor;
            ButtonX.BackgroundColor = backgroundColor;
            ButtonC.BackgroundColor = backgroundColor;
            ButtonV.BackgroundColor = backgroundColor;
            ButtonB.BackgroundColor = backgroundColor;
            ButtonN.BackgroundColor = backgroundColor;
            ButtonM.BackgroundColor = backgroundColor;
        }
    }
}
