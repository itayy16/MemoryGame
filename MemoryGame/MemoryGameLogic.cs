using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryGame
{
    internal class MemoryGameLogic
    {
        private Board m_Board;
        private int m_NumOfRows;
        private int m_NumOfColumns;
        private readonly bool v_Expose = true;
        private readonly bool v_UnExpose = false;
        private Random m_Random;
        private Dictionary<Tuple<int, int>, char> m_UnExposeCells; // using for computer moves
        private Dictionary<Tuple<int, int>, char> m_AIMemory; // using for remember cells
        private bool m_IsComputerPlaying;

        // constructor of MemoryGameLogic, initialize MemoryBoard and number of rows and columns
        internal MemoryGameLogic(int i_NumOfRows, int i_NumOfColumns, bool i_IsComputerPlaying)
        {
            m_Board = new Board(i_NumOfRows, i_NumOfColumns);
            m_NumOfRows = i_NumOfRows;
            m_NumOfColumns = i_NumOfColumns;
            m_Random = new Random();
            m_IsComputerPlaying = i_IsComputerPlaying;

            if (i_IsComputerPlaying)
            {
                m_UnExposeCells = new Dictionary<Tuple<int, int>, char>();
                m_AIMemory = new Dictionary<Tuple<int, int>, char>();
            }
        }

        // generate random board by using Fisher-Yates alogrithm
        internal void GenerateRandomizedBoard()
        {
            char[] valuesOfBoards = new char[(m_NumOfRows * m_NumOfColumns)];
            char currentValue = 'A';

            for (int i = 0; i < valuesOfBoards.Length; i += 2)
            {
                valuesOfBoards[i] = currentValue;
                valuesOfBoards[i + 1] = currentValue;
                currentValue++;
            }
            // reshuffle the order of the board
            for (int i = 0; i < m_NumOfRows; i++)
            {
                for (int j = 0; j < m_NumOfColumns; j++)
                {
                    int generatedNumber = m_Random.Next((i * m_NumOfColumns) + j, valuesOfBoards.Length);
                    m_Board.SetCellValue(i, j, valuesOfBoards[generatedNumber]);

                    // intizlied the list of UnExposedCell for computer turn
                    if (m_IsComputerPlaying)
                    {
                        addElementToDict(i, j, valuesOfBoards[generatedNumber], v_Expose);
                    }

                    // swap
                    char tempSwapChar = valuesOfBoards[(i * m_NumOfColumns) + j];
                    valuesOfBoards[(i * m_NumOfColumns) + j] = valuesOfBoards[generatedNumber];
                    valuesOfBoards[generatedNumber] = tempSwapChar;
                }
            }
        }

        // if i_Flag is true add the element to m_UnExposeCells else add to m_AIMemory
        private void addElementToDict(int i_IndexRow, int i_IndexCol, char i_Value, bool i_Flag)
        {
            Tuple<int, int> item = new Tuple<int, int>(i_IndexRow, i_IndexCol);

            if (i_Flag)
            {
                m_UnExposeCells.Add(item, i_Value);
            }
            else
            {
                if (!m_AIMemory.ContainsKey(item))
                {
                    m_AIMemory.Add(item, i_Value);
                }
            }
        }

        // remove element from list
        private void removeElementFromDict(int i_IndexRow, int i_IndexCol)
        {
            if (m_IsComputerPlaying)
            {
                Tuple<int, int> item = new Tuple<int, int>(i_IndexRow, i_IndexCol);
                m_UnExposeCells.Remove(item);
            }
        }

        // return the object MemoryBoard
        internal Board GetBoard
        {
            get { return m_Board; }
            set { }
        }

        // checks if the cell is not exposed
        internal bool IsValidCell(string i_StrToCheck)
        {
            int rowIndex = i_StrToCheck[1] - '1';
            int colIndex = i_StrToCheck[0] - 'A';

            return !m_Board.GetBoolExpose(rowIndex, colIndex);
        }

        // checks if the given string is in valid range 
        internal bool IsValidRangeForCell(string i_StrToCheck)
        {
            int rowIndex = i_StrToCheck[1] - '1';
            int colIndex = i_StrToCheck[0] - 'A';
            bool retVal = (rowIndex >= 0 && rowIndex < m_NumOfRows);
            retVal &= (colIndex >= 0 && colIndex < m_NumOfColumns);

            return retVal;
        }

        // checks if the given string is valid format of choice cell
        internal bool IsValidInputCell(string i_StrToCheck)
        {
            bool retVal = i_StrToCheck.Length == 2;
            if (retVal)
            {
                retVal = char.IsUpper(i_StrToCheck[0]);
                retVal &= char.IsDigit(i_StrToCheck[1]);
            }

            return retVal;
        }

        // set the boolean field of the current cell to be true
        internal void ExposeCell(int i_RowIndex, int i_ColIndex)
        {
            m_Board.SetBoolExpose(i_RowIndex, i_ColIndex, v_Expose);

            if (m_IsComputerPlaying)
            {
                char value = m_Board.GetCellValue(i_RowIndex, i_ColIndex);
                addElementToDict(i_RowIndex, i_ColIndex, value, v_UnExpose);
                removeElementFromDict(i_RowIndex, i_ColIndex);
            }
        }

        // set the boolean field of the current cell to be false
        internal void UnExposeCell(int i_RowIndex, int i_ColIndex)
        {
            if (m_IsComputerPlaying)
            {
                if (!m_UnExposeCells.ContainsKey(new Tuple<int, int>(i_RowIndex, i_ColIndex)))
                {
                    addElementToDict(i_RowIndex, i_ColIndex, m_Board.GetCellValue(i_RowIndex, i_ColIndex), v_Expose);
                }
            }
            m_Board.SetBoolExpose(i_RowIndex, i_ColIndex, v_UnExpose);
        }

        // return true if the value in given cells is equal, false otherwise
        internal bool CellsAreEquals(int i_RowIndex1, int i_ColIndex1, int i_RowIndex2, int i_ColIndex2)
        {
            bool retVal = m_Board.GetCellValue(i_RowIndex1, i_ColIndex1) == m_Board.GetCellValue(i_RowIndex2, i_ColIndex2);

            // if retVal = true and we play against computer, update the relevant dict
            if (retVal && m_IsComputerPlaying)
            {
                removeElementFromDict(i_RowIndex1, i_ColIndex1);
                removeElementFromDict(i_RowIndex2, i_ColIndex2);
            }

            return retVal;
        }

        internal Tuple<int, int> ComputerFirstChoice()
        {
            int generatedNum = m_Random.Next(m_UnExposeCells.Count());
            Tuple<int, int> keyOfFirst = m_UnExposeCells.ElementAt(generatedNum).Key;
            this.ExposeCell(keyOfFirst.Item1, keyOfFirst.Item2);

            return keyOfFirst;
        }

        // return value of tuple, item1 = rowindex , item2= colindex , iten3 = score
        internal Tuple<int, int, int> ComputerSecondChoice(Tuple<int, int> i_Item)
        {
            int scoreForMatch = 0;
            Tuple<int, int, int> returnTuple = new Tuple<int, int, int>(0, 0, scoreForMatch);
            char valueToCheck = '0'; // default value

            if (m_AIMemory.TryGetValue(i_Item, out valueToCheck))
            {
                m_AIMemory.Remove(i_Item);
            }

            if (m_AIMemory.ContainsValue(valueToCheck))
            {
                // scan for valueToCheck in the AIMemory Dict
                foreach (var itemOfDict in m_AIMemory)
                {
                    if (valueToCheck == itemOfDict.Value)
                    {
                        Tuple<int, int> pairOfIndex = itemOfDict.Key;
                        ExposeCell(pairOfIndex.Item1, pairOfIndex.Item2);
                        scoreForMatch++;
                        removeElementFromDict(pairOfIndex.Item1, pairOfIndex.Item2);
                        removeElementFromDict(i_Item.Item1, i_Item.Item2);
                        returnTuple = new Tuple<int, int, int>(0, 0, scoreForMatch);
                        break;
                    }
                }
            }
            else
            {
                m_AIMemory.Add(i_Item, valueToCheck);
                int generatedNum = m_Random.Next(m_UnExposeCells.Count());
                Tuple<int, int> keyOfFirst = m_UnExposeCells.ElementAt(generatedNum).Key;
                this.ExposeCell(keyOfFirst.Item1, keyOfFirst.Item2);
                if (CellsAreEquals(i_Item.Item1, i_Item.Item2, keyOfFirst.Item1, keyOfFirst.Item2))
                {
                    scoreForMatch++;
                    returnTuple = new Tuple<int, int, int>(0, 0, scoreForMatch);
                }
                else
                {
                    addElementToDict(i_Item.Item1, i_Item.Item2, valueToCheck, v_Expose);
                    returnTuple = new Tuple<int, int, int>(keyOfFirst.Item1, keyOfFirst.Item2, scoreForMatch);
                }
            }

            return returnTuple;
        }

        internal int NumOfRows
        {
            get { return m_NumOfRows; }
            set { m_NumOfRows = value; }
        }
        internal int NumOfColumns
        {
            get { return m_NumOfColumns; }
            set { m_NumOfColumns = value; }
        }
    }
}
