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

        public abstract void MakeMoveForTreblecross(int size);


        public void ConfirmMove()
        {
            State = true;
        }

        public void InitializeState()
        {
            State = false;
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

                if (choice == 1) return 1;

                else if (choice == 2) return 2;

                else if (choice == 3) return 3;

                else if (choice == 4) return 4;

                else
                    Console.WriteLine("Invalid choice, please enter a valid number.");

                Console.WriteLine();
            }
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

        public ComputerPlayer(int id, string name, bool state)
        {
            ID = id;
            Name = name;
            Type = "Computer";
            State = state;

        }

        public override void MakeMoveForTreblecross(int size)
        {
            Random rand = new Random();

            int position = rand.Next(size);

            CurrentMove = new Move(position, 0);

        }

    }

    public class Game
    {
        public Player[] Players;
        public BoardGame? BoardGame;

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
                Console.WriteLine("Gane type:");
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

        public List<History> WriteMoveRecords(List<History> history, Move currentMove,  int playerID)
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

            history.Add(newRecord);

            return history;
        }

        public List<History> LoadHistoryFromFile()
        {
            if (!File.Exists(DATA_FILENAME))
                return new List<History>();

            return JsonSerializer.Deserialize<List<History>>(File.ReadAllText(DATA_FILENAME));
        }

        public void SetGameFromHistory()
        {
            List<History> history = LoadHistoryFromFile();

            const int PLAYERNUMBER = 2;
            Players = new Player[PLAYERNUMBER];

            if (history.Count >= 1)
            {
                if (history[0].BoardID == 1)
                {
                    BoardGame = new Treblecross(history[0].BoardSize);

                }
                else
                {
                    Console.WriteLine("Not complete yet!!!");
                    return;
                }

                Players[0] = SetHumanPlayer(history[0].PlayerId, history[0].PlayerName, history[0].PlayerState);

                if (history[0].opponentType == "Computer")
                {
                    Players[1] = SetComputerPlayer(2, history[0].opponentState);
                }

                else
                {
                    Players[1] = SetHumanPlayer(2, history[0].opponentName, history[0].opponentState);
                }

                foreach (var his in history)
                {
                    BoardGame.Board.PlacePiece(his.MoveList.Col, his.MoveList.Row, BoardGame.BoardPiece);
                }

                if (history[history.Count - 1].PlayerId == 1)
                {
                    Players[0].State = true;
                    Players[1].State = false;
                }

                else
                {
                    Players[0].State = false;
                    Players[1].State = true;
                }
                BoardGame.Board.PrintBoard();

                //start playing game
                PlayGame(history);
            }
            else Console.WriteLine("No History Record.");
        }

        public void StartNewGame()
        {
            const int PLAYERNUMBER = 2;
            Players = new Player[PLAYERNUMBER];
            List<History> history = new List<History>();

            int gameID = SelectGame();

            if (gameID == 1)
            {
                BoardGame = new Treblecross();

                BoardGame.Initialize();
            }
            else
            {
                Console.WriteLine("Not complete yet!!!");
                return;
            }

            Players[0] = CreateHumanPlayer(1);

            string type = SelectPlayerType();

            if (type == "H") Players[1] = CreateHumanPlayer(2);
            else if (type == "C") Players[1] = CreateComputerPlayer(2);

            BoardGame.Board.PrintBoard();

            //start playing game
            PlayGame(history);

        }

        public void PlayGame(List<History> history)
        {
            bool isValid;

            bool winner = false;

            int option = 0;

            while (!winner)
            {
                Player currentplayer = WhosTurn();

                int currentID = currentplayer.ID - 1;

                int nextID;

                if (currentID == 0) nextID = 1;

                else nextID = 0;

                while (!Players[currentID].State)
                {
                    do
                    {
                        Players[currentID].MakeMoveForTreblecross(BoardGame.Board.BoardLayout.GetLength(1));
                        isValid = BoardGame.Board.CheckSquare(Players[currentID].CurrentMove.Col, Players[currentID].CurrentMove.Row);
                        if (!isValid && Players[currentID].Type == "Human") Console.WriteLine("Cannot place here");

                    } while (!isValid);

                    BoardGame.Board.PlacePiece(Players[currentID].CurrentMove.Col, Players[currentID].CurrentMove.Row, BoardGame.BoardPiece);

                    history = WriteMoveRecords(history, Players[currentID].CurrentMove, currentID);

                    if (Players[currentID].Type == "Computer")
                    {
                        Console.WriteLine("Computer's turn");
                    }

                    BoardGame.Board.PrintBoard();

                    winner = BoardGame.HasWinner();

                    if (winner)
                    {
                        Console.WriteLine("Player #{0}: {1} wins !!!", Players[currentID].ID, Players[currentID].Name);
                        break;
                    }

                    //DisplayHistory(history);

                    if (Players[currentID].Type == "Human")
                    {
                        do
                        {
                            option = Players[currentID].PlayerMenu();

                            History undo = new History();

                            switch (option)
                            {
                                case 1:
                                    Players[currentID].ConfirmMove();
                                    Players[nextID].InitializeState();
                                    break;

                                case 2:
                                    if (history.Count() > 0)
                                    {

                                        undo = history[history.Count() - 1];

                                        history.RemoveAt(history.Count() - 1);

                                        BoardGame.Board.RemovePiece(Players[currentID].CurrentMove.Col, Players[currentID].CurrentMove.Row);

                                        BoardGame.Board.PrintBoard();

                                        int choice;

                                        do
                                        {
                                            Console.WriteLine("1. Place in new square");
                                            Console.WriteLine("2. Redo");


                                            Console.Write("Enter choice: ");
                                            choice = PromptForInt();

                                            if (choice == 1)
                                            {
                                                do
                                                {
                                                    Players[currentID].MakeMoveForTreblecross(BoardGame.Board.BoardLayout.GetLength(1));
                                                    isValid = BoardGame.Board.CheckSquare(Players[currentID].CurrentMove.Col, Players[currentID].CurrentMove.Row);
                                                    if (!isValid) Console.WriteLine("Cannot place here");

                                                } while (!isValid);

                                                BoardGame.Board.PlacePiece(Players[currentID].CurrentMove.Col, Players[currentID].CurrentMove.Row, BoardGame.BoardPiece);

                                                history = WriteMoveRecords(history, Players[currentID].CurrentMove, currentID);

                                                BoardGame.Board.PrintBoard();

                                                break;
                                            }

                                            else if (choice == 2)
                                            {
                                                BoardGame.Board.PlacePiece(undo.MoveList.Col, undo.MoveList.Row, BoardGame.BoardPiece);

                                                history.Add(undo);

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

                                case 3:

                                    Players[currentID].ConfirmMove();
                                    Players[nextID].InitializeState();
                                    SaveHistoryToFile(history);
                                    break;

                                case 4:
                                    BoardGame.DisplayRule();
                                    break;

                            }

                        } while (!Players[currentID].State && !(option == 3));

                    }

                    else
                    {
                        Players[currentID].ConfirmMove();
                        Players[nextID].InitializeState();

                    }

                    if (option == 3) break;

                    winner = BoardGame.HasWinner();

                    if (winner)
                    {
                        Console.WriteLine("Player #{0}: {1} wins !!!", Players[currentID].ID, Players[currentID].Name);
                        break;
                    }


                    currentplayer = WhosTurn();

                    currentID = currentplayer.ID - 1;

                    if (currentID == 0) nextID = 1;

                    else nextID = 0;

                }

                if (option == 3) break;

            }
        }

        public HumanPlayer CreateHumanPlayer(int id)
        {

            Console.Write("Enter your name Player #{0}: ", id);
            string name = PromptForString("");

            return new HumanPlayer(id, name)
            {
                ID = id,
                Name = name,
            };

        }

        public HumanPlayer SetHumanPlayer(int id, string name, bool state)
        {
            return new HumanPlayer(id, name)
            {
                ID = id,
                Name = name,
                State = state,
            };

        }

        public ComputerPlayer CreateComputerPlayer(int id)
        {

            ComputerPlayer player = new ComputerPlayer(id, "Computer");

            return player;

        }

        public ComputerPlayer SetComputerPlayer(int id, bool state)
        {

            ComputerPlayer player = new ComputerPlayer(id, "Computer", state);

            return player;

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

        public void SaveHistoryToFile(List<History> history)
        {
            File.WriteAllText(DATA_FILENAME, JsonSerializer.Serialize(history));
        }


        public void DisplayHistory(List<History> history)
        {

            foreach (var temp in history)
            {
                Console.WriteLine("ID #{0}:", temp.PlayerId);
                Console.WriteLine("Name: {0}", temp.PlayerName);
                Console.WriteLine("Move: {0}", temp.MoveList.Col);
                Console.WriteLine("Game name: {0}", temp.BoardID);
            }

        }


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
            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("| | | | | | | | | |");
            Console.WriteLine("+++++++++++++++++++");

            Console.WriteLine("+++++++++++++++++++");
            Console.WriteLine("|1|2|3|4|5|6|7|8|9|");
            Console.WriteLine("+++++++++++++++++++");
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

        public Move(int col, int row)
        {
            Col = col;
            Row = row;
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
            // const int PLAYERNUMBER = 2;
            // Game game = new Game();
            // game.Players = new Player[PLAYERNUMBER];

            while (true)
            {
                Console.WriteLine("1. Start a new board game");
                Console.WriteLine("2. Load file to continue the board game");

                Console.WriteLine("5. Exit");

                Console.Write("Enter choice: ");
                int choice = PromptForInt();

                if (choice == 1)
                {
                    Game game = new Game();
                    game.StartNewGame();
                }


                else if (choice == 2)
                {
                    Game game = new Game();
                    game.SetGameFromHistory();
                }
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


