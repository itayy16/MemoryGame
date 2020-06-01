/**
 * Class that present the board of the game
 */

namespace MemoryGame
{
    internal class Board
    {
        private Cell<char>[,] m_Board;
        private const int k_RowDimention = 0;
        private const int k_ColDimention = 1;

        internal Board(int i_NumOfRows, int i_NumOfColumns)
        {
            m_Board = new Cell<char>[i_NumOfRows, i_NumOfColumns];

            for (int i = 0; i < i_NumOfRows; i++)
            {
                for (int j = 0; j < i_NumOfColumns; j++)
                {
                    m_Board[i, j] = new Cell<char>(' '); // defualt initialize
                }
            }
        }

        internal void SetCellValue(int i_RowIndex, int i_ColIndex, char i_CharToValue)
        {
            m_Board[i_RowIndex, i_ColIndex].ObjInCell = i_CharToValue;
        }

        internal char GetCellValue(int i_RowIndex, int i_ColIndex)
        {
            return m_Board[i_RowIndex, i_ColIndex].ObjInCell;
        }

        internal void SetBoolExpose(int i_RowIndex, int i_ColIndex, bool i_ExposeBool)
        {
            m_Board[i_RowIndex, i_ColIndex].IsExposed = i_ExposeBool;
        }

        internal bool GetBoolExpose(int i_RowIndex, int i_ColIndex)
        {
            return m_Board[i_RowIndex, i_ColIndex].IsExposed;
        }

        internal int GetNumberOfRows()
        {
            return m_Board.GetLength(k_RowDimention);
        }

        internal int GetNumberOfColumns()
        {
            return m_Board.GetLength(k_ColDimention);
        }

    }
}
