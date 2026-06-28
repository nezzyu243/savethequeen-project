namespace SaveTheQueen;

public class Player : Character
{
    private readonly Dictionary<ConsoleKey, Vector2> _inputMap;

    public string LastMessage { get; private set; } = "";
    public bool ReachedStairs { get; private set; }
    public bool HasWon { get; private set; }

    public Player(char avatar, Vector2 startingPosition, Map map, Dictionary<ConsoleKey, Vector2> inputMap)
        : base(avatar, startingPosition, map, maxHp: 20)
    {
        _inputMap = inputMap;
    }

    public override bool TakeTurn(Map map, List<Character> others)
    {
        bool isPlaying = true;
        ReachedStairs = false;
        LastMessage = "";

        var input = Console.ReadKey(true);

        if (_inputMap.ContainsKey(input.Key))
        {
            Vector2 direction = _inputMap[input.Key];
            int targetX = _position.X + direction.X;
            int targetY = _position.Y + direction.Y;

            Npc? target = FindNpcAt(others, targetX, targetY);

            if (target != null)
            {
                Fight(target, others);
            }
            else
            {
                bool moved = Move(direction, map);
                if (moved)
                {
                    Cell cell = map.GetCell(_position.X, _position.Y);
                    if (cell.IsStairs())
                        ReachedStairs = true;
                }
            }
        }
        else
        {
            switch (input.Key)
            {
                case ConsoleKey.Q:
                    isPlaying = false;
                    break;

                case ConsoleKey.I:
                    ShowInventoryMenu();
                    break;
            }
        }

        return isPlaying;
    }

    private static Npc? FindNpcAt(List<Character> others, int x, int y)
    {
        foreach (Character c in others)
        {
            if (c is Npc npc && npc.IsAlive &&
                npc.GetPosition().X == x && npc.GetPosition().Y == y)
            {
                return npc;
            }
        }
        return null;
    }

    private void Fight(Npc npc, List<Character> others)
    {
        if (!npc.IsHostile)
        {
            HasWon = true;
            LastMessage = "Uratowalas Krolowa! WYGRANA!";
            return;
        }

        const int playerDamage = 4;
        const int npcDamage = 2;

        npc.TakeDamage(playerDamage);
        LastMessage = $"Zadajesz {playerDamage} obrazen.";

        if (!npc.IsAlive)
        {
            others.Remove(npc);
            LastMessage += " Wrog pokonany!";
        }
        else
        {
            TakeDamage(npcDamage);
            LastMessage += $" Wrog odpowiada za {npcDamage} obrazen.";
        }
    }

    private void ShowInventoryMenu()
    {
        _inventory.Display();
        Console.WriteLine();
        Console.WriteLine("U = uzyj, G = wyrzuc, inny klawisz = wyjdz bez akcji");

        var action = Console.ReadKey(true).Key;

        if (action == ConsoleKey.U || action == ConsoleKey.G)
        {
            Console.WriteLine("Wcisnij cyfre przedmiotu (1-9):");
            char numberKey = Console.ReadKey(true).KeyChar;

            if (char.IsDigit(numberKey))
            {
                int index = numberKey - '1';

                if (action == ConsoleKey.U)
                {
                    LastMessage = _inventory.UseItem(index);
                }
                else
                {
                    Item? dropped = _inventory.RemoveAt(index);
                    LastMessage = dropped != null
                        ? $"Wyrzucono: {dropped.Name}"
                        : "Nie ma takiego przedmiotu.";
                }
            }
        }

        _inventory.Hide();
    }
}