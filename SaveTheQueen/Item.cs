namespace SaveTheQueen;

public class Item : GameObject
{
    public string Name { get; }
    public ItemEffect Effect { get; }
    public int Value { get; }

    public Item(char avatar, Vector2 position, string name, ItemEffect effect, int value)
        : base(avatar, position)
    {
        Name = name;
        Effect = effect;
        Value = value;
    }

    public void PlaceOnMap(Map map)
    {
        Cell cell = map.GetCell(_position.X, _position.Y);
        cell.PutItem(this);
    }

    public override string ToString()
    {
        return Effect switch
        {
            ItemEffect.Heal => $"{Name} (leczy {Value} HP)",
            ItemEffect.Key => $"{Name} (otwiera zamkniete drzwi)",
            ItemEffect.Gold => $"{Name} (wart {Value} zlota)",
            _ => Name
        };
    }
}