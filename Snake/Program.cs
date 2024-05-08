namespace Snake;

internal static class Program
{
    public static void Main()
    {
        Console.CursorVisible = false;

        Snake snake = new Snake('$', new Position(5, 5), new Position(4, 5));
        PlayingMap playingMap = new PlayingMap(10, 10, '#');
        Apple apple = new Apple(1, 1);
        bool isRunningGame = true;


        while (isRunningGame)
        {
            playingMap.ClearField();
            SetObjectOnMap(playingMap, snake);
            SetObjectOnMap(playingMap, apple);
            PrintMap(playingMap);
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            Console.Clear();
            ProcessInput(keyInfo, ref snake);
            snake.MoveSnake();


            if (IsQuit(keyInfo))
            {
                Console.WriteLine("You stoped the game");
                isRunningGame = false;
            }

            if (IsSnakeOnBorder(snake, playingMap))
            {
                Console.WriteLine("Snake is on border");
                Console.WriteLine("You lost this game");
                isRunningGame = false;
            }

            if (snake.IsSnakeEatItself())
            {
                Console.WriteLine("Snake ate itself");
                Console.WriteLine("You lose this game");
                isRunningGame = false;
            }

            if (IsSnakeAteApple(snake, apple))
            {
                snake.IncreaseLength();

                do
                {
                    apple.GenerateNewPosition();
                } while (!IsValidPosition(apple, playingMap));
            }
        }
    }

    static bool IsSnakeAteApple(Snake snake, Apple apple)
    {
        if (snake.Position[0].Equals(apple.Position))
        {
            return true;
        }

        return false;
    }

    static bool IsQuit(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key == ConsoleKey.Q;
    }

    static void ProcessInput(ConsoleKeyInfo keyInfo, ref Snake snake)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
            {
                if (snake.Dx == -1)
                {
                    break;
                }

                snake.ChangeDirection(1, 0);
                break;
            }
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
            {
                if (snake.Dx == 1)
                {
                    break;
                }

                snake.ChangeDirection(-1, 0);
                break;
            }
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
            {
                if (snake.Dy == 1)
                {
                    break;
                }

                snake.ChangeDirection(0, -1);
                break;
            }
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
            {
                if (snake.Dy == -1)
                {
                    break;
                }

                snake.ChangeDirection(0, 1);
                break;
            }
        }
    }

    static void SetObjectOnMap(PlayingMap playingMap, Snake snake)
    {
        foreach (Position position in snake.Position)
        {
            playingMap.SetObjectOnField(position, snake.Symbol);
        }
    }

    static void SetObjectOnMap(PlayingMap playingMap, Apple apple)
    {
        playingMap.SetObjectOnField(apple.Position, apple.Symbol);
    }

    static bool IsValidPosition(Apple apple, PlayingMap playingMap)
    {
        Position applePosition = apple.Position;

        if (applePosition.Y >= playingMap.RowsCount)
        {
            return false;
        }

        if (applePosition.X >= playingMap.ColsCount)
        {
            return false;
        }

        if (applePosition.Y <= 0)
        {
            return false;
        }

        if (applePosition.X <= 0)
        {
            return false;
        }

        if (playingMap.Field[applePosition.Y, applePosition.X] == ' ')
        {
            return true;
        }

        return false;
    }

    static void PrintMap(PlayingMap playingMap)
    {
        for (int row = 0; row < playingMap.Field.GetLength(0); row++)
        {
            for (int col = 0; col < playingMap.Field.GetLength(1); col++)
            {
                if (playingMap.Field[row, col] == '$')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else if (playingMap.Field[row, col] == '@')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (playingMap.Field[row, col] == playingMap.BorderSymbol)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.Write(playingMap.Field[row, col] + " ");
            }

            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.Gray;
    }

    static bool IsSnakeOnBorder(Snake snake, PlayingMap playingMap)
    {
        Position snakeHeadPosition = snake.Position[0];

        return playingMap.Field[snakeHeadPosition.Y, snakeHeadPosition.X] == playingMap.BorderSymbol;
    }
}

internal class Snake
{
    public List<Position> Position { get; } = [];
    public int Dx { get; private set; }
    public int Dy { get; private set; }
    public char Symbol { get; }

    public Snake(char symbol, Position head, Position position1)
    {
        Symbol = symbol;
        Position.Add(head);
        Position.Add(position1);
    }

    public void MoveSnake()
    {
        for (int i = Position.Count - 1; i > 0; i--)
        {
            Position[i] = Position[i - 1];
        }

        Position[0] = new Position(Position[0].X + Dx, Position[0].Y + Dy);
    }

    public void IncreaseLength()
    {
        Position.Add(new Position(Position[^1].X, Position[^1].Y));
    }

    public bool IsSnakeEatItself()
    {
        for (int i = 1; i < Position.Count; i++)
        {
            if (Position[0].Equals(Position[i]))
            {
                return true;
            }
        }

        return false;
    }

    public void ChangeDirection(int dx, int dy)
    {
        Dx = dx;
        Dy = dy;
    }
}

internal class PlayingMap(int rowsCount, int colsCount, char borderSymbol)
{
    public char BorderSymbol { get; private set; } = borderSymbol;
    public int RowsCount { get; private set; } = rowsCount;
    public int ColsCount { get; private set; } = colsCount;
    public char[,] Field { get; private set; } = new char[rowsCount, colsCount];


    public void SetObjectOnField(Position position, char symbol)
    {
        Field[position.Y, position.X] = symbol;
    }

    public void ClearField()
    {
        Field = new char[RowsCount, ColsCount];

        for (int i = 0; i < ColsCount; i++)
        {
            for (int j = 0; j < RowsCount; j++)
            {
                Field[j, i] = ' ';

                if (i == 0 || i == ColsCount - 1)
                {
                    Field[j, i] = BorderSymbol;
                }

                if (j == 0 || j == RowsCount - 1)
                {
                    Field[j, i] = BorderSymbol;
                }
            }
        }
    }
}

internal readonly struct Position(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;
}

internal class Apple(int x, int y, char symbol = '@')
{
    public Position Position { get; private set; } = new(x, y);
    public char Symbol { get; } = symbol;

    public void GenerateNewPosition()
    {
        Random random = new Random();

        Position = new Position(random.Next(11), random.Next(11));
    }
}