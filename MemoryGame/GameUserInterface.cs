using System;
using System.Text;

namespace MemoryGame
{
    internal class GameUserInterface
    {
        private const char k_Player = 'P'; // stands for Player
        private const char k_Computer = 'C'; // stands for Computer
        private const byte k_MaxLengthOfBoard = 6; // max size of columns or rows
        private const byte k_MinLengthOfBoard = 4; // min size of columns or rows
        private const char k_FirstRowIndex = '1'; // present the first row index
        private const char k_FirstColumnIndex = 'A'; // present the first column index

        // retunrs a string that represnents the name of the player
        internal static string GetNameFromUser()
        {
            Console.WriteLine("Please enter your name:");
            string retVal = Console.ReadLine();

            return retVal;
        }

        // ask the player against who he want to play
        // return true if the choice is player
        // return flase if the choice is computer
        internal static bool CheckOponent()
        {
            Console.WriteLine("Against who you want to play? (C \\ P) C - Computer , P - Another player");
            string answer = Console.ReadLine().ToUpper();

            while (answer.Length != 1 || (answer[0] != k_Player && answer[0] != k_Computer))
            {
                Console.WriteLine("Invalid input please enter again: (C \\ P) ");
                answer = Console.ReadLine().ToUpper();
            }

            return (answer[0] == k_Player);
        }

        // reads 2 numbers from the user that represent the size of the board, 
        // checks if the 2 numbers multiplication is even (valid board)
        // receive 2 output variables that contain the final valid number of rows and columns
        internal static void SetBoardSize(ref int o_RowsOfBoard, ref int o_ColumnsOfBoard)
        {
            Console.WriteLine(string.Format("Please enter the number of rows: ({0} - {1})", k_MinLengthOfBoard, k_MaxLengthOfBoard));
            int numOfRows = numberInRange(getNumbeFromUser());

            Console.WriteLine(string.Format("Please enter the number of columns: ({0} - {1})", k_MinLengthOfBoard, k_MaxLengthOfBoard));
            int numOfColumns = numberInRange(getNumbeFromUser());

            while ((numOfRows * numOfColumns) % 2 == 1)
            {
                Console.WriteLine("Invalid size, board size must be even please enter another number");
                numOfColumns = numberInRange(getNumbeFromUser());
            }

            o_RowsOfBoard = numOfRows;
            o_ColumnsOfBoard = numOfColumns;
        }

        private static int numberInRange(int i_NumToCheck)
        {
            while (i_NumToCheck < k_MinLengthOfBoard || i_NumToCheck > k_MaxLengthOfBoard)
            {
                Console.WriteLine(string.Format("Invalid size, please enter a number between {0} - {1}", k_MinLengthOfBoard, k_MaxLengthOfBoard));
                i_NumToCheck = getNumbeFromUser();
            }

            return i_NumToCheck;
        }

        private static int getNumbeFromUser()
        {
            string answer = Console.ReadLine();
            int retVal = 0;

            while (!int.TryParse(answer, out retVal))
            {
                Console.WriteLine("Invalid input, your input is not a number please try again");
                answer = Console.ReadLine();
            }

            return retVal;
        }

        // given a board, prints the board in the required format as needed.  
        internal static void printBoardToScreen(Board i_Board)
        {
            StringBuilder boardToPrint = new StringBuilder();
            int rowLength = i_Board.GetNumberOfRows();
            int colLength = i_Board.GetNumberOfColumns();
            // prints the first row
            for (int i = 0; i <= colLength; i++)
            {
                if (i == 0)
                {
                    boardToPrint.Append("    ");
                }
                else
                {
                    boardToPrint.Append((char)(k_FirstColumnIndex + i - 1));
                    boardToPrint.Append("   ");
                }
            }

            helperPrinter(colLength, ref boardToPrint);

            for (int i = 0; i < rowLength; i++)
            {
                boardToPrint.Append((char)(k_FirstRowIndex + i));
                boardToPrint.Append(" | ");
                for (int j = 0; j < colLength; j++)
                {
                    string strToAppend = i_Board.GetBoolExpose(i, j) ? i_Board.GetCellValue(i, j).ToString() : (" ");
                    boardToPrint.Append(strToAppend);
                    boardToPrint.Append(" | ");
                }

                helperPrinter(colLength, ref boardToPrint);
            }

            Console.Write(boardToPrint);
        }

