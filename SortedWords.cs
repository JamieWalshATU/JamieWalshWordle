﻿using System.Diagnostics;

namespace JamieWalshWordle
{
    //SortedWords creates a collection of words and shuffles them
    public class SortedWords
    {
        //Creates a list to hold sorted words
        private List<string> _wordListSorted = new List<string>(); //List to hold sorted words
        public List<string> WordListSorted 
        {
            get => _wordListSorted;
            set => _wordListSorted = value;
        }
        private static SortedWords _instance; //Singleton instance of SortedWords class to avoid multiple instances of the class,   
        public static SortedWords GetInstance(WordViewModel wordViewModel) 
        {
            if (_instance == null)
            {
                _instance = new SortedWords(wordViewModel);
            }
            return _instance;
        }

        private readonly WordViewModel _wordViewModel;

        public SortedWords(WordViewModel wordViewModel)
        {
            _wordViewModel = wordViewModel;
        }

        //Sorts words and creates a subset of 30
        public void SortWords()
        {
            if (_wordViewModel.WordList != null && _wordViewModel.WordList.Count > 0)
            {

                Random random = new Random();
                List<string> selectedWords = new List<string>();
                //Copy of list to avoid modifications
                var wordListCopy = new List<string>(_wordViewModel.WordList);

                //Selects 30 words
                for (int i = 0; i < 30 && wordListCopy.Count > 0; i++)
                {
                    int randomIndex = random.Next(wordListCopy.Count);
                    selectedWords.Add(wordListCopy[randomIndex]);
                    wordListCopy.RemoveAt(randomIndex);
                }
                //Sorts words alphabetically, this is helpful for when debuging but is not nessecary
                selectedWords.Sort(StringComparer.Ordinal);
                WordListSorted = selectedWords;
                Debug.WriteLine($"Sorted {WordListSorted.Count} words.");
            }
            else
            {
                Debug.WriteLine("WordList is empty or null.");
            }
        }
        public async Task<List<string>> MultiplayerWords()
        {
            Random random = new Random();
            List<string> multiplayerWords = new List<string>();
            var multiplayerListCopy = new List<string>(_wordViewModel.WordList);

            //Selects 30 words
            for (int i = 0; i < 30 && multiplayerListCopy.Count > 0; i++)
            {
                int randomIndex = random.Next(multiplayerListCopy.Count);
                multiplayerWords.Add(multiplayerListCopy[randomIndex]);
                multiplayerListCopy.RemoveAt(randomIndex);
            }
            return multiplayerWords;

        }
        public bool Remove(string word)
        {
            return WordListSorted.Remove(word); // Removes the word from the list
        }
    }
}

