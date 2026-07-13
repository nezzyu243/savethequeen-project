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
    public int Count => _items.Count;

    public Item? GetLast() => _items.Count > 0 ? _items[^1] : null;

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

    // Przyjmuje gracza jako parametr, żeby móc faktycznie zmienić jego HP/złoto
    public string UseItem(int index, Player player)
    {
        Item? item = _items.ElementAtOrDefault(index);
        if (item == null) return "Nie masz takiego przedmiotu.";

        string result;

        switch (item.Effect)
        {
            case ItemEffect.Heal:
                player.Heal(item.Value);
                Remove(item);
                result = $"Uzywasz {item.Name}, leczysz {item.Value} HP. HP: {player.HP}/{player.MaxHP}";
                break;

            case ItemEffect.Gold:
                player.Gold += item.Value;
                Remove(item);
                result = $"Sprzedajesz {item.Name} za {item.Value} zlota. Zloto: {player.Gold}";
                break;

            case ItemEffect.Key:
                result = "Klucze uzywaja sie same przy zamknietych drzwiach.";
                break;

            case ItemEffect.QuestItem:
                result = "Oddaj ten przedmiot Krolowej.";
                break;

            default:
                result = "Nic sie nie stalo.";
                break;
        }

        return result;
    }

    public void Display()
    {
        Console.Clear();
        Console.WriteLine("=== EKWIPUNEK ===");

        if (_items.Count == 0)
        {
            Console.WriteLine("(pusto)");
        }
        else
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_items[i]}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Wcisnij dowolny klawisz, aby wrocic...");
    }

    public void Hide()
    {
        Console.Clear();
    }
}