using System;
using System.Net.Sockets;
using System.Text;

namespace TicTacToeClient1
{
    class Program
    {
        static TcpClient client;
        static NetworkStream stream;
        static bool isGameOver = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Witaj w grze Kółko i Krzyżyk!");

            Console.Write("Podaj adres IP serwera: ");
            //string serverIP = Console.ReadLine();
            string serverIP = "192.168.0.216";

            client = new TcpClient(serverIP, 8888);
            stream = client.GetStream();

            Console.WriteLine("Połączono z serwerem. Czekanie na drugiego gracza...");

            // Odbieranie informacji o planszy
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string boardString = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            //Console.WriteLine("Aktualna plansza:");
            //Console.WriteLine(boardString);

            // Rozpoczęcie pętli gry
            while (!isGameOver)
            {
                // Odbieranie informacji o turze gracza
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Obsługa różnych komunikatów od serwera
                if (message.StartsWith("YourTurn"))
                {
                    Console.Write("Twoja kolej. Podaj numer pola (1-9): ");
                    int choice = GetPlayerChoice();

                    byte[] choiceData = Encoding.ASCII.GetBytes(choice.ToString());
                    stream.Write(choiceData, 0, choiceData.Length);
                }
                else if (message.StartsWith("OpponentTurn"))
                {
                    Console.WriteLine("Oczekiwanie na ruch przeciwnika...");
                }
                else if (message.StartsWith("Wygrywa gracz nr."))
                {
                    //Console.WriteLine(message);
                    isGameOver = true;
                    Console.WriteLine("Gra zakończona. Wynik: " + message);
                    break; // Zakończ pętlę gry
                }
                else if (message.StartsWith("Draw"))
                {
                    Console.WriteLine(message);
                    isGameOver = true;
                    Console.WriteLine("Gra zakończona. Wynik: " + message);
                    break; // Zakończ pętlę gry
                }
                else
                {
                    Console.WriteLine(message);

                    
                }
            }

            // Zakończenie połączenia
            //stream.Close();
            //client.Close();
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
