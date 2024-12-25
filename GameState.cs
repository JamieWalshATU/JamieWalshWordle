﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2A
{
    public class GameState
    {
        public List<string> GuessEntries { get; set; } = new List<string>();
        public string SelectedWord { get; set; } = string.Empty;
        public string RoundNumString { get; set; } = "0";
        public int[][] HistoryArr { get; set; } = new int[30][];
        public List<string> CorrectGuesses { get; set; } = new List<string>();
        public GameState()
        {
            for (int i = 0; i < 30; i++)
            {
                HistoryArr[i] = new int[6];
            }
        }
    }
}
