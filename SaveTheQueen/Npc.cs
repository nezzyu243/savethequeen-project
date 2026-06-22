namespace SaveTheQueen;
public class Npc : Character
{
    private readonly List<Vector2> availableDirections = new List<Vector2>
    {
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(0, 1)
    };

    public Npc(char avatar, Vector2 startingPosition, Map map)
        : base(avatar, startingPosition, map)
    {
    }

    public override bool TakeTurn(Map map)
    {
        Console.SetCursorPosition(_position.X, _position.Y);

        Cell cell = map.GetCell(_position.X, _position.Y);

        Random random = new Random();
        int index = random.Next(availableDirections.Count);

        Vector2 direction = availableDirections[index];

        if (Move(direction, map))
        {
            Cell newCell = map.GetCell(_position.X, _position.Y);
            newCell.Leave();
            newCell.Display();
        }

        Display();

        return true;
    }
}