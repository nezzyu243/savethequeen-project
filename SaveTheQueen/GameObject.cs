namespace SaveTheQueen;

public abstract class GameObject
{
    protected Vector2 Position;
    protected char Avatar;

    protected GameObject(char avatar, Vector2 position)
    {
        Avatar = avatar;
        Position = position;
    }

    public char GetAvatar() => Avatar;

    public void Display()
    {
        Console.SetCursorPosition(Position.X, Position.Y);
        Console.Write(Avatar);
    }
}