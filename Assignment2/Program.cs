using System;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

class Position
{
    int row;
    int column;
    public int X
    {
        get { return row; }
        set { row = value; }
    }

    public int Y
    {
        get { return column; }
        set { column = value; }
    }

    // Constructor 
    public Position(int x, int y)
    {
        if (ValidPosition(x, y))
        {
            this.row = x;
            this.column = y;
        }

    }
    //Method
    public bool ValidPosition(int X, int Y)
    {
        if (X < 0 || Y < 0 || X > 5 || Y > 5)
        {

            return false;
        }
        else
        {
            return true;
        }
    }
}

class Player
{
    public string name;
    public int gemCount = 0;
    public Position position;

    public Player(string name, int x, int y)
    {
        this.name = name;
        if (ValidPosition(x, y))
        {
            this.position = new Position(x, y);
        }



    }

    public bool ValidPosition(int X, int Y)
    {
        if (X < 0 || Y < 0 || X > 5 || Y > 5)
        {

            return false;
        }
        else
        {
            return true;
        }
    }

    public bool Move(string direction)
    {
        int newX = position.X;
        int newY = position.Y;

        if (position.ValidPosition(newX, newY))
        {

            switch (direction)


            {
                case "U":
                case "u":
                    newX -= 1;
                    break;
                case "D":
                case "d":
                    newX += 1;
                    break;
                case "L":
                case "l":
                    newY -= 1;
                    break;
                case "R":
                case "r":
                    newY += 1;
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    return false;

            }

            position.X = newX;
            position.Y = newY;

        }
        return true;

    }
}

class Cell
{
    private string occupant;

    public string Occupant
    {
        get { return occupant; }
        set
        {
            if (value == "P1" || value == "P2" || value == "G" || value == "-" || value == "O") { occupant = value; }
        }
    }

}
class Board
{
    private Random random = new Random();
    public Cell[,] Grid = new Cell[6, 6];
    public int totalGems = 5;

    //public Player currentTurn;


    public Board()
    {

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Grid[i, j] = new Cell { Occupant = "-" };
            }
        }

        Grid[0, 0].Occupant = "P1";
        Grid[5, 5].Occupant = "P2";
        PlaceItemsRandomly(5, "G");
        PlaceItemsRandomly(5, "O");

    }

    public void PlaceItemsRandomly(int count, string item)
    {
        for (int i = 0; i < count; i++)
        {
            int x, y;
            do
            {
                x = random.Next(6);
                y = random.Next(6);
            } while (Grid[x, y].Occupant != "-");

            Grid[x, y].Occupant = item;
        }
    }
    public void Display()
    {

        for (int i = 0; i < 6; i++)
        {
            Console.WriteLine("");
            for (int j = 0; j < 6; j++)
            {
                Console.Write($"{Grid[i, j].Occupant,-4}");

            }
        }
    }

    public void CollectGem(Player player, int row, int col)
    {
        if (Grid[row, col].Occupant == "G")
        {
            player.gemCount++;
            totalGems--;
        }

    }

    public bool IsValidMove(Player player, string direction)
    {
        int oldRow = player.position.X;
        int oldCol = player.position.Y;
        int newRow = oldRow, newCol = oldCol;
        switch (direction)
        {
            case "U":
            case "u":
                newRow -= 1;
                break;
            case "D":
            case "d":
                newRow += 1;
                break;
            case "L":
            case "l":
                newCol -= 1;
                break;
            case "R":
            case "r":
                newCol += 1;
                break;
            default:
                break;

        }

        if (newRow < 0 || newRow >= Grid.GetLength(0) || newCol < 0 || newCol >= Grid.GetLength(1))
        {
            Console.WriteLine("Out of Bounds");
            return false;
        }

        if (Grid[newRow, newCol].Occupant == "-" || Grid[newRow, newCol].Occupant == "G")
        {
            CollectGem(player, newRow, newCol);
            if (player.Move(direction))

            {
                Grid[player.position.X, player.position.Y].Occupant = player.name;
                Grid[oldRow, oldCol].Occupant = "-";
                return true;
            }

        }
        else if (Grid[newRow, newCol].Occupant == "O")
        {
            Console.WriteLine("Obstacle ahead!!");
        }
        else
        {
            Console.WriteLine("Action cannot be performed");
        }

        return false;
    }

}
class Game
{
    Board board;
    Player player1;
    Player player2;
    Player currentTurn;
    string direction;
    int totalTurns = 0;
    int xAxis, yAxis;


    public Game()
    {
        board = new Board();
        player1 = new Player("P1", 0, 0);
        player2 = new Player("P2", 5, 5);

    }

    public void start()
    {

        currentTurn = player1;
        while (!GameOver())
        {
            board.Display();
            Console.WriteLine($"Turn of {currentTurn.name}");
            Console.WriteLine("\nEnter direction");
            direction = Console.ReadLine();
            xAxis = currentTurn.position.X;
            yAxis = currentTurn.position.Y;
            Console.WriteLine();
            if (board.IsValidMove(currentTurn, direction))
            {

                board.Grid[currentTurn.position.X, currentTurn.position.Y].Occupant = currentTurn.name;
                board.Grid[xAxis, yAxis].Occupant = "-";

                SwitchTurn();
                totalTurns++;

            }

        }
        board.Display();
        AnnounceWinner();

    }


    public void SwitchTurn()
    {
        currentTurn = (currentTurn == player1) ? player2 : player1;
    }
    public bool GameOver()
    {
        if (totalTurns >= 30)
        {
            return true;
        }
        else if (board.totalGems == 0)
        {
            return true;
        }
        else return false;

    }
    public void AnnounceWinner()
    {
        Console.WriteLine("Game over!\n");
        Console.WriteLine($"Player 1 has collected {player1.gemCount} gems.");
        Console.WriteLine($"Player 2 has collected {player2.gemCount} gems.");

        if (player1.gemCount > player2.gemCount)
            Console.WriteLine("\nPlayer 1 won the game!");
        else if (player2.gemCount > player1.gemCount)
            Console.WriteLine("\nPlayer 2 won the game!");
        else
            Console.WriteLine("\nThe game ended in a Tie!");
    }


}
class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.start();

    }

}