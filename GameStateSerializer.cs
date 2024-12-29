using System.Diagnostics;
using System.Text.Json;

namespace JamieWalshWordle
{
    public static class GameStateSerializer
    {
        private static string appData = FileSystem.AppDataDirectory;

        private static string gameStateFilePath = Path.Combine(appData, "gameState.json");
        public static async Task SaveGameStateAsync(GameState gameState)
        {
            try
            {
                string json = JsonSerializer.Serialize(gameState);
                using (StreamWriter writer = new StreamWriter(gameStateFilePath))
                {
                    await writer.WriteAsync(json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error saving game state: " + ex.Message);
            }
        }

        // Loads the entire game state
        public static async Task<GameState> LoadGameStateAsync()
        {
            if (!File.Exists(gameStateFilePath))
            {
                Debug.WriteLine("Game state file does not exist.");
                return new GameState();
            }

            try
            {
                using (StreamReader reader = new StreamReader(gameStateFilePath))
                {
                    string json = await reader.ReadToEndAsync();
                    return JsonSerializer.Deserialize<GameState>(json) ?? new GameState();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading game state: " + ex.Message);
                return new GameState();
            }
        }

        //Below code is old code that I was using to save the game across multiple seperate files, left in for now in case theres issues with the new method

        ////Saves all user entries
        //public static async Task SaveEntriesAsync(List<string> guessEntries)
        //{
        //    string filePath = Path.Combine(appData, "gameSettings.json");
        //    string jsonarray = JsonSerializer.Serialize(guessEntries);

        //    using (StreamWriter writer = new StreamWriter(filePath))
        //    {
        //        await writer.WriteAsync(jsonarray);
        //    }
        //}
        ////Rather than trying to load the previous state of the game, the game reads your previous inputs and tries them again essentially returning to the same position and when the app was closed.
        //public static async Task<List<string>> LoadEntriesAsync()
        //{
        //    var loadedEntries = new List<string>();
        //    string filePath = Path.Combine(appData, "gameSettings.json");

        //    if (System.IO.File.Exists(filePath))
        //    {
        //        try
        //        {
        //            using (StreamReader reader = new StreamReader(filePath))
        //            {
        //                string jsonstring = await reader.ReadToEndAsync();
        //                var entriesFromFile = JsonSerializer.Deserialize<List<String>>(jsonstring);

        //                if (entriesFromFile != null)
        //                {
        //                    loadedEntries.AddRange(entriesFromFile);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("Failed");
        //        }
        //    }
        //    return loadedEntries;
        //}

        //private static string selectedWordFilePath = Path.Combine(appData, "selectedWord.json");
        ////Saves word to be guessed, will be used for other game settings later
        //public static async Task SaveSelectedWordAsync(string selectedWord)
        //{
        //    try
        //    {
        //        string json = JsonSerializer.Serialize(selectedWord);
        //        using (StreamWriter writer = new StreamWriter(selectedWordFilePath))
        //        {
        //            await writer.WriteAsync(json);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error saving selected word");
        //    }

        //}
        ////Loads word to be guessed, will be used for other game settings later
        //public static async Task<string> LoadSelectedWordAsync()
        //{
        //    string filePath = Path.Combine(appData, "selectedWord.json");
        //    if (!System.IO.File.Exists(selectedWordFilePath))
        //    {
        //        Debug.WriteLine("Selected word file does not exist.");
        //        return "ERROR";
        //    }
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(selectedWordFilePath))
        //        {
        //            string json = await reader.ReadToEndAsync();

        //            string selectedWord = JsonSerializer.Deserialize<string>(json);

        //            return selectedWord;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error loading selected word");
        //    }
        //    return string.Empty;

        //}

        //private static string roundNumStringFilePath = Path.Combine(appData, "roundNumString.json");
        ////Saves word to be guessed, will be used for other game settings later
        //public static async Task SaveRoundNumAsync(string roundNumString)
        //{
        //    try
        //    {
        //        string json = JsonSerializer.Serialize(roundNumString);
        //        using (StreamWriter writer = new StreamWriter(roundNumStringFilePath))
        //        {
        //            await writer.WriteAsync(json);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error saving roundNum");
        //    }

        //}

        ////Loads word to be guessed, will be used for other game settings later
        //public static async Task<string> LoadRoundNumAsync()
        //{
        //    string filePath = Path.Combine(appData, "roundNumString.json");
        //    if (!System.IO.File.Exists(roundNumStringFilePath))
        //    {
        //        Debug.WriteLine("RoundNum file does not exist.");
        //        return "0";
        //    }
        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(roundNumStringFilePath))
        //        {
        //            string json = await reader.ReadToEndAsync();

        //            string roundNumString = JsonSerializer.Deserialize<string>(json);

        //            return roundNumString;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error loading RoundNum");
        //    }
        //    return string.Empty;

        //}

        //private static string historyArrFilePath = Path.Combine(appData, "historyArr.json");
        ////Saves word to be guessed, will be used for other game settings later
        //public static async Task SaveHistoryArrAsync(int[][] historyArr)
        //{
        //    try
        //    {
        //        string json = JsonSerializer.Serialize(historyArr);
        //        using (StreamWriter writer = new StreamWriter(historyArrFilePath))
        //        {
        //            await writer.WriteAsync(json);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error saving history array");
        //    }
        //}

        //public static async Task<int[][]> LoadHistoryArrAsync()
        //{
        //    if (!File.Exists(historyArrFilePath))
        //    {
        //        Debug.WriteLine("HistoryArr file does not exist.");
        //        return Array.Empty<int[]>();
        //    }

        //    try
        //    {
        //        string json = await File.ReadAllTextAsync(historyArrFilePath);
        //        return JsonSerializer.Deserialize<int[][]>(json) ?? Array.Empty<int[]>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error loading history array: " + ex.Message);
        //        return Array.Empty<int[]>();
        //    }
        //}

        //private static string correctGuessesFilePath = Path.Combine(appData, "correctGuesses.json");
        //// Saves correct guesses
        //public static async Task SaveCorrectGuessesAsync(List<string> correctGuesses)
        //{
        //    try
        //    {
        //        string json = JsonSerializer.Serialize(correctGuesses);
        //        using (StreamWriter writer = new StreamWriter(correctGuessesFilePath))
        //        {
        //            await writer.WriteAsync(json);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error saving correct guesses");
        //    }
        //}

        //// Loads correct guesses
        //public static async Task<List<string>> LoadCorrectGuessesAsync()
        //{
        //    var loadedCorrectGuesses = new List<string>();
        //    if (!File.Exists(correctGuessesFilePath))
        //    {
        //        Debug.WriteLine("Correct guesses file does not exist.");
        //        return loadedCorrectGuesses;
        //    }

        //    try
        //    {
        //        using (StreamReader reader = new StreamReader(correctGuessesFilePath))
        //        {
        //            string json = await reader.ReadToEndAsync();
        //            var correctGuessesFromFile = JsonSerializer.Deserialize<List<string>>(json);

        //            if (correctGuessesFromFile != null)
        //            {
        //                loadedCorrectGuesses.AddRange(correctGuessesFromFile);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Error loading correct guesses");
        //    }
        //    return loadedCorrectGuesses;
        //}
    }
}