        // a helper function that prints a line of "====" coorsponding to the size of the line
        private static void helperPrinter(int i_NumOfEquals, ref StringBuilder io_StrToDraw)
        {
            io_StrToDraw.Append(Environment.NewLine);

            for (int i = 0; i <= i_NumOfEquals; i++)
            {
                if (i == 0)
                {
                    io_StrToDraw.Append("  ");
                }
                else
                {
                    io_StrToDraw.Append("====");
                }
            }

            io_StrToDraw.Append("=");
            io_StrToDraw.Append(Environment.NewLine);
        }

        // interacts with the player and require from the player a legal input of a cell to reveal
        // handle illegal input such as, out of bound of the board, a cell that is already exposed
        internal static Tuple<int, int> PlayerTurn(string i_NameOfPlayer, MemoryGameLogic i_Game, ref bool io_WantsToQuit)
        {
            string answerToCheck = Console.ReadLine();
            io_WantsToQuit = answerToCheck.Equals("Q");
            bool isValidInput = i_Game.IsValidInputCell(answerToCheck);
            bool isValidRange = isValidInput && i_Game.IsValidRangeForCell(answerToCheck);
            bool isValidCell = isValidRange && i_Game.IsValidCell(answerToCheck);

            while (!io_WantsToQuit && (!isValidInput || !isValidRange || !isValidCell))
            {
                if (io_WantsToQuit)
                {
                    break;
                }
                else
                {
                    if (!isValidInput)
                    {
                        Console.WriteLine("Invalid input, please try again");
                    }
                    else if (!isValidRange)
                    {
                        Console.WriteLine("Invalid range input, please try again");
                    }
                    else if (!isValidCell)
                    {
                        Console.WriteLine("The cell you choose is already exposed, please try another one");
                    }

                    answerToCheck = Console.ReadLine();
                    io_WantsToQuit = answerToCheck.Equals("Q");
                    isValidInput = i_Game.IsValidInputCell(answerToCheck);
                    isValidRange = isValidInput && i_Game.IsValidRangeForCell(answerToCheck);
                    isValidCell = isValidRange && i_Game.IsValidCell(answerToCheck);
                }
            }
            // default value won't be used if io_WantToQuit = true
            Tuple<int, int> retPair = new Tuple<int, int>(0, 0);
            if (!io_WantsToQuit)
            {
                retPair = new Tuple<int, int>(answerToCheck[1] - k_FirstRowIndex, answerToCheck[0] - k_FirstColumnIndex);
            }

            return retPair;
        }

        internal static void PrintMsgOnceTurn(string i_NameOfPlayer)
        {
            StringBuilder msg = new StringBuilder(i_NameOfPlayer);

            if (i_NameOfPlayer.Equals("Computer"))
            {
                msg.Append(" is Playing...");
            }
            else
            {
                msg.Append("'s turn, you can beat him!");
            }

            Console.WriteLine(msg);
        }

        internal static bool PrintRoundFinalMsg(StringBuilder i_Msg)
        {
            bool retVal;
            i_Msg.Append("Do you want to play again? ( Y \\ N )");
            Console.WriteLine(i_Msg); // print final result of the round and ask the play if he wants to keep playing or not

            string answer = Console.ReadLine();     // get input from user, if he wants to continue to play the game or not

            while (!answer.Equals("Y") && !answer.Equals("N"))
            {
                Console.WriteLine("Invalid input, please enter (Y \\ N) ");
                answer = Console.ReadLine();
            }

            retVal = answer.Equals("Y");

            if (retVal)
            {
                Ex02.ConsoleUtils.Screen.Clear();
                Console.WriteLine("Good choice, starting new game...");
            }
            else
            {
                Console.WriteLine("Goodbye see you next time, exiting the game...");
            }

            System.Threading.Thread.Sleep(1500);

            return retVal;
        }
    }
}
