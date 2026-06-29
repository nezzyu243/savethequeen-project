namespace SaveTheQueen;

public abstract class GameObject
{
    protected Vector2 _position;
    protected char Avatar;

    protected GameObject(char avatar, Vector2 position)
    {
        Avatar = avatar;
        _position = position;
    }

    public char GetAvatar() => Avatar;

    public void Display()
   {
    if (_position.X < 0 || _position.Y < 0) return;

    Console.SetCursorPosition(_position.X, _position.Y);
    Console.Write(Avatar);
   }
}