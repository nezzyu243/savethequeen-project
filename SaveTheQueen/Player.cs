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

        Console.SetCursorPosition(_position.X, _position.Y);

        if (_inputMap.ContainsKey(input.Key))
        {
            Vector2 direction = _inputMap[input.Key];

            Move(direction, map);
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
                    _inventory.Hide();
                    break;
            }
        }

        Display();

        return isPlaying;
    }
}