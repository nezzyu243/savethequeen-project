namespace SaveTheQueen;

public class Program
{
    public static void Main()
    {
        Console.Clear();
        Console.CursorVisible = false;

        ShowIntroduction();

        Dictionary<ConsoleKey, Vector2> inputMap =
            new()
            {
                {
                    ConsoleKey.W,
                    new Vector2(0, -1)
                },
                {
                    ConsoleKey.S,
                    new Vector2(0, 1)
                },
                {
                    ConsoleKey.A,
                    new Vector2(-1, 0)
                },
                {
                    ConsoleKey.D,
                    new Vector2(1, 0)
                }
            };

        string[] levelFiles =
        [
            "level1.txt",
            "level2.txt"
        ];

        /*
         * Każda mapa jest wczytywana i zapełniana przedmiotami
         * tylko raz podczas jednej rozgrywki.
         *
         * Po ponownym uruchomieniu gry przedmioty znowu zostaną
         * rozmieszczone w innych losowych miejscach.
         */
        Map[] levels =
            levelFiles
                .Select(LoadLevel)
                .ToArray();

        int currentLevel = 0;
        Map map = levels[currentLevel];

        Player player =
            new(
                '@',
                map.GetRandomFloorPosition(),
                map,
                inputMap);

        /*
         * NPC są również przechowywani osobno dla każdej mapy.
         * Dzięki temu pokonany goblin nie odrodzi się po powrocie.
         */
        List<Character>?[] charactersByLevel =
            new List<Character>?[levels.Length];

        List<Character> npcs =
            CreateCharactersForLevel(
                currentLevel,
                map,
                player);

        charactersByLevel[currentLevel] = npcs;

        bool isPlaying = true;

        while (isPlaying)
        {
            List<Character> allCharacters =
                [player];

            allCharacters.AddRange(npcs);

            Draw(
                map,
                player,
                npcs,
                currentLevel + 1);

            isPlaying =
                player.TakeTurn(
                    map,
                    npcs,
                    allCharacters);

            if (player.HP <= 0)
            {
                ShowGameOver();
                break;
            }

            if (player.HasWon)
            {
                ShowVictory();
                break;
            }

            if (!isPlaying)
            {
                break;
            }

            /*
             * Otwarte drzwi z serca prowadzą do korony.
             * Nie wczytujemy mapy ponownie.
             */
            if (currentLevel == 0 &&
                player.EnteredCastle)
            {
                currentLevel = 1;
                map = levels[currentLevel];

                player.SetMap(map);

                player.SetPosition(
                    map.GetFloorPositionNear('>'));

                npcs =
                    GetOrCreateCharactersForLevel(
                        charactersByLevel,
                        currentLevel,
                        map,
                        player);

                player.SetMessage(
                    "Wchodzisz do korony zamku.");
            }
            /*
             * Schody na planszy korony prowadzą z powrotem
             * do serca.
             */
            else if (currentLevel == 1 &&
                     player.ReachedStairs)
            {
                currentLevel = 0;
                map = levels[currentLevel];

                player.SetMap(map);

                player.SetPosition(
                    map.GetFloorPositionNear(
                        '(',
                        '/',
                        ')'));

                npcs =
                    GetOrCreateCharactersForLevel(
                        charactersByLevel,
                        currentLevel,
                        map,
                        player);

                player.SetMessage(
                    "Wracasz z korony do serca.");
            }

            foreach (Character npc in npcs.ToList())
            {
                if (!isPlaying)
                {
                    break;
                }

                List<Character> allForNpc =
                    [player];

                allForNpc.AddRange(npcs);

                npc.TakeTurn(
                    map,
                    allForNpc);
            }
        }

        Console.CursorVisible = true;
    }

    private static List<Character>
        GetOrCreateCharactersForLevel(
            List<Character>?[] charactersByLevel,
            int level,
            Map map,
            Player player)
    {
        List<Character>? existingCharacters =
            charactersByLevel[level];

        if (existingCharacters != null)
        {
            return existingCharacters;
        }

        List<Character> newCharacters =
            CreateCharactersForLevel(
                level,
                map,
                player);

        charactersByLevel[level] =
            newCharacters;

        return newCharacters;
    }

