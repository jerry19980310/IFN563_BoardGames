﻿using System.Drawing;
using System.Xml.Linq;
using static BoardGames.Program;

namespace BoardGames
{
    public abstract class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool State { get; set; }
        public Piece Piece { get; set; }
        public Move CurrentMove { get; set; }

        public abstract void MakeMoveForTreblecross(int size);

        public abstract void ConfirmMove();

        public void InitializeState()
        {
            State = false;
        }

    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(int id, string name)
        {
            ID = id;
            Name = name;
            Type = "Human";
            State = false;

        }

        public override void ConfirmMove()
        {
            bool state = PromptYesNo("Are you sure to make this move? ");

            if (state)
            {
                State = true;
            }

            else State = false;
        }

        public override void MakeMoveForTreblecross(int size)
        {

            bool isValid;
            int position;

            do
            {
                Console.Write("Enter a position number to place the piece. Player #{0}: ", ID);

                position = PromptForInt();

                if (position <= size && position > 0) isValid = true;

                else
                {
                    isValid = false;
                    Console.WriteLine("Invalid, must less or equal than {0}!!!", size);
                }

            }
            while (!isValid); // keep looping until a valid number is entered

            CurrentMove = new Move(position - 1, 0);



        }
    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer(int id, string name)
        {
            ID = id;
            Name = name;
            Type = "Computer";
            State = false;

        }

        public override void MakeMoveForTreblecross(int size)
        {
            Random rand = new Random();

            int position = rand.Next(size);

            CurrentMove = new Move(position, 0);


        }

