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
            List<Character> allCharacters = new() { player };
            allCharacters.AddRange(npcs);

            Draw(map, player, npcs, currentLevel + 1);

            isPlaying = player.TakeTurn(map, npcs, allCharacters);

            if (player.HP <= 0)
            {
                Console.Clear();
                Console.WriteLine("Zginelas w podziemiach. KONIEC GRY.");
                Console.WriteLine("Wcisnij dowolny klawisz...");
                Console.ReadKey(true);
                break;
            }

            if (player.HasWon)
            {
                Console.Clear();
                Console.WriteLine("Uratowalas Krolowa!");
                Console.WriteLine("Krolowa jest bezpieczna. WYGRANA!");
                Console.WriteLine("Wcisnij dowolny klawisz...");
                Console.ReadKey(true);
                break;
            }

            if (!isPlaying) break;

            if (player.ReachedStairs)
            {
                if (currentLevel < levelFiles.Length - 1)
                {
                    currentLevel++;
                    map = new Map();
                    map.LoadFromFile(levelFiles[currentLevel]);
                    ItemSpawner.SpawnRandomItems(map, 5);

                    floors = map.GetFloorPositions();
                    player.SetPosition(floors[Random.Shared.Next(floors.Count)]);

                    npcs.Clear();
                    npcs.Add(new Npc('Q', map, isHostile: false, isQueen: true));
                }
                else if (currentLevel > 0)
                {
                    currentLevel--;
                    map = new Map();
                    map.LoadFromFile(levelFiles[currentLevel]);
                    ItemSpawner.SpawnRandomItems(map, 5);

                    floors = map.GetFloorPositions();
                    player.SetPosition(floors[Random.Shared.Next(floors.Count)]);

                    npcs.Clear();
                    npcs.Add(new Npc('g', map, isHostile: true));
                }
            }

            foreach (Character npc in npcs.ToList())
            {
                if (!isPlaying) break;
                List<Character> allForNpc = new() { player };
                allForNpc.AddRange(npcs);
                npc.TakeTurn(map, allForNpc);
            }
        }
    }

    private static void Draw(Map map, Player player, List<Character> npcs, int levelNumber)
    {
        Console.Clear();
        map.Display();
        player.Display();

        foreach (Character npc in npcs)
        {
            if (npc.IsAlive)
                npc.Display();
        }

        Console.SetCursorPosition(0, map.GetHeight());
        Console.WriteLine();
        Console.WriteLine($"Poziom: {levelNumber}  HP: {player.HP}/{player.MaxHP}");

        if (!string.IsNullOrEmpty(player.LastMessage))
        {
            Console.WriteLine(player.LastMessage);
        }

        Console.WriteLine("WASD - ruch | I - ekwipunek (U=uzyj, G=wyrzuc) | Q - wyjscie");
    }
}