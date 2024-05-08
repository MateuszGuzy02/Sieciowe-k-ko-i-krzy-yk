using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace TicTacToe
{
    class Program
    {
        static char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static TcpListener listener;
        static TcpClient player1, player2;
        static NetworkStream stream1, stream2;
        static int currentPlayer = 1;

        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            Console.WriteLine("Oczekiwanie na graczy...");

            player1 = listener.AcceptTcpClient();
            Console.WriteLine("Gracz 1 dołączył.");
            SendTurn(player1.GetStream(), true);


            player2 = listener.AcceptTcpClient();
            Console.WriteLine("Gracz 2 dołączył.");
            SendTurn(player2.GetStream(), false);

            stream1 = player1.GetStream();
            stream2 = player2.GetStream();

            SendBoard();

            while (true)
            {
                HandlePlayerTurn();
                if (CheckWin())
                {
                    SendWinMessage(currentPlayer);
                    break;
                }
                SwapPlayers();
            }

            player1.Close();
            player2.Close();
            listener.Stop();
        }

        static void SendTurn(NetworkStream stream, bool isYourTurn)
        {
            byte[] turnData = Encoding.ASCII.GetBytes(isYourTurn ? "YourTurn" : "OpponentTurn");
            stream.Write(turnData, 0, turnData.Length);
        }

        static void SendBoard()
        {
            string boardString = string.Format(" {0} | {1} | {2} \n___|___|___\n {3} | {4} | {5} \n___|___|___\n {6} | {7} | {8} \n   |   |   ",
                board[0], board[1], board[2], board[3], board[4], board[5], board[6], board[7], board[8]);

            byte[] boardData = Encoding.ASCII.GetBytes(boardString);

            stream1.Write(boardData, 0, boardData.Length);
            stream2.Write(boardData, 0, boardData.Length);
        }

        static void HandlePlayerTurn()
        {
            NetworkStream currentPlayerStream = (currentPlayer == 1) ? stream1 : stream2;
            NetworkStream otherPlayerStream = (currentPlayer == 1) ? stream2 : stream1;

            SendTurn(currentPlayerStream, true);
            SendTurn(otherPlayerStream, false);

            byte[] buffer = new byte[1];
            currentPlayerStream.Read(buffer, 0, buffer.Length);
            int choice = int.Parse(Encoding.ASCII.GetString(buffer)) - 1;

            if (board[choice] != 'X' && board[choice] != 'O')
            {
                board[choice] = (currentPlayer == 1) ? 'X' : 'O';
                SendBoard();
            }
            else
            {
                byte[] error = Encoding.ASCII.GetBytes("Error");
                currentPlayerStream.Write(error, 0, error.Length);
            }
        }

        static bool CheckWin()
        {
            // Sprawdź wiersze, kolumny i przekątne
            for (int i = 0; i < 3; i++)
            {
                // Sprawdź wiersze
                if (board[i * 3] == board[i * 3 + 1] && board[i * 3 + 1] == board[i * 3 + 2])
                {
                    SendWinMessage(currentPlayer);
                    return true;
                }

                // Sprawdź kolumny
                if (board[i] == board[i + 3] && board[i + 3] == board[i + 6])
                {
                    SendWinMessage(currentPlayer);
                    return true;
                }
            }

            // Sprawdź przekątne
            if ((board[0] == board[4] && board[4] == board[8]) || (board[2] == board[4] && board[4] == board[6]))
            {
                SendWinMessage(currentPlayer);
                return true;
            }

            // Sprawdź remis
            if (board[0] != '1' && board[1] != '2' && board[2] != '3' && board[3] != '4' && board[4] != '5' &&
                board[5] != '6' && board[6] != '7' && board[7] != '8' && board[8] != '9')
            {
                SendDrawMessage();
                return true;
            }

            return false;
        }

        static void SendWinMessage(int player)
        {
            string winMessage = "Wygrywa gracz nr. " + player;
            byte[] winData = Encoding.ASCII.GetBytes(winMessage);

            stream1.Write(winData, 0, winData.Length);
            stream2.Write(winData, 0, winData.Length);
        }

        static void SendDrawMessage()
        {
            byte[] drawData = Encoding.ASCII.GetBytes("Remis");
            stream1.Write(drawData, 0, drawData.Length);
            stream2.Write(drawData, 0, drawData.Length);
        }

        static void SwapPlayers()
        {
            currentPlayer = (currentPlayer == 1) ? 2 : 1;
        }
    }
}
