namespace SaveTheQueen;

public class Program
{
    public static void Main()
    {
        Console.CursorVisible = false;

        Map map = new Map();
        map.LoadFromFile("level1.txt");

        ItemSpawner.SpawnRandomItems(map, 5);

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
        Npc npc = new Npc('$', npcPos, map);

        List<Character> characters = new()
        {
            player,
            npc
        };

        map.Display();

        foreach (Character c in characters)
            c.Display();

        bool isPlaying = true;

        while (isPlaying)
        {
            foreach (Character c in characters)
            {
                isPlaying = c.TakeTurn(map);
                if (!isPlaying) break;
            }
        }

        Console.SetCursorPosition(0, map.GetHeight() + 1);
        Console.WriteLine("Do widzenia!");
    }
}