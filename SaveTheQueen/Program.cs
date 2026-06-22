namespace SaveTheQueen;

public class Program
{
    public static void Main()
    {
        Map map = new Map();
        map.LoadFromFile("level1.txt");
      
        List<Item> items = ItemSpawner.SpawnRandomItems(map, 5);

        map.Display();

        //Console.SetCursorPosition(0, map.GetHeight() + 1);
        Console.WriteLine($"Rozmieszczono {items.Count} przedmiotow:");
        foreach (Item item in items)
        {
            Console.WriteLine($" - {item}");
        }
        bool isPlaying = true;
         var inputMap = new Dictionary<ConsoleKey, Vector2>
{
    { ConsoleKey.W, new Vector2(0, -1) },
    { ConsoleKey.S, new Vector2(0, 1) },
    { ConsoleKey.A, new Vector2(-1, 0) },
    { ConsoleKey.D, new Vector2(1, 0) }
};

List<Vector2> floors = map.GetFloorPositions();

Vector2 playerPos = floors[Random.Shared.Next(floors.Count)];
Vector2 npcPos = floors[Random.Shared.Next(floors.Count)];

Player player = new Player('@', playerPos, map, inputMap);
Npc npc = new Npc('N', npcPos, map);

List<Character> characters = new()
{
    player,
    npc
};

          while (isPlaying)
        {
            foreach (var c in characters)
            {
                isPlaying = c.TakeTurn(map);
            }
        }

    }
}