namespace SaveTheQueen;

public class Player : Character
{
    private readonly Dictionary<ConsoleKey, Vector2> _inputMap;

    public Player(char avatar, Vector2 startingPosition, Map map, Dictionary<ConsoleKey, Vector2> inputMap)
        : base(avatar, startingPosition, map)
    {
        _inputMap = inputMap;
    }

    public override bool TakeTurn(Map map)
    {
        bool isPlaying = true;
        var input = Console.ReadKey(true);

        Vector2 oldPosition = _position;

        if (_inputMap.ContainsKey(input.Key))
        {
            Vector2 direction = _inputMap[input.Key];
            bool moved = Move(direction, map);

            if (moved)
            {
                Console.SetCursorPosition(oldPosition.X, oldPosition.Y);
                map.GetCell(oldPosition.X, oldPosition.Y).Leave();
            }
        }
        else
        {
            switch (input.Key)
            {
                case ConsoleKey.Q:
                    isPlaying = false;
                    break;

                case ConsoleKey.I:
                    _inventory.Display();
                    Console.ReadKey(true);
                    _inventory.Hide(map);
                    break;
            }
        }

        Display();
        return isPlaying;
    }
}