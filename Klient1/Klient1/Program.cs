using System;
using System.Net.Sockets;
using System.Text;

namespace TicTacToeClient1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Witaj w grze Kółko i Krzyżyk! Jesteś Graczem 2.");

            Console.Write("Podaj adres IP serwera: ");
            string serverIP = Console.ReadLine();

            TcpClient client = new TcpClient(serverIP, 8888);
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string boardString = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            Console.WriteLine("Aktualna plansza:");
            Console.WriteLine(boardString);

            bool isGameOver = false;
            while (!isGameOver)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if (message.StartsWith("YourTurn"))
                {
                    Console.WriteLine("Twoja kolej. Podaj numer pola (1-9): ");
                    int choice = GetPlayerChoice();

                    byte[] choiceData = Encoding.ASCII.GetBytes(choice.ToString());
                    stream.Write(choiceData, 0, choiceData.Length);
                }
                else if (message.StartsWith("OpponentTurn"))
                {
                    Console.WriteLine("Oczekiwanie na ruch przeciwnika...");
                }
                else
                {
                    Console.WriteLine(message);

                    if (message.Contains("Win") || message.Contains("Draw"))
                    {
                        isGameOver = true;
                        Console.WriteLine("Gra zakończona. Wynik: " + message);
                    }
                }
            }

            client.Close();
        }

        static int GetPlayerChoice()
        {
            int choice = 0;
            bool isValidChoice = false;
            while (!isValidChoice)
            {
                string input = Console.ReadLine();

                if (int.TryParse(input, out choice))
                {
                    if (choice >= 1 && choice <= 9)
                    {
                        isValidChoice = true;
                    }
                    else
                    {
                        Console.WriteLine("Podano niepoprawny numer pola. Wpisz liczbę od 1 do 9.");
                    }
                }
                else
                {
                    Console.WriteLine("Podano niepoprawną wartość. Wpisz liczbę od 1 do 9.");
                }
            }
            return choice;
        }
    }
}
