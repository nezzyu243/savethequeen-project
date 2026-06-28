namespace SaveTheQueen;

public class Program
{
    public static void Main()
    {
        Console.Clear();
        Console.CursorVisible = false;

        var inputMap = new Dictionary<ConsoleKey, Vector2>
        {
            { ConsoleKey.W, new Vector2(0, -1) },
            { ConsoleKey.S, new Vector2(0, 1) },
            { ConsoleKey.A, new Vector2(-1, 0) },
            { ConsoleKey.D, new Vector2(1, 0) }
        };

        string[] levelFiles = { "level1.txt", "level2.txt" };
        int currentLevel = 0;

        Map map = new Map();
        map.LoadFromFile(levelFiles[currentLevel]);
        ItemSpawner.SpawnRandomItems(map, 5);

        List<Vector2> floors = map.GetFloorPositions();
        Vector2 playerPos = floors[Random.Shared.Next(floors.Count)];
        Player player = new Player('@', playerPos, map, inputMap);

        List<Character> npcs = new()
        {
            new Npc('g', map, isHostile: true)
        };

        bool isPlaying = true;

        while (isPlaying)
        {
            Draw(map, player, npcs);

            isPlaying = player.TakeTurn(map, npcs);

            if (player.HP <= 0)
            {
                Console.Clear();
                Console.WriteLine("Zginelas w podziemiach. KONIEC GRY.");
                break;
            }

            if (player.HasWon)
            {
                Console.Clear();
                Console.WriteLine("Uratowalas Krolowa! WYGRANA!");
                break;
            }

            if (!isPlaying) break;

            if (player.ReachedStairs && currentLevel < levelFiles.Length - 1)
            {
                currentLevel++;
                map = new Map();
                map.LoadFromFile(levelFiles[currentLevel]);
                ItemSpawner.SpawnRandomItems(map, 5);

                floors = map.GetFloorPositions();
                player.SetPosition(floors[Random.Shared.Next(floors.Count)]);

                npcs.Clear();
                npcs.Add(new Npc('Q', map, isHostile: false));
            }

            foreach (Character npc in npcs.ToList())
            {
                if (!isPlaying) break;
                isPlaying = npc.TakeTurn(map, npcs);
            }
        }
    }

    private static void Draw(Map map, Player player, List<Character> npcs)
    {
        Console.Clear();
        map.Display();
        player.Display();

        foreach (Character npc in npcs)
        {
            npc.Display();
        }

        Console.SetCursorPosition(0, map.GetHeight());
        Console.WriteLine();
        Console.WriteLine($"HP: {player.HP}/{player.MaxHP}");

        if (!string.IsNullOrEmpty(player.LastMessage))
        {
            Console.WriteLine(player.LastMessage);
        }

        Console.WriteLine("WASD - ruch | I - ekwipunek | Q - wyjscie");
    }
}