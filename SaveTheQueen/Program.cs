namespace SaveTheQueen;

public class Program
{
    public static void Main()
    {
        Map map = new Map();
        map.LoadFromFile("level1.txt");

        List<Item> items = ItemSpawner.SpawnRandomItems(map, 5);

        map.Display();

        Console.SetCursorPosition(0, map.GetHeight() + 1);
        Console.WriteLine($"Rozmieszczono {items.Count} przedmiotow:");
        foreach (Item item in items)
        {
            Console.WriteLine($" - {item}");
        }
    }
}