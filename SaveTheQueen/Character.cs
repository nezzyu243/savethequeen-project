namespace SaveTheQueen;

public abstract class Character : GameObject
{
    protected Inventory _inventory;
    protected Map _map = null!;

    public int MaxHP { get; }
    public int HP { get; protected set; }
    public int Gold { get; set; }
    public bool IsAlive => HP > 0;

    public Character(char avatar, Vector2 startingPosition, Map map, int maxHp = 10)
        : base(avatar, startingPosition)
    {
        _inventory = new Inventory();
        _map = map;
        MaxHP = maxHp;
        HP = maxHp;
        Gold = 0;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (HP < 0) HP = 0;
    }

    public void Heal(int amount)
    {
        HP += amount;
        if (HP > MaxHP) HP = MaxHP;
    }

    public Vector2 GetPosition() => _position;

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public bool Move(Vector2 direction, Map map)
    {
        return Move(direction.X, direction.Y, map, null);
    }

    public bool Move(int diffX, int diffY, Map map, List<Character>? others)
    {
        int targetX = _position.X + diffX;
        int targetY = _position.Y + diffY;

        if (targetY < 0 || targetY >= map.GetHeight()) return false;
        if (targetX < 0 || targetX >= map.GetRowWidth(targetY)) return false;

        Cell cell = map.GetCell(targetX, targetY);

        if (cell.Visuals == '#') return false;

        if (cell.IsLockedDoor() && !cell.TryUnlock(_inventory)) return false;

        if (others != null)
        {
            foreach (Character other in others)
            {
                if (other != this && other.IsAlive &&
                    other.GetPosition().X == targetX &&
                    other.GetPosition().Y == targetY)
                {
                    return false;
                }
            }
        }

        _position.X = targetX;
        _position.Y = targetY;

        if (this is Player && cell.HasItem())
        {
            Item? item = cell.TakeItem();
            if (item != null)
                AddItem(item);
        }

        return true;
    }

    public void AddItem(Item item)
    {
        _inventory.Add(item);
    }

    public abstract bool TakeTurn(Map map, List<Character> others);
}