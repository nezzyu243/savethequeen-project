namespace SaveTheQueen;

public class Player : Character
{
    private enum BattleOutcome
    {
        PlayerAttack,
        PlayerDefense,
        EnemyAttack
    }

    private readonly Dictionary<ConsoleKey, Vector2> _inputMap;

    public string LastMessage { get; private set; } = "";
    public bool ReachedStairs { get; private set; }
    public bool EnteredCastle { get; private set; }
    public bool HasWon { get; private set; }

    public Player(
        char avatar,
        Vector2 startingPosition,
        Map map,
        Dictionary<ConsoleKey, Vector2> inputMap)
        : base(
            avatar,
            startingPosition,
            map,
            maxHp: 20)
    {
        _inputMap = inputMap;
    }

    public void SetMessage(string message)
    {
        LastMessage = message;
    }

    public override bool TakeTurn(
        Map map,
        List<Character> others)
    {
        return TakeTurn(map, others, null);
    }

    public bool TakeTurn(
        Map map,
        List<Character> npcs,
        List<Character>? allCharacters)
    {
        ReachedStairs = false;
        EnteredCastle = false;
        LastMessage = "";

        ConsoleKeyInfo input = Console.ReadKey(true);

        if (_inputMap.TryGetValue(
                input.Key,
                out Vector2 direction))
        {
            HandleMovement(
                direction,
                map,
                npcs,
                allCharacters);

            return true;
        }

        switch (input.Key)
        {
            case ConsoleKey.Q:
                return false;

            case ConsoleKey.I:
                ShowInventoryMenu(map);
                break;
        }

        return true;
    }

    private void HandleMovement(
        Vector2 direction,
        Map map,
        List<Character> npcs,
        List<Character>? allCharacters)
    {
        int targetX = _position.X + direction.X;
        int targetY = _position.Y + direction.Y;

        Npc? targetNpc = FindNpcAt(
            npcs,
            targetX,
            targetY);

        if (targetNpc != null)
        {
            InteractWithNpc(
                targetNpc,
                map,
                npcs);

            return;
        }

        bool targetWasLockedDoor = false;

        if (targetY >= 0 &&
            targetY < map.GetHeight() &&
            targetX >= 0 &&
            targetX < map.GetRowWidth(targetY))
        {
            targetWasLockedDoor = map
                .GetCell(targetX, targetY)
                .IsLockedDoor();
        }

        bool moved = Move(
            direction.X,
            direction.Y,
            map,
            allCharacters);

        if (!moved)
        {
            if (targetWasLockedDoor)
            {
                LastMessage =
                    "Drzwi do zamku sa zamkniete. Potrzebujesz klucza.";
            }

            return;
        }

        Cell currentCell = map.GetCell(
            _position.X,
            _position.Y);

        HandleItemOnCell(currentCell);

        if (currentCell.IsOpenCastleEntrance())
        {
            EnteredCastle = true;

            LastMessage =
                "Otwierasz drzwi i wchodzisz do zamku...";

            return;
        }

        if (currentCell.IsStairs())
        {
            ReachedStairs = true;

            LastMessage =
                "Schodzisz po schodach...";
        }
    }

    private void HandleItemOnCell(Cell cell)
    {
        if (!cell.HasItem())
        {
            return;
        }

        Item? item = cell.Item;

        if (item == null)
        {
            return;
        }

        if (item.Effect == ItemEffect.Gold)
        {
            Item? collectedGold = cell.TakeItem();

            if (collectedGold == null)
            {
                return;
            }

            Gold += collectedGold.Value;

            LastMessage =
                $"Podnosisz zloto! +{collectedGold.Value}. " +
                $"Masz teraz {Gold} zlota.";

            return;
        }

        if (_inventory.IsFull)
        {
            LastMessage =
                $"Ekwipunek jest pelny. " +
                $"Nie mozesz podniesc: {item.Name}.";

            return;
        }

        Item? collectedItem = cell.TakeItem();

        if (collectedItem == null)
        {
            return;
        }

        bool added = _inventory.Add(collectedItem);

        if (!added)
        {
            cell.PutItem(collectedItem);

            LastMessage =
                $"Nie udalo sie podniesc: " +
                $"{collectedItem.Name}.";

            return;
        }

        LastMessage = collectedItem.Effect switch
        {
            ItemEffect.Key =>
                "Znalazlas klucz do zamku!",

            ItemEffect.Heal =>
                "Podnosisz miksture leczaca.",

            ItemEffect.QuestItem =>
                "Podnosisz lancuszek ksiezniczki...",

            _ =>
                $"Podnosisz: {collectedItem.Name}"
        };
    }

