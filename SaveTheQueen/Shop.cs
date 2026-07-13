namespace SaveTheQueen;

public static class Shop
{
    private const int HealingPotionPrice = 10;
    private const int MaxHpUpgradePrice = 25;

    public static string Open(Player player)
    {
        while (true)
        {
            DrawMenu(player);

            ConsoleKey input = Console.ReadKey(true).Key;

            switch (input)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    ShowResult(BuyHealingPotion(player));
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    ShowResult(BuyMaxHpUpgrade(player));
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                case ConsoleKey.Escape:
                    return "Odchodzisz od kupca.";

                default:
                    ShowResult(
                        "Nieprawidlowy wybor. Wybierz 1, 2 albo 3.");
                    break;
            }
        }
    }

    private static void DrawMenu(Player player)
    {
        Console.Clear();

        Console.WriteLine("==============================");
        Console.WriteLine("            KUPIEC");
        Console.WriteLine("==============================");
        Console.WriteLine();

        Console.WriteLine(
            "\"Witaj, podrozniczko. Mam kilka rzeczy,");
        Console.WriteLine(
            "ktore moga uratowac ci zycie.\"");

        Console.WriteLine();
        Console.WriteLine($"Twoje zloto: {player.Gold}");
        Console.WriteLine($"Twoje HP: {player.HP}/{player.MaxHP}");
        Console.WriteLine();

        Console.WriteLine(
            $"1. Mikstura zdrowia (+8 HP) - " +
            $"{HealingPotionPrice} zlota");

        Console.WriteLine(
            $"2. Zwiekszenie maksymalnego HP (+5) - " +
            $"{MaxHpUpgradePrice} zlota");

        Console.WriteLine(
            "3. Wyjscie");

        Console.WriteLine();
        Console.WriteLine(
            "Wybierz opcje 1-3:");
    }

    private static string BuyHealingPotion(Player player)
    {
        if (player.Gold < HealingPotionPrice)
        {
            return "Nie masz wystarczajaco duzo zlota.";
        }

        Item healingPotion = new(
            '!',
            new Vector2(0, 0),
            "Mikstura zdrowia",
            ItemEffect.Heal,
            8);

        bool added = player.AddItem(healingPotion);

        if (!added)
        {
            return "Twoj ekwipunek jest pelny.";
        }

        player.TrySpendGold(HealingPotionPrice);

        return
            $"Kupujesz miksture zdrowia za " +
            $"{HealingPotionPrice} zlota.";
    }

    private static string BuyMaxHpUpgrade(Player player)
    {
        if (!player.TrySpendGold(MaxHpUpgradePrice))
        {
            return "Nie masz wystarczajaco duzo zlota.";
        }

        int previousMaxHp = player.MaxHP;

        player.IncreaseMaxHp(5);

        return
            $"Twoje maksymalne HP wzroslo " +
            $"z {previousMaxHp} do {player.MaxHP}. " +
            $"Zostajesz również w pelni uleczona.";
    }

    private static void ShowResult(string message)
    {
        Console.Clear();

        Console.WriteLine("==============================");
        Console.WriteLine("            KUPIEC");
        Console.WriteLine("==============================");
        Console.WriteLine();

        Console.WriteLine(message);

        Console.WriteLine();
        Console.WriteLine(
            "Wcisnij dowolny klawisz, aby kontynuowac...");

        Console.ReadKey(true);
    }
}