        public override void ConfirmMove()
        {
            State = true;
        }


    }

    public class Game
    {
        public Player[] Players;
        public BoardGame BoardGame;


        public Player WhosTurn()
        {

            foreach (var player in Players)
            {
                if (player.State == false)
                {
                    return player;
                }

            }

            return null;

        }


    }

    public abstract class BoardGame
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Piece BoardPiece;
        //public int size { get; set; }
        public string[,] Board { get; set; }

        public abstract void Initialize();

        public abstract bool HasWinner();

        public void PrintBoard()
        {
            string piece;

            for (int i = 0; i < Board.GetLength(0) * 2 + 1; i++)
            {

                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    if ((i % 2) == 0 || i == (Board.GetLength(0) * 2 + 1))
                    {
                        Console.Write("++");
                    }

                    else
                    {
                        if (Board[i / 2, j] is null)
                        {
                            piece = " ";

                        }

                        else piece = Board[i / 2, j];

                        Console.Write("|{0}", piece);

                        if (j == (Board.GetLength(1) - 1))
                        {
                            Console.Write("|");
                        }
                    }

                }

                if ((i % 2) == 0 || i == (Board.GetLength(0) * 2 + 1))
                {
                    Console.WriteLine("+");
                }
                else Console.WriteLine("");

            }

        }

        public bool CheckSquare(int col, int row)
        {
            if (Board[row, col] is null)
                return true;
            else return false;
        }

        public void PlacePiece(int col, int row)
        {
            Board[row, col] = BoardPiece.Name;
        }

    }

    public class Treblecross : BoardGame
    {
        public override void Initialize()
        {
            const int MAX_LENGTH = 1;
            const int MIN_SIZE = 3;

            bool isValid;
            int size;

            do
            {
                Console.Write("Enter a number to decide board size (must greater than {0}): ", MIN_SIZE);

                size = PromptForInt();

                if (size >= MIN_SIZE) isValid = true;

                else
                {
                    isValid = false;
                    Console.WriteLine("Invalid, must greater than {0}: ", MIN_SIZE);
                }

            }
            while (!isValid); // keep looping until a valid number is entered

            ID = 1;
            Name = "Treblecross";
            Piece piece1 = new Piece(1, "X");
            BoardPiece = piece1;
            Board = new string[MAX_LENGTH, size];

        }

        public override bool HasWinner()
        {
            bool hasWinner = false;

            for (int i = 0; i < Board.GetLength(1) - 2; i++)
            {
                if (Board[0, i] == BoardPiece.Name && Board[0, i + 1] == BoardPiece.Name && Board[0, i + 2] == BoardPiece.Name)
                {
                    hasWinner = true;
                    break;
                }
            }

            return hasWinner;
        }
    }

    //public class Reversi : BoardGame
    //{
    //    public override void Initialize()
    //    {

    //    }
    //}

    public class Piece
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public Piece(int id, string name)
        {
            ID = id;
            Name = name;

        }

    }

    public class Move
    {
        public int Col { get; set; }
        public int Row { get; set; }

        //public Move() { };

        public Move(int col, int row)
        {
            Col = col;
            Row = row;
        }


    }


    internal class Program
    {
        
        public static int SelectGame()
        {

            bool isValid;
            int gameID;
            do
            {
                Console.Write("Enter a game code => 1. Treblecross 2.Reversl: ");
                gameID = PromptForInt();
                if (gameID == 1 || gameID == 2) isValid = true;

                else isValid = false;

                if (!isValid)
                    Console.WriteLine("Invalid, please check!!!");
            }
            while (!isValid); // keep looping until a valid number is entered

            return gameID;

        }

        public static void StartNewGame()
        {

            const int PLAYERNUMBER = 2;
            Game game = new Game();
            game.Players = new Player[PLAYERNUMBER];

            int gameID = SelectGame();

                    if (gameID == 1)
                    {
                        game.BoardGame = new Treblecross();

                        game.BoardGame.Initialize();

                    }
                    else
                    {
                        Console.WriteLine("Not complete yet!!!");
                        return;
                    }

                    game.Players[0] = CreateHumanPlayer(1);

                    string type = SelectPlayerType();

                    if (type == "H") game.Players[1] = CreateHumanPlayer(2);
                    else if (type == "C") game.Players[1] = CreateComputerPlayer(2);

                    game.BoardGame.PrintBoard();

                    //start game

                    bool isValid;

                    bool winner = false;

                    

                    while (!winner)
                    {
                        Player currentplayer = game.WhosTurn();

                        int currentID = currentplayer.ID - 1;
                        
                        while (!game.Players[currentID].State)
                        {
                            do
                            {
                                do
                                {
                                    game.Players[currentID].MakeMoveForTreblecross(game.BoardGame.Board.GetLength(1));
                                    isValid = game.BoardGame.CheckSquare(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);
                                    if (!isValid) Console.WriteLine("Cannot place here");

                                } while (!isValid);

                                game.Players[currentID].ConfirmMove();
                                //playermenu()

                            } while (!game.Players[currentID].State);

                            int count = 0;

                            foreach (var player in game.Players)
                            {

                                if (player.State == true)
                                {
                                    count++;
                                }

                            }

                            if (count == 2)
                            {
                                foreach (var player in game.Players)
                                {
                                    player.InitializeState();
                                }
                            }

                            game.BoardGame.PlacePiece(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);

                            game.BoardGame.PrintBoard();

                            winner = game.BoardGame.HasWinner();

                            if (winner)
                            {
                                Console.WriteLine("Player #{0}: {1} win !!!", game.Players[currentID].ID, game.Players[currentID].Name);
                                break;
                            }

                            currentplayer = game.WhosTurn();

                            currentID = currentplayer.ID - 1;

                        }

                        
                    }


        }

        

        public static HumanPlayer CreateHumanPlayer(int id)
        {
            string name = PromptForString("Enter your name: ");

            return new HumanPlayer(id, name)
            {
                ID = id,
                Name = name,
            };

        }

        public static ComputerPlayer CreateComputerPlayer(int id)
        {

            ComputerPlayer player = new ComputerPlayer(id, "Computer");

            return player;

        }

        public static string SelectPlayerType()
        {
            string type;
            bool isValid;
            do
            {
                type = PromptForString("Play with computer or human (Enter C or H): ");
                if (type.ToUpper() == "H" || type.ToUpper() == "C") isValid = true;

                else isValid = false;

                if (!isValid)
                    Console.WriteLine("Invalid, please check!!!");
            }
            while (!isValid); // keep looping until a valid number is entered

            return type.ToUpper();
        }

        static void Main(string[] args)
        {
            MainMenu();
        }

        public static void MainMenu()
        {
            const int PLAYERNUMBER = 2;
            Game game = new Game();
            game.Players = new Player[PLAYERNUMBER];

            while (true)
            {
                Console.WriteLine("1. Start a new board game");
                Console.WriteLine("2. Load file to continue the board game");
                Console.WriteLine("3. Find patient records");
                Console.WriteLine("4. Query owing patients");
                Console.WriteLine("5. Exit");

                Console.Write("Enter choice: ");
                int choice = PromptForInt();

                if (choice == 1)
                {
                    StartNewGame();
                }


                //else if (choice == 2)
                //    ReadPatientRecords();
                //else if (choice == 3)
                //    FindPatientRecords();
                //else if (choice == 4)
                //    QueryOwingPatients();
                else if (choice == 5)
                    return;
                else
                    Console.WriteLine("Invalid choice, please enter a valid number.");

                Console.WriteLine();
            }
        }

        public static int PromptForInt()
        {
            bool isValid;
            int value;
            do
            {
                //Console.Write(prompt); // display prompt
                isValid = int.TryParse(Console.ReadLine(), out value); // make sure it's actually an integer
                if (!isValid)
                {
                    Console.WriteLine("Invalid, must be a number.");
                    Console.Write("Please anter again: ");
                }

            }
            while (!isValid); // keep looping until a valid number is entered
            return value;
        }

        public static string PromptForString(string prompt)
        {
            string value;
            bool isValid;
            do
            {
                Console.Write(prompt);
                value = Console.ReadLine(); // get string input
                isValid = value.Length > 0; // it's valid if the string isn't empty
                if (!isValid)
                    Console.WriteLine("Invalid, must enter a string");
            }
            while (!isValid); // keep trying until we get a non-empty string
            return value;
        }

        public static double PromptForDouble(string prompt)
        {
            bool isValid;
            double value;
            do
            {
                Console.Write(prompt); // display prompt
                isValid = double.TryParse(Console.ReadLine(), out value); // make sure it's actually an integer
                if (!isValid)
                    Console.WriteLine("Invalid, must be a number");
            }
            while (!isValid); // keep looping until a valid number is entered
            return value;
        }

        public static bool PromptYesNo(string prompt)
        {
            bool isValid;
            bool value = false;
            do
            {
                Console.Write(prompt + " (Y/N): "); // ask the user the question
                string response = Console.ReadLine().ToLower(); // read the response, convert it to lower case
                isValid = false;
                if (response.Length == 1) // make sure it's a single character
                {
                    if (response[0] == 'y') // entered a Y?
                    {
                        isValid = true;
                        value = true;
                    }
                    else if (response[0] == 'n') // entered an N?
                    {
                        isValid = true;
                        value = false;
                    }
                }
                if (!isValid)
                    Console.WriteLine("Invalid response, please enter Y or N");
            }
            while (!isValid); // loop until they enter Y or N
            return value;
        }
    }

}


