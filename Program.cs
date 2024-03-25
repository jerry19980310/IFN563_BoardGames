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
        public Piece Piece { get; set; }
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

                Console.Write("Enter choice: ");
                int choice = PromptForInt();

                if (choice == 1) return 1;

                else if (choice == 2) return 2;

                else if (choice == 3) return 3;

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

        public void RemovePiece(int col, int row)
        {
            Board[row, col] = null;
        }

    }

    public class Treblecross : BoardGame
    {
        public Treblecross() {}
        public Treblecross(int size)
        {
            ID = 1;
            Name = "Treblecross";
            Piece piece1 = new Piece(1, "X");
            BoardPiece = piece1;
            Board = new string[1, size];
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
        public const string DATA_FILENAME = "history.json";

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

        public static List<History> WriteMoveRecords(List<History> history, Move currentMove, Game game, int playerID)
        {

            History newRecord = new History();

            int opponentID;

            if(playerID == 0) opponentID = 1;

            else opponentID = 0;

            newRecord.opponentName = game.Players[opponentID].Name;

            newRecord.opponentType = game.Players[opponentID].Type;

            newRecord.opponentState = game.Players[opponentID].State;

            newRecord.MoveList = currentMove;

            newRecord.PlayerId = game.Players[playerID].ID;

            newRecord.PlayerName = game.Players[playerID].Name;

            newRecord.PlayerType = game.Players[playerID].Type;

            newRecord.PlayerState = game.Players[playerID].State;

            newRecord.BoardID = game.BoardGame.ID;

            newRecord.BoardSize = game.BoardGame.Board.GetLength(1);

            history.Add(newRecord);

            return history;

        }

        public static List<History> LoadHistoryFromFile()
        {
            if (!File.Exists(DATA_FILENAME))
                return new List<History>();

            return JsonSerializer.Deserialize<List<History>>(File.ReadAllText(DATA_FILENAME));
        }

        public static void SetGameFromHistory()
        {
            List<History> history = LoadHistoryFromFile();
            //DisplayHistory(history);
            const int PLAYERNUMBER = 2;
            Game game = new Game();
            game.Players = new Player[PLAYERNUMBER];

            if (history.Count >= 1)
            {


                if (history[0].BoardID == 1)
                {
                    game.BoardGame = new Treblecross(history[0].BoardSize);

                }
                else
                {
                    Console.WriteLine("Not complete yet!!!");
                    return;
                }

                game.Players[0] = SetHumanPlayer(history[0].PlayerId, history[0].PlayerName, history[0].PlayerState);

                if (history[0].opponentType == "Computer")
                {
                    game.Players[1] = SetComputerPlayer(2, history[0].opponentState);
                }

                else
                {
                    game.Players[1] = SetHumanPlayer(2, history[0].opponentName, history[0].opponentState);
                }

                foreach(var his in history)
                {
                    game.BoardGame.PlacePiece(his.MoveList.Col, his.MoveList.Row);
                }

                if(history[history.Count-1].PlayerId == 1)
                {
                    game.Players[0].State = true;
                    game.Players[1].State = false;
                }

                else
                {
                    game.Players[0].State = false;
                    game.Players[1].State = true;
                } 

                game.BoardGame.PrintBoard();

                bool isValid;

                bool winner = false;
                
                int option = 0;

                while (!winner)
                {
                    Player currentplayer = game.WhosTurn();

                    int currentID = currentplayer.ID - 1;

                    int nextID;

                    if(currentID == 0) nextID = 1;

                    else nextID = 0;

                    while (!game.Players[currentID].State)
                    {
                        do
                        {
                            game.Players[currentID].MakeMoveForTreblecross(game.BoardGame.Board.GetLength(1));
                            isValid = game.BoardGame.CheckSquare(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);
                            if (!isValid && game.Players[currentID].Type == "Human") Console.WriteLine("Cannot place here");

                        } while (!isValid);

                        game.BoardGame.PlacePiece(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);

                        history = WriteMoveRecords(history, game.Players[currentID].CurrentMove, game, currentID);

                        game.BoardGame.PrintBoard();

                        //DisplayHistory(history);

                        if(game.Players[currentID].Type == "Human")
                        {
                            do
                            {
                                option = game.Players[currentID].PlayerMenu();

                                History undo = new History();

                                switch (option)
                                {
                                    case 1:
                                        game.Players[currentID].ConfirmMove();
                                        game.Players[nextID].InitializeState();
                                        break;

                                    case 2:
                                        if(history.Count() > 0)
                                        {

                                            undo = history[history.Count() - 1];

                                            history.RemoveAt(history.Count() - 1);

                                            game.BoardGame.RemovePiece(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);

                                            game.BoardGame.PrintBoard();

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
                                                        game.Players[currentID].MakeMoveForTreblecross(game.BoardGame.Board.GetLength(1));
                                                        isValid = game.BoardGame.CheckSquare(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);
                                                        if (!isValid) Console.WriteLine("Cannot place here");

                                                    } while (!isValid);

                                                    game.BoardGame.PlacePiece(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);

                                                    history = WriteMoveRecords(history, game.Players[currentID].CurrentMove, game, currentID);

                                                    game.BoardGame.PrintBoard();

                                                    break;
                                                }

                                                else if (choice == 2)
                                                {
                                                    game.BoardGame.PlacePiece(undo.MoveList.Col, undo.MoveList.Row);

                                                    history.Add(undo);

                                                    game.BoardGame.PrintBoard();

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

                                        game.Players[currentID].ConfirmMove();
                                        game.Players[nextID].InitializeState();

                                        SaveHistoryToFile(history);
                                        break;

                                }

                            } while (!game.Players[currentID].State && !(option == 3));

                        }

                        if(option == 3) break;

                        winner = game.BoardGame.HasWinner();

                        if (winner)
                        {
                            Console.WriteLine("Player #{0}: {1} wins !!!", game.Players[currentID].ID, game.Players[currentID].Name);
                            break;
                        }


                        currentplayer = game.WhosTurn();

                        currentID = currentplayer.ID - 1;

                        if(currentID == 0) nextID = 1;

                        else nextID = 0;

                    }

                    if(option == 3) break;

                }
                // Console.WriteLine("{0},{1},{2},{3}", history[0].PlayerId, history[0].PlayerName, history[0].opponentName, history[0].opponentType);

            }

            else Console.WriteLine("No History Record.");

        }

        public static void StartNewGame()
        {

            const int PLAYERNUMBER = 2;
            Game game = new Game();
            game.Players = new Player[PLAYERNUMBER];
            List<History> history = new List<History>();

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
            
            int option = 0;

            while (!winner)
            {
                Player currentplayer = game.WhosTurn();

                int currentID = currentplayer.ID - 1;

                int nextID;

                if(currentID == 0) nextID = 1;

                else nextID = 0;

                while (!game.Players[currentID].State)
                {
                    do
                    {
                        game.Players[currentID].MakeMoveForTreblecross(game.BoardGame.Board.GetLength(1));
                        isValid = game.BoardGame.CheckSquare(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);
                        if (!isValid && game.Players[currentID].Type == "Human") Console.WriteLine("Cannot place here");

                    } while (!isValid);

                    game.BoardGame.PlacePiece(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);

                    history = WriteMoveRecords(history, game.Players[currentID].CurrentMove, game, currentID);

                    game.BoardGame.PrintBoard();

                    //DisplayHistory(history);

                    if(game.Players[currentID].Type == "Human")
                    {
                        do
                        {
                            option = game.Players[currentID].PlayerMenu();

                            History undo = new History();

                            switch (option)
                            {
                                case 1:
                                    game.Players[currentID].ConfirmMove();
                                    game.Players[nextID].InitializeState();
                                    break;

                                case 2:
                                    if(history.Count() > 0)
                                    {

                                        undo = history[history.Count() - 1];

                                        history.RemoveAt(history.Count() - 1);

                                        game.BoardGame.RemovePiece(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);

                                        game.BoardGame.PrintBoard();

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
                                                    game.Players[currentID].MakeMoveForTreblecross(game.BoardGame.Board.GetLength(1));
                                                    isValid = game.BoardGame.CheckSquare(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);
                                                    if (!isValid) Console.WriteLine("Cannot place here");

                                                } while (!isValid);

                                                game.BoardGame.PlacePiece(game.Players[currentID].CurrentMove.Col, game.Players[currentID].CurrentMove.Row);

                                                history = WriteMoveRecords(history, game.Players[currentID].CurrentMove, game, currentID);

                                                game.BoardGame.PrintBoard();

                                                break;
                                            }

                                            else if (choice == 2)
                                            {
                                                game.BoardGame.PlacePiece(undo.MoveList.Col, undo.MoveList.Row);

                                                history.Add(undo);

                                                game.BoardGame.PrintBoard();

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

                                    game.Players[currentID].ConfirmMove();
                                    game.Players[nextID].InitializeState();
                                    SaveHistoryToFile(history);
                                    break;

                            }

                        } while (!game.Players[currentID].State && !(option == 3));

                    }

                    if(option == 3) break;

                    winner = game.BoardGame.HasWinner();

                    if (winner)
                    {
                        Console.WriteLine("Player #{0}: {1} wins !!!", game.Players[currentID].ID, game.Players[currentID].Name);
                        break;
                    }


                    currentplayer = game.WhosTurn();

                    currentID = currentplayer.ID - 1;

                }

                if(option == 3) break;

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

        public static HumanPlayer SetHumanPlayer(int id, string name, bool state)
        {
            return new HumanPlayer(id, name)
            {
                ID = id,
                Name = name,
                State = state,
            };

        }

        public static ComputerPlayer CreateComputerPlayer(int id)
        {

            ComputerPlayer player = new ComputerPlayer(id, "Computer");

            return player;

        }

        public static ComputerPlayer SetComputerPlayer(int id, bool state)
        {

            ComputerPlayer player = new ComputerPlayer(id, "Computer", state);

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

        public static void SaveHistoryToFile(List<History> history)
        {
            File.WriteAllText(DATA_FILENAME, JsonSerializer.Serialize(history));
        }


        public static void DisplayHistory(List<History> history)
        {

            foreach(var temp in history)
            {
                Console.WriteLine("ID #{0}:", temp.PlayerId);
                Console.WriteLine("Name: {0}", temp.PlayerName);
                Console.WriteLine("Move: {0}", temp.MoveList.Col);
                Console.WriteLine("Game name: {0}", temp.BoardID);
            }
            
        }

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
                    StartNewGame();
                }


                else if (choice == 2) SetGameFromHistory();
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