    private static void ShowIntroduction()
    {
        Console.Clear();

        Console.WriteLine("==============================");
        Console.WriteLine("       SAVE THE QUEEN");
        Console.WriteLine("==============================");
        Console.WriteLine();

        Console.WriteLine(
            "Krolowa zostala porwana i uwieziona");
        Console.WriteLine(
            "w najwyzszej czesci starego zamku.");

        Console.WriteLine();
        Console.WriteLine(
            "Aby do niej dotrzec, musisz odnalezc");
        Console.WriteLine(
            "klucz do zamknietych drzwi.");

        Console.WriteLine();
        Console.WriteLine(
            "W podziemiach czai sie goblin, ktory");
        Console.WriteLine(
            "ukradl cenny lancuszek Krolowej.");

        Console.WriteLine();
        Console.WriteLine(
            "Zbieraj zloto, korzystaj z pomocy kupca");
        Console.WriteLine(
            "i przygotuj sie do niebezpiecznej walki.");

        Console.WriteLine();
        Console.WriteLine("Sterowanie:");
        Console.WriteLine("WASD - ruch");
        Console.WriteLine("I - ekwipunek");
        Console.WriteLine("Q - zakonczenie gry");

        Console.WriteLine();
        Console.WriteLine(
            "Wcisnij dowolny klawisz, aby rozpoczac...");

        Console.ReadKey(true);
        Console.Clear();
    }

    private static Map LoadLevel(string levelFile)
    {
        Map map = new();

        map.LoadFromFile(levelFile);

        ItemSpawner.SpawnRandomItems(
            map,
            5);

        return map;
    }

    private static List<Character>
        CreateCharactersForLevel(
            int level,
            Map map,
            Player player)
    {
        List<Character> characters = [];

        if (level == 0)
        {
            Npc goblin =
                new(
                    avatar: 'g',
                    map: map,
                    isHostile: true,
                    maxHp: 20,
                    startingAttackDamage: 2,
                    damageIncrease: 2);

            PlaceCharacterAtFreePosition(
                goblin,
                map,
                player,
                characters);

            characters.Add(goblin);

            Npc merchant =
                new(
                    avatar: 'M',
                    map: map,
                    isHostile: false,
                    isMerchant: true);

            PlaceCharacterAtFreePosition(
                merchant,
                map,
                player,
                characters);

            characters.Add(merchant);
        }
        else
        {
            Npc queen =
                new(
                    avatar: 'Q',
                    map: map,
                    isHostile: false,
                    isQueen: true);

            PlaceCharacterAtFreePosition(
                queen,
                map,
                player,
                characters);

            characters.Add(queen);
        }

        return characters;
    }

    private static void PlaceCharacterAtFreePosition(
        Character character,
        Map map,
        Player player,
        List<Character> existingCharacters)
    {
        List<Vector2> availablePositions =
            map.GetFloorPositions()
                .Where(position =>
                    IsPositionFree(
                        position,
                        player,
                        existingCharacters))
                .ToList();

        if (availablePositions.Count == 0)
        {
            throw new InvalidOperationException(
                "Nie znaleziono wolnego miejsca dla NPC.");
        }

        character.SetPosition(
            availablePositions[
                Random.Shared.Next(
                    availablePositions.Count)]);
    }

    private static bool IsPositionFree(
        Vector2 position,
        Player player,
        List<Character> existingCharacters)
    {
        Vector2 playerPosition =
            player.GetPosition();

        if (position.X == playerPosition.X &&
            position.Y == playerPosition.Y)
        {
            return false;
        }

        foreach (Character character in existingCharacters)
        {
            Vector2 characterPosition =
                character.GetPosition();

            if (position.X == characterPosition.X &&
                position.Y == characterPosition.Y)
            {
                return false;
            }
        }

        return true;
    }

    private static void Draw(
        Map map,
        Player player,
        List<Character> npcs,
        int levelNumber)
    {
        Console.Clear();

        map.Display();
        player.Display();

        foreach (Character npc in npcs)
        {
            if (npc.IsAlive)
            {
                npc.Display();
            }
        }

        Console.SetCursorPosition(
            0,
            map.GetHeight());

        Console.WriteLine();

        Console.WriteLine(
            $"Poziom: {levelNumber}  " +
            $"HP: {player.HP}/{player.MaxHP}  " +
            $"Zloto: {player.Gold}");

        if (!string.IsNullOrWhiteSpace(
                player.LastMessage))
        {
            Console.WriteLine(
                player.LastMessage);
        }

        Console.WriteLine(
            "WASD - ruch | " +
            "I - ekwipunek | " +
            "Q - wyjscie");
    }

    private static void ShowGameOver()
    {
        Console.Clear();

        Console.WriteLine(
            "Goblin zadal ci smiertelny cios!");

        Console.WriteLine(
            "Zginelas w podziemiach. KONIEC GRY.");

        Console.WriteLine();
        Console.WriteLine(
            "Wcisnij dowolny klawisz...");

        Console.ReadKey(true);
    }

    private static void ShowVictory()
    {
        Console.Clear();

        Console.WriteLine(
            "Uratowalas Krolowa!");

        Console.WriteLine(
            "Lancuszek zostal odzyskany,");
        Console.WriteLine(
            "a Krolowa jest bezpieczna.");

        Console.WriteLine();
        Console.WriteLine("WYGRANA!");

        Console.WriteLine();
        Console.WriteLine(
            "Wcisnij dowolny klawisz...");

        Console.ReadKey(true);
    }
}