    private static Npc? FindNpcAt(
        List<Character> npcs,
        int x,
        int y)
    {
        foreach (Character character in npcs)
        {
            if (character is not Npc npc ||
                !npc.IsAlive)
            {
                continue;
            }

            Vector2 npcPosition = npc.GetPosition();

            if (npcPosition.X == x &&
                npcPosition.Y == y)
            {
                return npc;
            }
        }

        return null;
    }

    private void InteractWithNpc(
        Npc npc,
        Map map,
        List<Character> others)
    {
        if (npc.IsMerchant)
        {
            LastMessage = Shop.Open(this);
            return;
        }

        if (npc.IsQueen)
        {
            HandleQueenInteraction(npc);
            return;
        }

        if (npc.IsHostile)
        {
            PlayBattleRound(
                npc,
                map,
                others);
        }
    }

    private void PlayBattleRound(
        Npc npc,
        Map map,
        List<Character> others)
    {
        BattleOutcome[] outcomes =
        [
            BattleOutcome.PlayerAttack,
            BattleOutcome.PlayerDefense,
            BattleOutcome.EnemyAttack
        ];

        ShuffleOutcomes(outcomes);

        Console.Clear();

        Console.WriteLine("==============================");
        Console.WriteLine("       WALKA Z GOBLINEM");
        Console.WriteLine("==============================");
        Console.WriteLine();

        Console.WriteLine($"Twoje HP:   {HP}/{MaxHP}");
        Console.WriteLine($"HP goblina: {npc.HP}/{npc.MaxHP}");

        Console.WriteLine(
            $"Sila kolejnego ataku goblina: " +
            $"{npc.GetCurrentAttackDamage()}");

        Console.WriteLine();
        Console.WriteLine("Jedna liczba oznacza ATAK.");
        Console.WriteLine("Jedna liczba oznacza OBRONE.");
        Console.WriteLine("Jedna liczba oznacza OBRAZENIA.");
        Console.WriteLine();
        Console.WriteLine("Wybierz liczbe od 1 do 3:");

        int selectedNumber = ReadBattleChoice();

        BattleOutcome selectedOutcome =
            outcomes[selectedNumber - 1];

        Console.WriteLine();

        switch (selectedOutcome)
        {
            case BattleOutcome.PlayerAttack:
                HandlePlayerAttack(
                    npc,
                    map,
                    others,
                    selectedNumber);
                break;

            case BattleOutcome.PlayerDefense:
                HandlePlayerDefense(selectedNumber);
                break;

            case BattleOutcome.EnemyAttack:
                HandleEnemyAttack(
                    npc,
                    selectedNumber);
                break;
        }

        Console.WriteLine();
        Console.WriteLine(
            "Wcisnij dowolny klawisz, aby kontynuowac...");

        Console.ReadKey(true);
    }

    private static void ShuffleOutcomes(
        BattleOutcome[] outcomes)
    {
        for (int i = outcomes.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Shared.Next(i + 1);

            (outcomes[i], outcomes[randomIndex]) =
                (outcomes[randomIndex], outcomes[i]);
        }
    }

