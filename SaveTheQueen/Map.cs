namespace SaveTheQueen;

public class Map
{
    private Cell[][] _cells = [];

    public void LoadFromFile(string path)
    {
        string[] lines = File.ReadAllLines(path);
        _cells = new Cell[lines.Length][];

        for (var rowIndex = 0; rowIndex < lines.Length; rowIndex++)
        {
            string line = lines[rowIndex];
            _cells[rowIndex] = new Cell[line.Length];

            for (var columnIndex = 0; columnIndex < line.Length; columnIndex++)
            {
                _cells[rowIndex][columnIndex] = new Cell { Visuals = line[columnIndex] };
            }
        }
    }

    public void Display()
    {
        //Console.SetCursorPosition(0, 0);
        foreach (Cell[] row in _cells)
        {
            foreach (Cell cell in row)
            {
                cell.Display();
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
        for (int y = 0; y < GetHeight(); y++)
        {
            for (int x = 0; x < GetRowWidth(y); x++)
            {
                if (GetCell(x, y).Visuals == '.')
                {
                    positions.Add(new Vector2(x, y));
                }
            }
        }
        return positions;
    }
}