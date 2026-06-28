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
                        Npc? targetAfterMove = FindNpcAt(others, _position.X, _position.Y);

                        if (targetAfterMove != null)
                    {
                        Fight(targetAfterMove, others);
                    }
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

    Console.WriteLine();
    LastMessage = "Wróg patrzy na Ciebie groźnym spojrzeniem, przygotuj się na starcie!";
    Console.WriteLine();
    Console.WriteLine("NPC: Nigdy nie zdobędziesz księżniczki, nie oddam Ci jej bez walki!");
    Console.WriteLine("Zaczyna się pojedynek!");
    Console.ReadKey(true);

    bool playerWon = PlayRps(npc);
    if (playerWon)
{
    npc.TakeDamage(1);
    LastMessage = "Wygrana runda!";
}
else
{
    TakeDamage(3);
    LastMessage = "Przegrana runda!";
}

if (!npc.IsAlive)
{
    others.Remove(npc);
    LastMessage = "Pokonano wroga!";
}
    else
    {
        TakeDamage(3);
        LastMessage = "Przegrana walka!";
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
    private bool PlayRps(Npc npc)
{
    List<string> allowedSigns = new() { "rock", "paper", "scissors" };
    string endGameCommand = "exit";

    int playerPoints = 0;
    int npcPoints = 0;

    Console.WriteLine();
    Console.WriteLine("WALKA RPS (BEST OF 3)");
    Console.WriteLine("rock / paper / scissors lub exit\n");

    while (playerPoints < 2 && npcPoints < 2)
    {
        string firstSign;

        do
        {
            Console.WriteLine("Twój ruch:");
            firstSign = Console.ReadLine()?.Trim().ToLower() ?? "";
        }
        while (firstSign != endGameCommand && !allowedSigns.Contains(firstSign));

        if (firstSign == endGameCommand)
            return false;

      string secondSign = allowedSigns[Random.Shared.Next(allowedSigns.Count)];
      secondSign = secondSign.ToLower();

        Console.WriteLine($"NPC: {secondSign}");

        if (firstSign == secondSign)
        {
            Console.WriteLine("Remis!");
        }
        else
        {
            int firstIndex = allowedSigns.IndexOf(firstSign);
            int secondIndex = allowedSigns.IndexOf(secondSign);

            int winningIndex = (secondIndex + 1) % allowedSigns.Count;
            string winningSign = allowedSigns[winningIndex];

            if (firstSign == winningSign)
            {
                Console.WriteLine("Wygrywasz rundę!");
                playerPoints++;
            }
            else
            {
                Console.WriteLine("NPC wygrywa rundę!");
                npcPoints++;
            }
        }

        Console.WriteLine($"Wynik: Ty {playerPoints} - {npcPoints} NPC\n");
    }

   Console.WriteLine("KONIEC WALKI!");
   Console.WriteLine("Kliknij dowolny klawisz...");
   Console.ReadKey(true);

    return playerPoints > npcPoints;
}
}

public enum RpsChoice
{
    Rock,
    Paper,
    Scissors
}
