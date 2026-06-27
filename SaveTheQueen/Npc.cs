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
        int index = Random.Shared.Next(availableDirections.Count);
        Vector2 direction = availableDirections[index];
        Move(direction, map);
        return true;
    }
}