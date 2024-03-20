using static BoardGames.Program;

namespace BoardGames
{


    internal class Program
    {
        public abstract class Player
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public bool State { get; set; }
            public Piece Piece { get; set; }
            public Move CurrentMove { get; set; }

            public abstract int Makemove(int n);
        }

        public class HumanPlayer : Player
        {
            public HumanPlayer(int id, string name)
            {
                ID = id;
                Name = name;
                Type = "Human";
            }

            public override int Makemove(int n)
            {
                int position; 
                bool isValid;

                do
                {
                    position = PromptForInt("Enter a position number to place the piece: ");
                    if(position <= n) isValid = true;

                    else isValid = false;

                    if (!isValid) 
                        Console.WriteLine("Invalid, must less or equal than {0}!!!", n);
                }
                while (!isValid); // keep looping until a valid number is entered

                

                return position;
            }
        }

        public class ComputerPlayer : Player
        {
            public ComputerPlayer(int id, string name)
            {
                ID = id;
                Name = name;
                Type = "Computer";
            }

            public override int Makemove(int n)
            {
                Random rand = new Random();

                int position = rand.Next(n);

                return position;
            }
        }

        public class Game
        {
            public Player[] Players;
            public BoardGame BoardGame;




        }

        public class BoardGame
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Piece BoardPiece;
            //public int size { get; set; }
            public string[,] board { get; set; }

            public void PrintBoard()
            {
                string piece;

                for (int i = 0; i < board.GetLength(0) * 2 + 1; i++)
                {

                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        if( (i % 2) == 0 || i == (board.GetLength(0) * 2 + 1) )
                        {
                            Console.Write("++");
                        }

                        else
                        {
                            if (board[i/2, j] is null)
                            {
                                piece = " ";

                            }

                            else piece = board[i/2, j];

                            Console.Write("|{0}", piece);

                            if (j == (board.GetLength(1)-1) )
                            {
                                Console.Write("|");
                            }
                        }

                    }

                    if ((i % 2) == 0 || i == (board.GetLength(0) * 2 + 1))
                    {
                        Console.WriteLine("+");
                    }
                    else Console.WriteLine("");

                }

            }

        }

        //public class Treblecross: BoardGame
        //{
        //    public int[] layout { get; set; }
        //}

        //public class Reversi : BoardGame
        //{
        //    public int[][] layout { get; set; }
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
            public int[] movePosition { get; set; }
        }

        //public class Board
        //{
        //    public int[,] layout { get; set; }
        //}

        public static int StartNewGame()
        {

            bool isValid;
            int gameID;
            do
            {
                gameID = PromptForInt("Enter a game code => 1. Treblecross 2.Reversl: ");
                if(gameID == 1 || gameID == 2) isValid = true;

                else isValid = false;

                if (!isValid) 
                    Console.WriteLine("Invalid, please check!!!");
            }
            while (!isValid); // keep looping until a valid number is entered

            return gameID;

        }

        public static BoardGame CreateTreblecrossGame(int gameID)
        {

            const int MAX = 1;
            string name = "Treblecross";
            Piece piece1 = new Piece(1,"X");
            //Piece piece2 = new Piece(2, "W");
            //Piece piece3 = new Piece(3, "B");
            int n = PromptForInt("Enter a number to decide board size: ");

            return new BoardGame()
            {
                ID = gameID,
                Name = name,
                BoardPiece = piece1,
                board = new string[MAX,n]
                //board = new string[8, 8]

            };

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
            //game.BoardGame = new Treblecross();
            game.Players = new Player[PLAYERNUMBER];

            while (true)
            {
                Console.WriteLine("1. Start a new board game");
                Console.WriteLine("2. Load file to continue the board game");
                Console.WriteLine("3. Find patient records");
                Console.WriteLine("4. Query owing patients");
                Console.WriteLine("5. Exit");

                int choice = PromptForInt("Enter choice: ");

                if (choice == 1)
                {
                    int gameID = StartNewGame();
                    game.BoardGame = CreateTreblecrossGame(gameID);
                    game.Players[0] = CreateHumanPlayer(1);
                    string type = SelectPlayerType();
                    if (type == "H") game.Players[1] = CreateHumanPlayer(2);
                    else if (type == "C") game.Players[1] = CreateComputerPlayer(2);
                    game.BoardGame.PrintBoard();
                    //Console.WriteLine("{0},{1},{2},{3},{4}", game.Players[0].Type, game.Players[0].Name, game.Players[1].Type, game.Players[1].Name, game.BoardGame.board.GetLength(0));

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

        static int PromptForInt(string prompt)
        {
            bool isValid;
            int value;
            do
            {
                Console.Write(prompt); // display prompt
                isValid = int.TryParse(Console.ReadLine(), out value); // make sure it's actually an integer
                if (!isValid)
                    Console.WriteLine("Invalid, must be a number");
            }
            while (!isValid); // keep looping until a valid number is entered
            return value;
        }

        static string PromptForString(string prompt)
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

        static double PromptForDouble(string prompt)
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
    }

}


