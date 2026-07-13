namespace SaveTheQueen;

public class Npc : Character
{
    private static readonly Vector2[] AvailableDirections =
    [
        new Vector2(-1, 0),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(0, 1)
    ];

    private int _currentAttackDamage;
    private readonly int _damageIncrease;

    public bool IsHostile { get; }
    public bool IsQueen { get; }
    public bool IsMerchant { get; }
    public bool QuestCompleted { get; set; }

    public Npc(
        char avatar,
        Map map,
        bool isHostile,
        bool isQueen = false,
        bool isMerchant = false,
        int maxHp = 10,
        int startingAttackDamage = 2,
        int damageIncrease = 2)
        : base(
            avatar,
            GetRandomPosition(map),
            map,
            maxHp)
    {
        IsHostile = isHostile;
        IsQueen = isQueen;
        IsMerchant = isMerchant;

        _currentAttackDamage = startingAttackDamage;
        _damageIncrease = damageIncrease;
    }

    private static Vector2 GetRandomPosition(Map map)
    {
        List<Vector2> floorPositions =
            map.GetFloorPositions();

        if (floorPositions.Count == 0)
        {
            throw new InvalidOperationException(
                "Mapa nie zawiera pola, na ktorym mozna umiescic NPC.");
        }

        return floorPositions[
            Random.Shared.Next(floorPositions.Count)];
    }

    public int PerformAttack()
    {
        int damage = _currentAttackDamage;

        _currentAttackDamage += _damageIncrease;

        return damage;
    }

    public int GetCurrentAttackDamage()
    {
        return _currentAttackDamage;
    }

    public Item? DropQuestItem()
    {
        if (!IsHostile)
        {
            return null;
        }

        return new Item(
            '&',
            _position,
            "Lancuszek ksiezniczki",
            ItemEffect.QuestItem,
            0);
    }

    public override bool TakeTurn(
        Map map,
        List<Character> others)
    {
        if (!IsAlive || !IsHostile)
        {
            return true;
        }

        Vector2 direction =
            AvailableDirections[
                Random.Shared.Next(
                    AvailableDirections.Length)];

        List<Character> otherCharacters =
            others
                .Where(character => character != this)
                .ToList();

        Move(
            direction.X,
            direction.Y,
            map,
            otherCharacters);

        return true;
    }
}