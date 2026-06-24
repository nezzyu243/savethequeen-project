namespace SaveTheQueen;

public static class ItemSpawner
{
    private static readonly Random Rng = new();

   public static List<Item> SpawnRandomItems(Map map, int count)
{
    List<Vector2> floorPositions = map.GetFloorPositions();
    List<Item> spawned = [];

    // 1. Gwarantowany klucz
    if (floorPositions.Count > 0)
    {
        int keyIndex = Rng.Next(floorPositions.Count);
        Vector2 keyPosition = floorPositions[keyIndex];
        floorPositions.RemoveAt(keyIndex);

        Item key = new Item('k', keyPosition, "Klucz", ItemEffect.Key, 0);
        key.PlaceOnMap(map);
        spawned.Add(key);
    }

    // 2. Gwarantowane złoto
    if (floorPositions.Count > 0)
    {
        int goldIndex = Rng.Next(floorPositions.Count);
        Vector2 goldPosition = floorPositions[goldIndex];
        floorPositions.RemoveAt(goldIndex);

        Item gold = new Item('*', goldPosition, "Zloto", ItemEffect.Gold, 10);
        gold.PlaceOnMap(map);
        spawned.Add(gold);
    }

    // 3. Reszta itemów tak jak było wcześniej
    for (int i = spawned.Count; i < count && floorPositions.Count > 0; i++)
    {
        int index = Rng.Next(floorPositions.Count);
        Vector2 position = floorPositions[index];
        floorPositions.RemoveAt(index);
        Item item = CreateRandomItem(position);

        if (item.Effect == ItemEffect.Key)
    {
     item = new Item('!', position, "Mikstura zdrowia", ItemEffect.Heal, 8);
     }

item.PlaceOnMap(map);
spawned.Add(item);
    }

    return spawned;
}
    private static Item CreateRandomItem(Vector2 position)
    {
        return Rng.Next(3) switch
        {
            0 => new Item('!', position, "Mikstura zdrowia", ItemEffect.Heal, 8),
            1 => new Item('*', position, "Zloto", ItemEffect.Gold, 10),
            _ => new Item('k', position, "Klucz", ItemEffect.Key, 0)
        };
    }
}