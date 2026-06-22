namespace SaveTheQueen;

public class Program
{
    public static void Main()
    {
        Console.CursorVisible = false;

        Dictionary<ConsoleKey, Vector2> directions = new Dictionary<ConsoleKey, Vector2>
        {
            [ConsoleKey.A] = new Vector2(-1, 0),
            [ConsoleKey.D] = new Vector2(1, 0),
            [ConsoleKey.W] = new Vector2(0, -1),
            [ConsoleKey.S] = new Vector2(0, 1)
        };

        Map map = new Map();
        map.LoadFromFile("level1.txt");

        ItemSpawner.SpawnRandomItems(map, 5);

        Character hero = new Player('@', new Vector2(10, 10), map, directions);
        Character villain = new Npc('$', new Vector2(20, 10), map);
        List<Character> characters = [hero, villain];

        map.Display();
        foreach (Character character in characters)
        {
            character.Display();
        }
<<<<<<< HEAD
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
=======
>>>>>>> d82fb8ae015dedcafc83dfc6876fb01504becb86

        bool isPlaying = true;
        while (isPlaying)
        {
            foreach (Character character in characters)
            {
                isPlaying = character.TakeTurn(map);
                if (!isPlaying) break;
            }
        }

        Console.SetCursorPosition(0, map.GetHeight() + 1);
        Console.WriteLine("Do widzenia!");
    }
}