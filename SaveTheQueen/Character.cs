namespace SaveTheQueen;

public abstract class Character : GameObject
{
    protected readonly Inventory _inventory;
    protected Map _map;

    public int MaxHP { get; private set; }
    public int HP { get; protected set; }
    public int Gold { get; set; }
    public bool IsAlive => HP > 0;

    protected Character(
        char avatar,
        Vector2 startingPosition,
        Map map,
        int maxHp = 10)
        : base(avatar, startingPosition)
    {
        _inventory = new Inventory();
        _map = map;

        MaxHP = maxHp;
        HP = maxHp;
        Gold = 0;
    }

    public void SetMap(Map map)
    {
        _map = map;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        HP -= amount;

        if (HP < 0)
        {
            HP = 0;
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        HP += amount;

        if (HP > MaxHP)
        {
            HP = MaxHP;
        }
    }

    public void IncreaseMaxHp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        MaxHP += amount;
        HP = MaxHP;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0 || Gold < amount)
        {
            return false;
        }

        Gold -= amount;
        return true;
    }

    public Vector2 GetPosition()
    {
        return _position;
    }

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public bool Move(Vector2 direction, Map map)
    {
        return Move(direction.X, direction.Y, map, null);
    }

    public bool Move(
        int diffX,
        int diffY,
        Map map,
        List<Character>? others)
    {
        int targetX = _position.X + diffX;
        int targetY = _position.Y + diffY;

        if (targetY < 0 || targetY >= map.GetHeight())
        {
            return false;
        }

        if (targetX < 0 || targetX >= map.GetRowWidth(targetY))
        {
            return false;
        }

        Cell targetCell = map.GetCell(targetX, targetY);

        if (targetCell.Visuals == '#')
        {
            return false;
        }

        if (targetCell.IsLockedDoor() &&
            !targetCell.TryUnlock(_inventory))
        {
            return false;
        }

        if (others != null)
        {
            foreach (Character other in others)
            {
                if (other == this || !other.IsAlive)
                {
                    continue;
                }

                Vector2 otherPosition = other.GetPosition();

                if (otherPosition.X == targetX &&
                    otherPosition.Y == targetY)
                {
                    return false;
                }
            }
        }

        _position.X = targetX;
        _position.Y = targetY;

        return true;
    }

    public bool AddItem(Item item)
    {
        return _inventory.Add(item);
    }

    public abstract bool TakeTurn(
        Map map,
        List<Character> others);
}