namespace SaveTheQueen;

public class Inventory
{
    private readonly List<Item> _items = [];
    private readonly int _capacity;

    public Inventory(int capacity = 6)
    {
        _capacity = capacity;
    }

   
    public bool IsFull => _items.Count >= _capacity;

    public bool Add(Item item)
    {
        if (IsFull) return false;
        _items.Add(item);
        return true;
    }

    public bool HasItemWithEffect(ItemEffect effect)
{
    for (int i = 0; i < _items.Count; i++)
    {
        if (_items[i].Effect == effect)
            return true;
    }

    return false;
}

public Item? FindItemWithEffect(ItemEffect effect)
{
    for (int i = 0; i < _items.Count; i++)
    {
        if (_items[i].Effect == effect)
            return _items[i];
    }

    return null;
}

    public bool Remove(Item item) => _items.Remove(item);

    public Item? RemoveAt(int index)
    {
        if (index < 0 || index >= _items.Count) return null;
        Item item = _items[index];
        _items.RemoveAt(index);
        return item;
    }
     public void Display()
    {
        int x = 30;
        int y = 0;

        Console.SetCursorPosition(x, y);
        Console.WriteLine("Inventory:");

        y++;

        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].Display(new Vector2(x, y));
            y++;
        }
    }
    public void Hide()
{
    int x = 30;

    for (int y = 0; y < _items.Count + 1; y++)
    {
        Console.SetCursorPosition(x, y);
        Console.WriteLine("                        ");
    }
    
}
}