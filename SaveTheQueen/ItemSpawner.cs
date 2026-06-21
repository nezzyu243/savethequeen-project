namespace SaveTheQueen;

public static class ItemSpawner
{
    private static readonly Random Rng = new();

    public static List<Item> SpawnRandomItems(Map map, int count)
    {
        List<Vector2> floorPositions = map.GetFloorPositions();
        List<Item> spawned = [];

        for (int i = 0; i < count && floorPositions.Count > 0; i++)
        {
            int index = Rng.Next(floorPositions.Count);
            Vector2 position = floorPositions[index];
            floorPositions.RemoveAt(index); 

            Item item = CreateRandomItem(position);
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