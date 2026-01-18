using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class GameSaveData
    {
        public int Columns;
        public int Rows;
        public List<CardSaveData> Cards = new List<CardSaveData>();
        public int Score;
        public bool WasSuccessfulMatch;
    }
}