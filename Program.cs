using System.Text.Json;
using static BoardGames.Program;

namespace BoardGames
{
    public abstract class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool State { get; set; }
        public Move CurrentMove { get; set; }

        public abstract void MakeMove(int size);

        public void ConfirmMove()
        {
            State = true;
        }

        public void InitializeState()
        {
            State = false;
        }

        public void SetState(bool state)
        {
            State = state;
        }

    }

    public class Human : Player
    {

        public override void MakeMove(int size)
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

        public Human(int id, string name)
        {
            ID = id;
            Name = name;
            Type = "Human";
            State = false;
        }

    }

    public class Computer : Player
    {
        public override void MakeMove(int size)
        {
            Random rand = new Random();

            int position = rand.Next(size);

            CurrentMove = new Move(position, 0);
        }

        public Computer(int id, string name)
        {
            ID = id;
            Name = name;
            Type = "Computer";
            State = false;

        }
    }

    public abstract class PlayerFactory
    {

        public abstract Player CreatePlayer(int id, string name);

        public abstract Player Setstate(Player player, bool state);

    }

    public class HumanPlayerFactory: PlayerFactory
    {
        public override Player CreatePlayer(int id, string name)
        {
            return new Human(id, name);
        }

        public override Player Setstate(Player player, bool state)
        {
            player.SetState(state);

            return player;
        }

        public int PlayerMenu()
        {

            while (true)
            {
                Console.WriteLine("1. Confirm");
                Console.WriteLine("2. Undo");
                Console.WriteLine("3. Save game");
                Console.WriteLine("4. Help");

                Console.Write("Enter choice: ");
                int choice = PromptForInt();

                if (choice >= 1 && choice <= 4) return choice;

                else Console.WriteLine("Invalid choice, please enter a valid number.");

                Console.WriteLine();
            }
        }


    }

    public class ComputerPlayerFactory : PlayerFactory
    {
        public override Player CreatePlayer(int id, string name)
        {
            return new Computer(id, name);
        }

        public override Player Setstate(Player player, bool state)
        {
            player.SetState(state);

            return player;
        }
    }

    public class Game
    {
        public FileController fileController = FileController.Instance;

        public Player[] Players;

        public Player CurrentPlayer;

        public BoardGame? BoardGame;

        public List<History> GameHistory;

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

        public int SelectGame()
        {
            bool isValid;
            int gameID;
            do
            {
                Console.WriteLine("Game type:");
                Console.WriteLine("1: Treblecross");
                Console.WriteLine("2: Reversl");
                Console.Write("Enter a game code: ");
                gameID = PromptForInt();
                if (gameID == 1 || gameID == 2) isValid = true;

                else isValid = false;

                if (!isValid)
                    Console.WriteLine("Invalid, please check!!!");
            }
            while (!isValid); // keep looping until a valid number is entered

            return gameID;
        }

        public void WriteMoveToRecords(Move currentMove,  int playerID)
        {
            History newRecord = new History();

            int opponentID;

            if (playerID == 0) opponentID = 1;

            else opponentID = 0;

            newRecord.opponentName = Players[opponentID].Name;

            newRecord.opponentType = Players[opponentID].Type;

            newRecord.opponentState = Players[opponentID].State;

            newRecord.MoveList = currentMove;

            newRecord.PlayerId = Players[playerID].ID;

            newRecord.PlayerName = Players[playerID].Name;

            newRecord.PlayerType = Players[playerID].Type;

            newRecord.PlayerState = Players[playerID].State;

            newRecord.BoardID = BoardGame.ID;

            newRecord.BoardSize = BoardGame.Board.BoardLayout.GetLength(1);

            GameHistory.Add(newRecord);
            
        }

        public void SetGameFromHistory()
        {
            GameHistory = fileController.LoadHistoryFromFile();

            PlayerFactory computerFactory = new ComputerPlayerFactory();

            PlayerFactory humanFactory = new HumanPlayerFactory();

            const int PLAYERNUMBER = 2;
            Players = new Player[PLAYERNUMBER];

            //load history and place the piece by the recoard
            if (GameHistory.Count >= 1)
            {
                //set board game by recoard
                if (GameHistory[0].BoardID == 1)
                {
                    BoardGame = new Treblecross(GameHistory[0].BoardSize);
                }
                else
                {
                    Console.WriteLine("Not complete yet!!!");
                    return;
                }

                // create two players and load record by history file.

                // create player 1 from history record.
                Players[0] = humanFactory.CreatePlayer(GameHistory[0].PlayerId, GameHistory[0].PlayerName);
                //set player state
                Players[0] = humanFactory.Setstate(Players[0],GameHistory[0].PlayerState);


                // create player 2 from history record and set the state.
                if (GameHistory[0].opponentType == "Computer")
                {
                    Players[1] = computerFactory.CreatePlayer(2, GameHistory[0].opponentName);
                    Players[1] = computerFactory.Setstate(Players[1], GameHistory[0].opponentState);
                }

                else
                {
                    Players[1] = humanFactory.CreatePlayer(2, GameHistory[0].opponentName);
                    Players[1] = humanFactory.Setstate(Players[1], GameHistory[0].opponentState);
                }

                //Start place the piece
                foreach (var his in GameHistory)
                {
                    BoardGame.Board.PlacePiece(his.MoveList.Col, his.MoveList.Row, BoardGame.BoardPiece);
                }

                //set player's state to decise who is next player
                if (GameHistory[GameHistory.Count - 1].PlayerId == 1)
                {
                    Players[0] = humanFactory.Setstate(Players[0], true);

                    if(GameHistory[0].opponentType == "Computer")
                    {
                        Players[1] = computerFactory.Setstate(Players[1], false);
                    }

                    else Players[1] = humanFactory.Setstate(Players[1], false);

                }

                else
                {
                    Players[0] = humanFactory.Setstate(Players[0], false);

                    if (GameHistory[0].opponentType == "Computer")
                    {
                        Players[1] = computerFactory.Setstate(Players[1], true);
                    }

                    else Players[1] = humanFactory.Setstate(Players[1], true);

                }

                BoardGame.Board.PrintBoard();

                //start playing game
                PlayGame();
            }
            else Console.WriteLine("No History Record.");
        }

        public void StartNewGame()
        {
            PlayerFactory computerFactory = new ComputerPlayerFactory();

            PlayerFactory humanFactory = new HumanPlayerFactory();

            GameHistory = new List<History>();

            const int PLAYERNUMBER = 2;
            Players = new Player[PLAYERNUMBER];
            

            int gameID = SelectGame();

            //create Treblecross game
            if (gameID == 1)
            {
                BoardGame = new Treblecross();

                BoardGame.Initialize();
            }
            //create Reversi game
            else
            {
                Console.WriteLine("Not complete yet!!!");
                return;
            }

            //create player 1

            Console.Write("Enter your name Player #1: ");
            string name = PromptForString("");
            Players[0] = humanFactory.CreatePlayer(1, name);

            //create player2 by player 1 selectd
            string type = SelectPlayerType();

            if (type == "H")
            {
                Console.Write("Enter your name Player #2: ");
                name = PromptForString("");
                Players[1] = humanFactory.CreatePlayer(2, name);
            }
            else if (type == "C") Players[1] = computerFactory.CreatePlayer(2, "computer");

            //display current board
            BoardGame.Board.PrintBoard();



            //start playing game
            PlayGame();

        }

        public void PlayerMove(int currentID)
        {
            bool isValid;

            do
            {
                Players[currentID].MakeMove(BoardGame.Board.BoardLayout.GetLength(1));
                isValid = BoardGame.Board.CheckSquare(Players[currentID].CurrentMove.Col, Players[currentID].CurrentMove.Row);
                if (!isValid && Players[currentID].Type == "Human") Console.WriteLine("Cannot place here");

            } while (!isValid);

            BoardGame.Board.PlacePiece(Players[currentID].CurrentMove.Col, Players[currentID].CurrentMove.Row, BoardGame.BoardPiece);

            if (Players[currentID].Type == "Computer")
            {
                Console.WriteLine("Computer's turn");
            }

            BoardGame.Board.PrintBoard();
        }

        public void PlayGame()
        {
            bool winner = false;

            int option = 0;

            HumanPlayerFactory humanFactory = new HumanPlayerFactory();

            while (!winner)
            {
                CurrentPlayer = WhosTurn();

                int currentID = CurrentPlayer.ID - 1;

                int nextID;

                if (currentID == 0) nextID = 1;

                else nextID = 0;

                while (!Players[currentID].State)
                {

                    PlayerMove(currentID);

                    WriteMoveToRecords(Players[currentID].CurrentMove, currentID);

                    winner = BoardGame.HasWinner();

                    if (winner)
                    {
                        Console.WriteLine("Player #{0}: {1} wins !!!", Players[currentID].ID, Players[currentID].Name);
                        break;
                    }

                    //if player is human, show player menu
                    if (Players[currentID].Type == "Human")
                    {
                        do
                        {
                            option = humanFactory.PlayerMenu();
                            // Players[currentID].PlayerMenu();

                            HumanOperation(option, currentID, nextID);

                            winner = BoardGame.HasWinner();

                            if (winner) break;

                        } while (!Players[currentID].State && !(option == 3));

                    }

                    else
                    {
                        Players[currentID].ConfirmMove();
                        Players[nextID].InitializeState();

                    }

                    if (option == 3) break;

                    //check winner
                    winner = BoardGame.HasWinner();

                    //if has winner, end the game
                    if (winner)
                    {
                        Console.WriteLine("Player #{0}: {1} wins !!!", Players[currentID].ID, Players[currentID].Name);
                        break;
                    }

                }

                if (option == 3) break;

            }
        }
        
        public void HumanOperation(int option, int currentID, int nextID )
        {
            History undo = new History();

            switch (option)
            {
                //confirm
                case 1:
                    Players[currentID].ConfirmMove();
                    Players[nextID].InitializeState();
                    break;
                //undo
                case 2:
                    if (GameHistory.Count() > 0)
                    {

                        undo = GameHistory[GameHistory.Count() - 1];

                        GameHistory.RemoveAt(GameHistory.Count() - 1);

                        BoardGame.Board.RemovePiece(Players[currentID].CurrentMove.Col, Players[currentID].CurrentMove.Row);

                        BoardGame.Board.PrintBoard();

                        int choice;

                        do
                        {
                            Console.WriteLine("1. Place in new square");
                            Console.WriteLine("2. Redo");


                            Console.Write("Enter choice: ");
                            choice = PromptForInt();

                            //place in different square
                            if (choice == 1)
                            {
                                PlayerMove(currentID);

                                WriteMoveToRecords(Players[currentID].CurrentMove, currentID);

                                break;
                            }

                            //redo
                            else if (choice == 2)
                            {
                                BoardGame.Board.PlacePiece(undo.MoveList.Col, undo.MoveList.Row, BoardGame.BoardPiece);

                                GameHistory.Add(undo);

                                BoardGame.Board.PrintBoard();

                                break;
                            }

                            else
                                Console.WriteLine("Invalid choice, please enter a valid number.");

                            Console.WriteLine();
                        } while (!(choice == 1 || choice == 2));

                    }

                    else Console.WriteLine("Cannot undo!!!");

                    break;

                //save the recoard
                case 3:

                    Players[currentID].ConfirmMove();
                    Players[nextID].InitializeState();
                    fileController.SaveHistoryToFile(GameHistory);
                    break;

                //helping system 
                case 4:
                    BoardGame.DisplayRule();
                    BoardGame.Board.PrintBoard();
                    break;

            }
        }


        public string SelectPlayerType()
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

        // public void DisplayHistory(List<History> history)
        // {
        //     foreach (var temp in history)
        //     {
        //         Console.WriteLine("ID #{0}:", temp.PlayerId);
        //         Console.WriteLine("Name: {0}", temp.PlayerName);
        //         Console.WriteLine("Move: {0}", temp.MoveList.Col);
        //         Console.WriteLine("Game name: {0}", temp.BoardID);
        //     }
        // }
    }

    public class History
    {

        public string opponentName { get; set; }
        public string opponentType { get; set; }
        public bool opponentState { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerType { get; set; }
        public bool PlayerState { get; set; }
        public Move MoveList { get; set; }
        public int BoardID { get; set; }
        public int BoardSize { get; set; }
    }

    public class Board
    {
        public int BoardID { get; set; }
        public string BoardName { get; set; }
        public string[,] BoardLayout { get; set; }

        public Board(int id, string name, int col, int row)
        {
            BoardID = id;
            BoardName = name;
            BoardLayout = new string[row, col];

        }

        public bool CheckSquare(int col, int row)
        {
            if (BoardLayout[row, col] is null)
                return true;
            else return false;
        }

        public void PlacePiece(int col, int row, Piece BoardPiece)
        {
            BoardLayout[row, col] = BoardPiece.Name;
        }

        public void RemovePiece(int col, int row)
        {
            BoardLayout[row, col] = null;
        }

        public void PrintBoard()
        {
            string piece;

            Console.WriteLine("***** Current Game *****");

            for (int i = 0; i < BoardLayout.GetLength(0) * 2 + 1; i++)
            {

                for (int j = 0; j < BoardLayout.GetLength(1); j++)
                {
                    if ((i % 2) == 0 || i == (BoardLayout.GetLength(0) * 2 + 1))
                    {
                        Console.Write("++");
                    }

                    else
                    {
                        if (BoardLayout[i / 2, j] is null)
                        {
                            piece = " ";

                        }

                        else piece = BoardLayout[i / 2, j];

                        Console.Write("|{0}", piece);

                        if (j == (BoardLayout.GetLength(1) - 1))
                        {
                            Console.Write("|");
                        }
                    }

                }

                if ((i % 2) == 0 || i == (BoardLayout.GetLength(0) * 2 + 1))
                {
                    Console.WriteLine("+");
                }
                else Console.WriteLine("");
            }

            Console.WriteLine("");

        }

    }

    public abstract class BoardGame
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Piece BoardPiece;
        //public int size { get; set; }
        public Board Board { get; set; }

        public abstract void Initialize();

        public abstract bool HasWinner();

        public abstract void DisplayRule();

        

    }

    public class Treblecross : BoardGame
    {
        public Treblecross() { }
        public Treblecross(int size)
        {
            ID = 1;
            Name = "Treblecross";
            Piece piece1 = new Piece(1, "X");
            BoardPiece = piece1;
            Board board = new Board(1, "Treblecross", size, 1);
            Board = board;

        }
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
            Board board = new Board(1, "Treblecross", size, MAX_LENGTH);
            Board = board;

        }

        public override bool HasWinner()
        {
            bool hasWinner = false;

            for (int i = 0; i < Board.BoardLayout.GetLength(1) - 2; i++)
            {
                if (Board.BoardLayout[0, i] == BoardPiece.Name && Board.BoardLayout[0, i + 1] == BoardPiece.Name && Board.BoardLayout[0, i + 2] == BoardPiece.Name)
                {
                    hasWinner = true;
                    break;
                }
            }

            return hasWinner;
        }

        public override void DisplayRule()
        {
            Console.WriteLine("Game Rules:");
            Console.WriteLine("The game begins with all the 1×n spaces empty.");
            Console.WriteLine("Each player plays an X on the one-dimensional board in an empty cell."); 
            Console.WriteLine("The game is won when a player makes a row of three Xs.");
            Console.WriteLine("");
            Console.WriteLine("A player create a 9 spaces board, the board shows below:");
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("|1|2|3|4|5|6|7|8|9|");
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("You can enter 1 to 9 to place in the space");
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("| | | | | | | | | |");
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("For example, if you enter 3, then space 3 will be place the piece");
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("| | |X| | | | | | |");
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("if you makes a row of three Xs. You WINS !!!");
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("| | |X| | |X|X|X| |");
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("");
        }
    }

    public class Reversi : BoardGame
    {
       public override void Initialize()
       {

       }

       public override void DisplayRule()
       {
            Console.WriteLine("Game Rules:");
       }

       public override bool HasWinner()
       {
            return false;
       }

    //    public Piece GetCurrentPiece()
    //    {
            
    //    }

    }

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

        public Move(int col, int row)
        {
            Col = col;
            Row = row;
        }
    }

    public class FileController
    {
        private static FileController instance;
        private static readonly object lockObject = new object();

        private FileController() { }

        public static FileController Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new FileController();
                        }
                    }
                }
                return instance;
            }
        }

        public List<History> LoadHistoryFromFile()
        {
            if (!File.Exists(DATA_FILENAME))
                return new List<History>();

            return JsonSerializer.Deserialize<List<History>>(File.ReadAllText(DATA_FILENAME));
        }

        public void SaveHistoryToFile(List<History> history)
        {
            File.WriteAllText(DATA_FILENAME, JsonSerializer.Serialize(history));
        }
    }

    internal class Program
    {
        public const string DATA_FILENAME = "history.json";

        static void Main(string[] args)
        {
            MainMenu();
        }

        public static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("1. Start a new board game");
                Console.WriteLine("2. Load file to continue the board game");

                Console.WriteLine("5. Exit");

                Console.Write("Enter choice: ");
                int choice = PromptForInt();

                //start new game
                if (choice == 1)
                {
                    Game game = new Game();
                    game.StartNewGame();
                }
                
                //load history and continue the game
                else if (choice == 2)
                {
                    Game game = new Game();
                    game.SetGameFromHistory();
                }

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