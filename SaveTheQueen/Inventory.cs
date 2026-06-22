namespace SaveTheQueen;

public class Inventory
{
    private readonly List<Item> _items = [];
    private readonly int _capacity;

    public Inventory(int capacity = 6)
    {
        _capacity = capacity;
    }

    public IReadOnlyList<Item> Items => _items;
    public bool IsFull => _items.Count >= _capacity;

    public bool Add(Item item)
    {
        if (IsFull) return false;
        _items.Add(item);
        return true;
    }

    public bool HasItemWithEffect(ItemEffect effect) => _items.Any(i => i.Effect == effect);

    public Item? FindItemWithEffect(ItemEffect effect) => _items.FirstOrDefault(i => i.Effect == effect);

    public bool Remove(Item item) => _items.Remove(item);

    public Item? RemoveAt(int index)
    {
        if (index < 0 || index >= _items.Count) return null;
        Item item = _items[index];
        _items.RemoveAt(index);
        return item;
    }

    public string UseItem(int index)
    {
        Item? item = _items.ElementAtOrDefault(index);
        if (item == null) return "Nie masz takiego przedmiotu.";

        string result = item.Effect switch
        {
            ItemEffect.Heal => $"Uzywasz {item.Name}, leczysz {item.Value} HP. (TODO: podlaczyc do Player.HP)",
            ItemEffect.Gold => $"Sprzedajesz {item.Name} za {item.Value} zlota. (TODO: podlaczyc do Player.Gold)",
            ItemEffect.Key => "Klucze uzywaja sie same przy zamknietych drzwiach.",
            _ => "Nic sie nie stalo."
        };

        if (item.Effect != ItemEffect.Key)
        {
            Remove(item);
        }

        return result;
    }
}