    private static int ReadBattleChoice()
    {
        while (true)
        {
            ConsoleKey input = Console.ReadKey(true).Key;

            switch (input)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine("1");
                    return 1;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.WriteLine("2");
                    return 2;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Console.WriteLine("3");
                    return 3;

                default:
                    Console.WriteLine(
                        "Wybierz tylko 1, 2 albo 3.");
                    break;
            }
        }
    }

    private void HandlePlayerAttack(
        Npc npc,
        Map map,
        List<Character> others,
        int selectedNumber)
    {
        const int playerDamage = 4;

        npc.TakeDamage(playerDamage);

        Console.WriteLine(
            $"{selectedNumber} oznaczalo ATAK!");

        Console.WriteLine(
            $"Zadajesz goblinowi {playerDamage} obrazen.");

        Console.WriteLine(
            $"HP goblina: {npc.HP}/{npc.MaxHP}");

        LastMessage =
            $"Atakujesz goblina za {playerDamage} obrazen.";

        if (npc.IsAlive)
        {
            return;
        }

        others.Remove(npc);

        Console.WriteLine();
        Console.WriteLine("Goblin zostal pokonany!");

        LastMessage = "Goblin zostal pokonany!";

        Item? droppedItem = npc.DropQuestItem();

        if (droppedItem == null)
        {
            return;
        }

        droppedItem.PlaceOnMap(map);

        Console.WriteLine(
            "Goblin upuscil lancuszek ksiezniczki (&)!");

        LastMessage +=
            " Upuscil lancuszek ksiezniczki (&)!";
    }

    private void HandlePlayerDefense(int selectedNumber)
    {
        Console.WriteLine(
            $"{selectedNumber} oznaczalo OBRONE!");

        Console.WriteLine(
            "Blokujesz atak goblina.");

        Console.WriteLine(
            "Nie tracisz punktow zdrowia.");

        LastMessage =
            "Bronisz sie przed atakiem goblina.";
    }

    private void HandleEnemyAttack(
        Npc npc,
        int selectedNumber)
    {
        int enemyDamage = npc.PerformAttack();

        TakeDamage(enemyDamage);

        Console.WriteLine(
            $"{selectedNumber} oznaczalo OBRAZENIA!");

        Console.WriteLine(
            $"Goblin zadaje ci {enemyDamage} obrazen.");

        Console.WriteLine(
            $"Twoje HP: {HP}/{MaxHP}");

        LastMessage =
            $"Goblin zadaje ci {enemyDamage} obrazen. " +
            $"HP: {HP}/{MaxHP}.";

        if (!IsAlive)
        {
            Console.WriteLine();
            Console.WriteLine(
                "Goblin zadal ci smiertelny cios!");

            return;
        }

        Console.WriteLine(
            $"Nastepny atak goblina zada " +
            $"{npc.GetCurrentAttackDamage()} obrazen.");

        LastMessage +=
            $" Nastepny atak zada " +
            $"{npc.GetCurrentAttackDamage()} obrazen.";
    }

    private void HandleQueenInteraction(Npc queen)
    {
        Item? questItem =
            _inventory.FindItemWithEffect(
                ItemEffect.QuestItem);

        if (queen.QuestCompleted)
        {
            ShowDialogue(
                "Krolowa",
                "Jestes moja bohaterka.",
                "Nie zapomne tego nigdy...");

            HasWon = true;
            return;
        }

        if (questItem != null)
        {
            _inventory.Remove(questItem);
            queen.QuestCompleted = true;

            ShowDialogue(
                "Krolowa",
                "Moj lancuszek... Odnalazlas go!",
                "Nie wiem, jak ci dziekowac.",
                "Jestes dla mnie czyms wiecej",
                "niz tylko wybawczynia...");

            HasWon = true;
            return;
        }

        ShowDialogue(
            "Krolowa",
            "Odnalazlas mnie!",
            "Ale zgubilam moj lancuszek...",
            "Czy moglabys go odzyskac?");

        LastMessage =
            "Krolowa prosi o odnalezienie lancuszka (&).";
    }

    private static void ShowDialogue(
        string speaker,
        params string[] lines)
    {
        Console.Clear();
        Console.WriteLine($"=== {speaker} ===");
        Console.WriteLine();

        foreach (string line in lines)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine();
        Console.WriteLine(
            "Wcisnij dowolny klawisz, aby kontynuowac...");

        Console.ReadKey(true);
    }

    private void ShowInventoryMenu(Map map)
    {
        _inventory.Display();

        Console.WriteLine();
        Console.WriteLine(
            "U = uzyj, G = wyrzuc, " +
            "inny klawisz = wyjdz");

        ConsoleKey action = Console.ReadKey(true).Key;

        if (action != ConsoleKey.U &&
            action != ConsoleKey.G)
        {
            _inventory.Hide();
            return;
        }

        Console.WriteLine(
            "Wcisnij cyfre przedmiotu (1-9):");

        char numberKey = Console.ReadKey(true).KeyChar;

        if (!char.IsDigit(numberKey) ||
            numberKey == '0')
        {
            LastMessage =
                "Nieprawidlowy numer przedmiotu.";

            _inventory.Hide();
            return;
        }

        int index = numberKey - '1';

        if (action == ConsoleKey.U)
        {
            LastMessage =
                _inventory.UseItem(index, this);
        }
        else
        {
            DropInventoryItem(index, map);
        }

        _inventory.Hide();
    }

    private void DropInventoryItem(
        int index,
        Map map)
    {
        Cell currentCell = map.GetCell(
            _position.X,
            _position.Y);

        if (currentCell.HasItem())
        {
            LastMessage =
                "Na tym polu lezy juz inny przedmiot.";

            return;
        }

        Item? droppedItem =
            _inventory.RemoveAt(index);

        if (droppedItem == null)
        {
            LastMessage =
                "Nie ma takiego przedmiotu.";

            return;
        }

        currentCell.PutItem(droppedItem);

        LastMessage =
            $"Wyrzucasz na ziemie: {droppedItem.Name}.";
    }
}