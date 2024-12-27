using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2A
{
    public class GameState //GameState class to hold game state data
    {
        public List<string> GuessEntries { get; set; } = new List<string>();
        public string SelectedWord { get; set; } = string.Empty;
        public string RoundNumString { get; set; } = "0";
        public string[][] HistoryArr { get; set; } = new string[30][];
        public List<string> CorrectGuesses { get; set; } = new List<string>();

        public string PlayerName { get; set; } = "Please enter player name";

        public int TotalGuesses { get; set; } = 0;
        public int ItemLCount { get; set; } = 0;
        public int ItemRCount { get; set; } = 0;
        public GameState()
        {
            for (int i = 0; i < 30; i++)
            {
                HistoryArr[i] = new string[6];
            }
        }
    }
}
