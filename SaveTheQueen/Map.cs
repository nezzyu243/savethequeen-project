namespace SaveTheQueen;

public class Map
{
    private Cell[][] _cells = [];

    public void LoadFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path)
            .Where(line => line.Length > 0)
            .ToArray();

        if (lines.Length == 0)
        {
            throw new InvalidOperationException(
                $"Mapa '{path}' jest pusta.");
        }

        _cells = new Cell[lines.Length][];

        for (int rowIndex = 0;
             rowIndex < lines.Length;
             rowIndex++)
        {
            string line = lines[rowIndex];

            _cells[rowIndex] =
                new Cell[line.Length];

            for (int columnIndex = 0;
                 columnIndex < line.Length;
                 columnIndex++)
            {
                _cells[rowIndex][columnIndex] =
                    new Cell
                    {
                        Visuals = line[columnIndex]
                    };
            }
        }
    }

    public void Display()
    {
        Console.SetCursorPosition(0, 0);

        for (int y = 0;
             y < _cells.Length;
             y++)
        {
            for (int x = 0;
                 x < _cells[y].Length;
                 x++)
            {
                _cells[y][x].Display();
            }

            Console.WriteLine();
        }
    }

    public Cell GetCell(int x, int y)
    {
        return _cells[y][x];
    }

    public int GetHeight()
    {
        return _cells.Length;
    }

    public int GetRowWidth(int row)
    {
        return _cells[row].Length;
    }

    public List<Vector2> GetFloorPositions()
    {
        List<Vector2> positions = [];

        for (int y = 0;
             y < GetHeight();
             y++)
        {
            for (int x = 0;
                 x < GetRowWidth(y);
                 x++)
            {
                if (GetCell(x, y).Visuals == '.')
                {
                    positions.Add(
                        new Vector2(x, y));
                }
            }
        }

        return positions;
    }

    public Vector2? FindCellPosition(
        params char[] visuals)
    {
        for (int y = 0;
             y < GetHeight();
             y++)
        {
            for (int x = 0;
                 x < GetRowWidth(y);
                 x++)
            {
                if (visuals.Contains(
                        GetCell(x, y).Visuals))
                {
                    return new Vector2(x, y);
                }
            }
        }

        return null;
    }

    public Vector2 GetFloorPositionNear(
        params char[] visuals)
    {
        Vector2? targetPosition =
            FindCellPosition(visuals);

        if (targetPosition == null)
        {
            return GetRandomFloorPosition();
        }

        Vector2 target =
            targetPosition.Value;

        Vector2[] directions =
        [
            new Vector2(-1, 0),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(0, 1)
        ];

        foreach (Vector2 direction in directions)
        {
            int x = target.X + direction.X;
            int y = target.Y + direction.Y;

            if (y < 0 || y >= GetHeight())
            {
                continue;
            }

            if (x < 0 ||
                x >= GetRowWidth(y))
            {
                continue;
            }

            if (GetCell(x, y).Visuals == '.')
            {
                return new Vector2(x, y);
            }
        }

        return GetRandomFloorPosition();
    }

    public Vector2 GetRandomFloorPosition()
    {
        List<Vector2> floors =
            GetFloorPositions();

        if (floors.Count == 0)
        {
            throw new InvalidOperationException(
                "Mapa nie zawiera pol podlogi.");
        }

        return floors[
            Random.Shared.Next(floors.Count)];
    }
}