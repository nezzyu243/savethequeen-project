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

public Npc(char avatar, Map map)
    : base(avatar, GetRandomPosition(map), map)
{
}
 private static Vector2 GetRandomPosition(Map map)
    {
        List<Vector2> floors = map.GetFloorPositions();
        return floors[Random.Shared.Next(floors.Count)];
    }

    public override bool TakeTurn(Map map)
    {
        Vector2 oldPosition = _position;

        Random random = new Random();
        int index = random.Next(availableDirections.Count);
        Vector2 direction = availableDirections[index];

        if (Move(direction, map))
        {
            Console.SetCursorPosition(oldPosition.X, oldPosition.Y);
            map.GetCell(oldPosition.X, oldPosition.Y).Leave();
        }

        Display();
        return true;
    }
}