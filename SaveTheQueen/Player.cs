namespace SaveTheQueen;

public class Player : Character
{
    private readonly Dictionary<ConsoleKey, Vector2> _inputMap;
    private Item? _lastPickedItem;

    public string LastMessage { get; private set; } = "";
    public bool ReachedStairs { get; private set; }
    public bool HasWon { get; private set; }

    public Player(char avatar, Vector2 startingPosition, Map map,
        Dictionary<ConsoleKey, Vector2> inputMap)
        : base(avatar, startingPosition, map, maxHp: 20)
    {
        _inputMap = inputMap;
    }

    public void SetMessage(string message)
    {
        LastMessage = message;
    }

    public override bool TakeTurn(Map map, List<Character> others)
    {
        return TakeTurn(map, others, null);
    }

    public bool TakeTurn(Map map, List<Character> npcs, List<Character>? allCharacters)
    {
        bool isPlaying = true;
        ReachedStairs = false;
        LastMessage = "";
        _lastPickedItem = null;

        var input = Console.ReadKey(true);

        if (_inputMap.ContainsKey(input.Key))
        {
            Vector2 direction = _inputMap[input.Key];
            int targetX = _position.X + direction.X;
            int targetY = _position.Y + direction.Y;

            Npc? target = FindNpcAt(npcs, targetX, targetY);

            if (target != null)
            {
                InteractWithNpc(target, map, npcs);
            }
            else
            {
                int countBefore = _inventory.Count;
                bool moved = Move(direction.X, direction.Y, map, allCharacters);

                if (moved)
                {
                    if (_inventory.Count > countBefore)
                    {
                        _lastPickedItem = _inventory.GetLast();
                        HandlePickedUpItem();
                    }

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
                    ShowInventoryMenu(map);
                    break;
            }
        }

        return isPlaying;
    }

    private static Npc? FindNpcAt(List<Character> npcs, int x, int y)
    {
        foreach (Character c in npcs)
        {
            if (c is Npc npc && npc.IsAlive &&
                npc.GetPosition().X == x && npc.GetPosition().Y == y)
            {
                return npc;
            }
        }
        return null;
    }

    private void HandlePickedUpItem()
    {
        if (_lastPickedItem == null) return;

        if (_lastPickedItem.Effect == ItemEffect.Gold)
        {
            Gold += _lastPickedItem.Value;
            _inventory.Remove(_lastPickedItem);
            LastMessage = $"Podnosisz zloto! +{_lastPickedItem.Value} (lacznie: {Gold})";
            return;
        }

        LastMessage = _lastPickedItem.Effect switch
        {
            ItemEffect.Key => "Znalazlas klucz! Mozesz otworzyc nim zamkniete drzwi.",
            ItemEffect.Heal => "Podnosisz miksture leczaca zdrowie.",
            ItemEffect.QuestItem => "Podnosisz lancuszek ksiezniczki...",
            _ => $"Podnosisz: {_lastPickedItem.Name}"
        };
    }

    private void InteractWithNpc(Npc npc, Map map, List<Character> others)
    {
        if (npc.IsQueen)
        {
            HandleQueenInteraction(npc);
        }
        else if (npc.IsHostile)
        {
            Fight(npc, map, others);
        }
    }

    private void HandleQueenInteraction(Npc queen)
    {
        var questItem = _inventory.FindItemWithEffect(ItemEffect.QuestItem);

        if (queen.QuestCompleted)
        {
            ShowDialogue("Krolowa",
                "Jestes moja bohaterka. Nie zapomne tego nigdy...",
                "*Krolowa wyglada na szczesliwa i czule sie usmiecha.*");
            HasWon = true;
            return;
        }

        if (questItem != null)
        {
            _inventory.Remove(questItem);
            queen.QuestCompleted = true;

            ShowDialogue("Krolowa",
                "Moj lancuszek... Odnalazlas go!",
                "  Nie wiem jak ci dziekowac. Jestes dla mnie czyms wiecej",
                "  niz tylko wybawczynia... Mam nadzieje, ze to rozumiesz.",
                "  *Krolowa delikatnie sciska twoja dlon i patrzy ci w oczy.*");

            HasWon = true;
        }
        else
        {
            ShowDialogue("Krolowa",
                "Odnalazlas mnie! Ale... moj lancuszek...",
                "  Zgubilam go gdy mnie porwano. Czy moglabys go odzyskac?",
                "  To jedyna pamiatka po mojej matce...");

            LastMessage = "Krolowa prosi o odnalezienie lancuszka (&).";
        }
    }

    private void Fight(Npc npc, Map map, List<Character> others)
    {
        const int playerDamage = 4;
        const int npcDamage = 2;

        npc.TakeDamage(playerDamage);
        LastMessage = $"Atakujesz wroga zadajac {playerDamage} obrazen!";

        if (!npc.IsAlive)
        {
            others.Remove(npc);
            LastMessage += " Wrog pokonany!";

            Item? dropped = npc.DropQuestItem();
            if (dropped != null)
            {
                dropped.PlaceOnMap(map);
                LastMessage += " Upuscil lancuszek ksiezniczki (&)!";
            }
        }
        else
        {
            TakeDamage(npcDamage);
            LastMessage += $" Wrog odpowiada za {npcDamage} obrazen. HP: {HP}/{MaxHP}";
        }
    }

    private static void ShowDialogue(string speaker, params string[] lines)
    {
        Console.Clear();
        Console.WriteLine($"=== {speaker} ===");
        Console.WriteLine();

        foreach (string line in lines)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine();
        Console.WriteLine("Wcisnij dowolny klawisz, aby kontynuowac...");
        Console.ReadKey(true);
    }

    private void ShowInventoryMenu(Map map)
    {
        _inventory.Display();
        Console.WriteLine();
        Console.WriteLine("U = uzyj, G = wyrzuc, inny klawisz = wyjdz");

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
                    LastMessage = _inventory.UseItem(index, this);
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