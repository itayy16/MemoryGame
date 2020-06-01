using System;
using System.Text;

namespace MemoryGame
{
    internal class MemoryGame
    {
        private bool m_AgainstComputer = true;
        private bool m_WantsToQuit = false;
        private string m_OpponentName = "Computer"; // defualt against computer
        private const int k_TwoSeconds = 2000; // using for Thread.sleep()

        private string m_PlayerName;
        private bool m_PlayerTurn;
        private int m_ScoreOfPlayer;
        private int m_ScoreOfOpponent;
        private int m_TotalScore;
        private bool m_IsFirstTurn;
        private MemoryGameLogic m_Game;

        // recieve player's name, who is the opponent and if the opponent is another player 
        // then recieve also his name
        internal void StartGame()
        {
            m_PlayerName = GameUserInterface.GetNameFromUser();
            // if true the game is between 2 players
            if (GameUserInterface.CheckOponent())
            {
                m_OpponentName = GameUserInterface.GetNameFromUser();
                m_AgainstComputer = !m_AgainstComputer;
            }

            gameFlow();
        }

        // the central method of the game which get from the user the size of the board to play
        // and the main loop that plays untill the game is over
        private void gameFlow()
        {
            initializeGame();
            int totalScoreToEnd = (m_Game.NumOfRows * m_Game.NumOfColumns) / 2;

            while (!m_WantsToQuit && m_TotalScore != totalScoreToEnd)
            {
                msgAtStartOfTurn();
                // check if we play against computer and now is turn to play
                int scoreOfThisTurn = (!m_PlayerTurn && m_AgainstComputer) ? computerTurn() : playOneTurn();
                if (m_WantsToQuit)
                {
                    break;
                }
                changeTurnIfWon(scoreOfThisTurn);
            }

            endGame();
        }

        // Play one turn
        // First choose a valid cell to expose, then choose another valid cell to expose
        // if the cells are equals return value of 1 else return 0
        // at any time that 'Q' is pressed the function stops its action
        private int playOneTurn()
        {
            string nameOfPlayer = m_PlayerTurn ? m_PlayerName : m_OpponentName;
            int retVal = 1;
            Tuple<int, int> firstChoice = GameUserInterface.PlayerTurn(nameOfPlayer, m_Game, ref m_WantsToQuit);

            if (!m_WantsToQuit)
            {
                m_Game.ExposeCell(firstChoice.Item1, firstChoice.Item2);
                clearAndPrint();
                Tuple<int, int> secondChoice = GameUserInterface.PlayerTurn(nameOfPlayer, m_Game, ref m_WantsToQuit);

                if (!m_WantsToQuit)
                {
                    m_Game.ExposeCell(secondChoice.Item1, secondChoice.Item2);
                    clearAndPrint();

                    if (!m_Game.CellsAreEquals(firstChoice.Item1, firstChoice.Item2, secondChoice.Item1, secondChoice.Item2))
                    {
                        retVal = 0;
                        m_Game.UnExposeCell(firstChoice.Item1, firstChoice.Item2);
                        m_Game.UnExposeCell(secondChoice.Item1, secondChoice.Item2);
                        System.Threading.Thread.Sleep(k_TwoSeconds);
                        clearAndPrint();
                    }
                }
            }

            return retVal;
        }

        // Play one turn
        // First choose a valid cell to expose, then using AI make the second choice smarter
        // if the cells are equals return value of 1 else return 0
        private int computerTurn()
        {
            Tuple<int, int> firstChoice = m_Game.ComputerFirstChoice();
            clearAndPrint();
            System.Threading.Thread.Sleep(k_TwoSeconds); // lets computer second choice look like it took a little time

            Tuple<int, int, int> retVal = m_Game.ComputerSecondChoice(firstChoice);
            clearAndPrint();

            // check the score of the current turn
            if (retVal.Item3 == 0)
            {
                m_Game.UnExposeCell(retVal.Item1, retVal.Item2);
                m_Game.UnExposeCell(firstChoice.Item1, firstChoice.Item2);
                System.Threading.Thread.Sleep(k_TwoSeconds);
                clearAndPrint();
            }

            return retVal.Item3;
        }

        private void endGame()
        {
            if (m_WantsToQuit)
            {
                Console.WriteLine("You have given up, exiting the game.");
                System.Threading.Thread.Sleep(k_TwoSeconds);
            }
            else
            {
                StringBuilder msgToPrint = new StringBuilder(string.Format(@"The game is over, the results are:
{0}'s score is: {1}
{2}'s score is: {3}", m_PlayerName, m_ScoreOfPlayer, m_OpponentName, m_ScoreOfOpponent));
                msgToPrint.Append(Environment.NewLine);

                if (m_ScoreOfPlayer > m_ScoreOfOpponent)
                {
                    msgToPrint.Append(string.Format("The winner is {0}", m_PlayerName));
                }
                else if (m_ScoreOfPlayer < m_ScoreOfOpponent)
                {
                    msgToPrint.Append(string.Format("The winner is {0}", m_OpponentName));
                }
                else
                {
                    msgToPrint.Append("You managed to score the same points, It's a tie!");
                }

                msgToPrint.Append(Environment.NewLine);
                bool wantToPlayAgain = GameUserInterface.PrintRoundFinalMsg(msgToPrint);

                if (wantToPlayAgain)
                {
                    gameFlow();
                }
            }
        }

        // initialize game score and give the turn to player for next round
        private void initializeGame()
        {
            m_PlayerTurn = true;
            m_IsFirstTurn = true;
            m_ScoreOfOpponent = 0;
            m_ScoreOfPlayer = 0;
            m_TotalScore = 0;
            // this part is in charge of getting the board size from the user
            int numOfRows = 0;
            int numOfColumns = 0;
            GameUserInterface.SetBoardSize(ref numOfRows, ref numOfColumns);
            m_Game = new MemoryGameLogic(numOfRows, numOfColumns, m_AgainstComputer);
            m_Game.GenerateRandomizedBoard();
            clearAndPrint();
        }

        private void msgAtStartOfTurn()
        {
            if (!m_PlayerTurn && m_AgainstComputer)
            {
                GameUserInterface.PrintMsgOnceTurn(m_OpponentName);
                System.Threading.Thread.Sleep(k_TwoSeconds);
            }
            else if (m_PlayerTurn && m_IsFirstTurn)
            {
                GameUserInterface.PrintMsgOnceTurn(m_PlayerName);
                m_IsFirstTurn = !m_IsFirstTurn;
            }
            else if (!m_PlayerTurn && m_IsFirstTurn)
            {
                GameUserInterface.PrintMsgOnceTurn(m_OpponentName);
                m_IsFirstTurn = !m_IsFirstTurn;
            }
            else
            {
                Console.WriteLine("Great guess, you have another turn.");
            }
        }

        // if one of the players managed to reveal a pair, he play once again
        // otherwise the turn move to the other player
        private void changeTurnIfWon(int i_Score)
        {
            if (i_Score == 0)
            {
                m_PlayerTurn = !m_PlayerTurn;
                m_IsFirstTurn = true;
            }
            else
            {
                if (m_PlayerTurn)
                {
                    m_ScoreOfPlayer++;
                }
                else
                {
                    m_ScoreOfOpponent++;
                }

                m_TotalScore++;
            }
        }

        private void clearAndPrint()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            GameUserInterface.printBoardToScreen(m_Game.GetBoard);
        }

    }
}
