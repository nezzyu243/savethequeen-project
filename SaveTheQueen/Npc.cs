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

    public bool IsHostile { get; }
    public bool IsQueen { get; }
    public bool QuestCompleted { get; set; }

    public Npc(char avatar, Map map, bool isHostile, bool isQueen = false, int maxHp = 10)
        : base(avatar, GetRandomPosition(map), map, maxHp)
    {
        IsHostile = isHostile;
        IsQueen = isQueen;
    }

    private static Vector2 GetRandomPosition(Map map)
    {
        List<Vector2> floors = map.GetFloorPositions();
        return floors[Random.Shared.Next(floors.Count)];
    }

    public Item? DropQuestItem()
    {
        return new Item('&', _position, "Lancuszek ksiezniczki", ItemEffect.QuestItem, 0);
    }

    public override bool TakeTurn(Map map, List<Character> others)
    {
        if (!IsAlive) return true;

        int index = Random.Shared.Next(availableDirections.Count);
        Vector2 direction = availableDirections[index];

        List<Character> othersWithoutSelf = others.Where(c => c != this).ToList();
        Move(direction.X, direction.Y, map, othersWithoutSelf);
        return true;
    }
}